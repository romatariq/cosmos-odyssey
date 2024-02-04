using System.Text.Json;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace WebApp;

public class ApiBackgroundService: BackgroundService
{
    private readonly ILogger<ApiBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ApiBackgroundService(ILogger<ApiBackgroundService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var fetchAgainIn = await DoWork();
        _logger.LogInformation("Fetching again in {fetchAgainIn} seconds", fetchAgainIn);

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(fetchAgainIn));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                fetchAgainIn = await DoWork();
                timer.Period = TimeSpan.FromSeconds(fetchAgainIn);
                _logger.LogInformation("Fetching again in {fetchAgainIn} seconds", fetchAgainIn);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");
        }
    }

    private async Task<double> DoWork()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        
        var latestTravelPrice = await context.TravelPrices
            .OrderByDescending(x => x.ValidUntil)
            .FirstOrDefaultAsync();
        
        if (latestTravelPrice?.ValidUntil > DateTime.UtcNow)
        {
            return latestTravelPrice.ValidUntil.Subtract(DateTime.UtcNow).TotalSeconds;
        }
        
        using var client = new HttpClient();
        _logger.LogInformation("Fetching travel prices from external API");
        var result = await client.GetStringAsync("https://cosmos-odyssey.azurewebsites.net/api/v1.0/TravelPrices");

        var travelPrice = JsonSerializer.Deserialize<TravelPrice>(result, _jsonSerializerOptions);

        if (travelPrice == null || travelPrice.Id == latestTravelPrice?.Id)
        {
            var message = latestTravelPrice?.Id != null ? 
                "Price is already in the database. Miscalculated time." : "Failed to fetch from api.";
            _logger.LogError("Fetched travel price is invalid. {message}", message);
            return 10;
        }

        try
        {
            var companies = GetUniqueCompanies(travelPrice);
            await context.Companies.AddRangeAsync(companies);

            ParseTravelPriceToMatchDomain(travelPrice);
            await context.TravelPrices.AddAsync(travelPrice);
            
            await DeleteOldTravelPrices(context);
            
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to save travel price to db.");
            return 10;
        }
        
        return travelPrice.ValidUntil.Subtract(DateTime.UtcNow).TotalSeconds;
    }
    
    
    private static List<Company> GetUniqueCompanies(TravelPrice travelPrice)
    {
        return travelPrice.Legs!
            .SelectMany(l => l.Providers!)
            .Select(p =>
            {
                p.Company!.TravelPriceId = travelPrice.Id;
                return p.Company;
            })
            .GroupBy(c => c.Id)
            .Select(c => c.First())
            .ToList();
    }
    
    private static Task<int> DeleteOldTravelPrices(AppDbContext context)
    {
        return context.TravelPrices
            .OrderByDescending(tp => tp.ValidUntil)
            .Skip(15)
            .ExecuteDeleteAsync();
    }
    

    private static void ParseTravelPriceToMatchDomain(TravelPrice travelPrice)
    {
        foreach (var leg in travelPrice.Legs?? [])
        {
            leg.TravelPriceId = travelPrice.Id;
            
            leg.RouteInfoId = leg.RouteInfo!.Id;
            leg.RouteInfo.FromId = leg.RouteInfo.From!.Id;
            leg.RouteInfo.ToId = leg.RouteInfo.To!.Id;
            leg.RouteInfo.From.TravelPriceId = travelPrice.Id;
            leg.RouteInfo.To.TravelPriceId = travelPrice.Id;
            
            foreach (var legProvider in leg.Providers?? [])
            {
                legProvider.LegId = leg.Id;
                legProvider.CompanyId = legProvider.Company!.Id;
                legProvider.Company = null;
            }
        }
    }
}
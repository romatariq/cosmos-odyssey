using BLL;
using DAL;
using Helpers;

namespace WebApp;

public class ApiBackgroundService: BackgroundService
{
    private readonly ILogger<ApiBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ApiBackgroundService(ILogger<ApiBackgroundService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
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
        var uow = new UOW(context);

        var latestTravelPrice = await uow.ApiService.GetLatestTravelPrice();
        if (latestTravelPrice?.ValidUntil > DateTime.UtcNow)
        {
            return latestTravelPrice.ValidUntil.GetTimeDifferenceFromNowInSecondsWithSpare();
        }
        
        _logger.LogInformation("Fetching travel prices from external API");
        var travelPrice = await uow.ApiService.FetchTravelPriceFromApi();
        if (travelPrice == null || travelPrice.Id == latestTravelPrice?.Id)
        {
            var message = latestTravelPrice?.Id != null ? 
                "Price is already in the database. Miscalculated time." : "Failed to fetch from api.";
            _logger.LogError("Fetched travel price is invalid. {message}", message);
            return 10;
        }

        try
        {
            await uow.ApiService.ParseAndSaveTravelPrice(travelPrice);
            await uow.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to save travel price to db.");
            return 10;
        }
        
        return travelPrice.ValidUntil.GetTimeDifferenceFromNowInSecondsWithSpare();
    }
}
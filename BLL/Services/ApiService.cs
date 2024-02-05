using System.Text.Json;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class ApiService
{
    private readonly AppDbContext _context;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ApiService(AppDbContext context)
    {
        _context = context;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
    }
    
    
    public async Task<TravelPrice?> FetchTravelPriceFromApi()
    {
        using var client = new HttpClient();
        var result = await client.GetStringAsync("https://cosmos-odyssey.azurewebsites.net/api/v1.0/TravelPrices");
        return JsonSerializer.Deserialize<TravelPrice>(result, _jsonSerializerOptions);
    }
    
    public async Task<TravelPrice?> GetLatestTravelPrice()
    {
        return await _context.TravelPrices
            .OrderByDescending(x => x.ValidUntil)
            .FirstOrDefaultAsync();
    }
    
    public async Task ParseAndSaveTravelPrice(TravelPrice travelPrice)
    {
        var companies = GetUniqueCompanies(travelPrice);
        await _context.Companies.AddRangeAsync(companies);

        ParseTravelPriceToMatchDomain(travelPrice);
        await _context.TravelPrices.AddAsync(travelPrice);
            
        await DeleteOldTravelPrices(_context);
            
        await _context.SaveChangesAsync();
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
using System.Text.Json;
using DAL;
using Domain;
using DTO.BLL;
using Helpers.Constants;
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
            
        var flights = MapProviderToDomainFlights(travelPrice, companies);
        await _context.Flights.AddRangeAsync(flights);
        
        var validTrips = GetAllPossibleTrips(travelPrice, companies);
        await _context.Trips.AddRangeAsync(validTrips);
            
        await DeleteOldTravelPrices(_context);
    }


    private List<Domain.OptimisedSchema.Trip> GetAllPossibleTrips(TravelPrice travelPrice, List<Company> companies)
    {
        var allRoutes = MapTravelPriceToRoutes(travelPrice, companies);
        
        var allValidTrips = new List<Domain.OptimisedSchema.Trip>();

        foreach (var from in Planets.AllPlanets)
        {
            foreach (var to in Planets.AllPlanets)
            {
                if (from == to) continue;
                
                var validRoutes = RouteCalculator.GetAllPossibleTrips(from, to, allRoutes);
                var trips = validRoutes.Select(trip => new Domain.OptimisedSchema.Trip()
                    {
                        From = trip.First().From,
                        To = trip.Last().To,
                        Departure = trip.First().Departure,
                        Arrival = trip.Last().Arrival,
                        Price = trip.Sum(f => f.Price),
                        Distance = trip.Sum(f => f.Distance),
                        TravelPriceId = travelPrice.Id,
                        TripFlights = trip.Select(f => new Domain.OptimisedSchema.TripFlight()
                        {
                            FlightId = f.ProviderId
                        }).ToList()
                    }).ToList();
                allValidTrips.AddRange(trips);
            }
        }

        return allValidTrips;
    }

    private IEnumerable<Domain.OptimisedSchema.Flight> MapProviderToDomainFlights(TravelPrice travelPrice,
        List<Company> companies)
    {
        var companiesMap = GetCompanyIdNameMap(companies);
        return travelPrice.Legs!
            .SelectMany(l => l.Providers!
            .Select(p => new Domain.OptimisedSchema.Flight()
            {
                Id = p.Id,
                From = l.RouteInfo!.From!.Name,
                To = l.RouteInfo!.To!.Name,
                Departure = p.FlightStart,
                Arrival = p.FlightEnd,
                Distance = l.RouteInfo!.Distance,
                Price = p.Price,
                Company = companiesMap[p.CompanyId],
                ProviderId = p.Id
            })
            .ToList());
    }


    private Dictionary<Guid, string> GetCompanyIdNameMap(List<Company> companies)
    {
        return companies.ToDictionary(c => c.Id, c => c.Name);
    }

    private List<Route> MapTravelPriceToRoutes(TravelPrice travelPrice, List<Company> companies)
    {
        var companiesMap = GetCompanyIdNameMap(companies);
        
        return travelPrice
            .Legs!
            .Select(l => new Route()
            {
                From = l.RouteInfo!.From!.Name,
                To = l.RouteInfo!.To!.Name,
                Distance = l.RouteInfo!.Distance,
                Flights = l.Providers!.Select(p => new Flight()
                {
                    ProviderId = p.Id,
                    Company = companiesMap[p.CompanyId],
                    Price = p.Price,
                    Departure = p.FlightStart,
                    Arrival = p.FlightEnd,
                    From = l.RouteInfo!.From!.Name,
                    To = l.RouteInfo!.To!.Name,
                    Distance = l.RouteInfo!.Distance,
                }).ToList()
            }).ToList();
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
            .Skip(14)
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
using DAL;
using DTO.BLL;
using Helpers.Constants;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class RouteService
{
    private readonly AppDbContext _context;

    public RouteService(AppDbContext context)
    {
        _context = context;
    }


    public async Task<List<Trip>> GetAllTrips(EPlanet from, EPlanet to, string? filter)
    {
        if (from == to) return [];

        var fromStr = Planets.AllPlanets[(int)from];
        var toStr = Planets.AllPlanets[(int)to];

        var routes = await GetAllRoutes(filter);
        var validRoutes = RouteCalculator.GetAllPossibleTrips(fromStr, toStr, routes.ToList());

        return validRoutes.Select(route => new Trip()
        {
            From = route.First().Departure,
            To = route.Last().Arrival,
            Price = route.Sum(f => f.Price),
            Distance = route.Sum(f => f.Distance),
            Flights = route,
            Companies = route.Select(f => f.Company).Distinct().ToList()
        }).ToList();
    }

    private async Task<IEnumerable<Route>> GetAllRoutes(string? filter)
    {
        var routes = await _context.TravelPrices
            .Include(tp => tp.Legs!)
            .ThenInclude(l => l.RouteInfo!.From)
            .Include(tp => tp.Legs!)
            .ThenInclude(l => l.RouteInfo!.To)
            .Include(tp => tp.Legs!)
            .ThenInclude(l => l.Providers!)
            .ThenInclude(p => p.Company)
            .Where(tp => tp.ValidUntil > DateTime.UtcNow)
            .Select(tp => tp.Legs!
                .Select(l => new Route()
                {
                    From = l.RouteInfo!.From!.Name,
                    To = l.RouteInfo!.To!.Name,
                    Distance = l.RouteInfo!.Distance,
                    Flights = l.Providers!
                        .Where(p => filter == null ||
                                    p.Company!.Name.ToLower().Contains(filter.ToLower()))
                        .Select(p => new Flight()
                        {
                            ProviderId = p.Id,
                            Company = p.Company!.Name,
                            Price = p.Price,
                            Departure = p.FlightStart,
                            Arrival = p.FlightEnd,
                            From = l.RouteInfo!.From!.Name,
                            To = l.RouteInfo!.To!.Name,
                            Distance = l.RouteInfo!.Distance,
                        }).ToList()
                }))
            .SingleOrDefaultAsync();

        return routes ?? [];
    }
}
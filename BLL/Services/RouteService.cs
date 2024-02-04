using DAL;
using DTO.BLL;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class RouteService
{
    private readonly AppDbContext _context;

    public RouteService(AppDbContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<Route>> GetAllRoutes()
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
            .Select(tp => tp.Legs!.Select(l => new Route()
            {
                From = l.RouteInfo!.From!.Name,
                To = l.RouteInfo!.To!.Name,
                Distance = l.RouteInfo!.Distance,
                Flights = l.Providers!.Select(p => new Flight()
                {
                    ProviderId = p.Id,
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
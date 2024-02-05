using DAL;
using Helpers;
using Helpers.Constants;
using Microsoft.EntityFrameworkCore;
using Domain.OptimisedSchema;

namespace BLL.Services;

public class RouteService
{
    private readonly AppDbContext _context;

    public RouteService(AppDbContext context)
    {
        _context = context;
    }


    public async Task<(List<DTO.Public.Trip>, int)> GetAllTrips(EPlanet from, EPlanet to, ESortBy sortBy, string? filter, int pageNr, int pageSize)
    {
        if (from == to) return ([], 1);

        var fromStr = Planets.AllPlanets[(int)from];
        var toStr = Planets.AllPlanets[(int)to];
        filter = filter?.Trim().ToLower();

        var query = _context.Trips
            .Include(t => t.TravelPrice)
            .Include(t => t.TripFlights!)
            .ThenInclude(tp => tp.Flight)
            .Where(t => 
                t.From == fromStr &&
                t.To == toStr &&
                t.TravelPrice!.ValidUntil > DateTime.UtcNow &&
                t.TripFlights!.Any(f => filter == null || f.Flight!.Company.ToLower().Contains(filter)));

        query = OrderBy(query, sortBy);
            
        var pageCount = EfHelpers.GetPageCount(await query.CountAsync(), pageSize); 
            
        var items = await query
            .Paging(pageNr, pageSize)
            .Select(t => new DTO.Public.Trip()
            {
                Id = t.Id,
                Departure = t.Departure,
                Arrival = t.Arrival,
                Price = t.Price,
                Distance = t.Distance,
                Flights = t.TripFlights!
                    .Select(f => f.Flight!)
                    .OrderBy(f => f.Departure)
                    .Select(f => new DTO.Public.Flight
                    {
                        Id = f.Id,
                        From = f.From,
                        To = f.To,
                        Departure = f.Departure,
                        Arrival = f.Arrival,
                        Distance = f.Distance,
                        Price = f.Price,
                        Company = f.Company
                    }).ToList(),
            }).ToListAsync();

        return (items, pageCount);
    }


    private static  IOrderedQueryable<Trip> OrderBy(IQueryable<Trip> query, ESortBy sort)
    {
        return sort switch
        {
            ESortBy.PriceAsc => query.OrderBy(t => t.Price),
            ESortBy.PriceDesc => query.OrderByDescending(t => t.Price),
            ESortBy.DistanceAsc => query.OrderBy(t => t.Distance),
            ESortBy.DistanceDesc => query.OrderByDescending(t => t.Distance),
            ESortBy.DepartureAsc => query.OrderBy(t => t.Departure),
            ESortBy.DepartureDesc => query.OrderByDescending(t => t.Departure),
            ESortBy.ArrivalAsc => query.OrderBy(t => t.Arrival),
            ESortBy.ArrivalDesc => query.OrderByDescending(t => t.Arrival),
            ESortBy.TravelTimeAsc => query.OrderBy(t => t.Arrival - t.Departure),
            ESortBy.TravelTimeDesc => query.OrderByDescending(t => t.Arrival - t.Departure),
            _ => throw new ArgumentOutOfRangeException(nameof(sort), sort, null)
        };
    }

}
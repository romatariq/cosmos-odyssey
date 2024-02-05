using DAL;
using DTO.Public;
using Helpers;
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


    public async Task<(List<Trip>, int)> GetAllTrips(EPlanet from, EPlanet to, string? filter, int pageNr, int pageSize)
    {
        if (from == to) return ([], 1);

        var fromStr = Planets.AllPlanets[(int)from];
        var toStr = Planets.AllPlanets[(int)to];
        filter = filter?.Trim().ToLower();

        var query = _context.Trips
            .Include(t => t.TravelPrice)
            .Include(t => t.TripFlights!)
            .ThenInclude(tp => tp.Flight)
            .OrderBy(t => t.From)
            .Where(t => 
                t.From == fromStr &&
                t.To == toStr &&
                t.TravelPrice!.ValidUntil > DateTime.UtcNow &&
                t.TripFlights!.Any(f => filter == null || f.Flight!.Company.ToLower().Contains(filter)))
            .AsQueryable();
            
        var pageCount = EfHelpers.GetPageCount(await query.CountAsync(), pageSize); 
            
        var items = await query
            .Paging(pageNr, pageSize)
            .Select(t => new
            {
                flights = t.TripFlights!.Select(f => f.Flight!).OrderBy(f => f.Departure).ToList(),
                from = t.From,
                to = t,
                id = t.Id
            })
            .Select(t => new Trip()
            {
                Id = t.id,
                Departure = t.flights.First().Departure,
                Arrival = t.flights.Last().Arrival,
                Price = t.flights.Sum(f => f.Price),
                Distance = t.flights.Sum(f => f.Distance),
                Flights = t.flights.Select(f => new Flight()
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

}
using DAL;
using Domain;
using DTO.Public;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class ReservationService
{
    private readonly AppDbContext _context;

    public ReservationService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task CreateReservation(Reservation reservation)
    {
        await _context.Reservations.AddAsync(reservation);
    }
    
    public async Task<Trip?> GetTrip(Guid tripId)
    {
        return await _context.Trips
            .Include(t => t.TravelPrice)
            .Include(t => t.TripFlights!)
            .ThenInclude(tf => tf.Flight)
            .Where(t => t.Id == tripId)
            .Select(t => new Trip()
            {
                Id = t.Id,
                Departure = t.Departure,
                Arrival = t.Arrival,
                Price = t.Price,
                Distance = t.Distance,
                Flights = t.TripFlights!
                    .Select(tf => tf.Flight!)
                    .Select(f => new Flight()
                    {
                        Id = f.Id,
                        Departure = f.Departure,
                        Arrival = f.Arrival,
                        Distance = f.Distance,
                        From = f.From,
                        To = f.To,
                        Company = f.Company,
                        Price = f.Price
                    }).OrderBy(f => f.Departure).ToList()
            })
            .FirstOrDefaultAsync();
    }
    
    public async Task<bool> TripStillReservable(Reservation reservation)
    {
        return await _context.Trips
            .Include(t => t.TravelPrice)
            .Where(t => t.Id == reservation.TripId && t.TravelPrice!.ValidUntil > DateTime.UtcNow)
            .AnyAsync();
    }
}
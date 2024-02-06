using DAL;
using Domain;
using DTO.Public;
using Microsoft.EntityFrameworkCore;
using Reservation = DTO.Public.Reservation;

namespace BLL.Services;

public class ReservationService
{
    private readonly AppDbContext _context;

    public ReservationService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task CreateReservation(Domain.Reservation reservation)
    {
        await _context.Reservations.AddAsync(reservation);
    }
    
    public async Task<ReservationDetails?> GetReservation(Guid id)
    {
        return await _context.Reservations
            .Where(r => r.Id == id)
            .Select(r => new ReservationDetails()
            {
                LastName = r.LastName,
                FirstName = r.FirstName,
                Id = r.Id,
                TripId = r.TripId
            }).SingleOrDefaultAsync();
    }
    
    public async Task<List<Reservation>> GetReservations(Guid userId)
    {
        return await _context.Reservations
            .Include(r => r.Trip!)
            .ThenInclude(t => t.TripFlights!)
            .ThenInclude(tf => tf.Flight!)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.Trip!.Departure)
            .Select(r => new Reservation()
            {
                Id = r.Id,
                Departure = r.Trip!.Departure,
                Route = string.Join(" - ", r.Trip.TripFlights!
                    .Select(tf => tf.Flight!)
                    .OrderBy(tf => tf.Departure)
                    .Select(f => f.From)
                    .ToList()) + $" - {r.Trip.To}",
                FirstName = r.FirstName,
                LastName = r.LastName
            }).ToListAsync();
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
                ReservableTill = t.TravelPrice!.ValidUntil,
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
    
    public async Task<bool> TripStillReservable(Guid tripId)
    {
        return await _context.Trips
            .Include(t => t.TravelPrice)
            .Where(t => t.Id == tripId && t.TravelPrice!.ValidUntil > DateTime.UtcNow)
            .AnyAsync();
    }
}
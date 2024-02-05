using DAL;
using Domain;
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
    
    public async Task<bool> TripStillReservable(Reservation reservation)
    {
        return await _context.Trips
            .Include(t => t.TravelPrice)
            .Where(t => t.Id == reservation.TripId && t.TravelPrice!.ValidUntil > DateTime.UtcNow)
            .AnyAsync();
    }
}
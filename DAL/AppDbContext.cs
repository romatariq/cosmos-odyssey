using Domain;
using Domain.Identity;
using Domain.OptimisedSchema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext: IdentityDbContext<AppUser, AppRole, Guid>
{
    
    public DbSet<AppUser> AppUsers { get; set; } = default!;
    public DbSet<AppRole> AppRoles { get; set; } = default!;
    
    public DbSet<Company> Companies { get; set; } = default!;
    public DbSet<Leg> Legs { get; set; } = default!;
    public DbSet<Location> Locations { get; set; } = default!;
    public DbSet<Provider> Providers { get; set; } = default!;
    public DbSet<RouteInfo> RouteInfos { get; set; } = default!;
    public DbSet<TravelPrice> TravelPrices { get; set; } = default!;

    public DbSet<Reservation> Reservations { get; set; } = default!;
    
    public DbSet<Trip> Trips { get; set; } = default!;
    public DbSet<Flight> Flights { get; set; } = default!;
    public DbSet<TripFlight> TripFlights { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
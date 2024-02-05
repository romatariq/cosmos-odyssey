using Domain.OptimisedSchema;

namespace Domain;

public class TravelPrice
{
    public Guid Id { get; set; }

    public DateTime ValidUntil { get; set; }

    public ICollection<Leg>? Legs { get; set; }
    
    // For cascade delete
    public ICollection<Location>? Locations { get; set; }
    public ICollection<Company>? Companies { get; set; }
    
    public ICollection<Trip>? Trips { get; set; }
}
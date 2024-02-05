using Domain.Identity;
using Domain.OptimisedSchema;

namespace Domain;

public class Reservation
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = default!;
    
    public string LastName { get; set; } = default!;

    public Guid TripId { get; set; }
    public Trip? Trip { get; set; }

    public Guid UserId { get; set; }
    public AppUser? User { get; set; }
}
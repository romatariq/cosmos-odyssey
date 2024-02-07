using System.ComponentModel.DataAnnotations;
using Domain.Identity;
using Domain.OptimisedSchema;

namespace Domain;

public class Reservation
{
    public Guid Id { get; set; }

    [MinLength(1), MaxLength(200)]
    public string FirstName { get; set; } = default!;

    [MinLength(1), MaxLength(200)]
    public string LastName { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public Guid TripId { get; set; }
    public Trip? Trip { get; set; }

    public Guid UserId { get; set; }
    public AppUser? User { get; set; }
}
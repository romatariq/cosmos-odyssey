namespace DTO.Public;

public class ReservationDetails
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = default!;
    
    public string LastName { get; set; } = default!;

    public Guid TripId { get; set; }
}
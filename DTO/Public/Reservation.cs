namespace DTO.Public;

public class Reservation
{
    public Guid Id { get; set; }
    
    public string FirstName { get; set; } = default!;
    
    public string LastName { get; set; } = default!;

    public string Route { get; set; } = default!;
    
    public DateTime Departure { get; set; }
}
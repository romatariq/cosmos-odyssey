namespace DTO.Public;

public class Trip
{
    public Guid Id { get; set; }

    public DateTime ReservableTill { get; set; }

    public decimal Price { get; set; }

    public long Distance { get; set; }

    public DateTime Departure { get; set; }
    
    public DateTime Arrival { get; set; }

    public List<Flight> Flights { get; set; } = [];
}
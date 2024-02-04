namespace DTO.BLL;

public class Trip
{
    public DateTime From { get; set; }

    public DateTime To { get; set; }

    public decimal Price { get; set; }

    public long Distance { get; set; }

    public List<string> Companies { get; set; } = default!;

    public List<Flight> Flights { get; set; } = default!;
}
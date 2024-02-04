namespace DTO.BLL;

public class Trip
{
    public string From { get; set; } = default!;

    public string To { get; set; } = default!;

    public decimal TotalPrice { get; set; }

    public long TotalDistance { get; set; }

    public TimeSpan TotalTravelTime { get; set; }

    public List<string> Companies { get; set; } = default!;

    public List<Flight> Flights { get; set; } = default!;
}
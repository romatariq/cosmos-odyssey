namespace DTO.BLL;

public class Flight
{
    public Guid ProviderId { get; set; }

    public string From { get; set; } = default!;

    public string To { get; set; } = default!;
    
    public DateTime Departure { get; set; }

    public DateTime Arrival { get; set; }

    public long Distance { get; set; }

    public decimal Price { get; set; }
}
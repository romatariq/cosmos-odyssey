namespace Domain.OptimisedSchema;

public class Trip
{
    public Guid Id { get; set; }

    public string From { get; set; } = default!;
    
    public string To { get; set; } = default!;

    public DateTime Departure { get; set; }

    public DateTime Arrival { get; set; }

    public decimal Price { get; set; }

    public long Distance { get; set; }

    public Guid TravelPriceId { get; set; }
    public TravelPrice? TravelPrice { get; set; }


    public ICollection<TripFlight>? TripFlights { get; set; }
}
namespace Domain.OptimisedSchema;

public class Flight
{
    public Guid Id { get; set; }
    
    public string From { get; set; } = default!;
    
    public string To { get; set; } = default!;
    
    public DateTime Departure { get; set; }
    
    public DateTime Arrival { get; set; }
    
    public long Distance { get; set; }
    
    public decimal Price { get; set; }

    public string Company { get; set; } = default!;

    public Guid ProviderId { get; set; }
    public Provider? Provider { get; set; }
    
    public ICollection<TripFlight>? TripFlights { get; set; }
}
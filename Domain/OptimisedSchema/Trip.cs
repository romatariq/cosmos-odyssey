namespace Domain.OptimisedSchema;

public class Trip
{
    public Guid Id { get; set; }

    public string From { get; set; } = default!;
    
    public string To { get; set; } = default!;

    public Guid TravelPriceId { get; set; }
    public TravelPrice? TravelPrice { get; set; }


    public ICollection<TripFlight>? TripFlights { get; set; }
}
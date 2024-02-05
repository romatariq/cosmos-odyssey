namespace Domain.OptimisedSchema;

public class TripFlight
{
    public Guid Id { get; set; }

    public Guid TripId { get; set; }
    public Trip? Trip { get; set; }

    public Guid FlightId { get; set; }
    public Flight? Flight { get; set; }
}
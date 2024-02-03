namespace Domain;

public class TravelPrice
{
    public Guid Id { get; set; }

    public DateTime ValidUntil { get; set; }

    public ICollection<Leg>? Legs { get; set; }
}
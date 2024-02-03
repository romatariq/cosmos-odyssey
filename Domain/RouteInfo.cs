namespace Domain;

public class RouteInfo
{
    public Guid Id { get; set; }

    public long Distance { get; set; }

    public Guid FromId { get; set; }
    public Location? From { get; set; }

    public Guid ToId { get; set; }
    public Location? To { get; set; }



    public ICollection<Leg>? Legs { get; set; }
}
namespace Domain;

public class Leg
{
    public Guid Id { get; set; }

    public Guid RouteInfoId { get; set; }
    public RouteInfo? RouteInfo { get; set; }

    public ICollection<Provider>? Providers { get; set; }

    
    public Guid TravelPriceId { get; set; }
    public TravelPrice? TravelPrice { get; set; }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class Location
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;


    [InverseProperty(nameof(RouteInfo.From))]
    public ICollection<RouteInfo>? FromRoutes { get; set; }
    
    [InverseProperty(nameof(RouteInfo.To))]
    public ICollection<RouteInfo>? ToRoutes { get; set; }
    
    
    public Guid TravelPriceId { get; set; }
    public TravelPrice? TravelPrice { get; set; }
}
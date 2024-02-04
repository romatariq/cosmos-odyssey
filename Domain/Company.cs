namespace Domain;

public class Company
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public Guid TravelPriceId { get; set; }
    public TravelPrice? TravelPrice { get; set; }

    public ICollection<Provider>? Providers { get; set; }
}
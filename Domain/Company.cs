namespace Domain;

public class Company
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public ICollection<Provider>? Providers { get; set; }
}
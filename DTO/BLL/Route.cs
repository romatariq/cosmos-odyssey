namespace DTO.BLL;

public class Route
{
    public string From { get; set; } = default!;
 
    public string To { get; set; } = default!;

    public long Distance { get; set; }
    
    public List<Flight> Flights { get; set; } = [];
}
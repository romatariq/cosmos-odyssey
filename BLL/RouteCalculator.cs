using DTO.BLL;

namespace BLL;

public static class RouteCalculator
{
    public static List<List<Flight>> GetAllPossibleTrips(string from, string to, List<Route> routes)
    {
        var graph = GetFlightsGraph(routes);
        
        var allTrips = new List<List<Flight>>();
        var visited = new HashSet<string>();
        var currentTrip = new List<Flight>();

        GetAllPossibleTrips(from, to, graph, allTrips, visited, currentTrip);

        return allTrips;
    }
    
    
    private static void GetAllPossibleTrips(string from, string to, Dictionary<string, List<Flight>> graph,
        List<List<Flight>> allTrips, HashSet<string> visited, List<Flight> currentTrip)
    {
        visited.Add(from);
        
        if (from == to)
        {
            allTrips.Add( new List<Flight>(currentTrip));
        }
        else
        {
            foreach (var flight in graph[from])
            {
                if (visited.Contains(flight.To) || (currentTrip.Count > 0 && flight.Departure < currentTrip.Last().Arrival)) continue;
                
                currentTrip.Add(flight);
                GetAllPossibleTrips(flight.To, to, graph, allTrips, visited, currentTrip);
                currentTrip.Remove(flight);
            }
        }
        
        visited.Remove(from);
    }
    
    
    private static Dictionary<string, List<Flight>> GetFlightsGraph(List<Route> routes)
    {
        var graph = GetInitialFlightsGraph();
        foreach (var route in routes)
        {
            graph[route.From].AddRange(route.Flights);
        }
        return graph;
    }
    
    
    private static Dictionary<string, List<Flight>> GetInitialFlightsGraph()
    {
        return new Dictionary<string, List<Flight>>()
        {
            ["Mercury"] = [],
            ["Venus"] = [],
            ["Earth"] = [],
            ["Mars"] = [],
            ["Jupiter"] = [],
            ["Saturn"] = [],
            ["Uranus"] = [],
            ["Neptune"] = [],
        };
    }
}
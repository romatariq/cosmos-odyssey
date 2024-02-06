using BLL;
using DTO.BLL;
using Helpers.Constants;

namespace Tests.Helpers;

public static class RouteFinderTestHelpers
{
    public static List<List<Flight>> Calculate(EPlanet from, EPlanet to, List<Route> routes)
    {
        return RouteCalculator.GetAllPossibleTrips(GetPlanet(from), GetPlanet(to), routes);
    }
    
    public static string GetPlanet(this EPlanet planet)
    {
        return Planets.AllPlanets[(int) planet];
    }
    
    private static Flight GenerateFlight(EPlanet from, EPlanet to, DateTime time)
    {
        return new Flight
        {
            From = GetPlanet(from),
            To = GetPlanet(to),
            Departure = time,
            Arrival = time
        };
    }
    
    public static List<Route> GetExampleRoutes()
    {
        return
        [
            new Route
            {
                From = GetPlanet(EPlanet.Neptune),
                To = GetPlanet(EPlanet.Mercury),
                Flights =
                [
                    GenerateFlight(EPlanet.Neptune, EPlanet.Mercury, DateTime.UtcNow)
                ]
            },
            new Route
            {
                From = GetPlanet(EPlanet.Neptune),
                To = GetPlanet(EPlanet.Uranus),
                Flights =
                [
                    GenerateFlight(EPlanet.Neptune, EPlanet.Uranus, DateTime.UtcNow)
                ]
            },
            new Route
            {
                From = GetPlanet(EPlanet.Uranus),
                To = GetPlanet(EPlanet.Saturn),
                Flights =
                [
                    GenerateFlight(EPlanet.Uranus, EPlanet.Saturn, DateTime.UtcNow.AddHours(1))
                ]
            },
            new Route
            {
                From = GetPlanet(EPlanet.Saturn),
                To = GetPlanet(EPlanet.Earth),
                Flights =
                [
                    GenerateFlight(EPlanet.Saturn, EPlanet.Earth, DateTime.UtcNow.AddHours(2))
                ]
            },
            new Route
            {
                From = GetPlanet(EPlanet.Earth),
                To = GetPlanet(EPlanet.Jupiter),
                Flights =
                [
                    GenerateFlight(EPlanet.Earth, EPlanet.Jupiter, DateTime.UtcNow.AddHours(3))
                ]
            },
            new Route
            {
                From = GetPlanet(EPlanet.Jupiter),
                To = GetPlanet(EPlanet.Mars),
                Flights =
                [
                    GenerateFlight(EPlanet.Jupiter, EPlanet.Mars, DateTime.UtcNow.AddHours(4))
                ]
            },
            new Route
            {
                From = GetPlanet(EPlanet.Mars),
                To = GetPlanet(EPlanet.Venus),
                Flights =
                [
                    GenerateFlight(EPlanet.Mars, EPlanet.Venus, DateTime.UtcNow.AddHours(5))
                ]
            },
            new Route
            {
                From = GetPlanet(EPlanet.Jupiter),
                To = GetPlanet(EPlanet.Venus),
                Flights =
                [
                    GenerateFlight(EPlanet.Jupiter, EPlanet.Venus, DateTime.UtcNow.AddHours(5))
                ]
            },
            new Route
            {
                From = GetPlanet(EPlanet.Venus),
                To = GetPlanet(EPlanet.Mercury),
                Flights =
                [
                    GenerateFlight(EPlanet.Venus, EPlanet.Mercury, DateTime.UtcNow.AddHours(6))
                ]
            },
            new Route
            {
                From = GetPlanet(EPlanet.Saturn),
                To = GetPlanet(EPlanet.Neptune),
                Flights =
                [
                    GenerateFlight(EPlanet.Saturn, EPlanet.Neptune, DateTime.UtcNow),
                    GenerateFlight(EPlanet.Saturn, EPlanet.Neptune, DateTime.UtcNow.AddHours(1).AddMinutes(29)),
                    GenerateFlight(EPlanet.Saturn, EPlanet.Neptune, DateTime.UtcNow.AddHours(1).AddMinutes(30)),
                    GenerateFlight(EPlanet.Saturn, EPlanet.Neptune, DateTime.UtcNow.AddYears(1))
                ]
            }
        ];
    }
}
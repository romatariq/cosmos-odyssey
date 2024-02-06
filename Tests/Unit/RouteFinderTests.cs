using DTO.BLL;
using Helpers.Constants;
using Microsoft.IdentityModel.Tokens;
using Tests.Helpers;
using Helper = Tests.Helpers.RouteFinderTestHelpers;

namespace Tests.Unit;

public class RouteFinderTests
{
    private readonly List<Route> _routes = Helper.GetExampleRoutes();
    
    
    [Test]
    public void TestFindsSomePathsFromNeptuneToMercury()
    {
        var validRoutes = Helper.Calculate(EPlanet.Neptune, EPlanet.Mercury, _routes);
        
        Assert.That(validRoutes.IsNullOrEmpty(), Is.False);
    }
    
    [Test]
    public void TestFindsAllPathsFromNeptuneToMercury()
    {
        var validRoutes = Helper.Calculate(EPlanet.Neptune, EPlanet.Mercury, _routes);
        
        Assert.That(validRoutes, Has.Count.EqualTo(3));
        Assert.That(validRoutes.All(r => r.Last().To == EPlanet.Mercury.GetPlanet()), Is.True);
    }
    
    [Test]
    public void TestFindsDirectPathFromNeptuneToMercury()
    {
        var validRoutes = Helper.Calculate(EPlanet.Neptune, EPlanet.Mercury, _routes);
        
        var shortestRoute = validRoutes.OrderBy(r => r.Count).First();
        
        Assert.That(shortestRoute, Has.Count.EqualTo(1));
        Assert.That(shortestRoute[0].To, Is.EqualTo(EPlanet.Mercury.GetPlanet()));
    }
    
    [Test]
    public void TestFindsLongestPathFromNeptuneToMercury()
    {
        var validRoutes = Helper.Calculate(EPlanet.Neptune, EPlanet.Mercury, _routes);
        
        var longestRoute = validRoutes.OrderByDescending(r => r.Count).First();
        
        Assert.That(longestRoute, Has.Count.EqualTo(7));
        Assert.That(longestRoute[4].To, Is.EqualTo(EPlanet.Mars.GetPlanet()));
    }
    
        
    [Test]
    public void TestFindsMiddlePathFromNeptuneToMercury()
    {
        var validRoutes = Helper.Calculate(EPlanet.Neptune, EPlanet.Mercury, _routes);
        
        var middleRoute = validRoutes.OrderBy(r => r.Count).Skip(1).First();
        
        Assert.That(middleRoute, Has.Count.EqualTo(6));
        Assert.That(middleRoute[4].To, Is.EqualTo(EPlanet.Venus.GetPlanet()));
    }
    
    [Test]
    public void TestFindAllPathsFromUranusToNeptune()
    {
        var validRoutes = Helper.Calculate(EPlanet.Uranus, EPlanet.Neptune, _routes);
        
        Assert.That(validRoutes, Has.Count.EqualTo(2));
    }
    
    [Test]
    public void TestFindAllPathsFromUranusToNeptuneAreSingleStop()
    {
        var validRoutes = Helper.Calculate(EPlanet.Uranus, EPlanet.Neptune, _routes);
        
        Assert.That(validRoutes.All(r => r.Count == 2), Is.True);
    }
    
    [Test]
    public void TestTripNotValidIfNextFlightDeparturesBeforePreviousArrives()
    {
        var validRoutes = Helper.Calculate(EPlanet.Uranus, EPlanet.Neptune, _routes);
        
        Assert.That(validRoutes.All(r => r.First().Arrival < r.Last().Departure), Is.True);
    }
    
    [Test]
    public void TestTripNotValidIfLayoverShorterThan30Minutes()
    {
        var validRoutes = Helper.Calculate(EPlanet.Uranus, EPlanet.Neptune, _routes);
        
        Assert.That(validRoutes.All(r => r.First().Arrival.AddMinutes(30) < r.Last().Departure), Is.True);
    }
    
        
    [Test]
    public void TestTripValidEvenIfLayoverIsVerLong()
    {
        var validRoutes = Helper.Calculate(EPlanet.Uranus, EPlanet.Neptune, _routes);
        
        Assert.That(validRoutes.Any(r => r.First().Arrival.AddMonths(10) < r.Last().Departure), Is.True);
    }

    
}
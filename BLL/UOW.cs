using BLL.Services;
using DAL;

namespace BLL;

public class UOW
{
    private readonly AppDbContext _context;

    public UOW(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    
    private ApiService? _apiService;
    private RouteService? _routeService;
    
    public ApiService ApiService =>
        _apiService ??= new ApiService(_context);    
    
    public RouteService RouteService =>
        _routeService ??= new RouteService(_context);
}
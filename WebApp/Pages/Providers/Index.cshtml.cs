using BLL.Services;
using DAL;
using Domain;
using Domain.Constants;
using DTO.BLL;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages_Providers
{
    public class IndexModel : PageModel
    {
        private readonly RouteService _service;

        public IndexModel(AppDbContext context)
        {
            _service = new RouteService(context);
        }

        public IList<Trip> Trips { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Trips = await _service.GetAllTrips(Planets.Venus, Planets.Mars);
        }
    }
}

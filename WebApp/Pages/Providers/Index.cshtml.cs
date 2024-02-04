using BLL.Services;
using DAL;
using Domain.Constants;
using DTO.BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages_Providers
{
    public class IndexModel : PageModel
    {
        private readonly RouteService _service;

        public IndexModel(AppDbContext context)
        {
            _service = new RouteService(context);

            var enumValues = Enum.GetValues(typeof(EPlanet))
                .Cast<EPlanet>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = Planets.AllPlanets[(int) e]
                })
                .ToList();

            FromSelectList = new SelectList(enumValues, "Value", "Text");
            ToSelectList = new SelectList(enumValues, "Value", "Text");
        }

        public IList<Trip> Trips { get;set; } = default!;
        
        public SelectList FromSelectList { get; set; }
        public SelectList ToSelectList { get; set; }

        [BindProperty]
        public EPlanet From { get; set; } = EPlanet.Earth;
        
        [BindProperty]
        public EPlanet To { get; set; } = EPlanet.Mars;
        
        public string? Filter { get; set; }

        public async Task OnGetAsync(EPlanet? from, EPlanet? to, string? filter)
        {
            if (from != null && to != null)
            {
                From = from.Value;
                To = to.Value;
            }
            Trips = await _service.GetAllTrips(From, To, filter?.Trim());
        }
    }
}

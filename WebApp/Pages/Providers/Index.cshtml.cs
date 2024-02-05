using BLL.Services;
using DAL;
using DTO.Public;
using Helpers.Constants;
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

            var planetEnumValues = Enum.GetValues(typeof(EPlanet))
                .Cast<EPlanet>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = Planets.AllPlanets[(int) e]
                })
                .ToList();
            
            var sortEnumValues = Enum.GetValues(typeof(ESortBy))
                .Cast<ESortBy>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = SortBy.Sort[(int) e]
                })
                .ToList();

            FromSelectList = new SelectList(planetEnumValues, "Value", "Text");
            ToSelectList = new SelectList(planetEnumValues, "Value", "Text");
            SortBySelectList = new SelectList(sortEnumValues, "Value", "Text");
        }

        public IList<Trip> Trips { get;set; } = default!;
        
        public SelectList FromSelectList { get; set; }
        public SelectList ToSelectList { get; set; }
        public SelectList SortBySelectList { get; set; }

        [BindProperty]
        public EPlanet From { get; set; } = EPlanet.Earth;
        
        [BindProperty]
        public EPlanet To { get; set; } = EPlanet.Mars;
        
        [BindProperty]
        public ESortBy Sort { get; set; } = ESortBy.DepartureAsc;
        
        public string? Filter { get; set; }
        public int PageNr { get; set; } = 1;
        public int PageCount { get; set; } = 1;

        public async Task OnGetAsync(EPlanet? from, EPlanet? to, ESortBy? sort, string? filter, int pageNr = 1)
        {
            if (from != null && to != null)
            {
                From = from.Value;
                To = to.Value;
            }
            Sort = sort ?? Sort;
            Filter = filter;
            PageNr = Math.Max(1, pageNr);
            
            var (trips, pageCount) = await _service.GetAllTrips(From, To, Sort, filter?.Trim(), PageNr, 15);
            Trips = trips;
            PageCount = pageCount;
        }
    }
}

using BLL;
using DTO.Public;
using Helpers.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages_Providers
{
    public class IndexModel : PageModel
    {
        private readonly UOW _uow;

        public IndexModel(UOW uow)
        {
            _uow = uow;

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
        public string? Error { get; set; }

        public async Task OnGetAsync(EPlanet? from, EPlanet? to, ESortBy? sort, string? filter, string? error, int pageNr = 1)
        {
            if (from != null && to != null)
            {
                From = from.Value;
                To = to.Value;
            }
            Sort = sort ?? Sort;
            Filter = filter;
            PageNr = Math.Max(1, pageNr);
            Error = error;
            
            var (trips, pageCount) = await _uow.RouteService
                .GetAllTrips(From, To, Sort, filter?.Trim(), PageNr, 15);
            Trips = trips;
            PageCount = pageCount;
        }
    }
}

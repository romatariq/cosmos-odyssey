using BLL;
using DTO.Public;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages_Reservations
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UOW _uow;

        public IndexModel(UOW uow)
        {
            _uow = uow;
        }

        public IList<Reservation> Reservations { get;set; } = default!;

        public string? Message { get; set; }

        public async Task OnGetAsync(string? message)
        {
            Message = message;
            Reservations = await _uow.ReservationService.GetReservations(User.GetUserId());
        }
    }
}

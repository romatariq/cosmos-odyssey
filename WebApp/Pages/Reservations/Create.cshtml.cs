using BLL;
using Domain;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages_Reservations
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly UOW _uow;

        public CreateModel(UOW uow)
        {
            _uow = uow;
        }

        public IActionResult OnGet(Guid tripId)
        {
            if (tripId == Guid.Empty)
            {
                return RedirectToPage("/Providers/Index");
            }
            
            TripId = tripId;
            return Page();
        }

        [BindProperty]
        public Reservation Reservation { get; set; } = default!;

        [BindProperty]
        public Guid TripId { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            Reservation.TripId = TripId;
            Reservation.UserId = User.GetUserId();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            if (!await _uow.ReservationService.TripStillReservable(Reservation))
            {
                return RedirectToPage("/Providers/Index");
            }

            await _uow.ReservationService.CreateReservation(Reservation);
            await _uow.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

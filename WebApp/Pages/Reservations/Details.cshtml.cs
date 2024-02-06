using BLL;
using DTO.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages_Reservations
{
    public class DetailsModel : PageModel
    {
        private readonly UOW _uow;

        public DetailsModel(UOW uow)
        {
            _uow = uow;
        }

        public ReservationDetails Reservation { get; set; } = default!;
        public Trip Trip { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var reservation = await _uow.ReservationService.GetReservation(id);
            var trip = reservation == null ? null : await _uow.ReservationService.GetTrip(reservation.TripId);
            if (reservation == null || trip == null)
            {
                return NotFound();
            }
            
            Reservation = reservation;
            Trip = trip;

            return Page();
        }
    }
}

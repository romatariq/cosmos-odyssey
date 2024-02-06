using BLL;
using Domain;
using DTO.Public;
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

        public async Task<IActionResult> OnGet(Guid tripId)
        {
            TripId = tripId;
            var trip = await _uow.ReservationService.GetTrip(TripId);
            if (trip == null)
            {
                return RedirectToPage("/Providers/Index");
            }
            
            Trip = trip;
            
            return Page();
        }

        [BindProperty]
        public Domain.Reservation Reservation { get; set; } = default!;
        
        [BindProperty]
        public Trip Trip { get; set; } = default!;

        [BindProperty]
        public Guid TripId { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            Reservation.TripId = TripId;
            Reservation.UserId = User.GetUserId();
            if (!ModelState.IsValid)
            {
                var trip = await _uow.ReservationService.GetTrip(TripId);
                if (trip == null)
                {
                    return RedirectToPage("/Providers/Index", new {error = "Sorry! Unexpected error." });
                }

                Trip = trip;
                return Page();
            }
            
            if (!await _uow.ReservationService.TripStillReservable(Reservation))
            {
                return RedirectToPage("/Providers/Index", new {error = "Sorry! The trip was no longer reservable. Please choose another one." });
            }

            await _uow.ReservationService.CreateReservation(Reservation);
            await _uow.SaveChangesAsync();

            return RedirectToPage("./Index", new {message = "Reservation created successfully!"});
        }
    }
}

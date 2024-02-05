using Microsoft.AspNetCore.Identity;

namespace Domain.Identity;

public class AppUser: IdentityUser<Guid>
{
    public ICollection<Reservation>? Reservations { get; set; }
}
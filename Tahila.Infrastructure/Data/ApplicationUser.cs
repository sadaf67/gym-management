using Microsoft.AspNetCore.Identity;

namespace Tahila.Infrastructure.Data;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImage { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;

}

using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Domain.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(UserName), IsUnique = true)]
    public class User : IdentityUser
    {
        public string? Name { get; set; }
        public enStatus Status { get; set; }
        public DateTime? CreatedAt = DateTime.UtcNow;
    }
}

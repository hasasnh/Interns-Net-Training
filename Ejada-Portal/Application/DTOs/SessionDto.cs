using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class SessionDto
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string OwnerName { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}

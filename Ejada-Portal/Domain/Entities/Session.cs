using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Session
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string OwnerName { get; set; } = string.Empty;   // The Name of Session owner

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;        //Session Name

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

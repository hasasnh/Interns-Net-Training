using Domain.Enums; 
  
namespace Domain.Entities  
{  
    public class Contributor  
    {  
        public int Id { get; set; }  
        public string Name { get; set; } = string.Empty;  
        public string Description { get; set; } = string.Empty;  
        public string? Photo { get; set; }  
        public string? FullDescription { get; set; }  
        public string? Role { get; set; }  
        public string? Email { get; set; }  
        public string? LinkedInUrl { get; set; }  
        public string? GitHubUrl { get; set; }  
        public string? TwitterUrl { get; set; }  
        public enStatus Status { get; set; } = enStatus.Active;  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  
        public DateTime? UpdatedAt { get; set; }  
    }  
} 

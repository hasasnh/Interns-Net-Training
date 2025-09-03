using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Application.DTOs
{
    public class UserDTO
    {
        [ValidateNever]
        public string Id { get; set; }
        [ValidateNever]
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [ValidateNever]
        public string ConfirmPassword { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
namespace Ejada_Portal.Models
{
    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";
    }
}

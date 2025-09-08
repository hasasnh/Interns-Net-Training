using System.ComponentModel.DataAnnotations;
namespace Ejada_Portal.Models
{

    public class ResetPasswordViewModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Token { get; set; }

        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "كلمتا المرور غير متطابقتين.")]
        public string? ConfirmPassword { get; set; }
    }
}
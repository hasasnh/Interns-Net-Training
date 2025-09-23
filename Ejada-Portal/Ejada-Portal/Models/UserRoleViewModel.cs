
namespace Ejada_Portal.Controllers
{
    namespace Ejada_Portal.ViewModels
    {
        public class UserRoleViewModel
        {
            public string Id { get; set; }      
            public string Name { get; set; }     
            public string Email { get; set; }    
            public string Role { get; set; }
            public List<string> Permissions { get; set; }
        }
    }

}
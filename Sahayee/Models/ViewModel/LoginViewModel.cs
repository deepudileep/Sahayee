using Microsoft.AspNetCore.Mvc;

namespace Sahayee.Models.ViewModel
{
    public class LoginViewModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }
    }
}

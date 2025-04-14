using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Sahayee.Helper;
public class AdminModel : PageModel
{
    public IActionResult OnGet()
    {
        // Check if the user is authenticated
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            // Get the role claim
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            // Redirect if the user is an admin
            if (roleClaim != null && roleClaim.Value == "Admin")
            {
                return RedirectToPage("/AdminDashboard"); // Adjust the path to your Admin Dashboard
            }
        }

        // If not an Admin, continue to the page
        return Page();
    }
}



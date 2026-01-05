using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace gestion_biblio.Pages.User;

[Authorize(Roles = "Member,Admin")]
public class DashboardModel : PageModel
{
    public IActionResult OnGet()
    {
        return Page();
    }
}
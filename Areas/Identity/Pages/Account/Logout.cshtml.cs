using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace gestion_biblio.Areas.Identity.Pages.Account;

[Authorize]
public class LogoutModel : PageModel
{
    private readonly SignInManager<gestion_biblio.Models.ApplicationUser> _signInManager;
    public LogoutModel(SignInManager<gestion_biblio.Models.ApplicationUser> signInManager) => _signInManager = signInManager;

    public async Task<IActionResult> OnPost()
    {
        HttpContext.Session.Clear();
        await _signInManager.SignOutAsync();
        return RedirectToPage("/Index");
    }
}

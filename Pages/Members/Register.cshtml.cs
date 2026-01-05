using System.ComponentModel.DataAnnotations;
using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace gestion_biblio.Pages.Members;

[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<gestion_biblio.Models.ApplicationUser> _userManager;
    private readonly SignInManager<gestion_biblio.Models.ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RegisterModel(ApplicationDbContext db, UserManager<gestion_biblio.Models.ApplicationUser> userManager, SignInManager<gestion_biblio.Models.ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    [BindProperty]
    public Member Member { get; set; } = new();

    [BindProperty]
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        // Create Identity user
        var identityUser = new gestion_biblio.Models.ApplicationUser { UserName = Member.Email, Email = Member.Email, FullName = $"{Member.FirstName} {Member.LastName}", EmailConfirmed = true };
        var createResult = await _userManager.CreateAsync(identityUser, Password);
        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        // Ensure role exists and assign "Member"
        if (!await _roleManager.RoleExistsAsync("Member"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Member"));
        }
        await _userManager.AddToRoleAsync(identityUser, "Member");

        // Create domain Member record
        Member.RegistrationDate = DateTime.UtcNow;
        _db.Members.Add(Member);
        await _db.SaveChangesAsync();

        // Sign in and redirect to user dashboard
        await _signInManager.SignInAsync(identityUser, isPersistent: false);
        return RedirectToPage("/User/Dashboard");
    }
}

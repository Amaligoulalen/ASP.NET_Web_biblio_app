using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblio.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly SignInManager<gestion_biblio.Models.ApplicationUser> _signInManager;
    private readonly UserManager<gestion_biblio.Models.ApplicationUser> _userManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        ApplicationDbContext db, 
        SignInManager<gestion_biblio.Models.ApplicationUser> signInManager, 
        UserManager<gestion_biblio.Models.ApplicationUser> userManager,
        ILogger<LoginModel> logger)
    {
        _db = db;
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "L'adresse email est requise")]
        [EmailAddress(ErrorMessage = "Adresse email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is invalid");
            return Page();
        }

        try
        {
            var user = await _userManager.FindByEmailAsync(Input.Email);
            
            if (user == null)
            {
                _logger.LogWarning($"User not found: {Input.Email}");
                ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, 
                Input.Password, 
                isPersistent: false, 
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User {Input.Email} logged in successfully");
                
                var roles = await _userManager.GetRolesAsync(user);
                
                if (roles.Contains("Admin"))
                {
                    return RedirectToPage("/Admin/Dashboard");
                }
                if (roles.Contains("Member"))
                {
                    return RedirectToPage("/Books/Index");
                }
                
                return LocalRedirect(ReturnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning($"User {Input.Email} account locked out");
                ModelState.AddModelError(string.Empty, "Compte verrouillé. Réessayez plus tard.");
                return Page();
            }

            _logger.LogWarning($"Invalid login attempt for {Input.Email}");
            ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            ModelState.AddModelError(string.Empty, "Une erreur s'est produite. Veuillez réessayer.");
            return Page();
        }
    }
}
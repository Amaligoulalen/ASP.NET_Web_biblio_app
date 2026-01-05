using gestion_biblio.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblio.Pages.Admin;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<gestion_biblio.Models.ApplicationUser> _userManager;
    public DashboardModel(ApplicationDbContext db, UserManager<gestion_biblio.Models.ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public int TotalBooks { get; set; }
    public int TotalAuthors { get; set; }
    public int TotalMembers { get; set; }
    public int ActiveBorrows { get; set; }
    public int OverdueBooks { get; set; }

    public class SystemUser
    {
        public string Email { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
    }
    public IList<SystemUser> AllUsers { get; set; } = new List<SystemUser>();

    public async Task<IActionResult> OnGetAsync()
    {
        TotalBooks = await _db.Books.CountAsync();
        TotalAuthors = await _db.Authors.CountAsync();
        TotalMembers = await _db.Members.CountAsync();
        ActiveBorrows = await _db.BorrowRecords.CountAsync(r => r.ReturnDate == null);
        OverdueBooks = await _db.BorrowRecords.CountAsync(r => r.ReturnDate == null && r.DueDate < DateTime.UtcNow.Date);

        var users = await _userManager.Users.ToListAsync();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            AllUsers.Add(new SystemUser
            {
                Email = u.Email ?? u.UserName ?? string.Empty,
                Roles = string.Join(",", roles),
                EmailConfirmed = u.EmailConfirmed
            });
        }
        return Page();
    }
}

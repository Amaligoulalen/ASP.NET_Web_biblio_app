using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblio.Pages.User;

[Authorize(Roles = "Member,Admin")]
public class HistoryModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public HistoryModel(ApplicationDbContext db) => _db = db;

    public List<BorrowRecord> Records { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.Identity?.Name ?? string.Empty;
        IQueryable<BorrowRecord> query = _db.BorrowRecords.Include(r => r.Book).Include(r => r.Member);
        if (User.IsInRole("Member"))
        {
            query = query.Where(r => r.Member != null && r.Member.Email == email);
        }
        Records = await query.OrderByDescending(r => r.IssueDate).ToListAsync();
        return Page();
    }
}
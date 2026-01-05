using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblio.Pages.Borrowing;

[Authorize(Roles = "Librarian,Admin,Member")]
public class ExtendModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public ExtendModel(ApplicationDbContext db) => _db = db;

    public List<BorrowRecord> OpenRecords { get; set; } = new();

    [BindProperty]
    public int RecordID { get; set; }

    [BindProperty]
    public DateTime NewDueDate { get; set; } = DateTime.UtcNow.Date.AddDays(7);

    public async Task OnGetAsync()
    {
        IQueryable<BorrowRecord> query = _db.BorrowRecords
            .Include(r => r.Member)
            .Include(r => r.Book)
            .Where(r => r.ReturnDate == null);

        if (User.IsInRole("Member"))
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.Identity?.Name ?? string.Empty;
            query = query.Where(r => r.Member != null && r.Member.Email == email);
        }

        OpenRecords = await query.OrderBy(r => r.DueDate).ToListAsync();
        if (OpenRecords.Count > 0)
        {
            RecordID = OpenRecords[0].RecordID;
            NewDueDate = OpenRecords[0].DueDate.AddDays(7);
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var record = await _db.BorrowRecords.Include(r => r.Member).Include(r => r.Book).FirstOrDefaultAsync(r => r.RecordID == RecordID);
        if (record == null)
        {
            ModelState.AddModelError(string.Empty, "Emprunt introuvable.");
            await OnGetAsync();
            return Page();
        }
        if (record.ReturnDate != null)
        {
            ModelState.AddModelError(string.Empty, "Cet emprunt est déjà retourné.");
            await OnGetAsync();
            return Page();
        }
        if (User.IsInRole("Member"))
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.Identity?.Name ?? string.Empty;
            if (record.Member == null || !string.Equals(record.Member.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "Vous ne pouvez prolonger que vos propres emprunts.");
                await OnGetAsync();
                return Page();
            }
        }
        if (NewDueDate.Date <= record.DueDate.Date)
        {
            ModelState.AddModelError(string.Empty, "La nouvelle date d’échéance doit être après l’ancienne.");
            await OnGetAsync();
            return Page();
        }

        record.DueDate = NewDueDate.Date;
        await _db.SaveChangesAsync();
        return RedirectToPage("/User/History");
    }
}

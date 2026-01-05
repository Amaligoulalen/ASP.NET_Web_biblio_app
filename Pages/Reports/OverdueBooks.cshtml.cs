using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblio.Pages.Reports;

[Authorize(Roles = "Admin,Librarian")]
public class OverdueBooksModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public OverdueBooksModel(ApplicationDbContext db) => _db = db;

    public IList<BorrowRecord> Overdues { get; set; } = new List<BorrowRecord>();

    public async Task OnGetAsync()
    {
        var today = DateTime.UtcNow.Date;
        Overdues = await _db.BorrowRecords
            .Include(r => r.Member)
            .Include(r => r.Book)
            .Where(r => r.ReturnDate == null && r.DueDate < today)
            .OrderBy(r => r.DueDate)
            .ToListAsync();
    }
}

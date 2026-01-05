using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace gestion_biblio.Pages.Borrowing
{
    [Authorize(Roles = "Librarian,Admin,Member")]
    public class ReturnModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public ReturnModel(ApplicationDbContext db) => _db = db;
    
        public List<BorrowRecord> OpenRecords { get; set; } = new();
    
        [BindProperty]
        public DateTime ReturnDate { get; set; } = DateTime.UtcNow.Date;
    
        [BindProperty]
        public int RecordID { get; set; }
    
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
        }
    
        public async Task<IActionResult> OnPostAsync()
        {
            var record = await _db.BorrowRecords.Include(r => r.Book).Include(r => r.Member).FirstOrDefaultAsync(r => r.RecordID == RecordID);
            if (record == null)
            {
                ModelState.AddModelError(string.Empty, "Borrow record not found.");
                await OnGetAsync();
                return Page();
            }
            if (record.ReturnDate != null)
            {
                ModelState.AddModelError(string.Empty, "This record is already returned.");
                await OnGetAsync();
                return Page();
            }
    
            if (User.IsInRole("Member"))
            {
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.Identity?.Name ?? string.Empty;
                if (record.Member == null || !string.Equals(record.Member.Email, email, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty, "You can only return your own borrowed books.");
                    await OnGetAsync();
                    return Page();
                }
            }
    
            record.ReturnDate = ReturnDate.Date;
            var daysLate = (record.ReturnDate.Value - record.DueDate.Date).Days;
            record.FineAmount = daysLate > 0 ? daysLate * 1m : 0m; // simple fine: $1/day late
    
            if (record.Book != null)
            {
                record.Book.AvailableCopies += 1;
            }
    
            await _db.SaveChangesAsync();
            return RedirectToPage("/Books/Index");
        }
    }
}
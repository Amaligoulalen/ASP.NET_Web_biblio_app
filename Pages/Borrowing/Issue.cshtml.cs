using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace gestion_biblio.Pages.Borrowing;

[Authorize(Roles = "Librarian,Admin,Member")]
public class IssueModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public IssueModel(ApplicationDbContext db) => _db = db;

    [BindProperty]
    public BorrowRecord BorrowRecord { get; set; } = new();

    public IEnumerable<SelectListItem> MemberOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> BookOptions { get; set; } = Enumerable.Empty<SelectListItem>();

    public async Task OnGetAsync()
    {
        if (User.IsInRole("Member"))
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.Identity?.Name ?? string.Empty;
            var member = await _db.Members.FirstOrDefaultAsync(m => m.Email == email);
            MemberOptions = member != null
                ? new List<SelectListItem> { new SelectListItem { Value = member.MemberID.ToString(), Text = $"{member.FirstName} {member.LastName}" } }
                : new List<SelectListItem>();
        }
        else
        {
            MemberOptions = await _db.Members
                .OrderBy(m => m.LastName)
                .Select(m => new SelectListItem { Value = m.MemberID.ToString(), Text = $"{m.FirstName} {m.LastName}" })
                .ToListAsync();
        }

        BookOptions = await _db.Books
            .OrderBy(b => b.Title)
            .Select(b => new SelectListItem { Value = b.BookID.ToString(), Text = $"{b.Title} ({b.AvailableCopies}/{b.TotalCopies})" })
            .ToListAsync();

        BorrowRecord.IssueDate = DateTime.UtcNow.Date;
        BorrowRecord.DueDate = BorrowRecord.IssueDate.AddDays(14);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        if (User.IsInRole("Member"))
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.Identity?.Name ?? string.Empty;
            var member = await _db.Members.FirstOrDefaultAsync(m => m.Email == email);
            if (member == null || BorrowRecord.MemberID != member.MemberID)
            {
                ModelState.AddModelError(string.Empty, "You can only borrow books for your own account.");
                await OnGetAsync();
                return Page();
            }
        }

        var book = await _db.Books.FirstOrDefaultAsync(b => b.BookID == BorrowRecord.BookID);
        if (book == null) { ModelState.AddModelError(string.Empty, "Book not found."); return Page(); }
        if (book.AvailableCopies <= 0)
        {
            ModelState.AddModelError(string.Empty, "No available copies for the selected book.");
            await OnGetAsync();
            return Page();
        }

        book.AvailableCopies -= 1;
        BorrowRecord.FineAmount = 0m;
        _db.BorrowRecords.Add(BorrowRecord);
        await _db.SaveChangesAsync();
        return RedirectToPage("/Books/Index");
    }
}
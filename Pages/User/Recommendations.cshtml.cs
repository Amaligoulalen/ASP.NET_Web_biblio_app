using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblio.Pages.User;

[Authorize(Roles = "Member")]
public class RecommendationsModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public RecommendationsModel(ApplicationDbContext db) => _db = db;

    public IList<Book> RecommendedBooks { get; set; } = new List<Book>();

    public async Task OnGetAsync()
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.Identity?.Name ?? string.Empty;
        var member = await _db.Members.FirstOrDefaultAsync(m => m.Email == email);
        if (member == null)
        {
            RecommendedBooks = new List<Book>();
            return;
        }

        var favoriteCategoryIds = await _db.BorrowRecords
            .Include(r => r.Book)
            .Where(r => r.MemberID == member.MemberID && r.Book != null)
            .GroupBy(r => r.Book!.CategoryID)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .Where(id => id != null)
            .Cast<int>()
            .Take(2)
            .ToListAsync();

        var favoriteAuthorIds = await _db.BorrowRecords
            .Include(r => r.Book)
            .Where(r => r.MemberID == member.MemberID && r.Book != null)
            .GroupBy(r => r.Book!.AuthorID)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .Take(2)
            .ToListAsync();

        var query = _db.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Where(b => b.AvailableCopies > 0 &&
                        (favoriteCategoryIds.Contains(b.CategoryID ?? -1) || favoriteAuthorIds.Contains(b.AuthorID)))
            .OrderBy(b => b.Title)
            .Take(20);

        RecommendedBooks = await query.ToListAsync();
    }
}

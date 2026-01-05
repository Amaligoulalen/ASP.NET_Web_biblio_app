using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestion_biblio.Models;

public class Book
{
    [Key]
    public int BookID { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public int AuthorID { get; set; }

    [ForeignKey(nameof(AuthorID))]
    public Author? Author { get; set; }

    [Required]
    [StringLength(13, MinimumLength = 10)]
    public string ISBN { get; set; } = string.Empty;

    [Range(1500, 2100)]
    public int PublicationYear { get; set; }

    [StringLength(100)]
    public string? Genre { get; set; }

    public int? CategoryID { get; set; }
    public Category? Category { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalCopies { get; set; }

    [Range(0, int.MaxValue)]
    public int AvailableCopies { get; set; }
    public string? Description { get; set; }

    // Navigation: One Book -> Many BorrowRecords
    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
    public string? PicturePath { get; set; }

}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestion_biblio.Models;

public class BorrowRecord
{
    [Key]
    public int RecordID { get; set; }

    [Required]
    public int MemberID { get; set; }

    [ForeignKey(nameof(MemberID))]
    public Member? Member { get; set; }

    [Required]
    public int BookID { get; set; }

    [ForeignKey(nameof(BookID))]
    public Book? Book { get; set; }

    [DataType(DataType.Date)]
    public DateTime IssueDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? ReturnDate { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal FineAmount { get; set; } = 0m;
}
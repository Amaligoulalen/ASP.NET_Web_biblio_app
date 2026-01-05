using System.ComponentModel.DataAnnotations;

namespace gestion_biblio.Models;

public class Member
{
    [Key]
    public int MemberID { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [DataType(DataType.Date)]
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    // Navigation: One Member -> Many BorrowRecords
    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
}
using System.ComponentModel.DataAnnotations;

namespace gestion_biblio.Models;

public class Admin
{
    [Key]
    public int AdminID { get; set; }

    [Required]
    [StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    // Per requirements: Manual insert only. Keep simple string storage.
    [Required]
    [StringLength(200)]
    public string Password { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace gestion_biblio.Models;

public class Author
{
    [Key]
    public int AuthorID { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Biography { get; set; }

    // Navigation: One Author -> Many Books
    public ICollection<Book> Books { get; set; } = new List<Book>();

    public string FullName => $"{FirstName} {LastName}";
}
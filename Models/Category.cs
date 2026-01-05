using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace gestion_biblio.Models;

public class Category
{
    [Key]
    public int CategoryID { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    // Navigation: One Category -> Many Books
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
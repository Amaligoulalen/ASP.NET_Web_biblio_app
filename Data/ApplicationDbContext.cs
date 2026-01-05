using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using gestion_biblio.Models;

namespace gestion_biblio.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<BorrowRecord> BorrowRecords => Set<BorrowRecord>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Book>()
            .HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryID)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<BorrowRecord>()
            .HasOne(r => r.Member)
            .WithMany(m => m.BorrowRecords)
            .HasForeignKey(r => r.MemberID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BorrowRecord>()
            .HasOne(r => r.Book)
            .WithMany(b => b.BorrowRecords)
            .HasForeignKey(r => r.BookID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Admin>()
            .HasIndex(a => a.Email)
            .IsUnique();

        builder.Entity<Member>()
            .HasIndex(m => m.Email)
            .IsUnique();
    }
}

using Library.Service.Domain;
using Microsoft.EntityFrameworkCore;
namespace Library.Service.Data;
public sealed class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
{
 public DbSet<BookEntity> Books => Set<BookEntity>(); public DbSet<BorrowerEntity> Borrowers => Set<BorrowerEntity>(); public DbSet<LoanEntity> Loans => Set<LoanEntity>();
 protected override void OnModelCreating(ModelBuilder b) { b.Entity<BookEntity>().HasIndex(x=>x.Title); b.Entity<BorrowerEntity>().HasIndex(x=>x.Email).IsUnique(); b.Entity<LoanEntity>().HasOne(x=>x.Book).WithMany(x=>x.Loans).HasForeignKey(x=>x.BookId).OnDelete(DeleteBehavior.Restrict); b.Entity<LoanEntity>().HasOne(x=>x.Borrower).WithMany(x=>x.Loans).HasForeignKey(x=>x.BorrowerId).OnDelete(DeleteBehavior.Restrict); b.Entity<LoanEntity>().HasIndex(x=>new{x.BookId,x.BorrowedAt}); b.Entity<LoanEntity>().HasIndex(x=>new{x.BorrowerId,x.BorrowedAt}); }
}

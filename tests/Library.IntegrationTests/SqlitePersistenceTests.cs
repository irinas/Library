using Library.Service.Data;
using Library.Service.Domain;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Library.IntegrationTests;
public sealed class SqlitePersistenceTests
{
 [Fact] public async Task Sqlite_PersistsBookAndLoanRelationships()
 {
  await using var connection=new SqliteConnection("Data Source=:memory:"); await connection.OpenAsync(); var options=new DbContextOptionsBuilder<LibraryDbContext>().UseSqlite(connection).Options;
  await using(var db=new LibraryDbContext(options)){await db.Database.EnsureCreatedAsync();var book=new BookEntity{Title="Test Driven Development",Author="Kent Beck",Pages=240,TotalCopies=1,AvailableCopies=0};var borrower=new BorrowerEntity{Name="Test Reader",Email="test@example.com"};db.AddRange(book,borrower);db.Loans.Add(new LoanEntity{Book=book,Borrower=borrower,BorrowedAt=DateTimeOffset.UtcNow});await db.SaveChangesAsync();}
  await using(var db=new LibraryDbContext(options)){var loan=await db.Loans.Include(x=>x.Book).Include(x=>x.Borrower).SingleAsync();Assert.Equal("Test Driven Development",loan.Book!.Title);Assert.Equal("test@example.com",loan.Borrower!.Email);Assert.Equal(1,await db.Books.CountAsync());}
 }
 [Fact] public async Task Sqlite_EnforcesUniqueBorrowerEmail(){await using var c=new SqliteConnection("Data Source=:memory:");await c.OpenAsync();var o=new DbContextOptionsBuilder<LibraryDbContext>().UseSqlite(c).Options;await using var db=new LibraryDbContext(o);await db.Database.EnsureCreatedAsync();db.Borrowers.Add(new BorrowerEntity{Name="A",Email="same@example.com"});await db.SaveChangesAsync();db.Borrowers.Add(new BorrowerEntity{Name="B",Email="same@example.com"});await Assert.ThrowsAsync<DbUpdateException>(()=>db.SaveChangesAsync());}
}

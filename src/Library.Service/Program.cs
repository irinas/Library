using Library.Service.Data;
using Library.Service.Services;
using Microsoft.EntityFrameworkCore;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddGrpc(o=>{o.EnableDetailedErrors=false;o.MaxReceiveMessageSize=4*1024*1024;o.MaxSendMessageSize=4*1024*1024;}).AddServiceOptions<LibraryRpcService>(o=>o.Interceptors.Add<ExceptionHandlingInterceptor>());
var connection=builder.Configuration.GetConnectionString("Library")??"Data Source=/data/library.db;Cache=Shared";
builder.Services.AddDbContext<LibraryDbContext>(o=>o.UseSqlite(connection));
builder.Services.AddHealthChecks().AddDbContextCheck<LibraryDbContext>(name:"database");
var app=builder.Build();
using(var scope=app.Services.CreateScope()){var db=scope.ServiceProvider.GetRequiredService<LibraryDbContext>(); await db.Database.EnsureCreatedAsync(); if(!await db.Books.AnyAsync()){var b1=new Library.Service.Domain.BookEntity{Title="Clean Code",Author="Robert C. Martin",Pages=464,TotalCopies=4,AvailableCopies=4};var b2=new Library.Service.Domain.BookEntity{Title="The Pragmatic Programmer",Author="Andrew Hunt and David Thomas",Pages=352,TotalCopies=3,AvailableCopies=3};var b3=new Library.Service.Domain.BookEntity{Title="Domain-Driven Design",Author="Eric Evans",Pages=560,TotalCopies=2,AvailableCopies=2};db.Books.AddRange(b1,b2,b3);db.Borrowers.AddRange(new Library.Service.Domain.BorrowerEntity{Name="Alex Reader",Email="alex@example.com"},new Library.Service.Domain.BorrowerEntity{Name="Sam Reader",Email="sam@example.com"});await db.SaveChangesAsync();}}
app.MapGrpcService<LibraryRpcService>(); app.MapHealthChecks("/health"); app.MapGet("/",()=>Results.Ok(new{service="Library internal gRPC service",status="running"})); app.Run();

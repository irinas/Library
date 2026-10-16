using Grpc.Core;
using Library.Contracts;
namespace Library.Api.Services;
/// <summary>Thin gRPC edge: delegates business operations to the internal service and preserves gRPC status codes.</summary>
public sealed class LibraryGatewayService(LibraryRpc.LibraryRpcClient client, ILogger<LibraryGatewayService> logger) : LibraryRpc.LibraryRpcBase
{
 public override Task<BookReply> CreateBook(CreateBookRequest r,ServerCallContext c)=>Call(()=>client.CreateBookAsync(r,options:Options(c)).ResponseAsync,c);
 public override Task<BookReply> GetBook(GetBookRequest r,ServerCallContext c)=>Call(()=>client.GetBookAsync(r,options:Options(c)).ResponseAsync,c);
 public override Task<ListBooksReply> ListBooks(ListBooksRequest r,ServerCallContext c)=>Call(()=>client.ListBooksAsync(r,options:Options(c)).ResponseAsync,c);
 public override Task<BorrowerReply> RegisterBorrower(RegisterBorrowerRequest r,ServerCallContext c)=>Call(()=>client.RegisterBorrowerAsync(r,options:Options(c)).ResponseAsync,c);
 public override Task<LoanReply> BorrowBook(BorrowBookRequest r,ServerCallContext c)=>Call(()=>client.BorrowBookAsync(r,options:Options(c)).ResponseAsync,c);
 public override Task<LoanReply> ReturnBook(ReturnBookRequest r,ServerCallContext c)=>Call(()=>client.ReturnBookAsync(r,options:Options(c)).ResponseAsync,c);
 public override Task<BooksAnalyticsReply> GetMostBorrowedBooks(AnalyticsRequest r,ServerCallContext c)=>Call(()=>client.GetMostBorrowedBooksAsync(r,options:Options(c)).ResponseAsync,c);
 public override Task<BorrowerActivityReply> GetMostActiveBorrowers(BorrowerActivityRequest r,ServerCallContext c)=>Call(()=>client.GetMostActiveBorrowersAsync(r,options:Options(c)).ResponseAsync,c);
 public override Task<ReadingPaceReply> GetReadingPace(ReadingPaceRequest r,ServerCallContext c)=>Call(()=>client.GetReadingPaceAsync(r,options:Options(c)).ResponseAsync,c);
 public override Task<BooksAnalyticsReply> GetRelatedBooks(RelatedBooksRequest r,ServerCallContext c)=>Call(()=>client.GetRelatedBooksAsync(r,options:Options(c)).ResponseAsync,c);
 public override Task<HealthReply> Health(HealthRequest r,ServerCallContext c)=>Call(()=>client.HealthAsync(r,options:Options(c)).ResponseAsync,c);
 private static CallOptions Options(ServerCallContext c)=>new(deadline:c.Deadline,cancellationToken:c.CancellationToken);
 private async Task<T> Call<T>(Func<Task<T>> action,ServerCallContext context) { try{return await action();} catch(RpcException ex) { logger.LogWarning("Internal gRPC call failed with status {Status}: {Detail}",ex.StatusCode,ex.Status.Detail); throw; } catch(OperationCanceledException) when(context.CancellationToken.IsCancellationRequested) { throw; } catch(Exception ex) { logger.LogError(ex,"Unexpected internal service failure"); throw new RpcException(new Status(StatusCode.Unavailable,"The library service is temporarily unavailable.")); } }
}

using Grpc.AspNetCore.Server;
using Grpc.Core;
using Grpc.Core.Interceptors;
namespace Library.Service.Services;
/// <summary>Prevents implementation details from escaping over the RPC boundary.</summary>
public sealed class ExceptionHandlingInterceptor(ILogger<ExceptionHandlingInterceptor> logger) : Interceptor
{
 public override async Task<TResponse> UnaryServerHandler<TRequest,TResponse>(TRequest request,ServerCallContext context,UnaryServerMethod<TRequest,TResponse> continuation)
 {
  try { return await continuation(request,context); }
  catch(RpcException) { throw; }
  catch(OperationCanceledException) when(context.CancellationToken.IsCancellationRequested) { throw; }
  catch(Exception ex) { logger.LogError(ex,"Unhandled exception processing gRPC method {Method}; trace {TraceId}",context.Method,context.GetHttpContext().TraceIdentifier); throw new RpcException(new Status(StatusCode.Internal,"An unexpected error occurred. Contact support with the request trace identifier.")); }
 }
}

using Grpc.Net.Client.Configuration;
using Library.Api.Services;
using Library.Contracts;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddGrpc(o=>{o.EnableDetailedErrors=false;o.MaxReceiveMessageSize=4*1024*1024;o.MaxSendMessageSize=4*1024*1024;});
var serviceAddress=builder.Configuration["LibraryService:Address"]??"http://library-service:8080";
builder.Services.AddGrpcClient<LibraryRpc.LibraryRpcClient>(o=>o.Address=new Uri(serviceAddress)).ConfigureChannel(o=>{o.ServiceConfig=new ServiceConfig{MethodConfigs={new MethodConfig{Names={MethodName.Default},RetryPolicy=new RetryPolicy{MaxAttempts=3,InitialBackoff=TimeSpan.FromMilliseconds(200),MaxBackoff=TimeSpan.FromSeconds(2),BackoffMultiplier=2,RetryableStatusCodes={Grpc.Core.StatusCode.Unavailable}}}}};});
var app=builder.Build(); app.MapGrpcService<LibraryGatewayService>(); app.MapGet("/",()=>Results.Ok(new{service="Library API",protocol="gRPC",status="running"})); app.Run();
public partial class Program { }

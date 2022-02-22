using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ConfTool.Client.Utils
{
    public static class GrpcExtensions
    {
        public static GrpcChannel BuildGrpcChannel(IServiceProvider services, WebAssemblyHostBuilder builder)
        {
            var baseAddressMessageHandler = services.GetRequiredService<BaseAddressAuthorizationMessageHandler>();
            baseAddressMessageHandler.InnerHandler = new HttpClientHandler();
            var grpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, baseAddressMessageHandler);

            var channel = GrpcChannel.ForAddress(builder.Configuration["Conferences:BackendUrl"], new GrpcChannelOptions { HttpHandler = grpcWebHandler });

            return channel;
        }
    }
}

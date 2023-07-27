using Grpc.Net.Client;
using GrpcClient;
using WebAPIMicrosservicoConsumer.Features.Services.Models;

namespace WebAPIMicrosservicoConsumer.Services.Grpc
{
    public class ContractWebAPIClient : IContractWebAPIClient
    {
        public async Task MessageGrpc(UserViewModel userViewModel)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7119");
            var client = new MicrosservicoService.MicrosservicoServiceClient(channel);
            var reply = await client.ContractWebAPIAsync(new UserRequest { Id = userViewModel.Id, Message = userViewModel.Message });
            Console.WriteLine(reply.Message);
        }
    }
}

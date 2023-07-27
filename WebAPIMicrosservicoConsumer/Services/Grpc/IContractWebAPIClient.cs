using WebAPIMicrosservicoConsumer.Features.Services.Models;

namespace WebAPIMicrosservicoConsumer.Services.Grpc
{
    public interface IContractWebAPIClient
    {
        Task MessageGrpc(UserViewModel userViewModel);
    }
}

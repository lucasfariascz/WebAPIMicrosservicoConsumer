using Newtonsoft.Json;

namespace WebAPIMicrosservicoConsumer.Features.Services.Models
{
    public class UserViewModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string? Message { get; set; }
    }
}

using CosmosDBExemple.Data;
using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Text.Json;
using WebAPIMicrosservico.Config.ServiceBus;
using WebAPIMicrosservico.Features.User.Domain.Models;
using WebAPIMicrosservicoConsumer.Features.Services.Models;

namespace WebAPIMicrosservicoConsumer.Features.Services
{
    public class QueueConsumerService : IHostedService
    {
        private readonly NoSQLDatabase<UserViewModel> _noSQLDataBase;
        public string container = "WebAPIMicroConsumer";
        static IQueueClient queueClient;
        private static readonly string QueueName = AppSettings.QueueName;
        private static readonly string AzureServiceBus = AppSettings.AzureServiceBus;

        public QueueConsumerService()
        {
            _noSQLDataBase = new();
            queueClient = new QueueClient(AzureServiceBus, QueueName);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("############## Starting Consumer - Queue ####################");
            ProcessMessageHandler();
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("############## Stopping Consumer - Queue ####################");
            await queueClient.CloseAsync();
            await Task.CompletedTask;
        }

        private void ProcessMessageHandler()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            Console.WriteLine("### Processing Message - Queue ###");
            Console.WriteLine($"{DateTime.Now}");
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
            UserViewModel _userViewModel = JsonSerializer.Deserialize<UserViewModel>(message.Body);
            await _noSQLDataBase.Add(container, _userViewModel, _userViewModel.Id.ToString());

            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

    }
}

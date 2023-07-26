using CosmosDBExemple.Data;
using Microsoft.Azure.ServiceBus;
using System.Text.Json;
using WebAPIMicrosservico.Config.ServiceBus;
using WebAPIMicrosservicoConsumer.Features.Services.Models;

namespace WebAPIMicrosservicoConsumer.Features.Services
{
    public class QueueConsumerService : IHostedService
    {
        private readonly NoSQLDatabase<UserViewModel> noSQLDataBase;
        public string container = "WebAPIMicroConsumer";
        static IQueueClient queueClient;
        private static readonly string QueueName = AppSettings.QueueName;
        private static readonly string AzureServiceBus = AppSettings.AzureServiceBus;

        public QueueConsumerService()
        {
            noSQLDataBase = new();
            queueClient = new QueueClient(AzureServiceBus, QueueName);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Starting Consumer - Queue
            ProcessMessageHandler();
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stopping Consumer - Queue
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
            // Processing Message - Queue
            UserViewModel userViewModel = JsonSerializer.Deserialize<UserViewModel>(message.Body);
            await this.noSQLDataBase.Add(container, userViewModel, userViewModel.Id.ToString());

            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;   
            return Task.CompletedTask;
        }

    }
}

using com.vorba.sand.services.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace com.vorba.sand.services.ServiceBus
{
    public class QueueConsumerService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<QueueConsumerService> logger;
        private readonly ServiceBusOptions options;
        private readonly IHubContext<MessageHub> messageHubContext;

        //private readonly IMessageHub messageHub;

        //private Timer? _timer;
        private QueueClient? queueClient;

        public QueueConsumerService(
            ILogger<QueueConsumerService> logger,
            IOptions<ServiceBusOptions> options,
            //IMessageHub messageHub,
            IHubContext<MessageHub> messageHubContext
            )
        {
            this.logger = logger;
            this.options = options.Value;
            //this.messageHub = messageHub;
            this.messageHubContext = messageHubContext;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation($"{nameof(QueueConsumerService)} starting...");

            //_timer = new Timer(DoWork, null, TimeSpan.Zero,
            //    TimeSpan.FromSeconds(5));

            //var client = new ServiceBusClient(""); // todo: research Azure.Messaging.ServiceBus

            queueClient = new QueueClient(options.PrimaryConnectionString?.ToString(), options.QueueName);

            queueClient.RegisterMessageHandler(ProcessMessageAsync, new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = false,
                MaxConcurrentCalls = 1,
            });

            return Task.CompletedTask;
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            this.logger.LogError($"Exception: {arg.Exception.Message}");

            return Task.CompletedTask;
        }

        private async Task ProcessMessageAsync(Message message, CancellationToken arg2)
        {
            if (queueClient == null)
            {
                logger.LogWarning($"{nameof(queueClient)} is null");
                return;
            }
            var jsonString = Encoding.UTF8.GetString(message.Body);
            var model = JsonSerializer.Deserialize<SampleMessageModel>(jsonString);
            logger.LogInformation($"Message: ${model?.SomeStringProperty}");
            //await messageHub.task1(model?.SomeStringProperty ?? "(no data)");
            await messageHubContext.Clients.All.SendAsync("task1_Response", model?.SomeStringProperty ?? "(no data)"); //.ConfigureAwait(false);
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        //private void DoWork(object state)
        //{
        //    var count = Interlocked.Increment(ref executionCount);
        //
        //    _logger.LogInformation(
        //        $"{nameof(QueueConsumerService)} working. Count: {count}");
        //}

        public Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation($"{nameof(QueueConsumerService)} stopping...");

            //_timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            //_timer?.Dispose();
        }
    }
}

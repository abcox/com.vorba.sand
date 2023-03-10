using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace com.vorba.sand.services.ServiceBus
{
    public class QueueServices : IQueueServices
    {
        private readonly ServiceBusOptions options;

        public QueueServices(IOptions<ServiceBusOptions> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            this.options = options.Value;
        }

        public async Task SendMessageAsync<T>(T messageObject, string queueName)
        {
            var queueClient = new QueueClient(options.PrimaryConnectionString, queueName);
            string messageBody = JsonSerializer.Serialize(messageObject);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            await queueClient.SendAsync(message);
        }
    }
}

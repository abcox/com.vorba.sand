using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace com.vorba.sand.services2.ServiceBus
{
    public class QueueServices : IQueueServices
    {
        private readonly ServiceBusOptions _options;

        public QueueServices(IOptions<ServiceBusOptions> serviceBusOptions)
        {
            _options = serviceBusOptions.Value;
        }

        private QueueClient GetClient(ServiceBusOptions options, string queueName)
        {
            if (options == null) throw new ArgumentNullException(nameof(ServiceBusOptions));
            var primaryKey = options.PrimaryKey;
            var connectionString = $"{options.PrimaryConnectionString}{primaryKey}";
            return new QueueClient(connectionString, queueName);
        }

        public async Task SendMessageAsync<T>(T messageObject, string queueName)
        {
            string messageBody = JsonSerializer.Serialize(messageObject);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            var client = GetClient(_options, queueName);
            await client.SendAsync(message);
        }
    }
}

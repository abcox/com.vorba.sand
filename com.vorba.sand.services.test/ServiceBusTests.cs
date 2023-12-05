using com.vorba.sand.services.ServiceBus;
using Microsoft.Extensions.Options;
using Moq;

namespace com.vorba.sand.services.test
{
    [TestClass]
    public class ServiceBusTests
    {
        [TestMethod]
        public async Task SendMessageAsync()
        {
            string name = "vorba1";
            string primaryKey = "kzM //\\//\\ **** REDACTED **** //\\//\\ Wk=";
            var options = Options.Create<ServiceBusOptions>(new ServiceBusOptions
            {
                PrimaryConnectionString = $"Endpoint=sb://{name}.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey={primaryKey}",
            });
            var queueServices = new QueueServices(options);
            var messageObject = new SampleMessageModel { SomeStringProperty = $"{nameof(SampleMessageModel.SomeStringProperty)}Value" };
            await queueServices.SendMessageAsync(messageObject, "test");
        }
    }
}
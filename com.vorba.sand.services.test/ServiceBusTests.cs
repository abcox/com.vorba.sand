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
            var options = Options.Create<ServiceBusOptions>(new ServiceBusOptions
            {
                PrimaryConnectionString = "Endpoint=sb://vorba1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=INyR0mVeH3tsaXJFriIZP5JO4rQWoBG/r+ASbBrg9T4=",
            });
            var queueServices = new QueueServices(options);
            var messageObject = new SampleMessageModel { SomeStringProperty = $"{nameof(SampleMessageModel.SomeStringProperty)}Value" };
            await queueServices.SendMessageAsync(messageObject, "test");
        }
    }
}
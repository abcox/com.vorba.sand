using com.vorba.sand.services.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace com.vorba.sand.services.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> logger;
        private readonly ServiceBusOptions options;
        private readonly QueueServices queueServices;

        public MessageController(
            ILogger<MessageController> logger,
            IOptions<ServiceBusOptions> options
            )
        {
            this.logger = logger;
            if (options == null )
            {
                throw new ArgumentNullException(nameof(options));
            }
            this.options = options.Value;           
            queueServices = new QueueServices(options);
        }

        [HttpPost(Name = nameof(SendMessage))]
        public async Task SendMessage(string valueString)
        {
            var messageObject = new SampleMessageModel { SomeStringProperty = valueString };
            await queueServices.SendMessageAsync(messageObject, options.QueueName ?? "test");
        }
    }
}
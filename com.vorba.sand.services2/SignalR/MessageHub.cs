using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace com.vorba.sand.services2.SignalR
{
    public class Task1_Payload
    {
        public string? data { get; set; }
    }

    public class MessageHub : Hub, IMessageHub
    {
        private readonly ILogger<MessageHub> logger;

        public MessageHub(ILogger<MessageHub> logger)
        {
            this.logger = logger;
        }

        public async Task task1(object jsonPayload)
        {
            string messageOut = "empty message";

            if (jsonPayload == null)
                throw new ArgumentNullException(nameof(jsonPayload));

            try
            {
                var jsonString = jsonPayload.ToString();
                //var payload = JsonConvert.DeserializeObject<Task1_Payload>(jsonString);
                var payload = JsonSerializer.Deserialize<Task1_Payload>(jsonString);
                messageOut = $"{payload.data} there!";
                await task1_Response(messageOut);
            }
            catch (Exception ex)
            {
                logger.LogError($"{nameof(task1_Response)} ERROR: {ex.Message}", ex);
            }
        }

        public Task task1_Response(string message)
        {
            return Task.Run(() =>
            {
                Enumerable.Range(0, 3).ToList().ForEach(x =>
                {
                    Clients.Client(this.Context.ConnectionId).SendAsync(nameof(task1_Response), $"{message} {x}");
                    Thread.Sleep(10000);
                });
            });
        }
    }
}

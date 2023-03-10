namespace com.vorba.sand.services.ServiceBus
{
    public interface IQueueServices
    {
        Task SendMessageAsync<T>(T messageObject, string queueName);
    }
}
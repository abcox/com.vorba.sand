namespace com.vorba.sand.services.SignalR
{
    public interface IMessageHub
    {
        Task task1(object jsonPayload);
        Task task1_Response(string message);
    }
}
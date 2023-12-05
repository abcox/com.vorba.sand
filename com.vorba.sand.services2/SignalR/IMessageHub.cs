namespace com.vorba.sand.services2.SignalR
{
    public interface IMessageHub
    {
        Task task1(object jsonPayload);
        Task task1_Response(string message);
    }
}
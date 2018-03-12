namespace MessageBusLib
{
    public interface ISubscriber
    {
        object Receiver { get; set; }
        string Filter { get; set; }
        void UseAction(object message);
    }
}
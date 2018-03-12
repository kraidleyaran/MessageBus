using System;

namespace MessageBusLib
{
    public class Subscriber<T> : ISubscriber where T : EventMessage
    {
        public object Receiver { get; set; }
        public string Filter { get; set; }
        public Action<T> Action;

        public void UseAction(object message)
        {
            var msg = (T) message;

            Action.Invoke(msg);
        }
    }
}
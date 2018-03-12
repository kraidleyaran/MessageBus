using System;

namespace MessageBusLib
{
    [Serializable]
    public class EventMessage
    {
        public object Sender { get; set; }
    }

    [Serializable]
    internal class InternalRemoveSubscriptionMessage : EventMessage
    {
        public object Subscriber { get; set; }
    }

    [Serializable]
    internal class InternalSubscriptionActionMessage : EventMessage
    {
        public object Subscriber { get; set; }
        public object Message { get; set; }
        public string Filter { get; set; }
    }
}
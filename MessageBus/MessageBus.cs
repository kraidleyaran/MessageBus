using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MessageBusLib
{
    public delegate void MessageEvent();
    public static class MessageBus
    {
        //private static SubscriberDirectory Subjects = new SubscriberDirectory();
        private static Dictionary<Type, List<ISubscriber>> Subjects = new Dictionary<Type, List<ISubscriber>>();

        public const string ALL_FILTER = "*";

        public static void SubscribeWithFilter<T>(this object subscriber, Action<T> action, string filter = null) where T : EventMessage
        {
            var type = typeof(T);
            if (!Subjects.ContainsKey(type))
            {
                Subjects.Add(type, new List<ISubscriber>());
            }
            Subjects[type].Add(new Subscriber<T> { Receiver = subscriber, Filter = filter, Action = action });
        }

        public static void Subscribe<T>(this object subscriber, Action<T> action) where T : EventMessage
        {
            SubscribeWithFilter(subscriber, action, ALL_FILTER);
        }

        public static void Unsubscribe<T>(this object subscriber) where T: EventMessage
        {
            var msgType = typeof(T);
            if (Subjects.ContainsKey(msgType))
            {
                Subjects[msgType].RemoveAll(s => s.Receiver == subscriber);
            }
        }

        public static void UnsubscribeFromFilter<T>(this object subscriber, string filter) where T: EventMessage
        {
            var msgType = typeof(T);
            if (Subjects.ContainsKey(msgType))
            {
                Subjects[msgType].RemoveAll(s => s.Receiver == subscriber);
            }
        }

        public static void UnsubscribeFromAllMessages(this object subscriber)
        {
            var subjects = Subjects.Values.ToList();
            foreach (var sub in subjects.Where(subject => subject.Exists(subscribe => subscribe.Receiver == subscriber)))
            {
                sub.RemoveAll(s => s.Receiver == subscriber);
            }
        }

        public static void SendMessage<T>(this object sender,T message) where T : EventMessage
        {
            message.Sender = sender;
            var msgType = message.GetType();
            if (Subjects.ContainsKey(msgType))
            {
                foreach (var sub in Subjects[msgType].FindAll(o => o.Filter == null || o.Filter == ALL_FILTER))
                {
                    sub.UseAction(message);
                }
            }

        }

        public static void SendMessageWithFilter<T>(this object sender,T message, string filter) where T: EventMessage
        {
            message.Sender = sender;
            var msgType = message.GetType();
            if (Subjects.ContainsKey(msgType))
            {
                foreach (var sub in Subjects[msgType].FindAll(obj => obj.Filter == filter || obj.Filter == ALL_FILTER))
                {
                    sub.UseAction(message);
                }
            }

        }

        public static void SendMessageTo<T>(this object sender, T message, object receiver) where T: EventMessage
        {
            message.Sender = sender;
            var msgType = message.GetType();
            if (Subjects.ContainsKey(msgType))
            {
                var sub = Subjects[msgType].Find(s => s.Receiver == receiver);
                sub?.UseAction(message);
            }
        }

        public static void Clear()
        {
            Subjects.Clear();
        }
    }
}

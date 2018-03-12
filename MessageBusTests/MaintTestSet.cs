using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageBusLib;
using NUnit.Framework;

namespace MessageBusTests
{
    [TestFixture]
    public class MainTestSet
    {
        [SetUp]
        public void Setup()
        {
            MessageBus.Clear();
        }

        [Test]
        public void SubscribingWorksCorrectly()
        {
            var messenger = new object();
            var receivedMessage = false;
            var data = string.Empty;
            var testString = "Test String";
            messenger.Subscribe(new Action<TestEvent>(e =>
            {
                receivedMessage = true;
                data = e.Data;
            }));
            messenger.SendMessage(new TestEvent{Data = testString});
            Assert.AreEqual(testString, data);
            Assert.IsTrue(receivedMessage);
        }

        [Test]
        public void SubscribingWithAFilter_WorksCorrectly()
        {
            var messenger = new object();
            var otherMessenger = new object();

            var othermessageReceived = false;
            var receivedMessage = false;
            var data = string.Empty;
            var otherData = string.Empty;

            var testString = "Test String";
            var otherTestString = "not data";
            var testFilter = "test filter";
            messenger.SubscribeWithFilter(new Action<TestEvent>(e =>
            {
                receivedMessage = true;
                data = e.Data;
            }), testFilter);
            otherMessenger.SubscribeWithFilter(new Action<TestEvent>(e =>
            {
                othermessageReceived = true;
                otherData = e.Data;
            }), "other filter");
            messenger.SendMessageWithFilter(new TestEvent{Data = testString}, testFilter);
            messenger.SendMessageWithFilter(new TestEvent{Data = otherTestString}, "other filter" );
            Assert.IsTrue(receivedMessage);
            Assert.AreEqual(testString, data);

            Assert.IsTrue(othermessageReceived);
            Assert.AreEqual(otherData, otherTestString);
        }

        [Test]
        public void UnsubscribingFromASpecificMessage_WorksCorrectly()
        {
            var messenger = new object();
            var receivedMessage = false;
            var otherMessage = false;
            var otherData = string.Empty;
            var data = string.Empty;
            var testString = "Test String";
            messenger.Subscribe(new Action<TestEvent>(e =>
            {
                receivedMessage = true;
                data = e.Data;
            }));
            messenger.SendMessage(new TestEvent { Data = testString });
            Assert.AreEqual(testString, data);
            Assert.IsTrue(receivedMessage);

            receivedMessage = false;
            data = string.Empty;
            messenger.Subscribe(new Action<OtherTestEvent>(e =>
            {
                otherMessage = true;
                otherData = e.Data;
            }));
            messenger.Unsubscribe<TestEvent>();
            messenger.SendMessage(new TestEvent{Data = testString});
            messenger.SendMessage(new OtherTestEvent{Data = testString});
            
            Assert.AreEqual(string.Empty, data);
            Assert.IsFalse(receivedMessage);

            Assert.AreEqual(otherData, testString);
            Assert.IsTrue(otherMessage);
        }

        [Test]
        public void UnsubscribingFromAllMessages_WorksCorrectly()
        {
            var messenger = new object();
            var receivedMessage = false;
            var otherMessage = false;
            var otherData = string.Empty;
            var data = string.Empty;
            var testString = "Test String";
            messenger.Subscribe(new Action<TestEvent>(e =>
            {
                receivedMessage = true;
                data = e.Data;
            }));
            messenger.Subscribe(new Action<OtherTestEvent>(e =>
            {
                otherMessage = true;
                otherData = e.Data;
            }));
            messenger.SendMessage(new TestEvent { Data = testString });
            messenger.SendMessage(new OtherTestEvent { Data = testString });


            Assert.AreEqual(testString, data);
            Assert.IsTrue(receivedMessage);

            receivedMessage = false;
            data = string.Empty;


            otherMessage = false;
            otherData = string.Empty;

            messenger.UnsubscribeFromAllMessages();
            messenger.SendMessage(new TestEvent { Data = testString });
            messenger.SendMessage(new OtherTestEvent { Data = testString });


            Assert.AreEqual(string.Empty, data);
            Assert.IsFalse(receivedMessage);

            Assert.AreEqual(string.Empty, otherData);
            Assert.IsFalse(otherMessage);
        }

        [Test]
        public void SendingAMessageDirectlyToSomeoneSubscribed_WorksCorrectly()
        {
            var messenger = new object();
            var other = new object();
            var otherReceived = false;
            var data = string.Empty;
            var receivedMessage = false;
            var testString = "Test String";

            messenger.Subscribe(new Action<TestEvent>(msg =>
            {
                data = msg.Data;
                receivedMessage = true;
            }));
            other.Subscribe(new Action<TestEvent>(msg =>
            {
                otherReceived = true;
            }));
            messenger.SendMessageTo(new TestEvent{Data = testString}, messenger);
            Assert.IsTrue(receivedMessage);
            Assert.AreEqual(testString, data);
            Assert.IsFalse(otherReceived);
        }
    }

    public class TestEvent : EventMessage
    {
        public string Data { get; set; }
    }

    public class OtherTestEvent : EventMessage
    {
        public string Data { get; set; }
    }
}



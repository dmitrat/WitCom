﻿using NUnit.Framework;
using OutWit.Common.ProtoBuf.Messages;

namespace OutWit.Common.ProtoBuf.Tests.Messages
{
    [TestFixture]
    public class ProtoBufMessageTests
    {
        [Test]
        public void ConstructorTest()
        {
            var message = new ProtoBufMessage("testMessage", true);

            Assert.That(message.Message, Is.EqualTo("testMessage"));
            Assert.That(message.IsError, Is.EqualTo(true));
        }

        [Test]
        public void IsTest()
        {
            var message1 = new ProtoBufMessage("testMessage", true);
            var message2 = new ProtoBufMessage("testMessage", true);

            Assert.That(message1.Is(message2), Is.True);

            message2 = new ProtoBufMessage("testMessage1", true);
            Assert.That(message1.Is(message2), Is.False);

            message2 = new ProtoBufMessage("testMessage", false);
            Assert.That(message1.Is(message2), Is.False);
        }

        [Test]
        public void CloneTest()
        {
            var message1 = new ProtoBufMessage("testMessage", true);

            var message2 = message1.Clone() as ProtoBufMessage;
            Assert.That(message2, Is.Not.Null);

            Assert.That(message1, Is.Not.SameAs(message2));
            Assert.That(message1.Is(message2), Is.True);
        }

        [Test]
        public void SerializationTest()
        {
            var message1 = new ProtoBufMessage("testMessage", true);

            var bytes = message1.ToProtoBytes();
            Assert.That(bytes, Is.Not.Null);
            
            var message2 = bytes.FromProtoBytes<ProtoBufMessage>();
            Assert.That(message2, Is.Not.Null);

            Assert.That(message1, Is.Not.SameAs(message2));
            Assert.That(message1.Is(message2), Is.True);
        }
    }
}

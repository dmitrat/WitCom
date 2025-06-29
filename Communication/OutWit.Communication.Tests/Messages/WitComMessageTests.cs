﻿using OutWit.Common.Json;
using OutWit.Common.MemoryPack;
using OutWit.Common.MessagePack;
using OutWit.Common.ProtoBuf;
using OutWit.Communication.Utils;
using OutWit.Communication.Messages;
using OutWit.Common.Utils;

namespace OutWit.Communication.Tests.Messages
{
    [TestFixture]
    public class WitMessageTests
    {
        private const string GUID = "EE4E38B5-942D-4D2C-8A5C-E4A897B2C831";

        [Test]
        public void ConstructorTest()
        {
            var message = new WitMessage();
            Assert.That(message.Id, Is.EqualTo(Guid.Empty));
            Assert.That(message.Type, Is.EqualTo(WitMessageType.Unknown));
            Assert.That(message.Data, Is.EqualTo(null));

            message = new WitMessage
            {
                Id = Guid.Parse(GUID),
                Type = WitMessageType.Request,
                Data = new byte[] { 1, 2, 3 }
            };

            Assert.That(message.Id, Is.EqualTo(Guid.Parse(GUID)));
            Assert.That(message.Type, Is.EqualTo(WitMessageType.Request));
            Assert.That(message.Data, Is.EqualTo(new byte[] { 1, 2, 3 }));
        }

        [Test]
        public void IsTest()
        {
            var message = new WitMessage
            {
                Id = Guid.Parse(GUID),
                Type = WitMessageType.Request,
                Data = new byte[] { 1, 2, 3 }
            };

            Assert.That(message.Is(message.Clone()), Is.EqualTo(true));

            Assert.That(message.Is(message.With(x => x.Id = Guid.NewGuid())), Is.EqualTo(false));
            Assert.That(message.Is(message.With(x => x.Type = WitMessageType.Initialization)), Is.EqualTo(false));
            Assert.That(message.Is(message.With(x => x.Data = new byte[] { 1, 2 })), Is.EqualTo(false));
        }

        [Test]
        public void CloneTest()
        {
            var message1 = new WitMessage
            {
                Id = Guid.Parse(GUID),
                Type = WitMessageType.Request,
                Data = new byte[] { 1, 2, 3 }
            };
            var message2 = message1.Clone() as WitMessage;

            Assert.That(message2, Is.Not.Null);
            Assert.That(message1, Is.Not.SameAs(message2));

            Assert.That(message2.Id, Is.EqualTo(Guid.Parse(GUID)));
            Assert.That(message2.Type, Is.EqualTo(WitMessageType.Request));
            Assert.That(message2.Data, Is.EqualTo(new byte[] { 1, 2, 3 }));
        }

        [Test]
        public void JsonCloneTest()
        {
            var message1 = new WitMessage
            {
                Id = Guid.Parse(GUID),
                Type = WitMessageType.Request,
                Data = new byte[] { 1, 2, 3 }
            };
            var message2 = message1.JsonClone() as WitMessage;

            Assert.That(message2, Is.Not.Null);
            Assert.That(message1, Is.Not.SameAs(message2));
            Assert.That(message1.Is(message2), Is.True);
        }

        [Test]
        public void MessagePackSerializationTest()
        {
            var message1 = new WitMessage
            {
                Id = Guid.Parse(GUID),
                Type = WitMessageType.Request,
                Data = new byte[] { 1, 2, 3 }
            };

            var bytes = message1.ToMessagePackBytes();
            Assert.That(bytes, Is.Not.Null);

            var message2 = bytes.FromMessagePackBytes<WitMessage>();
            Assert.That(message2, Is.Not.Null);
            Assert.That(message1, Is.Not.SameAs(message2));
            Assert.That(message1.Is(message2), Is.True);

        }

        [Test]
        public void JsonSerializationTest()
        {
            var message1 = new WitMessage
            {
                Id = Guid.Parse(GUID),
                Type = WitMessageType.Request,
                Data = new byte[] { 1, 2, 3 }
            };

            var json = message1.ToJsonString();
            Assert.That(json, Is.Not.Null);

            var message2 = json.FromJsonString<WitMessage>();
            Assert.That(message2, Is.Not.Null);
            Assert.That(message1, Is.Not.SameAs(message2));
            Assert.That(message1.Is(message2), Is.True);
        }

        [Test]
        public void MemoryPackSerializationTest()
        {
            var message1 = new WitMessage
            {
                Id = Guid.Parse(GUID),
                Type = WitMessageType.Request,
                Data = new byte[] { 1, 2, 3 }
            };

            var json = message1.ToMemoryPackBytes();
            Assert.That(json, Is.Not.Null);

            var message2 = json.FromMemoryPackBytes<WitMessage>();
            Assert.That(message2, Is.Not.Null);
            Assert.That(message1, Is.Not.SameAs(message2));
            Assert.That(message1.Is(message2), Is.True);
        }

        [Test]
        public void ProtoBufSerializationTest()
        {
            var message1 = new WitMessage
            {
                Id = Guid.Parse(GUID),
                Type = WitMessageType.Request,
                Data = new byte[] { 1, 2, 3 }
            };

            var json = message1.ToProtoBytes();
            Assert.That(json, Is.Not.Null);

            var message2 = json.FromProtoBytes<WitMessage>();
            Assert.That(message2, Is.Not.Null);
            Assert.That(message1, Is.Not.SameAs(message2));
            Assert.That(message1.Is(message2), Is.True);
        }
    }
}

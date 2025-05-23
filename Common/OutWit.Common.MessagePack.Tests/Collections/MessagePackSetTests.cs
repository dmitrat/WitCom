﻿using System;
using OutWit.Common.Collections;
using OutWit.Common.MessagePack.Collections;
using OutWit.Common.Serialization;
using NUnit.Framework;

namespace OutWit.Common.MessagePack.Tests.Collections
{
    [TestFixture]
    public class MessagePackSetTests
    {
        [Test]
        public void ConstructorTest()
        {
            var collection = new MessagePackSet<int>(1, 2, 3, 4, 5);
            Assert.That(collection.Count, Is.EqualTo(5));
            Assert.That(collection[0], Is.EqualTo(1));
            Assert.That(collection[1], Is.EqualTo(2));
            Assert.That(collection[2], Is.EqualTo(3));
            Assert.That(collection[3], Is.EqualTo(4));
            Assert.That(collection[4], Is.EqualTo(5));
        }

        [Test]
        public void IsTest()
        {
            var collection1 = new MessagePackSet<int>(1, 2, 3, 4, 5);
            var collection2 = new MessagePackSet<int>(1, 2, 3, 4, 5);

            Assert.That(collection1.Is(collection2), Is.True);
            collection2 = new MessagePackSet<int>(2, 2, 3, 4, 5);
            Assert.That(collection1.Is(collection2), Is.False);
            collection1 = new MessagePackSet<int>(2, 2, 3, 4, 5);
            Assert.That(collection1.Is(collection2), Is.True);
        }

        [Test]
        public void MessagePackSerializationTests()
        {
            var collection = new MessagePackSet<int>(1, 2, 3, 4, 5);
            var bytes = collection.ToMessagePackBytes(false);

            Assert.That(bytes, Is.Not.Null);

            Console.WriteLine(bytes.Length);

            var collection1 = bytes.FromMessagePackBytes<MessagePackSet<int>>(false);

            Assert.That(collection1, Is.Not.Null);
            Assert.That(collection.Is(collection1), Is.True);

        }
    }
}

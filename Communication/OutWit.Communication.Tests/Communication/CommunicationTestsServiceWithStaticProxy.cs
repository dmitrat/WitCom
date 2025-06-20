﻿using System.Net;
using OutWit.Communication.Serializers;
using OutWit.Communication.Server.Authorization;
using OutWit.Communication.Server.Encryption;
using OutWit.Communication.Server.Pipes;
using OutWit.Communication.Server;
using OutWit.Communication.Tests.Mock;
using OutWit.Communication.Client;
using OutWit.Communication.Client.Authorization;
using OutWit.Communication.Client.Encryption;
using OutWit.Communication.Client.Pipes;
using System.Runtime.CompilerServices;
using OutWit.Communication.Processors;
using OutWit.Communication.Tests.Mock.Interfaces;
using Castle.DynamicProxy;
using OutWit.Common.Utils;
using OutWit.Communication.Interceptors;
using OutWit.Communication.Tests.Mock.Model;
using OutWit.Communication.Server.Discovery;
using OutWit.Communication.Tests._Mock.Interfaces;

namespace OutWit.Communication.Tests.Communication
{
    [TestFixture]
    public class CommunicationTestsServiceWithStaticProxy
    {
        private const string PIPE_NAME = "TestPipe";
        private const string AUTHORIZATION_TOKEN = "token";

        [TestCase(TransportType.MMF, SerializerType.Json)]
        [TestCase(TransportType.MMF, SerializerType.MessagePack)]
        [TestCase(TransportType.MMF, SerializerType.MemoryPack)]

        [TestCase(TransportType.Pipes, SerializerType.Json)]
        [TestCase(TransportType.Pipes, SerializerType.MessagePack)]
        [TestCase(TransportType.Pipes, SerializerType.MemoryPack)]

        [TestCase(TransportType.Tcp, SerializerType.Json)]
        [TestCase(TransportType.Tcp, SerializerType.MessagePack)]
        [TestCase(TransportType.Tcp, SerializerType.MemoryPack)]

        [TestCase(TransportType.TcpSecure, SerializerType.Json)]
        [TestCase(TransportType.TcpSecure, SerializerType.MessagePack)]
        [TestCase(TransportType.TcpSecure, SerializerType.MemoryPack)]

        [TestCase(TransportType.WebSocket, SerializerType.Json)]
        [TestCase(TransportType.WebSocket, SerializerType.MessagePack)]
        [TestCase(TransportType.WebSocket, SerializerType.MemoryPack)]
        public async Task SimpleRequestsSingleClientTest(TransportType transportType, SerializerType serializerType)
        {
            var testName = $"{nameof(SimpleRequestsSingleClientTest)}_{transportType}_{serializerType}";

            var server = Shared.GetServer(transportType, serializerType, 1, testName);

            server.StartWaitingForConnection();

            var client = Shared.GetClient(transportType, serializerType, testName);

            Assert.That(await client.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client.IsInitialized, Is.True);
            Assert.That(client.IsAuthorized, Is.True);

            var service = Shared.GetServiceStatic(client);

            Assert.That(service.StringProperty, Is.EqualTo("TestString"));
            Assert.That(service.DoubleProperty, Is.EqualTo(1.2));

            Assert.That(service.RequestData("text"), Is.EqualTo("text"));
        }

        [TestCase(TransportType.MMF, SerializerType.Json)]
        [TestCase(TransportType.MMF, SerializerType.MessagePack)]
        [TestCase(TransportType.MMF, SerializerType.MemoryPack)]

        [TestCase(TransportType.Pipes, SerializerType.Json)]
        [TestCase(TransportType.Pipes, SerializerType.MessagePack)]
        [TestCase(TransportType.Pipes, SerializerType.MemoryPack)]

        [TestCase(TransportType.Tcp, SerializerType.Json)]
        [TestCase(TransportType.Tcp, SerializerType.MessagePack)]
        [TestCase(TransportType.Tcp, SerializerType.MemoryPack)]

        [TestCase(TransportType.TcpSecure, SerializerType.Json)]
        [TestCase(TransportType.TcpSecure, SerializerType.MessagePack)]
        [TestCase(TransportType.TcpSecure, SerializerType.MemoryPack)]

        [TestCase(TransportType.WebSocket, SerializerType.Json)]
        [TestCase(TransportType.WebSocket, SerializerType.MessagePack)]
        [TestCase(TransportType.WebSocket, SerializerType.MemoryPack)]
        public async Task SimpleRequestsSingleClientAsyncTest(TransportType transportType, SerializerType serializerType)
        {
            var testName = $"{nameof(SimpleRequestsSingleClientAsyncTest)}_{transportType}_{serializerType}";

            var server = Shared.GetServer(transportType, serializerType, 1, testName);

            server.StartWaitingForConnection();

            var client = Shared.GetClient(transportType, serializerType, testName);

            Assert.That(await client.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client.IsInitialized, Is.True);
            Assert.That(client.IsAuthorized, Is.True);

            var service = Shared.GetServiceStatic(client);

            Assert.That(await service.RequestDataAsync("text"), Is.EqualTo("text"));
        }

        [TestCase(TransportType.MMF, SerializerType.Json)]
        //[TestCase(TransportType.MMF, SerializerType.MessagePack)]
        //[TestCase(TransportType.MMF, SerializerType.MemoryPack)]

        [TestCase(TransportType.Pipes, SerializerType.Json)]
        //[TestCase(TransportType.Pipes, SerializerType.MessagePack)]
        //[TestCase(TransportType.Pipes, SerializerType.MemoryPack)]

        [TestCase(TransportType.Tcp, SerializerType.Json)]
        //[TestCase(TransportType.Tcp, SerializerType.MessagePack)]
        //[TestCase(TransportType.Tcp, SerializerType.MemoryPack)]

        [TestCase(TransportType.TcpSecure, SerializerType.Json)]
        //[TestCase(TransportType.TcpSecure, SerializerType.MessagePack)]
        //[TestCase(TransportType.TcpSecure, SerializerType.MemoryPack)]

        [TestCase(TransportType.WebSocket, SerializerType.Json)]
        //[TestCase(TransportType.WebSocket, SerializerType.MessagePack)]
        //[TestCase(TransportType.WebSocket, SerializerType.MemoryPack)]
        public async Task PropertyChangedCallbackTest(TransportType transportType, SerializerType serializerType)
        {
            var testName = $"{nameof(PropertyChangedCallbackTest)}_{transportType}_{serializerType}";

            var server = Shared.GetServer(transportType, serializerType, 1, testName);

            server.StartWaitingForConnection();

            var client = Shared.GetClient(transportType, serializerType, testName);

            Assert.That(await client.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client.IsInitialized, Is.True);
            Assert.That(client.IsAuthorized, Is.True);

            var service = Shared.GetServiceStatic(client);

            int callbackCount = 0;
            service.PropertyChanged += (s, e) =>
            {
                if (e.IsProperty((IService ser) => ser.DoubleProperty))
                    callbackCount++;
            };

            Assert.That(service.DoubleProperty, Is.EqualTo(1.2));

            service.DoubleProperty = 3.4;

            Thread.Sleep(200);
            Assert.That(service.DoubleProperty, Is.EqualTo(3.4));
            Assert.That(callbackCount, Is.EqualTo(1));

        }

        [TestCase(TransportType.MMF, SerializerType.Json)]
        [TestCase(TransportType.MMF, SerializerType.MessagePack)]
        [TestCase(TransportType.MMF, SerializerType.MemoryPack)]

        [TestCase(TransportType.Pipes, SerializerType.Json)]
        [TestCase(TransportType.Pipes, SerializerType.MessagePack)]
        [TestCase(TransportType.Pipes, SerializerType.MemoryPack)]

        [TestCase(TransportType.Tcp, SerializerType.Json)]
        [TestCase(TransportType.Tcp, SerializerType.MessagePack)]
        [TestCase(TransportType.Tcp, SerializerType.MemoryPack)]

        [TestCase(TransportType.TcpSecure, SerializerType.Json)]
        [TestCase(TransportType.TcpSecure, SerializerType.MessagePack)]
        [TestCase(TransportType.TcpSecure, SerializerType.MemoryPack)]

        [TestCase(TransportType.WebSocket, SerializerType.Json)]
        [TestCase(TransportType.WebSocket, SerializerType.MessagePack)]
        [TestCase(TransportType.WebSocket, SerializerType.MemoryPack)]
        public async Task SingleSubscribeSingleClientSimpleCallbackTest(TransportType transportType, SerializerType serializerType)
        {
            var testName = $"{nameof(SingleSubscribeSingleClientSimpleCallbackTest)}_{transportType}_{serializerType}";

            var server = Shared.GetServer(transportType, serializerType, 1, testName);

            server.StartWaitingForConnection();

            var client = Shared.GetClient(transportType, serializerType, testName);

            Assert.That(await client.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client.IsInitialized, Is.True);
            Assert.That(client.IsAuthorized, Is.True);

            var service = Shared.GetServiceStatic(client);

            int callbackCount = 0;
            string actual = "";
            service.Error += text =>
            {
                callbackCount++;
                actual = text;
                Console.WriteLine(text);
            };

            service.ReportError("text1");
            Thread.Sleep(200);
            Assert.That(callbackCount, Is.EqualTo(1));
            Assert.That(actual, Is.EqualTo("text1"));

            service.ReportError("text2");
            Thread.Sleep(200);
            Assert.That(callbackCount, Is.EqualTo(2));
            Assert.That(actual, Is.EqualTo("text2"));
        }

        [TestCase(TransportType.MMF, SerializerType.Json)]
        [TestCase(TransportType.MMF, SerializerType.MessagePack)]
        [TestCase(TransportType.MMF, SerializerType.MemoryPack)]

        [TestCase(TransportType.Pipes, SerializerType.Json)]
        [TestCase(TransportType.Pipes, SerializerType.MessagePack)]
        [TestCase(TransportType.Pipes, SerializerType.MemoryPack)]

        [TestCase(TransportType.Tcp, SerializerType.Json)]
        [TestCase(TransportType.Tcp, SerializerType.MessagePack)]
        [TestCase(TransportType.Tcp, SerializerType.MemoryPack)]

        [TestCase(TransportType.TcpSecure, SerializerType.Json)]
        [TestCase(TransportType.TcpSecure, SerializerType.MessagePack)]
        [TestCase(TransportType.TcpSecure, SerializerType.MemoryPack)]

        [TestCase(TransportType.WebSocket, SerializerType.Json)]
        [TestCase(TransportType.WebSocket, SerializerType.MessagePack)]
        [TestCase(TransportType.WebSocket, SerializerType.MemoryPack)]
        public async Task SingleSubscribeSingleClientSimpleCallbackAsyncTest(TransportType transportType, SerializerType serializerType)
        {
            var testName = $"{nameof(SingleSubscribeSingleClientSimpleCallbackAsyncTest)}_{transportType}_{serializerType}";

            var server = Shared.GetServer(transportType, serializerType, 1, testName);

            server.StartWaitingForConnection();

            var client = Shared.GetClient(transportType, serializerType, testName);

            Assert.That(await client.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client.IsInitialized, Is.True);
            Assert.That(client.IsAuthorized, Is.True);

            var service = Shared.GetServiceStatic(client);

            int callbackCount = 0;
            string actual = "";
            service.Error += text =>
            {
                callbackCount++;
                actual = text;
                Console.WriteLine(text);
            };

            await service.ReportErrorAsync("text1");
            Thread.Sleep(200);
            Assert.That(callbackCount, Is.EqualTo(1));
            Assert.That(actual, Is.EqualTo("text1"));

            await service.ReportErrorAsync("text2");
            Thread.Sleep(200);
            Assert.That(callbackCount, Is.EqualTo(2));
            Assert.That(actual, Is.EqualTo("text2"));
        }

        [TestCase(TransportType.MMF, SerializerType.Json)]
        [TestCase(TransportType.MMF, SerializerType.MessagePack)]
        [TestCase(TransportType.MMF, SerializerType.MemoryPack)]

        [TestCase(TransportType.Pipes, SerializerType.Json)]
        [TestCase(TransportType.Pipes, SerializerType.MessagePack)]
        [TestCase(TransportType.Pipes, SerializerType.MemoryPack)]

        [TestCase(TransportType.Tcp, SerializerType.Json)]
        [TestCase(TransportType.Tcp, SerializerType.MessagePack)]
        [TestCase(TransportType.Tcp, SerializerType.MemoryPack)]

        [TestCase(TransportType.TcpSecure, SerializerType.Json)]
        [TestCase(TransportType.TcpSecure, SerializerType.MessagePack)]
        [TestCase(TransportType.TcpSecure, SerializerType.MemoryPack)]

        [TestCase(TransportType.WebSocket, SerializerType.Json)]
        [TestCase(TransportType.WebSocket, SerializerType.MessagePack)]
        [TestCase(TransportType.WebSocket, SerializerType.MemoryPack)]
        public async Task SingleSubscribeComplexTypeSingleClientCallbackTest(TransportType transportType, SerializerType serializerType)
        {
            var testName = $"{nameof(SingleSubscribeComplexTypeSingleClientCallbackTest)}_{transportType}_{serializerType}";

            var server = Shared.GetServer(transportType, serializerType, 1, testName);

            server.StartWaitingForConnection();

            var client = Shared.GetClient(transportType, serializerType, testName);

            Assert.That(await client.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client.IsInitialized, Is.True);
            Assert.That(client.IsAuthorized, Is.True);

            var service = Shared.GetServiceStatic(client);
            int callbackCount = 0;
            ComplexNumber<int, int>? actualNum = null;
            int actualIter = 0;
            service.StartProcessingRequested += (num, iter) =>
            {
                callbackCount++;
                actualNum = num;
                actualIter = iter;
                Console.WriteLine(num);
            };

            service.StartProcessing(new ComplexNumber<int, int>(1, 2), 3);
            Thread.Sleep(200);
            Assert.That(callbackCount, Is.EqualTo(1));
            Assert.That(actualNum!.A, Is.EqualTo(1));
            Assert.That(actualNum!.B, Is.EqualTo(2));
            Assert.That(actualIter, Is.EqualTo(3));

            service.StartProcessing(new ComplexNumber<int, int>(4, 5), 6);
            Thread.Sleep(200);
            Assert.That(callbackCount, Is.EqualTo(2));
            Assert.That(actualNum!.A, Is.EqualTo(4));
            Assert.That(actualNum!.B, Is.EqualTo(5));
            Assert.That(actualIter, Is.EqualTo(6));
        }

        [TestCase(TransportType.MMF, SerializerType.Json)]
        [TestCase(TransportType.MMF, SerializerType.MessagePack)]
        [TestCase(TransportType.MMF, SerializerType.MemoryPack)]

        [TestCase(TransportType.Pipes, SerializerType.Json)]
        [TestCase(TransportType.Pipes, SerializerType.MessagePack)]
        [TestCase(TransportType.Pipes, SerializerType.MemoryPack)]

        [TestCase(TransportType.Tcp, SerializerType.Json)]
        [TestCase(TransportType.Tcp, SerializerType.MessagePack)]
        [TestCase(TransportType.Tcp, SerializerType.MemoryPack)]

        [TestCase(TransportType.TcpSecure, SerializerType.Json)]
        [TestCase(TransportType.TcpSecure, SerializerType.MessagePack)]
        [TestCase(TransportType.TcpSecure, SerializerType.MemoryPack)]

        [TestCase(TransportType.WebSocket, SerializerType.Json)]
        [TestCase(TransportType.WebSocket, SerializerType.MessagePack)]
        [TestCase(TransportType.WebSocket, SerializerType.MemoryPack)]
        public async Task SingleSubscribeComplexTypeSingleClientCallbackAsyncTest(TransportType transportType, SerializerType serializerType)
        {
            var testName = $"{nameof(SingleSubscribeComplexTypeSingleClientCallbackAsyncTest)}_{transportType}_{serializerType}";

            var server = Shared.GetServer(transportType, serializerType, 1, testName);

            server.StartWaitingForConnection();

            var client = Shared.GetClient(transportType, serializerType, testName);

            Assert.That(await client.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client.IsInitialized, Is.True);
            Assert.That(client.IsAuthorized, Is.True);

            var service = Shared.GetServiceStatic(client);
            
            int callbackCount = 0;
            ComplexNumber<int, int>? actualNum = null;
            int actualIter = 0;
            service.StartProcessingRequested += (num, iter) =>
            {
                callbackCount++;
                actualNum = num;
                actualIter = iter;
                Console.WriteLine(num);
            };

            await service.StartProcessingAsync(new ComplexNumber<int, int>(1, 2), 3);
            Thread.Sleep(200);
            Assert.That(callbackCount, Is.EqualTo(1));
            Assert.That(actualNum!.A, Is.EqualTo(1));
            Assert.That(actualNum!.B, Is.EqualTo(2));
            Assert.That(actualIter, Is.EqualTo(3));

            await service.StartProcessingAsync(new ComplexNumber<int, int>(4, 5), 6);
            Thread.Sleep(200);
            Assert.That(callbackCount, Is.EqualTo(2));
            Assert.That(actualNum!.A, Is.EqualTo(4));
            Assert.That(actualNum!.B, Is.EqualTo(5));
            Assert.That(actualIter, Is.EqualTo(6));
        }

        [TestCase(TransportType.Pipes, SerializerType.Json)]
        [TestCase(TransportType.Pipes, SerializerType.MessagePack)]
        [TestCase(TransportType.Pipes, SerializerType.MemoryPack)]

        [TestCase(TransportType.Tcp, SerializerType.Json)]
        [TestCase(TransportType.Tcp, SerializerType.MessagePack)]
        [TestCase(TransportType.Tcp, SerializerType.MemoryPack)]

        [TestCase(TransportType.TcpSecure, SerializerType.Json)]
        [TestCase(TransportType.TcpSecure, SerializerType.MessagePack)]
        [TestCase(TransportType.TcpSecure, SerializerType.MemoryPack)]

        [TestCase(TransportType.WebSocket, SerializerType.Json)]
        [TestCase(TransportType.WebSocket, SerializerType.MessagePack)]
        [TestCase(TransportType.WebSocket, SerializerType.MemoryPack)]
        public async Task MultiSubscribeMultiClientsCallbackTest(TransportType transportType, SerializerType serializerType)
        {
            var testName = $"{nameof(MultiSubscribeMultiClientsCallbackTest)}_{transportType}_{serializerType}";

            var server = Shared.GetServer(transportType, serializerType, 5, testName);

            server.StartWaitingForConnection();

            var client1 = Shared.GetClient(transportType, serializerType, testName);

            Assert.That(await client1.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client1.IsInitialized, Is.True);
            Assert.That(client1.IsAuthorized, Is.True);

            var service1 = Shared.GetServiceStatic(client1);


            var client2 = Shared.GetClient(transportType, serializerType, testName);

            Assert.That(await client2.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client2.IsInitialized, Is.True);
            Assert.That(client2.IsAuthorized, Is.True);

            var service2 = Shared.GetServiceStatic(client1);

            int callbackFirstCount = 0;
            int callbackSecondCount = 0;
            string actualFirst = "";
            string actualSecond = "";
            service1.Error += text =>
            {
                callbackFirstCount++;
                actualFirst = text;
                Console.WriteLine(text);
            };

            service1.ReportError("text1");
            Thread.Sleep(200);
            Assert.That(callbackFirstCount, Is.EqualTo(1));
            Assert.That(actualFirst, Is.EqualTo("text1"));
            Assert.That(callbackSecondCount, Is.EqualTo(0));
            Assert.That(actualSecond, Is.EqualTo(""));

            service2.Error += text =>
            {
                callbackSecondCount++;
                actualSecond = text;
                Console.WriteLine(text);
            };

            service2.ReportError("text2");
            Thread.Sleep(200);
            Assert.That(callbackFirstCount, Is.EqualTo(2));
            Assert.That(actualFirst, Is.EqualTo("text2"));
            Assert.That(callbackSecondCount, Is.EqualTo(1));
            Assert.That(actualSecond, Is.EqualTo("text2"));

            service1.ReportError("text3");
            Thread.Sleep(200);
            Assert.That(callbackFirstCount, Is.EqualTo(3));
            Assert.That(actualFirst, Is.EqualTo("text3"));
            Assert.That(callbackSecondCount, Is.EqualTo(2));
            Assert.That(actualSecond, Is.EqualTo("text3"));
        }

        [TestCase(TransportType.Pipes, SerializerType.Json)]
        [TestCase(TransportType.Pipes, SerializerType.MessagePack)]
        [TestCase(TransportType.Pipes, SerializerType.MemoryPack)]

        [TestCase(TransportType.Tcp, SerializerType.Json)]
        [TestCase(TransportType.Tcp, SerializerType.MessagePack)]
        [TestCase(TransportType.Tcp, SerializerType.MemoryPack)]

        [TestCase(TransportType.TcpSecure, SerializerType.Json)]
        [TestCase(TransportType.TcpSecure, SerializerType.MessagePack)]
        [TestCase(TransportType.TcpSecure, SerializerType.MemoryPack)]

        [TestCase(TransportType.WebSocket, SerializerType.Json)]
        [TestCase(TransportType.WebSocket, SerializerType.MessagePack)]
        [TestCase(TransportType.WebSocket, SerializerType.MemoryPack)]
        public async Task MultiSubscribeMultiClientsCallbackAsyncTest(TransportType transportType, SerializerType serializerType)
        {
            var testName = $"{nameof(MultiSubscribeMultiClientsCallbackAsyncTest)}_{transportType}_{serializerType}";

            var server = Shared.GetServer(transportType, serializerType, 5, testName);

            server.StartWaitingForConnection();

            var client1 = Shared.GetClient(transportType, serializerType, testName);

            Assert.That(await client1.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client1.IsInitialized, Is.True);
            Assert.That(client1.IsAuthorized, Is.True);

            var service1 = Shared.GetServiceStatic(client1);


            var client2 = Shared.GetClient(transportType, serializerType, testName);

            Assert.That(await client2.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client2.IsInitialized, Is.True);
            Assert.That(client2.IsAuthorized, Is.True);

            var service2 = Shared.GetServiceStatic(client1);

            int callbackFirstCount = 0;
            int callbackSecondCount = 0;
            string actualFirst = "";
            string actualSecond = "";
            service1.Error += text =>
            {
                callbackFirstCount++;
                actualFirst = text;
                Console.WriteLine(text);
            };

            await service1.ReportErrorAsync("text1");
            Thread.Sleep(200);
            Assert.That(callbackFirstCount, Is.EqualTo(1));
            Assert.That(actualFirst, Is.EqualTo("text1"));
            Assert.That(callbackSecondCount, Is.EqualTo(0));
            Assert.That(actualSecond, Is.EqualTo(""));

            service2.Error += text =>
            {
                callbackSecondCount++;
                actualSecond = text;
                Console.WriteLine(text);
            };

            await service2.ReportErrorAsync("text2");
            Thread.Sleep(200);
            Assert.That(callbackFirstCount, Is.EqualTo(2));
            Assert.That(actualFirst, Is.EqualTo("text2"));
            Assert.That(callbackSecondCount, Is.EqualTo(1));
            Assert.That(actualSecond, Is.EqualTo("text2"));

            await service1.ReportErrorAsync("text3");
            Thread.Sleep(200);
            Assert.That(callbackFirstCount, Is.EqualTo(3));
            Assert.That(actualFirst, Is.EqualTo("text3"));
            Assert.That(callbackSecondCount, Is.EqualTo(2));
            Assert.That(actualSecond, Is.EqualTo("text3"));
        }

    }
}

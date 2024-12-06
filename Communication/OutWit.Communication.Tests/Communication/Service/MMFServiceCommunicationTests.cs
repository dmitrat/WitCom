﻿using OutWit.Communication.Converters;
using OutWit.Communication.Serializers;
using OutWit.Communication.Server.Authorization;
using OutWit.Communication.Server.Encryption;
using OutWit.Communication.Server;
using OutWit.Communication.Tests.Mock;
using OutWit.Communication.Client;
using OutWit.Communication.Client.Authorization;
using OutWit.Communication.Client.Encryption;
using System.Runtime.CompilerServices;
using OutWit.Communication.Processors;
using OutWit.Communication.Tests.Mock.Interfaces;
using Castle.DynamicProxy;
using OutWit.Communication.Client.MMF;
using OutWit.Communication.Interceptors;
using OutWit.Communication.Server.MMF;
using OutWit.Communication.Tests.Mock.Model;

namespace OutWit.Communication.Tests.Communication.Service
{
    [TestFixture]
    public class MMFServiceCommunicationTests
    {
        private const string MEMORY_MAPPED_FILE_NAME = "TestMMF";
        private const string AUTHORIZATION_TOKEN = "token";

        [Test]
        public async Task SimpleRequestsSingleClientTest()
        {
            var server = GetServer();
            server.StartWaitingForConnection();

            var client = GetClient();

            Assert.That(await client.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client.IsInitialized, Is.True);
            Assert.That(client.IsAuthorized, Is.True);

            var service = GetService(client);

            Assert.That(service.RequestData("text"), Is.EqualTo("text"));
            Assert.That(service.GenericSimple(12, "34", 5.6), Is.EqualTo(5.6));
            Assert.That(service.GenericComplex(12, "34", new ComplexNumber<int, double>(56, 6.7)).Is(new ComplexNumber<int, double>(56, 6.7)), Is.EqualTo(true));
            Assert.That(service.GenericComplexArray(12, "34", new List<ComplexNumber<int, double>>
            {
                new ComplexNumber<int, double>(56, 6.7),
                new ComplexNumber<int, double>(89, 10.11),
                new ComplexNumber<int, double>(123, 14.15),
            }).Is(new ComplexNumber<int, double>(56, 6.7)), Is.EqualTo(true));

            Assert.That(service.GenericComplexMulti(new ComplexNumber<string, string>("aa", "bb"), "34", new List<ComplexNumber<int, double>>
            {
                new ComplexNumber<int, double>(56, 6.7),
                new ComplexNumber<int, double>(89, 10.11),
                new ComplexNumber<int, double>(123, 14.15),
            }).Is(new ComplexNumber<string, int>("bb", 56)), Is.EqualTo(true));
        }

        [Test]
        public async Task SingleSubscribeSingleClientSimpleCallbackTest()
        {
            var server = GetServer();
            server.StartWaitingForConnection();

            var client = GetClient();

            Assert.That(await client.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client.IsInitialized, Is.True);
            Assert.That(client.IsAuthorized, Is.True);

            var service = GetService(client);

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

        [Test]
        public async Task SingleSubscribeComplexTypeSingleClientCallbackTest()
        {
            var server = GetServer();
            server.StartWaitingForConnection();

            var client = GetClient();

            Assert.That(await client.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client.IsInitialized, Is.True);
            Assert.That(client.IsAuthorized, Is.True);

            var service = GetService(client);
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

        private IService GetService(WitComClient client)
        {
            var proxyGenerator = new ProxyGenerator();
            var interceptor = new RequestInterceptor(client, true);

            return proxyGenerator.CreateInterfaceProxyWithoutTarget<IService>(interceptor);
        }

        private WitComServer GetServer([CallerMemberName] string pipeName = MEMORY_MAPPED_FILE_NAME)
        {
            var service = new MockService();

            var serverTransport = new MemoryMappedFileServerTransportFactory(new MemoryMappedFileServerTransportOptions()
            {
                Name = pipeName,
                Size = 1024 * 1024
            });
            return new WitComServer(serverTransport,
                new EncryptorServerFactory<EncryptorServerGeneral>(),
                new AccessTokenValidatorStatic(AUTHORIZATION_TOKEN),
                new MessageSerializerJson(),
                new ValueConverterJson(),
                new RequestProcessor<IService>(service));
        }

        private WitComClient GetClient([CallerMemberName] string pipeName = MEMORY_MAPPED_FILE_NAME)
        {
            var clientTransport = new MemoryMappedFileClientTransport(new MemoryMappedFileClientTransportOptions
            {
                Name = pipeName
            });

            return new WitComClient(clientTransport,
                new EncryptorClientGeneral(),
                new AccessTokenProviderStatic(AUTHORIZATION_TOKEN),
                new MessageSerializerJson(),
                new ValueConverterJson());
        }

    }
}

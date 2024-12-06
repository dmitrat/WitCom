﻿using OutWit.Communication.Converters;
using OutWit.Communication.Serializers;
using OutWit.Communication.Server.Authorization;
using OutWit.Communication.Server.Encryption;
using OutWit.Communication.Server;
using OutWit.Communication.Tests.Mock;
using OutWit.Communication.Client;
using OutWit.Communication.Client.Authorization;
using OutWit.Communication.Client.Encryption;
using OutWit.Communication.Model;
using OutWit.Communication.Requests;
using OutWit.Communication.Responses;
using System.Runtime.CompilerServices;
using OutWit.Communication.Client.Tcp;
using OutWit.Communication.Server.Tcp;

namespace OutWit.Communication.Tests.Communication.Basic
{
    [TestFixture]
    public class TcpBasicCommunicationTests
    {
        private const string AUTHORIZATION_TOKEN = "token";

        [Test]
        public async Task ConnectionTest()
        {
            var server = GetServer(1);
            server.StartWaitingForConnection();

            var client = GetClient();

            Assert.That(await client.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client.IsInitialized, Is.True);
            Assert.That(client.IsAuthorized, Is.True);
        }

        [Test]
        public async Task TooManyClientsSingleClientAllowedConnectionTest()
        {
            var server = GetServer(1);
            server.StartWaitingForConnection();

            var client1 = GetClient();

            Assert.That(await client1.ConnectAsync(TimeSpan.FromSeconds(1), CancellationToken.None), Is.True);
            Assert.That(client1.IsInitialized, Is.True);
            Assert.That(client1.IsAuthorized, Is.True);

            var client2 = GetClient();
            Assert.That(await client2.ConnectAsync(TimeSpan.FromSeconds(1), CancellationToken.None), Is.False);
            Assert.That(client2.IsInitialized, Is.False);
            Assert.That(client2.IsAuthorized, Is.False);

            await client1.Disconnect();

            Thread.Sleep(500);

            var client3 = GetClient();
            Assert.That(await client3.ConnectAsync(TimeSpan.FromSeconds(1), CancellationToken.None), Is.True);
            Assert.That(client3.IsInitialized, Is.True);
            Assert.That(client3.IsAuthorized, Is.True);
        }

        [Test]
        public async Task TooManyClientsMultiClientsAllowedConnectionTest()
        {
            var server = GetServer(3);
            server.StartWaitingForConnection();

            var client1 = GetClient();

            Assert.That(await client1.ConnectAsync(TimeSpan.FromSeconds(1), CancellationToken.None), Is.True);
            Assert.That(client1.IsInitialized, Is.True);
            Assert.That(client1.IsAuthorized, Is.True);

            var client2 = GetClient();
            Assert.That(await client2.ConnectAsync(TimeSpan.FromSeconds(1), CancellationToken.None), Is.True);
            Assert.That(client2.IsInitialized, Is.True);
            Assert.That(client2.IsAuthorized, Is.True);

            var client3 = GetClient();
            Assert.That(await client3.ConnectAsync(TimeSpan.FromSeconds(1), CancellationToken.None), Is.True);
            Assert.That(client3.IsInitialized, Is.True);
            Assert.That(client3.IsAuthorized, Is.True);

            var client4 = GetClient();
            Assert.That(await client4.ConnectAsync(TimeSpan.FromSeconds(1), CancellationToken.None), Is.False);
            Assert.That(client4.IsInitialized, Is.False);
            Assert.That(client4.IsAuthorized, Is.False);

            await client2.Disconnect();

            var client5 = GetClient();
            Assert.That(await client5.ConnectAsync(TimeSpan.FromSeconds(1), CancellationToken.None), Is.True);
            Assert.That(client5.IsInitialized, Is.True);
            Assert.That(client5.IsAuthorized, Is.True);
        }

        [Test]
        public async Task SingleClientBasicCommunicationTest()
        {
            var server = GetServer(1);
            server.StartWaitingForConnection();

            var client = GetClient();

            Assert.That(await client.ConnectAsync(TimeSpan.Zero, CancellationToken.None), Is.True);
            Assert.That(client.IsInitialized, Is.True);
            Assert.That(client.IsAuthorized, Is.True);

            var request = new WitComRequest
            {
                MethodName = "Test"
            };

            WitComResponse? response = await client.SendRequest(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Status, Is.EqualTo(CommunicationStatus.Ok));
            Assert.That(response.Data, Is.EqualTo("Test"));
        }

        [Test]
        public async Task MultiClientBasicCommunicationTest()
        {
            var server = GetServer(11);
            server.StartWaitingForConnection();

            var clients = new List<WitComClient>
            {
                GetClient(),
                GetClient(),
                GetClient(),
                GetClient(),
                GetClient(),
                GetClient(),
                GetClient(),
                GetClient(),
                GetClient(),
                GetClient(),
            };

            var start = DateTime.Now;

            Parallel.For(0, clients.Count, index =>
            {
                var client = clients[index];
                Assert.That(client.ConnectAsync(TimeSpan.Zero, CancellationToken.None).Result, Is.True);

                Assert.That(client.IsInitialized, Is.True);
                Assert.That(client.IsAuthorized, Is.True);
            });
            var end = DateTime.Now;
            Console.WriteLine($"Clients initialization duration: {(end - start).TotalMilliseconds} ms");

            start = DateTime.Now;
            Parallel.For(0, clients.Count, index =>
            {
                var client = clients[index];
                WitComResponse? response = client.SendRequest(new WitComRequest { MethodName = $"Test{index}" }).Result;
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Status, Is.EqualTo(CommunicationStatus.Ok));
                Assert.That(response.Data, Is.EqualTo($"Test{index}"));
            });

            end = DateTime.Now;
            Console.WriteLine($"Clients communication duration: {(end - start).TotalMilliseconds} ms");
        }

        private WitComServer GetServer(int maxNumberOfClients, [CallerMemberName] string callerMember = "")
        {
            var random = new Random(callerMember.GetHashCode());
            var serverTransport = new TcpServerTransportFactory(new TcpServerTransportOptions
            {
                Port = random.Next(100, 300),
                MaxNumberOfClients = maxNumberOfClients
            });
            return new WitComServer(serverTransport, 
                new EncryptorServerFactory<EncryptorServerGeneral>(), 
                new AccessTokenValidatorStatic(AUTHORIZATION_TOKEN), 
                new MessageSerializerJson(), 
                new ValueConverterJson(), 
                new MockRequestProcessor());
        }

        private WitComClient GetClient([CallerMemberName] string callerMember = "")
        {
            var random = new Random(callerMember.GetHashCode());
            var clientTransport = new TcpClientTransport(new TcpClientTransportOptions
            {
                Port = random.Next(100, 300),
                Host = "127.0.0.1"
            });

            return new WitComClient(clientTransport, 
                new EncryptorClientGeneral(),
                new AccessTokenProviderStatic(AUTHORIZATION_TOKEN),
                new MessageSerializerJson(),
                new ValueConverterJson());
        }
    }
}

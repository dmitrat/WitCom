﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using OutWit.Common.Json;
using OutWit.Common.MemoryPack;
using OutWit.Common.MessagePack;
using OutWit.Common.ProtoBuf;
using OutWit.Communication.Exceptions;
using OutWit.Communication.Interfaces;
using OutWit.Communication.Model;
using OutWit.Communication.Processors;
using OutWit.Communication.Serializers;
using OutWit.Communication.Server.Authorization;
using OutWit.Communication.Server.Discovery;
using OutWit.Communication.Server.Encryption;

namespace OutWit.Communication.Server
{
    public static class WitComServerBuilder
    {
        #region Constants

        private const string DEFAULT_DISCOVERY_IP = "239.255.255.250";
        private const int DEFAULT_DISCOVERY_PORT = 3702;

        #endregion

        public static WitComServer Build(Action<WitComServerBuilderOptions> optionsBuilder)
        {
            var options = new WitComServerBuilderOptions();
            optionsBuilder(options);

            return Build(options);
        }

        public static WitComServer Build(WitComServerBuilderOptions options)
        {
            if (options.TransportFactory == null)
                throw new WitComException("Transport cannot be empty");

            if (options.RequestProcessor == null)
                throw new WitComException("Request processor cannot be empty");

            return new WitComServer(options.TransportFactory, options.EncryptorFactory, options.TokenValidator, options.ParametersSerializer, options.MessageSerializer,
                options.RequestProcessor, options.DiscoveryServer, options.Logger, options.Timeout, options.Name, options.Description);
        }

        #region Processor

        public static WitComServerBuilderOptions WithRequestProcessor(this WitComServerBuilderOptions me, IRequestProcessor requestProcessor)
        {
            me.RequestProcessor = requestProcessor;
            return me;
        }

        public static WitComServerBuilderOptions WithService<TService>(this WitComServerBuilderOptions me, TService service, bool isStrongAssemblyMatch = true)
            where TService: class
        {
            me.RequestProcessor = new RequestProcessor<TService>(service, isStrongAssemblyMatch);
            return me;
        }

        public static WitComServerBuilderOptions WithService<TService>(this WitComServerBuilderOptions me, bool isStrongAssemblyMatch = true)
            where TService : class, new ()
        {
            me.RequestProcessor = new RequestProcessor<TService>(new TService(), isStrongAssemblyMatch);
            return me;
        }

        public static WitComServerBuilderOptions WithService<TService>(this WitComServerBuilderOptions me, Func<TService> serviceBuilder, bool isStrongAssemblyMatch = true)
            where TService : class, new()
        {
            me.RequestProcessor = new RequestProcessor<TService>(serviceBuilder(), isStrongAssemblyMatch);
            return me;
        }

        #endregion

        #region Authorization

        public static WitComServerBuilderOptions WithAccessTokenValidator(this WitComServerBuilderOptions me, IAccessTokenValidator validator)
        {
            me.TokenValidator = validator;
            return me;
        }

        public static WitComServerBuilderOptions WithAccessToken(this WitComServerBuilderOptions me, string accessToken)
        {
            me.TokenValidator = new AccessTokenValidatorStatic(accessToken);
            return me;
        }

        public static WitComServerBuilderOptions WithoutAuthorization(this WitComServerBuilderOptions me)
        {
            me.TokenValidator = new AccessTokenValidatorPlain();
            return me;
        }

        #endregion

        #region Encryption

        public static WitComServerBuilderOptions WithEncryptor(this WitComServerBuilderOptions me, IEncryptorServerFactory encryptorFactory)
        {
            me.EncryptorFactory = encryptorFactory;
            return me;
        }

        public static WitComServerBuilderOptions WithEncryption(this WitComServerBuilderOptions me)
        {
            me.EncryptorFactory = new EncryptorServerFactory<EncryptorServerGeneral>();
            return me;
        }

        public static WitComServerBuilderOptions WithoutEncryption(this WitComServerBuilderOptions me)
        {
            me.EncryptorFactory = new EncryptorServerFactory<EncryptorServerPlain>();
            return me;
        }

        #endregion

        #region Serialization

        public static WitComServerBuilderOptions WithMessageSerializer(this WitComServerBuilderOptions me, IMessageSerializer serializer)
        {
            me.MessageSerializer = serializer;
            return me;
        }

        public static WitComServerBuilderOptions WithParametersSerializer(this WitComServerBuilderOptions me, IMessageSerializer serializer)
        {
            me.ParametersSerializer = serializer;
            return me;
        }

        #endregion

        #region Json

        public static WitComServerBuilderOptions WithJson(this WitComServerBuilderOptions me)
        {
            me.ParametersSerializer = new MessageSerializerJson();
            return me;
        }

        public static WitComServerBuilderOptions WithJson(this WitComServerBuilderOptions me, Action<JsonOptions> options)
        {
            JsonUtils.Register(options);

            return me.WithJson();
        }

        #endregion

        #region MessagePack

        public static WitComServerBuilderOptions WithMessagePack(this WitComServerBuilderOptions me)
        {
            me.ParametersSerializer = new MessageSerializerMessagePack();
            return me;
        }

        public static WitComServerBuilderOptions WithMessagePack(this WitComServerBuilderOptions me, Action<MessagePackOptions> options)
        {
            MessagePackUtils.Register(options);
            
            return me.WithMessagePack();
        }
        
        #endregion

        #region MemoryPack

        public static WitComServerBuilderOptions WithMemoryPack(this WitComServerBuilderOptions me)
        {
            me.ParametersSerializer = new MessageSerializerMemoryPack();
            return me;
        }

        public static WitComServerBuilderOptions WithMemoryPack(this WitComServerBuilderOptions me, Action<MemoryPackOptions> options)
        {
            MemoryPackUtils.Register(options);
            return me.WithMemoryPack();
        }
        
        #endregion

        #region ProtoBuf

        public static WitComServerBuilderOptions WithProtoBuf(this WitComServerBuilderOptions me)
        {
            me.ParametersSerializer = new MessageSerializerProtoBuf();
            return me;
        }

        public static WitComServerBuilderOptions WithProtoBuf(this WitComServerBuilderOptions me, Action<ProtoBufOptions> options)
        {
            ProtoBufUtils.Register(options);
            
            return me.WithProtoBuf();
        }
        
        #endregion

        #region Logger

        public static WitComServerBuilderOptions WithLogger(this WitComServerBuilderOptions me, ILogger logger)
        {
            me.Logger = logger;
            return me;
        }

        #endregion

        #region Discovery

        public static WitComServerBuilderOptions WithName(this WitComServerBuilderOptions me, string name)
        {
            me.Name = name;
            return me;
        }

        public static WitComServerBuilderOptions WithDescription(this WitComServerBuilderOptions me, string description)
        {
            me.Description = description;
            return me;
        }

        public static WitComServerBuilderOptions WithDiscovery(this WitComServerBuilderOptions me, DiscoveryServerOptions options)
        {
            me.DiscoveryServer = new DiscoveryServer(options);
            return me;
        }

        public static WitComServerBuilderOptions WithDiscovery(this WitComServerBuilderOptions me, IPAddress ipAddress, int port = DEFAULT_DISCOVERY_PORT, TimeSpan? period = null)
        {
            var mode = (period != null && period.Value != TimeSpan.Zero)
                ? DiscoveryServerMode.Continuous 
                : DiscoveryServerMode.StartStop;

            return me.WithDiscovery(new DiscoveryServerOptions
            {
                IpAddress = ipAddress,
                Port = port,
                Mode = mode,
                Period = period
            });
        }

        public static WitComServerBuilderOptions WithDiscovery(this WitComServerBuilderOptions me, string ipAddress, int port = DEFAULT_DISCOVERY_PORT, TimeSpan? period = null)
        {
            if(!IPAddress.TryParse(ipAddress, out var address))
                throw new WitComException("Invalid IP address");

            return me.WithDiscovery(address, port, period);
        }

        public static WitComServerBuilderOptions WithDiscovery(this WitComServerBuilderOptions me, HostInfo hostInfo, TimeSpan? period = null)
        {
            if (hostInfo.Port == null || hostInfo.Port <= 0) 
                throw new WitComException("Invalid port");

            return me.WithDiscovery(hostInfo.Host, hostInfo.Port.Value, period);
        }

        public static WitComServerBuilderOptions WithDiscovery(this WitComServerBuilderOptions me, TimeSpan? period = null)
        {
            return me.WithDiscovery(DEFAULT_DISCOVERY_IP, DEFAULT_DISCOVERY_PORT, period);
        }

        #endregion

        #region Timeout

        public static WitComServerBuilderOptions WithTimeout(this WitComServerBuilderOptions me, TimeSpan timeout)
        {
            me.Timeout = timeout;
            return me;
        }

        #endregion
    }
}

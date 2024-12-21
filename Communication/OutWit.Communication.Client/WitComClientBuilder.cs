﻿using System;
using Castle.DynamicProxy;
using OutWit.Communication.Client.Authorization;
using OutWit.Communication.Client.Encryption;
using OutWit.Communication.Converters;
using OutWit.Communication.Exceptions;
using OutWit.Communication.Interceptors;
using OutWit.Communication.Interfaces;
using OutWit.Communication.Serializers;

namespace OutWit.Communication.Client
{
    public static class WitComClientBuilder
    {
        public static WitComClient Build(Action<WitComClientBuilderOptions> optionsBuilder)
        {
            var options = new WitComClientBuilderOptions();
            optionsBuilder(options);

            if (options.Transport == null)
                throw new WitComException("Transport cannot be empty");

            return new WitComClient(options.Transport, options.Encryptor, options.TokenProvider, options.Serializer,
                options.Converter);
        }

        public static TService GetService<TService>(this WitComClient me, bool strongAssemblyMatch = true)
            where TService : class
        {
            var proxyGenerator = new ProxyGenerator();
            var interceptor = new RequestInterceptor(me, strongAssemblyMatch);

            return proxyGenerator.CreateInterfaceProxyWithoutTarget<TService>(interceptor);
        }

        #region Authorization

        public static WitComClientBuilderOptions WithAccessTokenProvider(this WitComClientBuilderOptions me, IAccessTokenProvider provider)
        {
            me.TokenProvider = provider;
            return me;
        }

        public static WitComClientBuilderOptions WithAccessToken(this WitComClientBuilderOptions me, string accessToken)
        {
            me.TokenProvider = new AccessTokenProviderStatic(accessToken);
            return me;
        }

        public static WitComClientBuilderOptions WithoutAuthorization(this WitComClientBuilderOptions me)
        {
            me.TokenProvider = new AccessTokenProviderPlain();
            return me;
        }

        #endregion

        #region Encryption

        public static WitComClientBuilderOptions WithEncryptor(this WitComClientBuilderOptions me, IEncryptorClient encryptor)
        {
            me.Encryptor = encryptor;
            return me;
        }

        public static WitComClientBuilderOptions WithEncryption(this WitComClientBuilderOptions me)
        {
            me.Encryptor = new EncryptorClientGeneral();
            return me;
        }

        public static WitComClientBuilderOptions WithoutEncryption(this WitComClientBuilderOptions me)
        {
            me.Encryptor = new EncryptorClientPlain();
            return me;
        }

        #endregion

        #region Serialization

        public static WitComClientBuilderOptions WithConverter(this WitComClientBuilderOptions me, IValueConverter converter)
        {
            me.Converter = converter;
            return me;
        }


        public static WitComClientBuilderOptions WithSerializer(this WitComClientBuilderOptions me, IMessageSerializer serializer)
        {
            me.Serializer = serializer;
            return me;
        }


        public static WitComClientBuilderOptions WithJson(this WitComClientBuilderOptions me)
        {
            me.Converter = new ValueConverterJson();
            me.Serializer = new MessageSerializerJson();
            return me;
        }

        public static WitComClientBuilderOptions WithMessagePack(this WitComClientBuilderOptions me)
        {
            me.Converter = new ValueConverterMessagePack();
            me.Serializer = new MessageSerializerMessagePack();
            return me;
        }

        #endregion
    }
}
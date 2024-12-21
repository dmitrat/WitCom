﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutWit.Communication.Client.Authorization;
using OutWit.Communication.Client.Encryption;
using OutWit.Communication.Converters;
using OutWit.Communication.Interfaces;
using OutWit.Communication.Serializers;

namespace OutWit.Communication.Client
{
    public class WitComClientBuilderOptions
    {
        #region Constructors

        public WitComClientBuilderOptions()
        {
            Serializer = new MessageSerializerJson();
            Converter = new ValueConverterJson();
            Encryptor = new EncryptorClientPlain();
            TokenProvider = new AccessTokenProviderPlain();
        }

        #endregion

        #region Properties

        public ITransportClient? Transport { get; set; }

        public IMessageSerializer Serializer { get; set; }

        public IValueConverter Converter { get; set; }

        public IEncryptorClient Encryptor { get; set; }

        public IAccessTokenProvider TokenProvider { get; set; }

        #endregion
    }
}

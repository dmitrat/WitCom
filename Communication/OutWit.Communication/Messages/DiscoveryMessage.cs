﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessagePack;
using System.Runtime.Serialization;
using OutWit.Common.Abstract;
using OutWit.Common.Collections;
using OutWit.Common.Values;

namespace OutWit.Communication.Messages
{
    [MessagePackObject]
    [DataContract]
    public class DiscoveryMessage : ModelBase
    {
        #region Functions

        public override string ToString()
        {
            return $"Timestamp: {Timestamp}, Type: {Type}, Transport: {Transport}, ServiceName: {ServiceName}";
        }

        #endregion

        #region Model Base

        public override bool Is(ModelBase modelBase, double tolerance = 1E-07)
        {
            if (modelBase is not DiscoveryMessage message)
                return false;

            return (Timestamp?.UtcDateTime).Is(message.Timestamp?.UtcDateTime) &&
                   Type.Is(message.Type) &&
                   ServiceName.Is(message.ServiceName) &&
                   ServiceDescription.Is(message.ServiceDescription) &&
                   Transport.Is(message.Transport) &&
                   Data.Is(message.Data);
        }

        public override DiscoveryMessage Clone()
        {
            return new DiscoveryMessage
            {
                Timestamp = Timestamp,
                Type = Type,
                ServiceName = ServiceName,
                ServiceDescription = ServiceDescription,
                Transport = Transport,
                Data = Data?.ToDictionary(x => x.Key, x => x.Value)
            };
        }

        #endregion

        #region Properties

        [Key(0)]
        [DataMember]
        public DateTimeOffset? Timestamp { get; set; }
        
        [Key(1)]
        [DataMember]
        public DiscoveryMessageType? Type { get; set; }

        [Key(2)]
        [DataMember]
        public string? ServiceName { get; set; }

        [Key(3)]
        [DataMember]
        public string? ServiceDescription { get; set; }

        [Key(4)]
        [DataMember]
        public string? Transport { get; set; }

        [Key(5)]
        [DataMember]
        public Dictionary<string, string>? Data { get; set; }

        #endregion

    }

    public enum DiscoveryMessageType
    {
        Hello= 0,
        Heartbeat,
        Goodbye
    }
}

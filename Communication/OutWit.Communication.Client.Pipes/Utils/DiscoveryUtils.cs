﻿using System;
using OutWit.Communication.Exceptions;
using OutWit.Communication.Messages;

namespace OutWit.Communication.Client.Pipes.Utils
{
    public static class DiscoveryUtils
    {
        private const string TRANSPORT = "NamedPipe";

        public static bool IsNamedPipe(this DiscoveryMessage me)
        {
            return me.Transport == TRANSPORT;
        }

        public static string PipeName(this DiscoveryMessage me)
        {
            if(!me.IsNamedPipe())
                throw new WitComException($"Wrong transport type: {me.Transport}");

            if(me.Data == null)
                throw new WitComException($"Discovery data is empty", new ArgumentNullException(nameof(me.Data)));

            if (!me.Data.TryGetValue(nameof(NamedPipeClientTransportOptions.PipeName), out var pipeName))
                throw new WitComException($"Cannot find parameter value for parameter: {nameof(NamedPipeClientTransportOptions.PipeName)}");

            return pipeName;
        }

        public static WitComClientBuilderOptions WithNamedPipe(this WitComClientBuilderOptions me, DiscoveryMessage message)
        {
            return me.WithNamedPipe(message.PipeName());
        }

        public static WitComClientBuilderOptions WithNamedPipe(this WitComClientBuilderOptions me, string server, DiscoveryMessage message)
        {
            return me.WithNamedPipe(server, message.PipeName());
        }
    }
}

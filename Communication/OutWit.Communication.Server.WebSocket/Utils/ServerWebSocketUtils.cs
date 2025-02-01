﻿using System;
using OutWit.Communication.Model;

namespace OutWit.Communication.Server.WebSocket.Utils
{
    public static class ServerWebSocketUtils
    {
        private const int DEFAULT_BUFFER_SIZE = 4096;

        public static WitComServerBuilderOptions WithWebSocket(this WitComServerBuilderOptions me, WebSocketServerTransportOptions options)
        {
            me.TransportFactory = new WebSocketServerTransportFactory(options);
            return me;
        }

        public static WitComServerBuilderOptions WithWebSocket(this WitComServerBuilderOptions me, string url, int maxNumberOfClients, int bufferSize = DEFAULT_BUFFER_SIZE)
        {
            return me.WithWebSocket(new WebSocketServerTransportOptions
            {
                Url = url,
                MaxNumberOfClients = maxNumberOfClients,
                BufferSize = bufferSize
            });
        }

        public static WitComServerBuilderOptions WithWebSocket(this WitComServerBuilderOptions me, HostInfo hostInfo, int bufferSize = DEFAULT_BUFFER_SIZE)
        {
            return me.WithWebSocket(new WebSocketServerTransportOptions
            {
                Url = hostInfo.BuildConnection(true),
                MaxNumberOfClients = 1,
                BufferSize = bufferSize
            });
        }
    }
}

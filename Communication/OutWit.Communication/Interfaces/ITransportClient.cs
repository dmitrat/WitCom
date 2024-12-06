﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutWit.Communication.Interfaces
{
    public interface ITransportClient : ITransport
    {
        Task<bool> ConnectAsync(TimeSpan timeout, CancellationToken cancellationToken);

        Task<bool> ConnectAsync(CancellationToken cancellationToken);


        Task<bool> ReconnectAsync(TimeSpan timeout, CancellationToken cancellationToken);

        Task<bool> ReconnectAsync(CancellationToken cancellationToken);


        Task Disconnect();
    }
}

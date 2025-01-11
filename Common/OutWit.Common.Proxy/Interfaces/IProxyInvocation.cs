﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OutWit.Common.Proxy.Interfaces
{
    public interface IProxyInvocation
    {
        public string MethodName { get; }

        public object[] Parameters { get; }

        public string[] ParameterTypes { get; }

        public object ReturnValue { get; set; }

        public string ReturnType { get; }
    }
}

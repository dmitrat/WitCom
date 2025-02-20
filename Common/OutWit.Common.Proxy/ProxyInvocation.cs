﻿using System;
using OutWit.Common.Abstract;
using OutWit.Common.Collections;
using OutWit.Common.Proxy.Interfaces;
using OutWit.Common.Values;

namespace OutWit.Common.Proxy
{
    public class ProxyInvocation : ModelBase, IProxyInvocation
    {
        #region ModelBase

        public override bool Is(ModelBase modelBase, double tolerance = 1E-07)
        {
            if (modelBase is not ProxyInvocation hostInfo)
                return false;

            return MethodName.Is(hostInfo.MethodName) &&
                   Parameters.Is(hostInfo.Parameters) &&
                   ParametersTypes.Is(hostInfo.ParametersTypes) &&
                   GenericArguments.Is(hostInfo.GenericArguments) &&
                   HasReturnValue.Equals(hostInfo.HasReturnValue) &&
                   ReturnValue.Equals(hostInfo.ReturnValue) &&
                   ReturnType.Is(hostInfo.ReturnType) &&
                   ReturnsTask.Is(hostInfo.ReturnsTask) &&
                   ReturnsTaskWithResult.Is(hostInfo.ReturnsTaskWithResult) &&
                   TaskResultType.Is(hostInfo.TaskResultType);
        }

        public override ModelBase Clone()
        {
            return new ProxyInvocation
            {
                MethodName = MethodName,
                Parameters = Parameters,
                ParametersTypes = ParametersTypes,
                GenericArguments = GenericArguments,
                HasReturnValue = HasReturnValue,
                ReturnValue = ReturnValue,
                ReturnType = ReturnType,
                ReturnsTask = ReturnsTask,
                ReturnsTaskWithResult = ReturnsTaskWithResult,
                TaskResultType = TaskResultType
            };
        }

        #endregion

        #region Properties

        public string MethodName { get; set; }

        public object[] Parameters { get; set; }

        public string[] ParametersTypes { get; set; }

        public string[] GenericArguments { get; set; }

        public bool HasReturnValue { get; set; }

        public object ReturnValue { get; set; }

        public string ReturnType { get; set; }

        public bool ReturnsTask { get; set; }

        public bool ReturnsTaskWithResult { get; set; }

        public string TaskResultType { get; set; }

        #endregion
    }
}

﻿namespace Mx.NET.SDK.Core.Domain.Values
{
    public interface IBinaryType
    {
        TypeValue Type { get; }

        T ValueOf<T>() where T : IBinaryType;

        T ToObject<T>();

        string ToJson();
    }
}

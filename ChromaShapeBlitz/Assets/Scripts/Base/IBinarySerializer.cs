using System;
using System.Collections;
using System.Collections.Generic;

public interface IBinarySerializer<T>
{
    IEnumerator<T> Read(Action<T> callback);
    IEnumerator Write(T data);
}
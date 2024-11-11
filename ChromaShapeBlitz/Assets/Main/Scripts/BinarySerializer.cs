using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public abstract class BinarySerializer<T> where T : class
{
    protected abstract string BinaryPath { get; }

    /// <summary>
    /// Tells if an existing binary formatted data exists
    /// </summary>
    /// <returns>bool</returns>
    public bool FileExists => File.Exists(BinaryPath);

    /// <summary>
    /// Deserialize binary data back into its original form,
    /// such as Data Structures and Unity Objects.
    /// </summary>
    /// <param name="callback">Output to</param>
    /// <returns>Deserialized data</returns>
    public virtual IEnumerator Read(Action<T> callback, int overrideBeginStatusCode = -1, int overrideEndStatusCode = -1)
    {
        if (overrideBeginStatusCode == -1)
            ActionStatusNotifier.NotifyObserver( StatusCodes.BEGIN_READ_BINARY_SERIALIZE );
        else
            ActionStatusNotifier.NotifyObserver( overrideBeginStatusCode );

        var formatter = new BinaryFormatter();
        var stream    = new FileStream(BinaryPath, FileMode.Open);
        var data      = formatter.Deserialize(stream) as T;
        
        stream.Close();
        callback?.Invoke(data);

        if (overrideEndStatusCode == -1)
            ActionStatusNotifier.NotifyObserver( StatusCodes.DONE_READ_BINARY_SERIALIZE);
        else
            ActionStatusNotifier.NotifyObserver(overrideEndStatusCode);

        yield return null;
    }

    // public virtual IEnumerator Read<R>(Action<R> callback) where R : class
    // {
    //     var formatter = new BinaryFormatter();
    //     var stream    = new FileStream(BinaryPath, FileMode.Open);
    //     var data      = formatter.Deserialize(stream) as R;
        
    //     stream.Close();
    //     callback?.Invoke(data);

    //     yield return null;
    // }

    public virtual IEnumerator Write(T data, Action<T> callback)
    {
        var formatter = new BinaryFormatter();
        var stream    = new FileStream(BinaryPath, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();

       callback?.Invoke(data);

        yield return null;
    }

    public virtual IEnumerator Write(T data, int overrideBeginStatusCode = -1, int overrideEndStatusCode = -1)
    {
        var formatter = new BinaryFormatter();
        var stream    = new FileStream(BinaryPath, FileMode.Create);

        if (overrideBeginStatusCode == -1)
            ActionStatusNotifier.NotifyObserver( StatusCodes.BEGIN_WRITE_BINARY_SERIALIZE );
        else
            ActionStatusNotifier.NotifyObserver( overrideBeginStatusCode );

        formatter.Serialize(stream, data);
        stream.Close();

        if (overrideEndStatusCode == -1)
            ActionStatusNotifier.NotifyObserver( StatusCodes.DONE_WRITE_BINARY_SERIALIZE );
        else
            ActionStatusNotifier.NotifyObserver( overrideEndStatusCode );

        yield return null;
    }

    // public virtual IEnumerator Write(T data, Action<T> callback)
    // {
    //     var formatter = new BinaryFormatter();
    //     var stream    = new FileStream(BinaryPath, FileMode.Create);

    //     formatter.Serialize(stream, data);
    //     stream.Close();

    //     callback?.Invoke(data);

    //     yield return null;
    // }
}
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public abstract class BinarySerializer<T> where T : class
{
    /// <summary>
    /// Tells if an existing binary formatted data exists
    /// </summary>
    /// <returns>bool</returns>
    public bool FileExists => File.Exists(Constants.SavePath);

    /// <summary>
    /// Deserialize binary data back into its original form,
    /// such as Data Structures and Unity Objects.
    /// </summary>
    /// <param name="callback">Output to</param>
    /// <returns>Deserialized data</returns>
    public virtual IEnumerator Read(Action<T> callback)
    {
        var formatter = new BinaryFormatter();
        var stream    = new FileStream(Constants.SavePath, FileMode.Open);
        var data      = formatter.Deserialize(stream) as T;
        
        stream.Close();
        callback?.Invoke(data);

        yield return null;
    }

    public virtual IEnumerator Write(T data)
    {
        var formatter = new BinaryFormatter();
        var stream    = new FileStream(Constants.SavePath, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();

        yield return null;
    }
}
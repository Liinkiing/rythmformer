using System;
using FullSerializer;

public static class JSONSerializer {
    private static readonly fsSerializer Serializer = new fsSerializer();

    public static string Serialize(Type type, object value) {
        fsData data;
        Serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

        return fsJsonPrinter.CompressedJson(data);
    }

    public static T Deserialize<T>(Type type, string serializedState) where T : class
    {
        fsData data = fsJsonParser.Parse(serializedState);

        object deserialized = null;
        Serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

        return deserialized as T;
    }
}
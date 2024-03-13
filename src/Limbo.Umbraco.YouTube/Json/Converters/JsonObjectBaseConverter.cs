using System;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Newtonsoft;

namespace Limbo.Umbraco.YouTube.Json.Converters;

/// <summary>
/// JSON converter for serializing instances of <see cref="JsonObjectBase"/>.
/// </summary>
public class JsonObjectBaseConverter : JsonConverter {

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
        switch (value) {
            case null:
                writer.WriteNull();
                return;
            case JsonObjectBase obj: {
                if (obj.JObject == null) {
                    writer.WriteNull();
                } else {
                    obj.JObject.WriteTo(writer);
                }
                return;
            }
            default:
                throw new Exception("Unsupported type: " + value.GetType());
        }
    }

    /// <inheritdoc />
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override bool CanConvert(Type objectType) {
        return objectType == typeof(JsonObjectBase);
    }

}
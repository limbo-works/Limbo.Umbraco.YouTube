using System;
using Limbo.Umbraco.YouTube.Models.Videos.Intermediary;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.YouTube.Json.Newtonsoft.Converters;

public class YouTubeJsonConverter : JsonConverter {

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {

        if (value is YouTubeIntermediaryVideoDetails youtube) {
            string data = youtube.Data.JObject.ToString(Formatting.None);
            JObject details = new() { { "_data", data } };
            details.WriteTo(writer);
            return;
        }

        throw new NotImplementedException();

    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType) {
        return false;
    }

}
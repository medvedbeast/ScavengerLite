using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorableConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(Storable).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject item = JObject.Load(reader);
        var type = System.Type.GetType(item["$type"].Value<string>());
        return item.ToObject(type);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        JToken t = JToken.FromObject(value);

        if (t.Type != JTokenType.Object)
        {
            t.WriteTo(writer);
        }
        else
        {
            JObject o = (JObject)t;
            o.AddFirst(new JProperty("$type", JValue.CreateString(value.GetType().ToString())));
            o.WriteTo(writer);
        }
    }
}

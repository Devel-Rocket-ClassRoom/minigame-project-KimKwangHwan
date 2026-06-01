using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Vector2Converter : JsonConverter<Vector2>
{
    public override void WriteJson(JsonWriter w, Vector2 v, JsonSerializer s)
    {
        w.WriteStartObject();
        w.WritePropertyName("x"); w.WriteValue(v.x);
        w.WritePropertyName("y"); w.WriteValue(v.y);
        w.WriteEndObject();
    }

    public override Vector2 ReadJson(JsonReader r, Type t, Vector2 ev, bool has, JsonSerializer s)
    {
        if (r.TokenType == JsonToken.Null) return Vector2.zero;
        var o = JObject.Load(r);
        return new Vector2(o["x"]?.Value<float>() ?? 0f,
                           o["y"]?.Value<float>() ?? 0f);
    }
}

public class Vector3Converter : JsonConverter<Vector3>
{
    public override void WriteJson(JsonWriter w, Vector3 v, JsonSerializer s)
    {
        w.WriteStartObject();
        w.WritePropertyName("x"); w.WriteValue(v.x);
        w.WritePropertyName("y"); w.WriteValue(v.y);
        w.WritePropertyName("z"); w.WriteValue(v.z);
        w.WriteEndObject();
    }

    public override Vector3 ReadJson(JsonReader r, Type t, Vector3 ev, bool has, JsonSerializer s)
    {
        if (r.TokenType == JsonToken.Null) return Vector3.zero;
        var o = JObject.Load(r);
        return new Vector3(o["x"]?.Value<float>() ?? 0f,
                           o["y"]?.Value<float>() ?? 0f,
                           o["z"]?.Value<float>() ?? 0f);
    }
}

public class QuaternionConverter : JsonConverter<Quaternion>
{
    public override void WriteJson(JsonWriter w, Quaternion q, JsonSerializer s)
    {
        w.WriteStartObject();
        w.WritePropertyName("x"); w.WriteValue(q.x);
        w.WritePropertyName("y"); w.WriteValue(q.y);
        w.WritePropertyName("z"); w.WriteValue(q.z);
        w.WritePropertyName("w"); w.WriteValue(q.w);
        w.WriteEndObject();
    }

    public override Quaternion ReadJson(JsonReader r, Type t, Quaternion ev, bool has, JsonSerializer s)
    {
        if (r.TokenType == JsonToken.Null) return Quaternion.identity;
        var o = JObject.Load(r);
        return new Quaternion(o["x"]?.Value<float>() ?? 0f,
                              o["y"]?.Value<float>() ?? 0f,
                              o["z"]?.Value<float>() ?? 0f,
                              o["w"]?.Value<float>() ?? 1f);  // 기본값 identity
    }
}
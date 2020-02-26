using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WrapAround.Logic.Util
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            
            var x = reader.GetSingle();
            var y = reader.GetSingle();

            return new Vector2(x,y);

        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            ReadOnlySpan<char> xBuff = stackalloc char[]{'X'};
            ReadOnlySpan<char> yBuff = stackalloc char[]{'Y'};

            writer.WriteStartObject();
            writer.WriteNumber(xBuff, value.X);
            writer.WriteNumber(yBuff, value.Y);
            writer.WriteEndObject();

            writer.Flush();

        }
    }
}

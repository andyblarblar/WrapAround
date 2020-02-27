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
            float x = 0, y = 0;

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        var propName = reader.GetString();
                        if (propName.Equals("X", StringComparison.InvariantCulture))
                        {
                            reader.Read();
                            x = reader.GetSingle();
                        }
                        else if (propName.Equals("Y", StringComparison.InvariantCulture))
                        {
                            reader.Read();
                            y = reader.GetSingle();
                        }
                        break;
                }
            }

            return new Vector2(x, y);

        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {

            writer.WriteStartObject();
            writer.WriteNumber(stackalloc char[] { 'X' }, value.X);
            writer.WriteNumber(stackalloc char[] { 'Y' }, value.Y);
            writer.WriteEndObject();

            writer.Flush();

        }
    }
}

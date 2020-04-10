using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

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

                    case JsonTokenType.EndObject:
                        return new Vector2(x, y);
                }
            }

            return new Vector2(x, y);
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            
            writer.WriteStartObject();

            switch (value.X)
            {
                //write null for incompatible values
                case float.PositiveInfinity:
                    writer.WriteNull(stackalloc char[] { 'X' });
                    break;

                case float.NegativeInfinity:
                    writer.WriteNull(stackalloc char[] { 'X' });
                    break;

                case float.NaN:
                    writer.WriteNull(stackalloc char[] { 'X' });
                    break;
                
                default:
                    writer.WriteNumber(stackalloc char[] { 'X' }, value.X);
                    break;
            } 
            
            switch (value.Y)
            {
                //write null for incompatible values
                case float.PositiveInfinity:
                    writer.WriteNull(stackalloc char[] { 'Y' });
                    break;

                case float.NegativeInfinity:
                    writer.WriteNull(stackalloc char[] { 'Y' });
                    break;

                case float.NaN:
                    writer.WriteNull(stackalloc char[] { 'Y' });
                    break;
                
                default:
                    writer.WriteNumber(stackalloc char[] { 'Y' }, value.Y);
                    break;
            }

            writer.WriteEndObject();

            writer.Flush();
           

        }
    }
}

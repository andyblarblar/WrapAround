using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using NUnit.Framework;
using WrapAround.Logic.Util;

namespace WrapAround.Tests
{
    public class SerializationTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void HitboxSerializesProperly()
        {
            var con = new JsonSerializerOptions();
            con.Converters.Add(new Vector2Converter());

            var hitbox = new Hitbox(new Vector2(10f, 20f), new Vector2(30f, 10f));

            Console.WriteLine(JsonSerializer.Serialize(hitbox,con));
            var bl = JsonSerializer.Deserialize<Hitbox>(JsonSerializer.Serialize(hitbox,con),con);

            Console.WriteLine(bl.TopLeft);
            Assert.AreEqual("<10, 20>", bl.TopLeft);

            Assert.Pass();
        } 
        
        [Test]
        public void Vector2SerializesProperly()
        {
            var vec = new Vector2(10, 20);
            var con = new JsonSerializerOptions();
            con.Converters.Add(new Vector2Converter());

            var json = JsonSerializer.Serialize(vec, con);
            Console.WriteLine(json);

            var vec2 = JsonSerializer.Deserialize<Vector2>(json, con);
            Console.WriteLine(vec2);
            Assert.AreEqual(vec2, vec);

            Assert.Pass();
        }

    }
}
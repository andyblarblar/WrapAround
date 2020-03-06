using NUnit.Framework;
using System;
using System.Numerics;
using System.Text.Json;
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
            var hitbox = new Hitbox(new Vector2(10f, 20f), new Vector2(30f, 10f));

            var ser = JsonSerializer.Serialize(hitbox);
            Console.WriteLine(hitbox.TopLeft);
            Console.WriteLine(ser);
            var des = JsonSerializer.Deserialize<Hitbox>(ser);

            Console.WriteLine(des.TopLeft);
            Assert.AreEqual(hitbox, des);

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
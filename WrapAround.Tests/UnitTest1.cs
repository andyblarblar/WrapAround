using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using NUnit.Framework;
using WrapAround.Logic.Util;

namespace WrapAround.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var hitbox = new Hitbox(new Vector2(10f, 20f), new Vector2(30f, 10f));

            Console.WriteLine(JsonSerializer.Serialize(hitbox));
            var bl = JsonSerializer.Deserialize<Hitbox>(JsonSerializer.Serialize(hitbox));

            Console.WriteLine(bl.TopLeft);
            Assert.AreEqual("<10, 20>", bl.TopLeft);

            Assert.Pass();
        }
    }
}
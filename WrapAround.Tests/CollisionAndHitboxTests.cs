using NUnit.Framework;
using System.Numerics;
using WrapAround.Logic.Util;

namespace WrapAround.Tests
{
    public class CollisionAndHitboxTests
    {
        [SetUp]
        public void Setup()
        {
            hitbox1 = new Hitbox(new Vector2(-10, 10), new Vector2(1, 10));
            hitbox2 = new Hitbox(new Vector2(10, 0), new Vector2(30, -10));
            hitbox3 = new Hitbox(new Vector2(-20, 10), new Vector2(10, 0));
            hitbox4 = new Hitbox(new Vector2(-10, 10), new Vector2(1, 10));
        }

        private Hitbox hitbox1;
        private Hitbox hitbox2;
        private Hitbox hitbox3;
        private Hitbox hitbox4;

        [Test]
        public void EqualHitboxesAreEqual()
        {
            Assert.True(hitbox1 == hitbox4);

            Assert.Pass();
        }

        [Test]
        public void UnequalHitboxesAreUnequal()
        {
            Assert.False(hitbox1 == hitbox2);

            Assert.Pass();
        }

        [Test]
        public void CollidingHitboxesCollide()
        {
            Assert.True(hitbox1.IsCollidingWith(in hitbox4));

            Assert.Pass();
        }



    }
}

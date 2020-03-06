using NUnit.Framework;
using System.Numerics;
using WrapAround.Logic.Entities;
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
            goalZone = new Hitbox(new Vector2(0,0),new Vector2(10, 703));
            ball = new Hitbox(new Vector2(5,351),new Vector2(15, 361));
        }

        private Hitbox hitbox1;
        private Hitbox hitbox2;
        private Hitbox hitbox3;
        private Hitbox hitbox4;
        private Hitbox goalZone;
        private Hitbox ball;

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
        
        [Test]
        public void BallGoalZoneSimulation()
        {
            Assert.True(goalZone.IsCollidingWith(in ball));

            Assert.Pass();
        }



    }
}

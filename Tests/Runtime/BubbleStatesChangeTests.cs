using System;
using NUnit.Framework;
using UnityEngine;
using YodeGroup.BubbleShooter.GameElements.Bubbles;
using Object = UnityEngine.Object;

namespace YodeGroup.BubbleShooter.Tests
{
    public class BubbleStatesChangeTests
    {
        private SimpleBubble _bubble;
        private GameObject _bubbleGameObject;

        [SetUp]
        public void Setup()
        {
            _bubbleGameObject = new GameObject(nameof(SimpleBubble));
            _bubble = _bubbleGameObject.AddComponent<SimpleBubble>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_bubbleGameObject);
            _bubble = null;
            _bubbleGameObject = null;
        }

        [Test]
        public void ShootBubbleAndSetTarget()
        {
            _bubble.SetCurrentForThrow();
            _bubble.Throw();
            _bubble.SetTargetToDestroy();
        }

        [Test]
        public void ShootBubbleAndSetNone()
        {
            _bubble.SetCurrentForThrow();
            _bubble.Throw();
            _bubble.ResetState();
        }

        [Test]
        public void SetTargetFromDefault()
        {
            _bubble.SetTargetToDestroy();
        }

        [Test]
        public void ExceptionOnShootNextBubble()
        {
            _bubble.SetNextForThrow();
            Assert.Throws<InvalidOperationException>(() => _bubble.Throw());
        }

        [Test]
        public void ExceptionOnShootTargetBubble()
        {
            _bubble.SetTargetToDestroy();
            Assert.Throws<InvalidOperationException>(() => _bubble.Throw());
        }

        [Test]
        public void ExceptionOnShootDefaultBubble()
        {
            Assert.Throws<InvalidOperationException>(() => _bubble.Throw());
        }
    }
}
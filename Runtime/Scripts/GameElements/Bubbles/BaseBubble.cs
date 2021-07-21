using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YodeGroup.BubbleShooter.Map;

namespace YodeGroup.BubbleShooter.GameElements.Bubbles
{
    public abstract class BaseBubble : GameElement
    {
        public BubbleState CurrentState { get; private set; } = BubbleState.None;

        public abstract TypeBubble Type { get; }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out BaseBubble bubble))
            {
                OnCollisionWithBubble(bubble, collision);
                return;
            }

            if (collision.gameObject.TryGetComponent(out Wall wall) && CurrentState == BubbleState.Thrown)
            {
                OnCollisionWithWallOnShoot(wall, collision);
                return;
            }

            if (collision.gameObject.TryGetComponent<Border>(out Border border))
            {
                OnCollisionWithBorder(border, collision);
            }
        }

        public abstract void DestroyBubble();

        public abstract bool MatchType(BaseBubble compared);
        public event Action Thrown;

        public void Throw()
        {
            ChangeState(BubbleState.Thrown, BubbleState.CurrentForThrow);
            Thrown?.Invoke();
        }

        public void SetCurrentForThrow() =>
            ChangeState(BubbleState.CurrentForThrow, BubbleState.None, BubbleState.NextForThrow);

        public void SetNextForThrow() =>
            ChangeState(BubbleState.NextForThrow, BubbleState.None, BubbleState.CurrentForThrow);

        public void SetTargetToDestroy() =>
            ChangeState(BubbleState.TargetForDestroy, BubbleState.None, BubbleState.Thrown);

        public void ResetState() =>
            ChangeState(BubbleState.None, BubbleState.Thrown, BubbleState.TargetForDestroy);

        protected virtual void OnFirstTouchBubble(BaseBubble bubble)
        {
        }

        private void ChangeState(BubbleState newState, params BubbleState[] statesAllowedForTransition)
        {
            if (CurrentState == newState) return;

            if (statesAllowedForTransition?.Any(state => CurrentState == state) == false)
                throw new InvalidOperationException();

            CurrentState = newState;
        }

        private void OnCollisionWithBubble(BaseBubble bubble, Collision2D collision2D)
        {
            if (CurrentState == BubbleState.Thrown)
            {
                OnFirstTouchBubble(bubble);
                List<BaseBubble> chain = GetChain(this);
                if (chain.Count > 2)
                    chain.ForEach(baseBubble => baseBubble.SetTargetToDestroy());
                else
                    chain.ForEach(baseBubble => baseBubble.ResetState());

                chain.ForEach(baseBubble => baseBubble.FreezeBubble());
            }
        }

        private List<BaseBubble> GetChain(BaseBubble startBubble)
        {
            var chain = new List<BaseBubble> {startBubble};
            bool BubbleNotInChain(BaseBubble baseBubble) => chain.Contains(baseBubble) == false;

            for (var i = 0; i < chain.Count; i++)
            {
                chain.AddRange(chain[i].GetNearMatchTypeBubbles().Where(BubbleNotInChain));
            }

            return chain;
        }

        private List<BaseBubble> GetNearMatchTypeBubbles()
        {
            Collider2D[] circleList = Physics2D.OverlapCircleAll(transform.position, 1.5f);

            List<BaseBubble> list = circleList
                .Select(nearCollider => nearCollider.transform.GetComponent<BaseBubble>())
                .Where(bubble => bubble).Distinct().ToList();

            list.Remove(this);
            List<BaseBubble> bubbles = list.Where(MatchType).ToList();
            return bubbles;
        }

        private void OnCollisionWithWallOnShoot(Wall wall, Collision2D collision)
        {
            ContactPoint2D contact = collision.contacts.First();
            Quaternion rotate = Quaternion.AngleAxis(180, contact.normal);
            Vector2 relativeVelocity = rotate * contact.relativeVelocity;
            Rigidbody2D.velocity = contact.relativeVelocity.magnitude * relativeVelocity.normalized;
        }

        private void OnCollisionWithBorder(Border border, Collision2D collision2D)
        {
            FreezeBubble();
            ResetState();
        }

        private void FreezeBubble()
        {
            Rigidbody2D.velocity = Vector2.zero;
            Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX;
            Rigidbody2D.freezeRotation = true;
            Rigidbody2D.gravityScale = -0.25f;
            Rigidbody2D.mass = 0.1f;
        }
    }
}
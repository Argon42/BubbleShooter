using UnityEngine;
using YodeGroup.BubbleShooter.GameElements;
using YodeGroup.BubbleShooter.GameElements.Bubbles;

namespace YodeGroup.BubbleShooter.Map
{
    public class Border : MonoBehaviour
    {
        [SerializeField] private bool isBottomBorder;
        [SerializeField] private bool isTopBorder;
        [SerializeField] private Transform parentForHeroes;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            GameObject collisionGameObject = collision.gameObject;

            if (collisionGameObject.TryGetComponent<BaseBubble>(out BaseBubble bubble))
            {
                bool isCollisionWithTarget = bubble.CurrentState == BubbleState.TargetForDestroy;
                bool isTopBorder = gameObject.CompareTag("top_border");

                if (isCollisionWithTarget && isTopBorder)
                    OnBubbleCollision(bubble);
            }

            var collectableItem = collisionGameObject.GetComponent<CollectableItem>();
            bool isBottomBorder = gameObject.CompareTag("bottom_border");

            if (collectableItem && isBottomBorder)
                OnHeroCollision(collectableItem);
        }

        private void OnHeroCollision(CollectableItem hero)
        {
            Transform parentForHeroes = GameObject.Find("StageBuilderController").transform;
            hero.transform.SetParent(parentForHeroes);

            // TODO Collect items
            // BubblesController.Instance.numberTargetsSaved++;

            hero.CircleCollider2D.enabled = false;
            hero.Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }

        private void OnBubbleCollision(BaseBubble bubble)
        {
            Rigidbody2D bubbleRigidbody = bubble.Rigidbody2D;
            bubbleRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
            bubbleRigidbody.freezeRotation = true;
            bubbleRigidbody.gravityScale = -0.25f;
            bubbleRigidbody.mass = 0.1f;
        }
    }
}
using UnityEngine;
using UnityEngine.Events;
using YodeGroup.BubbleShooter.GameElements.Bubbles;

namespace YodeGroup.BubbleShooter.Map
{
    public class EndlessBorder : MonoBehaviour
    {
        [SerializeField] private UnityEvent collideWithBubble;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent(out BaseBubble bubble) && bubble.CurrentState == BubbleState.None)
            {
                collideWithBubble?.Invoke();
            }
        }

        public event UnityAction CollideWithBubble
        {
            add => collideWithBubble.AddListener(value);
            remove => collideWithBubble.RemoveListener(value);
        }
    }
}
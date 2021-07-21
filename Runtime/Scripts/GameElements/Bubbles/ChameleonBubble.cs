using UnityEngine;
using YodeGroup.BubbleShooter.Map;

namespace YodeGroup.BubbleShooter.GameElements.Bubbles
{
    [RequireComponent(typeof(Animator))]
    public class ChameleonBubble : BaseBubble
    {
        private static readonly int DestroyBoolParameter = Animator.StringToHash("destroy");

        private Animator _animator;
        private TypeBubble _type = TypeBubble.AnyColor;
        public override TypeBubble Type => _type;

        public Animator Animator
        {
            get
            {
                if (_animator == false)
                    _animator = GetComponent<Animator>();
                return _animator;
            }
        }

        public override void DestroyBubble()
        {
            Destroy(this);
            Destroy(CircleCollider2D);
            Destroy(Rigidbody2D);

            gameObject.name = "dead_bubble";

            Animator.enabled = true;
            Animator.SetBool(DestroyBoolParameter, true);

            Destroy(gameObject, 0.5f);
        }

        public override bool MatchType(BaseBubble compared) => compared.Type == Type;

        protected override void OnFirstTouchBubble(BaseBubble bubble)
        {
            _type = bubble.Type;
            var selfRenderer = GetComponent<SpriteRenderer>();
            var otherRenderer = bubble.GetComponent<SpriteRenderer>();

            Animator.enabled = false;
            selfRenderer.sprite = otherRenderer.sprite;
        }
    }
}
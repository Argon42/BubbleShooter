using UnityEngine;
using YodeGroup.BubbleShooter.Map;

namespace YodeGroup.BubbleShooter.GameElements.Bubbles
{
    [RequireComponent(typeof(Animator))]
    public class SimpleBubble : BaseBubble
    {
        private static readonly int DestroyBoolParameter = Animator.StringToHash("destroy");
        [SerializeField] private TypeBubble type;

        public override TypeBubble Type => type;

        public override void DestroyBubble()
        {
            Destroy(this);
            Destroy(CircleCollider2D);
            Destroy(Rigidbody2D);

            gameObject.name = "dead_bubble";

            GetComponent<Animator>().SetBool(DestroyBoolParameter, true);

            Destroy(gameObject, 0.5f);
        }

        public override bool MatchType(BaseBubble compared) => compared.Type == Type;
    }
}
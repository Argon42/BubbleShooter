using UnityEngine;

namespace YodeGroup.BubbleShooter.GameElements
{
    [RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
    public abstract class GameElement : MonoBehaviour
    {
        private CircleCollider2D _circleCollider2D;
        private Rigidbody2D _rigidbody2D;

        public CircleCollider2D CircleCollider2D
        {
            get
            {
                if (_circleCollider2D == false)
                    _circleCollider2D = GetComponent<CircleCollider2D>();
                return _circleCollider2D;
            }
        }

        public Rigidbody2D Rigidbody2D
        {
            get
            {
                if (_rigidbody2D == false)
                    _rigidbody2D = GetComponent<Rigidbody2D>();
                return _rigidbody2D;
            }
        }
    }
}
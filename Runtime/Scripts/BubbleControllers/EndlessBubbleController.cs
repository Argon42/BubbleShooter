using UnityEngine;

namespace YodeGroup.BubbleShooter
{
    public class EndlessBubbleController : BaseBubblesController
    {
        private const float HeightOfNewBubbles = 2.7f;
        private const float HighestBubblePositionForNewBubbles = 9.573f;

        [SerializeField] private float speed;

        private int maxBubbleHeight = 0;

        private void OnDrawGizmosSelected()
        {
            float size = 8;
            Vector3 up = Vector3.up * HighestBubblePositionForNewBubbles;
            Gizmos.DrawLine(
                up - Vector3.right * size,
                up + Vector3.right * size
            );
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                up + Vector3.up * HeightOfNewBubbles - Vector3.right * size,
                up + Vector3.up * HeightOfNewBubbles + Vector3.right * size
            );
        }

        protected override Vector3 GetTargetStagePosition(Vector3 stagePosition, float lowestBubble)
        {
            return new Vector3(0, -100f, 0);
        }

        protected override float GetSpeed(float lowestBubble)
        {
            float height = lowestBubble - maxBubbleHeight;
            if (height > 0)
                return height * height + speed;

            return speed;
        }

        protected override void OnLateUpdate()
        {
            if (GamePaused)
                return;

            if (FindTheHighestBubble() < HighestBubblePositionForNewBubbles)
            {
                TopBorder.position += Vector3.up * HeightOfNewBubbles;
                Factory.CreateNewBubbles();
            }
        }
    }
}
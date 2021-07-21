using UnityEngine;

namespace YodeGroup.BubbleShooter
{
    public class SimpleBubbleController : BaseBubblesController
    {
        [SerializeField] private float bubbleLine;
        [SerializeField] private float speed;

        private void OnDrawGizmosSelected()
        {
            float size = 8;
            Gizmos.DrawLine(
                Vector3.up * bubbleLine - Vector3.right * size,
                Vector3.up * bubbleLine + Vector3.right * size
            );
        }

        protected override Vector3 GetTargetStagePosition(Vector3 stagePosition, float lowestBubble)
        {
            return stagePosition + Vector3.up * (bubbleLine - lowestBubble);
        }

        protected override float GetSpeed(float lowestBubble)
        {
            return speed;
        }
    }
}
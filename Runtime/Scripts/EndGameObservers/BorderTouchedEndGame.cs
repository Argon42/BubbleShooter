using UnityEngine;
using YodeGroup.BubbleShooter.Map;

namespace YodeGroup.BubbleShooter.EndGameObservers
{
    public class BorderTouchedEndGame : BaseEndGameObserver
    {
        [SerializeField] private EndlessBorder endlessBorder;

        private bool _serviceEnabled;

        public override void StartService()
        {
            _serviceEnabled = true;
            endlessBorder.CollideWithBubble += EndlessBorderOnCollideWithBubble;
        }

        public override void StopService()
        {
            _serviceEnabled = false;
            endlessBorder.CollideWithBubble -= EndlessBorderOnCollideWithBubble;
        }

        public override void Pause() => _serviceEnabled = false;

        public override void Resume() => _serviceEnabled = true;

        private void EndlessBorderOnCollideWithBubble()
        {
            if (_serviceEnabled == false)
                return;

            EndGame();
        }
    }
}
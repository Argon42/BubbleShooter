using UnityEngine;
using YodeGroup.BubbleShooter.GameElements.Bubbles;

namespace YodeGroup.BubbleShooter
{
    public class BubbleContainer : GameService
    {
        private const float SmoothTime = 0.1f;

        [SerializeField] private Transform positionForSpawn;
        [SerializeField] private Transform positionForSelectedBall;
        [SerializeField] private Transform positionForNextBall;
        private BubbleFactory _factory;

        private bool _serviceEnabled;
        private Shooter _shooter;

        public BaseBubble CurrentBubble { get; private set; }
        public BaseBubble NextBubble { get; private set; }

        private void Update()
        {
            if (CurrentBubble)
                MoveToPosition(CurrentBubble.transform, positionForSelectedBall.position);
            if (NextBubble)
                MoveToPosition(NextBubble.transform, positionForNextBall.position);
        }

        public override void StartService() => _serviceEnabled = true;

        public override void StopService() => _serviceEnabled = false;

        public override void Pause() => _serviceEnabled = false;

        public override void Resume() => _serviceEnabled = true;

        public void Init(BubbleFactory factory, Shooter shooter)
        {
            if (_shooter)
                _shooter.Shoot -= ShootBubbleHandler;

            _factory = factory;
            _shooter = shooter;

            _shooter.Shoot += ShootBubbleHandler;
        }

        public void ChangeBubbleWithPlace()
        {
            if (_serviceEnabled == false) return;
            if (CurrentBubble == null || NextBubble == null) return;

            CurrentBubble.SetNextForThrow();
            NextBubble.SetCurrentForThrow();

            (CurrentBubble, NextBubble) = (NextBubble, CurrentBubble);
        }

        public void RecreateBubbles()
        {
            if (CurrentBubble) Destroy(CurrentBubble.gameObject);
            if (NextBubble) Destroy(NextBubble.gameObject);

            CurrentBubble = _factory.CreateNewShootBubble();
            CurrentBubble.SetCurrentForThrow();
            NextBubble = _factory.CreateNewShootBubble();
            NextBubble.SetNextForThrow();

            CurrentBubble.CircleCollider2D.enabled = false;
            NextBubble.CircleCollider2D.enabled = false;
        }

        private void ShootBubbleHandler()
        {
            CurrentBubble.CircleCollider2D.enabled = true;
            CurrentBubble = NextBubble;
            CurrentBubble.SetCurrentForThrow();
            NextBubble = _factory.CreateNewShootBubble();
            NextBubble.SetNextForThrow();
            NextBubble.transform.position = positionForSpawn.position;
            NextBubble.CircleCollider2D.enabled = false;
        }

        private void MoveToPosition(Transform bubble, Vector3 target) =>
            bubble.position = Vector3.Lerp(bubble.position, target, SmoothTime);
    }
}
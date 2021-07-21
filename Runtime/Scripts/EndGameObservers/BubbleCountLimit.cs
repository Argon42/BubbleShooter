using System;
using UnityEngine;
using UnityEngine.Events;

namespace YodeGroup.BubbleShooter.EndGameObservers
{
    public class BubbleCountLimit : BaseEndGameObserver
    {
        [SerializeField] private Shooter shooter;
        [SerializeField] private int defaultCount = 15;
        [SerializeField] private UnityEvent<int> bubbleCountChanged;

        private int _countOfBubbles;
        private bool _serviceEnabled;

        public override void StartService()
        {
            SetupLimitOfBubbles(defaultCount);
            shooter.Shoot += ShooterOnShoot;
            _serviceEnabled = true;
        }

        public override void StopService()
        {
            shooter.Shoot -= ShooterOnShoot;
            _serviceEnabled = false;
        }

        public override void Pause() => _serviceEnabled = false;

        public override void Resume() => _serviceEnabled = true;

        public event UnityAction<int> BubbleCountChanged
        {
            add => bubbleCountChanged.AddListener(value);
            remove => bubbleCountChanged.RemoveListener(value);
        }

        public void SetupLimitOfBubbles(int countOfBubbles)
        {
            if (countOfBubbles <= 0)
                throw new ArgumentOutOfRangeException(nameof(countOfBubbles), "is negative or zero");

            defaultCount = countOfBubbles;
            _countOfBubbles = countOfBubbles;
            bubbleCountChanged?.Invoke(_countOfBubbles);
        }

        private void ShooterOnShoot()
        {
            if (_serviceEnabled == false)
                return;

            _countOfBubbles = Mathf.Clamp(_countOfBubbles - 1, 0, int.MaxValue);
            bubbleCountChanged?.Invoke(_countOfBubbles);

            if (_countOfBubbles == 0)
            {
                shooter.Pause();
                Invoke(nameof(EndGame), 5);
            }
        }
    }
}
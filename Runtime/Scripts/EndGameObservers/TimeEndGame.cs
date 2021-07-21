using UnityEngine;
using UnityEngine.Events;

namespace YodeGroup.BubbleShooter.EndGameObservers
{
    public class TimeEndGame : BaseEndGameObserver
    {
        [SerializeField] private float time = 90;
        [SerializeField] private UnityEvent<float> remainingTimeChanged;

        private bool _serviceEnabled;
        private float _time;

        private void Update()
        {
            if (_serviceEnabled == false)
                return;

            _time += Time.deltaTime;
            remainingTimeChanged?.Invoke(Mathf.Clamp(time - _time, 0, float.MaxValue));

            if (_time >= time)
                EndGame();
        }

        public override void StartService()
        {
            _serviceEnabled = true;
            _time = 0;
        }

        public override void StopService()
        {
            _serviceEnabled = false;
        }

        public override void Pause() => _serviceEnabled = false;

        public override void Resume() => _serviceEnabled = true;

        public event UnityAction<float> RemainingTimeChanged
        {
            add => remainingTimeChanged.AddListener(value);
            remove => remainingTimeChanged.RemoveListener(value);
        }
    }
}
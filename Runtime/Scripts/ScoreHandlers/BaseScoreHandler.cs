using UnityEngine;
using UnityEngine.Events;

namespace YodeGroup.BubbleShooter.ScoreHandlers
{
    public abstract class BaseScoreHandler : MonoBehaviour
    {
        [SerializeField] private UnityEvent<int> scoreChanged;

        private int _score;

        public int Score
        {
            get => _score;
            protected set
            {
                _score = value;
                scoreChanged?.Invoke(_score);
            }
        }

        public event UnityAction<int> ScoreChanged
        {
            add => scoreChanged.AddListener(value);
            remove => scoreChanged.RemoveListener(value);
        }

        public void ResetScore() => Score = 0;
    }
}
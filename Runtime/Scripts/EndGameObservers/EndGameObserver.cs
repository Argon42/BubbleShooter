using UnityEngine;
using UnityEngine.Events;

namespace YodeGroup.BubbleShooter.EndGameObservers
{
    public abstract class BaseEndGameObserver : GameService
    {
        [SerializeField] private UnityEvent gameEnded;

        public event UnityAction GameEnded
        {
            add => gameEnded.AddListener(value);
            remove => gameEnded.RemoveListener(value);
        }

        protected void EndGame()
        {
            gameEnded?.Invoke();
        }
    }
}
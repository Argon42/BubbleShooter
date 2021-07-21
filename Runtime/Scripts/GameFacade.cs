using System.Collections.Generic;
using UnityEngine;
using YodeGroup.BubbleShooter.EndGameObservers;

namespace YodeGroup.BubbleShooter
{
    public class GameFacade : MonoBehaviour
    {
        [SerializeField] private BubbleContainer bubbleContainer;
        [SerializeField] private BaseBubblesController bubblesController;
        [SerializeField] private BubbleFactory factory;
        [SerializeField] private Shooter shooter;
        [SerializeField] private List<BaseEndGameObserver> endGameObservers;

        private void Awake()
        {
            bubbleContainer.Init(factory, shooter);
            bubblesController.Init(factory);
            factory.Init(this);
            shooter.Init(bubbleContainer);
        }

        public void StartGame()
        {
            endGameObservers.ForEach(endGameObserver => endGameObserver.GameEnded += StopGame);
            GetServices().ForEach(service => service.StartService());

            bubblesController.StartNewLevel();
            bubbleContainer.RecreateBubbles();
        }

        public void StopGame()
        {
            endGameObservers.ForEach(endGameObserver => endGameObserver.GameEnded -= StopGame);
            GetServices().ForEach(service => service.StopService());
        }

        public void Restart()
        {
            StopGame();
            StartGame();
        }

        public void SwapBubbles() => bubbleContainer.ChangeBubbleWithPlace();

        public void Pause() => GetServices().ForEach(service => service.Pause());

        public void Resume() => GetServices().ForEach(service => service.Resume());

        private List<GameService> GetServices()
        {
            var list = new List<GameService> {bubblesController, bubbleContainer, shooter};
            list.AddRange(endGameObservers);
            return list;
        }
    }
}
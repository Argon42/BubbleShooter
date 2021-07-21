using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YodeGroup.BubbleShooter.GameElements.Bubbles;
using YodeGroup.BubbleShooter.Map;

namespace YodeGroup.BubbleShooter.EndGameObservers
{
    public class CollectElementsEndGame : BaseEndGameObserver
    {
        [SerializeField] private BaseBubblesController bubblesController;
        [SerializeField] private CollectElementsDataset defaultDataset;

        private Dictionary<TypeBubble, int> _requiredBubbles;
        private bool _serviceEnabled;

        public override void StartService()
        {
            if (_requiredBubbles == null)
                SetTrackingDataset(defaultDataset);
            bubblesController.OnBubblesDestroy += BubblesControllerOnOnBubblesDestroy;
        }

        public override void StopService()
        {
            bubblesController.OnBubblesDestroy -= BubblesControllerOnOnBubblesDestroy;
            _requiredBubbles = null;
        }

        public override void Pause() => _serviceEnabled = false;

        public override void Resume() => _serviceEnabled = true;

        public void SetTrackingDataset(CollectElementsDataset dataset)
        {
            _requiredBubbles =
                dataset.ListOfTrackingData.ToDictionary(data => data.BubbleType, data => data.Count);
        }

        private void BubblesControllerOnOnBubblesDestroy(List<BaseBubble> bubbles)
        {
            if (_serviceEnabled == false || _requiredBubbles == null)
                return;

            Dictionary<TypeBubble, int> destroyedBubbles = bubbles.GroupBy(bubble => bubble.Type)
                .Where(grouping => _requiredBubbles.ContainsKey(grouping.Key))
                .ToDictionary(grouping => grouping.Key, grouping => grouping.Count());

            foreach (KeyValuePair<TypeBubble, int> pair in destroyedBubbles)
                _requiredBubbles[pair.Key] = Mathf.Clamp(_requiredBubbles[pair.Key] - pair.Value, 0, int.MaxValue);

            if (_requiredBubbles.All(pair => pair.Value == 0))
                EndGame();
        }
    }
}
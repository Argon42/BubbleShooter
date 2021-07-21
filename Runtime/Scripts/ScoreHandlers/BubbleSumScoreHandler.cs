using System.Collections.Generic;
using UnityEngine;
using YodeGroup.BubbleShooter.GameElements.Bubbles;

namespace YodeGroup.BubbleShooter.ScoreHandlers
{
    public class BubbleSumScoreHandler : BaseScoreHandler
    {
        [SerializeField] private int bubbleMultiplier = 10;
        [SerializeField] private BaseBubblesController controller;

        private void OnEnable()
        {
            controller.OnBubblesDestroy += OnBubblesDestroy;
        }

        private void OnDisable()
        {
            controller.OnBubblesDestroy -= OnBubblesDestroy;
        }

        private void OnBubblesDestroy(List<BaseBubble> bubbles)
        {
            Score += bubbles.Count * bubbleMultiplier;
        }
    }
}
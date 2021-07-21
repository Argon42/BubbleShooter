using System;
using UnityEngine;
using YodeGroup.BubbleShooter.Map;

namespace YodeGroup.BubbleShooter.EndGameObservers
{
    [Serializable]
    public class CollectElementData
    {
        [SerializeField] private TypeBubble bubbleType;
        [SerializeField] private int count;

        public TypeBubble BubbleType => bubbleType;
        public int Count => count;
    }
}
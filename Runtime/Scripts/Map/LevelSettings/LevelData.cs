using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YodeGroup.BubbleShooter.GameElements;
using YodeGroup.BubbleShooter.GameElements.Bubbles;

namespace YodeGroup.BubbleShooter.Map
{
    public class LevelData : ScriptableObject
    {
        public string stageName;
        public BubbleInfo[] bubblesInfo;
        public HeroInfo[] heroesInfo;

        private static Vector3[] FindAllPosition(IEnumerable<Component> bubbles) =>
            bubbles.Select(bubble => bubble.transform.position).ToArray();

        public void Init(string name)
        {
            BaseBubble[] bubbles = FindObjectsOfType<BaseBubble>();
            CollectableItem[] heroes = FindObjectsOfType<CollectableItem>();

            stageName = name;

            Vector3[] allBubblesPosition = FindAllPosition(bubbles);
            Vector3[] allHeroesPosition = FindAllPosition(heroes);

            bubblesInfo = new BubbleInfo[allBubblesPosition.Length];
            heroesInfo = new HeroInfo[allHeroesPosition.Length];

            for (var i = 0; i < allBubblesPosition.Length; i++)
            {
                bubblesInfo[i] = new BubbleInfo
                {
                    position = allBubblesPosition[i],
                    bubbleType = bubbles[i].Type
                };
            }

            for (var i = 0; i < allHeroesPosition.Length; i++)
            {
                heroesInfo[i] = new HeroInfo
                {
                    position = allHeroesPosition[i],
                    heroType = heroes[i].Type
                };
            }
        }
    }

    [Serializable]
    public class BubbleInfo
    {
        public Vector3 position;
        public TypeBubble bubbleType;
    }

    [Serializable]
    public class HeroInfo
    {
        public Vector3 position;
        public TypeHero heroType;
    }
}
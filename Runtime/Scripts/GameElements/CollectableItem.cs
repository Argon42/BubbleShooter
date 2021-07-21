using UnityEngine;
using YodeGroup.BubbleShooter.Map;

namespace YodeGroup.BubbleShooter.GameElements
{
    public class CollectableItem : GameElement
    {
        [SerializeField] private TypeHero type;

        public TypeHero Type => type;
    }
}
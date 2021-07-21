using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YodeGroup.BubbleShooter.GameElements;
using YodeGroup.BubbleShooter.GameElements.Bubbles;
using YodeGroup.BubbleShooter.Map;

namespace YodeGroup.BubbleShooter
{
    public class BubbleFactory : MonoBehaviour
    {
        [Header("-----------------------------------------------------------------"),
         Header("                           GAME OBJECTS                          "),
         Header("-----------------------------------------------------------------")]
        public BaseBubble[] bubbles;

        public BaseBubble[] bubblesForShoot;
        public CollectableItem[] heroes;
        public List<GameObject> bubblesClone;

        [SerializeField] private Transform bubblesParent;
        [SerializeField] private Transform shootBubblesParent;
        [SerializeField] private LevelData stageData;

        private GameFacade _gameFacade;

        public void Init(GameFacade gameFacade)
        {
            _gameFacade = gameFacade;
        }

        public BaseBubble CreateNewShootBubble()
        {
            int randomIndex = Random.Range(0, bubblesForShoot.Length);
            BaseBubble bubblePrefab = bubblesForShoot[randomIndex];

            BaseBubble bubble = Instantiate(bubblePrefab, shootBubblesParent);
            bubble.Thrown += ChangeParent;

            void ChangeParent()
            {
                bubble.transform.SetParent(bubblesParent);
                bubble.Thrown -= ChangeParent;
            }

            bubble.Rigidbody2D.gravityScale = 0f;
            bubble.Rigidbody2D.constraints = RigidbodyConstraints2D.None;
            bubble.Rigidbody2D.freezeRotation = true;

            return bubble;
        }

        public void CreateLevel()
        {
            if (stageData == false) return;

            ResetBubbles();

            foreach (BubbleInfo bubbleData in stageData.bubblesInfo)
            {
                bool WhereMatchBubbleType(BaseBubble b) =>
                    b.Type == bubbleData.bubbleType;

                BaseBubble prefab = bubbles.FirstOrDefault(WhereMatchBubbleType) ?? bubbles.First();
                BaseBubble clone = Instantiate(prefab, bubbleData.position, Quaternion.identity);
                clone.transform.SetParent(bubblesParent);
            }

            foreach (HeroInfo itemData in stageData.heroesInfo)
            {
                bool WhereMatchItemType(CollectableItem b) =>
                    b.Type == itemData.heroType;

                CollectableItem prefab = heroes.FirstOrDefault(WhereMatchItemType) ?? heroes.First();
                CollectableItem clone = Instantiate(prefab, itemData.position, Quaternion.identity, bubblesParent);
            }
        }

        public IEnumerable<BaseBubble> CreateNewBubbles()
        {
            Transform clone = Instantiate(bubblesClone[Random.Range(0, bubblesClone.Count)]).transform;

            List<BaseBubble> newBubbles = clone.GetComponentsInChildren<BaseBubble>().ToList();
            newBubbles.ForEach(bubble => bubble.transform.SetParent(bubblesParent));

            Destroy(clone.gameObject);
            return newBubbles;
        }

        public void ResetBubbles()
        {
            FindObjectsOfType<BaseBubble>().ToList().ForEach(bubble => Destroy(bubble.gameObject));
            FindObjectsOfType<CollectableItem>().ToList().ForEach(item => Destroy(item.gameObject));
        }
    }
}
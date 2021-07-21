using System;
using System.Collections.Generic;
using UnityEngine;

namespace YodeGroup.BubbleShooter.EndGameObservers
{
    [Serializable]
    public class CollectElementsDataset
    {
        [SerializeField] private List<CollectElementData> listOfTrackingData = new List<CollectElementData>();

        public List<CollectElementData> ListOfTrackingData => listOfTrackingData;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace YodeGroup.BubbleShooter.Samples
{
    public class Clock : MonoBehaviour
    {
        [SerializeField] private Transform hourHandSeconds;
        [SerializeField] private Transform hourHandMinutes;
        [SerializeField] private UnityEvent<string> remainingTimeChanged;

        public void SetRemainingTime(float time)
        {
            float seconds = time % 60;
            float minutes = time / 60;

            SetRotation(hourHandSeconds, seconds / 60);
            SetRotation(hourHandMinutes, minutes / 60);

            remainingTimeChanged?.Invoke($"{minutes:00}:{seconds:00}");
        }

        private void SetRotation(Transform hourHand, float normaliseTime) =>
            hourHand.localEulerAngles = Vector3.back * (normaliseTime * 360);
    }
}
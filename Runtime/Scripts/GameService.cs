using UnityEngine;

namespace YodeGroup.BubbleShooter
{
    public abstract class GameService : MonoBehaviour
    {
        public abstract void StartService();
        public abstract void StopService();
        public abstract void Pause();
        public abstract void Resume();
    }
}
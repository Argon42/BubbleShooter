using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using YodeGroup.BubbleShooter.GameElements.Bubbles;
using YodeGroup.BubbleShooter.Utility;

namespace YodeGroup.BubbleShooter
{
    public abstract class BaseBubblesController : GameService
    {
        private const int MinCountForDestroy = 2;
        private const float DelayBeforeDestroy = 0.1f;

        [SerializeField] private Transform topBorder;
        [SerializeField] private Transform stage;
        [SerializeField] private UnityEvent<List<BaseBubble>> onBubblesDestroy;

        private BaseBubble[] _bubbles;
        private Coroutine _coroutine;

        protected bool GamePaused { get; private set; } = true;

        public BubbleFactory Factory { get; private set; }

        public Transform TopBorder => topBorder;

        private void Awake()
        {
            FindObjectsOfType<BubbleMapEditorCollider>()?.ToList()
                .ForEach(editorCollider => Destroy(editorCollider.gameObject));

            TopBorder.gameObject.SetActive(true);
        }

        private void LateUpdate()
        {
            if (GamePaused) return;

            float lowestBubble = FindTheLowestBubble();
            float speed = GetSpeed(lowestBubble) * Time.deltaTime;
            stage.position = Vector3.MoveTowards(stage.position, GetTargetStagePosition(stage.position, lowestBubble),
                speed);
            OnLateUpdate();

            _bubbles = FindObjectsOfType<BaseBubble>();
            bool anyBubbleIsTarget = _bubbles.Any(bubble => bubble.CurrentState == BubbleState.TargetForDestroy);

            if (anyBubbleIsTarget && _coroutine == null)
                _coroutine = StartCoroutine(DelayBeforeProcessing());
        }

        protected abstract Vector3 GetTargetStagePosition(Vector3 stagePosition, float lowestBubble);

        protected abstract float GetSpeed(float lowestBubble);

        public override void StartService() => GamePaused = false;
        public override void StopService() => GamePaused = true;
        public override void Pause() => GamePaused = true;
        public override void Resume() => GamePaused = false;

        public event UnityAction<List<BaseBubble>> OnBubblesDestroy
        {
            add => onBubblesDestroy.AddListener(value);
            remove => onBubblesDestroy.RemoveListener(value);
        }

        public void Init(BubbleFactory factory)
        {
            Factory = factory;
        }

        public void ResetLevel() => Factory.ResetBubbles();

        public async void StartNewLevel()
        {
            ResetLevel();
            GamePaused = false;
            stage.position = Vector3.zero;

            Factory.CreateLevel();
            await Task.Yield();

            float highestBubble = FindTheHighestBubble();
            if (highestBubble > 7.5f)
                TopBorder.position = new Vector3(TopBorder.position.x, highestBubble + 1.75f, 0f);
        }

        protected virtual void OnLateUpdate()
        {
        }

        protected float FindTheLowestBubble() => FindObjectsOfType<BaseBubble>()
            .Where(bubble => bubble.CurrentState == BubbleState.None)
            .Min(bubble => bubble.transform.position.y);

        protected float FindTheHighestBubble() => FindObjectsOfType<BaseBubble>()
            .Where(bubble => bubble.CurrentState == BubbleState.None)
            .Max(bubble => bubble.transform.position.y);

        private IEnumerator DelayBeforeProcessing()
        {
            yield return new WaitForSeconds(DelayBeforeDestroy);

            _coroutine = null;
            ProcessingTargets();
        }

        private void ProcessingTargets()
        {
            List<BaseBubble> targets =
                _bubbles.Where(bubble => bubble.CurrentState == BubbleState.TargetForDestroy).ToList();

            if (targets.Count > MinCountForDestroy)
            {
                onBubblesDestroy?.Invoke(targets);
                targets.ForEach(bubble => bubble.DestroyBubble());
            }
            else
            {
                targets.ForEach(bubble => bubble.ResetState());
            }
        }
    }
}
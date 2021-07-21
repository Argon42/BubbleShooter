using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using YodeGroup.BubbleShooter.GameElements.Bubbles;
using YodeGroup.BubbleShooter.Map;

namespace YodeGroup.BubbleShooter
{
    public class Shooter : GameService
    {
        [SerializeField] private LineRenderer line;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private float maxLineLenght = 100;
        [SerializeField] private int maxRaycasts = 1;
        [SerializeField] private float speedThrow = 105;

        [SerializeField] private UnityEvent prepareToShoot;
        [SerializeField] private UnityEvent shoot;
        [SerializeField] private UnityEvent prepareCanceled;

        private BubbleContainer _bubbleContainer;

        private bool _inputEnabled;

        private void Update()
        {
            BaseBubble currentBubble = _bubbleContainer.CurrentBubble;

            if (_inputEnabled && currentBubble)
            {
                ProcessInputForBubble(currentBubble);
            }
            else if ((_inputEnabled == false || currentBubble == false) && line.enabled)
            {
                line.enabled = false;
                prepareCanceled?.Invoke();
            }
        }

        public override async void StartService()
        {
            await Task.Yield();
            _inputEnabled = true;
        }

        public override void StopService()
        {
            _inputEnabled = false;
        }

        public override void Pause()
        {
            _inputEnabled = false;
        }

        public override async void Resume()
        {
            await Task.Yield();
            _inputEnabled = true;
        }

        public event UnityAction PrepareToShoot
        {
            add => prepareToShoot.AddListener(value);
            remove => prepareToShoot.RemoveListener(value);
        }

        public event UnityAction Shoot
        {
            add => shoot.AddListener(value);
            remove => shoot.RemoveListener(value);
        }

        public event UnityAction PrepareCanceled
        {
            add => prepareCanceled.AddListener(value);
            remove => prepareCanceled.RemoveListener(value);
        }

        public void Init(BubbleContainer bubbleContainer)
        {
            _bubbleContainer = bubbleContainer;
        }

        private void ThrowBubble(BaseBubble bubble, Vector2 direction)
        {
            bubble.Rigidbody2D.AddForce(direction.normalized * speedThrow);

            line.enabled = false;

            bubble.Throw();
        }

        private void ProcessInputForBubble(BaseBubble bubble)
        {
            Vector3 mousePosition = gameCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 bubblePosition = bubble.transform.position;
            var ray = new Ray2D(bubblePosition, mousePosition - bubblePosition);

            if (Input.GetMouseButton(0) && Input.mousePosition.y > Screen.height / 6f)
            {
                Vector3[] trajectory = CalculateTrajectory(ray, maxLineLenght, maxRaycasts);

                line.positionCount = trajectory.Length;
                line.SetPositions(trajectory);
                line.enabled = true;

                prepareToShoot?.Invoke();
            }

            if (Input.GetMouseButtonUp(0) && Input.mousePosition.y > Screen.height / 6f)
            {
                ThrowBubble(bubble, ray.direction);
                shoot?.Invoke();
            }

            if (Input.mousePosition.y < Screen.height / 6f)
            {
                line.enabled = false;
                prepareCanceled?.Invoke();
            }
        }

        private Vector3[] CalculateTrajectory(Ray2D ray, float maxLineLength, int maxRayBounds)
        {
            float lineDistance = maxLineLength;
            var positions = new List<Vector3>(maxRayBounds) {ray.origin};
            Vector2 direction = ray.direction;
            var contactFilter2D = new ContactFilter2D {useTriggers = false};

            for (var i = 0; i < maxRayBounds && lineDistance > 0; i++)
            {
                var hits = new RaycastHit2D[1];
                Vector2 origin = (Vector2) positions[i] + direction * 0.1f;
                Physics2D.Raycast(origin, direction * lineDistance, contactFilter2D, hits);

                RaycastHit2D hit = hits.FirstOrDefault();

                positions.Add(hit ? hit.point : (Vector2) positions[i] + direction * lineDistance);

                if (hit == false) break;

                Quaternion rotate = Quaternion.AngleAxis(180, hit.normal);
                direction = ((Vector2) (rotate * -direction)).normalized;
                lineDistance -= hit.distance;

                if (hit.transform.GetComponent<Wall>() == false) break;
            }

            return positions.ToArray();
        }
    }
}
using UnityEngine;

namespace YodeGroup.BubbleShooter.Utility
{
    [ExecuteAlways, RequireComponent(typeof(Camera))]
    public class CameraPivot : MonoBehaviour
    {
        [Header("Сhanges the local position so that the center of the camera is on the pivot"), SerializeField]
        private Vector2 pivot = Vector2.one / 2;

        private Camera _camera;

        private Camera Camera
        {
            get
            {
                if (_camera == null)
                    _camera = GetComponent<Camera>();
                return _camera;
            }
        }

        private void LateUpdate()
        {
            float size = Camera.orthographicSize;
            float aspect = Camera.aspect;
            transform.localPosition = -CalculateOffset(size, aspect);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            float size = Camera.orthographicSize;
            float aspect = Camera.aspect;
            Gizmos.DrawWireSphere(transform.position + CalculateOffset(size, aspect), 0.15f);
        }

        private Vector3 CalculateOffset(float size, float aspect)
        {
            var vector3 = (Vector3) (pivot * 2 - Vector2.one);
            vector3.x *= size * aspect;
            vector3.y *= size;
            return vector3;
        }
    }
}
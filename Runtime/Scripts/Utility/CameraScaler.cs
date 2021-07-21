using UnityEngine;

namespace YodeGroup.BubbleShooter.Utility
{
    [ExecuteAlways, RequireComponent(typeof(Camera))]
    public class CameraScaler : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float widthOrHeight;
        [SerializeField] private Vector2 referenceResolution = new Vector2(1080, 1920);
        [SerializeField] private float referenceSize = 5;
        [SerializeField] private float referenceFov = 60;

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

        private void Update()
        {
            float requirementAspect = referenceResolution.x / referenceResolution.y;

            if (Camera.orthographic)
            {
                float constantWidthSize = referenceSize * (requirementAspect / Camera.aspect);
                Camera.orthographicSize = Mathf.Lerp(constantWidthSize, referenceSize, widthOrHeight);
            }
            else
            {
                float constantWidthFov = CalcVerticalFov(referenceFov, Camera.aspect);
                Camera.fieldOfView = Mathf.Lerp(constantWidthFov, referenceFov, widthOrHeight);
            }
        }

        private static float CalcVerticalFov(float hFovInDeg, float aspectRatio)
        {
            float hFovInRads = hFovInDeg * Mathf.Deg2Rad;
            float vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2) / aspectRatio);

            return vFovInRads * Mathf.Rad2Deg;
        }
    }
}
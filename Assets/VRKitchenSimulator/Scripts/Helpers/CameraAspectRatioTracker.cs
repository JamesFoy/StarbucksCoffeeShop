using UnityEngine;
using VRKitchenSimulator.VRTK;
using VRTK;

namespace VRKitchenSimulator.Helpers
{
    public class CameraAspectRatioTracker: VRTK_SetupListener
    {
        [SerializeField] string targetTag;
        new Camera camera;
        RectTransform rectTransform;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        void Reset()
        {
            targetTag = "Player";
        }

        protected override void OnSDKSetupInitialized()
        {
            var cameras = VRTK_SharedMethods.FindEvenInactiveComponents<Camera>(true);
            foreach (var cam in cameras)
            {
                if (cam.CompareTag(targetTag))
                {
                    camera = cam;
                    return;
                }
            }

            camera = null;
        }

        protected override void Update()
        {
            base.Update();
            if (rectTransform == null || camera == null)
            {
                return;
            }

            var rectSize = rectTransform.rect.size;
            if (rectSize.x <= 0 || rectSize.y <= 0)
            {
                return;
            }

            var ar = rectSize.x / rectSize.y;
            camera.aspect = ar;
        }
    }
}
using UnityEngine;

namespace VRKitchenSimulator.VRTK
{
    [RequireComponent(typeof(MeshRenderer))]
    public class NearTouchMaterialSwapper : MonoBehaviour
    {
        [SerializeField] Material[] nearTouchMaterial;
        [SerializeField] Material[] standardMaterial;
        new MeshRenderer renderer;

        VRTK_BaseNearTouchEvents eventSource;

        void Reset()
        {
            var renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                standardMaterial = (Material[]) renderer.sharedMaterials.Clone();
                nearTouchMaterial = (Material[]) renderer.sharedMaterials.Clone();
            }
        }

        void OnEnable()
        {
            renderer = GetComponent<MeshRenderer>();
            if (standardMaterial == null)
            {
                standardMaterial = (Material[]) renderer.sharedMaterials.Clone();
            }

            eventSource = GetComponent<VRTK_BaseNearTouchEvents>();
            if (eventSource == null)
            {
                eventSource = GetComponentInParent<VRTK_BaseNearTouchEvents>();
            }

            if (eventSource != null)
            {
                eventSource.NearTouch.AddListener(OnNearTouch);
                eventSource.NearUntouch.AddListener(OnNearUntouch);
            }
        }

        void OnDisable()
        {
            if (eventSource != null)
            {
                eventSource.NearTouch.RemoveListener(OnNearTouch);
                eventSource.NearUntouch.RemoveListener(OnNearUntouch);
            }
        }

        void OnNearTouch()
        {
            if (renderer != null)
            {
                renderer.sharedMaterials = nearTouchMaterial;
            }
        }

        void OnNearUntouch()
        {
            if (renderer != null)
            {
                renderer.sharedMaterials = standardMaterial;
            }
        }
    }
}
using UnityEngine;

namespace VRKitchenSimulator.Helpers
{
    public class ButtonMaterialSwitcher : MonoBehaviour
    {
        void Awake()
        {
            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }
        }

        public void ToggleOn()
        {
            if (meshRenderer != null)
            {
                meshRenderer.materials = materialsActive;
            }
        }

        public void ToggleOff()
        {
            if (meshRenderer != null)
            {
                meshRenderer.materials = materialsInactive;
            }
        }

#pragma warning disable 649
        [SerializeField] Material[] materialsActive;
        [SerializeField] Material[] materialsInactive;
        [SerializeField] MeshRenderer meshRenderer;
#pragma warning restore 649
    }
}
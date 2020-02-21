using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

namespace VRKitchenSimulator.Prototypes
{
    public class BasketBehaviour : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        [Required]
        VRTK_SnapDropZone dropZone;
#pragma warning restore 649
        public UnityEvent Unsnapped;

        void OnEnable()
        {
            dropZone.ObjectUnsnappedFromDropZone += OnUnsnapped;
        }

        void OnDisable()
        {
            dropZone.ObjectUnsnappedFromDropZone -= OnUnsnapped;
        }

        void OnUnsnapped(object sender, SnapDropZoneEventArgs e)
        {
            Unsnapped?.Invoke();
        }
    }
}
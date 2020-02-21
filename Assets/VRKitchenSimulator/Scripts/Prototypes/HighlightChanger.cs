using UnityEngine;
using VRTK;

namespace VRKitchenSimulator.Prototypes
{
    public class HighlightChanger : MonoBehaviour
    {
        VRTK_SnapDropZone dropZone;

        void Awake()
        {
            dropZone = GetComponent<VRTK_SnapDropZone>();
            dropZone.highlightAlwaysActive = false;
        }

        public void ActivateHighlight()
        {
            dropZone.highlightAlwaysActive = true;
        }

        public void DeactivateHighlight()
        {
            dropZone.highlightAlwaysActive = false;
        }
    }
}
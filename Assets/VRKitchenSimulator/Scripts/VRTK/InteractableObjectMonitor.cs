using UnityEngine;
using VRTK;

namespace VRKitchenSimulator.VRTK
{
    // A debug class. 
    public class InteractableObjectMonitor : MonoBehaviour
    {
        VRTK_InteractableObject source;

        void OnEnable()
        {
            source = GetComponent<VRTK_InteractableObject>();
            if (source != null)
            {
                source.InteractableObjectGrabbed += OnInteractableObjectGrabbed;
                source.InteractableObjectUngrabbed += OnInteractableObjectUngrabbed;
            }
        }

        void OnInteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            Debug.Log("Object " + gameObject.name + " ungrabbed");
        }

        void OnInteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            Debug.Log("Object " + gameObject.name + " grabbed");
        }

        void OnDisable()
        {
            if (source != null)
            {
                source.InteractableObjectGrabbed -= OnInteractableObjectGrabbed;
                source.InteractableObjectUngrabbed -= OnInteractableObjectUngrabbed;
                source = null;
            }
        }
    }
}
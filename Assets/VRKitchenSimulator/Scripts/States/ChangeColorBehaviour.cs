using UnityEngine;
using UnityEngine.UI;

namespace VRKitchenSimulator.States
{
    public class ChangeColorBehaviour : MonoBehaviour
    {
        public Color Activated;
        public Color Deactivated;
        Graphic graphic;

        public Graphic Graphic
        {
            get
            {
                if (graphic == null)
                {
                    graphic = GetComponent<Graphic>();
                }

                return graphic;
            }
        }

        void Reset()
        {
            Deactivated = Color.red;
            Activated = Color.green;
        }

        public void OnActivated()
        {
            if (Graphic == null)
            {
                Debug.Log("No Graphic yet");
                return;
            }

            Graphic.color = Activated;
        }

        public void OnDeactivated()
        {
            if (Graphic == null)
            {
                Debug.Log("No Graphic yet");
                return;
            }

            Graphic.color = Deactivated;
        }
    }
}
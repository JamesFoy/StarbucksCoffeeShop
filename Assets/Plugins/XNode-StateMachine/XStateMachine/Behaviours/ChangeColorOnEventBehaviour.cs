using UnityEngine;
using UnityEngine.UI;
using XStateMachine.States;

namespace XStateMachine.Behaviours
{
    public class ChangeColorOnEventBehaviour : MonoBehaviour
    {
        public Color Activated;
        public Color Deactivated;
        Graphic graphic;
        public BaseState Source;

        void Reset()
        {
            Deactivated = Color.red;
            Activated = Color.green;
        }

        void Awake()
        {
            var runtimeGraph = this.GetComponentInParent<RuntimeGraphHolder>();
            if (runtimeGraph != null)
            {
                Source = runtimeGraph.RuntimeGraph.FindRuntimeState(Source);
            }
        }

        void OnEnable()

        {
            if (Source == null)
            {
                return;
            }

            Source.Changed.AddListener(OnStateActivated);
            graphic = GetComponent<Graphic>();
            OnStateActivated();
        }

        void OnStateActivated()
        {
            if (graphic == null)
            {
                return;
            }

            if (Source.IsActive)
            {
                graphic.color = Activated;
            }
            else
            {
                graphic.color = Deactivated;
            }
        }

        void OnDisable()
        {
            Source.Changed.RemoveListener(OnStateActivated);
        }
    }
}
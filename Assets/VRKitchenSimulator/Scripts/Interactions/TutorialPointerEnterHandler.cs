using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace VRKitchenSimulator.Interactions
{
    public class TutorialPointerEnterHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        float timeHovered;
        bool hovered;

        public void OnPointerEnter(PointerEventData eventData)
        {
            hovered = true;
            timeHovered = Time.time;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            timeHovered = 0;
            hovered = false;
        }

        void Update()
        {
            if (hovered && (timeHovered + delay < Time.time))
            {
                HoverComplete.Invoke();
                hovered = false;
            }
        }
#pragma warning disable 649
        [SerializeField] float delay;
        [SerializeField] UnityEvent HoverComplete;
#pragma warning restore 649
    }
}
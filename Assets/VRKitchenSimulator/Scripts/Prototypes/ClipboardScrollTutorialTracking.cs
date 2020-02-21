using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VRKitchenSimulator.Prototypes
{
    public class ClipboardScrollTutorialTracking : MonoBehaviour
    {
        float lastEventFired;
        float scrollBarValue;

        void Reset()
        {
            scrollOffset = 0.2f;
            eventFireInterval = 0.25f;
        }

        void OnEnable()
        {
            if (ScrollBarVertical != null)
            {
                ScrollBarVertical.onValueChanged.AddListener(ValueChangeCheck);
                scrollBarValue = ScrollBarVertical.value;
            }
        }

        void OnDisable()
        {
            if (ScrollBarVertical != null)
            {
                ScrollBarVertical.onValueChanged.RemoveListener(ValueChangeCheck);
            }
        }

        void ValueChangeCheck(float value)
        {
            // Debug.Log($"value change check: {ScrollBarValue} - {ScrollBarVertical.value}");
            if (Mathf.Abs(scrollBarValue - ScrollBarVertical.value) >= scrollOffset)
            {
                if (lastEventFired + eventFireInterval < Time.time)
                {
                    // Debug.Log("event success");
                    ScrollBarHasMoved.Invoke();
                    lastEventFired = Time.time;
                    ScrollBarVertical.onValueChanged.RemoveListener(ValueChangeCheck);
                }
            }
        }
#pragma warning disable 649
        [Tooltip("This event is fired when the scrollbar moves")]
        [SerializeField]
        UnityEvent ScrollBarHasMoved;

        [Tooltip("The scrollbar that is tracked for changes")]
        [SerializeField]
        [Required]
        Scrollbar ScrollBarVertical;

        [Tooltip("How much (as percentage of the total height) should the player have to scroll before the event is fired.")]
        [SerializeField]
        float scrollOffset;

        [Tooltip("How much time need to pass between events being fired?")]
        [SerializeField]
        float eventFireInterval;
#pragma warning restore 649
    }
}
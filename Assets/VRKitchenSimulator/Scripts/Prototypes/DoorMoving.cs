using UnityEngine;
using UnityEngine.Events;
using VRTK.Controllables;
using VRTK.Controllables.ArtificialBased;

namespace VRKitchenSimulator.Prototypes
{
    public class DoorMoving : MonoBehaviour
    {
        VRTK_ArtificialPusher pusher;

        public bool isPressed;

        Animator anim;

        // Use this for initialization
        void Start()
        {
            anim = GetComponentInParent<Animator>();

            pusher = GetComponentInChildren<VRTK_ArtificialPusher>();

            if (pusher != null)
            {
                pusher.MaxLimitReached += OnMaxLimitReached;
                pusher.MinLimitReached += OnMinLimitReached;
                isPressed = pusher.AtMaxLimit();
                anim.SetBool("Pressed", isPressed);
            }
        }

        void OnMinLimitReached(object sender, ControllableEventArgs e)
        {
            isPressed = false;
            anim.SetBool("Pressed", isPressed);
            NotPressed.Invoke();
        }

        void OnMaxLimitReached(object sender, ControllableEventArgs e)
        {
            isPressed = true;
            anim.SetBool("Pressed", isPressed);
            Pressed.Invoke();
        }

#pragma warning disable 649
        [SerializeField] UnityEvent Pressed;
        [SerializeField] UnityEvent NotPressed;
#pragma warning restore 649
    }
}
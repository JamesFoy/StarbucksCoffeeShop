using UnityEngine;
using UnityEngine.Events;

namespace VRKitchenSimulator.Prototypes
{
    public class CoffeePlate : MonoBehaviour, ICoffeePowderReceiver
    {
#pragma warning disable 649
        [SerializeField] Animation coffeeFillAnimation;
#pragma warning restore 649
        public UnityEvent CoffeeAdded;

        bool coffeePowderInserted;

        public void ReceivedCoffeePowder()
        {
            if (coffeePowderInserted == false)
            {
                CoffeeAdded.Invoke();
                coffeePowderInserted = true;
                coffeeFillAnimation.gameObject.SetActive(true);
                coffeeFillAnimation.Play();
            }
        }

        // Use this for initialization
        void Awake()
        {
            coffeeFillAnimation.gameObject.SetActive(false);
        }
    }
}
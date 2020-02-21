using UnityEngine;

namespace VRKitchenSimulator.Prototypes
{
    public class ObjectDeleting : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ScrunchedPaper") || other.CompareTag("CoffeeSachet") || other.CompareTag("FilterPaper"))
            {
                Destroy(other.gameObject);
                Debug.Log("destoryed");
            }
        }
    }
}
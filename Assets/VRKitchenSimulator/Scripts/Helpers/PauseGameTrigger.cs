using UnityEngine;

namespace VRKitchenSimulator.Helpers
{
    public class PauseGameTrigger : MonoBehaviour
    {
#if UNITY_EDITOR
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                Debug.Break();
            }
        }
#endif
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace VRKitchenSimulator.Prototypes
{
    public class CoffeeSachetToolTipTrigger : MonoBehaviour
    {
        public UnityEvent SachetShowToolTip;
        public UnityEvent SachetHideToolTip;
        private bool ToolTipVisible = false;
        private bool SachetReleasedEarly = false;
        private float ToolTipWait = 2.0f;
        public void CoffeeSachetToolTipActivate()
        {
            SachetReleasedEarly = false;
            if (ToolTipVisible == false)
            {
                StartCoroutine(ToolTipTimer());
            }
        }
        public void CoffeeSachetToolTipDeactivate()
        {
            if (ToolTipVisible == false)
            {
                SachetReleasedEarly = true;
                SachetHideToolTip.Invoke();
                ToolTipVisible = false;
            }
            else
            {
                SachetHideToolTip.Invoke();
                ToolTipVisible = false;
            }
        }
        private IEnumerator ToolTipTimer()
        {
            yield return new WaitForSeconds(ToolTipWait);
            Debug.Log("timer finished");
            if (ToolTipVisible == false && SachetReleasedEarly == false)
            {
                SachetShowToolTip.Invoke();
                ToolTipVisible = true;
            }
            yield return null;
        }
    }
}
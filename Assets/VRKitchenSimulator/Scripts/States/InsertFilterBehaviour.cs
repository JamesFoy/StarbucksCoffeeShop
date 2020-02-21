using UnityEngine;
using VRKitchenSimulator.States.Parts;

namespace VRKitchenSimulator.States
{
    public class InsertFilterBehaviour : MonoBehaviour
    {
        public FilterPanStates Filter;
        public FilterPanBinding Binding;

        public void ToggleBinding()
        {
            if (Binding == null)
            {
                return;
            }

            if (Binding.SourceRuntime == null)
            {
                Binding.SourceRuntime = Filter;
            }
            else
            {
                Binding.SourceRuntime = null;
            }
        }
    }
}
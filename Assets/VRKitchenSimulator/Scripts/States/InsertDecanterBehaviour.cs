using UnityEngine;
using VRKitchenSimulator.States.Parts;

namespace VRKitchenSimulator.States
{
    public class InsertDecanterBehaviour : MonoBehaviour
    {
        public DecanterStates Filter;
        public DecanterBinding Binding;

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
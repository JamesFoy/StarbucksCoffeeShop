using NaughtyAttributes;
using UnityEngine;

namespace VRKitchenSimulator.Helpers
{
    [CreateAssetMenu(menuName = "Read Me")]
    public class ReadMeHolder : ScriptableObject
    {
        [ResizableTextArea]
        public string Documentation;
    }
}
using UnityEngine;
using VRTK;

namespace VRKitchenSimulator.VRTK
{
    public class DefaultInteractObjectHighlighter : VRTK_InteractObjectHighlighter
    {
        void Reset()
        {
            nearTouchHighlight = Color.gray;
            touchHighlight = Color.yellow;
            grabHighlight = Color.green;
            useHighlight = Color.blue;
        }
    }
}
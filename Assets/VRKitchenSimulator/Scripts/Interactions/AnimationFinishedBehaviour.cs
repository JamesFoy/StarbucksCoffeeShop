using UnityEngine;

namespace VRKitchenSimulator.Interactions
{
    public class AnimationFinishedBehaviour : StateMachineBehaviour
    {
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var target = animator.gameObject.GetComponent<ActivateNextInteractionIndicator>();
            if (target != null)
            {
                target.OnAnimationFinished();
            }
        }
    }
}
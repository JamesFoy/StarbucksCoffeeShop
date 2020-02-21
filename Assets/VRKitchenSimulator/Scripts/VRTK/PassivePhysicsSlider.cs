using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using VRTK;
using VRTK.Controllables.PhysicsBased;

namespace VRKitchenSimulator.VRTK
{
    /// <summary>
    ///     A slider that defers all interactions to a interactable object located elsewhere.
    ///     This is useful for shifting the slider via handle game objects that are attached
    ///     via joints.
    /// </summary>
    public class PassivePhysicsSlider : VRTK_PhysicsSlider
    {
        public ConfigurableJoint UserDefinedJoint;

        [ShowNativeProperty]
        [UsedImplicitly]
        public float NormalizeValue => GetNormalizedValue();

        public VRTK_InteractableObject GrabObject
        {
            get { return controlInteractableObject; }
            set
            {
                if (ReferenceEquals(controlInteractableObject, value))
                {
                    return;
                }

                ManageInteractableObjectListeners(false);
                controlInteractableObject = value;
                ManageInteractableObjectListeners(true);
                if (controlInteractableObject != null)
                {
                    if (controlInteractableObject.IsGrabbed())
                    {
                        AttemptMove();
                    }
                    else
                    {
                        AttemptRelease();
                    }
                }
                else
                {
                    AttemptRelease();
                }
            }
        }

        // ignore base method; called when OnEnableEvent is handled.
        protected override void SetupInteractableObject()
        {
            ManageInteractableObjectListeners(true);
        }

        protected override void SetupJoint()
        {
            //move transform towards activation distance
            transform.localPosition = originalLocalPosition + AxisDirection() * (maximumLength * 0.5f);

            controlJoint = UserDefinedJoint;
            createControlJoint = false;
            if (controlJoint == null)
            {
                controlJoint = gameObject.AddComponent<ConfigurableJoint>();
                createControlJoint = true;

                controlJoint.angularXMotion = ConfigurableJointMotion.Locked;
                controlJoint.angularYMotion = ConfigurableJointMotion.Locked;
                controlJoint.angularZMotion = ConfigurableJointMotion.Locked;

                controlJoint.xMotion = operateAxis == OperatingAxis.xAxis ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked;
                controlJoint.yMotion = operateAxis == OperatingAxis.yAxis ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked;
                controlJoint.zMotion = operateAxis == OperatingAxis.zAxis ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked;

                var linearLimit = new SoftJointLimit();
                linearLimit.limit = Mathf.Abs(maximumLength * 0.5f);
                controlJoint.linearLimit = linearLimit;
                controlJoint.connectedBody = connectedTo;

                EnableJointDriver();
            }
        }
    }
}
using UnityEngine;
using UnityEngine.Events;
using VRTK;

namespace VRKitchenSimulator.VRTK
{
    /// <summary>
    ///     A snap-drop-zone that uses joints for connecting objects and that creates joints on
    ///     demand, so that objects are no longer anchored in space.
    /// </summary>
    public class OnDemandJointSnapDropZone : VRTK_SnapDropZone
    {
        public Joint snapDropZoneJoint;
        public float breakForce;
        public UnityEvent JointBroken;

        protected override void Awake()
        {
            snapType = SnapTypes.UseJoint;
            base.Awake();
        }

        /// <summary>
        ///     Skip the check of the actual joint and use the defined break force instead. It is the same anyway.
        /// </summary>
        /// <param name="interactableObjectCheck"></param>
        /// <returns></returns>
        protected override bool ValidUnsnap(VRTK_InteractableObject interactableObjectCheck)
        {
            if (interactableObjectCheck.IsGrabbed())
            {
                return true;
            }

            return ((snapType != SnapTypes.UseJoint) || float.IsInfinity(breakForce)) &&
                   (interactableObjectCheck.validDrop == VRTK_InteractableObject.ValidDropTypes.DropAnywhere);
        }

        /// <summary>
        ///     Modified joint method. Instead of requiring an empty joint (which would anchor the object in world space),
        ///     we create a joint only when we can guarantee that it will be connected to an object.
        /// </summary>
        /// <param name="snapTo"></param>
        protected override void SetSnapDropZoneJoint(Rigidbody snapTo)
        {
            if (snapTo == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_SnapDropZone", "Rigidbody",
                                                               "the `VRTK_InteractableObject`"));
                return;
            }

            if (snapDropZoneJoint == null)
            {
                snapDropZoneJoint = gameObject.AddComponent<FixedJoint>();
                snapDropZoneJoint.breakForce = breakForce;
            }

            snapDropZoneJoint.connectedBody = snapTo;
            originalJointCollisionState = snapDropZoneJoint.enableCollision;
            //need to set this to true otherwise highlighting doesn't work again on grab
            snapDropZoneJoint.enableCollision = true;
        }

        protected override void ResetSnapDropZoneJoint()
        {
            if (snapDropZoneJoint != null)
            {
                Destroy(snapDropZoneJoint);
                snapDropZoneJoint = null;
            }
        }

        /// <summary>
        ///     Removed check for joint existence. There will be no joint until an object is snapped on.
        /// </summary>
        protected override void CreateHighlightersInEditor()
        {
            if (VRTK_SharedMethods.IsEditTime())
            {
                GenerateHighlightObject();

                GenerateEditorHighlightObject();
                ForceSetObjects();
                if (highlightEditorObject != null)
                {
                    highlightEditorObject.SetActive(displayDropZoneInEditor);
                }
            }
        }

        void OnJointBreak(float breakForce)
        {
            JointBroken?.Invoke();
        }

        /// <summary>
        ///     Helper function: Take over this interactable object for smooth transitions between different states.
        /// </summary>
        /// <param name="interactableObjectToSnap"></param>
        public void ForceSnapWithGrab(VRTK_InteractableObject interactableObjectToSnap)
        {
            if (interactableObjectToSnap == null)
            {
                return;
            }

            if (attemptTransitionAtEndOfFrameRoutine != null)
            {
                StopCoroutine(attemptTransitionAtEndOfFrameRoutine);
            }

            if (checkCanSnapRoutine != null)
            {
                StopCoroutine(checkCanSnapRoutine);
            }

            if (gameObject.activeInHierarchy)
            {
                attemptTransitionAtEndOfFrameRoutine = StartCoroutine(AttemptForceSnapAtEndOfFrame(interactableObjectToSnap));
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            var colliders = GetComponentsInChildren<Collider>();
            foreach (var c in colliders)
            {
                c.enabled = true;
            }
        }

        protected override void OnDisable()
        {
            var colliders = GetComponentsInChildren<Collider>();
            foreach (var c in colliders)
            {
                c.enabled = false;
            }

            base.OnDisable();
        }
    }
}
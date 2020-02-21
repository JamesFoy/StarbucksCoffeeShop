using System;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using VRTK;
using VRTK.Controllables;
using VRTK.GrabAttachMechanics;

namespace VRKitchenSimulator.VRTK
{
    public enum TriggerZone
    {
        [UsedImplicitly]
        Minimum,
        Maximum
    }

    public enum SliderPosition
    {
        Minimum,
        Partial,
        Maximum
    }

    public class SliderMountController : MonoBehaviour
    {
        public UnityEvent SliderDetachPositionReached;
        public UnityEvent SliderDetachPositionExited;
        public UnityEvent SliderLimitReached;
        public UnityEvent SliderLimitExited;
        public UnityEvent ObjectAttached;
        public UnityEvent ObjectDetached;

        public OnDemandJointSnapDropZone dropZone;
        public PassivePhysicsSlider slider;
        public TriggerZone TriggerZone;
        public float velocityLimit = float.PositiveInfinity;
        bool handlingSnap;
        SavedState state;
        bool waitForLeaveMaxLimit;
        bool waitForSnapComplete;

        public SliderPosition SliderPosition { get; set; }
        [ShowNativeProperty] public VRTK_InteractableObject MountedObject => state.interactiveObject;

        void OnEnable()
        {
            dropZone.ObjectSnappedToDropZone += OnSnap;
            dropZone.ObjectUnsnappedFromDropZone += OnUnsnap;
            dropZone.JointBroken.AddListener(OnJointBroken);

            if (TriggerZone == TriggerZone.Maximum)
            {
                slider.MaxLimitReached += OnDetachLimitReached;
                slider.MaxLimitExited += OnDetachLimitExited;
                slider.MinLimitReached += OnSliderLimitReached;
                slider.MinLimitExited += OnSliderLimitExited;
            }
            else
            {
                slider.MinLimitReached += OnDetachLimitReached;
                slider.MinLimitExited += OnDetachLimitExited;
                slider.MaxLimitReached += OnSliderLimitReached;
                slider.MaxLimitExited += OnSliderLimitExited;
            }

            // VRTK always seem to insist on positioning sliders at the minimum level.
            SliderPosition = SliderPosition.Minimum;
        }

        void OnDisable()
        {
            dropZone.ObjectSnappedToDropZone -= OnSnap;
            dropZone.ObjectUnsnappedFromDropZone -= OnUnsnap;
            dropZone.JointBroken.RemoveListener(OnJointBroken);

            if (TriggerZone == TriggerZone.Maximum)
            {
                slider.MaxLimitReached -= OnDetachLimitReached;
                slider.MaxLimitExited -= OnDetachLimitExited;
                slider.MinLimitReached -= OnSliderLimitReached;
                slider.MinLimitExited -= OnSliderLimitExited;
            }
            else
            {
                slider.MinLimitReached -= OnDetachLimitReached;
                slider.MinLimitExited -= OnDetachLimitExited;
                slider.MaxLimitReached -= OnSliderLimitReached;
                slider.MaxLimitExited -= OnSliderLimitExited;
            }
        }

        /// <summary>
        ///     Called when the slider has left the opposite side of the detach point. This is usually
        ///     the point where the object is considered fully inserted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSliderLimitExited(object sender, ControllableEventArgs e)
        {
            SliderLimitExited.Invoke();
        }

        /// <summary>
        ///     Called when the slider has reached the opposite side of the detach point. This is usually
        ///     the point where the object is considered fully inserted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSliderLimitReached(object sender, ControllableEventArgs e)
        {
            SliderLimitReached.Invoke();
        }

        void OnDetachLimitExited(object sender, ControllableEventArgs e)
        {
            // Debug.Log("Detach Zone left");
            waitForLeaveMaxLimit = false;
            SliderDetachPositionExited.Invoke();
        }

        void OnDetachLimitReached(object sender, ControllableEventArgs e)
        {
            // Debug.Log("Detach Zone Entered");
            if (waitForLeaveMaxLimit)
            {
                Debug.Log("Ignoring, as Max-Zone not left");
                return;
            }

            DetachFromSlider();
        }

        void OnJointBroken()
        {
            OnUnsnap(this, new SnapDropZoneEventArgs());
        }

        void OnUnsnap(object sender, SnapDropZoneEventArgs e)
        {
            if (handlingSnap)
            {
                return;
            }

            DetachFromSlider();
        }

        void OnSnap(object sender, SnapDropZoneEventArgs e)
        {
            if (waitForSnapComplete)
            {
                waitForSnapComplete = false;
                return;
            }

            handlingSnap = true;

            try
            {
                AttachSnapPointObjectToSlider();
                ObjectAttached.Invoke();
                waitForLeaveMaxLimit = true;
            }
            finally
            {
                handlingSnap = false;
            }
        }

        void AttachSnapPointObjectToSlider()
        {
            var interactiveObject = dropZone.GetCurrentSnappedInteractableObject();
            if (interactiveObject == null)
            {
                Debug.LogError("There is no game object currently snapped on to the snap drop zone.");
                return;
            }

            var joint = dropZone.snapDropZoneJoint;
            if (joint == null)
            {
                throw new Exception("Joint not yet assigned? - This should not happen at this point");
            }

            var grabber = interactiveObject.FindGrabController();
            if (grabber != null)
            {
                grabber.StopTouchAndGrab(interactiveObject);
            }

            state = new SavedState(dropZone, velocityLimit);

            if (grabber != null)
            {
                grabber.InitiateTouch(interactiveObject);
            }

            slider.GrabObject = interactiveObject;

            dropZone.enabled = false;
        }

        void DetachFromSlider()
        {
            var interactiveObject = state.interactiveObject;
            if (interactiveObject == null)
            {
                // this happens when the slider first starts up.
                return;
            }

            ObjectDetached.Invoke();

            var grabber = interactiveObject.FindGrabController();
            if (grabber != null)
            {
                grabber.StopTouchAndGrab(interactiveObject);
            }

            SliderDetachPositionReached.Invoke();
            slider.GrabObject = null;

            state = state.Restore();

            if (grabber != null)
            {
                grabber.InitiateTouchAndGrab(interactiveObject);
            }

            dropZone.enabled = true;
            state = new SavedState();
        }

        struct SavedState
        {
            readonly bool disableWhenIdleMemo;
            readonly VRTK_BaseGrabAttach oldGrabAttach;
            readonly VRTK_TrackObjectGrabAttach grabbingBehaviour;
            readonly FixedJoint newJoint;
            public readonly VRTK_InteractableObject interactiveObject;

            public SavedState(OnDemandJointSnapDropZone dropZone, float velocityLimit)
            {
                var interactiveObject = dropZone.GetCurrentSnappedInteractableObject();

                disableWhenIdleMemo = interactiveObject.disableWhenIdle;
                oldGrabAttach = interactiveObject.grabAttachMechanicScript;
                newJoint = CreateJoint(dropZone);

                grabbingBehaviour = interactiveObject.gameObject.AddComponent<VRTK_TrackObjectGrabAttach>();
                grabbingBehaviour.velocityLimit = velocityLimit;
                grabbingBehaviour.precisionGrab = true;

                this.interactiveObject = interactiveObject;
                this.interactiveObject.isGrabbable = true;
                this.interactiveObject.grabAttachMechanicScript = grabbingBehaviour;
            }

            public SavedState Restore()
            {
                interactiveObject.disableWhenIdle = disableWhenIdleMemo;
                interactiveObject.grabAttachMechanicScript = oldGrabAttach;

                if (grabbingBehaviour != null)
                {
                    Destroy(grabbingBehaviour);
                }

                if (newJoint != null)
                {
                    Destroy(newJoint);
                }

                return new SavedState();
            }

            static FixedJoint CreateJoint(OnDemandJointSnapDropZone dropZone)
            {
                var joint = dropZone.snapDropZoneJoint;

                var newJoint = dropZone.gameObject.AddComponent<FixedJoint>();
                newJoint.breakForce = float.PositiveInfinity;
                newJoint.breakTorque = float.PositiveInfinity;
                newJoint.connectedBody = joint.connectedBody;
                newJoint.connectedBody.velocity = Vector3.zero;
                newJoint.enableCollision = false;
                newJoint.anchor = joint.anchor;
                return newJoint;
            }
        }
    }
}
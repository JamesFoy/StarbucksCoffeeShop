using System.Collections.Generic;
using UnityEngine;

namespace VRKitchenSimulator.Helpers
{
    public class FillLevelAnimationController : MonoBehaviour
    {
        readonly List<AnimationState> fillLevelAnimationStates;

        public FillLevelAnimationController()
        {
            fillLevelAnimationStates = new List<AnimationState>();
        }

        void OnEnable()
        {
            UpdateFillLevel(level);
            if (fillLevelAnimation != null)
            {
                fillLevelAnimation.Play();
                fillLevelAnimationStates.Clear();
                foreach (AnimationState state in fillLevelAnimation)
                {
                    if (state != null)
                    {
                        fillLevelAnimationStates.Add(state);
                    }
                }
            }
        }

        void Update()
        {
            UpdateFillLevel(level);
        }

        public void UpdateFillLevel(float level)
        {
            var clampedLevel = Mathf.Clamp01(level);
            this.level = clampedLevel;
            if ((fillLevelAnimation == null) || (target == null))
            {
                return;
            }

            if (this.level <= 0)
            {
                target.gameObject.SetActive(false);
                return;
            }

            if (target.gameObject.activeSelf == false)
            {
                target.gameObject.SetActive(true);
            }

            foreach (var animState in fillLevelAnimationStates)
            {
                if (animState == null)
                {
                    continue;
                }

                var clipLength = animState.clip.length;
                animState.time = clampedLevel * clipLength;
                animState.speed = 0;
            }

            // fillLevelAnimation.Play();
        }
#pragma warning disable 649
        [SerializeField] GameObject target;
        [SerializeField] Animation fillLevelAnimation;
        [SerializeField] float level;
#pragma warning restore 649
    }
}
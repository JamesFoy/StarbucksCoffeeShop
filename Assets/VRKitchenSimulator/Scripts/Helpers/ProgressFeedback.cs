using System.Collections.Generic;
using UnityEngine;
using UnityTutorialSystem.Aggregators;
using UnityTutorialSystem.Events;

namespace VRKitchenSimulator.Helpers
{
    public abstract class ProgressFeedback : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] List<EventMessageAggregator> aggregators;
#pragma warning restore 649


        void OnEnable()
        {
            foreach (var aggregator in aggregators)
            {
                aggregator.MatchProgress.AddListener(OnEventProgress);
            }
        }

        void OnDisable()
        {
            foreach (var aggregator in aggregators)
            {
                aggregator.MatchProgress.RemoveListener(OnEventProgress);
            }
        }

        protected abstract void OnEventProgress(EventMessageAggregator source, BasicEventStreamMessage message);
    }
}
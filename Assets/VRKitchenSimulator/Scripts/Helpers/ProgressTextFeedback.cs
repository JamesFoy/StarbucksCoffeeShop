using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityTutorialSystem.Aggregators;
using UnityTutorialSystem.Events;

namespace VRKitchenSimulator.Helpers
{
    public class ProgressTextFeedback : ProgressFeedback
    {
        readonly HashSet<BasicEventStreamMessage> errorsHandled;
        float score;

        public ProgressTextFeedback()
        {
            errorsHandled = new HashSet<BasicEventStreamMessage>();
        }

        void Reset()
        {
            messagePattern = "Correct: {0}";
        }

        void Awake()
        {
            score = 0;
            textCorrect.text = string.Format(messagePattern, score);
        }

        protected override void OnEventProgress(EventMessageAggregator source, BasicEventStreamMessage message)
        {
            if (!errorsHandled.Contains(message))
            {
                score += 1;
                textCorrect.text = string.Format(messagePattern, score);
                errorsHandled.Add(message);
            }
        }

#pragma warning disable 649
        [SerializeField]
        Text textCorrect;

        [SerializeField]
        string messagePattern;
#pragma warning restore 649
    }
}
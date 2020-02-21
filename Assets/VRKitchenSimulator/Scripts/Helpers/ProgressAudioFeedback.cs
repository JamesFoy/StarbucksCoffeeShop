using UnityEngine;
using UnityTutorialSystem.Aggregators;
using UnityTutorialSystem.Events;

namespace VRKitchenSimulator.Helpers
{
    public class ProgressAudioFeedback : ProgressFeedback
    {
#pragma warning disable 649
        [SerializeField] AudioSource sound;
#pragma warning restore 649

        protected override void OnEventProgress(EventMessageAggregator source, BasicEventStreamMessage message)
        {
            if ((sound != null) && (sound.clip != null))
            {
                sound.PlayOneShot(sound.clip);
            }
        }
    }
}
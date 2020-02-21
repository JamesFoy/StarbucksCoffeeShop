using UnityEngine;

namespace VRKitchenSimulator.Interactions
{
    public class InteractableCollisionSoundSource : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] float delayAfterPlay;
        [SerializeField] float pitchRange;
        float nextPlay;
        float defaultPitch;

        void Reset()
        {
            delayAfterPlay = 0.1f;
            pitchRange = 0.1f;
            audioSource = GetComponent<AudioSource>();
        }

        void Awake()
        {
            if (delayAfterPlay <= 0)
            {
                delayAfterPlay = 0.5f;
            }

            nextPlay = Time.time + delayAfterPlay;
            if (audioSource != null)
            {
                defaultPitch = audioSource.pitch;
            }
            else
            {
                defaultPitch = 1;
            }
        }

        void OnCollisionEnter(Collision other)
        {
            if ((audioSource != null) && (audioSource.clip != null))
            {
                if (nextPlay <= 0)
                {
                    Debug.Log("Play before awake");
                }
                else if (nextPlay < Time.time)
                {
                    Debug.Log("Play sound for " + name);
                    audioSource.pitch = defaultPitch + RandomizePitch();
                    audioSource.Play();
                    nextPlay = Time.time + delayAfterPlay;
                }
            }
        }

        float RandomizePitch()
        {
            var randomNoise = (Random.value - 0.5f) * 2 * pitchRange;
            return defaultPitch + randomNoise;
        }
    }
}
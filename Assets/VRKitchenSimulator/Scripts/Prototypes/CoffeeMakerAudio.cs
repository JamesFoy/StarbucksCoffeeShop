using UnityEngine;

namespace VRKitchenSimulator.Prototypes
{
    public class CoffeeMakerAudio : MonoBehaviour
    {
        void OnEnable()
        {
            coffeeMaker.HalfButtonActivated.AddListener(OnButtonClicked);
            coffeeMaker.FullButtonActivated.AddListener(OnButtonClicked);
            coffeeMaker.StopButtonActivated.AddListener(OnButtonClicked);
            coffeeMaker.BrewingStartedEvent.AddListener(OnBrewStarted);
            coffeeMaker.BrewingFinishedEvent.AddListener(OnBrewFinished);
            coffeeMaker.FilterFullyInserted.AddListener(OnFilterLock);
            coffeeMaker.FilterNoLongerInserted.AddListener(OnFilterLock);
            coffeeMaker.FlaskAttached.RemoveListener(OnFlaskAttached);
            coffeeMaker.FlaskAttached.RemoveListener(OnFlaskRemoved);
        }

        void OnDisable()
        {
            coffeeMaker.HalfButtonActivated.RemoveListener(OnButtonClicked);
            coffeeMaker.FullButtonActivated.RemoveListener(OnButtonClicked);
            coffeeMaker.StopButtonActivated.RemoveListener(OnButtonClicked);
            coffeeMaker.BrewingStartedEvent.RemoveListener(OnBrewStarted);
            coffeeMaker.BrewingFinishedEvent.RemoveListener(OnBrewFinished);
            coffeeMaker.FilterFullyInserted.RemoveListener(OnFilterLock);
            coffeeMaker.FilterNoLongerInserted.RemoveListener(OnFilterLock);
            coffeeMaker.FlaskAttached.RemoveListener(OnFlaskAttached);
            coffeeMaker.FlaskAttached.RemoveListener(OnFlaskRemoved);
        }

        void OnFlaskRemoved()
        {
            if (coffeeMaker.Brewing)
            {
                if ((waterSplashing != null) && (waterSplashing.clip != null))
                {
                    waterSplashing.Play();
                }
            }
        }

        void OnFlaskAttached()
        {
            if (coffeeMaker.Brewing)
            {
                if ((waterSplashing != null) && (waterSplashing.clip != null))
                {
                    waterSplashing.Stop();
                }
            }
        }

        void OnFilterLock()
        {
            if ((filterLock != null) && (filterLock.clip != null))
            {
                filterLock.PlayDelayed(1f);
            }
        }

        void OnBrewStarted()
        {
            if ((coffeeBrewing != null) && (coffeeBrewing.clip != null))
            {
                coffeeBrewing.PlayDelayed(1f);
            }

            if (!coffeeMaker.HasFlask)
            {
                if ((waterSplashing != null) && (waterSplashing.clip != null))
                {
                    waterSplashing.Play();
                }
            }
        }

        void OnBrewFinished()
        {
            if ((coffeeBrewing != null) && (coffeeBrewing.clip != null))
            {
                coffeeBrewing.Stop();
            }

            if ((waterSplashing != null) && (waterSplashing.clip != null))
            {
                waterSplashing.Stop();
            }
        }

        void OnButtonClicked()
        {
            if ((buttonClicked != null) && (buttonClicked.clip != null))
            {
                buttonClicked.PlayOneShot(buttonClicked.clip);
            }
        }
#pragma warning disable 649
        [SerializeField] CoffeeMakerBehaviour coffeeMaker;
        [SerializeField] AudioSource buttonClicked;
        [SerializeField] AudioSource coffeeBrewing;
        [SerializeField] AudioSource waterSplashing;
        [SerializeField] AudioSource filterLock;
#pragma warning restore 649
    }
}
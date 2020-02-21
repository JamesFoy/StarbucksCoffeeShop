using UnityEngine;
using UnityEngine.Events;
using VRKitchenSimulator.Helpers;
using VRTK;

namespace VRKitchenSimulator.Prototypes
{
    [RequireComponent(typeof(VRTK_InteractableObject))]
    public class CoffeeSachetBehaviour : MonoBehaviour
    {
        readonly ActivationTimer pourTimer;

        VRTK_InteractableObject interactableObject;
        bool haveHitSomething;
        float contents;

        public UnityEvent pour;
        public UnityEvent stopPour;
        public UnityEvent Opened;

        bool pouring;

        public CoffeeSachetBehaviour()
        {
            pourTimer = new ActivationTimer();
        }

        public bool Pouring
        {
            get { return pouring; }
            set
            {
                if (pouring == value)
                {
                    return;
                }

                pouring = value;
                if (pouring)
                {
                    pourTimer.Start();
                    pour.Invoke();
                }
                else
                {
                    contents = Mathf.Max(0, contents - pourTimer.TimePassed);
                    stopPour.Invoke();
                }
            }
        }

        void Awake()
        {
            contents = pouringTime;
            openModel.SetActive(isOpen);
            closedModel.SetActive(!isOpen);
            interactableObject = GetComponent<VRTK_InteractableObject>();
        }

        void OnEnable()
        {
            interactableObject.InteractableObjectUsed += OnUse;
        }

        void OnDisable()
        {
            interactableObject.InteractableObjectUsed -= OnUse;
        }

        void OnUse(object sender, InteractableObjectEventArgs e)
        {
            if (isOpen)
            {
                return;
            }

            isOpen = true;
            Opened.Invoke();
            openModel.SetActive(isOpen);
            closedModel.SetActive(!isOpen);
        }

        void Update()
        {
            if (!isOpen)
            {
                return;
            }

            var direction = (coffeeTarget.transform.position - raycastSource.transform.position).normalized;
            var dotResult = Vector3.Dot(direction, Vector3.down);

            var isPouring = dotResult > 0.8f;
            haveHitSomething = false;
            if (isPouring && (contents > 0))
            {
                Pouring = true;
                pourTimer.Update();

                RaycastHit hit;
                if (Physics.Raycast(raycastSource.transform.position, Vector3.down, out hit, 50))
                {
                    haveHitSomething = true;
                    var maybeFilter = hit.collider.GetComponentInParent<ICoffeePowderReceiver>();
                    if (maybeFilter != null)
                    {
                        maybeFilter.ReceivedCoffeePowder();
                    }
                }

                if (contents - pourTimer.TimePassed <= 0)
                {
                    Pouring = false;
                }

                if (Time.frameCount % 10 == 0)
                {
                    Debug.Log("Pouring : " + pourTimer.TimePassed);
                }
            }
            else
            {
                Pouring = false;
            }
        }

        void OnDrawGizmos()
        {
            if (raycastSource == null)
            {
                return;
            }

            if (haveHitSomething)
            {
                Gizmos.color = Color.yellow;
            }
            else
            {
                Gizmos.color = Color.white;
            }

            Gizmos.DrawLine(raycastSource.transform.position, raycastSource.transform.position + Vector3.down * 10);
        }
#pragma warning disable 649
        [SerializeField] GameObject raycastSource;
        [SerializeField] GameObject coffeeTarget;
        [SerializeField] GameObject openModel;
        [SerializeField] GameObject closedModel;
        [SerializeField] bool isOpen;
        [SerializeField] float pouringTime;
#pragma warning restore 649
    }
}
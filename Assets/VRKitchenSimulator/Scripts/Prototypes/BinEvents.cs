using UnityEngine;
using UnityTutorialSystem.Events;

namespace VRKitchenSimulator.Prototypes
{
    [RequireComponent(typeof(BinBehaviour))]
    public class BinEvents : StreamEventSource
    {
#pragma warning disable 649
        [SerializeField]
        BasicEventStreamMessage BinOpenedMessage;
#pragma warning restore 649

        BinBehaviour binBehaviour;

        void Awake()
        {
            binBehaviour = GetComponent<BinBehaviour>();
        }

        void OnEnable()
        {
            binBehaviour.binOpened.AddListener(OnBinOpened);
        }

        void OnDisable()
        {
            binBehaviour.binOpened.RemoveListener(OnBinOpened);
        }

        void OnBinOpened()
        {
            if (BinOpenedMessage != null)
            {
                BinOpenedMessage.Publish();
            }
        }

        public override bool WillGenerateMessage(BasicEventStreamMessage msg)
        {
            return Equals(BinOpenedMessage, msg);
        }
    }
}
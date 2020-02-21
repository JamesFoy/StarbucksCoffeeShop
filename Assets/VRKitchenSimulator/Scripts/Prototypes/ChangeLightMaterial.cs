using UnityEngine;

namespace VRKitchenSimulator.Prototypes
{
    public class ChangeLightMaterial : MonoBehaviour
    {
        [SerializeField] bool initialState;
        MeshRenderer lightRender;
        bool state;

        public bool State
        {
            get { return state; }
            set
            {
                state = value;
                if (value)
                {
                    lightRender.materials = new[] {emissive};
                    lightSource.SetActive(true);
                }
                else
                {
                    lightRender.materials = new[] {nonEmissive};
                    lightSource.SetActive(false);
                }
            }
        }

        void Awake()
        {
            lightRender = GetComponent<MeshRenderer>();
            State = initialState;
        }

        public void ToggleLight()
        {
            State = !State;
        }
#pragma warning disable 649
        [SerializeField] Material emissive;
        [SerializeField] Material nonEmissive;
        [SerializeField] GameObject lightSource;
#pragma warning restore 649
    }
}
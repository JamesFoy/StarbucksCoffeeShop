using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.Controllables;
using VRTK.Controllables.ArtificialBased;

public class ButtonReset : MonoBehaviour
{
    private VRTK_ArtificialPusher button;

    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponentInChildren<VRTK_ArtificialPusher>();
    }

    private void OnDisable()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{ 
    //this is where we mark the event we're listening to
    public GameEvent Event;

    //this provides a dropdown to show the response
    public UnityEvent Response;

    /*When the object is enabled we register as an event listener 
    with the event and when disabled the opposite*/
    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    //When we raise (trigger) the event, do our responses
    public void OnEventRaised()
    {
        Response.Invoke();
        ManagerUI.Instance.UpdateScore(Event);
    }
}

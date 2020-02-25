using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    public AudioClip correctClip;
    public AudioClip incorrectClip;

    public bool correctEvent;

    //When you create a game event create a new empty list of things listening to it
    public List<GameEventListener> listeners = new List<GameEventListener>();

    //trigger event and find every listener for the event and trigger it on them
    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();

            if (correctEvent)
            {
                //play correct audio sound
            }
            else
            {
                //play incorrect audio sound
            }
        }
    }

    //Tell the event "This is listening"
    public void RegisterListener(GameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }
}

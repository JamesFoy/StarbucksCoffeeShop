using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    public bool isCorrect;
    bool eventHasBeenRaised;
    bool npcEventHasBeenRaised;

    //When you create a game event create a new empty list of things listening to it
    public List<GameEventListener> listeners = new List<GameEventListener>();

    public List<NPCGameEventListener> npcListeners = new List<NPCGameEventListener>();

    //trigger event and find every listener for the event and trigger it on them
    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            if (!eventHasBeenRaised)
            {
                eventHasBeenRaised = true;
                listeners[i].OnEventRaised();
            }
        }

        eventHasBeenRaised = false;

        for (int i = npcListeners.Count - 1; i >= 0; i--)
        {
            if (!npcEventHasBeenRaised)
            {
                npcEventHasBeenRaised = true;
                npcListeners[i].OnEventRaised();
            }
        }

        npcEventHasBeenRaised = false;
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

    //Tell the event "This is listening"
    public void RegisterListener(NPCGameEventListener npcListener)
    {
        npcListeners.Add(npcListener);
    }

    public void UnregisterListener(NPCGameEventListener npcListener)
    {
        npcListeners.Remove(npcListener);
    }
}

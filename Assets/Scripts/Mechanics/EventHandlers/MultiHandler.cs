using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiHandler : MonoBehaviour, EventHandler
{
    [SerializeField] GameObject[] handlersSerialized;
    List<EventHandler> handlers;

    void Start()
    {
        handlers = new List<EventHandler>();
        for(int i = 0; i < handlersSerialized.Length; i++)
        {
            handlers.Add(handlersSerialized[i].GetComponent(typeof(EventHandler)) as EventHandler);
        }
    }

    public void activate()
    {
        for(int i = 0; i < handlers.Count; i++)
        {
            handlers[i].activate();
        }
    }

    public void deactivate()
    {
        for (int i = 0; i < handlers.Count; i++)
        {
            handlers[i].deactivate();
        }
    }
}

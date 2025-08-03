using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverserHandler : MonoBehaviour, EventHandler
{
    [SerializeField] GameObject activator;
    EventHandler handler;

    // Start is called before the first frame update
    void Start()
    {
        handler = activator.GetComponent(typeof(EventHandler)) as EventHandler;
    }

    public void activate()
    {
        handler.deactivate();
    }

    public void deactivate()
    {
        handler.activate();
    }
}

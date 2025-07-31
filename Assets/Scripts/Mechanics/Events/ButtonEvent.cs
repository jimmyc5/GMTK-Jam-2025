using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    private int collisionCount;
    private bool pressing;
    [SerializeField] GameObject buttonSerialized;
    EventHandler button;

    [SerializeField] GameObject[] eventListenersSerialized;
    List<EventHandler> eventListeners;

    // Start is called before the first frame update
    void Start()
    {
        collisionCount = 0;
        button = buttonSerialized.GetComponent(typeof(EventHandler)) as EventHandler;

        eventListeners = new List<EventHandler>();
        for (int i = 0; i < eventListenersSerialized.Length; i++)
        {
            eventListeners.Add(eventListenersSerialized[i].GetComponent(typeof(EventHandler)) as EventHandler);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!pressing)
        {
            pressing = true;
            button.activate();
            foreach(EventHandler e in eventListeners)
            {
                e.activate();
            }
        }
        collisionCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        collisionCount = Mathf.Max(collisionCount - 1, 0);
        if(collisionCount == 0)
        {
            pressing = false;
            button.deactivate();
            foreach (EventHandler e in eventListeners)
            {
                e.deactivate();
            }
        }
    }
}

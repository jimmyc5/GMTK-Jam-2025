using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    public int collisionCount;
    private bool pressing;
    [SerializeField] GameObject buttonSerialized;
    EventHandler button;

    [SerializeField] LayerMask layersToInteractWith;

    [SerializeField] GameObject[] eventListenersSerialized;
    List<EventHandler> eventListeners;

    //public Light pointLight;

    public AudioClip buttonSound;
    private float lastSFXTime = 0f;
    private float SFXCooldown = 0.1f;

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
        if(layersToInteractWith == (layersToInteractWith | (1 << other.gameObject.layer)))
        {
            if (!pressing)
            {
                pressing = true;
                button.activate();
                foreach (EventHandler e in eventListeners)
                {
                    e.activate();
                }
                //if (pointLight)
                //    pointLight.intensity = 1f;
                if (SoundManager.instance && Time.time > lastSFXTime + SFXCooldown)
                {
                    SoundManager.instance.PlaySoundClip(buttonSound, transform.position, 0.5f, 1.2f);
                    lastSFXTime = Time.time;
                }
            }
            collisionCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (layersToInteractWith == (layersToInteractWith | (1 << other.gameObject.layer)))
        {
            collisionCount = Mathf.Max(collisionCount - 1, 0);
            if (collisionCount == 0)
            {
                pressing = false;
                button.deactivate();
                foreach (EventHandler e in eventListeners)
                {
                    e.deactivate();
                }
                //if (pointLight)
                //    pointLight.intensity = 0.5f;
                if (SoundManager.instance && Time.time > lastSFXTime + SFXCooldown)
                {
                    SoundManager.instance.PlaySoundClip(buttonSound, transform.position, 0.5f, 0.8f);
                    lastSFXTime = Time.time;
                }
            }
        }
    }
}

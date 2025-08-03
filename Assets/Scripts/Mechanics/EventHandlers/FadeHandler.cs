using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeHandler : MonoBehaviour, EventHandler
{
    [SerializeField] Image imageFade;
    [SerializeField] SpriteRenderer spriteFade;
    [SerializeField] float fadeOutTime;
    [SerializeField] GameObject fadeOutHandlerSerialized;
    EventHandler fadeOutHandler;
    Color curColor;
    bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        if (imageFade != null)
            curColor = imageFade.color;
        else
            curColor = spriteFade.color;

        if (fadeOutHandlerSerialized != null)
            fadeOutHandler = fadeOutHandlerSerialized.GetComponent(typeof(EventHandler)) as EventHandler;
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            curColor.a -= Time.deltaTime / fadeOutTime;
            if (imageFade != null)
                imageFade.color = curColor;
            else
                spriteFade.color = curColor;
        }
        if (curColor.a < 0)
        {
            curColor.a = 0;
            if(fadeOutHandlerSerialized != null)
                fadeOutHandler.activate();
            gameObject.SetActive(false);
        }
    }

    public void activate()
    {
        activated = true;
    }

    public void deactivate() { }
}

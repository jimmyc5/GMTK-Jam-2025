using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextWriteHandler : MonoBehaviour, EventHandler
{
    private static readonly HashSet<char> punctuationMap = new HashSet<char> { ',', '.', '!', '?' };

    [SerializeField] string textToDisplay;
    [SerializeField] TMP_Text textHolder;
    [SerializeField] GameObject continueCursor;
    [SerializeField] GameObject continueEventSerialized;
    EventHandler continueEvent;
    [SerializeField] bool typewriterEffect;
    [SerializeField] float delayTime;
    [SerializeField] bool requireInput;

    private bool isWriting = false;
    private bool awaitingInput = false;
    private float delayTimer = 0;
    private int tracker = 0;

    public AudioClip continueSound;

    // Start is called before the first frame update
    void Start()
    {
        if (continueEventSerialized != null)
            continueEvent = continueEventSerialized.GetComponent(typeof(EventHandler)) as EventHandler;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWriting)
            return;

        if(awaitingInput)
        { 
            if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
            {
                if (continueEvent != null)
                {
                    if(continueSound)
                        SoundManager.instance.PlaySoundClip(continueSound, transform.position, 0.25f);
                    continueEvent.activate();
                }
                    
                continueCursor.SetActive(false);
                gameObject.SetActive(false);
            }
            return;
        }

        if(tracker == textToDisplay.Length)
        {
            if(requireInput)
            {
                awaitingInput = true;
                continueCursor.SetActive(true);
            }
            else
            {
                if (continueEvent != null)
                    continueEvent.activate();
                gameObject.SetActive(false);
            }
            return;
        }

        if(typewriterEffect)
        {
            if (delayTimer <= 0f)
            {
                delayTimer = delayTime;
                char nextChar = textToDisplay[tracker];
                if (punctuationMap.Contains(nextChar))
                    delayTimer *= 5;
                textHolder.text += nextChar;
                tracker++;
            }
            else
            {
                delayTimer -= Time.deltaTime;
            }
        }
    }

    public void activate()
    {
        isWriting = true;
        if(!typewriterEffect)
        {
            textHolder.text = textToDisplay;
            tracker = textToDisplay.Length;
        }
    }

    public void deactivate() { }
}

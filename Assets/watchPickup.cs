using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class watchPickup : MonoBehaviour
{
    public float rotationSpeed = 1f;
    public TMP_Text gameobjectToActivate;
    public PlayerCauseLoop playerLoop;
    public AudioClip pickupSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, rotationSpeed * Time.deltaTime, 0f));
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerLoop.StartLoopable();
            gameobjectToActivate.text = "Left click on a box to put it in a time loop \n (click on it again to release it)";
            SoundManager.instance.PlaySoundClip(pickupSound, transform.position, 0.25f);
            gameObject.SetActive(false);
        }
    }
}

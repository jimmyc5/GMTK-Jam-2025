using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCauseLoop : MonoBehaviour
{
    private Collider col;
    private Outline outline;
    private float timer;
    public LayerMask loopableMask;
    private PlayerGrab playerGrabScript;
    public bool canCauseLoop = true;
    public GameObject watchInHand;

    // Start is called before the first frame update
    void Start()
    {
        col = null;
        outline = null;
        timer = 0.5f;
        playerGrabScript = GetComponent<PlayerGrab>();
        watchInHand.SetActive(canCauseLoop);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!GlobalParameters.isDead)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            if (canCauseLoop && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var HitInfo, 100.0f, loopableMask))
            {
                if(HitInfo.rigidbody == playerGrabScript.grabbedObject)
                {
                    return;
                }
                LoopController looper = HitInfo.collider.GetComponent<LoopController>();
                if (looper)
                {
                    bool isLooping = looper.ToggleLoop();
                    if (isLooping)
                    {
                        Debug.Log("HERE");
                        looper.transform.parent.GetChild(1).GetComponent<ParticleSystem>().Play();
                        //outline.OutlineColor = GlobalColors.REJECT;
                    }
                    else
                    {
                        looper.transform.parent.GetChild(1).GetComponent<ParticleSystem>().Stop();
                        // outline.OutlineColor = GlobalColors.ACCEPT;
                    }
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Disable object selection temporarily for lag purposes...
        //if (timer > 0)
        //{
        //    timer -= Time.deltaTime;
        //    return;
        //}

        //if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var HitInfo, 100.0f, loopableMask))
        //{
        //    //disableOldOutline();
        //    col = null;
        //    outline = null;
        //    return;
        //}

        //if (HitInfo.collider.tag == "Loopable")
        //{
        //    if(col != HitInfo.collider)
        //    {
        //        disableOldOutline();
        //        col = HitInfo.collider;
        //        enableNewOutline();
        //    }
        //}
        //else
        //{
        //    disableOldOutline();
        //    col = null;
        //    outline = null;
        //}
    }

    void disableOldOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    void enableNewOutline()
    {
        outline = col.GetComponent<Outline>();
        outline.enabled = true;
        timer = 0.25f;
    }

    public void StartLoopable()
    {
        canCauseLoop = true;
        watchInHand.SetActive(true);
    }
}

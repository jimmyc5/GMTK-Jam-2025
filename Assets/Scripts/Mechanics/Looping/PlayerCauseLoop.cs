using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCauseLoop : MonoBehaviour
{
    private Collider col;
    private Outline outline;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        col = null;
        outline = null;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (col != null && Input.GetMouseButtonDown(0))
        {
            LoopController looper = col.GetComponent<LoopController>();
            if (looper)
            {
                bool isLooping = looper.ToggleLoop();
                if (isLooping)
                {
                    col.transform.parent.GetChild(1).GetComponent<ParticleSystem>().Play();
                    outline.OutlineColor = GlobalColors.REJECT;
                }
                else
                {
                    col.transform.parent.GetChild(1).GetComponent<ParticleSystem>().Stop();
                    outline.OutlineColor = GlobalColors.ACCEPT;
                }
            }
        }

        // Disable object selection temporarily for lag purposes...
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            return;
        }

        if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var HitInfo, 100.0f))
        {
            disableOldOutline();
            col = null;
            outline = null;
            return;
        }

        if (HitInfo.collider.tag == "Loopable")
        {
            if(col != HitInfo.collider)
            {
                disableOldOutline();
                col = HitInfo.collider;
                enableNewOutline();
            }
        }
        else
        {
            disableOldOutline();
            col = null;
            outline = null;
        }
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
}

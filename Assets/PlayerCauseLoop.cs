using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCauseLoop : MonoBehaviour
{
    public LayerMask layersToHit;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var HitInfo, 100.0f, layersToHit))
            {
                LoopController looper = HitInfo.collider.GetComponent<LoopController>();
                if (looper)
                {
                    looper.ToggleLoop();
                }
            }
        }
    }
}

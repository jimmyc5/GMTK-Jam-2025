using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCauseLoop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var HitInfo, 100.0f))
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

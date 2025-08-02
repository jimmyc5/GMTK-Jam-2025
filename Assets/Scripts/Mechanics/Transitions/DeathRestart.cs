using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathRestart : MonoBehaviour
{
    private static readonly float RESTART_DELAY_TIME = 2f;

    void OnTriggerEnter(Collider col)
    {
        if (GlobalParameters.isDead)
            return;

        if (col.tag == "Player")
        {
            GlobalParameters.isDead = true;

            Transform playerModel = col.transform.GetChild(0);
            playerModel.GetComponent<Animator>().enabled = false;
            
            Transform root = playerModel.GetChild(1);
            Collider[] colliders = root.GetComponentsInChildren<Collider>();
            foreach(Collider c in colliders)
            {
                c.enabled = true;
            }
            Rigidbody[] bodies = root.GetComponentsInChildren<Rigidbody>();
            foreach(Rigidbody rb in bodies)
            {
                rb.isKinematic = false;
            }

            TransitionManager.instance.RestartSceneAfterDelay(RESTART_DELAY_TIME);
        }
    }
}

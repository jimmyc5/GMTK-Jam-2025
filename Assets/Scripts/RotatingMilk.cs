using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RotatingMilk : MonoBehaviour
{
    [SerializeField] GameObject crosshair;
    [SerializeField] CinemachineBrain cb;
    [SerializeField] GameObject gameOver;
    float rotation = 0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        rotation += (Time.deltaTime * 360) % 360;
        Vector3 curRot = transform.rotation.eulerAngles;
        curRot.y = rotation;
        transform.rotation = Quaternion.Euler(curRot);
    }

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
            foreach (Collider c in colliders)
            {
                c.enabled = true;
            }
            Rigidbody[] bodies = root.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in bodies)
            {
                rb.isKinematic = false;
            }

            crosshair.SetActive(false);
            cb.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            gameOver.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}

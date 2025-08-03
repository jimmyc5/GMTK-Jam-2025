using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectActiveHandler : MonoBehaviour, EventHandler
{
    [SerializeField] List<GameObject> gameObjects;
    [SerializeField] List<MonoBehaviour> scripts;

    public void activate()
    {
        foreach(GameObject o in gameObjects)
            o.SetActive(true);

        foreach (MonoBehaviour s in scripts)
            s.enabled = true;
    }

    public void deactivate()
    {
        foreach(GameObject o in gameObjects)
            o.SetActive(false);

        foreach (MonoBehaviour s in scripts)
            s.enabled = false;
    }
}

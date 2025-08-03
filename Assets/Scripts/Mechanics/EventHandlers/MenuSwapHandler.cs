using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSwapHandler : MonoBehaviour, EventHandler
{
    [SerializeField] private GameObject swapFrom;
    [SerializeField] private GameObject swapTo;

    public void activate()
    {
        swapFrom.SetActive(false);
        swapTo.SetActive(true);
    }

    public void deactivate()
    {
        swapFrom.SetActive(true);
        swapTo.SetActive(false);
    }
}

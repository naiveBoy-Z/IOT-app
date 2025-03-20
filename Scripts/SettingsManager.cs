using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject profileCanvas;
    private bool isOpen;

    private void Start()
    {
        isOpen = false;
    }

    public void ToggleProfile()
    {
        profileCanvas.SetActive(!isOpen);
        isOpen = !isOpen;
    }
}

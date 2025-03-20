using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabManager : MonoBehaviour
{
    public GameObject sensorDataTab, deviceControlTab;

    public void OpenSensorDataTab()
    {
        sensorDataTab.SetActive(true);
        deviceControlTab.SetActive(false);
    }

    public void OpenDeviceControlTab()
    {
        sensorDataTab.SetActive(false);
        deviceControlTab.SetActive(true);
    }
}

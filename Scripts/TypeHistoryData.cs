using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypeHistoryData : MonoBehaviour
{
    public TextMeshProUGUI id, deviceName, actionName, time;

    public void SetData(int idValue, string device, string action, string timeValue)
    {
        id.text = idValue.ToString();
        deviceName.text = device;
        actionName.text = action;
        time.text = ConvertToCustomFormat(timeValue);
    }


    private string ConvertToCustomFormat(string dateTimeStr)
    {
        DateTime dateTime = DateTime.ParseExact(dateTimeStr, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

        return dateTime.ToString("HH:mm dd-MM-yyyy");
    }
}

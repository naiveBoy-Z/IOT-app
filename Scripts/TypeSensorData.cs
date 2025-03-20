using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypeSensorData : MonoBehaviour
{
    public TextMeshProUGUI id, temp, humid, bright, time;

    public void SetData(int idValue, float tempValue, float humidValue, float bightValue, string timeValue)
    {
        id.text = idValue.ToString();
        temp.text = ((int)tempValue).ToString();
        humid.text = ((int)humidValue).ToString();
        bright.text = ((int)bightValue).ToString();
        time.text = ConvertToCustomFormat(timeValue);
    }


    private string ConvertToCustomFormat(string dateTimeStr)
    {
        DateTime dateTime = DateTime.ParseExact(dateTimeStr, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

        return dateTime.ToString("HH:mm dd-MM-yyyy");
    }
}

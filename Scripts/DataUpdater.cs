using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataUpdater : MonoBehaviour
{
    public GameObject graphUpdaterGameobject;
    private GraphUpdater graphUpdater;

    public TextMeshProUGUI txtTemperature, txtHumidity, txtLight;
    private int temperature, humidity, lightIntensity, maxTemperature, maxLightIntensity;
    public Image temperatureGaugeChart, humidityGaugeChart, lightGaugeChart;


    private void Awake()
    {
        graphUpdater = graphUpdaterGameobject.GetComponent<GraphUpdater>();

        humidity = -1;

        txtTemperature.text = "Nhiệt độ:\n0°C";
        txtHumidity.text = "Độ ẩm:\n0%";
        txtLight.text = "Ánh sáng:\n0 lux";

        temperatureGaugeChart.fillAmount = 1f;
        humidityGaugeChart.fillAmount = 1f;
        lightGaugeChart.fillAmount = 1f;
        StartCoroutine(GetDataSensor());
    }

    private void OnEnable()
    {
        if (humidity >= 0)
        {
            UpdateData(temperature, humidity, lightIntensity);
        }
    }


    public void UpdateData(int temp, int humid, int light)
    {
        if (humidity < 0) graphUpdater.InitialMinAndMaxValue(temp, humid / 4f, light / 50f);

        temperature = temp;
        humidity = humid;
        lightIntensity = light;

        if (temp > maxTemperature) maxTemperature = temp;
        if (light > maxLightIntensity) maxLightIntensity = light;


        graphUpdater.AddTemperatureValues(temperature);
        graphUpdater.AddHumidityValues(humidity / 4f);
        graphUpdater.AddLightValues(lightIntensity / 50f);
        graphUpdater.AddTimeValues(DateTime.Now.ToString("HH:mm:ss"));
        graphUpdater.UpdateGraph();


        txtTemperature.text = "Nhiệt độ:\n" + temperature + "°C";
        txtHumidity.text = "Độ ẩm:\n" + humidity + "%";
        txtLight.text = "Ánh sáng:\n" + lightIntensity + " lux";

        temperatureGaugeChart.fillAmount = (float)temperature / maxTemperature;
        humidityGaugeChart.fillAmount = (float)humidity / 100;
        lightGaugeChart.fillAmount = (float)lightIntensity / maxLightIntensity;
    }

    private IEnumerator GetDataSensor()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            UpdateData(20 + UnityEngine.Random.Range(-1, 2), 50 + UnityEngine.Random.Range(-1, 2), 1000 + UnityEngine.Random.Range(-200, 201));
        }
    }
}

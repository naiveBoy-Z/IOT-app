using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using UnityEngine;
using Newtonsoft.Json;

public class MQTTClient : MonoBehaviour
{
    [Header("MQTT config")]
    public string brokerAddress;
    public int brokerPort;
    public string topic = "data", userName, password;

    [Header("Target Object")]
    public DataUpdater dataUpdater;


    private IMqttClient mqttClient;
    private bool getSensorData;
    private SensorData data;


    private class SensorData
    {
        public float temp;
        public float humid;
        public int light;
    }


    async void Start()
    {
        getSensorData = false;
        await ConnectMqtt();
    }

    async Task ConnectMqtt()
    {
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerAddress, brokerPort)
            .WithCredentials(userName, password)
            .WithClientId(Guid.NewGuid().ToString())
            .WithCleanSession()
            .Build();

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

            try
            {
                data = JsonConvert.DeserializeObject<SensorData>(payload);
                getSensorData = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Lỗi parse JSON: {ex.Message}");
            }

            await Task.Yield();
        };

        mqttClient.ConnectedAsync += async e =>
        {
            await mqttClient.SubscribeAsync(topic);
        };

        mqttClient.DisconnectedAsync += async e =>
        {
            Debug.LogWarning("Mất kết nối MQTT. Đang thử kết nối lại...");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await ConnectMqtt();
        };

        try
        {
            await mqttClient.ConnectAsync(options);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Lỗi kết nối MQTT: {ex.Message}");
        }
    }


    private void Update()
    {
        if (getSensorData) {
            dataUpdater.UpdateData(Mathf.RoundToInt(data.temp), Mathf.RoundToInt(data.humid), data.light);
            getSensorData = false;
        }
    }


    void OnApplicationQuit()
    {
        mqttClient?.Dispose();
    }

}

using Newtonsoft.Json;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ControlManager : MonoBehaviour
{
    public TextMeshProUGUI txtFanToggler, txtConditionerToggler, txtLightToggler;
    public Animator fanAnimator, conditionerAnimator;
    public GameObject lightGlow;

    private bool fanState, conditionerState, lightState;


    private void Awake()
    {
        fanState = false;
        conditionerState = false;
        lightState = false;
        GetDevicesState();
    }


    private void OnEnable()
    {
        ChangeTheFanUI(fanState);
        ChangeTheConditionerUI(conditionerState);
        ChangeTheLightUI(lightState);
    }


    #region Hàm được gọi mỗi lần bật/tắt thiết bị
    public void ToggleTheFan()
    {
        fanState = !fanState;
        UpdateHistoryDataTable("Quạt", fanState);
        ChangeTheFanUI(fanState);
    }

    public void ToggleTheConditioner()
    {
        conditionerState = !conditionerState;
        UpdateHistoryDataTable("Điều hòa", conditionerState);
        ChangeTheConditionerUI(conditionerState);
    }

    public void ToggleTheLight()
    {
        lightState = !lightState;
        UpdateHistoryDataTable("Đèn", lightState);
        ChangeTheLightUI(lightState);
    }
    #endregion


    #region Lấy trạng thái hiện tại của các thiết bị
    private void GetDevicesState ()
    {
        StartCoroutine(GetDevicesStateCoroutine());
    }

    private IEnumerator GetDevicesStateCoroutine()
    {
        string url = "http://localhost/iot/get_devices_state.php";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;

            DevicesState response = JsonConvert.DeserializeObject<DevicesState>(jsonResponse);

            if (fanState != (response.fan_state == "BẬT"))
            {
                fanState = !fanState;
                ChangeTheFanUI(fanState);
            }
            if (conditionerState != (response.conditioner_state == "BẬT"))
            {
                conditionerState = !conditionerState;
                ChangeTheConditionerUI(conditionerState);
            }
            if (lightState != (response.light_state == "BẬT"))
            {
                lightState = !lightState;
                ChangeTheLightUI(lightState);
            }
        }
        else
        {
            Debug.Log("Request failed: " + request.error);
        }
    }

    private class DevicesState
    {
        public string fan_state;
        public string conditioner_state;
        public string light_state;
    }
    #endregion


    #region Lưu thao tác điều khiển vào cơ sở dữ liệu và làm mới lịch sử điều khiển...
    private void UpdateHistoryDataTable(string deviceName, bool isOn)
    {
        StartCoroutine(UpdateHistoryTableCoroutine(deviceName, isOn));
    }

    private IEnumerator UpdateHistoryTableCoroutine(string deviceName, bool isOn)
    {
        string url = "http://localhost/iot/update_action_history.php";

        WWWForm form = new();
        form.AddField("device_name", deviceName);
        form.AddField("action_name", isOn ? "BẬT" : "TẮT");

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            
        }
        else
        {
            Debug.Log("Request failed: " + request.error);
        }
    }
    #endregion


    #region Thay đổi UI bộ điều khiển...
    private void ChangeTheFanUI(bool isOn)
    {
        fanAnimator.SetBool("isWorking", isOn);
        if (isOn)
        {
            txtFanToggler.text = "ON";
            txtFanToggler.color = Color.black;
        }
        else
        {
            txtFanToggler.text = "OFF";
            txtFanToggler.color = Color.red;
        }
    }

    private void ChangeTheConditionerUI(bool isOn)
    {
        conditionerAnimator.SetBool("isWorking", isOn);
        if (isOn)
        {
            txtConditionerToggler.text = "ON";
            txtConditionerToggler.color = Color.black;
        }
        else
        {
            txtConditionerToggler.text = "OFF";
            txtConditionerToggler.color = Color.red;
        }
    }

    private void ChangeTheLightUI(bool isOn)
    {
        lightGlow.SetActive(isOn);
        if (isOn)
        {
            txtLightToggler.text = "ON";
            txtLightToggler.color = Color.black;
        }
        else
        {
            txtLightToggler.text = "OFF";
            txtLightToggler.color = Color.red;
        }
    }
    #endregion
}

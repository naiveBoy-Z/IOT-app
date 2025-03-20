using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class PanelManager : MonoBehaviour
{
    public RectTransform scrollViewContent;
    public GameObject txtHistoryPrefab;

    private void OnEnable()
    {
        UpdateHistoryPanel();
    }

    public void UpdateHistoryPanel()
    {
        StartCoroutine(UpdateRecentManipulations());
    }

    private IEnumerator UpdateRecentManipulations()
    {
        string url = "http://localhost/iot/get_action_history.php";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;

            if (jsonResponse != "[]")
            {
                ControlAction[] actions = JsonHelper.FromJson<ControlAction>(jsonResponse);

                foreach (RectTransform child in scrollViewContent) Destroy(child.gameObject);

                foreach (var action in actions)
                {
                    GameObject txtHistory = Instantiate(txtHistoryPrefab);
                    txtHistory.GetComponent<RectTransform>().SetParent(scrollViewContent, false);
                    txtHistory.GetComponent<TextMeshProUGUI>().text = "[" + ConvertToCustomFormat(action.time) + "]: " + action.action_name + " " + action.device_name;
                }
            }
        }
        else
        {
            Debug.Log("Request failed: " + request.error);
        }
    }

    public static string ConvertToCustomFormat(string dateTimeStr)
    {
        DateTime dateTime = DateTime.ParseExact(dateTimeStr, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

        return dateTime.ToString("HH:mm dd-MM-yyyy");
    }

    [System.Serializable]
    public class ControlAction
    {
        public int id;
        public string time;
        public string device_name;
        public string action_name;
    }
}


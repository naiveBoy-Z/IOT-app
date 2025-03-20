using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Globalization;
using System;

public class HistoryTableManager : MonoBehaviour
{
    [Header("API")]
    public string api;

    [Header("Data View")]
    public RectTransform scrollViewContent;
    public GameObject dataPrefab;

    [Header("Pagination Controls")]
    public TMP_InputField edtPageSize;
    public TextMeshProUGUI txtPaginationInfo;
    public GameObject btnPrev, btnNext;

    [Header("Filter")]
    public TextMeshProUGUI txtRemoveFilter;
    public GameObject removeFilterObject;
    public TMP_InputField edtTimeValue;



    private int pageSize, currentPage, filterNum;
    private string condition1, condition2;
    private bool appliedTimeFilter;
    private Coroutine queryData, activateKeyboard;



    private void Awake()
    {
        pageSize = 10;
        edtPageSize.text = pageSize.ToString();
        currentPage = 1;

        filterNum = 0;
        condition1 = null; condition2 = null;

        removeFilterObject.SetActive(false);
        appliedTimeFilter = false;
    }

    private void OnEnable()
    {
        QueryDatabase();
    }


    #region Truy vấn cơ sở dữ liệu
    private void QueryDatabase()
    {
        if (queryData != null)
        {
            StopCoroutine(queryData);
        }
        queryData = StartCoroutine(QueryDataCoroutine());
    }

    private IEnumerator QueryDataCoroutine()
    {
        string url = "http://localhost/iot/" + api + ".php";

        WWWForm form = new();
        form.AddField("page_size", pageSize);
        form.AddField("current_page", currentPage);
        if (condition1 != null) form.AddField("data_condition", condition1);
        if (condition2 != null) form.AddField("time_condition", condition2);

        UnityWebRequest request = UnityWebRequest.Post(url, form);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;

            ApiResponse response = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);


            txtPaginationInfo.text = $"Hiển thị {pageSize * (currentPage - 1) + 1}-{Mathf.Min(pageSize * currentPage, response.count)} trên tổng số {response.count}";

            if (currentPage != 1)
            {
                btnPrev.SetActive(true);
            }
            else
            {
                btnPrev.SetActive(false);
            }

            if (currentPage == Mathf.Ceil((float)response.count / pageSize))
            {
                btnNext.SetActive(false);
            }
            else
            {
                btnNext.SetActive(true);
            }

            if (response != null)
            {
                // Xóa danh sách cũ
                foreach (Transform child in scrollViewContent) Destroy(child.gameObject);

                // Hiển thị dữ liệu mới
                int stt = pageSize * (currentPage - 1) + 1;
                foreach (var item in response.history)
                {
                    GameObject data = Instantiate(dataPrefab);
                    data.GetComponent<RectTransform>().SetParent(scrollViewContent, false);
                    data.GetComponent<TypeHistoryData>().SetData(stt++, item.device_name, item.action_name, item.time);
                }
            }
        }
        else
        {
            Debug.Log("Request failed: " + request.error);
        }
    }

    private class ApiResponse
    {
        public int count;
        public List<HistoryData> history;
    }

    private class HistoryData
    {
        public int id;
        public string time;
        public string device_name;
        public string action_name;
    }
    #endregion


    #region Các hàm được gọi khi tương tác với UI
    public void GoToNextPage()
    {
        currentPage++;
        QueryDatabase();
    }

    public void GoToPreviousPage()
    {
        currentPage--;
        QueryDatabase();
    }


    public void ApplyTimeFilter()
    {
        string timeValue = edtTimeValue.text;

        if (!appliedTimeFilter && timeValue != "")
        {
            appliedTimeFilter = true;
            filterNum++;
            removeFilterObject.SetActive(true);
            txtRemoveFilter.text = $"đang dùng {filterNum} bộ lọc";

            currentPage = 1;
            condition2 = $"time LIKE \"{DateTime.ParseExact(timeValue, "HH:mm dd-MM-yyyy", CultureInfo.InvariantCulture):yyyy-MM-dd HH:mm}%\"";

            QueryDatabase();
        }

    }

    public void RemoveFilter()
    {
        appliedTimeFilter = false;
        filterNum = 0;
        removeFilterObject.SetActive(false);

        currentPage = 1;

        condition1 = null;
        condition2 = null;

        QueryDatabase();
    }


    public void ApplyChangedPageSize()
    {
        currentPage = 1;
        pageSize = int.Parse(edtPageSize.text);
        QueryDatabase();
    }


    public void OpenKeyboard(TMP_InputField inpFld)
    {
        TouchScreenKeyboard keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad, false, false, false, false, "", 12);
        activateKeyboard = StartCoroutine(OpenKeyboardCoroutine(keyboard, inpFld));
    }

    private IEnumerator OpenKeyboardCoroutine(TouchScreenKeyboard keyboard, TMP_InputField inpFld)
    {
        while (keyboard.active)
        {
            inpFld.text = keyboard.text;
            yield return null;

        }
    }
    #endregion
}

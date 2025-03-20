using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphUpdater : MonoBehaviour
{
    #region Khai báo biến
    [Header("Graph UI Settings")]
    public RectTransform graphView;
    public List<RectTransform> graphTemperatureLines, graphHumidityLines, graphLightLines;
    public List<TextMeshProUGUI> timePoints;

    [Header("Display Graph Settings")]
    public float translatingTime;
    public int maxVisibleValues;


    private float translatingDistance;
    private List<float> temperatureValues = new(), humidityValues = new(), lightValues = new();
    private List<string> timeValues = new();
    private float minValue, maxValue, graphViewWidth, graphViewHeight;
    #endregion



    private void Start()
    {
        translatingTime = 0.2f;
        maxVisibleValues = 4;
        graphViewWidth = graphView.sizeDelta.x;
        graphViewHeight = graphView.sizeDelta.y;

        translatingDistance = graphViewWidth / (maxVisibleValues - 1);
    }


    public void InitialMinAndMaxValue(float temp, float humid, float light)
    {
        minValue = Mathf.Min(temp, Mathf.Min(humid, light));
        maxValue = Mathf.Max(temp, Mathf.Max(humid, light));
    }


    #region Thêm các dữ liệu vào List<>
    public void AddTemperatureValues(float value)
    {
        temperatureValues.Add(value);
        if (temperatureValues.Count > maxVisibleValues + 1) temperatureValues.RemoveAt(0);
        minValue = Mathf.Min(value, minValue);
        maxValue = Mathf.Max(value, maxValue);
    }

    public void AddHumidityValues(float value)
    {
        humidityValues.Add(value);
        if (humidityValues.Count > maxVisibleValues + 1) humidityValues.RemoveAt(0);
        minValue = Mathf.Min(value, minValue);
        maxValue = Mathf.Max(value, maxValue);
    }

    public void AddLightValues(float value)
    {
        lightValues.Add(value);
        if (lightValues.Count > maxVisibleValues + 1) lightValues.RemoveAt(0);
        minValue = Mathf.Min(value, minValue);
        maxValue = Mathf.Max(value, maxValue);
    }

    public void AddTimeValues(string time)
    {
        timeValues.Add(time);
        if (timeValues.Count > maxVisibleValues + 1) timeValues.RemoveAt(0);
    }
    #endregion


    public void UpdateGraph()
    {
        DrawGraph(graphTemperatureLines, temperatureValues);
        DrawGraph(graphHumidityLines, humidityValues);
        DrawGraph(graphLightLines, lightValues);
        DisplayTimePoints();
    }

    private void DrawGraph(List<RectTransform> lines, List<float> values)
    {
        if (values.Count == 1)
        {
            DrawPoint(lines[0], values[0]);
        }
        else if (values.Count <= maxVisibleValues)
        {
            for (int i = 0; i < values.Count - 1; i++)
            {
                DrawLine(i, lines[i], values[i], values[i + 1], graphViewWidth / (values.Count - 1));
            }
            lines[values.Count - 2].gameObject.SetActive(true);
        }
        else
        {
            lines[^1].gameObject.SetActive(true);

            for (int i = 0; i < maxVisibleValues; i++)
            {
                DrawLine(i, lines[i], values[i], values[i + 1], graphViewWidth / (maxVisibleValues - 1));
            }

            // tịnh tiến các đường về bên trái
            foreach (RectTransform line in lines)
            {
                TranslateToLeft(line, translatingDistance);
            }
        }
    }

    private void DrawPoint(RectTransform rect, float a)
    {
        rect.anchoredPosition = new Vector2(9, (a - minValue) / (maxValue - minValue) * graphViewHeight + 16);
        rect.sizeDelta = new Vector2(16, 16);
    }

    private void DrawLine(int order, RectTransform rect, float a, float b, float milestoneWidth)
    {
        // thiết lập vị trí của 1 đầu của đoạn thẳng
        rect.pivot = new Vector2(0, 0.5f);
        float xPosition = order * milestoneWidth - 8;
        float yPosition = ((a - minValue) / (maxValue - minValue) * 0.9f + 0.05f) * graphViewHeight;
        rect.anchoredPosition = new Vector2(xPosition, yPosition);

        // quay đoạn thẳng
        rect.pivot = new Vector2(8 / (milestoneWidth + 16), 0.5f);
        float k = (b - a) / (maxValue - minValue) * 0.9f * graphViewHeight; // k = cạnh đối
        float angle = Mathf.Atan2(k, milestoneWidth) * Mathf.Rad2Deg; // góc xoay
        rect.rotation = Quaternion.Euler(0f, 0f, angle);

        // tính độ dài đoạn thẳng
        rect.pivot = new Vector2(0, 0.5f);
        float lineLength = Mathf.Sqrt(k * k + milestoneWidth * milestoneWidth) + 8; // cạnh huyền
        rect.sizeDelta = new Vector2(lineLength, 16);
    }

    private void DisplayTimePoints()
    {
        if (temperatureValues.Count <= maxVisibleValues)
        {
            int lastIndex = temperatureValues.Count - 1;
            for (int i = 0; i <= lastIndex; i++)
            {
                timePoints[i].text = timeValues[i];
                if (i > 0)
                {
                    RectTransform rect = timePoints[i].GetComponent<RectTransform>();
                    Vector2 pos = rect.anchoredPosition;
                    pos.x = i * 810 / lastIndex;
                    rect.anchoredPosition = pos;
                }
            }
            timePoints[lastIndex].gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < maxVisibleValues - 1; i++)
            {
                timePoints[i].text = timeValues[i + 1];
                RectTransform rect = timePoints[i].GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + 270, rect.anchoredPosition.y);
                TranslateToLeft(rect, 270);
            }

            timePoints[^1].text = timeValues[^1];
            RectTransform lastRect = timePoints[^1].GetComponent<RectTransform>();
            StartCoroutine(ToggleTimePoint(lastRect));
        }
    }

    private void TranslateToLeft(RectTransform rect, float distance)
    {
        StartCoroutine(TranslateCoroutine(rect, distance));
    }

    private IEnumerator TranslateCoroutine(RectTransform rect, float distance)
    {
        float xTargetPosition = rect.anchoredPosition.x - distance;
        while (rect.anchoredPosition.x > xTargetPosition)
        {
            yield return null;
            float xPosition = rect.anchoredPosition.x - (1 / translatingTime) * Time.deltaTime * distance;
            if (xPosition < xTargetPosition) xPosition = xTargetPosition;
            rect.anchoredPosition = new Vector2(xPosition, rect.anchoredPosition.y);
        }
    }

    private IEnumerator ToggleTimePoint(RectTransform rect)
    {
        rect.gameObject.SetActive(false);
        yield return new WaitForSeconds(translatingTime);
        rect.gameObject.SetActive(true);
    }
}

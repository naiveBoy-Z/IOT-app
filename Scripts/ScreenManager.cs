using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    private RectTransform rectTransform;
    public RectTransform accountView, bottomNavBar;
    private float bottomPadding, scalingFactor;

    private void Start()
    {
        scalingFactor = 1080f / Screen.width;
        rectTransform = GetComponent<RectTransform>();
        Rect safeArea = Screen.safeArea;
        rectTransform.anchoredPosition = new Vector2(safeArea.x * scalingFactor, -safeArea.y * scalingFactor);
        rectTransform.sizeDelta = new Vector2(safeArea.width * scalingFactor, safeArea.height * scalingFactor);

        float topPadding = safeArea.y;
        if (topPadding > 0)
        {
            accountView.sizeDelta = new Vector2(accountView.sizeDelta.x, accountView.sizeDelta.y + topPadding);
            accountView.anchoredPosition = new Vector2(accountView.anchoredPosition.x, accountView.anchoredPosition.y + topPadding);
        }

        bottomPadding = 35;
        float safeAreaMargin = safeArea.yMax - Screen.height;
        if (safeAreaMargin < 0)
        {
            bottomNavBar.sizeDelta = new Vector2(bottomNavBar.sizeDelta.x, bottomNavBar.sizeDelta.y + bottomPadding);
            bottomNavBar.anchoredPosition = new Vector2(bottomNavBar.anchoredPosition.x, safeAreaMargin);
        }
    }
}

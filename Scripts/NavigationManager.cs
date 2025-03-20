using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{
    public RectTransform indicator1, indicator2, indicator3, indicator4;
    private RectTransform currentIndicator;
    public GameObject activity1, activity2, activity3, activity4;
    private GameObject currentActivity;
    public float seconds;

    private void Start()
    {
        currentIndicator = indicator1;
        currentActivity = activity1;
        seconds = 0.2f;
    }

    public void NavigateToDashboardActivity()
    {
        if (currentIndicator != indicator1)
        {
            StartCoroutine(AnimateIndicator(currentIndicator, indicator1));
            NavigateToOtherActivity(activity1);
        }
    }

    public void NavigateToDataActivity()
    {
        if (currentIndicator != indicator2)
        {
            StartCoroutine(AnimateIndicator(currentIndicator, indicator2));
            NavigateToOtherActivity(activity2);
        }
    }

    public void NavigateToControlActivity()
    {
        if (currentIndicator != indicator3)
        {
            StartCoroutine(AnimateIndicator(currentIndicator, indicator3));
            NavigateToOtherActivity(activity3);
        }
    }

    public void NavigateToHistoryActivity()
    {
        if (currentIndicator != indicator4)
        {
            StartCoroutine(AnimateIndicator(currentIndicator, indicator4));
            NavigateToOtherActivity(activity4);
        }
    }

    private IEnumerator AnimateIndicator(RectTransform currentIndicator, RectTransform targetIndicator)
    {
        while (currentIndicator.sizeDelta.x > 0)
        {
            float width = currentIndicator.sizeDelta.x - 270 / seconds * Time.deltaTime;
            if (width < 0) width = 0;
            currentIndicator.sizeDelta = new Vector2(width, currentIndicator.sizeDelta.y);
            yield return null;
        }

        while (targetIndicator.sizeDelta.x < 270)
        {
            float width = targetIndicator.sizeDelta.x + 270 / seconds * Time.deltaTime;
            if (width > 270) width = 270;
            targetIndicator.sizeDelta = new Vector2(width, targetIndicator.sizeDelta.y);
            yield return null;
        }

        this.currentIndicator = targetIndicator;
    }

    private void NavigateToOtherActivity(GameObject activity)
    {
        currentActivity.SetActive(false);
        activity.SetActive(true);
        currentActivity = activity;
    }
}

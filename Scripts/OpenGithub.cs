using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenGithub : MonoBehaviour
{
    public void OpenLink()
    {
        string url = "https://github.com/naiveBoy-Z";
        Application.OpenURL(url);
    }
}

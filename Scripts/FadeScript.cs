using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScript : MonoBehaviour
{
    
    private void Awake()
    {
        GetComponent<RectTransform>().position = new Vector2(Screen.width / 2, Screen.height / 2);
    }
}

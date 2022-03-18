using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeScript : MonoBehaviour
{
    int volume; // 0 min, 10 max
    public GameObject[] rectangles;
    Text text;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    public void ChangeVolume(bool isRight)
    {
        if (isRight)
        {
            if(volume < 10)
                volume++;
        }
        else
        {
            if (volume > 0)
                volume--;
        }
        for(int i = 0; i < 10; i++) // sets rectangles to proper visual indication of volume level
        {
            if (volume - i > 0)
                rectangles[i].SetActive(true);
            else
                rectangles[i].SetActive(false);
        }
        audioSource.volume = ((float)volume) / 10f;
    }

    public void RefreshVolume()
    {
        for (int i = 0; i < 10; i++) // sets rectangles to proper visual indication of volume level
        {
            if (volume - i > 0)
                rectangles[i].SetActive(true);
            else
                rectangles[i].SetActive(false);
        }
    }

    public void HideText()
    {
        text.enabled = false;
    }

    public void ShowText()
    {
        text.enabled = true;
    }

    public int GetVolume()
    {
        return volume;
    }

    public void SetVolume(int v)
    {
        volume = v;
    }

}

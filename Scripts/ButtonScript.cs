using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    SceneManagerScript smS;
    public bool mainMenu;
    public GameObject[] mainButtons;
    public GameObject[] optionsButtons;
    public GameObject[] selectLevelButtons;
    public VolumeScript volumeS;
    public GameObject[] pauseButtons;
    PlayerScript playerS;

    // Start is called before the first frame update
    void Start()
    {
        smS = GameObject.FindGameObjectWithTag("scenemanager").GetComponent<SceneManagerScript>();
        if (!mainMenu)
        {
            playerS = GameObject.FindWithTag("player").GetComponent<PlayerScript>();
        }
    }

    public void LoadGame()
    {
        smS.LoadNewScene(smS.GetReadString("SaveFile"));
    }

    public void SaveGame()
    {
        smS.SaveLevel("Level" + smS.GetScene());
    }

    public void CallLoadNewScene(string s)
    {
        smS.LoadNewScene(s);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetToMain()
    {
        for (int i = 0; i < mainButtons.Length; i++)
        {
            mainButtons[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < optionsButtons.Length; i++)
        {
            optionsButtons[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < selectLevelButtons.Length; i++)
        {
            selectLevelButtons[i].gameObject.SetActive(false);
        }
        volumeS.HideText();
        this.gameObject.SetActive(false); // since what is clicked must be on the options page
    }

    public void SetToOptions()
    {
        for (int i = 0; i < mainButtons.Length; i++)
        {
            mainButtons[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < optionsButtons.Length; i++)
        {
            optionsButtons[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < selectLevelButtons.Length; i++)
        {
            selectLevelButtons[i].gameObject.SetActive(false);
        }
        volumeS.RefreshVolume();
        volumeS.ShowText();
        this.gameObject.SetActive(false); // since what is clicked must be on the main page
    }

    public void SetToSelectLevel()
    {
        for (int i = 0; i < mainButtons.Length; i++)
        {
            mainButtons[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < optionsButtons.Length; i++)
        {
            optionsButtons[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < selectLevelButtons.Length; i++)
        {
            selectLevelButtons[i].gameObject.SetActive(true);
        }
        volumeS.HideText();
        this.gameObject.SetActive(false); // since what is clicked must be on the main page
    }

    public void ResumeGame()
    {
        foreach (GameObject b in pauseButtons)
            b.gameObject.SetActive(false);
        playerS.UnpauseMenu();
        Time.timeScale = 1;
        this.gameObject.SetActive(false);
    }

}

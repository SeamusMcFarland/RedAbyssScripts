using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadPlayerScript : MonoBehaviour
{
    AnimationScript animationS;
    bool walking;
    const float WALKING_SPEED = 2.5f;
    bool playingWalk;

    GameObject[] audioM;
    AudioScript audioS;
    const float AUDIO_NUM = 1;

    PlayerScript playerS;

    public Text myName;
    public Text[] allDeathText;
    UIManagerScript uimanagerS;
    SceneManagerScript smS;

    // Start is called before the first frame update
    void Start()
    {
        uimanagerS = GameObject.FindGameObjectWithTag("uimanager").GetComponent<UIManagerScript>();
        playerS = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerScript>();
        animationS = GetComponent<AnimationScript>();
        SetupAudio();
        smS = GameObject.FindGameObjectWithTag("scenemanager").GetComponent<SceneManagerScript>();
    }

    private void SetupAudio()
    {
        audioM = GameObject.FindGameObjectsWithTag("audiomanager");
        int searching = 0;
        while (searching != -1)
        {
            if (audioM[searching].GetComponent<AudioScript>().GetAudioNum() == AUDIO_NUM)
            {
                audioS = audioM[searching].GetComponent<AudioScript>();
                searching = -1;
            }
            else if (searching == 10)
                print("ERROR! MISSING AUDIOMANAGER");
            else
                searching++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (walking)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + WALKING_SPEED * Time.deltaTime);
            
            if (!playingWalk)
            {
                playingWalk = true;
                audioS.PlayPlayerWalk();
                StartCoroutine("EndWalkSound", 0.9f);
            }
        }
    }

    IEnumerator EndWalkSound(float wait) // prevents multiple walk sounds from being played at once
    {
        yield return new WaitForSeconds(wait);
        playingWalk = false;
    }

    public void GetUp()
    {
        StartCoroutine("DelayGetUp");
    }

    IEnumerator DelayGetUp()
    {
        yield return new WaitForSeconds(3f);
        animationS.SetState(3);
        yield return new WaitForSeconds(3f);
        transform.rotation = Quaternion.Euler(90f, 270f, 0);
        yield return new WaitForSeconds(1f);
        walking = true;
        yield return new WaitForSeconds(4f);
        walking = false;
        yield return new WaitForSeconds(1f);
        uimanagerS.ApplyFade();
        foreach(Text dt in allDeathText)
            dt.enabled = false;
        yield return new WaitForSeconds(5f);
        myName.enabled = true;
        yield return new WaitForSeconds(4f);
        myName.enabled = false;
        yield return new WaitForSeconds(4f);
        smS.LoadNewScene("MainMenu");
    }


}

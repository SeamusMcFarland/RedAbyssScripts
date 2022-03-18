using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueScript : MonoBehaviour
{

    GameObject[] audioM;
    AudioScript audioS;
    const int AUDIO_NUM = 2; // monster audio manager

    public JacobScript jacobS;
    UIManagerScript uimS;
    Text theText;
    string dialogueChoice;
    int dialogueI; // dialogue iterator, progressing over the different dedicated text documents
    bool finalDoc;
    int fadeoutScene; // scene in which fadeout music was called
    //string[] splitTextString;

    const float FULL_DELAY = 2.2f;
    const float TYPE_RATE = 0.02f;

    SceneManagerScript smS;
    string textString;
    int iteratedDocument;

    bool available; // if false, will wait to display dialogue
    List<string> dialogueQueue = new List<string>();

    AudioSource musicSource;
    bool everyOther;

    bool dInterrupt; // halts all progress of typing out dialogue

    LectureJuneScript lectureJuneS;

    // Start is called before the first frame update
    void Start()
    {
        musicSource = GameObject.Find("MusicManager").GetComponent<AudioSource>();
        uimS = GameObject.FindWithTag("uimanager").GetComponent<UIManagerScript>();
        available = true;
        smS = GameObject.FindGameObjectWithTag("scenemanager").GetComponent<SceneManagerScript>();
        theText = GetComponent<Text>();

        if (smS.GetScene() == 4)
        {
            LectureJuneScript[] allLJS = GameObject.Find("AllLectureJunes").GetComponentsInChildren<LectureJuneScript>();
            foreach (LectureJuneScript ljs in allLJS)
            {
                if (ljs.GetFinal())
                {
                    lectureJuneS = ljs;
                    break;
                }
            }
        }

        SetupAudio();
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

    public void PrintDialogue(string dc)
    {
        print("dialogue: " + dc + " available?: " + available + " at: " + Time.time);
        if (available && !dInterrupt)
        {
            available = false;
            print("now available for: " + dc);
            NowAvailable(dc);
        }
        else
        {
            print("not available!");
            dialogueQueue.Add(dc);
            StartCoroutine("DelayDialogue");
        }
    }

    IEnumerator DelayDialogue()
    {
        yield return new WaitForSeconds(0.2f);
        if (dialogueQueue.Count > 0)
        {
            if (available && !dInterrupt)
            {
                print("delaying at: " + Time.time);
                available = false;
                print("now available for: " + dialogueQueue[0]);
                NowAvailable(dialogueQueue[0]);
                dialogueQueue.RemoveAt(0);
            }

            StartCoroutine("DelayDialogue");
        }
    }

    private void NowAvailable(string dc)
    {
        print("now available: " + dc + " with interrupt?: " + dInterrupt);

        if (!dInterrupt)
        {
            dialogueChoice = dc;

            switch (dialogueChoice) // specifies how many text documents are to be read for the entered string
            {
                case "FirstTutorial":
                    dialogueI = 4;
                    break;
                case "JacobIntro":
                    dialogueI = 3;
                    break;
                case "JacobMimic":
                    dialogueI = 2;
                    break;
                case "JacobMid":
                    dialogueI = 2;
                    break;
                case "Keisler1":
                    dialogueI = 17;
                    break;
                case "Keisler2":
                    dialogueI = 16;
                    break;
                case "Keisler3":
                    dialogueI = 22;
                    break;
                case "Lemmer1":
                    dialogueI = 3;
                    break;
                case "Lemmer2":
                    dialogueI = 2;
                    break;
                case "LemmerBoss1":
                    dialogueI = 2;
                    break;
                case "LectureJune7":
                    dialogueI = 2;
                    break;
                case "LectureJune8":
                    dialogueI = 3;
                    break;
                case "LemmerBegging1":
                    dialogueI = 2;
                    break;
                default:
                    dialogueI = 1;
                    break;
            }
            StartCoroutine("DialogueIteration");
        }
    }

    IEnumerator DialogueIteration()
    {
        finalDoc = false;
        for (int i = 1; i <= dialogueI; i++)
        {
            string currentDialogue = dialogueChoice;
            print("dialogue iteration... document: " + i + " aquired text string: " + smS.GetReadString(dialogueChoice + i));
            iteratedDocument = i;
            textString = smS.GetReadString(dialogueChoice + i);
            if (i == dialogueI)
                finalDoc = true;
            StartCoroutine("TypeEffect", textString.Length);
            print("location 1 on dialogue i: " + i);
            int localLengthSave = textString.Length;
            print("location 2 on dialogue i: " + i);
            float storedWait = FULL_DELAY + (localLengthSave * TYPE_RATE + ((float)localLengthSave) / 30f);
            print("location 3 on dialogue i: " + i + " with dialogue choice: " + dialogueChoice);
            yield return new WaitForSeconds(storedWait);
            if (dInterrupt || currentDialogue != dialogueChoice)
                break;
        }
    }

    IEnumerator TypeEffect(int numLeft)
    {
        yield return new WaitForSeconds(TYPE_RATE);
        if (!dInterrupt)
        {
            if (everyOther)
            {
                print("point 1 for dialogueChoice: " + dialogueChoice);
                if (dialogueChoice.Contains("Helpless") || dialogueChoice.Contains("June"))
                {
                    print("point 2 for dialogueChoice: " + dialogueChoice);
                    if (!(dialogueChoice.Contains("LectureJune7") && iteratedDocument == 2))
                        audioS.PlayClick2();
                }
                else if (dialogueChoice.Contains("Keisler"))
                {
                    if (smS.GetScene() == -1)
                    {
                        if (iteratedDocument != 9)
                        {
                            audioS.PlayPaperGlitch();
                        }
                    }
                    else if (smS.GetScene() == -2)
                    {
                        if (iteratedDocument != 8 && iteratedDocument != 12 && iteratedDocument != 16)
                        {
                            audioS.PlayPaperGlitch();
                        }
                    }
                    else if (smS.GetScene() == -3)
                    {
                        audioS.PlayPaperGlitch();
                    }
                    print("point 2 ELSE for dialogueChoice: " + dialogueChoice);
                }
                else if (dialogueChoice.Contains("Cronenburg"))
                    audioS.PlayDeepBreath();
                else if (dialogueChoice.Contains("Lemmer") && !(dialogueChoice.Contains("Boss1") && iteratedDocument == 3))
                    audioS.PlayMoan();
                else if (dialogueChoice.Contains("Tortured"))
                    audioS.PlayCrackle();
                else if (dialogueChoice.Contains("Jacob"))
                    audioS.PlayClick1();
            }
            everyOther = !everyOther;
            print("point 3 for dialogueChoice: " + dialogueChoice + " with textString: " + textString + " numLeft: " + numLeft);
            theText.text = textString.Substring(0, textString.Length - numLeft);
            print("point 3 AFTER for dialogueChoice: " + dialogueChoice + " with textString: " + textString);
            numLeft--;
            if (numLeft >= 0)
                StartCoroutine("TypeEffect", numLeft);
            else if (finalDoc)
                StartCoroutine("AfterDelay");
        }
    }

    bool afterDelaying;

    IEnumerator AfterDelay()
    {
        if (!afterDelaying)
        {
            afterDelaying = true;
            print("after delay called");
            finalDoc = false;
            if (dialogueChoice == "JacobDeath")
                theText.color = Color.black;
            float passedTime = 0;
            float recordedTime;
            while (FULL_DELAY * 1.5f > passedTime)
            {
                recordedTime = Time.time;
                if (!(dInterrupt && dialogueQueue.Count > 0))
                {
                    yield return new WaitForSeconds(TYPE_RATE); // TYPE_RATE is minimum this can be delayed
                    passedTime += Time.time - recordedTime;
                }
                else
                {
                    yield return new WaitForSeconds(TYPE_RATE); // TYPE_RATE is minimum this can be delayed
                    break;
                }
            }
            if (!(!afterDelaying && dInterrupt))
            {
                theText.color = Color.white;
                theText.text = "";
                dInterrupt = false;
                available = true;

                if (dialogueChoice == "JacobIntro")
                    jacobS.Activate();
                if (smS.GetScene() == -1 || smS.GetScene() == -2)
                {
                    EndCutscene();
                }
                else if(dialogueChoice == "LectureJune8")
                    lectureJuneS.ForwardEnd();

                afterDelaying = false;
            }
        }
    }

    public void EndCutscene()
    {
        uimS.OutroScreen();
        audioS.PlayMetalBang();
        StartCoroutine("DelayNextScene");
    }

    IEnumerator DelayNextScene()
    {
        fadeoutScene = smS.GetScene();
        StartCoroutine("FadeoutMusic");
        yield return new WaitForSeconds(3f);
        if(dialogueChoice == "Keisler1")
            smS.LoadNewScene("Level1");
        else if (dialogueChoice == "Keisler2")
            smS.LoadNewScene("Level2");
        else if (smS.GetScene() == -3)
            smS.LoadNewScene("Level3");
    }

    IEnumerator FadeoutMusic()
    {
        yield return new WaitForSeconds(0.1f);
        if (fadeoutScene == smS.GetScene())
        {
            print("fading out at: " + Time.time);
            musicSource.volume -= 0.02f;
            StartCoroutine("FadeoutMusic");
        }
    }

    public void NotifyWhenDone(CutsceneKeislerScript ckS)
    {
        StartCoroutine("NotifyDelay", ckS);
    }

    IEnumerator NotifyDelay(CutsceneKeislerScript ckS)
    {
        yield return new WaitForSeconds(0.1f);
        while (iteratedDocument < 16)
        {
            while (iteratedDocument % 2 == 0)
            {
                yield return new WaitForSeconds(0.1f);
            }
            ckS.DilogueNotified();
            while (iteratedDocument % 2 == 1)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public void InterruptDialogue()
    {
        dInterrupt = true;
        theText.color = Color.black;
        StartCoroutine("AfterDelay");
    }

    public void ClearDialogueQueue()
    {
        dialogueQueue.Clear();
    }

}

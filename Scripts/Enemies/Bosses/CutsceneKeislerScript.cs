using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneKeislerScript : MonoBehaviour
{
    public Material[] allMaterials;
    public Material deathMaterial;
    private bool dead;
    BloodManagerScript bloodMS;
    MeshRenderer mr;
    int currentImage;
    public DialogueScript dialogueS;
    CutsceneKeislerScript ownScript;
    SceneManagerScript smS;
    Vector3 savedPosition;
    int animIterator;

    const float ANIMATION_SPEED = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        bloodMS = GameObject.FindGameObjectWithTag("bloodmanager").GetComponent<BloodManagerScript>();
        dead = false;
        smS = GameObject.FindWithTag("scenemanager").GetComponent<SceneManagerScript>();
        animIterator = 1;
        savedPosition = transform.localPosition;
        currentImage = 1;
        mr = GetComponent<MeshRenderer>();
        ownScript = this.GetComponent<CutsceneKeislerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginDialogue()
    {
        if (smS.GetSceneName().Contains("1"))
        {
            dialogueS.PrintDialogue("Keisler1");
            dialogueS.NotifyWhenDone(ownScript);
        }
        else if(smS.GetSceneName().Contains("2"))
        {
            dialogueS.PrintDialogue("Keisler2");
            dialogueS.NotifyWhenDone(ownScript);
        }
        else if (smS.GetSceneName().Contains("3"))
        {
            dialogueS.PrintDialogue("Keisler3");
            dialogueS.NotifyWhenDone(ownScript);
        }
    }

    public void DilogueNotified()
    {
        if (!dead && smS.GetScene() != -3)
        {
            if (currentImage == 4)
            {
                animIterator++;
                if (animIterator == 1)
                    StartCoroutine("FourToOne");
                else if (animIterator == 2)
                    StartCoroutine("FourToSix");
                else
                {
                    animIterator = 0;
                    StartCoroutine("FourToEight");
                }
            }
            else if (currentImage == 1)
                StartCoroutine("OneToFour");
            else if (currentImage == 6)
                StartCoroutine("SixToFour");
            else if (currentImage == 8)
                StartCoroutine("EightToFour");
        }
    }

    IEnumerator OneToFour()
    {
        SetToImage(2);
        yield return new WaitForSeconds(ANIMATION_SPEED);
        SetToImage(3);
        yield return new WaitForSeconds(ANIMATION_SPEED);
        SetToImage(4);
        currentImage = 4;
    }

    IEnumerator FourToSix()
    {
        SetToImage(5);
        yield return new WaitForSeconds(ANIMATION_SPEED);
        SetToImage(6);
        currentImage = 6;
    }

    IEnumerator SixToFour()
    {
        SetToImage(5);
        yield return new WaitForSeconds(ANIMATION_SPEED);
        SetToImage(4);
        currentImage = 4;
    }

    IEnumerator FourToOne()
    {
        SetToImage(3);
        yield return new WaitForSeconds(ANIMATION_SPEED);
        SetToImage(2);
        yield return new WaitForSeconds(ANIMATION_SPEED);
        SetToImage(1);
        currentImage = 1;
    }

    IEnumerator FourToEight()
    {
        SetToImage(7);
        yield return new WaitForSeconds(ANIMATION_SPEED);
        SetToImage(8);
        currentImage = 8;
    }

    IEnumerator EightToFour()
    {
        SetToImage(7);
        yield return new WaitForSeconds(ANIMATION_SPEED);
        SetToImage(4);
        currentImage = 4;
    }

    private void SetToImage(int num)
    {
        if (!dead)
        {
            mr.material = allMaterials[num - 1];
            if (num == 1)
            {
                transform.localPosition = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z);
                mr.transform.localScale = new Vector3(2.3f, 2.0f, 1f);
            }
            else if (num == 2)
            {
                transform.localPosition = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z + 0.05f);
                mr.transform.localScale = new Vector3(2.3f, 2.1f, 1f);
            }
            else if (num == 3)
            {
                transform.localPosition = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z + 0.1f);
                mr.transform.localScale = new Vector3(2.3f, 2.2f, 1f);
            }
            else if (num == 4)
            {
                transform.localPosition = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z + 0.1f);
                mr.transform.localScale = new Vector3(2.3f, 2.4f, 1f);
            }
            else if (num == 5)
            {
                transform.localPosition = new Vector3(savedPosition.x + 0.1f, savedPosition.y, savedPosition.z + 0.1f);
                mr.transform.localScale = new Vector3(2.5f, 2.4f, 1f);
            }
            else if (num == 6)
            {
                transform.localPosition = new Vector3(savedPosition.x + 0.2f, savedPosition.y, savedPosition.z + 0.05f);
                mr.transform.localScale = new Vector3(2.7f, 2.3f, 1f);
            }
            else if (num == 7)
            {
                transform.localPosition = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z + 0.1f);
                mr.transform.localScale = new Vector3(2.3f, 2.2f, 1f);
            }
            else if (num == 8)
            {
                transform.localPosition = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z + 0.05f);
                mr.transform.localScale = new Vector3(2.3f, 2.1f, 1f);
            }
        }
    }

    public void Death()
    {
        if (!dead)
        {
            dead = true;
            transform.localPosition = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z - 0.15f);
            mr.transform.localScale = new Vector3(2.3f, 3.0f, 1f);
            mr.material = deathMaterial;
            dialogueS.InterruptDialogue();
            StartCoroutine("DelayEndCutsceneKeisler");
        }
        for (int i = 0; i < 3; i++)
            bloodMS.SpawnBlood(new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f));
    }

    IEnumerator DelayEndCutsceneKeisler()
    {
        yield return new WaitForSeconds(2f);
        dialogueS.EndCutscene();
    }
}

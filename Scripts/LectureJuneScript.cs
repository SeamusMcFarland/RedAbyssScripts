using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LectureJuneScript : MonoBehaviour
{
    StrikeEffectManagerScript semS;

    public Collider hurtColl;
    public Collider proxColl;
    public MeshRenderer mr;
    public DialogueScript dialogueS;
    float xDiff, zDiff;
    float rotation;
    GameObject player;
    PlayerScript playerS;

    GameObject bloodM;
    BloodManagerScript bloodMS;
    GoreManagerScript goreMS;
    bool dead;

    public int numLecture;
    public LectureJuneScript nextljS;
    
    public bool visible;
    bool activationReady;
    bool active;

    public bool finalJune;
    SceneManagerScript smS;

    const float MAX_DISTANCE = 28f;

    bool requestedComeBack;

    public Image madnessEye;

    // Start is called before the first frame update
    void Start()
    {
        semS = GameObject.FindGameObjectWithTag("strikeeffectmanager").GetComponent<StrikeEffectManagerScript>();
        smS = GameObject.FindGameObjectWithTag("scenemanager").GetComponent<SceneManagerScript>();
        active = false;
        activationReady = false;
        dead = false;
        player = GameObject.FindWithTag("player");
        playerS = player.GetComponent<PlayerScript>();
        bloodM = GameObject.FindGameObjectWithTag("bloodmanager");
        bloodMS = bloodM.GetComponent<BloodManagerScript>();
        goreMS = GameObject.FindGameObjectWithTag("goremanager").GetComponent<GoreManagerScript>();
        
        if (!visible)
            mr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        xDiff = player.transform.position.x - transform.position.x;
        zDiff = player.transform.position.z - transform.position.z;
        rotation = -(Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff));
        transform.rotation = Quaternion.Euler(90, rotation, 0);

        if (activationReady && visible)
            Activate();

        if (player.transform.position.z - transform.position.z > MAX_DISTANCE && visible && !dead && !requestedComeBack)
        {
            requestedComeBack = true;
            dialogueS.ClearDialogueQueue();
            dialogueS.PrintDialogue("LectureJuneComeBack" + (int)Random.Range(1f,6f));
        }

    }

    public void Activate()
    {
        if (!active)
        {
            print("activate: " + this.name + Time.time);
            active = true;
            hurtColl.enabled = true;
            dialogueS.ClearDialogueQueue();
            dialogueS.PrintDialogue("LectureJune" + numLecture);
        }
    }

    public void Death()
    {
        if (!dead)
        {
            semS.SpawnStrikeEffect(transform.position, 1);
            dialogueS.InterruptDialogue();
            if (nextljS != null)
                nextljS.Visible();
            dead = true;
            mr.enabled = false;
            hurtColl.enabled = false;
            BloodAndGore();
            
            if (finalJune)
            {
                dialogueS.ClearDialogueQueue();
                StartCoroutine("DelayEndScene");
                madnessEye.enabled = true;
            }
        }
    }

    public void ForwardEnd()
    {
        if (finalJune)
        {
            Death();
        }
    }

    public bool GetFinal()
    {
        return finalJune;
    }

    IEnumerator DelayEndScene()
    {
        playerS.ForwardPlayFlash();
        yield return new WaitForSeconds(0.2f);
        playerS.TeleportToTortured();
        yield return new WaitForSeconds(0.05f);
        madnessEye.enabled = false;
    }

    public void Visible()
    {
        mr.enabled = true;
        visible = true;
    }

    private void BloodAndGore()
    {
        for (int i = 0; i < 10; i++)
            bloodMS.SpawnBlood(transform.position);

        for (int i = 37; i < 45; i++)
            goreMS.SpawnGore(i, transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
            activationReady = true;
    }
}

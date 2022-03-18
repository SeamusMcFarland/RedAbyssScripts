using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelplessJuneScript : MonoBehaviour
{
    StrikeEffectManagerScript semS;

    Collider coll;
    MeshRenderer mr;
    public DialogueScript dialogueS;
    float xDiff, zDiff;
    float rotation;
    GameObject player;
    PlayerScript playerS;

    GameObject bloodM;
    BloodManagerScript bloodMS;
    GoreManagerScript goreMS;
    bool dead;

    // Start is called before the first frame update
    void Start()
    {
        semS = GameObject.FindGameObjectWithTag("strikeeffectmanager").GetComponent<StrikeEffectManagerScript>();
        dead = false;
        mr = transform.GetChild(1).GetComponent<MeshRenderer>();
        coll = transform.GetChild(0).GetComponent<Collider>();
        player = GameObject.FindWithTag("player");
        playerS = player.GetComponent<PlayerScript>();
        bloodM = GameObject.FindGameObjectWithTag("bloodmanager");
        bloodMS = bloodM.GetComponent<BloodManagerScript>();
        goreMS = GameObject.FindGameObjectWithTag("goremanager").GetComponent<GoreManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        xDiff = player.transform.position.x - transform.position.x;
        zDiff = player.transform.position.z - transform.position.z;
        rotation = -(Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff));
        transform.rotation = Quaternion.Euler(90, rotation, 0);
    }

    public void Activate()
    {
        coll.enabled = true;
        StartCoroutine("DelayDialogue");
    }

    IEnumerator DelayDialogue()
    {
        yield return new WaitForSeconds(1.7f);
        dialogueS.PrintDialogue("Helpless");
    }

    public void Death()
    {
        if (!dead)
        {
            semS.SpawnStrikeEffect(transform.position, 1);
            dialogueS.InterruptDialogue();
            dead = true;
            mr.enabled = false;
            coll.enabled = false;
            BloodAndGore();
            playerS.EndLevel();
        }
    }

    private void BloodAndGore()
    {
        for (int i = 0; i < 10; i++)
            bloodMS.SpawnBlood(transform.position);

        for (int i = 37; i < 45; i++)
            goreMS.SpawnGore(i, transform.position);
    }

}

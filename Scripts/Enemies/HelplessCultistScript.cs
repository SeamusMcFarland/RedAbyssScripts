using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelplessCultistScript : MonoBehaviour
{
    StrikeEffectManagerScript semS;

    Collider coll, collAid;
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
    Rigidbody rb;

    public int type;

    public float speed;
    public float health;

    GameObject[] audioM;
    AudioScript audioS;
    const int AUDIO_NUM = 2;
    bool burnNoisy;

    AnimationScript animS;
    bool moving;

    // Start is called before the first frame update
    void Start()
    {
        semS = GameObject.FindGameObjectWithTag("strikeeffectmanager").GetComponent<StrikeEffectManagerScript>();
        moving = false;
        animS = GetComponentInChildren<AnimationScript>();
        if(speed > 0)
            rb = GetComponent<Rigidbody>();
        dead = false;
        mr = transform.GetChild(1).GetComponent<MeshRenderer>();
        coll = transform.GetChild(0).GetComponent<Collider>();
        collAid = transform.GetChild(2).GetComponent<Collider>();
        player = GameObject.FindWithTag("player");
        playerS = player.GetComponent<PlayerScript>();
        bloodM = GameObject.FindGameObjectWithTag("bloodmanager");
        bloodMS = bloodM.GetComponent<BloodManagerScript>();
        goreMS = GameObject.FindGameObjectWithTag("goremanager").GetComponent<GoreManagerScript>();
        /*xDiff = player.transform.position.x - transform.position.x;
        zDiff = player.transform.position.z - transform.position.z;
        rotation = Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff) + 180f;*/
        rotation = transform.rotation.z * 180f;
        transform.rotation = Quaternion.Euler(90f, 0, rotation);
        animS.SetState(1);

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


    // Update is called once per frame
    void Update()
    {
        if (speed > 0)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < 10f)
            {
                rb.velocity = new Vector3(Mathf.Cos(rotation * Mathf.Deg2Rad), 0f, Mathf.Sin(rotation * Mathf.Deg2Rad)) * speed;
                if (!moving)
                {
                    moving = true;
                    animS.SetState(3);
                }
            }
            else
            {
                rb.velocity = new Vector3(0,0,0);
                if (moving)
                {
                    moving = false;
                    animS.SetState(1);
                }
            }
        }
        transform.rotation = Quaternion.Euler(90f, 0, rotation);
    }

    public void Hit(float dam)
    {
        if (!dead)
        {
            semS.SpawnStrikeEffect(transform.position, 1);
            health -= dam;
            bloodMS.SpawnBlood(transform.position);
            bloodMS.SpawnBlood(transform.position);

            if (health <= 0f)
                Death();
        }
    }

    public void Hit(float dam, float hitstunMod)
    {
        if (!dead)
        {
            semS.SpawnStrikeEffect(transform.position, 1);
            health -= dam;
            bloodMS.SpawnBlood(transform.position);

            if (health <= 0f)
                Death();
        }
    }

    public void BurnHit(float dam)
    {
        if (!dead)
        {
            if (!burnNoisy)
            {
                burnNoisy = true;
                StartCoroutine("EndBurnNoisy");
                if (dam == 0.03f)
                    audioS.PlayFizzClip();
                else
                    audioS.PlayBurnClip();
            }
            health -= dam;

            if (health <= 0f)
                Death();
        }

    }

    IEnumerator EndBurnNoisy()
    {
        yield return new WaitForSeconds(1.05f);
        burnNoisy = false;
    }


    public void Death()
    {
        if (!dead)
        {
            audioS.PlayBloodExplosion();
            dead = true;
            mr.enabled = false;
            coll.enabled = false;
            collAid.enabled = false;
            BloodAndGore();
        }
    }

    private void BloodAndGore()
    {
        for (int i = 0; i < 10; i++)
            bloodMS.SpawnBlood(transform.position);

        switch(type)
        {
            case 1:
                for (int i = 101; i < 103; i++)
                    goreMS.SpawnGore(i, transform.position);
                break;

            case 2:
                for (int i = 103; i < 105; i++)
                    goreMS.SpawnGore(i, transform.position);
                break;

            case 3:
                for (int i = 105; i < 107; i++)
                    goreMS.SpawnGore(i, transform.position);
                break;
            case 4:
                for (int i = 109; i < 114; i++)
                    goreMS.SpawnGore(i, transform.position);
                break;
        }
        if (type < 4)
        {
            for (int i = 107; i < 109; i++)
                goreMS.SpawnGore(i, transform.position);
        }
    }
}

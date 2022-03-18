using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CronenburgCorpseScript : MonoBehaviour
{
    float health;
    public Material[] allMaterials;
    MeshRenderer mr;
    Vector3 savedPosition;
    bool ending;
    bool spawned;
    PlayerScript playerS;
    int breakPoint;

    GoreManagerScript goreMS;
    BloodManagerScript bloodMS;

    const int AUDIO_NUM = 7;
    AudioScript audioS;
    const int AUDIO_NUM2 = 2;
    AudioScript audioS2;

    public MeshRenderer redFlash;
    float redness;

    // Start is called before the first frame update
    void Start()
    {
        redness = 0;
        redFlash.material.color = new Color(255f, 0, 0, 0);
        redFlash.material.SetColor("_EmissionColor", new Color(0, 0, 0));

        goreMS = GameObject.FindGameObjectWithTag("goremanager").GetComponent<GoreManagerScript>();
        bloodMS = GameObject.FindGameObjectWithTag("bloodmanager").GetComponent<BloodManagerScript>();

        breakPoint = 0;
        spawned = false;
        playerS = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerScript>();
        mr = GetComponent<MeshRenderer>();
        health = 100f;
        transform.position = new Vector3(300f,300f,300f);
        ending = false;

        SetupAudio();
    }

    private void SetupAudio()
    {
        GameObject[] audioM = GameObject.FindGameObjectsWithTag("audiomanager");
        int searching = 0;
        while (searching != -1)
        {
            if (audioM[searching].GetComponent<AudioScript>().GetAudioNum() == AUDIO_NUM)
            {
                audioS = audioM[searching].GetComponent<AudioScript>();
                searching = -1;
            }
            else
                searching++;
        }
        searching = 0;
        while (searching != -1)
        {
            if (audioM[searching].GetComponent<AudioScript>().GetAudioNum() == AUDIO_NUM2)
            {
                audioS2 = audioM[searching].GetComponent<AudioScript>();
                searching = -1;
            }
            else
                searching++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        redFlash.material.SetColor("_EmissionColor", new Color(redness, 0, 0));
        if (redness > 0)
            redness -= 0.002f;
        if (redness < 0)
            redness = 0;
    }

    public void Spawn(Vector3 pos, float rotation)
    {
        transform.position = new Vector3(pos.x, 0.02f, pos.z);
        savedPosition = transform.position;
        transform.rotation = Quaternion.Euler(90f, -rotation, 0);
        spawned = true;
    }

    public void Hit(float damage)
    {
        bloodMS.SpawnBlood(transform.position);
        health -= damage;
        SetMaterial();
        audioS.IncreaseVolume();
        redness += 0.05f;
        if(redness > 1)
            redness = 1;
    }

    private void SetMaterial()
    {
        if (health > 90f)
        {
            mr.material = allMaterials[0];
            transform.position = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z);
            transform.localScale = new Vector3(2.9f, 3.3f, 1f);
        }
        else if (health > 80f)
        {
            if (breakPoint < 1)
            {
                bloodMS.SpawnBlood(transform.position);
                goreMS.SpawnGore(76, transform.position);
                audioS2.PlayBloodExplosion();
                breakPoint++;
            }
            mr.material = allMaterials[1];
            transform.position = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z);
            transform.localScale = new Vector3(2.9f, 3.3f, 1f);
        }
        else if (health > 70f)
        {
            if (breakPoint < 2)
            {
                bloodMS.SpawnBlood(transform.position);
                goreMS.SpawnGore(77, transform.position);
                audioS2.PlayBloodExplosion();
                breakPoint++;
            }
            mr.material = allMaterials[2];
            transform.position = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z - 0.05f);
            transform.localScale = new Vector3(2.9f, 3.2f, 1f);
        }
        else if (health > 60f)
        {
            if (breakPoint < 3)
            {
                bloodMS.SpawnBlood(transform.position);
                goreMS.SpawnGore(78, transform.position);
                audioS2.PlayBloodExplosion();
                breakPoint++;
            }
            mr.material = allMaterials[3];
            transform.position = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z - 0.05f);
            transform.localScale = new Vector3(2.9f, 3.2f, 1f);
        }
        else if (health > 50f)
        {
            if (breakPoint < 4)
            {
                bloodMS.SpawnBlood(transform.position);
                audioS2.PlayBloodExplosion();
                breakPoint++;
            }
            mr.material = allMaterials[4];
            transform.position = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z - 0.1f);
            transform.localScale = new Vector3(2.9f, 3.1f, 1f);
        }
        else if (health > 40f)
        {
            if (breakPoint < 5)
            {
                bloodMS.SpawnBlood(transform.position);
                audioS2.PlayBloodExplosion();
                breakPoint++;
            }
            mr.material = allMaterials[5];
            transform.position = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z + 0.1f);
            transform.localScale = new Vector3(2.9f, 2.7f, 1f);
        }
        else if (health > 30f)
        {
            if (breakPoint < 6)
            {
                bloodMS.SpawnBlood(transform.position);
                audioS2.PlayBloodExplosion();
                breakPoint++;
            }
            if (!ending)
                StartCoroutine("EndIt");
            mr.material = allMaterials[6];
            transform.position = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z + 0.05f);
            transform.localScale = new Vector3(2.9f, 2.6f, 1f);
        }
        else if (health > 20f)
        {
            if (breakPoint < 7)
            {
                bloodMS.SpawnBlood(transform.position);
                audioS2.PlayBloodExplosion();
                breakPoint++;
            }
            mr.material = allMaterials[7];
            transform.position = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z + 0.15f);
            transform.localScale = new Vector3(2.9f,2.4f,1f);
        }
    }

    IEnumerator EndIt()
    {
        yield return new WaitForSeconds(2f);
        if (!ending)
        {
            ending = true;
            playerS.EndLevel();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooScript : MonoBehaviour
{
    const float PROJECTILE_SPEED = 20f;
    GameObject player;
    PlayerScript playerS;

    GameObject[] audioM;
    AudioScript audioS;
    const int AUDIO_NUM = 3;

    float xDiff;
    float zDiff;
    float rotation;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("player");
        playerS = player.GetComponent<PlayerScript>();
        audioM = GameObject.FindGameObjectsWithTag("audiomanager");
        SetupAudio();
        transform.position = new Vector3(200f, 0f, 200f);
    }

    private void SetupAudio()
    {
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
    }

    public void Shoot(Vector3 pos)
    {
        transform.position = pos;
        PointTowards();
        rb.velocity = new Vector3(Mathf.Cos(rotation * Mathf.Deg2Rad), 0f, Mathf.Sin(rotation * Mathf.Deg2Rad)) * PROJECTILE_SPEED;
    }

    void PointTowards()
    {
        xDiff = player.transform.position.x - transform.position.x;
        zDiff = player.transform.position.z - transform.position.z;
        rotation = Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff);
        transform.rotation = Quaternion.Euler(90,0, rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            playerS.Hit(1);
            audioS.PlayAcid();
            transform.position = new Vector3(400f, 100f, 400f);
        }
        else if (other.gameObject.tag == "wall")
        {
            audioS.PlayAcidMiss();
            transform.position = new Vector3(400f, 100f, 400f);
        }
    }


}

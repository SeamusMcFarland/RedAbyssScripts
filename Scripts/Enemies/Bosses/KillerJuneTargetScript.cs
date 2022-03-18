using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerJuneTargetScript : MonoBehaviour
{
    GameObject player;
    PlayerScript playerS;
    MeshRenderer mr;
    AnimationScript animationS;
    Rigidbody rb;
    bool hitboxActive; // when collider is triggered
    bool struckPlayer;
    const float DAMAGE = 1.5f;

    GameObject[] audioM;
    AudioScript audioS;
    const int AUDIO_NUM = 2; // monster audio manager

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("player");
        playerS = player.GetComponent<PlayerScript>();
        animationS = GetComponent<AnimationScript>();
        mr = GetComponent<MeshRenderer>();
        mr.enabled = false;

        animationS.SetState(1);

        SetupAudio();
    }

    // Update is called once per frame
    void Update()
    {

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

    public void TargetPlayer()
    {
        struckPlayer = false;
        rb.MovePosition(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        mr.enabled = true;
        animationS.SetState(1);
        StartCoroutine("Flicker", 15);
    }

    IEnumerator Flicker(int remaining)
    {
        yield return new WaitForSeconds(0.03f);
        mr.enabled = false;
        yield return new WaitForSeconds(0.03f);
        mr.enabled = true;
        if (remaining <= 1)
            StartCoroutine("Explode");
        else
            StartCoroutine("Flicker", remaining - 1);
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.11f);
        if (hitboxActive)
            playerS.Hit(DAMAGE);
        audioS.PlayFireExplosion();
        animationS.SetState(3);
        yield return new WaitForSeconds(0.38f);
        mr.enabled = false;
        rb.MovePosition(new Vector3(200f, transform.position.y, 200f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("player"))
            hitboxActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player"))
            hitboxActive = false;
    }
}

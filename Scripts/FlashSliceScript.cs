using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashSliceScript : MonoBehaviour
{
    //public MeshRenderer cameraFlashMR;
    Rigidbody rb;
    bool shrinking;
    bool rotating;
    bool rotatingDirection;
    float rotation;
    ParticleSystem ps;
    const float SMALL_SIZE = 1f;
    const float BIG_SIZE = 2f;
    const float STRIKE_DELAY = 0.5f;
    const float DAMAGE = 6.9f;
    const float SELF_DAMAGE = 3f;
    const float SHRINK_SPEED = 0.05f;
    const float ROTATION_SPEED = 1f;
    //const float FLASH_FADE_RATE = 5f;

    Light flashL;

    float whiteness;

    PlayerScript playerS;
    bool playerInside; // if player in hitbox
    List<GameObject> junesInside = new List<GameObject>();

    float frameNormalizer;

    GameObject[] audioM;
    AudioScript audioS;
    const int AUDIO_NUM = 2; // monster audio manager

    TorturedScript torturedS;
    bool torturedInside;

    // Start is called before the first frame update
    void Start()
    {
        playerS = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerScript>();
        flashL = GameObject.FindGameObjectWithTag("flashsliceflash").GetComponent<Light>();
        flashL.intensity = 0f;
        torturedS = GameObject.FindGameObjectWithTag("tortured").GetComponent<TorturedScript>();
        ps = GetComponent<ParticleSystem>();
        ps.Stop();
        rb = GetComponent<Rigidbody>();
        rb.MovePosition(new Vector3(400f, 0.0299f, 400f));

        /*cameraFlashMR.material.color = new Color(1f, 1f, 1f, 0);
        cameraFlashMR.material.SetColor("_EmissionColor", new Color(0, 0, 0));
        cameraFlashMR.enabled = false;*/

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
        frameNormalizer = Time.deltaTime / 0.014f;

        if (shrinking)
        {
            var main1 = ps.main;
            main1.startSizeMultiplier -= SHRINK_SPEED * frameNormalizer;
        }

        if (rotating)
        {
            if (rotatingDirection)
                rotation += ROTATION_SPEED * frameNormalizer;
            else
                rotation -= ROTATION_SPEED * frameNormalizer;
            rb.MoveRotation(Quaternion.Euler(0, rotation, 0));
        }

        /*whiteness -= Time.deltaTime * FLASH_FADE_RATE;
        print("whiteness: " + whiteness + " at: " + Time.time);
        cameraFlashMR.material.SetColor("_EmissionColor", new Color(whiteness, whiteness, whiteness));*/
    }
    
    public void FlashSlice(Vector3 pos)
    {
        StartCoroutine("DelayFlashSlice", pos);
    }

    IEnumerator DelayFlashSlice(Vector3 pos)
    {
        //Initial appearence
        ps.Play();
        audioS.PlayKnifeSlice();
        rotation = Random.Range(-180f, 180f);
        rb.MoveRotation(Quaternion.Euler(0, rotation, 0));
        var main2 = ps.main;
        main2.startSizeMultiplier = SMALL_SIZE;
        rb.MovePosition(new Vector3(pos.x, transform.position.y, pos.z));
        shrinking = true;
        //rotating = true;
        if (Random.value < 0.5f)
            rotatingDirection = true;
        else
            rotatingDirection = false;
        yield return new WaitForSeconds(STRIKE_DELAY);

        //strike
        rotating = false;
        whiteness = 150f;
        //cameraFlashMR.enabled = true;
        flashL.intensity = 10f;
        audioS.PlayLaserBurst();
        main2.startSizeMultiplier = BIG_SIZE;
        shrinking = false;

        //damage
        if (playerInside)
        {
            print("player hit! at: " + Time.time);
            playerS.Hit(DAMAGE);
            playerS.KnockOver(new Vector3(0, 0.01f, 0));
        }
        foreach (GameObject o in junesInside)
            o.GetComponent<KillerJuneScript>().Hit(100f);
        if (torturedInside)
            torturedS.Hit(SELF_DAMAGE);
        yield return new WaitForSeconds(0.1f);

        //End
        ps.Stop();
        rb.MovePosition(new Vector3(400f, transform.position.y, 400f));
        rotation = 0;
        rb.MoveRotation(Quaternion.Euler(0, rotation, 0));
        main2.startSizeMultiplier = SMALL_SIZE;

        

    }

    private void OnTriggerEnter(Collider other)
    {
        print("trigger entering: " + Time.time + " with name: " + other.name);
        if (other.CompareTag("player"))
            playerInside = true;
        else if (other.CompareTag("killerjune"))
            junesInside.Add(other.gameObject);
        else if (other.CompareTag("torturedhurtbox"))
            torturedInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        print("trigger exiting: " + Time.time + " with name: " + other.name);
        if (other.CompareTag("player"))
            playerInside = false;
        else if (other.CompareTag("killerjune"))
            junesInside.Remove(other.gameObject);
        else if (other.CompareTag("torturedhurtbox"))
            torturedInside = false;
    }
}

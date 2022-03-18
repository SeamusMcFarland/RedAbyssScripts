using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapScript : MonoBehaviour
{
    Rigidbody rb;
    Rigidbody ohrb;
    public GameObject otherHalf;
    bool triggered;
    const float TRAP_DAMAGE = 6f;
    const float HITSTUN_MOD = 10f;

    GameObject[] audioM;
    AudioScript audioS;
    const int AUDIO_NUM = 3;
    bool active;

    // Start is called before the first frame update
    void Start()
    {
        triggered = false;
        rb = GetComponent<Rigidbody>();
        ohrb = otherHalf.GetComponent<Rigidbody>();

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
        
    }

    public void PlaceTrap(Vector3 pos, float rotation)
    {
        transform.rotation = Quaternion.Euler(90, rotation, 0); // all four lines for setting velocity
        otherHalf.transform.rotation = Quaternion.Euler(90, rotation, 0); //
        rb.velocity = (transform.right * 1f) + new Vector3(0f, 0f, 0f); //
        ohrb.velocity = (transform.right * 1f) + new Vector3(0f, 0f, 0f); //

        gameObject.layer = 10; // prevents from glitching player after being spawned in inside of the player's collision box
        otherHalf.layer = 10; //
        transform.position = new Vector3(pos.x, pos.y + 2f, pos.z + 0.175f) + (transform.right * 1f);
        otherHalf.transform.position = new Vector3(pos.x, pos.y + 2f, pos.z - 0.175f) + (transform.right * 1f);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        otherHalf.transform.rotation = Quaternion.Euler(0, 180, 0);
        active = false;
        StartCoroutine("EnableTrap");
    }

    IEnumerator EnableTrap() // prevents trap from being shot
    {
        yield return new WaitForSeconds(0.01f);
        gameObject.layer = 2;
        otherHalf.layer = 2;
        triggered = false;
        yield return new WaitForSeconds(0.6f);
        active = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (active && (!triggered && (other.CompareTag("player") || other.CompareTag("runnerhurtbox")
            || other.CompareTag("muscleshurtbox") || other.CompareTag("spitterhurtbox") || other.CompareTag("striderhurtbox")
            || other.CompareTag("eyehurtbox") || other.CompareTag("zombiehurtbox") || other.CompareTag("jacobhurtbox")
            || other.CompareTag("cronenburghurtbox") || other.CompareTag("posessedhurtbox") || other.CompareTag("cultisthurtbox")
            || other.CompareTag("suicidebomberhurtbox") || other.CompareTag("lemmerhurtbox")
            || other.CompareTag("lecturejunehurtbox") || other.CompareTag("killerjunehurtbox") || other.CompareTag("torturedhurtbox"))))
        {
            triggered = true;
            TriggerTrap(other);
        }
    }

    public void TriggerTrap(Collider t)
    {
        if (t.CompareTag("player"))
            t.GetComponent<PlayerScript>().Hit(TRAP_DAMAGE);
        else if (t.CompareTag("runnerhurtbox"))
            t.transform.parent.gameObject.GetComponent<RunnerScript>().Hit(TRAP_DAMAGE, HITSTUN_MOD);
        else if (t.CompareTag("muscleshurtbox"))
            t.transform.parent.gameObject.GetComponent<MusclesScript>().Hit(TRAP_DAMAGE, HITSTUN_MOD);
        else if (t.CompareTag("spitterhurtbox"))
            t.transform.parent.gameObject.GetComponent<SpitterScript>().Hit(TRAP_DAMAGE, HITSTUN_MOD);
        else if (t.CompareTag("striderhurtbox"))
            t.transform.parent.gameObject.GetComponent<StriderScript>().Hit(TRAP_DAMAGE, HITSTUN_MOD);
        else if (t.CompareTag("eyehurtbox"))
            t.transform.parent.gameObject.GetComponent<EyeScript>().Hit(TRAP_DAMAGE, HITSTUN_MOD);
        else if (t.CompareTag("zombiehurtbox"))
            t.transform.parent.gameObject.GetComponent<ZombieScript>().Hit(TRAP_DAMAGE, HITSTUN_MOD);
        else if (t.CompareTag("smileyhurtbox"))
            t.transform.parent.gameObject.GetComponent<SmileyScript>().Hit(TRAP_DAMAGE, HITSTUN_MOD);
        else if (t.CompareTag("suicidebomberhurtbox"))
            t.transform.parent.gameObject.GetComponent<SuicideBomberScript>().Hit(TRAP_DAMAGE, HITSTUN_MOD);
        else if (t.CompareTag("cultisthurtbox"))
            t.transform.parent.gameObject.GetComponent<CultistScript>().Hit(TRAP_DAMAGE, HITSTUN_MOD);
        else if (t.CompareTag("posessedhurtbox"))
            t.transform.parent.gameObject.GetComponent<PosessedScript>().Hit(TRAP_DAMAGE, HITSTUN_MOD);
        else if (t.CompareTag("jacobhurtbox"))
            t.transform.parent.gameObject.GetComponent<JacobScript>().Hit(TRAP_DAMAGE);
        else if (t.CompareTag("helplesscultisthurtbox"))
            t.transform.parent.gameObject.GetComponent<HelplessCultistScript>().Hit(TRAP_DAMAGE, HITSTUN_MOD);
        else if (t.CompareTag("lecturejunehurtbox"))
            t.transform.parent.gameObject.GetComponent<LectureJuneScript>().Death();
        else if (t.CompareTag("killerjunehurtbox"))
            t.transform.parent.gameObject.GetComponent<KillerJuneScript>().Hit(TRAP_DAMAGE, HITSTUN_MOD);
        else if (t.CompareTag("torturedhurtbox"))
            t.transform.parent.gameObject.GetComponent<TorturedScript>().Hit(TRAP_DAMAGE, HITSTUN_MOD);
        else if (t.CompareTag("cronenburghurtbox"))
        {
            if(t.transform.parent.GetComponent<CronenburgBossScript>() != null)
                t.transform.parent.gameObject.GetComponent<CronenburgBossScript>().Hit(TRAP_DAMAGE);
            else
                t.transform.parent.gameObject.GetComponent<CronenburgChaseScript>().Hit(TRAP_DAMAGE);
        }
        else if (t.CompareTag("lemmerhurtbox"))
            t.transform.parent.gameObject.GetComponent<LemmerScript>().Hit(TRAP_DAMAGE);
        audioS.PlayTrapClose();
        gameObject.layer = 10;
        otherHalf.layer = 10;
        //rb.velocity = new Vector3(0,20f,0);
        rb.angularVelocity = new Vector3(1000f,0,0);
        StartCoroutine("RenewCollision");
        StartCoroutine("RemoveTrap");
    }

    IEnumerator RenewCollision()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.layer = 2;
        otherHalf.layer = 2;
    }

    IEnumerator RemoveTrap() // removes trap after it is triggered
    {
        yield return new WaitForSeconds(2f);
        transform.position = new Vector3(500f, 500f + 2f, 500f + 0.175f) + (transform.right * 1f);
        otherHalf.transform.position = new Vector3(500f, 500f + 2f, 500f - 0.175f) + (transform.right * 1f);
    }

}

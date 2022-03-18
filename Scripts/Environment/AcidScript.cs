using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidScript : MonoBehaviour
{

    bool quenched;
    Vector3 newVector;
    Rigidbody rb;
    MeshRenderer mr;
    const float BURN_DAMAGE = 0.03f;

    bool sameObject;
    List<GameObject> targets = new List<GameObject>();
    GameObject savedBarrel;

    bool noisyMonster;
    GameObject[] audioM;
    AudioScript audioS;
    const int AUDIO_NUM = 3;

    public bool temporaryAcid;
    const float LIFESPAN = 15f;

    PlayerScript playerS;

    float frameNormalizer;

    // Start is called before the first frame update
    void Start()
    {
        playerS = GameObject.FindWithTag("player").GetComponent<PlayerScript>();
        mr = GetComponent<MeshRenderer>();
        noisyMonster = false;
        rb = GetComponent<Rigidbody>();
        quenched = true;

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
        if (quenched == false && !playerS.GetPaused())
        {
            BurnTargets();
        }
    }

    void BurnTargets()
    {
        foreach (GameObject t in targets)
        {
            if (t.CompareTag("player"))
                t.GetComponent<PlayerScript>().BurnHit(BURN_DAMAGE, 2);
            else if (t.CompareTag("runnerhurtbox"))
                t.transform.parent.gameObject.GetComponent<RunnerScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("muscleshurtbox"))
                t.transform.parent.gameObject.GetComponent<MusclesScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("striderhurtbox"))
                t.transform.parent.gameObject.GetComponent<StriderScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("eyehurtbox"))
                t.transform.parent.gameObject.GetComponent<EyeScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("zombiehurtbox"))
                t.transform.parent.gameObject.GetComponent<ZombieScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("smileyhurtbox"))
                t.transform.parent.gameObject.GetComponent<SmileyScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("suicidebomberhurtbox"))
                t.transform.parent.gameObject.GetComponent<SuicideBomberScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("cultisthurtbox"))
                t.transform.parent.gameObject.GetComponent<CultistScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("posessedhurtbox"))
                t.transform.parent.gameObject.GetComponent<PosessedScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("lemmerhurtbox"))
                t.transform.parent.gameObject.GetComponent<LemmerScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("plantish"))
            {
                if (Random.value < 0.005f)
                {
                    if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                        audioS.PlayBulletStrike();
                    noisyMonster = true;
                    audioS.PlaySnap();
                    StartCoroutine("EndNoisy");

                    t.GetComponent<PlantishScript>().DestroyPlantish();
                }
            }
        }
    }

    IEnumerator EndNoisy()
    {
        yield return new WaitForSeconds(0.05f);
        noisyMonster = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        sameObject = false; //
        foreach (GameObject o in targets) //
            if (other.gameObject == o) //
                sameObject = true; //these lines assure not adding same target
        if (!sameObject)
        {
            if (other.gameObject.CompareTag("player"))
                targets.Add(other.gameObject);
            else if (other.gameObject.CompareTag("runnerhurtbox") || other.gameObject.CompareTag("muscleshurtbox") || other.gameObject.CompareTag("jacobhurtbox") || other.gameObject.CompareTag("striderhurtbox") || other.gameObject.CompareTag("eyehurtbox") || other.gameObject.CompareTag("zombiehurtbox") || other.gameObject.CompareTag("smileyhurtbox") || other.gameObject.CompareTag("suicidebomberhurtbox") || other.gameObject.CompareTag("cultisthurtbox") || other.gameObject.CompareTag("posessedhurtbox") || other.gameObject.CompareTag("lemmerhurtbox") || other.gameObject.CompareTag("plantish"))
                targets.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (other.gameObject == targets[i]) // checks if any of the burning targets match whatever left the trigger
            {
                targets.RemoveAt(i);
                break;
            }
        }
    }

    public void Spill(Vector3 pos)
    {
        rb.velocity = new Vector3(0, 0, 0);
        transform.position = pos;
        quenched = false;
        if (temporaryAcid)
            StartCoroutine("Disappear");
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(LIFESPAN);
        for (int i = 0; i < 7; i++)
        {
            mr.enabled = false;
            yield return new WaitForSeconds(0.2f);
            mr.enabled = true;
            yield return new WaitForSeconds(0.2f);
        }
        transform.position = new Vector3(400f, 400f, 400f);
    }

}

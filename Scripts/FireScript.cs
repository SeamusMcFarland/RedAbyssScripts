using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    bool quenched;
    public GameObject burnO;
    const float BASE_DECREASE = 0.01f;
    float setDecrease;
    Vector3 newVector;
    Rigidbody rb;
    ParticleSystem ps;
    const float BURN_DAMAGE = 0.025f;

    bool sameObject;
    List<GameObject> targets = new List<GameObject>();
    GameObject savedBarrel;

    GameObject[] audioM;
    AudioScript audioS;
    const int AUDIO_NUM = 2; //monster audio manager
    bool noisyMonster;

    PlayerScript playerS;

    float hotter; // makes more likely to break over time

    float frameNormalizer;

    // Start is called before the first frame update
    void Start()
    {
        playerS = GameObject.FindWithTag("player").GetComponent<PlayerScript>();
        noisyMonster = false;
        rb = GetComponent<Rigidbody>();
        ps = GetComponent<ParticleSystem>();
        quenched = true;
        setDecrease = 10f; //for error detection
        SetupAudio();
    }

    private void SetupAudio() // as of 1/15/20 exclusively for triggering explosive sound effect since explosive barrel script doesn't have access to an audiomanager
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
            Burning();
            BurnTargets();
        }
    }

    void BurnTargets()
    {
        foreach (GameObject t in targets) // this is very inefficient due to aquiring components each frame and can eventually be fixed by checking to see if the effected target has changed, storing the target script for future use
        {
            if (t.CompareTag("player"))
                t.GetComponent<PlayerScript>().BurnHit(BURN_DAMAGE * frameNormalizer, 1);
            else if (t.CompareTag("runnerhurtbox"))
                t.transform.parent.gameObject.GetComponent<RunnerScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("muscleshurtbox"))
                t.transform.parent.gameObject.GetComponent<MusclesScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("spitterhurtbox"))
                t.transform.parent.gameObject.GetComponent<SpitterScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
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
            else if (t.CompareTag("jacobhurtbox"))
                t.transform.parent.gameObject.GetComponent<JacobScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("lemmerhurtbox"))
                t.transform.parent.gameObject.GetComponent<LemmerScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("torturedhurtbox"))
                t.transform.parent.gameObject.GetComponent<TorturedScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("killerjunehurtbox"))
                t.transform.parent.gameObject.GetComponent<KillerJuneScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.transform.CompareTag("helplesscultisthurtbox"))
                    t.transform.parent.gameObject.GetComponent<HelplessCultistScript>().BurnHit(BURN_DAMAGE * frameNormalizer);
            else if (t.CompareTag("plantish"))
            {
                if (Random.value < 0.05f)
                {
                    if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                        audioS.PlayBulletStrike();
                    noisyMonster = true;
                    audioS.PlaySnap();
                    StartCoroutine("EndNoisy");

                    t.GetComponent<PlantishScript>().DestroyPlantish();
                }
            }
            else if (t.CompareTag("box"))
            {
                if (Random.value < 0.0001f + hotter)
                {
                    audioS.PlayBoxBreak();
                    t.gameObject.GetComponent<BoxScript>().BreakBox();
                }
                else
                    hotter += 0.000001f;
            }
            else if (t.CompareTag("breakablewall"))
            {
                if (Random.value < 0.0001f + hotter)
                {
                    hotter = 0;
                    BreakableWallScript tempBreakableWallScript = t.transform.GetComponent<BreakableWallScript>();
                    if (!(tempBreakableWallScript.GetDestroyed()))
                    {
                        tempBreakableWallScript.BreakWall();
                        if (tempBreakableWallScript.GetDestroyed())
                        {
                            audioS.PlayDestroyWall();
                        }
                        audioS.PlayDamageWall();
                    }
                }
                else
                    hotter += 0.000001f;
            }
        }
    }

    IEnumerator EndNoisy()
    {
        yield return new WaitForSeconds(0.01f);
        noisyMonster = false;
    }

    private void Burning()
    {
        if (transform.localScale.x > 4f)
            setDecrease = BASE_DECREASE * frameNormalizer;
        else
            setDecrease = Mathf.Pow(BASE_DECREASE * frameNormalizer, (transform.localScale.x / 4f)) + 0.01f; // once decreased to < 1f decreases exponentially

        newVector = new Vector3(transform.localScale.x - setDecrease, transform.localScale.y - setDecrease, transform.localScale.z - setDecrease);
        transform.localScale = newVector;
        burnO.transform.localScale = newVector;
        if (transform.localScale.x < 0.03f)
        {
            quenched = true;
            transform.position = new Vector3(100f, 100f, 100f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("explosivebarrel"))
        {
            savedBarrel = other.gameObject;
            StartCoroutine("ExplodeBarrel", Random.value*2f); // prevents multiple triggerings at once AND creates a more realistic (possibly delayed) triggering
        }
        else
        {
            sameObject = false; //
            foreach (GameObject o in targets) //
                if (other.gameObject == o) //
                    sameObject = true; //these lines assure not adding same target
            if (!sameObject)
            {
                if (other.gameObject.CompareTag("player")) // split player and monster for readability
                    targets.Add(other.gameObject);
                else if (other.gameObject.CompareTag("runnerhurtbox") || other.gameObject.CompareTag("muscleshurtbox")
                            ||other.gameObject.CompareTag("spitterhurtbox") || other.gameObject.CompareTag("jacobhurtbox")
                            || other.gameObject.CompareTag("striderhurtbox") || other.gameObject.CompareTag("eyehurtbox")
                            || other.gameObject.CompareTag("zombiehurtbox") || other.gameObject.CompareTag("smileyhurtbox")
                            || other.gameObject.CompareTag("suicidebomberhurtbox") || other.gameObject.CompareTag("cultisthurtbox")
                            || other.gameObject.CompareTag("posessedhurtbox") || other.gameObject.CompareTag("lemmerhurtbox")
                            || other.gameObject.CompareTag("torturedhurtbox") || other.gameObject.CompareTag("killerjunehurtbox")
                            || other.gameObject.CompareTag("helplesscultisthurtbox") || other.gameObject.CompareTag("plantish")
                            || other.gameObject.CompareTag("box") || other.gameObject.CompareTag("breakablewall"))
                    targets.Add(other.gameObject);
            }
        }
    }

    IEnumerator ExplodeBarrel(float time)
    {
        yield return new WaitForSeconds(time);
        if (savedBarrel.GetComponent<ExplosiveBarrelScript>().GetExploding() == false)
        {
            savedBarrel.GetComponent<ExplosiveBarrelScript>().Explode();
            audioS.PlayExplosion();
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

    public void Enflame(Vector3 pos)
    {
        rb.velocity = new Vector3(0,0,0);
        transform.position = pos;
        transform.localScale = new Vector3(8f,8f,8f);
        burnO.transform.localScale = new Vector3(8f,8f,8f);
        quenched = false;
    }

}

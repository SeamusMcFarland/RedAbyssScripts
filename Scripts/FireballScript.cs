using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballScript : MonoBehaviour
{

    public GameObject dropPoint;
    Rigidbody rb;
    bool active;
    bool exploding;
    const float BASE_DAMAGE = 6.9f;
    float fireballDamage;
    float fireballDistance;
    bool boxBroken;

    GameObject player;
    PlayerScript playerS;
    List<GameObject> targets = new List<GameObject>();

    RaycastHit hit;
    bool validHit;
    GameObject hitObject;

    public Light light;
    public ParticleSystem ps;
    public ExplosionEffectScript eeS;

    GameObject[] audioM;
    AudioScript audioS;
    const int AUDIO_NUM = 2; // monster audio manager

    const int RAYCAST_DIVISIONS = 20;

    // Start is called before the first frame update
    void Start()
    {
        active = false;
        rb = GetComponent<Rigidbody>();
        exploding = false;
        boxBroken = false;
        player = GameObject.FindGameObjectWithTag("player");
        playerS = player.GetComponent<PlayerScript>();
        light.enabled = false;
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

    public void DropFireball()
    {
        ps.Play();
        transform.position = new Vector3(dropPoint.transform.position.x + Random.Range(-8f, 8f), dropPoint.transform.position.y, dropPoint.transform.position.z +Random.Range(-8f, 8f));
        rb.velocity = new Vector3(0,0,0);
        rb.useGravity = true;
        active = true;
        exploding = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (active && other.gameObject.CompareTag("floor"))
        {
            Explode();
            eeS.CreateEffect(transform.position);
        }
    }

    private void Explode()
    {
        if (exploding == false) // second backup to prevent multiple triggerings at once
        {
            exploding = true;
            light.enabled = true;
            CheckTargets();

            foreach (GameObject t in targets)
            {
                fireballDistance = Mathf.Pow(Mathf.Pow(t.transform.position.x - transform.position.x, 2) + Mathf.Pow(t.transform.position.y - transform.position.y, 2) + Mathf.Pow(t.transform.position.z - transform.position.z, 2), 0.5f);
                if (fireballDistance < 5f) // hardcoded 85f range of effect
                    fireballDamage = BASE_DAMAGE; // originally fireballDamage = BASE_DAMAGE - fireballDistance; but wanted to remove drop-off effect and reduce range of effect
                else
                    fireballDamage = 0f;
                if (fireballDamage > 0)
                {
                    if (t.CompareTag("runnerhurtbox"))
                    {
                        t.transform.parent.GetComponent<RunnerScript>().Hit(fireballDamage);
                    }
                    else if (t.CompareTag("muscleshurtbox"))
                    {
                        t.transform.parent.GetComponent<MusclesScript>().Hit(fireballDamage);
                    }
                    else if (t.CompareTag("spitterhurtbox"))
                    {
                        t.transform.parent.GetComponent<SpitterScript>().Hit(fireballDamage);
                    }
                    else if (t.CompareTag("explosivebarrel"))
                    {
                        if (t.name == this.name)
                            break;
                        t.transform.GetComponent<ExplosiveBarrelScript>().Explode();
                    }
                    else if (t.CompareTag("box"))
                    {
                        if (!boxBroken) //forwarded since explosivebarrelscript does not have access to an audiomanager, also only wanting to only have one sound played hence it being here instead of on the boxscript
                        {
                            boxBroken = true;
                            player.GetComponent<PlayerScript>().ForwardPlayBoxBreak();
                        }
                        t.transform.GetComponent<BoxScript>().BreakBox();
                    }
                    else if (t.CompareTag("screen"))
                    {
                        t.transform.GetChild(0).transform.GetComponent<ScreenScript>().BreakScreen();
                    }
                    else if (t.CompareTag("player"))
                    {
                        playerS.Hit(fireballDamage);
                    }
                }
            }

            audioS.PlayFireball();

            StartCoroutine("Dispose");
        }
    }

    IEnumerator Dispose()
    {
        yield return new WaitForSeconds(0.05f);
        ps.Stop();
        active = false;
        light.enabled = false;
        rb.useGravity = false;
        transform.position = new Vector3(100f, -100f, 150f);
        rb.velocity = new Vector3(0, 0, 0);
    }

    private void CheckTargets()
    {
        targets.Clear();

        for (int i = 0; i < RAYCAST_DIVISIONS; i++) // first quadrent
        {
            IncludeIfUnique((1f * (RAYCAST_DIVISIONS - i) / RAYCAST_DIVISIONS), (1f * i / RAYCAST_DIVISIONS), i);
        }
        for (int i = 0; i < RAYCAST_DIVISIONS; i++) // second quadrent
        {
            IncludeIfUnique(-(1f * (RAYCAST_DIVISIONS - i) / RAYCAST_DIVISIONS), (1f * i / RAYCAST_DIVISIONS), i);
        }
        for (int i = 0; i < RAYCAST_DIVISIONS; i++) // third quadrent
        {
            IncludeIfUnique(-(1f * (RAYCAST_DIVISIONS - i) / RAYCAST_DIVISIONS), -(1f * i / RAYCAST_DIVISIONS), i);
        }
        for (int i = 0; i < RAYCAST_DIVISIONS; i++) // fourth quadrent
        {
            IncludeIfUnique(-(1f * (RAYCAST_DIVISIONS - i) / RAYCAST_DIVISIONS), -(1f * i / RAYCAST_DIVISIONS), i);
        }
    }

    private void IncludeIfUnique(float x, float z, int i) // adds if not already in list
    {
        if(Physics.Raycast(new Vector3(transform.position.x, 0.5f, transform.position.z), new Vector3(x, 0, z), out hit))
        {
            hitObject = hit.transform.gameObject;
            validHit = true;
            foreach (GameObject t in targets) // compares to see if hit is a unique hit
                if (t == hitObject)
                    validHit = false;
            if (validHit)
                targets.Add(hitObject);
        }
    }


}

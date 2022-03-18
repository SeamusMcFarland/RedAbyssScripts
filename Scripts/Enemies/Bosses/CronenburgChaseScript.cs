using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CronenburgChaseScript : MonoBehaviour
{
    const float MAX_HEALTH = 12f;
    const float RECOVERY_TIME = 20f;
    const float BASE_SPEED = 4f;
    float health;
    bool recovering;
    bool striking;
    bool locked;
    bool hitboxTriggered;
    AnimationScript animationS;
    GameObject bloodM;
    BloodManagerScript bloodMS;
    StrikeEffectManagerScript semS;
    const float BASE_HITSTUN = 0.15f;
    GameObject player;
    PlayerScript playerS;

    float playerDistance;
    float strikeDistance;
    const float BASE_DAMAGE = 3f;
    NavMeshAgent nma;
    float startup;
    float afterlag;

    float xDiff;
    float zDiff;
    float rotation;
    float passiveX;
    float passiveZ;

    List<Collider> allColliders = new List<Collider>();
    GameObject rendererO;
    GameObject tankO;

    public MeshRenderer tankMesh;
    public Material darkTank;
    public Material lightTank;

    public DialogueScript dialogueS;
    bool seenEvent, unseenEvent, p2Event, p4Event;
    SceneManagerScript smS;

    // Start is called before the first frame update
    void Start()
    {
        smS = GameObject.FindWithTag("scenemanager").GetComponent<SceneManagerScript>();
        nma = GetComponent<NavMeshAgent>();
        strikeDistance = 1.5f;
        startup = 0.1f;
        afterlag = 1f;
        
        foreach(Collider coll in this.GetComponentsInChildren<Collider>())
            allColliders.Add(coll);
        rendererO = this.GetComponentInChildren<MeshRenderer>().transform.gameObject;
        tankO = tankMesh.gameObject;

        player = GameObject.FindGameObjectWithTag("player");
        playerS = player.GetComponent<PlayerScript>();
        animationS = GetComponentInChildren<AnimationScript>();
        hitboxTriggered = false;
        health = MAX_HEALTH;
        recovering = false;
        locked = false;
        striking = false;
        bloodM = GameObject.FindGameObjectWithTag("bloodmanager");
        bloodMS = bloodM.GetComponent<BloodManagerScript>();
        semS = GameObject.FindGameObjectWithTag("strikeeffectmanager").GetComponent<StrikeEffectManagerScript>();
        passiveX = transform.position.x;
        passiveZ = transform.position.y;
        if (smS.GetTankSaveProgress() >= 2)
            p2Event = true;
        if (smS.GetTankSaveProgress() >= 4)
            p4Event = true;
    }

    private void CheckDirection()
    {
        if (!recovering && !locked)
        {
            if (PlayerWithinSight())
            {
                xDiff = player.transform.position.x - transform.position.x;
                zDiff = player.transform.position.z - transform.position.z;
            }
            else
            {
                xDiff = transform.position.x - passiveX;
                zDiff = transform.position.z - passiveZ;
                passiveX = transform.position.x;
                passiveZ = transform.position.z;
            }
            rotation = Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff);
        }
    }

    private bool PlayerWithinSight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit))
        {
            if (hit.transform.CompareTag("player"))
                return true;
            else
                return false;
        }
        else
            return false;
    }

    // Update is called once per frame
    void Update()
    {
        playerDistance = Get2DDistance(transform, player.transform);
        if (!recovering)
        {

            if (smS.GetTankSaveProgress() < 1)
            {
                if (PlayerWithinSight())
                {
                    if (!seenEvent)
                    {
                        dialogueS.PrintDialogue("CronenburgSeen");
                        seenEvent = true;
                    }
                }
                else if (!unseenEvent && seenEvent)
                {
                    dialogueS.PrintDialogue("CronenburgUnseen");
                    unseenEvent = true;
                }
            }
            else if (smS.GetTankSaveProgress() == 2 && !p2Event)
            {
                dialogueS.PrintDialogue("CronenburgProgress2");
                p2Event = true;
            }
            else if (smS.GetTankSaveProgress() == 4 && !p4Event)
            {
                dialogueS.PrintDialogue("CronenburgProgress4");
                p4Event = true;
            }


            if (playerDistance < strikeDistance)
                    Attack();
            SetSpeed();
            CheckDirection();
            GetComponent<NavMeshAgent>().destination = player.transform.position;
        }
        transform.rotation = Quaternion.Euler(90f, -rotation, 0);
    }

    public void SetHitboxTriggered(bool t)
    {
        hitboxTriggered = t;
    }

    public void Hit(float dam)
    {
        if (!recovering)
        {
            health -= dam;
            semS.SpawnStrikeEffect(transform.position, 1);
            bloodMS.SpawnBlood(transform.position);
            if (health <= 0f)
                Recover();
            else
                StartCoroutine("EndLocked", 1f);
        }
    }

    public void Hit(float dam, float hitstunMod)
    {
        if (!recovering)
        {
            animationS.SetState(1);
            health -= dam;
            locked = true;
            GetComponent<NavMeshAgent>().speed = 0f;
            semS.SpawnStrikeEffect(transform.position, 1);
            bloodMS.SpawnBlood(transform.position);
            if (health <= 0f)
                Recover();
            else
                StartCoroutine("EndLocked", hitstunMod);
        }
    }

    IEnumerator EndLocked(float hitstunMod) // will automatically get free even if shot again while in hitstun after first hitstun is over
    {
        yield return new WaitForSeconds(BASE_HITSTUN * hitstunMod);
        if (!recovering && !striking)
        {
            animationS.SetState(3);
            locked = false;
        }
    }

    private void Recover()
    {
        rendererO.transform.localPosition = new Vector3(rendererO.transform.localPosition.x, rendererO.transform.localPosition.y, rendererO.transform.localPosition.z + 0.04f); // moves renderer below player renderer
        tankO.transform.localPosition = new Vector3(tankO.transform.localPosition.x, tankO.transform.localPosition.y, tankO.transform.localPosition.z + 0.04f); //
        foreach (Collider coll in allColliders)
            coll.enabled = false;
        animationS.SetState(4);
        tankMesh.material = darkTank;
        recovering = true;
        StartCoroutine("FinishRecovery");
    }

    IEnumerator FinishRecovery()
    {
        yield return new WaitForSeconds(0.01f);
        nma.velocity = new Vector3(0,0,0);
        nma.isStopped = true;
        yield return new WaitForSeconds(RECOVERY_TIME);
        nma.isStopped = false;
        rendererO.transform.localPosition = new Vector3(rendererO.transform.localPosition.x, rendererO.transform.localPosition.y, rendererO.transform.localPosition.z - 0.04f);
        tankO.transform.localPosition = new Vector3(tankO.transform.localPosition.x, tankO.transform.localPosition.y, tankO.transform.localPosition.z - 0.04f);
        foreach (Collider coll in allColliders)
            coll.enabled = true;
        animationS.SetState(3);
        tankMesh.material = lightTank;
        health = MAX_HEALTH;
        locked = false;
        striking = false;
        recovering = false;
    }

    private void SetSpeed()
    {
        if (striking)
            GetComponent<NavMeshAgent>().speed = 0f;
        else
        {
            GetComponent<NavMeshAgent>().speed = BASE_SPEED;
        }
    }

    void Attack()
    {
        if (locked == false)
        {
            animationS.SetState(2);
            locked = true;
            striking = true;
            nma.velocity = new Vector3(0, 0, 0);
            nma.isStopped = true;
            StartCoroutine("Startup");
        }
    }

    IEnumerator Startup()
    {
        yield return new WaitForSeconds(startup);
        if (!recovering)
        {
            StartCoroutine("Strike");
        }
    }

    IEnumerator Strike()
    {
        if (!recovering)
        {
            if (hitboxTriggered)
            {
                playerS.Hit(BASE_DAMAGE);
                float tempXDiff = player.transform.position.x - transform.position.x;
                float tempZDiff = player.transform.position.z - transform.position.z;
                float tempDevide = Mathf.Abs(tempXDiff) + Mathf.Abs(tempZDiff);
                playerS.KnockOver(3f * new Vector3(tempXDiff / tempDevide, 0, tempZDiff / tempDevide));
                afterlag = 2f;
            }
            yield return new WaitForSeconds(afterlag);
            afterlag = 1;
            nma.isStopped = false;
            if (!recovering)
            {
                locked = false;

                animationS.SetState(3);
                striking = false;
            }
        }
    }


    private float Get2DDistance(Transform t1, Transform t2)
    {
        return Mathf.Sqrt(Mathf.Pow(t1.position.x - t2.position.x, 2f) + Mathf.Pow(t1.position.z - t2.position.z, 2f));
    }


}

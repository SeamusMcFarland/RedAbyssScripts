using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CronenburgBossScript : MonoBehaviour
{
    const float MAX_HEALTH = 30f;
    const float RECOVERY_TIME = 4f;
    const float BASE_SPEED = 6.5f;
    float health; //temporary hp
    float gas; //permanent hp
    bool recovering;
    bool dead;
    bool striking;
    bool locked;
    bool hitboxTriggered;
    bool chargeHitbox;
    AnimationScript animationS;
    GameObject bloodM;
    BloodManagerScript bloodMS;
    StrikeEffectManagerScript semS;
    const float BASE_HITSTUN = 0.15f;
    GameObject player;
    PlayerScript playerS;

    float playerDistance;
    float strikeDistance;
    const float BASE_DAMAGE = 2f;
    NavMeshAgent nma;
    float startup;
    float afterlag;

    float xDiff;
    float zDiff;
    float rotation;
    float tempRotation;
    float passiveX;
    float passiveZ;

    public Collider[] allColliders;
    GameObject rendererO;

    MeshRenderer tankMesh;
    public GameObject tankO;
    public Material[] flashTanks;
    public Material[] lightTanks;
    float flashTimer;
    bool flashingState;
    bool reddened;
    int tanksRemaining;
    bool tankVulnerable;
    Collider tankColl;

    public GameObject psO;
    ParticleSystem ps;

    const float BASE_ATTACK_COOLDOWN = 1f;
    const float MAX_GAS = 500f;
    float attackCooldown;
    float tsla; // time since last attack
    bool attackState;

    bool active;
    public Image healthbar; // is actually the "gas bar" in this instance
    public Image healthbarOuter;
    public Image healthbarInner;
    float savedX;
    public RawImage minimap; // need to switch this off or else it gets in the way

    MeshRenderer mr;
    Color savedColor;
    bool charging;
    Vector3 targetPosition;
    RaycastHit hit;
    bool hitWithCharge;

    float bRepeatMod;
    float fRepeatMod;
    float lRepeatMod;
    float tRepeatMod; // make it less likely to have repeated actions taken

    public AcidBombScript abS;

    const float FLAMETHROWER_STARTUP = 0.1f;
    const float TURN_RATE = 3f;
    public ParticleSystem psf; // flamethrower
    float hitboxTimer;
    int hitboxSelected;
    public FlameHitboxScript[] flameHitboxes;
    bool flaming;

    GameObject[] audioM;
    AudioScript audioS;
    const int AUDIO_NUM = 2; // monster audio manager

    int dialogueNum;
    public DialogueScript dialogueS;

    public CronenburgCorpseScript ccS;

    Collider[] actuallyEveryCollider;

    // Start is called before the first frame update
    void Start()
    {
        dialogueNum = 0;

        actuallyEveryCollider = GetComponentsInChildren<Collider>();

        bRepeatMod = 0;
        fRepeatMod = 0;
        lRepeatMod = 0;
        tRepeatMod = 0;

        psf.Stop();
        flaming = false;

        ps = psO.GetComponent<ParticleSystem>();
        mr = GetComponentInChildren<MeshRenderer>();
        savedColor = mr.material.color;

        gas = MAX_GAS;
        SetHealthbarEnabled(false);
        savedX = healthbar.rectTransform.localPosition.x;

        active = false;
        tankVulnerable = false;
        attackState = false;

        nma = GetComponent<NavMeshAgent>();
        strikeDistance = 1.5f;
        startup = 0.1f;
        afterlag = 1f;
        
        flashTimer = 0;
        flashingState = false;
        reddened = false;
        tanksRemaining = 3;
        tankMesh = tankO.GetComponent<MeshRenderer>();
        tankColl = tankO.GetComponent<Collider>();
        tankColl.enabled = false;

        rendererO = this.GetComponentInChildren<MeshRenderer>().transform.gameObject;

        player = GameObject.FindGameObjectWithTag("player");
        playerS = player.GetComponent<PlayerScript>();
        animationS = GetComponentInChildren<AnimationScript>();
        hitboxTriggered = false;
        chargeHitbox = false;
        health = MAX_HEALTH;
        recovering = false;
        dead = false;
        locked = false;
        striking = false;
        bloodM = GameObject.FindGameObjectWithTag("bloodmanager");
        bloodMS = bloodM.GetComponent<BloodManagerScript>();
        semS = GameObject.FindGameObjectWithTag("strikeeffectmanager").GetComponent<StrikeEffectManagerScript>();
        passiveX = transform.position.x;
        passiveZ = transform.position.y;

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
        if (active && !playerS.GetPaused())
        {
            if (!dead)
            {
                RefreshHealthbar();
                attackCooldown = BASE_ATTACK_COOLDOWN + (1f * gas / MAX_GAS);
                playerDistance = Get2DDistance(transform.position, player.transform.position);
                if (!recovering && !attackState)
                {
                    if (tsla > attackCooldown)
                        SpecialAttack();
                    else
                        tsla += Time.deltaTime;

                    if (playerDistance < strikeDistance)
                        Attack();
                    SetSpeed();
                    CheckDirection();
                    GetComponent<NavMeshAgent>().destination = player.transform.position;
                }
                if (!recovering)
                    flashingState = false;
                else if (tanksRemaining > 0)
                {
                    if (flashTimer > 0.5f)
                    {
                        flashingState = !flashingState;
                        flashTimer = 0;
                    }
                    flashTimer += Time.deltaTime;
                }

                if (tanksRemaining == 0)
                    gas -= Time.deltaTime * 10f;
                else if (tanksRemaining == 1)
                    gas -= Time.deltaTime * 5f;
                if (tanksRemaining == 2)
                    gas -= Time.deltaTime * 2f;
                SetTankMaterial();

                transform.rotation = Quaternion.Euler(90f, -rotation, 0);

                if (charging && chargeHitbox && !hitWithCharge)
                {
                    hitWithCharge = true;
                    playerS.Hit(BASE_DAMAGE);
                    playerS.KnockOver(1.7f * new Vector3(player.transform.position.x - transform.position.x, 0, player.transform.position.z - transform.position.z));
                }

                if (reddened)
                    mr.material.color = new Color(255, 0, 0);
                else
                    mr.material.color = savedColor;

                if (!recovering && flaming)
                {
                    //rotation
                    xDiff = player.transform.position.x - transform.position.x;
                    zDiff = player.transform.position.z - transform.position.z;
                    tempRotation = Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff);

                    if (Mathf.Abs(tempRotation - rotation) < 180f)
                    {
                        if (tempRotation < rotation)
                            rotation = rotation - TURN_RATE;
                        else
                            rotation = rotation + TURN_RATE;
                    }
                    else
                    {
                        if (tempRotation < rotation)
                            rotation = rotation + TURN_RATE;
                        else
                            rotation = rotation - TURN_RATE;
                    }
                    transform.rotation = Quaternion.Euler(90f, -rotation, 0);

                    //hitboxes
                    hitboxTimer += Time.deltaTime;
                    if (hitboxTimer > 0.1f)
                    {
                        hitboxTimer = 0;
                        hitboxSelected++;
                        if (hitboxSelected >= flameHitboxes.Length)
                            hitboxSelected = 0;
                        flameHitboxes[hitboxSelected].Release(rotation, psf.transform.position);
                    }
                }

                if (gas <= 0)
                    Death();
            }
            else
            {
                foreach (Collider c in actuallyEveryCollider)
                    c.enabled = false;
                mr.enabled = false;
                transform.position = new Vector3(300f, 300f, 300f);
            }

            //dialogue
            if (gas < MAX_GAS * 0.95f && dialogueNum == 0)
            {
                dialogueS.PrintDialogue("Cronenburg1");
                dialogueNum++;
            }
            else if (gas < MAX_GAS * 0.90f && dialogueNum == 1)
            {
                dialogueS.PrintDialogue("Cronenburg2");
                dialogueNum++;
            }
            else if (gas < MAX_GAS * 0.80f && dialogueNum == 2)
            {
                dialogueS.PrintDialogue("Cronenburg3");
                dialogueNum++;
            }
            else if (gas < MAX_GAS * 0.65f && dialogueNum == 3)
            {
                dialogueS.PrintDialogue("Cronenburg4");
                dialogueNum++;
            }
            else if (gas < MAX_GAS * 0.45f && dialogueNum == 4)
            {
                dialogueS.PrintDialogue("Cronenburg5");
                dialogueNum++;
            }
            else if (gas < MAX_GAS * 0.20f && dialogueNum == 5)
            {
                dialogueS.PrintDialogue("Cronenburg6");
                dialogueNum++;
            }

        }
    }

    private void SpecialAttack() // delayed 1 frame if all conditions fail
    {
        if ((Random.value * bRepeatMod) < 0.2f)
        {
            attackState = true;
            animationS.SetState(1);
            StartCoroutine("FlameThrower");
            NewRepeats(1);
        }
        else if ((Random.value * fRepeatMod) < 0.25f)
        {
            attackState = true;
            animationS.SetState(1);
            StartCoroutine("FlashCharge");
            NewRepeats(2);
        }
        else if ((Random.value * lRepeatMod) < 0.33f)
        {
            attackState = true;
            animationS.SetState(1);
            TossAcid();
            NewRepeats(3);
        }
        /*else if ((Random.value * lRepeatMod) < 0.5f)
        {
            attackState = true;
            animationS.SetState(1);
            TossAcid();
            NewRepeats(4);
        }*/
    }

    private void NewRepeats(int n) // increments picked action and resets all others to 0
    {
        if (n == 1)
        {
            bRepeatMod++;
            fRepeatMod = 1f;
            lRepeatMod = 1f;
            tRepeatMod = 1f;
        }
        else if (n == 2)
        {
            bRepeatMod = 1f;
            fRepeatMod++;
            lRepeatMod = 1f;
            tRepeatMod = 1f;
        }
        else if (n == 3)
        {
            bRepeatMod = 1f;
            fRepeatMod = 1f;
            lRepeatMod++;
            tRepeatMod = 1f;
        }
        else if (n == 4)
        {
            bRepeatMod = 1f;
            fRepeatMod = 1f;
            lRepeatMod = 1f;
            tRepeatMod++;
        }
        else
            print("ERROR, INVALID NEW REPEAT METHOD CALL INPUT");
    }

    IEnumerator FlameThrower()
    {
        animationS.SetState(5);
        GetComponent<NavMeshAgent>().speed = 0;
        nma.isStopped = true;
        nma.velocity = new Vector3(0,0,0);
        audioS.PlayHeavyEquip();

        yield return new WaitForSeconds(FLAMETHROWER_STARTUP);
        if (!recovering && !dead)
        {
            psf.Play();
            hitboxSelected = 0;
            hitboxTimer = 0;
            flaming = true;
            yield return new WaitForSeconds(2f);
        }
            psf.Stop();
            flaming = false;
            yield return new WaitForSeconds(1f);
            if (!recovering)
                animationS.SetState(3);
            tsla = 0;
            attackState = false;
        nma.isStopped = false;

    }

    IEnumerator FlashCharge()
    {
        for (int i = 0; i < 18 + (int)(8f * gas / MAX_GAS); i++)
        {
            if (!dead)
            {
                yield return new WaitForSeconds(0.01f);
                mr.material.color = new Color(255f, 0, 0);
            }
        }
        if (!recovering && !dead)
        {
            GetComponent<NavMeshAgent>().speed = BASE_SPEED * 3f;
            charging = true;
            animationS.SetState(3);

            hitWithCharge = false;

            xDiff = player.transform.position.x - transform.position.x;
            zDiff = player.transform.position.z - transform.position.z;
            rotation = Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff);
            transform.rotation = Quaternion.Euler(90f, -rotation, 0);
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 4f, transform.position.z), transform.right, out hit))
            {
                targetPosition = new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z);
            }

            animationS.SetAnimationGap(0);
        }
        StartCoroutine("CheckIfDoneCharging");
    }

    IEnumerator CheckIfDoneCharging()
    {
        print("still charging at: " + Time.time);
        yield return new WaitForSeconds(0.01f);
        if (!recovering && !dead)
        {
            GetComponent<NavMeshAgent>().speed = BASE_SPEED * 6f;
            GetComponent<NavMeshAgent>().destination = targetPosition;
        }
        if ((playerDistance < strikeDistance || Get2DDistance(transform.position, targetPosition) < 0.5f) || dead || recovering)
        {
            print("reached destination at: " + Time.time);
            tsla = 0;
            charging = false;
            attackState = false;
            animationS.SetAnimationGap(0.15f);
        }
        else
            StartCoroutine("CheckIfDoneCharging");
    }

    private void TossAcid()
    {
        GetComponent<NavMeshAgent>().speed = 0;
        nma.velocity = new Vector3(0, 0, 0);

        animationS.SetState(6);

        abS.Throw(transform.position, rotation, Random.Range(5f,8f));

        StartCoroutine("EndToss");
    }

    IEnumerator EndToss()
    {
        yield return new WaitForSeconds(.3f);
        tsla = 0;
        if(!recovering)
            animationS.SetState(3);
        attackState = false;
    }

    

    private void SetTankMaterial()
    {
        if (flashingState)
            tankMesh.material = flashTanks[3 - tanksRemaining];
        else
            tankMesh.material = lightTanks[3 - tanksRemaining];
        
        if (tanksRemaining == 3)
            tankO.transform.localScale = new Vector3(0.6f,1.9f,1f);
        else if(tanksRemaining == 2)
            tankO.transform.localScale = new Vector3(0.7f, 1.9f, 1f);
        else if (tanksRemaining == 2)
            tankO.transform.localScale = new Vector3(0.8f, 1.9f, 1f);
        else if (tanksRemaining == 2)
            tankO.transform.localScale = new Vector3(0.8f, 1.9f, 1f);
    }

    public void SetHitboxTriggered(bool t)
    {
        hitboxTriggered = t;
    }

    public void SetChargeHitbox(bool t)
    {
        chargeHitbox = t;
    }

    public void Hit(float dam)
    {
        if (!recovering)
        {
            health -= dam;
            semS.SpawnStrikeEffect(transform.position, 1);
            gas -= dam;
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
            health -= dam;
            semS.SpawnStrikeEffect(transform.position, 1);
            if (!attackState)
            {
                print("stun hit");
                animationS.SetState(1);
                locked = true;
                GetComponent<NavMeshAgent>().speed = 0f;
            }
            bloodMS.SpawnBlood(transform.position);
            if (health <= 0f)
                Recover();
            else if(!attackState)
                StartCoroutine("EndLocked", hitstunMod);
        }
    }

    IEnumerator EndLocked(float hitstunMod) // will automatically get free even if shot again while in hitstun after first hitstun is over
    {
        yield return new WaitForSeconds(BASE_HITSTUN * hitstunMod);
        if (!recovering && !striking && !attackState)
        {
            animationS.SetState(3);
            locked = false;
        }
    }

    private void Recover()
    {
        rendererO.transform.localPosition = new Vector3(rendererO.transform.localPosition.x, rendererO.transform.localPosition.y, rendererO.transform.localPosition.z + 0.04f); // moves renderer below player renderer
        tankO.transform.localPosition = new Vector3(tankO.transform.localPosition.x, tankO.transform.localPosition.y, tankO.transform.localPosition.z + 0.04f); //
        nma.velocity = new Vector3(0,0,0);
        nma.isStopped = true;
        if (tanksRemaining > 0)
        {
            print("tank vulnerable");
            tankVulnerable = true;
            tankColl.enabled = true;
        }
        foreach (Collider coll in allColliders)
            coll.enabled = false;
        animationS.SetState(4);
        recovering = true;
        flaming = false;
        psf.Stop();
        StartCoroutine("FinishRecovery");
    }

    IEnumerator FinishRecovery()
    {
        yield return new WaitForSeconds(0.01f);
        nma.velocity = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(RECOVERY_TIME);
        nma.isStopped = false;
        if (!dead)
        {
            foreach (Collider coll in allColliders)
                coll.enabled = true;
            tankVulnerable = false;
            tankColl.enabled = false;
            animationS.SetState(3);
            health = MAX_HEALTH;
            locked = false;
            striking = false;
            recovering = false;
            rendererO.transform.localPosition = new Vector3(rendererO.transform.localPosition.x, rendererO.transform.localPosition.y, rendererO.transform.localPosition.z - 0.04f);
            tankO.transform.localPosition = new Vector3(tankO.transform.localPosition.x, tankO.transform.localPosition.y, tankO.transform.localPosition.z - 0.04f);
        }
    }

    private void SetSpeed()
    {
        if (striking)
            GetComponent<NavMeshAgent>().speed = 0f;
        else
        {
            GetComponent<NavMeshAgent>().speed = BASE_SPEED + (1.3f - (1.3f * Mathf.Pow(gas / MAX_GAS, 2f)));
        }
    }

    void Attack()
    {
        if (!locked)
        {
            print("attacking at: " + Time.time);
            attackState = true;
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
        StartCoroutine("Strike");
    }

    IEnumerator Strike()
    {
        if (!recovering && !dead)
        {
            if (!charging && hitboxTriggered)
            {
                print("punch attack at: " + Time.time);
                playerS.Hit(BASE_DAMAGE);
                float tempXDiff = player.transform.position.x - transform.position.x;
                float tempZDiff = player.transform.position.z - transform.position.z;
                float tempDevide = Mathf.Abs(tempXDiff) + Mathf.Abs(tempZDiff);
                playerS.KnockOver(3f * new Vector3(tempXDiff / tempDevide, 0, tempZDiff / tempDevide));
                afterlag = 2f;
            }
            yield return new WaitForSeconds(afterlag);
            afterlag = 1;
            if (!recovering && !dead)
            {
                locked = false;

                animationS.SetState(3);
                striking = false;
            }
            CheckDirection();
            attackState = false;
            nma.isStopped = false;
        }
    }

    public void BurstTank()
    {
        if (tankVulnerable && tanksRemaining > 0)
        {
            if (tanksRemaining == 3)
                ps.transform.localPosition = new Vector3(-0.95f, 0.45f, -0.35f);
            else if (tanksRemaining == 2)
                ps.transform.localPosition = new Vector3(-0.95f, -0.77f, -0.35f);
            else if (tanksRemaining == 1)
                ps.transform.localPosition = new Vector3(-0.95f, -0.15f, -0.35f);
            else
                print("ERROR! INVALID TANKS REMAINING ON BURST");
            ps.Play();
            tankVulnerable = false;
            flashingState = false;
            tankColl.enabled = false;
            tanksRemaining--;
        }
    }

    private float Get2DDistance(Vector3 t1, Vector3 t2)
    {
        return Mathf.Sqrt(Mathf.Pow(t1.x - t2.x, 2f) + Mathf.Pow(t1.z - t2.z, 2f));
    }

    public void Activate()
    {
        SetHealthbarEnabled(true);
        minimap.enabled = false;
        active = true;
    }

    private void SetHealthbarEnabled(bool e)
    {
        healthbar.enabled = e;
        healthbarOuter.enabled = e;
        healthbarInner.enabled = e;
    }

    private void RefreshHealthbar()
    {
        if (gas >= 0) // to prevent underflow
        {
            healthbar.transform.localScale = new Vector2(gas / MAX_GAS, healthbar.transform.localScale.y);
            healthbar.rectTransform.localPosition = new Vector2(savedX - ((50f / MAX_GAS) * (MAX_GAS - gas)), healthbar.rectTransform.localPosition.y);
        }
    }

    private void Death()
    {
        dead = true;
        playerS.CronenburgDowned();

        foreach (Collider c in actuallyEveryCollider)
            c.enabled = false;
        mr.enabled = false;

        ccS.Spawn(transform.position, rotation);
    }


}

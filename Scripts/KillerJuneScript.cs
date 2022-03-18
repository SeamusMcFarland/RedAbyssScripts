using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class KillerJuneScript : MonoBehaviour
{
    public int num;

    const float MAX_HEALTH = 4f;
    const float RECOVERY_TIME = 4f;
    const float BASE_SPEED = 5f;
    const float SPIN_SPEED = 40f;
    const float BASE_ACCELERATION = 100f;
    const float SPIN_ACCELERATION = 2000f;
    float health;
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
    UnityEngine.AI.NavMeshAgent nma;
    float startup;
    float afterlag;

    float xDiff;
    float zDiff;
    float rotation;
    float tempRotation;
    float passiveX;
    float passiveZ;

    GameObject rendererO;

    const float BASE_ATTACK_COOLDOWN = 1f;
    float attackCooldown;
    float tsla; // time since last attack
    bool attackState;

    bool active;

    MeshRenderer mr;
    Color savedColor;
    bool charging;
    Vector3 targetPosition;
    RaycastHit hit;
    bool hitWithCharge;

    float aRepeatMod;
    float bRepeatMod;
    float cRepeatMod;
    float dRepeatMod; // make it less likely to have repeated attacks taken

    GameObject[] audioM;
    AudioScript audioS;
    const int AUDIO_NUM = 2; // monster audio manager

    int dialogueNum;
    public DialogueScript dialogueS;

    Collider[] actuallyEveryCollider;

    //From JacobScript
    float cooldown;
    bool rotationLocked;
    Vector3 facingVector;
    float extraCooldown; // reduced as fight goes on, making attacks come out faster and more difficult to avoid as health decreases

    public GameObject hurtbox;
    bool omniscient;
    bool burnNoisy;

    const float BURN_MODIFIER = 0.5f;

    bool dead;
    bool actuallyDead;
    bool deathMuted;

    GameObject goreM;
    GoreManagerScript goreMS;

    const float DEATH_POINT = 2f;

    KillerJuneTargetManagerScript targetMS;
    int currentTarget;
    SwipeManagerScript swipeMS;
    const float SWIPE_DELAY = 0.3f;
    const float EXPLODE_DELAY = 0.5f;
    const float SPIN_DELAY = 0.3f;
    const float SPIN_RATE = 500f;
    bool shaking;
    Vector3 savedPosition;

    MonsterHealthbarScript mhS;
    MonsterHealthbarManagerScript mhmS;
    float healthbarTimer; // counts down until disabling healthbar
    const float HEALTHBAR_TIME = 2f;

    float meleeCooldown;
    const float MELEE_COOLDOWN = 0.4f;
    const float MELEE_DAMAGE = 2f;

    public bool deathOnSpawn;

    float countDown;
    const float MAX_COUNTDOWN = 10f;
    bool explodeDying;
    ExplodeEffectScript explodeES;
    const float EXPLOSION_DAMAGE = 5;
    bool explosionHitboxTriggered;

    Quaternion delayedRotation; // need this or else won't calculate rotation collisions

    float timeDied; // prevents extra blood explosion sound effect

    bool explodingFromWhole; // if explode attacking while not zombified
    bool spinning;

    public bool clone;
    public KillerJuneScript attachedClone;
    bool clonedSelf;
    bool cloning;
    const float CLONE_SPEED = 0.01f;

    float frameNormalizer;

    bool spinRight;

    TorturedScript torturedS;

    void Start()
    {
        torturedS = GameObject.FindGameObjectWithTag("tortured").GetComponent<TorturedScript>();

        omniscient = true;

        explodingFromWhole = false;

        if (deathOnSpawn)
            StartCoroutine("DelayDeathOnSpawn");

        explodeES = GameObject.FindGameObjectWithTag("explodeeffect").GetComponent<ExplodeEffectScript>();
        targetMS = GameObject.FindGameObjectWithTag("targetmanager").GetComponent<KillerJuneTargetManagerScript>();
        swipeMS = GameObject.FindGameObjectWithTag("swipemanager").GetComponent<SwipeManagerScript>();
        goreM = GameObject.FindGameObjectWithTag("goremanager");
        goreMS = goreM.GetComponent<GoreManagerScript>();

        burnNoisy = false;

        cooldown = 0;
        rotationLocked = false;

        dialogueNum = 0;

        actuallyEveryCollider = GetComponentsInChildren<Collider>();

        aRepeatMod = 1;
        bRepeatMod = 1;
        cRepeatMod = 1;
        dRepeatMod = 1;

        mr = GetComponentInChildren<MeshRenderer>();
        savedColor = mr.material.color;

        active = false;
        attackState = false;

        nma = GetComponent<NavMeshAgent>();
        nma.speed = BASE_SPEED;
        nma.acceleration = BASE_ACCELERATION;
        strikeDistance = 1.5f;
        startup = 0.1f;
        afterlag = 1f;

        rendererO = this.GetComponentInChildren<MeshRenderer>().transform.gameObject;

        player = GameObject.FindGameObjectWithTag("player");
        playerS = player.GetComponent<PlayerScript>();
        animationS = GetComponentInChildren<AnimationScript>();
        hitboxTriggered = false;
        chargeHitbox = false;
        health = MAX_HEALTH;
        dead = false;
        locked = false;
        bloodM = GameObject.FindGameObjectWithTag("bloodmanager");
        bloodMS = bloodM.GetComponent<BloodManagerScript>();
        semS = GameObject.FindGameObjectWithTag("strikeeffectmanager").GetComponent<StrikeEffectManagerScript>();
        passiveX = transform.position.x;
        passiveZ = transform.position.y;

        mhmS = Object.FindObjectOfType<MonsterHealthbarManagerScript>();

        animationS.SetState(1);
        mr.enabled = false;

        nma.updateRotation = false;

        SetupAudio();
    }

    IEnumerator DelayDeathOnSpawn()
    {
        deathMuted = true;
        yield return new WaitForSeconds(0.01f);
        Death();
        yield return new WaitForSeconds(0.1f);
        deathMuted = false;
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
        if (!actuallyDead && !torturedS.GetDead())
        {
            if ((PlayerWithinSight()) || omniscient)
            {
                xDiff = player.transform.position.x - transform.position.x;
                zDiff = player.transform.position.z - transform.position.z;

                rotation = Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff);
            }
            else
            {
                xDiff = transform.position.x - passiveX;
                zDiff = transform.position.z - passiveZ;
                passiveX = transform.position.x;
                passiveZ = transform.position.z;

                if (Mathf.Sqrt(Mathf.Pow(xDiff, 2f) + Mathf.Pow(zDiff, 2f)) >= 0.001f)
                    rotation = Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff);
            }
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
        frameNormalizer = Time.deltaTime / 0.014f;
        playerDistance = Get2DDistance(transform.position, player.transform.position);
        if (active && !actuallyDead && !playerS.GetPaused() && !torturedS.GetDead())
        {
            CheckHealthbar();

            if (shaking)
                nma.Warp(savedPosition + new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f)));

            if (dead && !actuallyDead)
                nma.destination = player.transform.position;

            if (!dead)
                CheckAttack();

            print("rotation locked? " + rotationLocked + "   at: " + Time.time);
            if (!rotationLocked)
                CheckDirection();

            delayedRotation = Quaternion.Euler(0, -rotation, 0);
            // !!!!! PUT IN SOUNDS OF ATTACKS, ADD RAYCAST TO HIT PLAYER FOR LIGHTNING


            if (dead)
            {
                if (!locked && meleeCooldown <= 0 && hitboxTriggered && !spinning)
                {
                    playerS.Hit(MELEE_DAMAGE);
                    meleeCooldown = MELEE_COOLDOWN;
                }
                else
                    meleeCooldown -= Time.deltaTime;

                if (countDown <= 0) // eventually destroys zombies to prevent continuous kiting
                {
                    explodeDying = true;
                    ExplodeAttack();
                }
                else
                    countDown -= Time.deltaTime;

            }

            if (spinning)
            {
                if(spinRight)
                    rotation = ((Time.time % 360f) - 180f) * SPIN_RATE;
                else
                    rotation = (180f - (Time.time % 360f)) * SPIN_RATE;
            }

            if (cloning)
            {
                if(clone)
                    nma.Warp(transform.position + transform.forward * CLONE_SPEED * frameNormalizer);
                else
                    nma.Warp(transform.position - transform.forward * CLONE_SPEED * frameNormalizer);
                nma.destination = transform.position;
            }    
        }

    }

    private void FixedUpdate()
    {
        transform.rotation = delayedRotation; // need this or else won't calculate rotation collisions
    }

    private void CheckHealthbar()
    {
        if (mhS != null)
        {
            if (healthbarTimer <= 0f)
            {
                mhS.DisableHealthbar();
                mhS = null;
            }
            else
            {
                mhS.UpdateHealthbar(health / MAX_HEALTH, transform.position);
                healthbarTimer -= Time.deltaTime;
            }
        }
    }

    private void CheckAttack()
    {
        //print("cooldown: " + cooldown + " at: " + Time.deltaTime);
        //print("actually dead:" + actuallyDead + " dead: " + dead);
        if (cooldown <= 0)
        {
            extraCooldown = 0.8f + (health * 2 / MAX_HEALTH) + Random.value;
            CheckDirection();
            rotationLocked = true;
            if ((Random.value * aRepeatMod) < 0.2f)
            {
                cooldown = 100f;
                NewRepeats(1);
                TargetAttack();
            }
            else if ((Random.value * bRepeatMod) < 0.25f)
            {
                cooldown = 100f;
                NewRepeats(2);
                SwipeAttack();
            }
            else if ((Random.value * cRepeatMod) < 0.33f)
            {
                cooldown = 100f;
                explodingFromWhole = true;
                NewRepeats(3);
                ExplodeAttack();
            }
            else if ((Random.value * dRepeatMod) < 0.5f && !clone && !clonedSelf)
            {
                cooldown = 100f;
                NewRepeats(4);
                CloneAttack();
            }
        }
        else
            cooldown -= Time.deltaTime;
    }

    private void TargetAttack()
    {
        print("target attacking at: " + Time.time);
        cooldown = 100f;
        animationS.SetState(9);
        StartCoroutine("TargetAttackLoop", 3);
    }

    IEnumerator TargetAttackLoop(int remaining)
    {
        yield return new WaitForSeconds(0.3f);
        if (!dead && !torturedS.GetDead())
        {
            targetMS.TargetPlayer();
            if (remaining > 0)
                StartCoroutine("TargetAttackLoop", remaining - 1);
            else
            {
                animationS.SetState(1);
                StartCoroutine("Afterlag");
            }
        }
    }

    private void SwipeAttack()
    {
        print("swipe attacking at: " + Time.time);
        cooldown = 100f;
        animationS.SetState(8);
        StartCoroutine("DelaySwipeAttack");
    }

    IEnumerator DelaySwipeAttack()
    {
        yield return new WaitForSeconds(SWIPE_DELAY);
        if (!dead && !torturedS.GetDead())
        {
            animationS.SetState(2);
            swipeMS.Swipe(transform.position, rotation);
        }
        yield return new WaitForSeconds(0.5f);
        if (!dead && !torturedS.GetDead())
        {
            animationS.SetState(1);
            StartCoroutine("Afterlag");
        }
    }

    private void ExplodeAttack()
    {
        print("explode attacking at: " + Time.time);
        countDown = 100f; // prevents repeated calls
        cooldown = 100f;
        if (!dead)
        {
            print("I'm not dead at: " + Time.time);
            animationS.SetState(6);
        }
        dead = true;
        animationS.noCycling = true;
        savedPosition = transform.position;
        shaking = true;
        StartCoroutine("ExplodeShaking");
    }

    IEnumerator ExplodeShaking()
    {
        yield return new WaitForSeconds(EXPLODE_DELAY);
        if (!actuallyDead && !torturedS.GetDead())
        {
            if (explodingFromWhole)
            {
                BrainsGore();
                animationS.SetState(11);
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                BrainsGore();
            }

            if (!actuallyDead && !torturedS.GetDead())
            {
                if (explodeDying || explodingFromWhole)
                {
                    shaking = false;
                    if (explodeDying)
                        animationS.SetState(13);
                    else if (explodingFromWhole)
                        animationS.SetState(12);
                    nma.speed = SPIN_SPEED;
                    nma.acceleration = SPIN_ACCELERATION;
                    spinning = true;
                    StartCoroutine("SpinToExplode", SPIN_DELAY);
                }
                else
                {
                    StartCoroutine("SpinToExplode", 0);
                }
            }
        }
    }

    private void BrainsGore()
    {
        for (int i = 118; i < 123; i++)
            goreMS.SpawnGore(i, transform.position);
        audioS.PlaySmallerBloodExplosion();
    }

    IEnumerator SpinToExplode(float time)
    {
        print("time waiting: " + time);
        yield return new WaitForSeconds(time);
        if (!actuallyDead && !torturedS.GetDead())
        {
            spinning = false;
            shaking = false;
            if (explosionHitboxTriggered)
            {
                /*float tempXDiff = player.transform.position.x - transform.position.x;
                float tempZDiff = player.transform.position.z - transform.position.z;
                float tempDevide = Mathf.Abs(tempXDiff) + Mathf.Abs(tempZDiff);
                playerS.KnockOver(3f * new Vector3(tempXDiff / tempDevide, 0, tempZDiff / tempDevide));*/
                playerS.Hit(EXPLOSION_DAMAGE);
            }

            explodeES.Explode(transform.position);
            Hit(health - DEATH_POINT + 0.01f);
            cooldown = 0;
            semS.SpawnStrikeEffect(transform.position, 1);
            bloodMS.SpawnBlood(transform.position);
            HealthbarHit();
            rotationLocked = false;
            animationS.noCycling = false;
            animationS.SetState(4);
            if (explodeDying || explodingFromWhole) // if WAS triggered by countdown OR doing explode attack while whole
            {
                ActualDeath();
            }
            else
            {
                goreMS.SpawnGore(38, transform.position);
                countDown = Random.Range(MAX_COUNTDOWN / 2f, MAX_COUNTDOWN); // If was NOT triggered by countdown
            }
        }
    }

    private void CloneAttack()
    {
        clonedSelf = true;
        audioS.PlayMultiply();
        countDown = 100f; // prevents repeated calls
        cooldown = 100f;
        animationS.SetState(14);
        if(attachedClone != null)
            attachedClone.BecomeCloned(this.transform);
        cloning = true;
        StartCoroutine("EndClone");
    }

    IEnumerator EndClone()
    {
        yield return new WaitForSeconds(0.5f);
        if (!dead && !torturedS.GetDead())
        {
            cloning = false;
            animationS.SetState(1);
            StartCoroutine("Afterlag");
        }
    }

    public void BecomeCloned(Transform pos)
    {
        Respawn(pos);
        CloneAttack();
    }

    IEnumerator Afterlag()
    {
        yield return new WaitForSeconds(Random.Range(0.3f, 0.5f));
        if (!actuallyDead && !torturedS.GetDead())
        {
            rotationLocked = false;
            cooldown = Random.Range(0.2f, 0.5f);
        }
    }

    private void NewRepeats(int n) // increments picked action and resets all others to 0
    {
        if (n == 1)
        {
            aRepeatMod++;
            bRepeatMod = 1f;
            cRepeatMod = 1f;
            dRepeatMod = 1f;
        }
        else if (n == 2)
        {
            aRepeatMod = 1f;
            bRepeatMod++;
            cRepeatMod = 1f;
            dRepeatMod = 1f;
        }
        else if (n == 3)
        {
            aRepeatMod = 1f;
            bRepeatMod = 1f;
            cRepeatMod++;
            dRepeatMod = 1f;
        }
        else if (n == 4)
        {
            aRepeatMod = 1f;
            bRepeatMod = 1f;
            cRepeatMod = 1f;
            dRepeatMod++;
        }
        else
            print("ERROR, INVALID NEW REPEAT METHOD CALL INPUT");
    }

    public void SetHitboxTriggered(bool t)
    {
        hitboxTriggered = t;
    }

    public void SetExplosionHitboxTriggered(bool t)
    {
        explosionHitboxTriggered = t;
    }

    public void Respawn(Transform t)
    {
        print("respawn called at: " + Time.time);
        if (!torturedS.GetDead())
        {
            cloning = false;
            explodingFromWhole = false;
            if (attachedClone != null && !attachedClone.GetActive())
                clonedSelf = false;
            nma.speed = BASE_SPEED;
            nma.acceleration = BASE_ACCELERATION;
            animationS.SetState(1);
            shaking = false;
            spinning = false;
            locked = false;
            dead = false;
            actuallyDead = false;
            health = MAX_HEALTH;
            nma.Warp(t.position);
            nma.destination = t.position;
            nma.Warp(t.position); // took me a while to figure out that you have to use WARP and not a regular transform.position Vector assignment
            savedPosition = transform.position;
            Activate();
        }
    }

    public void Hit(float dam)
    {
        if (!actuallyDead && !torturedS.GetDead())
        {
            health -= dam;
            semS.SpawnStrikeEffect(transform.position, 1);
            bloodMS.SpawnBlood(transform.position);
            HealthbarHit();
            if (health <= 0)
                ActualDeath();
            else if (!dead)
            {
                if (health <= DEATH_POINT)
                {
                    if (Random.value > 0.75f)
                        ExplodeAttack();
                    else
                        Death();
                }
            }
        }
    }

    public void Hit(float dam, float hitstunMod)
    {
        if (!actuallyDead)
        {
            health -= dam;
            semS.SpawnStrikeEffect(transform.position, 1);
            bloodMS.SpawnBlood(transform.position);
            HealthbarHit();
            if (health <= 0)
                ActualDeath();
            else if (!dead)
            {
                if (health <= DEATH_POINT)
                {
                    if (Random.value > 0.75f)
                        ExplodeAttack();
                    else
                        Death();
                }
            }
        }
    }

    public void BurnHit(float dam)
    {
        if (!actuallyDead && active && !torturedS.GetDead())
        {
            HealthbarHit();
            if (!burnNoisy)
            {
                burnNoisy = true;
                StartCoroutine("EndBurnNoisy");
                if (dam == 0.03f)
                    audioS.PlayFizzClip();
                else
                    audioS.PlayBurnClip();
            }
            health -= dam * BURN_MODIFIER;

            if (health <= 0)
                ActualDeath();
            else if (!dead)
            {
                if (health <= DEATH_POINT)
                {
                    if (Random.value > 0.75f)
                        ExplodeAttack();
                    else
                        Death();
                }
            }
        }
    }

    private void HealthbarHit()
    {
        if (mhS != null)
        {
            if (health <= 0f || (health <= 1f && transform.tag == "cultist")) // second arguement is for special cultist case
            {
                if (mhS.GetEnabled())
                {
                    mhS.DisableHealthbar();
                    mhS = null;
                }
            }
            else
            {
                healthbarTimer = HEALTHBAR_TIME;
            }
        }
        else if (health > 0f)
        {
            healthbarTimer = HEALTHBAR_TIME;
            mhS = mhmS.BeginHealthbar(health / MAX_HEALTH, transform.position);
        }
    }

    void Death()
    {
        print("death called at: " + Time.time);
            countDown = Random.Range(MAX_COUNTDOWN / 2f, MAX_COUNTDOWN);
            rotationLocked = false;
            animationS.noCycling = false;
            if (!deathMuted)
                audioS.PlayBloodExplosion();
            goreMS.SpawnGore(38, transform.position);
            animationS.SetState(4);
            dead = true;
        cooldown = 0f;
        cloning = false;
        timeDied = Time.time;
    }

    void ActualDeath()
    {
        Gore();
        healthbarTimer = 0;
        CheckHealthbar();
        dead = true;
        actuallyDead = true;
        animationS.noCycling = false;
        nma.Warp(new Vector3(150f, transform.position.y, 150f));
        locked = true;
        active = false;
    }

    private void Gore()
    {
        for (int i = 0; i < 5; i++)
            bloodMS.SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y, transform.position.z + Random.Range(-1.5f, 1.5f)));
        if (!deathMuted && !explodeDying && timeDied != Time.time)
            audioS.PlayBloodExplosion();
        goreMS.SpawnGore(37, transform.position); // skips 38
        goreMS.SpawnGore(39, transform.position);
        goreMS.SpawnGore(40, transform.position);
        goreMS.SpawnGore(41, transform.position);
        goreMS.SpawnGore(42, transform.position);
        goreMS.SpawnGore(43, transform.position);
        goreMS.SpawnGore(44, transform.position);
    }

    IEnumerator EndBurnNoisy()
    {
        yield return new WaitForSeconds(1.05f);
        burnNoisy = false;
    }

    private float Get2DDistance(Vector3 t1, Vector3 t2)
    {
        return Mathf.Sqrt(Mathf.Pow(t1.x - t2.x, 2f) + Mathf.Pow(t1.z - t2.z, 2f));
    }

    public void Activate()
    {
        if (Random.value < 0.5f)
            spinRight = true;
        else
            spinRight = false;
        mr.enabled = true;
        explodeDying = false;
        active = true;
        cooldown = Random.Range(0.8f, 1.2f);
        rotationLocked = false;
        CheckDirection();
        countDown = 100f;
    }

    public bool GetActive()
    {
        return active;
    }

    public int GetNum() // get unique killer june int
    {
        return num;
    }

    public bool GetClone()
    {
        return clone;
    }


}

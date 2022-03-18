using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class LemmerScript : MonoBehaviour
{
    const float MAX_HEALTH = 120f;
    const float RECOVERY_TIME = 4f;
    const float BASE_SPEED = 9f;
    float health;
    bool dead;
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
    const float ESCAPE_TIME = 2f;
    float attackCooldown;
    float tsla; // time since last attack
    bool attackState;

    bool active;
    public Image healthbar;
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

    float aRepeatMod;
    float bRepeatMod;
    float cRepeatMod; // make it less likely to have repeated attacks taken

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

    //Unique to LemmerScript
    const float SPIT_COOLDOWN = 1f; // must be more than x2 SPIT_ANIMATION_TIME
    const float CHARGE_COOLDOWN = 1f; // must be more than CHARGE_TIME + 0.1f
    const float CEILING_COOLDOWN = 1f;
    const float SPIT_ANIMATION_TIME = 0.1f;
    const float CHARGE_TIME = 1.5f;
    const float CEILING_TIME = 0.6f;
    const float GRAB_TIME = 3f;
    bool jumpingUp;
    bool jumpingDown;
    const float JUMP_SPEED = 0.1f;
    const float JUMP_HEIGHT = -5f; // z is locally downwards
    float savedHeight;
    Vector3 savedPosition;
    bool grabbing;
    float grabTimer; // for striking the player repeatedly while grabbed

    bool lbd1, lbd2, lbd3, lbd4, lbd5; // lemmer boss dialogue
    
    public GameObject[] escapePoints;
    const float MIN_ESCAPE_DISTANCE = 18f;
    public GameObject hurtbox;
    const float DIVE_STARTUP = 0.3f;
    public TrailRenderer innerTR, outerTR;
    TongueScript tongueS;
    bool diving; // only for determining whether or not to hit the player
    bool crawlEscaping; // for rotation
    bool omniscient;
    bool burnNoisy;

    const float BURN_MODIFIER = 0.5f;

    public ImprovedSpawnScript isS;

    bool knockedOver;
    Vector3 knockedOverVelocity;
    bool actuallyDead;

    GameObject goreM;
    GoreManagerScript goreMS;

    public MeshRenderer whiteFlash;

    void Start()
    {
        whiteFlash.material.color = new Color(255f, 255f, 255f, 0);
        whiteFlash.material.SetColor("_EmissionColor", new Color(0, 0, 0));

        goreM = GameObject.FindGameObjectWithTag("goremanager");
        goreMS = goreM.GetComponent<GoreManagerScript>();

        burnNoisy = false;

        tongueS = outerTR.gameObject.GetComponent<TongueScript>();

        grabbing = false;
        jumpingUp = false;
        jumpingDown = false;


        cooldown = 0;
        rotationLocked = false;

        dialogueNum = 0;

        actuallyEveryCollider = GetComponentsInChildren<Collider>();

        aRepeatMod = 1;
        bRepeatMod = 1;
        cRepeatMod = 1;
        
        mr = GetComponentInChildren<MeshRenderer>();
        savedHeight = mr.transform.localPosition.z;
        savedColor = mr.material.color;

        SetHealthbarEnabled(false);
        savedX = healthbar.rectTransform.localPosition.x;

        active = false;
        attackState = false;

        nma = GetComponent<NavMeshAgent>();
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

        animationS.SetState(3);
        mr.enabled = false;

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
        if (!dead)
        {
            if ((PlayerWithinSight() && !crawlEscaping) || omniscient)
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

                if(Mathf.Sqrt(Mathf.Pow(xDiff, 2f) + Mathf.Pow(zDiff, 2f)) >= 0.001f)
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
        //transform.rotation = Quaternion.Euler(90f,transform.rotation.y, transform.rotation.z);
        playerDistance = Get2DDistance(transform.position, player.transform.position);
        if (active && !dead && !playerS.GetPaused())
        {
            if (crawlEscaping && Vector3.Distance(transform.position, nma.destination) < 1f)
                EndCrawlEscape();

            CheckDialogue();

            CheckJumping();

            if (charging)
            {
                nma.destination = player.transform.position;
                if (hitboxTriggered == true)
                    GrabPlayer();
            }

            if (cooldown <= 0)
            {
                if (!PlayerWithinSight())
                {
                    ApproachPlayer();
                }
                else
                {
                    extraCooldown = 0.8f + (health * 2 / MAX_HEALTH) + Random.value;
                    CheckDirection();
                    rotationLocked = true;
                    if ((Random.value * aRepeatMod) < 0.25f)
                    {
                        cooldown = 100f;
                        NewRepeats(1);
                        DiveAttack();
                    }
                    else if ((Random.value * bRepeatMod) < 0.333f)
                    {
                        cooldown = 100f;
                        NewRepeats(2);
                        ChargeAttack();
                    }
                    else if ((Random.value * cRepeatMod) < 0.5f)
                    {
                        cooldown = 100f;
                        NewRepeats(3);
                        TongueAttack();
                    }
                }
            }
            else
                cooldown -= Time.deltaTime;


            if (!rotationLocked)
                CheckDirection();
            
            // !!!!! PUT IN SOUNDS OF ATTACKS, ADD RAYCAST TO HIT PLAYER FOR LIGHTNING
        }

            transform.rotation = Quaternion.Euler(90f, -rotation, 0);

        if (knockedOver && !actuallyDead)
            nma.velocity = knockedOverVelocity;

    }

    private void ApproachPlayer()
    {
        cooldown = 100f; // prevents cooldown from ending until done attacking
        animationS.SetState(7); // on hands but not crawling
        rotationLocked = false;
        nma.destination = player.transform.position;
        StartCoroutine("Approach");
    }

    IEnumerator Approach()
    {
        while (!PlayerWithinSight())
        {
            nma.destination = player.transform.position;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.3f);
        if (!dead)
        {
            nma.destination = transform.position; // stops tracking player destination
            StartCoroutine("Afterlag");
        }
    }

    private void TongueAttack()
    {
        cooldown = 100f;
        animationS.SetState(11);

        StartCoroutine("TongueStartup");
    }

    IEnumerator TongueStartup()
    {
        yield return new WaitForSeconds(0.8f);

        if (!dead)
        {
            rotationLocked = false;
            CheckDirection();
            rotationLocked = true;

            xDiff = player.transform.position.x - transform.position.x;
            zDiff = player.transform.position.z - transform.position.z;
            rotation = Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff);
            transform.rotation = Quaternion.Euler(90f, -rotation, 0);

            tongueS.Launch();
            animationS.SetState(10);
            StartCoroutine("DelayTongueEmission");
        }
    }

    IEnumerator DelayTongueEmission() // needed since determines on last frame
    {
        yield return new WaitForSeconds(0.01f);
        if (!dead)
        {
            audioS.PlaySpit();
            innerTR.emitting = true;
            outerTR.emitting = true;
        }
    }

    public void SuccessfulTongue()
    {
        print("successful tongue! DDDDDDD");
        rotationLocked = false;
        omniscient = true;
        playerS.SetTongueGrabbed(true);
        audioS.PlayElasticHit();
    }

    public Vector3 GetTonguePosition()
    {
        return tongueS.gameObject.transform.position;
    }

    public void EndTongue() // either triggered by missing or by a PlayerScript call after resisting
    {
        omniscient = false;
        rotationLocked = true;
        tongueS.EndGrab();
        innerTR.emitting = false;
        outerTR.emitting = false;
        innerTR.Clear();
        outerTR.Clear();
        Escape();
    }

    IEnumerator Afterlag()
    {
        animationS.SetState(3);
        yield return new WaitForSeconds(1f);
        if (!dead)
        {
            rotationLocked = false;
            cooldown = 0;
        }
    }

    private void CheckDialogue()
    {
        if (!lbd1 && health / MAX_HEALTH < 0.95f)
        {
            lbd1 = true;
            dialogueS.PrintDialogue("LemmerBoss2");
        }
        else if (!lbd2 && health / MAX_HEALTH < 0.8f)
        {
            lbd2 = true;
            dialogueS.PrintDialogue("LemmerBoss3");
        }
        else if (!lbd3 && health / MAX_HEALTH < 0.65f)
        {
            lbd3 = true;
            dialogueS.PrintDialogue("LemmerBoss4");
        }
        else if (!lbd4 && health / MAX_HEALTH < 0.5f)
        {
            lbd4 = true;
            dialogueS.PrintDialogue("LemmerBoss5");
        }
        else if (!lbd5 && health / MAX_HEALTH < 0.35f)
        {
            lbd5 = true;
            dialogueS.PrintDialogue("LemmerBoss6");
        }
    }

    private void CheckJumping()
    {
        if (jumpingUp)
        {
            if (JUMP_HEIGHT < mr.transform.localPosition.z)
                mr.transform.localPosition = new Vector3(mr.transform.localPosition.x, mr.transform.localPosition.y, mr.transform.localPosition.z - JUMP_SPEED); // local z is in the downwards direction
            else
                jumpingUp = false;
        }
        if (jumpingDown)
        {
            if (savedHeight > mr.transform.localPosition.z)
                mr.transform.localPosition = new Vector3(mr.transform.localPosition.x, mr.transform.localPosition.y, mr.transform.localPosition.z + JUMP_SPEED); // local z is in the downwards direction
            else
            {
                jumpingDown = false;
                EndCeilingEscape();
            }
        }
    }


    private void DiveAttack()
    {
        omniscient = true;
        CheckDirection();
        cooldown = 100f; // makes sure not to break out until finished
        animationS.SetState(4); // Crouch to dive
        nma.acceleration = 0f;
        nma.destination = player.transform.position;
        StartCoroutine("DelayDive");
    }

    IEnumerator DelayDive()
    {
        yield return new WaitForSeconds(DIVE_STARTUP);
        if(!dead)
        {
            audioS.PlayGroundStep();
            animationS.SetState(5); // Diving
            diving = true;
            nma.speed = BASE_SPEED * 4f;
            nma.acceleration = 10000f;
        }
        yield return new WaitForSeconds(0.01f);
        if (!dead)
        {
            CheckDirection();
            omniscient = false;
        }
        yield return new WaitForSeconds(0.01f);
        if(!dead)
            nma.acceleration = 0f; // prevents change in direction after pushing off into a dive
        yield return new WaitForSeconds(0.5f);
        if (!dead)
        {
            diving = false;
            nma.speed = BASE_SPEED;
            nma.acceleration = 100f;
            nma.destination = transform.position;
            nma.velocity = new Vector3(0, 0, 0);
            animationS.SetState(3);
            CheckDirection();
            Escape();
        }
    }

    /*private void SpitAttack()
    {
        cooldown = SPIT_COOLDOWN + extraCooldown;
        rotationLocked = true;
        animationS.SetState(2); // startup spit
        StartCoroutine("ReleaseSpit");
    }

    IEnumerator ReleaseSpit()
    {
        yield return new WaitForSeconds(SPIT_ANIMATION_TIME);
        if (!dead)
            animationS.SetState(3); // release spit
        yield return new WaitForSeconds(SPIT_ANIMATION_TIME);
        if (!dead)
            EndSpit();
    }

    private void EndSpit()
    {
        rotationLocked = false;
        animationS.SetState(1);
    }*/

    private void ChargeAttack()
    {
        cooldown = 100f; // prevents cooldown from ending until done attacking
        animationS.SetState(6); // on hands but not crawling
        StartCoroutine("DelayCharge");
    }

    IEnumerator DelayCharge()
    {
        yield return new WaitForSeconds(0.5f);
        if (!dead)
        {
            animationS.SetState(7); // on hands and is crawling
            charging = true;
            rotationLocked = false;
        }
        yield return new WaitForSeconds(CHARGE_TIME);
        if(charging && !dead)
            EndCharge();
    }

    private void EndCharge()
    {
        charging = false;
        nma.destination = transform.position; // stops tracking player destination
        rotationLocked = false;
        animationS.SetState(3);
        Escape();
    }

    private void GrabPlayer()
    {
        playerS.SetGrabbed(true);
        mr.enabled = false;
        charging = false;
        grabbing = true;
        foreach (Collider c in actuallyEveryCollider)
            c.enabled = false;
        StartCoroutine("EndGrab");
    }

    IEnumerator EndGrab()
    {
        yield return new WaitForSeconds(GRAB_TIME / 4f);
        if(!dead)
            playerS.Hit(0.5f);
        yield return new WaitForSeconds(GRAB_TIME / 4f);
        if(!dead)
            playerS.Hit(0.5f);
        yield return new WaitForSeconds(GRAB_TIME / 4f);
        if(!dead)
            playerS.Hit(0.5f);
        yield return new WaitForSeconds(GRAB_TIME / 4f);
        if (!dead)
        {
            playerS.Hit(0.5f);
            transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            playerS.SetGrabbed(false);
            grabbing = false;
            mr.enabled = true;
            foreach (Collider c in actuallyEveryCollider)
                c.enabled = true;
            Escape();
        }
    }

    private void Escape()
    {
        animationS.SetState(3);
        StartCoroutine("DelayEscape");
    }

    IEnumerator DelayEscape()
    {
        yield return new WaitForSeconds(1f);
        if (!dead)
        {
            if (Random.value < 0.5f)
                StartCoroutine("CeilingEscape");
            else
                StartCoroutine("CrawlEscape");
        }
    }

    IEnumerator CeilingEscape()
    {
        rotationLocked = true;
        animationS.SetState(8); // crouching to jump
        yield return new WaitForSeconds(0.1f);
        if(!dead)
            JumpUpward();
        yield return new WaitForSeconds(0.1f);
        if (!dead)
        {
            foreach (Collider c in actuallyEveryCollider)
                c.enabled = false;
        }
        yield return new WaitForSeconds(0.4f);
        if (!dead)
            CeilingCrawl();
        yield return new WaitForSeconds(ESCAPE_TIME);
        if(!dead)
            JumpDownward();
        //Next point is begun by the end of jumpDownward = false; in CheckJumping()
    }

    IEnumerator CrawlEscape()
    {
        crawlEscaping = true;
        rotationLocked = false;
        animationS.SetState(7);// on hands and is crawling
        nma.destination = GetEscapePoint();
        yield return new WaitForSeconds(ESCAPE_TIME);
        if(crawlEscaping)
            EndCrawlEscape();
    }

    private void EndCrawlEscape()
    {
        crawlEscaping = false;
        nma.destination = transform.position;
        rotationLocked = false;
        StartCoroutine("Afterlag");
    }

    private Vector3 GetEscapePoint()
    {
        GameObject picked = null;
        GameObject tempPicked;
        while(picked == null)
        {
            tempPicked = escapePoints[(int)Random.Range(0, escapePoints.Length)];
            if (Vector3.Distance(tempPicked.transform.position, player.transform.position) > MIN_ESCAPE_DISTANCE)
                picked = tempPicked;
        }
        return new Vector3(picked.transform.position.x, transform.position.y, picked.transform.position.z);
    }

    /*private void CeilingAttack()
    {
        cooldown = CEILING_COOLDOWN + extraCooldown;
        rotationLocked = true;
        animationS.SetState(6); // crouching to jump
        StartCoroutine("DelayJump");
    }*/

    /*IEnumerator DelayJump()
    {
        yield return new WaitForSeconds(0.1f);
        JumpUpward();
        yield return new WaitForSeconds(0.1f);
        CeilingCrawl();
        yield return new WaitForSeconds(CEILING_TIME);
        JumpDownward();
        //Next point is begun by the end of jumpDownward = false; in CheckJumping()
        
    }*/

    private void JumpUpward()
    {
        animationS.SetState(9); // jumping upward
        jumpingUp = true; // activates checkjumping script portion
    }

    private void CeilingCrawl()
    {
        mr.enabled = false;
        nma.destination = GetEscapePoint();
        rotationLocked = false;
    }

    private void JumpDownward()
    {
        mr.enabled = true;
        nma.destination = transform.position; // stops tracking charge destination
        jumpingDown = true; // activates checkjumping script portion
    }

    private void EndCeilingEscape()
    {
        if (!dead)
            mr.enabled = true;
        foreach (Collider c in actuallyEveryCollider)
            c.enabled = true;
        mr.transform.localPosition = new Vector3(mr.transform.localPosition.x, mr.transform.localPosition.y, savedHeight);
        nma.destination = transform.position; // stops tracking player destination
        if (!dead)
            animationS.SetState(3);
        rotationLocked = false;
        StartCoroutine("Afterlag");
    }

    /*private void AttemptGrab()
    {
        if (hitboxTriggered)
        {
            animationS.SetState(8); // Grabbing hold of player
            savedPosition = mr.transform.position;
            grabbing = true;
            mr.enabled = false;
            playerS.SetGrabbed(true);
            StartCoroutine("DelayEndCeiling");
        }
        else
            EndCeiling();
    }

    IEnumerator DelayEndCeiling()
    {
        yield return new WaitForSeconds(GRAB_TIME);
        EndCeiling();
    }

    private void EndCeiling()
    {
        if(!dead)
            mr.enabled = true;
        playerS.SetGrabbed(false);
        grabbing = false;

        mr.transform.localPosition = new Vector3(mr.transform.localPosition.x, mr.transform.localPosition.y, savedHeight);
        nma.destination = transform.position; // stops tracking player destination
        if(!dead)
            animationS.SetState(1);
    }*/

    private void NewRepeats(int n) // increments picked action and resets all others to 0
    {
        if (n == 1)
        {
            aRepeatMod++;
            bRepeatMod = 1f;
            cRepeatMod = 1f;
        }
        else if (n == 2)
        {
            aRepeatMod = 1f;
            bRepeatMod++;
            cRepeatMod = 1f;
        }
        else if (n == 3)
        {
            aRepeatMod = 1f;
            bRepeatMod = 1f;
            cRepeatMod++;
        }
        else
            print("ERROR, INVALID NEW REPEAT METHOD CALL INPUT");
    }

    public void SetHitboxTriggered(bool t)
    {
        hitboxTriggered = t;
        if (diving)
        {
            float tempXDiff = player.transform.position.x - transform.position.x;
            float tempZDiff = player.transform.position.z - transform.position.z;
            float tempDevide = Mathf.Abs(tempXDiff) + Mathf.Abs(tempZDiff);
            playerS.KnockOver(3f * new Vector3(tempXDiff / tempDevide, 0, tempZDiff / tempDevide));
            playerS.Hit(2f);
            diving = false;
        }
    }

    public void Hit(float dam)
    {
        if (!dead && !grabbing)
        {
            health -= dam;
            semS.SpawnStrikeEffect(transform.position, 1);
            bloodMS.SpawnBlood(transform.position);
            RefreshHealthbar();
            if (health <= 0f)
                Death();
        }
        else if (dead && !actuallyDead)
            ActualDeath();
    }

    public void Hit(float dam, float hitstunMod)
    {
        if (!dead && !grabbing)
        {
            health -= dam;
            semS.SpawnStrikeEffect(transform.position, 1);
            bloodMS.SpawnBlood(transform.position);
            RefreshHealthbar();
            if (health <= 0f)
                Death();
        }
        else if (dead && !actuallyDead)
            ActualDeath();
    }

    public void BurnHit(float dam)
    {
        if (!dead && active)
        {
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
            RefreshHealthbar();
            if (health <= 0)
                Death();
        }
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
        transform.position = GetEscapePoint();
        mr.enabled = true;
        SetHealthbarEnabled(true);
        minimap.enabled = false;
        active = true;
        isS.Activate();
    }

    private void SetHealthbarEnabled(bool e)
    {
        healthbar.enabled = e;
        healthbarOuter.enabled = e;
        healthbarInner.enabled = e;
    }

    private void RefreshHealthbar()
    {
        if (health >= 0) // to prevent underflow
        {
            healthbar.transform.localScale = new Vector2(health / MAX_HEALTH, healthbar.transform.localScale.y);
            healthbar.rectTransform.localPosition = new Vector2(savedX - ((50f / MAX_HEALTH) * (MAX_HEALTH - health)), healthbar.rectTransform.localPosition.y);
        }
    }

    private void Death()
    {
        print("death triggered");
        rotation += 180f;
        transform.rotation = Quaternion.Euler(90f, -rotation, 0);
        nma.isStopped = true;
        dead = true;
        audioS.PlayShove();
        animationS.SetState(12);
        KnockOver(0.3f * new Vector3(transform.position.x - player.transform.position.x, 0, transform.position.z - player.transform.position.z));

        StartCoroutine("StopBeingShoved");
    }

    IEnumerator StopBeingShoved()
    {
        yield return new WaitForSeconds(1f);
        if (!actuallyDead)
        {
            dialogueS.PrintDialogue("LemmerBegging1");
            knockedOver = false;
            animationS.SetState(13);
            nma.velocity = new Vector3(0, 0, 0);
        }
    }

    private void ActualDeath()
    {
        if (!actuallyDead)
        {
            audioS.PlayFlash();
            whiteFlash.material.SetColor("_EmissionColor", new Color(255f, 255f, 255f));
            StartCoroutine("TurnOffFlash");
            dialogueS.InterruptDialogue();
            animationS.SetState(14);
            nma.velocity = new Vector3(0, 0, 0);
            actuallyDead = true;
            Gore();

            foreach (Collider c in actuallyEveryCollider)
                c.enabled = false;

            playerS.EndLevel();
        }
    }

    IEnumerator TurnOffFlash()
    {
        yield return new WaitForSeconds(0.05f);
        whiteFlash.material.color = new Color(255f, 255f, 255f, 0);
        whiteFlash.material.SetColor("_EmissionColor", new Color(0, 0, 0));
    }

    private void Gore()
    {
        for (int i = 0; i < 4; i++)
            bloodMS.SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y, transform.position.z + Random.Range(-1.5f, 1.5f)));
        audioS.PlayBloodExplosion();
        for(int j = 0; j < 3; j++)
            for (int i = 114; i < 118; i++)
                goreMS.SpawnGore(i, transform.position + transform.right * 1.2f);
    }

    private void KnockOver(Vector3 vel)
    {
        if (!knockedOver)
        {
            knockedOverVelocity = vel;
            knockedOver = true;
            locked = true;
        }
    }

    public bool GetActuallyDead()
    {
        return actuallyDead;
    }

}

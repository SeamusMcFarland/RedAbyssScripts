using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JacobScript : MonoBehaviour
{

    bool activated;
    bool dead;
    BloodManagerScript bmS;
    GoreManagerScript goreMS;
    StrikeEffectManagerScript semS;
    GameObject player;
    PlayerScript playerS;
    public DialogueScript dialogueS;
    public LightningBlockadeScript lbS;

    public Material neutralM;
    public Material redM;
    public Material darkM;
    public Material lightsM;
    public Material pointM;
    public Material spreadM;
    public Material bleedM;
    public GameObject mrO;
    MeshRenderer mr;

    const float BURST_COOLDOWN = 1.5f; // must be higher than burst startup
    const float FIREBALL_COOLDOWN = 0.5f; // must be higher than hardcoded 0.4 seconds
    const float LIGHTNING_COOLDOWN = 1f; // must be higher than lightning startup and hardcoded 0.1 seconds for lightning renderer remaining enabled. This is not the cooldown between consecutive lightning attacks.
    const float BURST_STARTUP = 2f;
    const float LIGHTNING_STARTUP = 0.16f; // also serves as time in between consecutive lightning attacks if combined with a hardcoded 0.1f delay
    const float BURST_DAMAGE = 4.5f;
    const float LIGHTNING_DAMAGE = 2f;
    const float MAX_HEALTH = 200f;
    const float TELEPORT_COOLDOWN = 1.5f;
    const float BURN_MODIFIER = 0.25f;

    public LightningScript lightningS;
    float lightningDistance;
    RaycastHit[] allHits;
    GameObject chosenWall;

    bool rotationLocked;
    float cooldown;
    float extraCooldown; // reduced as fight goes on, making attacks come out faster and more difficult to avoid as health decreases
    float health;
    GameObject bloodM;
    BloodManagerScript bloodMS;
    public Image healthbar;
    public Image healthbarOuter;
    public Image healthbarInner;
    float savedX;
    public RawImage minimap; // need to switch this off or else it gets in the way

    public FireballScript[] fireballsS;
    int currentFireball;
    bool shaking;
    bool hitboxTriggered;
    public EnergyBallScript energyBallS;
    Vector3 savedPosition;
    int fireballNum;
    int lightningNum; // keeps track of consecutive lightning strikes
    int flashNum; // flash number on teleport
    Color savedColor;

    float bRepeatMod;
    float fRepeatMod;
    float lRepeatMod;
    float tRepeatMod; // make it less likely to have repeated actions taken

    float rotation;
    float xDiff;
    float zDiff;
    Vector3 facingVector; // thing that jacob is currently facing
    float rotationVariation;

    GameObject[] audioM;
    AudioScript audioS;
    const int AUDIO_NUM = 2; //monster audio manager

    public bool isMimic; // marks as original or duplicate Jacob
    bool copiedYet;
    public JacobScript otherJacob;

    int numThreaten; // keeps track of which dialogue threat he's on

    bool burnNoisy;

    // Start is called before the first frame update
    void Start()
    {
        burnNoisy = false;
        goreMS = GameObject.FindGameObjectWithTag("goremanager").GetComponent<GoreManagerScript>();
        numThreaten = 1;
        bmS = GameObject.FindGameObjectWithTag("bloodmanager").GetComponent<BloodManagerScript>();
        semS = GameObject.FindGameObjectWithTag("strikeeffectmanager").GetComponent<StrikeEffectManagerScript>();
        copiedYet = false;
        bRepeatMod = 0;
        fRepeatMod = 0;
        lRepeatMod = 0;
        tRepeatMod = 0;
        savedX = healthbar.rectTransform.localPosition.x;
        fireballNum = 1;
        lightningNum = 1;
        rotationLocked = false;
        mr = mrO.GetComponent<MeshRenderer>();
        shaking = false;
        player = GameObject.FindGameObjectWithTag("player");
        playerS = player.GetComponent<PlayerScript>();
        currentFireball = 0;
        health = MAX_HEALTH;
        bloodM = GameObject.FindGameObjectWithTag("bloodmanager");
        bloodMS = bloodM.GetComponent<BloodManagerScript>();
        dead = false;
        activated = false;
        cooldown = 0f;
        SetToNeutral();
        savedColor = mr.material.color;
        SetHealthbarEnabled(false);
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

    private void SetHealthbarEnabled(bool e)
    {
        healthbar.enabled = e;
        healthbarOuter.enabled = e;
        healthbarInner.enabled = e;
    }

    // Update is called once per frame
    void Update()
    {
        if (activated && !dead && !playerS.GetPaused())
        {
            if(!isMimic)
                CheckThreaten();
            
            if (cooldown <= 0)
            {
                if(!copiedYet)
                    extraCooldown = 0.8f + (health * 2 / MAX_HEALTH) + Random.value;
                else
                    extraCooldown = 1.7f + (health * 2 / MAX_HEALTH) + Random.value;
                rotationLocked = true;
                if (!copiedYet && health < MAX_HEALTH * 0.75f) // checks if it should take the copy action
                {
                    if (isMimic)
                    {
                        copiedYet = true;
                        Teleport();
                    }
                    else
                        CopySelf();
                }
                else
                {
                    if ((Random.value * bRepeatMod) < 0.2f)
                        BurstAttack();
                    else if ((Random.value * fRepeatMod) < 0.25f)
                        FireballAttack();
                    else if ((Random.value * lRepeatMod) < 0.33f)
                        LightningAttack();
                    else if ((Random.value * tRepeatMod) < 0.5f) // is delayed a frame if all action checks are denied
                        Teleport();
                }
            }
            else
                cooldown -= Time.deltaTime;

            
            
            if(!rotationLocked)
                CheckRotation();
            // !!!!! PUT IN SOUNDS OF ATTACKS, ADD RAYCAST TO HIT PLAYER FOR LIGHTNING
        }
        if (shaking)
            transform.position = new Vector3(savedPosition.x + Random.Range(-0.1f, 0.1f), transform.position.y, savedPosition.z + Random.Range(-0.1f, 0.1f));
    }

    private void CheckThreaten()
    {
        if (health / MAX_HEALTH < 0.75f && numThreaten == 1)
        {
            numThreaten++;
            dialogueS.PrintDialogue("JacobMimic");
        }
        else if (health / MAX_HEALTH < 0.5f && numThreaten == 2)
        {
            numThreaten++;
            dialogueS.PrintDialogue("JacobExactMid");
        }
        else if (health / MAX_HEALTH < 0.25f && numThreaten == 3)
        {
            numThreaten++;
            dialogueS.PrintDialogue("JacobMid");
        }

    }

    private void CopySelf()
    {
        copiedYet = true;
        otherJacob.Activate();
        Teleport();
    }

    private void Teleport()
    {
        NewRepeats(4);
        audioS.PlayGlitch();
        flashNum = 0;
        cooldown = TELEPORT_COOLDOWN + extraCooldown;
        StartCoroutine("Flashing");
    }

    IEnumerator Flashing()
    {
        mr.material.color = new Color(Random.Range(0, 255f), Random.Range(0, 255f), Random.Range(0, 255f));
        yield return new WaitForSeconds(0.01f);
        if (flashNum < 5)
        {
            flashNum++;
            StartCoroutine("Flashing");
        }
        else
        {
            rotationLocked = false;
            flashNum = 0;
            mr.material.color = savedColor;
        }
        transform.position = new Vector3(Random.Range(60f, 80f), transform.position.y, Random.Range(-13f, 6f));
    }

    private void BurstAttack()
    {
        NewRepeats(1);
        SetToDark();
        cooldown = BURST_COOLDOWN + extraCooldown;
        StartCoroutine("PrepareBurstStrike");
    }

    IEnumerator PrepareBurstStrike()
    {
        yield return new WaitForSeconds(BURST_STARTUP / 3f);
        if (!dead)
        {
            SetToLights();
            StartCoroutine("BurstStrike");
        }
    }

    IEnumerator BurstStrike()
    {
        yield return new WaitForSeconds(BURST_STARTUP / 6f);
        if (!dead)
        {
            SetToSpread();
            audioS.PlayBurst();
            energyBallS.Burst();
        }
        yield return new WaitForSeconds(BURST_STARTUP / 6f);
        if (!dead)
        {
            if (hitboxTriggered)
                playerS.Hit(BURST_DAMAGE);
            StartCoroutine("WindupBurst1");
        }
    }
    
    IEnumerator WindupBurst1()
    {
        yield return new WaitForSeconds(0.1f);
        if (!dead)
        {
            SetToLights();
            StartCoroutine("WindupBurst2");
        }
    }

    IEnumerator WindupBurst2()
    {
        yield return new WaitForSeconds(0.1f);
        if (!dead)
        {
            SetToNeutral();
        }
        rotationLocked = false;
    }

    private void FireballAttack()
    {
        NewRepeats(2);
        SetToRed();
        StartShake();
        cooldown = FIREBALL_COOLDOWN + extraCooldown;
        fireballsS[currentFireball].DropFireball();
        if (currentFireball < fireballsS.Length - 1) // cycles through fireballs
            currentFireball++;
        else
            currentFireball = 0;
        if (fireballNum < (4 - (int)(health * 2.5f / MAX_HEALTH))) // max of 4, min of 2
        {
            fireballNum++;
            FireballAttack();
        }
        else
        {
            fireballNum = 1;
            StartCoroutine("ResetMaterial", 0.4f);
        }
    }

    private void StartShake()
    {
        shaking = true;
        savedPosition = transform.position;
    }

    private void EndShake()
    {
        shaking = false;
    }

    IEnumerator ResetMaterial(float wait)
    {
        yield return new WaitForSeconds(wait);
        if (!dead)
            SetToNeutral();
        EndShake();
        rotationLocked = false;
    }

    private void LightningAttack()
    {
        CheckRotation(); // redirects toward the player for consecutive lightning attacks
        NewRepeats(3);
        cooldown = LIGHTNING_COOLDOWN + extraCooldown;
        rotationVariation = -2.5f;
        transform.rotation = Quaternion.Euler(90, -rotation - rotationVariation, 0); // Offsets angle randomly
        rotation = rotation + rotationVariation; // sets rotation variable so lightning faces right direction
        SetToPoint();
        StartCoroutine("LightningStrike");
    }

    IEnumerator LightningStrike()
    {
        if(lightningNum == 1)
            yield return new WaitForSeconds(LIGHTNING_STARTUP * 4f); // higher startup for initial lightning attack
        else
            yield return new WaitForSeconds(LIGHTNING_STARTUP);
        //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), transform.TransformDirection(new Vector3(transform.forward.x + 90f, transform.forward.y, transform.forward.z)), Color.blue, 100.0f);
        allHits = Physics.RaycastAll(new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), transform.TransformDirection(new Vector3(transform.forward.x + 90f, transform.forward.y, transform.forward.z)), 100.0f);
        chosenWall = null;
        audioS.PlayZap();
        foreach (RaycastHit hit in allHits)
        {
            if (hit.transform.CompareTag("wall") && (chosenWall == null || Get2DDistance(hit.transform.gameObject) < Get2DDistance(chosenWall)))
                chosenWall = hit.transform.gameObject;
        }
        lightningDistance = Get2DDistance(chosenWall) / 5f;
        lightningS.SummonLightning(lightningDistance, rotation, LIGHTNING_DAMAGE);
        if (lightningNum < 5 - (int)(health * 3f / MAX_HEALTH)) // max of 5, min of 3 strikes (2 strikes if full health)
        {
            lightningNum++;
            StartCoroutine("ConsecutiveLightningDelay");
        }
        else
        {
            lightningNum = 1;
            StartCoroutine("ResetMaterial", 0.3f);
        }
 
    }

    IEnumerator ConsecutiveLightningDelay() // need this or else will rotate while lightning renderer is still enabled
    {
        yield return new WaitForSeconds(0.1f);
        LightningAttack();
    }

    float Get2DDistance(GameObject obj2)
    {
        return Mathf.Pow(Mathf.Pow(transform.position.x - obj2.transform.position.x, 2f) + Mathf.Pow(transform.position.z - obj2.transform.position.z, 2f), 0.5f);
    }

    public void Greeting()
    {
        dialogueS.PrintDialogue("JacobIntro");
        lbS.Blockade();
    }

    public void Activate()
    {
        transform.position = new Vector3(70f, transform.position.y, -4f);
        if (!isMimic)
        {
            minimap.enabled = false;
            SetHealthbarEnabled(true);
        }
        activated = true;
    }

    public void SetHitboxTriggered(bool t)
    {
        hitboxTriggered = t;
    }

    private void SetToNeutral()
    {
        mr.material = neutralM;
        mrO.transform.localScale = new Vector3(1.0f, 1.8f, 1f);
    }

    private void SetToPoint()
    {
        mr.material = pointM;
        mrO.transform.localScale = new Vector3(1.5f, 1.8f, 1f);
    }

    private void SetToRed()
    {
        mr.material = redM;
        mrO.transform.localScale = new Vector3(1.0f, 1.5f, 1f);
    }

    private void SetToDark()
    {
        mr.material = darkM;
        mrO.transform.localScale = new Vector3(1.0f, 1.8f, 1f);
    }

    private void SetToLights()
    {
        mr.material = lightsM;
        mrO.transform.localScale = new Vector3(1.0f, 1.8f, 1f);
    }

    private void SetToSpread()
    {
        mr.material = spreadM;
        mrO.transform.localScale = new Vector3(1.0f, 2.9f, 1f);
    }

    private void SetToBleed()
    {
        mr.material = bleedM;
        mrO.transform.localScale = new Vector3(1.0f, 1.8f, 1f);
    }

    private void CheckRotation()
    {
        facingVector = player.transform.position;
        xDiff = facingVector.x - transform.position.x;
        zDiff = facingVector.z - transform.position.z;
        rotation = Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff);
        transform.rotation = Quaternion.Euler(90, -rotation, 0);
    }

    public void Hit(float dam)
    {
        if (!dead && activated)
        {
            health -= dam;
            semS.SpawnStrikeEffect(transform.position, 1);
            otherJacob.SetHealth(health);
            bloodMS.SpawnBlood(transform.position);
            RefreshHealthbar();
            if (health <= 0f)
                Death();
        }
    }

    public void BurnHit(float dam)
    {
        if (!dead && activated)
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
            otherJacob.SetHealth(health);
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

    private void RefreshHealthbar()
    {
        healthbar.transform.localScale = new Vector2(health / MAX_HEALTH, healthbar.transform.localScale.y);
        healthbar.rectTransform.localPosition = new Vector2(savedX - ((50f / MAX_HEALTH) * (MAX_HEALTH - health)), healthbar.rectTransform.localPosition.y);
    }

    public void SetHealth(float h)
    {
        health = h;
    }

    public void Death()
    {
        if (!dead) // prevents death from running multiple times
        {
            dead = true;
            if (!otherJacob.GetDead()) // kills duplicate jacob if not already dead
                otherJacob.Death();
            StartShake();
            SetToRed();
            if (!isMimic)
            {
                SetHealthbarEnabled(false);
                dialogueS.PrintDialogue("JacobDeath");
            }
            StartCoroutine("ExplodeSelf");
            playerS.EndLevel();
        }
    }

    public bool GetDead()
    {
        return dead;
    }

    IEnumerator ExplodeSelf()
    {
        yield return new WaitForSeconds(0.4f);
        EndShake();
        audioS.PlayBloodExplosion();
        for (int i = 28; i < 37; i++)
            goreMS.SpawnGore(i, transform.position);
        print("exploding position: " + transform.position.x + " " + transform.position.y + " " + transform.position.z);
        mr.enabled = false;
        for (int x = -1; x <= 1; x++) // spawns 3 blood in all 9 position combinations of 1 up, down, right, left
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int i = 0; i < 3; i++)
                    bmS.SpawnBlood(new Vector3 (transform.position.x + x, transform.position.y + y, transform.position.z));
            }
        }
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

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//NEED TO FIX:
//shot wiffs if enemy on top

//Fix someday:
//rare enemy double hit (attack called at the same time so not locked for second?)
public class PlayerScript : MonoBehaviour
{
    public Text endOfDemo;

    public GameObject rendererObject;
    MeshRenderer mr;

    Rigidbody rb;
    float speed;
    float damage;
    float health;
    float maxHealth;
    const float BASE_SPEED = 8f;
    float healingCooldown;
    float hCool;
    float healRate;
    float bandageRate;

    int bandageNum;
    bool bandaging;
    float bandageAnimationTimer;

    public Image healthbar;
    public Image healthbarOuter;
    public Image healthbarInner;
    float savedHealthX;

    const float SPRINT_MODIFIER = 1.6f;
    bool locked;
    bool lockLocked;
    bool cooldown; // similiar to locked except releasing a thrown weapon requires this to be true
    bool dead;
    float newVelX;
    float newVelZ;
    bool wPress, aPress, sPress, dPress, shiftPress, qPress;
    bool mouseLeft;
    float energy;
    const float ENERGY_MAX = 2f;

    float rotation;
    float xDiff;
    float zDiff;
    Plane mousePlane;
    float mouseDistance;
    Vector3 mouseTarget;
    Ray mouseRay;
    List<GameObject> explodedBarrels = new List<GameObject>();
    bool barrelMatch;
    CameraScript cameraS;

    public SparkEffectScript[] sparkES;
    GameObject shotLight;
    Light shotLightL;
    GameObject flashlight;
    Light flashlightL;
    public Light bodyLightL;
    bool lightLocked;

    int stepType; // audio sound, 1 for normal, 2 for mushy, 3 for watery
    bool explodingBarrel;
    bool noisy;
    bool noisyWall, noisyMonster, noisyExplosion, noisyBox, noisyAcid, noisyBreakableWall, noisyMetal, noisyRubble;
    GameObject[] audioM;
    AudioScript audioS;
    AudioScript audioS2; // for fire burning sound effect
    AudioScript audioS3; // for acid burning sound effect
    const int AUDIO_NUM = 1;
    const int AUDIO_NUM2 = 4;
    const int AUDIO_NUM3 = 5;

    BreakableWallScript tempBreakableWallScript; // more efficient by not having to call get component twice

    GameObject bloodM;
    BloodManagerScript bloodMS;
    GoreManagerScript goreMS;
    BulletCasingManagerScript bcmS;
    StrikeEffectManagerScript semS;

    public Material playerDefault;
    public Material[] playerBandage;
    public Material playerReload;
    public Material playerShotgun;
    public Material playerShotgunRecoil;
    public Material playerShotgunReload;
    public Material playerThrow;
    public Material playerPullback;
    public Material playerFoot;
    public Material playerAxe;
    public Material swingAxe;
    public Material raiseAxe;
    public Material playerKnockedOver;
    public Material playerGetUp1;
    public Material playerGetUp2;
    public Material playerGetUp3;
    public Material playerGrabbed;
    public Material playerTongueGrabbed;
    public Material playerResistTongue1;
    public Material playerResistTongue2;
    public Material playerHeadache;

    float throwCD;
    float throwForce;
    const float THROW_COOLDOWN = 3f;
    GameObject thrownBottle;
    ThrownBottleScript thrownBS;
    const float MIN_THROW_FORCE = 5f;
    const float MAX_THROW_FORCE = 20f;
    bool throwingBottle;

    const float TRAP_COOLDOWN = 1f;
    float trapCooldown;

    TrapManagerScript trapMS;

    int bulletCount;
    int shotgunBulletCount;
    int molotovCount;
    int trapCount;
    float gunCooldown;
    float startupLag;
    RaycastHit hit;
    int weaponSelected; // for main weapon. 0 is for axe, 1 is default gun, 2 is shotgun.
    int reloadWeapon; // for reload interrupts
    bool axeReadied;
    bool knockedOver;
    Vector3 knockedOverVelocity;
    bool grabbed;
    bool tongueGrabbed;
    UIManagerScript uimanagerS;
    const bool AXE_ENABLED = false;
    const int BULLET_MAX = 9;
    const int SHOTGUN_BULLET_MAX = 2;
    const int MOLOTOV_MAX = 5;
    const int TRAP_MAX = 3;

    const float RELOAD_COOLDOWN = 1f;
    const float AXE_STARTUP = 0.2f;
    const float AXE_COOLDOWN = 0.5f;
    const float AXE_RANGE = 3f;
    const float AXE_DAMAGE_MOD = 1.34f; // a full hit results in about 4 damage
    const float AXE_HITSTUN_MOD = 2.2f;
    const float DEFAULT_GUN_COOLDOWN = 0.1f;
    const float SHOTGUN_COOLDOWN = 0.6f;
    const float SHOTGUN_SPREAD_MOD = 0.05f;
    const float SHOTGUN_HITSTUN_MOD = 1.8f;

    public Image staminaBar;
    public Image staminaBarOuter;
    public Image staminaBarInner;
    float savedStaminaX;
    bool staminaFilled;

    bool playingWalk;

    public GameObject[] pauseButtons;
    bool paused;

    HelplessJuneScript hjS;
    bool levelCutscene;
    public DialogueScript dialogueS;
    SceneManagerScript smS;
    CutsceneKeislerScript cutsceneKeislerS;
    AudioSource musicSource;

    CronenburgBossScript cbS;
    LemmerScript lemmerS;
    TorturedScript torturedS;
    public KillerJuneSpawnerScript kjsS; // killer june spawner

    float flickerSum; // amount of flicker chance accumilated for this frame

    bool reloading; // handles reload interrupts
    bool reloadInterrupt; //

    bool ld1, ld2, ld3, ld4; // lemmer dialogue skipped if already played

    float grabResist;
    bool wResist, aResist, sResist, dResist;
    float struggleTimer;
    const float STRUGGLE_TIME = 0.5f;
    const float BREAK_FREE_TIME = 2f;
    const float DRAG_SPEED = 2f;
    bool altResist; // switches between resistance materials
    float resistDelay; // minimum delay until another resitance frame can be comitted
    const float RESIST_INPUT_DELAY = 0.1f; //

    Text mashText;

    float frameNormalizer;

    public SplashScript splashS;

    bool alternateStep;

    void Start()
    {
        mashText = GetComponentInChildren<Text>();
        bandageAnimationTimer = 0;
        lockLocked = false;
        knockedOver = false;
        grabbed = false;
        tongueGrabbed = false;
        startupLag = 0;
        axeReadied = false;
        explodingBarrel = false;
        musicSource = GameObject.Find("MusicManager").GetComponent<AudioSource>();
        levelCutscene = false;
        Time.timeScale = 1;
        paused = false;
        cameraS = GameObject.FindWithTag("MainCamera").GetComponent<CameraScript>();
        trapCooldown = 0;
        trapMS = GameObject.FindWithTag("trap").transform.parent.GetComponentInParent<TrapManagerScript>();
        playingWalk = false;
        staminaFilled = false;
        SetStaminaBarEnabled(false);
        savedStaminaX = staminaBar.rectTransform.localPosition.x;
        savedHealthX = healthbar.rectTransform.localPosition.x;
        mr = rendererObject.GetComponent<MeshRenderer>();
        gunCooldown = 0;
        throwingBottle = false;
        hCool = 0;
        weaponSelected = 1;
        bulletCount = BULLET_MAX;
        shotgunBulletCount = SHOTGUN_BULLET_MAX;
        molotovCount = MOLOTOV_MAX;
        trapCount = TRAP_MAX;
        uimanagerS = GameObject.FindGameObjectWithTag("uimanager").GetComponent<UIManagerScript>();
        dead = false;
        throwCD = 0;
        thrownBottle = GameObject.FindGameObjectWithTag("thrownbottle");
        thrownBS = thrownBottle.GetComponent<ThrownBottleScript>();
        throwForce = MIN_THROW_FORCE;
        energy = ENERGY_MAX;
        damage = 1f;
        noisy = false;
        noisyWall = false;
        noisyMonster = false;
        noisyExplosion = false;
        noisyBox = false;
        noisyAcid = false;
        noisyBreakableWall = false;
        shotLight = GameObject.FindGameObjectWithTag("shotlight");
        shotLightL = shotLight.GetComponent<Light>();
        flashlight = GameObject.FindGameObjectWithTag("flashlight");
        flashlightL = flashlight.GetComponent<Light>();
        shotLightL.enabled = false;
        audioM = GameObject.FindGameObjectsWithTag("audiomanager");
        SetupDoubleAudio();
        wPress = false;
        aPress = false;
        sPress = false;
        dPress = false;
        qPress = false;
        rotation = 0;
        newVelX = 0;
        newVelZ = 0;
        locked = true;
        cooldown = false;
        rb = this.GetComponent<Rigidbody>();
        bloodM = GameObject.FindGameObjectWithTag("bloodmanager");
        bloodMS = bloodM.GetComponent<BloodManagerScript>();
        goreMS = GameObject.FindGameObjectWithTag("goremanager").GetComponent<GoreManagerScript>();
        bcmS = GameObject.FindGameObjectWithTag("bulletcasingmanager").GetComponent<BulletCasingManagerScript>();
        semS = GameObject.FindGameObjectWithTag("strikeeffectmanager").GetComponent<StrikeEffectManagerScript>();
        SetHealthbarEnabled(false);
        smS = GameObject.FindWithTag("scenemanager").GetComponent<SceneManagerScript>();
        if (smS.GetNewScene())
            StartCoroutine("IntroDialogue");
        if (smS.GetScene() == 100)
            hjS = GameObject.FindWithTag("helplessjunehurtbox").GetComponentInParent<HelplessJuneScript>();

        if (smS.GetScene() == 2)
            stepType = 2;
        else if (smS.GetScene() == 4)
            stepType = 4;
        else
            stepType = 1;

        if (smS.GetScene() == 4)
        {
            bodyLightL.enabled = false;
            flashlightL.enabled = false;
            lightLocked = true;
        }

        if (smS.GetScene() == 3)
        {
            if (smS.GetProgress() > 0)
            {
                ld1 = true;
                if (smS.GetProgress() > 1)
                {
                    ld2 = true;
                    if (smS.GetProgress() > 2)
                    {
                        ld3 = true;
                        if (smS.GetProgress() > 3)
                        {
                            ld4 = true;
                        }
                    }
                }
            }

            foreach (GameObject o in GameObject.FindGameObjectsWithTag("lemmer"))
            {
                if (o.GetComponent<LemmerScript>() != null)
                    lemmerS = o.GetComponent<LemmerScript>();
            }
        }
        else if (smS.GetScene() == 2)
        {
            foreach (GameObject o in GameObject.FindGameObjectsWithTag("cronenburg"))
            {
                if (o.GetComponent<CronenburgBossScript>() != null)
                    cbS = o.GetComponent<CronenburgBossScript>();
            }
            StartCoroutine("CheckTanksFinished");
        }
        else if (smS.GetScene() == 4)
        {
            foreach (GameObject o in GameObject.FindGameObjectsWithTag("tortured"))
            {
                if (o.GetComponent<TorturedScript>() != null)
                    torturedS = o.GetComponent<TorturedScript>();
            }
        }


        if (smS.GetScene() < 0) // if cutscene
        {
            lightLocked = true;
            cutsceneKeislerS = GameObject.Find("CutsceneKeisler").GetComponent<CutsceneKeislerScript>();
            if (smS.GetScene() == -1 || smS.GetScene() == -2)
            {
                StartCoroutine("DelayWalkForward", 3f);
            }
            else
            {
                SetToNeutral();
                StartCoroutine("DelayCutsceneUntilAbove");
            }
        }
        else
            SetToNeutral();
        UnpauseMenu();
    }

    IEnumerator DelayCutsceneUntilAbove()
    {
        yield return new WaitForSeconds(0.1f);
        if (transform.position.z > -11f)
            StartCoroutine("DelayCutscene", 1f);
        else
            StartCoroutine("DelayCutsceneUntilAbove");
    }

    IEnumerator CheckTanksFinished()
    {
        yield return new WaitForSeconds(0.1f);
        if (smS.CheckTankFinished())
            BeginCronenburgFight();
    }

    private void SetupDoubleAudio() // was once actually only two audio managers
    {
        int searching = 0;
        while (searching != -1)
        {
            if (audioM[searching].GetComponent<AudioScript>().GetAudioNum() == AUDIO_NUM)
            {
                audioS = audioM[searching].GetComponent<AudioScript>();
                searching = -1;
            }
            else
                searching++;
        }
        searching = 0;
        while (searching != -1)
        {
            if (audioM[searching].GetComponent<AudioScript>().GetAudioNum() == AUDIO_NUM2)
            {
                audioS2 = audioM[searching].GetComponent<AudioScript>();
                searching = -1;
            }
            else
                searching++;
        }
        searching = 0;
        while (searching != -1)
        {
            if (audioM[searching].GetComponent<AudioScript>().GetAudioNum() == AUDIO_NUM3)
            {
                audioS3 = audioM[searching].GetComponent<AudioScript>();
                searching = -1;
            }
            else
                searching++;
        }
    }


    IEnumerator DelayWalkForward(float time)
    {
        yield return new WaitForSeconds(time);
        rb.velocity = new Vector3(0, 0, .7f * BASE_SPEED);
        if (!playingWalk) // checks if moving in any direction and if not already playing sound effect so to play walking sound effect
        {
            playingWalk = true;
            if (stepType == 1)
                audioS.PlayPlayerWalk();
            else if (stepType == 2)
                audioS.PlayWetStep();
            else if (stepType == 3)
                audioS.PlayPuddleStep();
            else if (stepType == 4)
                audioS.PlayWaterSplash();
            StartCoroutine("EndWalkSound", 0.7f);

            if (smS.GetScene() == 4)
                splashS.Splash(transform.position);
        }

        if (transform.position.z < -3f)
            StartCoroutine("DelayWalkForward", 0.1f);
        else
        {
            rb.velocity = new Vector3(0, 0, 0);
            StartCoroutine("DelayCutscene", 2f);
        }
    }

    IEnumerator DelayCutscene(float time)
    {
        yield return new WaitForSeconds(time);
        cutsceneKeislerS.BeginDialogue();
    }

    IEnumerator IntroDialogue()
    {
        yield return new WaitForSeconds(0.5f);
        if (smS.GetScene() == 100)
            dialogueS.PrintDialogue("Tutorial");
        else if (smS.GetScene() == 1)
            dialogueS.PrintDialogue("Level1");
        else if (smS.GetScene() == 2)
            dialogueS.PrintDialogue("Level2");
        else if (smS.GetScene() == 3)
            dialogueS.PrintDialogue("Level3");
        else if (smS.GetScene() == 4)
            dialogueS.PrintDialogue("Level4");
    }

    public void SetDifficulty(int difficulty)
    {
        if (difficulty == 1)
        {
            maxHealth = 50f;
            healingCooldown = 0.5f;
            healRate = 0.1f;
            bandageRate = 0.1f;
        }
        else if (difficulty == 2)
        {
            maxHealth = 30f;
            healingCooldown = 1f;
            healRate = 0.03f;
            bandageRate = 0.05f;
        }
        else if (difficulty == 3)
        {
            maxHealth = 20f;
            healingCooldown = 2f;
            healRate = 0.02f;
            bandageRate = 0.04f;
        }
        else if (difficulty == 4)
        {
            maxHealth = 15f;
            healingCooldown = 10f;
            healRate = 0.0f;
            bandageRate = 0.045f;
        }
        else if (difficulty == 5)
        {
            maxHealth = 10f;
            healingCooldown = 10f;
            healRate = 0.0f;
            bandageRate = 0.03f;
        }
        else if (difficulty == 6)
        {
            maxHealth = 7f;
            healingCooldown = 10f;
            healRate = 0.0f;
            bandageRate = 0.02f;
        }
        health = maxHealth;
    }

    public void SetLock(bool b)
    {
        locked = b;
    }

    private void SwitchWeapons(int n) // not used for switching to axe due to inconsistancies
    {
        weaponSelected = n;
        SetToNeutral();
        uimanagerS.SetWeaponType(n);
        if (weaponSelected == 1)
            uimanagerS.SetBulletCount(bulletCount); // refreshes display so to show correct weapon type ammo
        else if (weaponSelected == 2)
            uimanagerS.SetBulletCount(shotgunBulletCount); // refreshes display so to show correct weapon type ammo
        else
            print("ERROR! INVALID WEAPON TYPE in SWITCH WEAPONS");
        if (reloading)
            reloadInterrupt = true;
    }

    private void SetToNeutral()
    {
        if (!grabbed && !tongueGrabbed)
        {
            if (weaponSelected == 0)
                SetToAxe();
            else if (weaponSelected == 1)
                SetToDefaultGun();
            else if (weaponSelected == 2)
                SetToShotgun();
            else
                print("ERROR, INVALID WEAPON TYPE in SETTONEUTRAL");
        }
    }

    private void SetToBandage1()
    {
        mr.material = playerBandage[0];
        rendererObject.transform.localPosition = new Vector3(0, 0, 0);
        mr.transform.localScale = new Vector3(1.2f, 1.6f, 1f);
    }

    private void SetToBandage2()
    {
        mr.material = playerBandage[1];
        rendererObject.transform.localPosition = new Vector3(0, 0, 0);
        mr.transform.localScale = new Vector3(1.4f, 1.7f, 1f);
    }

    private void SetToBandage3()
    {
        mr.material = playerBandage[2];
        rendererObject.transform.localPosition = new Vector3(0, 0, 0);
        mr.transform.localScale = new Vector3(1.4f, 1.9f, 1f);
    }

    private void SetToThrow()
    {
        mr.material = playerThrow;
        rendererObject.transform.localPosition = new Vector3(0, 0, 0);
        mr.transform.localScale = new Vector3(1.1f, 1.9f, 1f);
    }

    private void SetToPullback()
    {
        mr.material = playerPullback;
        rendererObject.transform.localPosition = new Vector3(0, 0, 0);
        mr.transform.localScale = new Vector3(1.2f, 2.0f, 1f);
    }
    private void SetToDefaultGun()
    {
        mr.material = playerDefault;
        rendererObject.transform.localPosition = new Vector3(0, 0, 0);
        mr.transform.localScale = new Vector3(1.7f, 1.5f, 1f);
    }

    private void SetToShotgun()
    {
        mr.material = playerShotgun;
        rendererObject.transform.localPosition = new Vector3(0.35f, 0, 0);
        mr.transform.localScale = new Vector3(2.6f, 1.5f, 1f);
    }

    private void SetToShotgunRecoil()
    {
        mr.material = playerShotgunRecoil;
        rendererObject.transform.localPosition = new Vector3(0.15f, 0, 0);
        mr.transform.localScale = new Vector3(2.3f, 1.5f, 1f);
    }

    private void SetToFoot()
    {
        mr.material = playerFoot;
        rendererObject.transform.localPosition = new Vector3(0, 0, 0);
        mr.transform.localScale = new Vector3(1.2f, 1.9f, 1f);
    }

    public void SetToDefaultReload()
    {
        mr.material = playerReload;
        rendererObject.transform.localPosition = new Vector3(0, 0, 0);
        mr.transform.localScale = new Vector3(1.4f, 1.8f, 1f);
    }

    public void SetToShotgunReload()
    {
        mr.material = playerShotgunReload;
        rendererObject.transform.localPosition = new Vector3(0, 0, 0);
        mr.transform.localScale = new Vector3(1.4f, 1.6f, 1f);
    }

    public void SetToAxe()
    {
        mr.material = playerAxe;
        rendererObject.transform.localPosition = new Vector3(0, 0, 0);
        mr.transform.localScale = new Vector3(1.5f, 1.9f, 1f);
    }

    public void SetToRaiseAxe()
    {
        mr.material = raiseAxe;
        rendererObject.transform.localPosition = new Vector3(-0.20f, 0, 0);
        mr.transform.localScale = new Vector3(2.5f, 2.1f, 1f);
    }

    public void SetToKnockedOver()
    {
        mr.material = playerKnockedOver;
        rendererObject.transform.localPosition = new Vector3(-0.10f, 0, 0);
        mr.transform.localScale = new Vector3(3.9f, 1.7f, 1f);
    }

    public void SetToGetUp1()
    {
        mr.material = playerGetUp1;
        rendererObject.transform.localPosition = new Vector3(-0.10f, 0, 0);
        mr.transform.localScale = new Vector3(3.7f, 1.8f, 1f);
    }

    public void SetToGetUp2()
    {
        mr.material = playerGetUp2;
        rendererObject.transform.localPosition = new Vector3(-0.10f, 0, 0);
        mr.transform.localScale = new Vector3(2.9f, 1.3f, 1f);
    }

    public void SetToGetUp3()
    {
        mr.material = playerGetUp3;
        rendererObject.transform.localPosition = new Vector3(-0.10f, 0, 0);
        mr.transform.localScale = new Vector3(2.0f, 1.7f, 1f);
    }

    public void SetToGrabbed()
    {
        print("set to normal grabbed");
        mr.material = playerGrabbed;
        rendererObject.transform.localPosition = new Vector3(0, 0, 0);
        mr.transform.localScale = new Vector3(1.4f, 2.3f, 1f);
    }

    public void SetToTongueGrabbed()
    {
        mr.material = playerTongueGrabbed;
        rendererObject.transform.localPosition = new Vector3(0, 0, 0);
        mr.transform.localScale = new Vector3(2.9f, 1.6f, 1f);
    }

    public void SetToResistTongue1()
    {
        mr.material = playerResistTongue1;
        rendererObject.transform.localPosition = new Vector3(0, 0, 0);
        mr.transform.localScale = new Vector3(3.1f, 1.6f, 1f);
    }

    public void SetToResistTongue2()
    {
        mr.material = playerResistTongue2;
        rendererObject.transform.localPosition = new Vector3(0, 0, 0);
        mr.transform.localScale = new Vector3(2.9f, 1.9f, 1f);
    }

    public void SetToSwingAxe()
    {
        mr.material = swingAxe;
        audioS.PlaySwoosh();
        rendererObject.transform.localPosition = new Vector3(0.15f, 0, 0);
        mr.transform.localScale = new Vector3(2.3f, 2.1f, 1f);
    }

    private void SetToHeadache()
    {
        mr.material = playerHeadache;
        rendererObject.transform.localPosition = new Vector3(-0.5f, 0, 0);
        mr.transform.localScale = new Vector3(1.2f, 1.9f, 1f);
    }

    void Update()
    {
        CheckMouseLeftClick();
        CheckPause();
        if (!paused)
        {
            if (smS.GetScene() == 3)
                CheckLevel3Dialogue();
            CheckKeys();
            if (dead == false)
                CheckHealing();
            if (locked == false && dead == false && !tongueGrabbed)
            {
                CheckVelocity();
                CheckEnergy();
                if (!grabbed)
                {
                    CheckDirection();
                    CheckAttack();
                }
            }
            CheckGrabbed();

            if (throwCD > 0) // throw cooldown
                throwCD -= Time.deltaTime;
            if (gunCooldown > 0)
                gunCooldown -= Time.deltaTime;
            if (!lightLocked)
                CheckFlicker();

            if (knockedOver)
                rb.velocity = knockedOverVelocity;
        }

        if (mashText != null)
        {
            if (tongueGrabbed)
                mashText.enabled = !mashText.enabled;
            else
                mashText.enabled = false;
        }

        frameNormalizer = Time.deltaTime / 0.014f;
    }

    private void CheckLevel3Dialogue()
    {
        if (!ld1 && smS.GetProgress() == 1)
        {
            ld1 = true;
            dialogueS.PrintDialogue("Lemmer1");
        }
        else if (!ld2 && smS.GetProgress() == 2)
        {
            ld2 = true;
            dialogueS.PrintDialogue("Lemmer2");
        }
        else if (!ld3 && smS.GetProgress() == 3)
        {
            ld3 = true;
            dialogueS.PrintDialogue("Lemmer3");
        }
        else if (!ld4 && smS.GetProgress() == 4)
        {
            ld4 = true;
            dialogueS.PrintDialogue("Lemmer4");
        }
    }

    private void CheckFlicker()
    {
        if (!dead)
        {
            if (flickerSum > 0)
            {
                if (flashlightL.enabled)
                {
                    if (flickerSum / 1.5f < Random.value)
                        flashlightL.enabled = false;
                }
                else
                {
                    if (flickerSum / 1.5f > Random.value)
                        flashlightL.enabled = true;
                }
                flickerSum = 0;
            }
            else
                flashlightL.enabled = true;
        }
    }

    private void CheckMouseLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
            mouseLeft = true;
        if (Input.GetMouseButtonUp(0))
            mouseLeft = false;
    }

    public void JuneCutscene()
    {
        if (!levelCutscene)
        {
            levelCutscene = true;
            locked = true;
            rb.MoveRotation(Quaternion.Euler(90, 270f, 0));
            hjS.Activate();
            StartCoroutine("JuneWalk");
        }
    }

    IEnumerator JuneWalk()
    {
        yield return new WaitForSeconds(0.001f);

        if (!playingWalk)
        {
            playingWalk = true;
            audioS.PlayPlayerWalk();
            StartCoroutine("EndWalkSound", 0.5f);
        }

        speed = BASE_SPEED;
        newVelZ = speed;

        rb.velocity = new Vector3(0, 0, newVelZ);

        if (levelCutscene)
            StartCoroutine("JuneWalk");
    }

    public void EndCutscene()
    {
        levelCutscene = false;
        locked = false;
    }

    private void CheckPause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                PauseMenu();
            }
            else
            {
                UnpauseMenu();
            }
        }
    }

    public void PauseMenu()
    {
        paused = true;
        AudioListener.pause = true;
        Time.timeScale = 0;
        foreach (GameObject p in pauseButtons)
        {
            p.SetActive(true);
            print("revealing button: " + p.name);
        }
    }

    public bool GetPaused()
    {
        return paused;
    }

    public void UnpauseMenu()
    {
        foreach (GameObject b in pauseButtons)
            b.gameObject.SetActive(false);
        paused = false;
        //GameObject[] allAudioO = GameObject.FindGameObjectsWithTag("audiomanager");
        //foreach(GameObject audioO in allAudioO)
        //    audioO.GetComponent<AudioSource>().UnPause();
        AudioListener.pause = false;
        Time.timeScale = 1;
    }

    private void CheckHealing()
    {
        if (hCool < 0)
        {
            if (health < maxHealth - healRate)
            {
                health += healRate;
                RefreshHealthbar();
            }
            else
                SetHealthbarEnabled(false);
        }
        else
        {
            hCool -= Time.deltaTime;
        }
    }

    private void CheckVelocity()
    {
        if ((wPress || aPress || sPress || dPress) && !playingWalk && !qPress) // checks if moving in any direction and if not already playing sound effect so to play walking sound effect
        {
            playingWalk = true;
            if (stepType == 1)
                audioS.PlayPlayerWalk();
            else if (stepType == 2)
                audioS.PlayWetStep();
            else if (stepType == 3)
                audioS.PlayPuddleStep();
            else if (stepType == 4)
                audioS.PlayWaterSplash();
            if (shiftPress)
                StartCoroutine("EndWalkSound", 0.25f);
            else
                StartCoroutine("EndWalkSound", 0.5f);

            if (smS.GetScene() == 4)
            {
                if (alternateStep)
                    splashS.Splash(transform.position + transform.up * 0.2f);
                else
                    splashS.Splash(transform.position - transform.up * 0.2f);
                alternateStep = !alternateStep;
            }
        }

        if (qPress)
            speed = 0;
        else
        {
            if (grabbed)
            {
                if (smS.GetScene() == 4)
                    speed = 0;
                else
                    speed = BASE_SPEED / 3f;
            }
            else
                speed = BASE_SPEED;
            if ((aPress && (wPress || sPress)) || (dPress && (wPress || sPress)))
                speed = speed / Mathf.Sqrt(2);
        }
        if (shiftPress && energy > 0.1f)
        {
            speed *= SPRINT_MODIFIER;
        }
        if (aPress)
            newVelX = -speed;
        else if (dPress)
            newVelX = speed;
        else
            newVelX = 0;
        if (wPress)
            newVelZ = speed;
        else if (sPress)
            newVelZ = -speed;
        else
            newVelZ = 0;

        rb.velocity = new Vector3(newVelX, 0, newVelZ);
    }

    IEnumerator EndWalkSound(float wait) // prevents multiple walk sounds from being played at once
    {
        yield return new WaitForSeconds(wait);
        playingWalk = false;
    }

    private void CheckEnergy()
    {
        if (!staminaFilled)
            RefreshStaminaBar();
        if ((wPress || aPress || sPress || dPress) && !qPress)
        {
            if (shiftPress && (energy > 0))
            {
                energy -= Time.deltaTime;
                SetStaminaBarEnabled(true);
                staminaFilled = false;
            }
            else if (energy < ENERGY_MAX)
                energy += Time.deltaTime / 2;
        }
        else if (energy < ENERGY_MAX)
            energy += Time.deltaTime;
    }

    private void RefreshStaminaBar()
    {
        if (energy >= ENERGY_MAX - 0.01f)
        {
            SetStaminaBarEnabled(false);
            staminaFilled = true;
        }
        else
        {
            staminaBar.transform.localScale = new Vector2(energy / ENERGY_MAX, staminaBar.transform.localScale.y);
            staminaBar.rectTransform.localPosition = new Vector2(savedStaminaX - ((50f / ENERGY_MAX) * (ENERGY_MAX - energy)), staminaBar.rectTransform.localPosition.y);
        }
    }

    private void SetStaminaBarEnabled(bool e)
    {
        staminaBar.enabled = e;
        staminaBarOuter.enabled = e;
        staminaBarInner.enabled = e;
    }

    private void CheckDirection()
    {
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        mousePlane = new Plane(Vector3.up, Vector3.zero);
        if (mousePlane.Raycast(mouseRay, out mouseDistance))
        {
            mouseTarget = mouseRay.GetPoint(mouseDistance);
            xDiff = mouseTarget.x - transform.position.x;
            zDiff = mouseTarget.z - transform.position.z;
            rotation = -(Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff));
            rb.MoveRotation(Quaternion.Euler(90, rotation, 0));
        }
    }

    private void CheckKeys()
    {
        if (Input.GetKeyDown(KeyCode.W))
            wPress = true;
        else if (Input.GetKeyUp(KeyCode.W))
            wPress = false;
        if (Input.GetKeyDown(KeyCode.A))
            aPress = true;
        else if (Input.GetKeyUp(KeyCode.A))
            aPress = false;
        if (Input.GetKeyDown(KeyCode.S))
            sPress = true;
        else if (Input.GetKeyUp(KeyCode.S))
            sPress = false;
        if (Input.GetKeyDown(KeyCode.D))
            dPress = true;
        else if (Input.GetKeyUp(KeyCode.D))
            dPress = false;
        if (Input.GetKeyDown(KeyCode.LeftShift))
            shiftPress = true;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            shiftPress = false;
        if (Input.GetKeyDown(KeyCode.Q))
            qPress = true;
        else if (Input.GetKeyUp(KeyCode.Q))
            qPress = false;
    }

    private void CheckAttack()
    {
        if (Input.GetMouseButtonDown(0) && cooldown == false && gunCooldown <= 0 && !bandaging) // MAIN WEAPON
        {
            if (weaponSelected == 0)
            {
                if (axeReadied == false)
                    ReadyAxe();
            }
            else if (weaponSelected == 1)
                FireDefaultGun();
            else if (weaponSelected == 2)
                FireShotgun();
            else
                print("ERROR! INVALID WEAPON SELECTED in CHECKATTACK");
        }
        else if (Input.GetKeyDown(KeyCode.R) && cooldown == false && weaponSelected != 0 && !bandaging) // RELOAD
        {
            if ((bulletCount < BULLET_MAX && weaponSelected == 1) || (shotgunBulletCount < SHOTGUN_BULLET_MAX && weaponSelected == 2))
                Reload();
        }
        else if (qPress && cooldown == false && gunCooldown <= 0 && !knockedOver) // bandage self
        {
            BandageAnimation();
            RefreshHealthbar();
            if (health < maxHealth)
                health += bandageRate;
        }
        else if (!mouseLeft && axeReadied == true && startupLag <= 0) // when AXE is released and swung
        {
            SwingAxe();
        }
        if (Input.GetMouseButtonDown(1) && throwCD <= 0 && cooldown == false && !bandaging && smS.GetScene() > -1) // BOTTLE THROW
        {
            SetToPullback();
            throwingBottle = true;
            cooldown = true;
        }
        else if (Input.GetMouseButtonUp(1) && cooldown == true && throwCD <= 0) // when BOTTLE is released and thrown
        {
            if (molotovCount > 0) // will still acts as if throwing if no molotovs left, but will not have a molotov to throw.
            {
                molotovCount--;
                uimanagerS.SetMolotovCount(molotovCount);

                thrownBS.Throw(transform.position, rotation, throwForce);
            }
            throwingBottle = false;
            SetToThrow();
            throwCD = THROW_COOLDOWN;
            throwForce = MIN_THROW_FORCE;
            StartCoroutine("EndDelay", 0.2f);
        }
        else if ((Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.E)) && (cooldown == false || reloading)) // SWITCH WEAPONS
        {
            SwitchWeapons();
        }
        else if ((Input.GetKeyDown(KeyCode.T)) && cooldown == false) // ACTIVATE AXE
        {
            if (AXE_ENABLED)
            {
                if (weaponSelected != 0)
                {
                    SwitchToAxe();
                }
                else
                    SwitchWeapons();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space) && cooldown == false && trapCooldown <= 0 && trapCount > 0 && smS.GetScene() > -1) // TRAP PLACEMENT
        {

            trapMS.SpawnTrap(transform.position, rotation);
            audioS.PlayMetalClick();
            trapCount--;
            uimanagerS.SetTrapCount(trapCount);
            cooldown = true;
            locked = true;
            rb.velocity = new Vector3(0, 0, 0);
            SetToFoot();
            StartCoroutine("EndDelay", 0.4f);
            trapCooldown = TRAP_COOLDOWN;
        }

        if (throwingBottle && throwForce < MAX_THROW_FORCE) // builds up force the longer the bottle is held back from being thrown
            throwForce += 0.2f;

        if (trapCooldown > 0)
            trapCooldown -= Time.deltaTime;

        if (startupLag > 0)
            startupLag -= Time.deltaTime;

        if (Input.GetKeyUp(KeyCode.Q) && bandaging)
        {
            bandaging = false;
            SetToNeutral();
        }
    }

    private void BandageAnimation()
    {
        if (bandageAnimationTimer <= 0)
        {
            bandaging = true;
            bandageAnimationTimer = 0.2f;

            if (bandageNum == 1)
            {
                SetToBandage1();
                bandageNum++;
            }
            else if (bandageNum == 2)
            {
                SetToBandage2();
                bandageNum++;
            }
            else if (bandageNum == 3)
            {
                SetToBandage3();
                bandageNum++;
            }
            else
            {
                SetToBandage2();
                bandageNum = 1;
            }
        }
        else
        {
            bandageAnimationTimer -= Time.deltaTime;
        }

    }

    private void SwitchToAxe()
    {
        audioS.PlaySwitchWeapons();
        SetToAxe();
        bandaging = false;
        weaponSelected = 0;
        cooldown = true;
        StartCoroutine("EndDelay", 0.2f);
    }

    private void SwitchWeapons()
    {
        if (weaponSelected == 0)
            SwitchWeapons(1);
        else if (weaponSelected == 1)
            SwitchWeapons(2);
        else if (weaponSelected == 2)
            SwitchWeapons(1);
        else
            print("ERORR! INVALID WEAPON TYPE in mouse input 2");
        audioS.PlaySwitchWeapons();
        cooldown = true;
        StartCoroutine("EndDelay", 0.2f);
    }

    private void FireDefaultGun() // for default gun
    {
        if (bulletCount > 0)
        {
            cameraS.ScreenShake(0.03f);
            gunCooldown = DEFAULT_GUN_COOLDOWN;
            cooldown = true;
            bulletCount--;
            uimanagerS.SetBulletCount(bulletCount);
            Bullet(0);
            bcmS.SpawnBullet(transform.position, 1);
            Gunshot();
            StartCoroutine("EndDelay", gunCooldown);
        }
        else
            Reload();
    }

    private void ReadyAxe()
    {
        startupLag = AXE_STARTUP;
        cooldown = true;
        SetToRaiseAxe();
        axeReadied = true;
    }

    private void SwingAxe()
    {
        Bullet(0);
        Bullet(4f);
        Bullet(-4f);
        gunCooldown = AXE_COOLDOWN;
        axeReadied = false;
        cooldown = false;
        SetToSwingAxe();
        StartCoroutine("EndShotLight"); // ends "noisy" sound variables
        StartCoroutine("EndDelay", AXE_COOLDOWN);
    }

    private void FireShotgun()
    {
        if (shotgunBulletCount > 0)
        {
            if (!grabbed && !tongueGrabbed)
                SetToShotgunRecoil();
            cameraS.ScreenShake(0.1f);
            cooldown = true;
            gunCooldown = SHOTGUN_COOLDOWN;
            StartCoroutine("EndRecoilMaterial");
            shotgunBulletCount--;
            uimanagerS.SetBulletCount(shotgunBulletCount);
            for (float i = -2f; i <= 2f; i++) //fires 5 bullets with i spread
                Bullet(i); // note that angleMod is also used for iterating over spark effects
            bcmS.SpawnBullet(transform.position, 2);
            Gunshot();
            StartCoroutine("EndDelay", gunCooldown);
        }
        else
            Reload();
    }

    IEnumerator EndRecoilMaterial()
    {
        yield return new WaitForSeconds(SHOTGUN_COOLDOWN - 0.01f); // slight decrease hopefully prevents errors of changing materials right when something else is preformed that changes the material
        if (!dead) // otherwise will have missing reference error to renderer component
            SetToNeutral();
    }

    private void Bullet(float angleMod) // handles individual bullets from gunshots
    {
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), transform.right + (Vector3.Cross(transform.right, Vector3.up.normalized) * SHOTGUN_SPREAD_MOD * angleMod), Color.red, 100f);
        //for (float i = -2f; i <= 2f; i++)
        //    Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), transform.right + (Vector3.Cross(transform.right, Vector3.up.normalized) * SHOTGUN_SPREAD_MOD * i), Color.blue, 100f);
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), transform.right + (Vector3.Cross(transform.right, Vector3.up.normalized) * SHOTGUN_SPREAD_MOD * angleMod), out hit))
        {
            if (weaponSelected != 0 || Get2DDistance(transform, hit.transform) < AXE_RANGE || (Get2DDistance(transform, hit.transform) < AXE_RANGE * 2f && hit.transform.CompareTag("breakablewall"))) // for detecting if in range for axe
            {
                if (!hit.transform.CompareTag("player"))
                {
                    if (hit.transform.CompareTag("wall"))
                    {
                        if (weaponSelected != 0)
                            sparkES[(int)(angleMod + 2f)].Spark(hit.point); // angleMod is used for iterator over spark effects
                        else
                            sparkES[(int)((angleMod / 2f) + 2f)].Spark(hit.point);

                        if (noisyWall == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletClang();
                        noisyWall = true;
                    }
                    else if (hit.transform.CompareTag("runnerhurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        if (weaponSelected == 1)
                            hit.collider.gameObject.transform.parent.GetComponent<RunnerScript>().Hit(damage);
                        else if (weaponSelected == 2)
                            hit.collider.gameObject.transform.parent.GetComponent<RunnerScript>().Hit(damage, SHOTGUN_HITSTUN_MOD);
                        else if (weaponSelected == 0)
                            hit.collider.gameObject.transform.parent.GetComponent<RunnerScript>().Hit(damage * AXE_DAMAGE_MOD, AXE_HITSTUN_MOD);
                        else
                            print("INVALID WEAPON TYPE in BULLET");
                    }
                    else if (hit.transform.CompareTag("muscleshurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        if (weaponSelected == 1)
                            hit.collider.gameObject.transform.parent.GetComponent<MusclesScript>().Hit(damage);
                        else if (weaponSelected == 2)
                            hit.collider.gameObject.transform.parent.GetComponent<MusclesScript>().Hit(damage, SHOTGUN_HITSTUN_MOD);
                        else if (weaponSelected == 0)
                            hit.collider.gameObject.transform.parent.GetComponent<MusclesScript>().Hit(damage * AXE_DAMAGE_MOD, AXE_HITSTUN_MOD);
                        else
                            print("INVALID WEAPON TYPE in BULLET");
                    }
                    else if (hit.transform.CompareTag("spitterhurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        if (weaponSelected == 1)
                            hit.collider.gameObject.transform.parent.GetComponent<SpitterScript>().Hit(damage);
                        else if (weaponSelected == 2)
                            hit.collider.gameObject.transform.parent.GetComponent<SpitterScript>().Hit(damage, SHOTGUN_HITSTUN_MOD);
                        else if (weaponSelected == 0)
                            hit.collider.gameObject.transform.parent.GetComponent<SpitterScript>().Hit(damage * AXE_DAMAGE_MOD, AXE_HITSTUN_MOD);
                        else
                            print("INVALID WEAPON TYPE in BULLET");
                    }
                    else if (hit.transform.CompareTag("striderhurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        if (weaponSelected == 1)
                            hit.collider.gameObject.transform.parent.GetComponent<StriderScript>().Hit(damage);
                        else if (weaponSelected == 2)
                            hit.collider.gameObject.transform.parent.GetComponent<StriderScript>().Hit(damage, SHOTGUN_HITSTUN_MOD);
                        else if (weaponSelected == 0)
                            hit.collider.gameObject.transform.parent.GetComponent<StriderScript>().Hit(damage * AXE_DAMAGE_MOD, AXE_HITSTUN_MOD);
                        else
                            print("INVALID WEAPON TYPE in BULLET");
                    }
                    else if (hit.transform.CompareTag("eyehurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        if (weaponSelected == 1)
                            hit.collider.gameObject.transform.parent.GetComponent<EyeScript>().Hit(damage);
                        else if (weaponSelected == 2)
                            hit.collider.gameObject.transform.parent.GetComponent<EyeScript>().Hit(damage, SHOTGUN_HITSTUN_MOD);
                        else if (weaponSelected == 0)
                            hit.collider.gameObject.transform.parent.GetComponent<EyeScript>().Hit(damage * AXE_DAMAGE_MOD, AXE_HITSTUN_MOD);
                        else
                            print("INVALID WEAPON TYPE in BULLET");
                    }
                    else if (hit.transform.CompareTag("zombiehurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        if (weaponSelected == 1)
                            hit.collider.gameObject.transform.parent.GetComponent<ZombieScript>().Hit(damage);
                        else if (weaponSelected == 2)
                            hit.collider.gameObject.transform.parent.GetComponent<ZombieScript>().Hit(damage, SHOTGUN_HITSTUN_MOD);
                        else if (weaponSelected == 0)
                            hit.collider.gameObject.transform.parent.GetComponent<ZombieScript>().Hit(damage * AXE_DAMAGE_MOD, AXE_HITSTUN_MOD);
                        else
                            print("INVALID WEAPON TYPE in BULLET");
                    }
                    else if (hit.transform.CompareTag("smileyhurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        if (weaponSelected == 1)
                            hit.collider.gameObject.transform.parent.GetComponent<SmileyScript>().Hit(damage);
                        else if (weaponSelected == 2)
                            hit.collider.gameObject.transform.parent.GetComponent<SmileyScript>().Hit(damage, SHOTGUN_HITSTUN_MOD);
                        else if (weaponSelected == 0)
                            hit.collider.gameObject.transform.parent.GetComponent<SmileyScript>().Hit(damage * AXE_DAMAGE_MOD, AXE_HITSTUN_MOD);
                        else
                            print("INVALID WEAPON TYPE in BULLET");
                    }
                    else if (hit.transform.CompareTag("suicidebomberhurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        if (weaponSelected == 1)
                            hit.collider.gameObject.transform.parent.GetComponent<SuicideBomberScript>().Hit(damage);
                        else if (weaponSelected == 2)
                            hit.collider.gameObject.transform.parent.GetComponent<SuicideBomberScript>().Hit(damage, SHOTGUN_HITSTUN_MOD);
                        else if (weaponSelected == 0)
                            hit.collider.gameObject.transform.parent.GetComponent<SuicideBomberScript>().Hit(damage * AXE_DAMAGE_MOD, AXE_HITSTUN_MOD);
                        else
                            print("INVALID WEAPON TYPE in BULLET");
                    }
                    else if (hit.transform.CompareTag("cultisthurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        if (weaponSelected == 1)
                            hit.collider.gameObject.transform.parent.GetComponent<CultistScript>().Hit(damage);
                        else if (weaponSelected == 2)
                            hit.collider.gameObject.transform.parent.GetComponent<CultistScript>().Hit(damage, SHOTGUN_HITSTUN_MOD);
                        else if (weaponSelected == 0)
                            hit.collider.gameObject.transform.parent.GetComponent<CultistScript>().Hit(damage * AXE_DAMAGE_MOD, AXE_HITSTUN_MOD);
                        else
                            print("INVALID WEAPON TYPE in BULLET");
                    }
                    else if (hit.transform.CompareTag("posessedhurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        if (weaponSelected == 1)
                            hit.collider.gameObject.transform.parent.GetComponent<PosessedScript>().Hit(damage);
                        else if (weaponSelected == 2)
                            hit.collider.gameObject.transform.parent.GetComponent<PosessedScript>().Hit(damage, SHOTGUN_HITSTUN_MOD);
                        else if (weaponSelected == 0)
                            hit.collider.gameObject.transform.parent.GetComponent<PosessedScript>().Hit(damage * AXE_DAMAGE_MOD, AXE_HITSTUN_MOD);
                        else
                            print("INVALID WEAPON TYPE in BULLET");
                    }
                    else if (hit.transform.CompareTag("plantish"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        audioS.PlaySnap();
                        hit.transform.gameObject.GetComponent<PlantishScript>().DestroyPlantish();
                    }
                    else if (hit.transform.CompareTag("cronenburgtank"))
                    {
                        print("hit tank");
                        if (noisyMetal == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayMetal();
                        noisyMetal = true;
                        hit.transform.parent.GetComponent<CronenburgBossScript>().BurstTank();
                    }
                    else if (hit.transform.CompareTag("jacobhurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        if (weaponSelected != 0)
                            hit.collider.gameObject.transform.parent.GetComponent<JacobScript>().Hit(damage);
                        else
                            hit.collider.gameObject.transform.parent.GetComponent<JacobScript>().Hit(damage * AXE_DAMAGE_MOD);
                    }
                    else if (hit.transform.CompareTag("cronenburghurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        if (hit.collider.gameObject.transform.parent.GetComponent<CronenburgBossScript>() != null)
                        {
                            if (weaponSelected != 0)
                                hit.collider.gameObject.transform.parent.GetComponent<CronenburgBossScript>().Hit(damage);
                            else
                                hit.collider.gameObject.transform.parent.GetComponent<CronenburgBossScript>().Hit(damage * AXE_DAMAGE_MOD);
                        }
                        else
                        {
                            if (weaponSelected != 0)
                                hit.collider.gameObject.transform.parent.GetComponent<CronenburgChaseScript>().Hit(damage);
                            else
                                hit.collider.gameObject.transform.parent.GetComponent<CronenburgChaseScript>().Hit(damage * AXE_DAMAGE_MOD);
                        }
                    }
                    else if (hit.transform.CompareTag("lemmerhurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        hit.collider.gameObject.transform.parent.GetComponent<LemmerScript>().Hit(damage);
                    }
                    else if (hit.transform.CompareTag("torturedhurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        hit.collider.gameObject.transform.parent.GetComponent<TorturedScript>().Hit(damage);
                    }
                    else if (hit.transform.CompareTag("killerjunehurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        hit.collider.gameObject.transform.parent.GetComponent<KillerJuneScript>().Hit(damage);
                    }
                    else if (hit.transform.CompareTag("cronenburgcorpse"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        if (weaponSelected != 0)
                            hit.transform.GetComponent<CronenburgCorpseScript>().Hit(damage);
                        else
                            hit.transform.GetComponent<CronenburgCorpseScript>().Hit(damage * AXE_DAMAGE_MOD);
                    }
                    else if (hit.transform.CompareTag("keisler"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        hit.transform.GetComponent<CutsceneKeislerScript>().Death();
                    }
                    else if (hit.transform.CompareTag("explosivebarrel"))
                    {
                        explodingBarrel = true;
                        StartCoroutine("EndExplodingBarrel");

                        if (noisyWall == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletClang();
                        if (noisyExplosion == false)
                        {
                            for (int i = 0; i < 2; i++) //volume increase
                                audioS.PlayExplosion();
                        }
                        noisyWall = true;
                        noisyExplosion = true;
                        barrelMatch = false;
                        foreach (GameObject e in explodedBarrels) // assures not to blow up the same explosive barrel twice
                        {
                            if (e == hit.transform.gameObject)
                                barrelMatch = true;
                        }
                        if (barrelMatch == false)
                        {
                            explodedBarrels.Add(hit.transform.gameObject);
                            hit.collider.gameObject.GetComponent<ExplosiveBarrelScript>().Explode();
                        }
                    }
                    else if (hit.transform.CompareTag("box"))
                    {
                        if (noisyBox == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBoxBreak();
                        noisyBox = true;
                        hit.collider.gameObject.GetComponent<BoxScript>().BreakBox();
                    }
                    else if (hit.transform.CompareTag("acidcontainer"))
                    {
                        if (noisyAcid == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayShatter();
                        noisyAcid = true;
                        hit.collider.gameObject.GetComponent<AcidContainerScript>().PunctureContainer();
                    }
                    else if (hit.transform.CompareTag("gastank"))
                    {
                        if (noisyAcid == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayGlassBreak2();
                        noisyAcid = true;
                        hit.collider.gameObject.GetComponent<GasTankScript>().BreakGasTank();
                        if (smS.CheckTankFinished())
                            BeginCronenburgFight();
                    }
                    else if (hit.transform.CompareTag("breakablewall"))
                    {
                        tempBreakableWallScript = hit.transform.GetComponent<BreakableWallScript>();
                        if (weaponSelected == 0)
                            tempBreakableWallScript.CompletelyBreakWall();
                        else
                            tempBreakableWallScript.BreakWall();
                        if (tempBreakableWallScript.GetDestroyed() && !noisyBox)
                        {
                            audioS.PlayDestroyWall();
                            noisyBox = true;
                        }
                        if (!noisyBreakableWall)
                        {
                            audioS.PlayDamageWall();
                            noisyBreakableWall = true;
                        }
                    }
                    else if (hit.transform.CompareTag("rubble"))
                    {
                        Vector3 tempVector = new Vector3(hit.transform.position.x - transform.position.x, Random.Range(3f, 5f), hit.transform.position.z - transform.position.z);
                        tempVector = new Vector3(14f * tempVector.x / Get2DDistance(transform, hit.transform), tempVector.y, 14f * tempVector.z / Get2DDistance(transform, hit.transform));
                        hit.transform.GetComponent<RubbleScript>().Struck(tempVector);
                        if (!noisyBreakableWall)
                        {
                            audioS.PlayCrumbleHit();
                            noisyRubble = true;
                        }
                    }
                    else if (hit.transform.CompareTag("helplessjunehurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        audioS.PlayBloodExplosion();
                        hjS.Death();
                    }
                    else if (hit.transform.CompareTag("lecturejunehurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        audioS.PlayBloodExplosion();
                        hit.transform.parent.GetComponent<LectureJuneScript>().Death();
                    }
                    else if (hit.transform.CompareTag("helplesscultisthurtbox"))
                    {
                        if (noisyMonster == false) // prevents multi-shots from causing it to play multiple times at once.
                            audioS.PlayBulletStrike();
                        noisyMonster = true;
                        if (weaponSelected == 1)
                        {
                            if (hit.transform.parent.GetComponent<HelplessCultistScript>() != null)
                                hit.transform.parent.GetComponent<HelplessCultistScript>().Hit(damage);
                            else
                                hit.transform.GetComponent<HelplessCultistScript>().Hit(damage);
                        }
                        else if (weaponSelected == 2)
                        {
                            if (hit.transform.parent.GetComponent<HelplessCultistScript>() != null)
                                hit.transform.parent.GetComponent<HelplessCultistScript>().Hit(damage);
                            else
                                hit.transform.GetComponent<HelplessCultistScript>().Hit(damage);
                        }
                        else if (weaponSelected == 0)
                        {
                            if (hit.transform.parent.GetComponent<HelplessCultistScript>() != null)
                                hit.transform.parent.GetComponent<HelplessCultistScript>().Hit(damage);
                            else
                                hit.transform.GetComponent<HelplessCultistScript>().Hit(damage);
                        }
                        else
                            print("INVALID WEAPON TYPE in BULLET");
                    }
                }
            }
        }
    }

    private void BeginCronenburgFight()
    {
        uimanagerS.ApplyFade();
        lockLocked = true;
        locked = true;
        rb.velocity = new Vector3(0, 0, 0);
        rb.MovePosition(new Vector3(-130f, transform.position.y, -35f));
        audioS.PlayLightSwitch();
        stepType = 1;
        StartCoroutine("CronenburgBegin");
    }

    IEnumerator CronenburgBegin()
    {
        yield return new WaitForSeconds(1f);
        dialogueS.PrintDialogue("CronenburgBeginFight");
        yield return new WaitForSeconds(2f);
        locked = false;
        lockLocked = false;
        audioS.PlayLightSwitch();
        uimanagerS.RemoveFade();
        cbS.Activate();
    }

    private float Get2DDistance(Transform t1, Transform t2)
    {
        return Mathf.Sqrt(Mathf.Pow(t1.position.x - t2.position.x, 2f) + Mathf.Pow(t1.position.z - t2.position.z, 2f));
    }

    IEnumerator EndExplodingBarrel()
    {
        yield return new WaitForSeconds(0.2f);
        explodingBarrel = false;
    }

    private void Reload()
    {
        reloadWeapon = weaponSelected;
        reloading = true;
        audioS.PlayReload();
        if (!grabbed && !tongueGrabbed)
        {
            if (weaponSelected == 1)
            {
                SetToDefaultReload();
            }
            else if (weaponSelected == 2)
            {
                SetToShotgunReload();
            }
            else
                print("ERROR! INVALID WEAPON TYPE in RELOAD");
        }
        cooldown = true;
        StartCoroutine("EndReload"); // slight 0.01f offset hopefully prevents material changes in the same frame
    }

    IEnumerator EndReload()
    {
        yield return new WaitForSeconds(RELOAD_COOLDOWN * 0.7f - 0.01f); // allows some flexability to still get the reload
        if (reloadWeapon == weaponSelected) // makes sure hasn't switched weapons before reloading
        {
            if (weaponSelected == 1)
            {
                bulletCount = BULLET_MAX;
                uimanagerS.SetBulletCount(bulletCount);
            }
            else if (weaponSelected == 2)
            {
                shotgunBulletCount = SHOTGUN_BULLET_MAX;
                uimanagerS.SetBulletCount(shotgunBulletCount);
            }
            else
                print("ERROR! INVALID WEAPON TYPE in ENDRELOAD");
        }
        yield return new WaitForSeconds(RELOAD_COOLDOWN * 0.3f - 0.01f);
        reloading = false;
        if (!reloadInterrupt)
        {
            if (!knockedOver && !dead && !grabbed && !tongueGrabbed)
                SetToNeutral();
            StartCoroutine("EndDelay", 0.01f);
        }
        else
            reloadInterrupt = false;
    }

    private void Freeze(float time)
    {
        locked = true;
        rb.velocity = new Vector3(0, 0, 0);
        StartCoroutine("EndDelay", time);
    }

    IEnumerator EndDelay(float time)
    {
        yield return new WaitForSeconds(time);
        if (!dead)
        {
            if (!knockedOver)
            {
                if (!grabbed && !tongueGrabbed)
                    SetToNeutral();
                if (!lockLocked)
                    locked = false;
            }
            cooldown = false;
        }
    }

    private void Gunshot() // occurs reguardless of what is hit
    {
        if (noisy == false) // prevents multi-shots from causing it to play multiple times at once.
        {
            if (weaponSelected == 1)
                audioS.PlayGunshot();
            else if (weaponSelected == 2)
                audioS.PlayShotgun();
            else
                print("ERROR! INVALID WEAPON TYPE IN GUNSHOT");
        }
        noisy = true;
        shotLightL.enabled = true;
        StartCoroutine("EndShotLight");
    }

    IEnumerator EndShotLight() // turns off light produced by gunshot after 0.05 seconds
    {
        yield return new WaitForSeconds(0.05f);
        noisy = false;
        noisyWall = false;
        noisyMonster = false;
        noisyExplosion = false;
        noisyBox = false;
        noisyAcid = false;
        noisyBreakableWall = false;
        noisyMetal = false;
        noisyRubble = false;
        shotLightL.enabled = false;
    }

    public bool GetNoisy()
    {
        return noisy;
    }

    public float GetHealthRatio()
    {
        return (health / maxHealth);
    }

    public void Hit(float dam)
    {
        if (dead == false)
        {
            semS.SpawnStrikeEffect(transform.position, 1);
            health -= dam;
            hCool = healingCooldown;
            bloodMS.SpawnBlood(transform.position);
            audioS.PlayStrike();
            SetHealthbarEnabled(true);
            RefreshHealthbar();
            if (health <= 0)
            {
                bodyLightL.intensity = 20f;
                Death();
            }
        }
    }

    public void BurnHit(float dam, int burnType) // 1 for fire, 2 for acid
    {
        if (dead == false)
        {
            SetHealthbarEnabled(true);
            RefreshHealthbar();
            health -= dam;
            hCool = healingCooldown;
            if (burnType == 1)
                audioS2.IncreaseVolume();
            else if (burnType == 2)
                audioS3.IncreaseVolume();
            else if (burnType == 3)
            {
                audioS2.IncreaseVolume();
                audioS2.IncreaseVolume();
            }
            SetHealthbarEnabled(true);
            RefreshHealthbar();
            if (health <= 0)
                Death();
        }
    }

    public void KnockOver(Vector3 vel)
    {
        if (dead == false && !knockedOver)
        {
            knockedOverVelocity = vel;
            knockedOver = true;
            bandaging = false;
            locked = true;
            StartCoroutine("GetUp", 0.2f);
        }
    }

    public void SetGrabbed(bool b)
    {
        if (dead == false)
        {
            if (b)
            {
                grabbed = b;
                bandaging = false;
                SetToGrabbed();
            }
            else
            {
                grabbed = b;
                SetToNeutral();
            }
            grabResist = 0;
            rb.velocity = new Vector3(0, 0, 0);
        }
    }

    public void SetTongueGrabbed(bool b)
    {
        if (dead == false)
        {
            print("tongue grabbed!");
            if (b)
            {
                tongueGrabbed = b;
                bandaging = false;
                SetToTongueGrabbed();
            }
            else
            {
                tongueGrabbed = b;
                SetToNeutral();
            }
            grabResist = 0;
            rb.velocity = new Vector3(0, 0, 0);
        }
    }


    private void CheckGrabbed() // grabs can be broken out of by mashing WASD
    {
        if (grabbed)
        {

        }
        if (tongueGrabbed)
        {
            CheckGrabResist();
            FaceAwayFromLemmer();
            rb.velocity = new Vector3(Mathf.Cos(rotation * Mathf.Deg2Rad), 0f, Mathf.Sin(rotation * Mathf.Deg2Rad)) * DRAG_SPEED;
        }
    }

    private void FaceAwayFromLemmer()
    {
        xDiff = lemmerS.transform.position.x - transform.position.x;
        zDiff = lemmerS.transform.position.z - transform.position.z;
        rotation = Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff);
        rb.MoveRotation(Quaternion.Euler(90f, -rotation, 0));
    }

    private void CheckGrabResist()
    {
        struggleTimer -= Time.deltaTime;
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) && resistDelay <= 0)
        {
            resistDelay = RESIST_INPUT_DELAY;
            AlternateResistanceMaterials();
            struggleTimer += STRUGGLE_TIME;
        }

        if (struggleTimer > 0)
        {
            grabResist += Time.deltaTime;
            if (grabResist > BREAK_FREE_TIME)
            {
                tongueGrabbed = false;
                audioS.PlayElasticBreak();
                KnockOver(new Vector3(0, 0, 0));
                lemmerS.EndTongue();
            }
        }

        resistDelay -= Time.deltaTime;
    }

    private void AlternateResistanceMaterials()
    {
        if (altResist)
            SetToResistTongue2();
        else
            SetToResistTongue1();
        altResist = !altResist;
    }

    IEnumerator GetUp(float time)
    {
        SetToKnockedOver();
        yield return new WaitForSeconds(time);
        if (!dead && !grabbed && !tongueGrabbed)
            SetToGetUp1();
        yield return new WaitForSeconds(0.2f);
        if (!dead && !grabbed && !tongueGrabbed)
            SetToGetUp2();
        yield return new WaitForSeconds(0.2f);
        if (!dead && !grabbed && !tongueGrabbed)
            SetToGetUp3();
        yield return new WaitForSeconds(0.2f);
        if (!dead && !grabbed && !tongueGrabbed)
        {
            SetToNeutral();
            knockedOver = false;
            StartCoroutine("EndDelay", 0f);
        }
    }

    private void Death()
    {
        if (!dead && (smS.GetScene() != 3 || !lemmerS.GetActuallyDead()))
        {
            dead = true;
            SetHealthbarEnabled(false);
            uimanagerS.GameOver();
            SetAllToDead();
            flashlightL.enabled = false;
            locked = true;
            rb.velocity = new Vector3(0, 0, 0);
            for (int i = 0; i < 6; i++)
                bloodMS.SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y, transform.position.z + Random.Range(-1.5f, 1.5f)));
            Gore();
            mr.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void SetAllToDead()
    {
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("runnerhurtbox"))
            o.transform.parent.GetComponent<RunnerScript>().PlayerDead();
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("muscleshurtbox"))
            o.transform.parent.GetComponent<MusclesScript>().PlayerDead();
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("spitterhurtbox"))
            o.transform.parent.GetComponent<SpitterScript>().PlayerDead();
    }

    private void Gore()
    {
        audioS.PlayBloodExplosion();
        for (int i = 6; i < 12; i++)
            goreMS.SpawnGore(i, transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "computer")
        {
            if (other.gameObject.GetComponent<ComputerScript>().Access()) // returns true if not already accessed while also preforming the job of opening the door
                audioS.PlayOpenDoor();
        }
    }

    public void CronenburgDowned()
    {
        locked = true;
        lockLocked = true;
        rb.velocity = new Vector3(0, 0, 0);
        musicSource.volume = 0;

        //directs player's sight towards Cronenburg
        Vector3 tempBoss = GameObject.FindGameObjectWithTag("cronenburgtank").transform.parent.transform.position;
        xDiff = tempBoss.x - transform.position.x;
        zDiff = tempBoss.z - transform.position.z;
        rotation = Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff);
        rb.MoveRotation(Quaternion.Euler(90f, -rotation, 0));

        StartCoroutine("AxeCronenburg");
    }

    IEnumerator AxeCronenburg()
    {
        yield return new WaitForSeconds(1f);
        if (!dead)
            SwitchToAxe();
        yield return new WaitForSeconds(2f);
        if (!dead)
            locked = false;
        lockLocked = false;
    }

    public void EndGame()
    {
        lockLocked = true;
        locked = true;
        health = 100000f;
        cameraS.DetatchEndGame();
    }

    public void EndLevel()
    {
        musicSource.volume = 0;
        StartCoroutine("DelayEndLevel");
    }

    IEnumerator DelayEndLevel()
    {
        yield return new WaitForSeconds(3f);
        locked = true;
        audioS.PlayMetalBang();
        yield return new WaitForSeconds(0.4f);
        uimanagerS.OutroScreen();
        yield return new WaitForSeconds(3f);
        if (smS.GetScene() == 100)
            smS.LoadNewScene("Cutscene1");
        else if (smS.GetScene() == 1)
            smS.LoadNewScene("Cutscene2");
        else if (smS.GetScene() == 2)
            smS.LoadNewScene("Cutscene3");
        else if (smS.GetScene() == 3)
            smS.LoadNewScene("Level4");
        else if (smS.GetScene() == 4)
            smS.LoadNewScene("MainMenu");
    }

    public void TeleportToLemmer(float p1, float p2)
    {
        lightLocked = true;
        flashlightL.enabled = false;
        bodyLightL.enabled = false;
        audioS.PlayLightSwitch();
        StartCoroutine("DelayLemmerActivation", new Vector2(p1, p2));
    }

    public void TeleportToTortured()
    {
        rb.MovePosition(new Vector3(71.6f, transform.position.y, 61.6f));
        kjsS.Activate();
        torturedS.Activate();
    }

    IEnumerator DelayLemmerActivation(Vector2 posVec)
    {
        yield return new WaitForSeconds(0.5f);
        rb.MovePosition(new Vector3(posVec.x, transform.position.y, posVec.y));
        yield return new WaitForSeconds(0.01f);
        lightLocked = false;
        flashlightL.enabled = true;
        bodyLightL.enabled = true;
        audioS.PlayLightSwitch();
        yield return new WaitForSeconds(1.5f);
        lemmerS.Activate();
    }

    public void Headache()
    {
        grabbed = true;
        bandaging = false;
        rb.velocity = new Vector3(0, 0, 0);
        SetToHeadache();
        cameraS.ScreenShake(0.1f, 10);
        StartCoroutine("EndHeadache");
    }

    IEnumerator EndHeadache()
    {
        yield return new WaitForSeconds(1f);
        if (!dead)
        {
            grabbed = false;
            SetToNeutral();
        }

    }

    public void BeginLemmerBossDialogue()
    {
        dialogueS.PrintDialogue("LemmerBoss1");
    }

    public void AddFlickerSum(float sum)
    {
        flickerSum += sum;
    }

    public void ForwardPlayBoxBreak() //forwarded since explosivebarrelscript does not have access to an audiomanager
    {
        audioS.PlayBoxBreak();
    }

    public void ForwardPlayExplosion2()
    {
        audioS.PlayExplosion2();
    }

    public void ForwardPlayFlash()
    {
        audioS.PlayFlash();
    }

    private void SetHealthbarEnabled(bool e)
    {
        healthbar.enabled = e;
        healthbarOuter.enabled = e;
        healthbarInner.enabled = e;
    }
    private void RefreshHealthbar()
    {
        healthbar.transform.localScale = new Vector2(health / maxHealth, healthbar.transform.localScale.y);
        healthbar.rectTransform.localPosition = new Vector2(savedHealthX - ((50f / maxHealth) * (maxHealth - health)), healthbar.rectTransform.localPosition.y);
    }

    public void SetMolotovCount(int num)
    {
        audioS.PlayBottlePickup();
        molotovCount = num;
    }

    public int GetMolitovCount()
    {
        return molotovCount;
    }

    public void SetTrapCount(int num)
    {
        audioS.PlayTrapPickup();
        trapCount = num;
    }

    public int GetTrapCount()
    {
        return trapCount;
    }

    public float GetRotation()
    {
        return rotation;
    }

    public bool GetPlayerDead()
    {
        return dead;
    }

    public bool GetExplodingBarrel()
    {
        return explodingBarrel;
    }


}




    /*private void CheckDirection() //OLD CODE FOR FACING DIRECTION OF KEY INPUT
    {
        if (aPress)
        {
            if (wPress)
                rotation = 225f;
            else if (sPress)
                rotation = 135f;
            else
                rotation = 180f;
        }
        else if (dPress)
        {
            if (wPress)
                rotation = 315f;
            else if (sPress)
                rotation = 45f;
            else
                rotation = 0;
        }
        else if (wPress)
            rotation = 270f;
        else if (sPress)
            rotation = 90f;
        transform.rotation = Quaternion.Euler(90, rotation, 0);
    }
    */

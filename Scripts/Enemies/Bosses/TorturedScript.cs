using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TorturedScript : MonoBehaviour
{
    float rotation;
    PlayerScript playerS;

    bool dead;
    bool active;
    float health;
    const float MAX_HEALTH = 200f;
    StrikeEffectManagerScript semS;

    const float BURN_MODIFIER = 0.2f;

    bool burnNoisy;
    GameObject[] audioM;
    AudioScript audioS;
    const int AUDIO_NUM = 2; // monster audio manager

    public Image healthbar;
    public Image healthbarOuter;
    public Image healthbarInner;
    float savedX;

    public RawImage minimap; // need to switch this off or else it gets in the way

    GameObject bloodM;
    BloodManagerScript bloodMS;

    public KillerJuneScript[] allKJS;

    AnimationScript animS;
    bool freshAnimation;

    float animationTimer;
    float endTime;

    FlashSliceManagerScript fsmS;
    SceneManagerScript smS;
    float attackDelay;

    const float LIGHT_FADE_RATE = 10f;
    Light flashL;

    bool dialogue1, dialogue2, dialogue3, dialogue4;
    public DialogueScript dialogueS;

    public Text[] allDeathText;

    List<AudioSource> allAudioSources = new List<AudioSource>();

    UIManagerScript uimanagerS;

    public AudioSource musicSource;
    public AudioClip endSong;

    public DeadPlayerScript deadPlayerS;

    public HypnoticScript hypnoticS;

    // Start is called before the first frame update
    void Start()
    {
        uimanagerS = GameObject.FindGameObjectWithTag("uimanager").GetComponent<UIManagerScript>();

        flashL = GameObject.FindGameObjectWithTag("flashsliceflash").GetComponent<Light>();
        smS = GameObject.FindGameObjectWithTag("scenemanager").GetComponent<SceneManagerScript>();
        fsmS = GameObject.FindGameObjectWithTag("flashslicemanager").GetComponent<FlashSliceManagerScript>();
        animS = GetComponentInChildren<AnimationScript>();

        health = MAX_HEALTH;
        rotation = 0;
        playerS = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerScript>();
        semS = GameObject.FindGameObjectWithTag("strikeeffectmanager").GetComponent<StrikeEffectManagerScript>();
        SetHealthbarEnabled(false);
        savedX = healthbar.rectTransform.localPosition.x;
        bloodM = GameObject.FindGameObjectWithTag("bloodmanager");
        bloodMS = bloodM.GetComponent<BloodManagerScript>();

        SetupAudio();

        foreach(GameObject am in audioM)
        allAudioSources.Add(am.GetComponent<AudioSource>());
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
        if (!playerS.GetPaused() && active)
        {
            rotation -= Time.deltaTime / 2f;
            transform.rotation = Quaternion.Euler(0, rotation, 0);
            animationTimer -= Time.deltaTime;
            if (animationTimer < 0)
            {
                if (!freshAnimation)
                {
                    endTime = Random.Range(-4f, -1f);
                    freshAnimation = true;
                    animS.SetState((int)Random.Range(2f, 5.999f));
                }
                else if (animationTimer < endTime)
                {
                    animS.SetState(1);
                    freshAnimation = false;
                    animationTimer = Random.Range(1f, 3f);
                }
            }

            flashL.intensity -= LIGHT_FADE_RATE * Time.deltaTime;
            CheckDialogue();
        }
    }

    private void CheckDialogue()
    {
        if (!dialogue1 && health / MAX_HEALTH < 0.85f)
        {
            dialogue1 = true;
            dialogueS.PrintDialogue("Tortured1");
        }
        else if (!dialogue2 && health / MAX_HEALTH < 0.65f)
        {
            dialogue2 = true;
            dialogueS.PrintDialogue("Tortured2");
        }
        else if (!dialogue3 && health / MAX_HEALTH < 0.45f)
        {
            dialogue3 = true;
            dialogueS.PrintDialogue("Tortured3");
        }
        else if (!dialogue4 && health / MAX_HEALTH < 0.25f)
        {
            dialogue4 = true;
            dialogueS.PrintDialogue("Tortured4");
        }
    }

    public void Activate()
    {
        allKJS[0].Activate();
        allKJS[1].Activate();
        print("activating");
        SetHealthbarEnabled(true);
        minimap.enabled = false;
        active = true;
        StartCoroutine("AttackLoop");
    }

    IEnumerator AttackLoop()
    {
        attackDelay = (6f - smS.GetDifficulty() / 2f) * Random.Range(0.8f, 1.2f);
        yield return new WaitForSeconds(attackDelay);
        if (!dead)
        {
            if (Random.value < 0.5f)
                StartCoroutine("FlashSliceAttack");
            else
                StartCoroutine("HypnoticAttack");
        }
    }

    IEnumerator FlashSliceAttack()
    {
        int loopTimes = (int)(3f - ((health - 0.001f) / MAX_HEALTH) * 2f);
        if (smS.GetDifficulty() > 4)
            loopTimes++;
        for (int i = 0; i < loopTimes;  i++) //
        {
            if(!dead)
                fsmS.FlashSlice(new Vector3(Random.Range(67.5f, 74.5f), 0, Random.Range(65f, 72f)));
            yield return new WaitForSeconds(Random.Range(0.6f, 0.8f));
        }
        yield return new WaitForSeconds(0.01f); // backup since coroutines don't like not having a return
        StartCoroutine("AttackLoop");
    }

    IEnumerator HypnoticAttack()
    {
        hypnoticS.Burst();
        yield return new WaitForSeconds(0.5f);
        audioS.PlayBriefEarRing();
        yield return new WaitForSeconds(0.5f);
        playerS.Headache();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("AttackLoop");
    }

    private void SetHealthbarEnabled(bool e)
    {
        print("enabling healthbar: " + e);
        healthbar.enabled = e;
        healthbarOuter.enabled = e;
        healthbarInner.enabled = e;
    }

    public void Hit(float dam)
    {
        if (!dead && active)
        {
            health -= dam;
            semS.SpawnStrikeEffect(transform.position, 1);
            bloodMS.SpawnBlood(transform.position);
            RefreshHealthbar();
            if (health <= 0f)
                Death();
        }
    }

    public void Hit(float dam, float hitstunMod)
    {
        if (!dead && active)
        {
            health -= dam;
            semS.SpawnStrikeEffect(transform.position, 1);
            bloodMS.SpawnBlood(transform.position);
            RefreshHealthbar();
            if (health <= 0f)
                Death();
        }
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
        foreach (AudioSource auSo in allAudioSources)
            auSo.Stop();
        dead = true;
        uimanagerS.ApplyFade();
        musicSource.clip = endSong;
        musicSource.Play();
        StartCoroutine("Words", 4);
        playerS.EndGame();
        SetHealthbarEnabled(false);
    }

    IEnumerator Words(int num)
    {
        yield return new WaitForSeconds(1.5f);
        switch (num)
        {
            case 1:
                allDeathText[3].enabled = true;
                uimanagerS.RemoveFade();
                deadPlayerS.GetUp();
                break;
            case 2:
                allDeathText[2].enabled = true;
                StartCoroutine("Words", 1);
                break;
            case 3:
                allDeathText[1].enabled = true;
                StartCoroutine("Words", 2);
                break;
            case 4:
                allDeathText[0].enabled = true;
                StartCoroutine("Words", 3);
                break;
        }
    }

    public bool GetDead()
    {
        return dead;
    }

}

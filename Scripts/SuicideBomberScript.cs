using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideBomberScript : MonsterScript
{
    //not explosive stuff
    public Light theLight;
    bool blinking;
    bool initialAggro;
    const float EXPLOSIVE_DISTANCE = 2f;
     
    //explosive stuff
    const float BASE_DAMAGE = 5f; // concurrent with radius
    const float EXTRA_DAMAGE = 2f;
    float barrelDamage; // varies inversely with distance from barrel
    float barrelDistance;
    bool exploding;
    bool boxBroken;
    const int RAYCAST_DIVISIONS = 20;

    List<GameObject> targets2 = new List<GameObject>();
    GameObject[] screens;
    List<ScreenScript> screensS = new List<ScreenScript>();
    CameraScript cameraS;

    RaycastHit hit2;
    bool validHit2;
    GameObject hitObject2;

    public Light explosionLight;
    Vector3 savPos;

    public override void assignTraits()
    {
        //not explosive stuff
        theLight.enabled = false;
        blinking = false;
        initialAggro = false;

        //explosive stuff
        cameraS = GameObject.FindWithTag("MainCamera").GetComponent<CameraScript>();
        screens = GameObject.FindGameObjectsWithTag("screen");
        foreach (GameObject o in screens)
            screensS.Add(o.transform.GetChild(0).GetComponent<ScreenScript>());
        boxBroken = false;
        exploding = false;
        explosionLight.enabled = false;

        SetHealth(4f);
        SetDamage(2f);
        SetAfterlag(0.10f);
        SetStartup(0f);
        SetBaseSpeed(8.5f);
        SetSightDistance(10f);
        SetLightSensitivity(1.3f);
        SetHearingDistance(9f);
        SetHitstun(0.2f);
        SetStrikeDistance(1f);
        SetVisionRange(20f);
        SetPackDistance(1f);
    }

    public override void Gore()
    {
        Explode();
        for (int i = 0; i < 3; i++)
            GetBloodMS().SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y, transform.position.z + Random.Range(-1.5f, 1.5f)));
        for (int i = 0; i < 5; i++)
            GetBloodMS().SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y, transform.position.z + Random.Range(-1.5f, 1.5f)), 2);
        if (!GetDeathMuted())
            GetAudioS().PlayBloodExplosion();
        for (int i = 87; i < 96; i++)
            GetGoreMS().SpawnGore(i, transform.position);
    }

    public override void AddedMethods()
    {
        if (Get2DDistance(GetPlayer().transform, transform) < EXPLOSIVE_DISTANCE)
            Hit(10f); // will always kill self

        if (!initialAggro)
        {
            if (GetAggro())
                initialAggro = true;
        }
        else
        {
            if (!GetPlayerSeen())
            {
                DetectedPlayer();
                Aggroed();
            }
        }

        if (!blinking && GetAggro())
        {
            blinking = true;
            StartCoroutine("Blink");
        }
    }

    IEnumerator Blink() // blinks red light until death
    {
        yield return new WaitForSeconds(0.1f);
        if(!GetDead())
            theLight.enabled = true;
        yield return new WaitForSeconds(0.1f);
        theLight.enabled = false;
        if (!GetDead())
            StartCoroutine("Blink");
    }

    List<GameObject> tempList = new List<GameObject>();

    public override void PackReaction()
    {
        tempList = GetSameNearby();
        foreach (GameObject o in tempList)
        {
            o.transform.parent.GetComponent<SuicideBomberScript>().DetectedPlayer();
            o.transform.parent.GetComponent<SuicideBomberScript>().Aggroed();
        }
    }

    public void Explode()
    {
        if (exploding == false) // second backup to prevent multiple triggerings at once
        {
            if (!GetDeathMuted())
                GetPlayerScript().ForwardPlayExplosion2(); ;
            exploding = true;
            explosionLight.transform.position = new Vector3(transform.position.x, explosionLight.transform.position.y, transform.position.z);
            explosionLight.enabled = true;

            if(!GetDeathMuted())
                cameraS.ScreenShake(1f, 2);
            BreakScreens();
            if (!GetDeathMuted())
                CheckTargets();

            foreach (GameObject t in targets2)
            {
                barrelDistance = Mathf.Pow(Mathf.Pow(t.transform.position.x - transform.position.x, 2) + Mathf.Pow(t.transform.position.y - transform.position.y, 2) + Mathf.Pow(t.transform.position.z - transform.position.z, 2), 0.5f);
                barrelDamage = BASE_DAMAGE - barrelDistance;
                barrelDamage *= 2f;
                if (barrelDamage > 0)
                {
                    barrelDamage += EXTRA_DAMAGE;
                    if (t.CompareTag("runnerhurtbox"))
                    {
                        t.transform.parent.GetComponent<RunnerScript>().Hit(barrelDamage);
                    }
                    else if (t.CompareTag("muscleshurtbox"))
                    {
                        t.transform.parent.GetComponent<MusclesScript>().Hit(barrelDamage);
                    }
                    else if (t.CompareTag("spitterhurtbox"))
                    {
                        t.transform.parent.GetComponent<SpitterScript>().Hit(barrelDamage);
                    }
                    else if (t.CompareTag("striderhurtbox"))
                    {
                        t.transform.parent.GetComponent<StriderScript>().Hit(barrelDamage);
                    }
                    else if (t.CompareTag("zombiehurtbox"))
                    {
                        t.transform.parent.GetComponent<ZombieScript>().Hit(barrelDamage);
                    }
                    else if (t.CompareTag("eyehurtbox"))
                    {
                        t.transform.parent.GetComponent<EyeScript>().Hit(barrelDamage);
                    }
                    else if (t.CompareTag("smileyhurtbox"))
                    {
                        t.transform.parent.GetComponent<SmileyScript>().Hit(barrelDamage);
                    }
                    else if (t.CompareTag("suicidebomberhurtbox"))
                    {
                        t.transform.parent.GetComponent<SuicideBomberScript>().Hit(barrelDamage);
                    }
                    else if (t.CompareTag("cultisthurtbox"))
                    {
                        t.transform.parent.GetComponent<CultistScript>().Hit(barrelDamage);
                    }
                    else if (t.CompareTag("posessedhurtbox"))
                    {
                        t.transform.parent.GetComponent<PosessedScript>().Hit(barrelDamage);
                    }
                    else if (t.CompareTag("lemmerhurtbox"))
                    {
                        t.transform.parent.GetComponent<LemmerScript>().Hit(barrelDamage);
                    }
                    else if (t.CompareTag("rubble"))
                    {
                        Vector3 tempVector = new Vector3(t.transform.position.x - transform.position.x, Random.Range(3f, 5f), t.transform.position.z - transform.position.z);
                        tempVector = new Vector3(25f * tempVector.x / Get2DDistance(transform, t.transform), tempVector.y, 25f * tempVector.z / Get2DDistance(transform, t.transform));
                        t.transform.GetComponent<RubbleScript>().Struck(tempVector);
                    }
                    else if (t.CompareTag("cronenburghurtbox"))
                    {
                        if (t.transform.parent.GetComponent<CronenburgBossScript>() != null)
                            t.transform.parent.GetComponent<CronenburgBossScript>().Hit(barrelDamage);
                        else
                            t.transform.parent.GetComponent<CronenburgChaseScript>().Hit(barrelDamage);
                    }
                    else if (t.CompareTag("plantish"))
                    {
                        t.transform.GetComponent<PlantishScript>().DestroyPlantish();
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
                            GetPlayerScript().ForwardPlayBoxBreak();
                        }
                        t.transform.GetComponent<BoxScript>().BreakBox();
                    }
                    else if (t.CompareTag("breakablewall"))
                    {
                        if (!boxBroken) //forwarded since explosivebarrelscript does not have access to an audiomanager, also only wanting to only have one sound played hence it being here instead of on the boxscript
                        {
                            boxBroken = true;
                            GetPlayerScript().ForwardPlayBoxBreak();
                        }
                        t.transform.GetComponent<BreakableWallScript>().CompletelyBreakWall();
                    }
                    else if (t.CompareTag("player"))
                    {
                        GetPlayerScript().Hit(barrelDamage);
                    }
                }
            }

            StartCoroutine("Dispose");
        }

    }

    IEnumerator Dispose()
    {
        savPos = transform.position;
        yield return new WaitForSeconds(0.01f);
        explosionLight.transform.position = new Vector3(savPos.x, explosionLight.transform.position.y, savPos.z); // needs to be moved back since body is brought away
        yield return new WaitForSeconds(0.05f);
        explosionLight.enabled = false;
        exploding = false;
    }

    private void CheckTargets()
    {
        targets2.Clear();

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
        Physics.Raycast(new Vector3(transform.position.x, 0.5f, transform.position.z), new Vector3(x, 0, z), out hit2);
        hitObject2 = hit2.transform.gameObject;
        validHit2 = true;
        foreach (GameObject t in targets2)
            if (t == hitObject2)
                validHit2 = false;
        if (validHit2)
            targets2.Add(hitObject2);
    }

    private void BreakScreens()
    {
        for (int i = 0; i < screens.Length; i++)
        {
            if (Physics.Raycast(transform.position, screens[i].transform.position - transform.position, out hit2))
            {
                Debug.DrawRay(transform.position, screens[i].transform.position - transform.position, Color.green, 100f);
                if (hit2.transform.CompareTag("screen"))
                    screensS[i].BreakScreen();
            }
        }
    }

}

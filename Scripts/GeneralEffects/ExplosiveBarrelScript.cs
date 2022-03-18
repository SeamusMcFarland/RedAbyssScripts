using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrelScript : MonoBehaviour
{
    const float BASE_DAMAGE = 8f; // concurrent with radius
    const float EXTRA_DAMAGE = 2f;
    float barrelDamage; // varies inversely with distance from barrel
    float barrelDistance;
    public BarrelPiecesScript[] pieces;
    bool exploding;
    bool boxBroken;
    const int RAYCAST_DIVISIONS = 20;

    GameObject player;
    PlayerScript playerS;
    List<GameObject> targets = new List<GameObject>();
    GameObject[] screens;
    List<ScreenScript> screensS = new List<ScreenScript>();
    CameraScript cameraS;

    RaycastHit hit;
    bool validHit;
    GameObject hitObject;

    void Start()
    {
        cameraS = GameObject.FindWithTag("MainCamera").GetComponent<CameraScript>();
        screens = GameObject.FindGameObjectsWithTag("screen");
        foreach (GameObject o in screens)
            screensS.Add(o.transform.GetChild(0).GetComponent<ScreenScript>());
        boxBroken = false;
        exploding = false;
        player = GameObject.FindGameObjectWithTag("player");
        playerS = player.GetComponent<PlayerScript>();
        GetComponent<Light>().enabled = false;
    }

    public void Explode()
    {
        if (exploding == false) // second backup to prevent multiple triggerings at once
        {
            exploding = true;
            GetComponent<Light>().enabled = true;

            cameraS.ScreenShake(1f, 2);
            BreakScreens();
            CheckTargets();

            foreach (GameObject t in targets)
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
                    else if (t.CompareTag("rubble"))
                    {
                        Vector3 tempVector = new Vector3(t.transform.position.x - transform.position.x, Random.Range(3f, 5f), t.transform.position.z - transform.position.z);
                        tempVector = new Vector3(25f * tempVector.x / Get2DDistance(transform, t.transform), tempVector.y, 25f * tempVector.z / Get2DDistance(transform, t.transform));
                        t.transform.GetComponent<RubbleScript>().Struck(tempVector);
                    }
                    else if (t.CompareTag("cronenburghurtbox"))
                    {
                        if (t.transform.parent.name == "CronenburgBoss")
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
                            playerS.ForwardPlayBoxBreak();
                        }
                        t.transform.GetComponent<BoxScript>().BreakBox();
                    }
                    else if (t.CompareTag("breakablewall"))
                    {
                        if (!boxBroken) //forwarded since explosivebarrelscript does not have access to an audiomanager, also only wanting to only have one sound played hence it being here instead of on the boxscript
                        {
                            boxBroken = true;
                            playerS.ForwardPlayBoxBreak();
                        }
                        t.transform.GetComponent<BreakableWallScript>().CompletelyBreakWall();
                    }
                    else if (t.CompareTag("player"))
                    {
                        playerS.Hit(barrelDamage);
                    }
                }
            }
            

            //
            foreach (BarrelPiecesScript p in pieces)
                p.ReleaseScrap(transform.position);
            StartCoroutine("Dispose");
        }
    }

    IEnumerator Dispose()
    {
        yield return new WaitForSeconds(0.05f);
        GetComponent<Light>().enabled = false;
        transform.position = new Vector3(300f, 300f, 300f);
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }

    public bool GetExploding()
    {
        return exploding;
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
        Physics.Raycast(new Vector3(transform.position.x, 0.5f, transform.position.z), new Vector3(x, 0, z), out hit);
        hitObject = hit.transform.gameObject;
        validHit = true;
        foreach (GameObject t in targets)
            if (t == hitObject)
                validHit = false;
        if(validHit)
            targets.Add(hitObject);
    }

    private void BreakScreens()
    {
        for (int i = 0; i < screens.Length; i++)
        {
            if (Physics.Raycast(transform.position, screens[i].transform.position - transform.position, out hit))
            {
                Debug.DrawRay(transform.position, screens[i].transform.position - transform.position, Color.green, 100f);
                if(hit.transform.CompareTag("screen"))
                    screensS[i].BreakScreen();
            }
        }
    }

    private float Get2DDistance(Transform t1, Transform t2)
    {
        return Mathf.Sqrt(Mathf.Pow(t1.position.x - t2.position.x, 2f) + Mathf.Pow(t1.position.z - t2.position.z, 2f));
    }

    /*private void OnTriggerEnter(Collider other)
    {
        sameObject = false; //
        foreach (GameObject o in targets) //
            if (other.gameObject == o) //
                sameObject = true; //these lines assure not adding same target
        if (!sameObject)
        {
            if (other.gameObject.CompareTag("player")) // split for readability
                targets.Add(other.gameObject);
            else if (other.gameObject.CompareTag("runnerhurtbox") || other.gameObject.CompareTag("muscleshurtbox") || other.gameObject.CompareTag("spitterhurtbox"))
                targets.Add(other.gameObject);
            else if(other.gameObject.CompareTag("explosivebarrel") || other.gameObject.CompareTag("box") || other.gameObject.CompareTag("screen"))
                targets.Add(other.gameObject);
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
    }*/

}



//Old code finding all strikable objects and testing distance between them to see if they are hit.


/*
            foreach (GameObject r in GameObject.FindGameObjectsWithTag("runnerhurtbox"))
            {
                barrelDistance = Mathf.Pow(Mathf.Pow(r.transform.position.x - transform.position.x, 2) + Mathf.Pow(r.transform.position.y - transform.position.y, 2) + Mathf.Pow(r.transform.position.z - transform.position.z, 2), 0.5f);
                barrelDamage = BASE_DAMAGE - (int)barrelDistance;
                if (barrelDamage > 0)
                    r.transform.parent.GetComponent<RunnerScript>().Hit(barrelDamage);
            }
            foreach (GameObject m in GameObject.FindGameObjectsWithTag("muscleshurtbox"))
            {
                barrelDistance = Mathf.Pow(Mathf.Pow(m.transform.position.x - transform.position.x, 2) + Mathf.Pow(m.transform.position.y - transform.position.y, 2) + Mathf.Pow(m.transform.position.z - transform.position.z, 2), 0.5f);
                barrelDamage = BASE_DAMAGE - (int)barrelDistance;
                if (barrelDamage > 0)
                    m.transform.parent.GetComponent<MusclesScript>().Hit(barrelDamage);
            }
            foreach (GameObject s in GameObject.FindGameObjectsWithTag("spitterhurtbox"))
            {
                barrelDistance = Mathf.Pow(Mathf.Pow(s.transform.position.x - transform.position.x, 2) + Mathf.Pow(s.transform.position.y - transform.position.y, 2) + Mathf.Pow(s.transform.position.z - transform.position.z, 2), 0.5f);
                barrelDamage = BASE_DAMAGE - (int)barrelDistance;
                if (barrelDamage > 0)
                    s.transform.parent.GetComponent<SpitterScript>().Hit(barrelDamage);
            }
            foreach (GameObject b in GameObject.FindGameObjectsWithTag("explosivebarrel"))
            {
                if (b.name == this.name)
                    break;
                barrelDistance = Mathf.Pow(Mathf.Pow(b.transform.position.x - transform.position.x, 2) + Mathf.Pow(b.transform.position.y - transform.position.y, 2) + Mathf.Pow(b.transform.position.z - transform.position.z, 2), 0.5f);
                barrelDamage = BASE_DAMAGE - (int)barrelDistance;
                if (barrelDamage > 0)
                    b.transform.GetComponent<ExplosiveBarrelScript>().Explode();
            }
            foreach (GameObject b in GameObject.FindGameObjectsWithTag("box"))
            {
                if (!boxBroken) //forwarded since explosivebarrelscript does not have access to an audiomanager, also only wanting to only have one sound played hence it being here instead of on the boxscript
                {
                    boxBroken = true;
                    player.GetComponent<PlayerScript>().ForwardPlayBoxBreak();
                }
                barrelDistance = Mathf.Pow(Mathf.Pow(b.transform.position.x - transform.position.x, 2) + Mathf.Pow(b.transform.position.y - transform.position.y, 2) + Mathf.Pow(b.transform.position.z - transform.position.z, 2), 0.5f);
                barrelDamage = BASE_DAMAGE - (int)barrelDistance;
                if (barrelDamage > 0)
                    b.transform.GetComponent<BoxScript>().BreakBox();
            }
            */

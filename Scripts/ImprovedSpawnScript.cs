using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImprovedSpawnScript : MonoBehaviour
{
    List<Transform> spawnPos = new List<Transform>();
    GameObject player;

    public float maxTime;

    public bool runnerE, musclesE, spitterE, striderE, eyeE, zombieE, smileyE, suicideBomberE, cultistE, posessedE, killerJuneE; // enabled or disabled
    GameObject[] runnerO;
    GameObject[] musclesO;
    GameObject[] spitterO;
    GameObject[] striderO;
    GameObject[] eyeO;
    GameObject[] zombieO;
    GameObject[] smileyO;
    GameObject[] suicideBomberO;
    GameObject[] cultistO;
    GameObject[] posessedO;
    GameObject[] killerJuneO;
    List<RunnerScript> runnerS = new List<RunnerScript>();
    List<MusclesScript> musclesS = new List<MusclesScript>();
    List<SpitterScript> spitterS = new List<SpitterScript>();
    List<StriderScript> striderS = new List<StriderScript>();
    List<EyeScript> eyeS = new List<EyeScript>();
    List<ZombieScript> zombieS = new List<ZombieScript>();
    List<SmileyScript> smileyS = new List<SmileyScript>();
    List<SuicideBomberScript> suicideBomberS = new List<SuicideBomberScript>();
    List<CultistScript> cultistS = new List<CultistScript>();
    List<PosessedScript> posessedS = new List<PosessedScript>();
    List<KillerJuneScript> killerJuneS = new List<KillerJuneScript>();
    RunnerScript foundRS;
    MusclesScript foundMS;
    SpitterScript foundSpS;
    StriderScript foundStS;
    EyeScript foundES;
    ZombieScript foundZS;
    SmileyScript foundSmS;
    SuicideBomberScript foundSBS;
    CultistScript foundCS;
    PosessedScript foundPS;
    KillerJuneScript foundKJS;

    bool[] foundType = new bool[11];
    public int[] preferenceType; // sets preference for those lowest in this array
    bool foundPreference;
    bool isFound;
    int randInt;
    bool foundLocation;

    Transform availableLocation;
    List<Transform> tempList = new List<Transform>();

    public bool noShuffle;

    public float minDistance; // minimum distance away from the player to spawn

    public bool active; // can use this to call at start or have "Activate()" called

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        Transform[] tempSL = GetComponentsInChildren<Transform>();
        foreach (Transform t in tempSL) // prevents adding parent object as a spawn point
        {
            if(t.name != name)
                spawnPos.Add(t);
        }

        runnerO = GameObject.FindGameObjectsWithTag("runner");
        musclesO = GameObject.FindGameObjectsWithTag("muscles");
        spitterO = GameObject.FindGameObjectsWithTag("spitter");
        striderO = GameObject.FindGameObjectsWithTag("strider");
        eyeO = GameObject.FindGameObjectsWithTag("eye");
        zombieO = GameObject.FindGameObjectsWithTag("zombie");
        smileyO = GameObject.FindGameObjectsWithTag("smiley");
        suicideBomberO = GameObject.FindGameObjectsWithTag("suicidebomber");
        cultistO = GameObject.FindGameObjectsWithTag("cultist");
        posessedO = GameObject.FindGameObjectsWithTag("posessed");
        killerJuneO = GameObject.FindGameObjectsWithTag("killerjune");

        foreach (GameObject o in runnerO)
            runnerS.Add(o.GetComponent<RunnerScript>());
        foreach (GameObject o in musclesO)
            musclesS.Add(o.GetComponent<MusclesScript>());
        foreach (GameObject o in spitterO)
            spitterS.Add(o.GetComponent<SpitterScript>());
        foreach (GameObject o in striderO)
            striderS.Add(o.GetComponent<StriderScript>());
        foreach (GameObject o in eyeO)
            eyeS.Add(o.GetComponent<EyeScript>());
        foreach (GameObject o in zombieO)
            zombieS.Add(o.GetComponent<ZombieScript>());
        foreach (GameObject o in smileyO)
            smileyS.Add(o.GetComponent<SmileyScript>());
        foreach (GameObject o in suicideBomberO)
            suicideBomberS.Add(o.GetComponent<SuicideBomberScript>());
        foreach (GameObject o in cultistO)
            cultistS.Add(o.GetComponent<CultistScript>());
        foreach (GameObject o in posessedO)
            posessedS.Add(o.GetComponent<PosessedScript>());
        foreach (GameObject o in killerJuneO)
            killerJuneS.Add(o.GetComponent<KillerJuneScript>());

        if (active)
            StartCoroutine("Spawn");
    }

    public void Activate()
    {
        if (!active) // makes sure not to activate twice
        {
            active = true;
            StartCoroutine("Spawn");
        }
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(Random.Range(maxTime/1.5f, maxTime));
        if (active)
        {
            FindInactive();

            if(!noShuffle)
                ShuffleSpawnPoints();

            FindAvailableLocation();


            if (isFound)
            {
                for (int i = 0; i < preferenceType.Length; i++)
                {
                    if (((float)i + 1f) / preferenceType.Length > Random.value)
                    {
                        CheckPreference(preferenceType[i]);
                        if (foundPreference)
                            break;
                    }
                }
                if (!foundPreference)
                    CheckAvailable();
            }
            StartCoroutine("Spawn");
        }
    }

    private void CheckPreference(int pType)
    {
        foundPreference = false;
        switch (pType)
        {
            case 1:
                if (foundType[0])
                {
                    foundRS.Respawn(availableLocation);
                    foundPreference = true;
                }
                break;
            case 2:
                if (foundType[1])
                {
                    foundMS.Respawn(availableLocation);
                    foundPreference = true;
                }
                break;
            case 3:
                if (foundType[2])
                {
                    foundSpS.Respawn(availableLocation);
                    foundPreference = true;
                }
                break;
            case 4:
                if (foundType[3])
                {
                    foundStS.Respawn(availableLocation);
                    foundPreference = true;
                }
                break;
            case 5:
                if (foundType[4])
                {
                    foundES.Respawn(availableLocation);
                    foundPreference = true;
                }
                break;
            case 6:
                if (foundType[5])
                {
                    foundZS.Respawn(availableLocation);
                    foundPreference = true;
                }
                break;
            case 7:
                if (foundType[6])
                {
                    foundSmS.Respawn(availableLocation);
                    foundPreference = true;
                }
                break;
            case 8:
                if (foundType[7])
                {
                    foundSBS.Respawn(availableLocation);
                    foundPreference = true;
                }
                break;
            case 9:
                if (foundType[8])
                {
                    foundCS.Respawn(availableLocation);
                    foundPreference = true;
                }
                break;
            case 10:
                if (foundType[9])
                {
                    foundPS.Respawn(availableLocation);
                    foundPreference = true;
                }
                break;
            case 11:
                if (foundType[10])
                {
                    foundKJS.Respawn(spawnPos[foundKJS.GetNum()]); // unique to prevent overlap
                    foundPreference = true;
                }
                break;
        }
    }

    private void CheckAvailable()
    {
        if (foundType[0])
            foundRS.Respawn(availableLocation);
        else if (foundType[1])
            foundMS.Respawn(availableLocation);
        else if (foundType[2])
            foundSpS.Respawn(availableLocation);
        else if (foundType[3])
            foundStS.Respawn(availableLocation);
        else if (foundType[4])
            foundES.Respawn(availableLocation);
        else if (foundType[5])
            foundZS.Respawn(availableLocation);
        else if (foundType[6])
            foundSmS.Respawn(availableLocation);
        else if (foundType[7])
            foundSBS.Respawn(availableLocation);
        else if (foundType[8])
            foundCS.Respawn(availableLocation);
        else if (foundType[9])
            foundPS.Respawn(availableLocation);
        else if (foundType[10])
            foundKJS.Respawn(spawnPos[foundKJS.GetNum()]); // unique to prevent overlap
    }

    private void FindAvailableLocation()
    {
        foundLocation = false;
        foreach (Transform t in spawnPos)
        {
            if (!PlayerWithinSight(t.position) && Get2DDistance(t, player.transform) > minDistance)
            {
                availableLocation = t;
                foundLocation = true;
                break;
            }

        }
        if (!foundLocation)
            print("ERROR! FAILED TO FIND AVAILABLE SPAWN POINT");
    }

    private bool PlayerWithinSight(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos, (player.transform.position - pos), out hit))
        {
            if (hit.transform.CompareTag("player"))
                return true;
            else
                return false;
        }
        else
            return false;
    }

    private void FindInactive()
    {
        isFound = false;
        if (runnerE)
        {
            foreach (RunnerScript s in runnerS)
            {
                if (!s.GetActive())
                {
                    foundType[0] = true;
                    foundRS = s;
                    isFound = true;
                    break;
                }
            }
        }
        if (musclesE)
        {
            foreach (MusclesScript s in musclesS)
            {
                if (!s.GetActive())
                {
                    foundType[1] = true;
                    foundMS = s;
                    isFound = true;
                    break;
                }
            }
        }
        if (spitterE)
        {
            foreach (SpitterScript s in spitterS)
            {
                if (!s.GetActive())
                {
                    foundType[2] = true;
                    foundSpS = s;
                    isFound = true;
                    break;
                }
            }
        }
        if (striderE)
        {
            foreach (StriderScript s in striderS)
            {
                if (!s.GetActive())
                {
                    foundType[3] = true;
                    foundStS = s;
                    isFound = true;
                    break;
                }
            }
        }
        if (eyeE)
        {
            foreach (EyeScript s in eyeS)
            {
                if (!s.GetActive())
                {
                    foundType[4] = true;
                    foundES = s;
                    isFound = true;
                    break;
                }
            }
        }
        if (zombieE)
        {
            foreach (ZombieScript s in zombieS)
            {
                if (!s.GetActive())
                {
                    foundType[5] = true;
                    foundZS = s;
                    isFound = true;
                    break;
                }
            }
        }
        if (smileyE)
        {
            foreach (SmileyScript s in smileyS)
            {
                if (!s.GetActive())
                {
                    foundType[6] = true;
                    foundSmS = s;
                    isFound = true;
                    break;
                }
            }
        }
        if (suicideBomberE)
        {
            foreach (SuicideBomberScript s in suicideBomberS)
            {
                if (!s.GetActive())
                {
                    foundType[7] = true;
                    foundSBS = s;
                    isFound = true;
                    break;
                }
            }
        }
        if (cultistE)
        {
            foreach (CultistScript s in cultistS)
            {
                if (!s.GetActive())
                {
                    foundType[8] = true;
                    foundCS = s;
                    isFound = true;
                    break;
                }
            }
        }
        if (posessedE)
        {
            foreach (PosessedScript s in posessedS)
            {
                if (!s.GetActive())
                {
                    foundType[9] = true;
                    foundPS = s;
                    isFound = true;
                    break;
                }
            }
        }
        if (killerJuneE)
        {
            foreach (KillerJuneScript s in killerJuneS)
            {
                if (!s.GetActive() && !s.GetClone())
                {
                    foundType[10] = true;
                    foundKJS = s;
                    isFound = true;
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ShuffleSpawnPoints()
    {
        foreach (Transform t in spawnPos)
            tempList.Add(t);
        for (int i = 0; i < spawnPos.Count; i++)
        {
            randInt = (int)Random.Range(0, tempList.Count);
            spawnPos[i] = tempList[randInt];
            tempList.RemoveAt(randInt);
        }
    }

    public float Get2DDistance(Transform t1, Transform t2)
    {
        return Mathf.Sqrt(Mathf.Pow(t1.position.x - t2.position.x, 2f) + Mathf.Pow(t1.position.z - t2.position.z, 2f));
    }
}

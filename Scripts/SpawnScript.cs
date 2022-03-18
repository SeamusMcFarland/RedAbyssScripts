using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{

    Transform[] spawnPos;
    GameObject player;

    GameObject[] runnerO;
    GameObject[] musclesO;
    GameObject[] spitterO;
    GameObject[] striderO;
    GameObject[] eyeO;
    GameObject[] zombieO;
    List<RunnerScript> runnerS = new List<RunnerScript>();
    List<MusclesScript> musclesS = new List<MusclesScript>();
    List<SpitterScript> spitterS = new List<SpitterScript>();
    List<StriderScript> striderS = new List<StriderScript>();
    List<EyeScript> eyeS = new List<EyeScript>();
    List<ZombieScript> zombieS = new List<ZombieScript>();
    RunnerScript foundRS;
    MusclesScript foundMS;
    SpitterScript foundSpS;
    StriderScript foundStS;
    EyeScript foundES;
    ZombieScript foundZS;

    int foundType;
    bool isFound;
    int randInt;
    bool foundLocation;

    Transform availableLocation;
    List<Transform> tempList = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        spawnPos = GetComponentsInChildren<Transform>();
        
        runnerO = GameObject.FindGameObjectsWithTag("runner");
        musclesO = GameObject.FindGameObjectsWithTag("muscles");
        spitterO = GameObject.FindGameObjectsWithTag("spitter");
        striderO = GameObject.FindGameObjectsWithTag("strider");
        eyeO = GameObject.FindGameObjectsWithTag("eye");
        zombieO = GameObject.FindGameObjectsWithTag("zombie");

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

        StartCoroutine("Spawn");
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(Random.Range(5f, 10f));
        print("ready");
        yield return new WaitForSeconds(1f);

        FindInactive();

        ShuffleSpawnPoints();

        FindAvailableLocation();

        if (isFound)
        {
            print("found spawnable: " + foundType);
            if (foundType == 1)
                foundRS.Respawn(availableLocation);
            else if (foundType == 2)
                foundMS.Respawn(availableLocation);
            else if (foundType == 3)
                foundSpS.Respawn(availableLocation);
            else if (foundType == 4)
                foundStS.Respawn(availableLocation);
            else if (foundType == 5)
                foundES.Respawn(availableLocation);
            else if (foundType == 6)
                foundZS.Respawn(availableLocation);
        }
        StartCoroutine("Spawn");
    }

    private void FindAvailableLocation()
    {
        foundLocation = false;
        foreach (Transform t in spawnPos)
        {
            if (Get2DDistance(t, player.transform) > 20f)
            {
                availableLocation = t;
                foundLocation = true;
                break;
            }
            
        }
        if(!foundLocation)
            print("ERROR! FAILED TO FIND AVAILABLE SPAWN POINT");
    }

    private void FindInactive()
    {
        isFound = false;
        if (!isFound)
        {
            foreach (RunnerScript s in runnerS)
            {
                if (!s.GetActive())
                {
                    foundType = 1;
                    foundRS = s;
                    isFound = true;
                    break;
                }
            }
        }
        if (!isFound)
        {
            foreach (MusclesScript s in musclesS)
            {
                if (!s.GetActive())
                {
                    foundType = 2;
                    foundMS = s;
                    isFound = true;
                    break;
                }
            }
        }
        if (!isFound)
        {
            foreach (SpitterScript s in spitterS)
            {
                if (!s.GetActive())
                {
                    foundType = 3;
                    foundSpS = s;
                    isFound = true;
                    break;
                }
            }
        }
        if (!isFound)
        {
            foreach (StriderScript s in striderS)
            {
                if (!s.GetActive())
                {
                    foundType = 4;
                    foundStS = s;
                    isFound = true;
                    break;
                }
            }
        }
        if (!isFound)
        {
            foreach (EyeScript s in eyeS)
            {
                if (!s.GetActive())
                {
                    foundType = 5;
                    foundES = s;
                    isFound = true;
                    break;
                }
            }
        }
        if (!isFound)
        {
            foreach (ZombieScript s in zombieS)
            {
                if (!s.GetActive())
                {
                    foundType = 6;
                    foundZS = s;
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
        for (int i = 0; i < spawnPos.Length; i++)
        {
            randInt = (int)Random.Range(0, tempList.Count);
            spawnPos[i] = tempList[randInt];
            tempList.RemoveAt(randInt);
        }
        foreach (Transform t in spawnPos)
            print("all shuffled: " + t);
    }

    public float Get2DDistance(Transform t1, Transform t2)
    {
        print("distance: " + Mathf.Sqrt(Mathf.Pow(t1.position.x - t2.position.x, 2f) + Mathf.Pow(t1.position.z - t2.position.z, 2f)));
        return Mathf.Sqrt(Mathf.Pow(t1.position.x - t2.position.x, 2f) + Mathf.Pow(t1.position.z - t2.position.z, 2f));
    }
}

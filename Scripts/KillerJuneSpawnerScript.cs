using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerJuneSpawnerScript : MonoBehaviour
{
    bool active;
    float timer;
    List<KillerJuneScript> allKJS = new List<KillerJuneScript>();
    List<KillerJuneScript> availableKJS = new List<KillerJuneScript>();

    public Transform[] spawnPoints;

    int chosenRandom;
    float difficultyMod;



    // Start is called before the first frame update
    void Start()
    {
        difficultyMod = (6f - GameObject.FindGameObjectWithTag("scenemanager").GetComponent<SceneManagerScript>().GetDifficulty()) / 5f; // ranges from 0 to 1
        timer = 4f;
        foreach(GameObject o in GameObject.FindGameObjectsWithTag("killerjune"))
            allKJS.Add(o.GetComponent<KillerJuneScript>());
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = Random.Range(3f + difficultyMod,3.5f + difficultyMod);
                Spawn();
            }
        }
    }

    private void Spawn()
    {
        availableKJS.Clear();
        foreach (KillerJuneScript kjS in allKJS)
        {
            if (!kjS.GetActive())
                availableKJS.Add(kjS);
        }

        if (availableKJS.Count > 0)
        {
            chosenRandom = Random.Range(0, availableKJS.Count);
            availableKJS[chosenRandom].Respawn(spawnPoints[availableKJS[chosenRandom].GetNum()].transform);
        }
    }

    public void Activate()
    {
        active = true;
    }    


}

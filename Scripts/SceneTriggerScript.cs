using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTriggerScript : MonoBehaviour
{
    
    SceneManagerScript smS;
    bool newGame;
    string level;

    // Start is called before the first frame update
    void Start()
    {
        smS = GameObject.FindWithTag("scenemanager").GetComponent<SceneManagerScript>();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetToNewGame(string l)
    {
        newGame = true;
        level = l;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("other: " + other.tag);
        if (other.CompareTag("player"))
        {
            if(newGame)
                smS.LoadNewScene(level);
            else
                smS.LoadNewScene("MainMenu");
        }
    }
}

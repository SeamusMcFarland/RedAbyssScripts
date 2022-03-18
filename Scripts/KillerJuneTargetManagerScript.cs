using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerJuneTargetManagerScript : MonoBehaviour
{
    List<KillerJuneTargetScript> kjtS = new List<KillerJuneTargetScript>();
    int current;

    SceneManagerScript smS;

    // Start is called before the first frame update
    void Start()
    {
        smS = GameObject.FindGameObjectWithTag("scenemanager").GetComponent<SceneManagerScript>();
        current = 0;
        foreach (KillerJuneTargetScript sScript in GetComponentsInChildren<KillerJuneTargetScript>())
            kjtS.Add(sScript);
    }

    public void TargetPlayer()
    {
        kjtS[current].TargetPlayer();
        if (current < kjtS.Count - 1)
            current++;
        else
            current = 0;
    }
}

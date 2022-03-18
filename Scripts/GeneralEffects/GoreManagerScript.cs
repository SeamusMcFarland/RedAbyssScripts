using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoreManagerScript : MonoBehaviour
{

    List<GoreScript> goreS = new List<GoreScript>();
    int current;

    // Start is called before the first frame update
    void Start()
    {
        current = 0;
        foreach (GameObject goreO in GameObject.FindGameObjectsWithTag("gore"))
            goreS.Add(goreO.GetComponent<GoreScript>());
    }

    public void SpawnGore(int num, Vector3 pos)
    {
        goreS[current].PlaceGore(num, pos);
        if (current < goreS.Count - 1)
            current++;
        else
            current = 0;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapManagerScript : MonoBehaviour
{
    List<TrapScript> trapS = new List<TrapScript>();
    int current;

    // Start is called before the first frame update
    void Start()
    {
        current = 0;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("trap"))
            trapS.Add(obj.transform.GetComponentInChildren<TrapScript>());
    }

    public void SpawnTrap(Vector3 pos, float rotation)
    {
        trapS[current].PlaceTrap(pos, rotation);
        if (current < trapS.Count - 1)
            current++;
        else
            current = 0;
    }
}

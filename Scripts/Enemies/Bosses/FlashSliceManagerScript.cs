using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashSliceManagerScript : MonoBehaviour
{
    List<FlashSliceScript> flashSliceS = new List<FlashSliceScript>();
    int current;

    SceneManagerScript smS;


    // Start is called before the first frame update
    void Start()
    {
        smS = GameObject.FindGameObjectWithTag("scenemanager").GetComponent<SceneManagerScript>();
        current = 0;
        foreach (FlashSliceScript fss in GetComponentsInChildren<FlashSliceScript>())
            flashSliceS.Add(fss);
    }

    public void FlashSlice(Vector3 pos)
    {
            flashSliceS[current].FlashSlice(pos);
            if (current < flashSliceS.Count - 1)
                current++;
            else
                current = 0;
        
    }

}

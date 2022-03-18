using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManagerScript : MonoBehaviour
{
    public bool minimap;
    public Material lampOn;
    MeshRenderer[] mrs;

    // Start is called before the first frame update
    void Start()
    {
        if (minimap)
        {
            mrs = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mr in mrs)
                mr.material = lampOn;
        }
    }
}

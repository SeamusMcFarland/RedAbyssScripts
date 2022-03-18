using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringWallScript : MonoBehaviour
{
    MeshRenderer mr;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Random.value < 0.5f)
            mr.enabled = false;
        else
            mr.enabled = true;
    }
}

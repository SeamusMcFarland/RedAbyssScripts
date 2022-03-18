using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerScript : MonoBehaviour
{

    public bool moderate;

    // Update is called once per frame
    void Update()
    {
        if (moderate)
        {
            if (GetComponent<Light>().enabled)
            {
                if (Random.value > 0.95f)
                    GetComponent<Light>().enabled = false;
            }
            else
            {
                if (Random.value > 0.95f)
                    GetComponent<Light>().enabled = true;
            }
        }
        else
        {
            if (GetComponent<Light>().enabled)
            {
                if (Random.value > 0.7f)
                    GetComponent<Light>().enabled = false;
            }
            else
            {
                if (Random.value > 0.7f)
                    GetComponent<Light>().enabled = true;
            }
        }
    }
}

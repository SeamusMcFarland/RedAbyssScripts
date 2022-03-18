using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBallScript : MonoBehaviour
{
    Light theLight;
    MeshRenderer mr;
    bool expanding;
    float size;

    const float STARTING_SIZE = 0.4f;
    const float ENDING_SIZE = 22f; // hitbox is hardcoded 20f
    const float EXPANSION_RATE = 1f;
    const float LIGHT_MAX_INTENSITY = 10f;

    // Start is called before the first frame update
    void Start()
    {
        theLight = GetComponent<Light>();
        theLight.intensity = 0;
        size = STARTING_SIZE;
        expanding = false;
        mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (expanding)
        {
            theLight.intensity += LIGHT_MAX_INTENSITY / Mathf.Round(ENDING_SIZE - STARTING_SIZE);

            transform.localScale = new Vector3(size, size, size);

            if (size < ENDING_SIZE)
                size += EXPANSION_RATE;
            else
                EndBurst();
        }
    }

    public void Burst()
    {
        expanding = true;
        size = STARTING_SIZE;
        transform.localScale = new Vector3(size, size/2f, size);
        mr.enabled = true;
    }

    private void EndBurst()
    {
        theLight.intensity = 0;
        expanding = false;
        mr.enabled = false;
    }

}

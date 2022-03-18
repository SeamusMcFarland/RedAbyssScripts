using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBlockadeScript : MonoBehaviour
{
    Collider coll;
    ParticleSystem[] pss;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<BoxCollider>();
        coll.enabled = false;
        pss = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in pss)
            ps.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Blockade()
    {
        foreach (ParticleSystem ps in pss)
            ps.Play();
        coll.enabled = true;
    }

}

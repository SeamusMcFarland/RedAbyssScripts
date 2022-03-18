using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitterHitboxScript : MonoBehaviour
{
    SpitterScript spitterS;

    // Start is called before the first frame update
    void Start()
    {
        spitterS = transform.parent.GetComponent<SpitterScript>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            spitterS.SetHitboxTriggered(true);
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            spitterS.SetHitboxTriggered(false);
    }
}

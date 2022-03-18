using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmileyHitboxScript : MonoBehaviour
{
    SmileyScript smileyS;
    const bool DISABLED = true;

    void Start()
    {
        smileyS = transform.parent.GetComponent<SmileyScript>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (!DISABLED && coll.gameObject.tag == "player")
            smileyS.SetHitboxTriggered(true);
    }

    private void OnTriggerExit(Collider coll)
    {
        if (!DISABLED && coll.gameObject.tag == "player")
            smileyS.SetHitboxTriggered(false);
    }
}

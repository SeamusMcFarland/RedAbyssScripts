using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultistHitboxScript : MonoBehaviour
{
    CultistScript cultistS;
    const bool DISABLED = true;

    void Start()
    {
        cultistS = transform.parent.GetComponent<CultistScript>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (!DISABLED && coll.gameObject.tag == "player")
            cultistS.SetHitboxTriggered(true);
    }

    private void OnTriggerExit(Collider coll)
    {
        if (!DISABLED && coll.gameObject.tag == "player")
            cultistS.SetHitboxTriggered(false);
    }
}

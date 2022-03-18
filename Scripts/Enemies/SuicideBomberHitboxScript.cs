using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideBomberHitboxScript : MonoBehaviour
{
    SuicideBomberScript suicideBomberS;
    const bool DISABLED = true;

    void Start()
    {
        suicideBomberS = transform.parent.GetComponent<SuicideBomberScript>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (!DISABLED && coll.gameObject.tag == "player")
            suicideBomberS.SetHitboxTriggered(true);
    }

    private void OnTriggerExit(Collider coll)
    {
        if (!DISABLED && coll.gameObject.tag == "player")
            suicideBomberS.SetHitboxTriggered(false);
    }
}

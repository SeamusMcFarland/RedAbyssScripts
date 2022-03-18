using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StriderHitboxScript : MonoBehaviour
{
    StriderScript striderS;
    void Start()
    {
        striderS = transform.parent.GetComponent<StriderScript>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            striderS.SetHitboxTriggered(true);
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            striderS.SetHitboxTriggered(false);
    }
}

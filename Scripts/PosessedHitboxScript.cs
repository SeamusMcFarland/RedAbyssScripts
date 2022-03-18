using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosessedHitboxScript : MonoBehaviour
{
    PosessedScript posessedS;

    // Start is called before the first frame update
    void Start()
    {
        posessedS = transform.parent.GetComponent<PosessedScript>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            posessedS.SetHitboxTriggered(true);
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            posessedS.SetHitboxTriggered(false);
    }
}

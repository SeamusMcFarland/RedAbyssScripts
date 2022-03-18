using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerJuneHitboxScript : MonoBehaviour
{
    KillerJuneScript killerJuneS;
    public bool explosionType;

    // Start is called before the first frame update
    void Start()
    {
        killerJuneS = transform.parent.GetComponent<KillerJuneScript>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "player")
        {
            if (explosionType)
                killerJuneS.SetExplosionHitboxTriggered(true);
            else
                killerJuneS.SetHitboxTriggered(true);
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "player")
        {
            if (explosionType)
                killerJuneS.SetExplosionHitboxTriggered(false);
            else
                killerJuneS.SetHitboxTriggered(false);
        }
    }
}

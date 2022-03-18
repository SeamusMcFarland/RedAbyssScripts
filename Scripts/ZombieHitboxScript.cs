using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHitboxScript : MonoBehaviour
{
    ZombieScript zombieS;

    // Start is called before the first frame update
    void Start()
    {
        zombieS = transform.parent.GetComponent<ZombieScript>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            zombieS.SetHitboxTriggered(true);
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            zombieS.SetHitboxTriggered(false);
    }
}

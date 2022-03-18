using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JacobHitboxScript : MonoBehaviour
{
    JacobScript jacobS;

    // Start is called before the first frame update
    void Start()
    {
        jacobS = transform.parent.GetComponent<JacobScript>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            jacobS.SetHitboxTriggered(true);
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            jacobS.SetHitboxTriggered(false);
    }
}

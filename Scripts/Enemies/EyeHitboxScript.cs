using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeHitboxScript : MonoBehaviour
{
    EyeScript eyeS;

    // Start is called before the first frame update
    void Start()
    {
        eyeS = transform.parent.GetComponent<EyeScript>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            eyeS.SetHitboxTriggered(true);
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            eyeS.SetHitboxTriggered(false);
    }
}

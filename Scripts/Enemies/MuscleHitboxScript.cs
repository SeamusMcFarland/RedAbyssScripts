using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleHitboxScript : MonoBehaviour
{
    MusclesScript musclesS;

    // Start is called before the first frame update
    void Start()
    {
        musclesS = transform.parent.GetComponent<MusclesScript>();
    }
    
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            musclesS.SetHitboxTriggered(true);
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            musclesS.SetHitboxTriggered(false);
    }
}

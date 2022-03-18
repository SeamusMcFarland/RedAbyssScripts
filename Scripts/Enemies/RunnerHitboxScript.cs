using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerHitboxScript : MonoBehaviour
{

    RunnerScript runnerS;

    // Start is called before the first frame update
    void Start()
    {
        runnerS = transform.parent.GetComponent<RunnerScript>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            runnerS.SetHitboxTriggered(true);
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "player")
            runnerS.SetHitboxTriggered(false);
    }

}

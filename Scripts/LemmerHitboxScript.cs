using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LemmerHitboxScript : MonoBehaviour
{
    LemmerScript lemmerS;

    // Start is called before the first frame update
    void Start()
    {
        lemmerS = transform.parent.GetComponent<LemmerScript>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("player"))
            lemmerS.SetHitboxTriggered(true);
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("player"))
            lemmerS.SetHitboxTriggered(false);
    }
}

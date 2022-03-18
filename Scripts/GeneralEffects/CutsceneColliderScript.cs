using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneColliderScript : MonoBehaviour
{

    PlayerScript playerS;
    public int type;
    Collider coll;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider>();
        playerS = GameObject.FindWithTag("player").GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            if(type == 1)
                playerS.JuneCutscene();
            else if (type == 2)
                playerS.EndCutscene();
            coll.enabled = false;
        }
    }

}

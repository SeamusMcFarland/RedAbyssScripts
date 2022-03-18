using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishScript : MonoBehaviour
{
    GameObject player;
    bool seen;
    RaycastHit hit;
    bool checking;
    bool vanished;

    // Start is called before the first frame update
    void Start()
    {
        checking = false;
        seen = false;
        vanished = false;
        player = GameObject.FindGameObjectWithTag("player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!vanished)
        {
            if (seen)
            {
                if (!checking && Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit))
                {
                    if (!hit.transform.CompareTag("player"))
                    {
                        checking = true;
                        StartCoroutine("CheckIfStillUnseen");
                    }
                }
            }
            else if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit))
            {
                if (hit.transform.CompareTag("player"))
                    seen = true;
            }
        }
    }

    IEnumerator CheckIfStillUnseen()
    {
        yield return new WaitForSeconds(1f);
        if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit))
        {
            if (!hit.transform.CompareTag("player"))
            {
                transform.position = new Vector3(300f, 300f, 300f);
                vanished = true;
            }
            else
                checking = false;
        }
        else
            checking = false;
    }

}

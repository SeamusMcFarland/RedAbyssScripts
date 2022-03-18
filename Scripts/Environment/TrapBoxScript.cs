using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBoxScript : MonoBehaviour
{
    const int TRAP_RESTORE = 2;
    int restoreNum;
    bool active;

    UIManagerScript uimanagerS;
    PlayerScript playerS;

    // Start is called before the first frame update
    void Start()
    {
        active = true;
        uimanagerS = GameObject.FindGameObjectWithTag("uimanager").GetComponent<UIManagerScript>();
        playerS = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player") && active == true)
        {
            restoreNum = TRAP_RESTORE + playerS.GetTrapCount();
            if (restoreNum > 3)
                restoreNum = 3;
            uimanagerS.SetTrapCount(restoreNum);
            playerS.SetTrapCount(restoreNum);
            active = false;
            transform.position = new Vector3(300f, 10f, 300f);
        }
    }
}

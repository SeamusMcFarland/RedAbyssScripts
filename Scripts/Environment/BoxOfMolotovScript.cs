using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOfMolotovScript : MonoBehaviour
{
    const int MOLOTOV_RESTORE = 3;
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
            restoreNum = MOLOTOV_RESTORE + playerS.GetMolitovCount();
            if (restoreNum > 5)
                restoreNum = 5;
            uimanagerS.SetMolotovCount(restoreNum);
            playerS.SetMolotovCount(restoreNum);
            active = false;
            transform.position = new Vector3(300f,10f, 300f);
        }
    }

}

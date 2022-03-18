using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardScript : MonoBehaviour
{

    public DoorScript doorS;
    bool used;

    // Start is called before the first frame update
    void Start()
    {
        used = false;
    }

    public void Pickup()
    {
        if (used == false)
        {
            used = true;
            GetComponent<CapsuleCollider>().enabled = false;
            transform.position = new Vector3(90f, 5f, 90f);
            doorS.Opened();
        }
    }
}

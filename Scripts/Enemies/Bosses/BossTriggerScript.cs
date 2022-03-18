using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTriggerScript : MonoBehaviour
{
    bool triggered;
    public JacobScript jacobS;
    // Start is called before the first frame update
    void Start()
    {
        triggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player") && triggered == false)
        {
            triggered = true;
            jacobS.Greeting();
        }

    }
}

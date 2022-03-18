using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmScript : MonoBehaviour
{
    Rigidbody rb;

    float xDiff, zDiff;
    GameObject player;
    float targetRotation;
    const float TURN_RATE = 1f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        rb = GetComponent<Rigidbody>();    
    }

    // Update is called once per frame
    void Update()
    {
        CheckTargetDirection();
        print("target rotation: " + targetRotation);
        print("registered rotation: " + transform.rotation.eulerAngles.y);
        if (Mathf.Abs((targetRotation + 180f) - transform.rotation.eulerAngles.y) < 180f)
        {
            if(targetRotation + 180f > transform.rotation.eulerAngles.y)
                rb.angularVelocity = new Vector3(0, -TURN_RATE, 0);
            else
                rb.angularVelocity = new Vector3(0, TURN_RATE, 0);
        }
        /*else
        {
            if (targetRotation + 180f > transform.rotation.eulerAngles.y)
                rb.angularVelocity = new Vector3(0, -TURN_RATE, 0);
            else
                rb.angularVelocity = new Vector3(0, TURN_RATE, 0);
        }*/
    }

    public void CheckTargetDirection()
    {
        xDiff = player.transform.position.x - transform.position.x;
        zDiff = player.transform.position.z - transform.position.z;
        targetRotation = -(Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff));
    }
}

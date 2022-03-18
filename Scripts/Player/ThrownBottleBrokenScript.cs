using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownBottleBrokenScript : MonoBehaviour
{
    public ThrownBottleScript tbs;

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "floor" || coll.gameObject.tag == "wall")
        {
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
            tbs.Broken();
        }
    }

}

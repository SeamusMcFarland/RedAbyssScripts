using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    public GameObject[] pieces;
    public bool destructable;
    bool broken;

    private void Start()
    {
        broken = false;
    }

    public void BreakBox()
    {
        if (broken == false)
        {
            broken = true;
            foreach (GameObject o in pieces)
            {
                o.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                o.transform.position = transform.position;
            }
            if (destructable == true)
                GetComponent<LaunchableChairScript>().MarkAsDestroyed();
            transform.position = new Vector3(200f, 200f, 200f);
        }
    }

}

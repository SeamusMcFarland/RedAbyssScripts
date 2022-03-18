using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelPiecesScript : MonoBehaviour
{

    const float MAX_FORCE = 50f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(300f, 300f, 300f);
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }

    public void ReleaseScrap(Vector3 pos)
    {
        GetComponent<Rigidbody>().useGravity = true;
        transform.position = pos;
        GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-MAX_FORCE, MAX_FORCE), Random.Range(-MAX_FORCE, MAX_FORCE), Random.Range(-MAX_FORCE, MAX_FORCE));
    }

}

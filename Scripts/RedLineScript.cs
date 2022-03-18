using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLineScript : MonoBehaviour
{
    Vector3 savedPosition;

    // Start is called before the first frame update
    void Start()
    {
        savedPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z + 0.01f);
        if (transform.position.x >= savedPosition.x + 10f)
            transform.position = savedPosition;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingWallsScript : MonoBehaviour
{
    float rotation;
    PlayerScript playerS;

    // Start is called before the first frame update
    void Start()
    {
        rotation = 0;
        playerS = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerS.GetPaused())
        {
            rotation += Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }
    }
}

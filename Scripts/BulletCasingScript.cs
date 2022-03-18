using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCasingScript : MonoBehaviour
{
    public Material[] bulletM;
    const float BASE_FRICTION = 0.01f;

    Rigidbody rb;
    bool used;
    float artRotVel;
    float newZR;

    SceneManagerScript smS;

    // Start is called before the first frame update
    void Start()
    {
        smS = GameObject.FindGameObjectWithTag("scenemanager").GetComponent<SceneManagerScript>();
        used = false;
        rb = GetComponent<Rigidbody>();
        artRotVel = Random.Range(-20f, 20f);
        newZR = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (used == true && Mathf.Abs(artRotVel) > 0.1f)
        {
            if (artRotVel > 0.05f)
                artRotVel -= BASE_FRICTION * Mathf.Pow(artRotVel, 2) + BASE_FRICTION;
            if (artRotVel < 0.05f)
                artRotVel += BASE_FRICTION * Mathf.Pow(artRotVel, 2) + BASE_FRICTION;
            newZR += artRotVel;
            transform.rotation = Quaternion.Euler(90f, 0f, newZR);
        }
    }

    public void PlaceBullet(Vector3 pos, int type)
    {
        used = true;
        transform.position = new Vector3(pos.x, Random.Range(0.0010f, 0.0020f), pos.z);
        if (smS.GetScene() != 4)
        {
            rb.velocity = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
            artRotVel = Random.Range(-15f, 15f);
        }

        GetComponent<Renderer>().material = bulletM[type - 1];
        if (type == 1) // default
            transform.localScale = new Vector3(0.2f, 0.1f, 1f);
        else if (type == 2) // shotgun
            transform.localScale = new Vector3(0.3f, 0.1f, 1f);
        else
            print("ERROR! INVALID BULLET TYPE IN BULLETCASINGSCRIPT");

        
        transform.rotation = Quaternion.Euler(90, Random.Range(0, 360), 0);
        
    }

    public void FadeBullet()
    {

    }
}

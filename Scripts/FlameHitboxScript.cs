using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class FlameHitboxScript : MonoBehaviour
{
    bool burningPlayer;
    PlayerScript playerS;
    const float BURN_DAMAGE = 0.07f;
    bool active;
    SphereCollider coll;
    float rotation;
    const float SIZE_RATE = 0.02f;
    float savedRadius;
    float tsr; // time since release
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<SphereCollider>();
        savedRadius = coll.radius;
        active = false;
        transform.position = new Vector3(300f,300f,300f);
        burningPlayer = false;
        playerS = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            coll.radius += SIZE_RATE;
            tsr += Time.deltaTime;

            if (burningPlayer)
                playerS.BurnHit(BURN_DAMAGE, 3);

            if (tsr > 0.75f) // average lifespan of particles
            {
                active = false;
                rb.velocity = new Vector3(0, 0, 0);
                transform.position = new Vector3(300f,300f,300f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
            burningPlayer = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player"))
            burningPlayer = false;
    }

    public void Release(float r, Vector3 pos)
    {
        print("flame: " + name + " released at: " + Time.deltaTime);
        tsr = 0;
        coll.radius = savedRadius;
        transform.position = pos;
        rotation = r;
        transform.rotation = Quaternion.Euler(90, -rotation, 0);
        rb.velocity = transform.right * 10f;
        active = true;
    }
    

}

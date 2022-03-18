using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchableChairScript : MonoBehaviour
{
    const float BASE_DAMAGE = 0.5f;
    const float MINIMUM_VELOCITY = 5f;
    const float VELOCITY_MODIFIER = 0.4f;
    const float BASE_LAUNCH = 10f;

    bool active;
    PlayerScript playerS;
    Rigidbody rb;

    bool destroyed;

    void Start()
    {
        destroyed = false;
        active = false;
        playerS = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerScript>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(destroyed == false)
            if (active && GetVelocityMagnitude() < MINIMUM_VELOCITY) // ends hitbox after hitbox is activated but becomes slowed to below a minimum velocity required to deal damage
                active = false;
    }

    public void Clump(Vector3 pos)
    {
        if(destroyed == false)
            rb.velocity = new Vector3((pos.x - transform.position.x) * VELOCITY_MODIFIER + rb.velocity.x, (pos.y - transform.position.y) * VELOCITY_MODIFIER + rb.velocity.y, (pos.z - transform.position.z) * VELOCITY_MODIFIER + rb.velocity.z);
    }

    public void Launch()
    {
        if (destroyed == false)
        {
            rb.velocity = new Vector3(Random.Range(-BASE_LAUNCH, BASE_LAUNCH), Random.Range(-BASE_LAUNCH, BASE_LAUNCH), Random.Range(-BASE_LAUNCH, BASE_LAUNCH));
            active = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (active && collision.gameObject.CompareTag("player"))
                playerS.Hit(BASE_DAMAGE * GetVelocityMagnitude());
    }

    private float GetVelocityMagnitude()
    {
        return Mathf.Pow(Mathf.Pow(rb.velocity.x, 2f) + Mathf.Pow(rb.velocity.y, 2f) + Mathf.Pow(rb.velocity.z, 2f), 0.5f);
    }

    public void MarkAsDestroyed()
    {
        destroyed = true;
        active = false;
    }

}

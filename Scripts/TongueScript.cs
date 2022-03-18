using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueScript : MonoBehaviour
{

    GameObject player;
    Rigidbody rb;
    bool active;
    LemmerScript lemmerS;
    bool grabbing;
    bool atPlayer;
    public TrailRenderer innerTR, outerTR;
    float rotation;

    // Start is called before the first frame update
    void Start()
    {
        lemmerS = GameObject.FindWithTag("lemmer").GetComponent<LemmerScript>();
        player = GameObject.FindGameObjectWithTag("player");
        rb = GetComponent<Rigidbody>();
        active = false;
        grabbing = false;
        atPlayer = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbing) // alternates each frame between the player and Lemmer
        {
            innerTR.Clear();
            outerTR.Clear();

            if (atPlayer)
                transform.position = lemmerS.transform.position; // sets to ontop of lemmer
            else
                transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            atPlayer = !atPlayer;
        }
        else if (active)
        {
            rb.velocity = new Vector3(Mathf.Cos(rotation * Mathf.Deg2Rad), 0f, Mathf.Sin(rotation * Mathf.Deg2Rad)) * 20f;
            //transform.localPosition = new Vector3(10f,10f,10f);
            print("active but not grabbing");
        }
    }

    public void Launch()
    {
        transform.position = lemmerS.transform.position; // sets to ontop of lemmer
        StartCoroutine("DelayLaunch");
        print("tongue LAUNCHED");
    }

    IEnumerator DelayLaunch()
    {
        yield return new WaitForSeconds(0.01f);
        CheckDirection();
        active = true;

    }

    public void EndGrab()
    {
        active = false;
        grabbing = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (active && !grabbing && (other.CompareTag("wall") || other.CompareTag("player")))
        {
            grabbing = true;
            if (other.CompareTag("player"))
            {
                lemmerS.SuccessfulTongue();
            }
            else
            {
                lemmerS.EndTongue();
                EndGrab();
            }
            rb.velocity = new Vector3(0, 0, 0);

            print("STOPPED FROM: " + other.tag);
        }
    }

    public void CheckDirection()
    {
        float xDiff = player.transform.position.x - transform.position.x;
        float zDiff = player.transform.position.z - transform.position.z;
        rotation = Mathf.Rad2Deg * Mathf.Atan2(zDiff, xDiff);
        transform.rotation = Quaternion.Euler(90f, rotation, 0);

    }
}

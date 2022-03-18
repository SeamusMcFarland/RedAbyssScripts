using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    RaycastHit[] allHits;
    bool playerHit;
    GameObject player;
    PlayerScript playerS;
    RaycastHit closestWall;
    bool foundWall;
    ParticleSystem.MainModule psm;
    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        playerS = player.GetComponent<PlayerScript>();
        ps = GetComponent<ParticleSystem>();
        psm = ps.main;
        ps.Stop();
        playerHit = false;
        foundWall = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(Transform tran, float laserDamage, float rotation)
    {
        playerHit = false;
        foundWall = false;

        //LASER DETECTION AND DAMAGE
        allHits = Physics.RaycastAll(tran.position, tran.TransformDirection(Vector3.right), 300.0f);
        Debug.DrawRay(tran.position, tran.TransformDirection(Vector3.right), Color.blue, 300.0f);
        foreach (RaycastHit hit in allHits)
        {
            if (!foundWall)
            {
                if (hit.transform.gameObject.CompareTag("wall"))
                {
                    closestWall = hit;
                    foundWall = true;
                }
            }
            else if (hit.transform.gameObject.CompareTag("wall") && Get2DDistance(closestWall.transform, transform) > Get2DDistance(hit.transform, transform))
            {
                print("closestwall assign");
                closestWall = hit;
            }
        }
        /*print("distance: " + Get2DDistance(closestWall.transform, transform));
        foreach (RaycastHit hit in allHits)
            print("all hits BEFORE: " + hit.transform.tag);
        allHits = Physics.RaycastAll(transform.position + Vector3.right, transform.TransformDirection(Vector3.forward), Get2DDistance(closestWall.transform, transform));
        foreach (RaycastHit hit in allHits)
            print("all hits AFTER: " + hit.transform.tag);
        print("length after: " + allHits.Length);*/
        foreach (RaycastHit hit in allHits)
        {
            if (hit.transform.gameObject.CompareTag("player"))
            {
                if (Get2DDistance(closestWall.transform, transform) > Get2DDistance(player.transform, transform))
                {
                    playerHit = true;
                    print("found player after!");
                    playerS.Hit(laserDamage);
                }
                break;
            }
        }
        //LASER VISUAL EFFECT
        transform.rotation = Quaternion.Euler(0, -rotation + 90f, 0);
        psm.startLifetime = Get2DDistance(closestWall.transform, transform) / 5f; // has a length of about 25 with a lifetime of 5
        ps.Play();
        playerHit = false;
        StartCoroutine("EndLaser");


    }

    IEnumerator EndLaser()
    {
        yield return new WaitForSeconds(0.4f);
        ps.Stop();
    }

    private float Get2DDistance(Transform t1, Transform t2)
    {
        return Mathf.Sqrt(Mathf.Pow(t1.position.x - t2.position.x, 2f) + Mathf.Pow(t1.position.z - t2.position.z, 2f));
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningScript : MonoBehaviour
{
    public GameObject jacobPointer;
    ParticleSystem.MainModule psm;
    ParticleSystem ps;
    PlayerScript playerS;
    RaycastHit[] allHits;
    bool playerHit; // prevents multiple hit calls from multiple raycasts

    // Start is called before the first frame update
    void Start()
    {
        playerHit = false;
        playerS = GameObject.FindWithTag("player").GetComponent<PlayerScript>();
        //psm = GetComponent<ParticleSystem.MainModule>();
        ps = GetComponent<ParticleSystem>();
        psm = ps.main;
        ps.Stop();
    }

    public void SummonLightning(float distance, float rotation, float lightningDamage)
    {
        transform.position = new Vector3 (jacobPointer.transform.position.x, 0.5f, jacobPointer.transform.position.z);
        transform.rotation = Quaternion.Euler(0, -rotation + 90f, 0);
        psm.startLifetime = distance; // has a length of about 25 with a lifetime of 5
        ps.Play();
        playerHit = false;
        DamageRaycast(lightningDamage, 0);
        if(!playerHit)
            DamageRaycast(lightningDamage, 0.2f);
        if (!playerHit)
            DamageRaycast(lightningDamage, -0.2f);
        StartCoroutine("EndLightning");
    }

    private void DamageRaycast(float lightningDamage, float variation)
    {
        allHits = Physics.RaycastAll(transform.position + Vector3.right * variation, transform.TransformDirection(Vector3.forward), 100.0f);
        //Debug.DrawRay(transform.position + transform.right * variation, transform.TransformDirection(Vector3.forward), Color.blue, 100.0f);
        foreach (RaycastHit hit in allHits)
        {
            if (hit.transform.gameObject.CompareTag("player"))
            {
                playerHit = true;
                playerS.Hit(lightningDamage);
                break;
            }
        }
    }

    IEnumerator EndLightning()
    {
        yield return new WaitForSeconds(0.1f);
        ps.Stop();
    }

}

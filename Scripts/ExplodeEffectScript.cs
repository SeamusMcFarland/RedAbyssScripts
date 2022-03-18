using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeEffectScript : MonoBehaviour
{
    public ParticleSystem bloodPS;
    public ParticleSystem smokePS;
    public Light flashL;

    void Start()
    {
        bloodPS.Stop();
        smokePS.Stop();
        flashL.enabled = false;
    }

    public void Explode(Vector3 posVec)
    {
        transform.position = new Vector3(posVec.x, transform.position.y, posVec.z);
        bloodPS.Play();
        smokePS.Play();
        flashL.enabled = true;
        StartCoroutine("EndLight");
        StartCoroutine("EndOthers");
    }

    IEnumerator EndLight()
    {
        yield return new WaitForSeconds(0.01f);
        flashL.enabled = false;
    }

    IEnumerator EndOthers()
    {
        yield return new WaitForSeconds(0.6f);
        bloodPS.Stop();
        smokePS.Stop();
    }
}

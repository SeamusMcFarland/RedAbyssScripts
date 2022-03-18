using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffectScript : MonoBehaviour
{
    public ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Stop();
    }

    public void CreateEffect(Vector3 pos)
    {
        transform.position = pos;
        ps.Play();
        StartCoroutine("EndEffect");
    }

    IEnumerator EndEffect()
    {
        yield return new WaitForSeconds(0.2f);
        ps.Stop();
    }

}

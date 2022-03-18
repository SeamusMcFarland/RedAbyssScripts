using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenScript : MonoBehaviour
{
    float addedTime;
    public Texture[] allTex;
    public Material[] allMat;
    MeshRenderer mr;
    public Light theLight;
    int chosen;
    bool broken;
    public Material brokenMaterial;
    Rigidbody parentRB;
    BoxCollider parentCollider;

    // Start is called before the first frame update
    void Start()
    {
        broken = false;
        mr = GetComponent<MeshRenderer>();
        addedTime = Random.Range(0.1f, 0.4f);
        parentRB = transform.parent.GetComponent<Rigidbody>();
        parentCollider = transform.parent.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (broken == false)
        {
            if (addedTime > 0)
                addedTime -= Time.deltaTime;
            else
            {
                addedTime = Random.Range(0.1f, 0.8f);
                chosen = (int)Random.Range(0, 5);
                mr.material = allMat[chosen];
                //mr.material.SetTexture("_EmissionMap", allTex[chosen]);
                theLight.cookie = allTex[chosen];
            }
        }
    }

    public void BreakScreen()
    {
        broken = true;
        theLight.enabled = false;
        parentCollider.isTrigger = false;
        transform.parent.gameObject.layer = 2;
        parentRB.useGravity = true;
        parentRB.velocity = new Vector3(Random.Range(-3f,3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        mr.material = brokenMaterial;
    }

}

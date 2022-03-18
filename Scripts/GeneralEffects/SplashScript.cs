using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScript : MonoBehaviour
{
    MeshRenderer mr;
    AnimationScript animationS;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        animationS = GetComponent<AnimationScript>();
        animationS.SetState(1);
        mr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Splash(Vector3 pos)
    {
        mr.enabled = true;
        transform.position = new Vector3(pos.x, transform.position.y, pos.z);
        animationS.SetState(3);
        StartCoroutine("EndSplash");
    }

    IEnumerator EndSplash()
    {
        yield return new WaitForSeconds(0.25f);
        mr.enabled = false;
        animationS.SetState(1);
    }

    
}

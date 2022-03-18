using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypnoticScript : MonoBehaviour
{
    public Material[] allM;
    MeshRenderer mr;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Burst()
    {
        StartCoroutine("DelayBurst");
    }

    IEnumerator DelayBurst()
    {
        SwitchSprite(0);
        mr.enabled = true;
        yield return new WaitForSeconds(0.05f);
        SwitchSprite(1);
        yield return new WaitForSeconds(0.05f);
        SwitchSprite(2);
        yield return new WaitForSeconds(0.05f);
        SwitchSprite(3);
        yield return new WaitForSeconds(0.05f);
        SwitchSprite(4);
        yield return new WaitForSeconds(0.05f);
        SwitchSprite(5);
        yield return new WaitForSeconds(0.05f);
        mr.enabled = false;
    }

    private void SwitchSprite(int num)
    {
        switch (num)
        {
            case 0:
                mr.material = allM[0];
                transform.localScale = new Vector3(1.5f, 1.5f, transform.localScale.z);
                break;
            case 1:
                mr.material = allM[1];
                transform.localScale = new Vector3(4.5f, 4.5f, transform.localScale.z);
                break;
            case 2:
                mr.material = allM[2];
                transform.localScale = new Vector3(9.3f, 9.3f, transform.localScale.z);
                break;
            case 3:
                mr.material = allM[3];
                transform.localScale = new Vector3(17.7f, 17.7f, transform.localScale.z);
                break;
            case 4:
                mr.material = allM[4];
                transform.localScale = new Vector3(33.9f, 33.9f, transform.localScale.z);
                break;
            case 5:
                mr.material = allM[5];
                transform.localScale = new Vector3(66.9f, 66.9f, transform.localScale.z);
                break;
        }
    }
}

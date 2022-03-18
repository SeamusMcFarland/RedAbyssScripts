using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFlickerScript : MonoBehaviour
{
    public float activationDistance;

    GameObject player;
    MeshRenderer mr;
    bool done;
    bool on;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        player = GameObject.FindWithTag("player");
        done = true;
        mr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (done)
        {
            if (on)
            {
                if (GetPlayerDistance() > activationDistance)
                    StartCoroutine("FlickerOff");
            }
            else
            {
                if (GetPlayerDistance() < activationDistance)
                    StartCoroutine("FlickerOn");
            }
        }
    }

    IEnumerator FlickerOn()
    {
        done = false;
        for (int i = 0; i < (int)Random.Range(3f, 8f); i++)
        {
            mr.enabled = true;
            yield return new WaitForSeconds(Random.Range(0.005f, 0.04f));
            mr.enabled = false;
            yield return new WaitForSeconds(Random.Range(0.005f, 0.04f));
        }
        mr.enabled = true;
        on = true;
        done = true;
    }

    IEnumerator FlickerOff()
    {
        done = false;
        for (int i = 0; i < (int)Random.Range(3f, 8f); i++)
        {
            mr.enabled = false;
            yield return new WaitForSeconds(Random.Range(0.005f, 0.04f));
            mr.enabled = true;
            yield return new WaitForSeconds(Random.Range(0.005f, 0.04f));
        }
        mr.enabled = false;
        on = false;
        done = true;
    }

    private float GetPlayerDistance()
    {
        return Mathf.Pow(Mathf.Pow(this.transform.position.x - player.transform.position.x, 2) + Mathf.Pow(this.transform.position.z - player.transform.position.z, 2), 0.5f);
    }

}

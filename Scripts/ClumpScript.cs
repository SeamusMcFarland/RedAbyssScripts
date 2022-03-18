using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClumpScript : MonoBehaviour
{
    bool clumping;
    public GameObject[] lc;
    List<LaunchableChairScript> lcS = new List<LaunchableChairScript>();

    const float GATHER_TIME = 3f;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject o in lc)
            lcS.Add(o.GetComponent<LaunchableChairScript>());
        clumping = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (clumping)
        {
            foreach (LaunchableChairScript s in lcS)
                s.Clump(transform.position);
        }
    }

    public void Clump() // First method to be called to initiate clumped and launched objects attack
    {
        clumping = true;
        StartCoroutine("Gather");
    }

    IEnumerator Gather()
    {
        yield return new WaitForSeconds(GATHER_TIME);
        clumping = false;
        Launch();
    }

    private void Launch()
    {
        foreach (LaunchableChairScript s in lcS)
            s.Launch();
    }

}

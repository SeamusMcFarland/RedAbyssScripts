using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterFlickerScript : MonoBehaviour
{
    public MeshRenderer[] letterR;
    public MeshRenderer[] letterE;
    public MeshRenderer[] letterD;
    public MeshRenderer[] letterA;
    public MeshRenderer[] letterB;
    public MeshRenderer[] letterY;
    public MeshRenderer[] letterS1;
    public MeshRenderer[] letterS2;
    public Material lightM;
    public Material darkM;

    bool[] flickering = new bool[8];


    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 8; i++)
        {
            if (!flickering[i] && Random.value < 0.0015f)
            {
                flickering[i] = true;
                StartCoroutine("Flicker", i);
            }
        }
    }

    IEnumerator Flicker(int num)
    {
        MeshRenderer[] selectedL;

        if (num == 1)
            selectedL = letterR;
        else if (num == 2)
            selectedL = letterE;
        else if (num == 3)
            selectedL = letterD;
        else if (num == 4)
            selectedL = letterA;
        else if (num == 5)
            selectedL = letterB;
        else if (num == 6)
            selectedL = letterY;
        else if (num == 7)
            selectedL = letterS1;
        else // num 8
            selectedL = letterS2;

        foreach (MeshRenderer mr in selectedL)
            mr.material = darkM;

        yield return new WaitForSeconds(Random.Range(0.005f, 0.04f));
        foreach (MeshRenderer mr in selectedL)
            mr.material = lightM;
        yield return new WaitForSeconds(Random.Range(0.005f, 0.04f));
        print("reached end");
        if(Random.value > 0.2f)
            StartCoroutine("Flicker", num);
        else
            flickering[num] = false;

    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidContainerScript : MonoBehaviour
{
    bool broke;
    int acidOffset;
    public AcidScript[] acidS;

    // Start is called before the first frame update
    void Start()
    {
        broke = false;
    }

    public void PunctureContainer()
    {
        if (broke == false)
        {
            broke = true;
            acidOffset = 1;
            foreach (AcidScript f in acidS)
            {
                f.Spill(new Vector3(transform.position.x + GetAcidOffsetX(acidOffset) + Random.Range(-1f, 1f), .001f, transform.position.z + GetAcidOffsetZ(acidOffset) + Random.Range(-1f, 1f)));
                acidOffset += 1;
            }
        }
    }

    private float GetAcidOffsetX(int an) // starts in middle, then goes out directly right and travels around counterclockwise
    {
        if (an > 9) // allows a second cycle through
            an -= 9;
        if (an == 1)
            return 0;
        else if (an == 2)
            return 1f;
        else if (an == 3)
            return 1f;
        else if (an == 4)
            return 0f;
        else if (an == 5)
            return -1f;
        else if (an == 6)
            return -1f;
        else if (an == 7)
            return -1f;
        else if (an == 8)
            return 0;
        else if (an == 9)
            return 1f;
        else
            return 100f; //returns if error
    }

    private float GetAcidOffsetZ(int an) // starts in middle, then goes out directly right and travels around counterclockwise
    {
        if (an > 9) // allows a second cycle through
            an -= 9;
        if (an == 1)
            return 0;
        else if (an == 2)
            return 0f;
        else if (an == 3)
            return 1f;
        else if (an == 4)
            return 1f;
        else if (an == 5)
            return 1f;
        else if (an == 6)
            return 0f;
        else if (an == 7)
            return -1f;
        else if (an == 8)
            return -1f;
        else if (an == 9)
            return -1f;
        else
            return 100f; //returns if error
    }

}

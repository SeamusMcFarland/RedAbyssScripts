using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHealthbarManagerScript : MonoBehaviour
{
    List<MonsterHealthbarScript> mhS = new List<MonsterHealthbarScript>();
    int current;

    // Start is called before the first frame update
    void Start()
    {
        current = 0;
        foreach (MonsterHealthbarScript s in GetComponentsInChildren<MonsterHealthbarScript>())
            mhS.Add(s.GetComponent<MonsterHealthbarScript>());
    }

    public MonsterHealthbarScript BeginHealthbar(float healthDecimal, Vector3 pos)
    {
        if (current < mhS.Count - 1)
            current++;
        else
            current = 0;
        mhS[current].ActivateHealthbar(healthDecimal, pos);
        return mhS[current];
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeEffectManagerScript : MonoBehaviour
{
    List<StrikeEffectScript> strikeEffectS = new List<StrikeEffectScript>();
    int current;

    // Start is called before the first frame update
    void Start()
    {
        current = 0;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("strikeeffect"))
            strikeEffectS.Add(obj.GetComponent<StrikeEffectScript>());
    }

    public void SpawnStrikeEffect(Vector3 pos, int modColor) // 2 for white
    {
        strikeEffectS[current].PlaceStrikeEffect(pos, modColor);
        if (current < strikeEffectS.Count - 1)
            current++;
        else
            current = 0;
    }

}

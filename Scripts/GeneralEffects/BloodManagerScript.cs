using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodManagerScript : MonoBehaviour
{
    List<BloodScript> bloodS = new List<BloodScript>();
    int current;

    SceneManagerScript smS;

    // Start is called before the first frame update
    void Start()
    {
        smS = GameObject.FindGameObjectWithTag("scenemanager").GetComponent<SceneManagerScript>();
        current = 0;
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("blood"))
            bloodS.Add(obj.GetComponent<BloodScript>());
    }

    public void SpawnBlood(Vector3 pos)
    {
        if (smS.GetScene() != 4)
        {
            bloodS[current].PlaceBlood(pos);
            if (current < bloodS.Count - 5)
                bloodS[current + 5].FadeBlood();
            else
                bloodS[current + 5 - bloodS.Count].FadeBlood();
            if (current < bloodS.Count - 1)
                current++;
            else
                current = 0;
        }
    }

    public void SpawnBlood(Vector3 pos, int modColor) // 1 for white
    {
        if (smS.GetScene() != 4)
        {
            bloodS[current].PlaceBlood(pos, modColor);
            if (current < bloodS.Count - 5)
                bloodS[current + 5].FadeBlood();
            else
                bloodS[current + 5 - bloodS.Count].FadeBlood();
            if (current < bloodS.Count - 1)
                current++;
            else
                current = 0;
        }
    }

}

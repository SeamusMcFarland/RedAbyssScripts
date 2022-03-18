using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCasingManagerScript : MonoBehaviour
{
    List<BulletCasingScript> bulletS = new List<BulletCasingScript>();
    int current;

    // Start is called before the first frame update
    void Start()
    {
        current = 0;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("bulletcasing"))
            bulletS.Add(obj.GetComponent<BulletCasingScript>());
    }

    public void SpawnBullet(Vector3 pos, int type) // 1 is default, 2 is shotgun
    {
        bulletS[current].PlaceBullet(pos, type);
        if (current < bulletS.Count - 5)
            bulletS[current + 5].FadeBullet();
        else
            bulletS[current + 5 - bulletS.Count].FadeBullet();
        if (current < bulletS.Count - 1)
            current++;
        else
            current = 0;
    }

}

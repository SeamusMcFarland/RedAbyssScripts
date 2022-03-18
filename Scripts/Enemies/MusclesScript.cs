using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusclesScript : MonsterScript
{
    public override void assignTraits()
    {
        SetHealth(7f);
        SetDamage(3f);
        SetAfterlag(0.75f);
        SetStartup(0f);
        SetBaseSpeed(10f);
        SetSightDistance(9f);
        SetLightSensitivity(1.1f);
        SetHearingDistance(6f);
        SetHitstun(0.3f);
        SetStrikeDistance(1.5f);
        SetVisionRange(20f);
        SetPackDistance(2f);
    }

    public override void Gore()
    {
        for (int i = 0; i < 5; i++)
            GetBloodMS().SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y, transform.position.z + Random.Range(-1.5f, 1.5f)));
        if (!GetDeathMuted())
            GetAudioS().PlayBloodExplosion();
        for (int i = 12; i < 14; i++)
            GetGoreMS().SpawnGore(i, transform.position);
    }

    List<GameObject> tempList = new List<GameObject>();

    public override void PackReaction()
    {
        tempList = GetSameNearby();
        foreach (GameObject o in tempList)
        {
            print("the tag: " + o.transform.tag);
            o.transform.parent.GetComponent<MusclesScript>().DetectedPlayer();
            o.transform.parent.GetComponent<MusclesScript>().Aggroed();
        }
    }

}

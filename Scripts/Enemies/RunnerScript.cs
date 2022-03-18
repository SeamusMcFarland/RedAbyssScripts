using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerScript : MonsterScript
{
    public override void assignTraits()
    {
        SetHealth(3f);
        SetDamage(2f);
        SetAfterlag(0.60f);
        SetStartup(0f);
        SetBaseSpeed(10f);
        SetSightDistance(11f);
        SetLightSensitivity(1.1f);
        SetHearingDistance(8f);
        SetHitstun(0.2f);
        SetStrikeDistance(1.5f);
        SetVisionRange(20f);
        SetPackDistance(5f);
    }

    public override void Gore()
    {
        for (int i = 0; i < 5; i++)
            GetBloodMS().SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y, transform.position.z + Random.Range(-1.5f, 1.5f)));
        if (!GetDeathMuted())
            GetAudioS().PlayBloodExplosion();
        for (int i = 0; i < 6; i++)
            GetGoreMS().SpawnGore(i, transform.position);
    }

    List<GameObject> tempList = new List<GameObject>();

    public override void PackReaction()
    {
        tempList = GetSameNearby();
        foreach (GameObject o in tempList)
        {
            o.transform.parent.GetComponent<RunnerScript>().DetectedPlayer();
            o.transform.parent.GetComponent<RunnerScript>().Aggroed();
        }
    }


}

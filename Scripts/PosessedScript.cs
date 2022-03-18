using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosessedScript : MonsterScript
{

    const float FLICKER_DISTANCE = 12f;
    public override void assignTraits()
    {
        SetHealth(6f);
        SetDamage(4f);
        SetAfterlag(1f);
        SetStartup(0f);
        SetBaseSpeed(6f);
        SetSightDistance(11f);
        SetLightSensitivity(1f);
        SetHearingDistance(11f);
        SetHitstun(0.3f);
        SetStrikeDistance(1.5f);
        SetVisionRange(20f);
        SetPackDistance(3.5f);
    }

    public override void AddedMethods()
    {
        if (!GetPlayerScript().GetPlayerDead())
        {
            if (Get2DDistance(GetPlayer().transform, transform) < FLICKER_DISTANCE && GetPlayerSeen())
                GetPlayerScript().AddFlickerSum(Get2DDistance(GetPlayer().transform, transform)/FLICKER_DISTANCE);
        }
    }

    public override void Gore()
    {
        for (int i = 0; i < 5; i++)
            GetBloodMS().SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y, transform.position.z + Random.Range(-1.5f, 1.5f)));
        if (!GetDeathMuted())
            GetAudioS().PlayBloodExplosion();
        for (int i = 97; i < 101; i++)
            GetGoreMS().SpawnGore(i, transform.position);
        GetGoreMS().SpawnGore(64, transform.position);
    }

    List<GameObject> tempList = new List<GameObject>();

    public override void PackReaction()
    {
        tempList = GetSameNearby();
        foreach (GameObject o in tempList)
        {
            o.transform.parent.GetComponent<PosessedScript>().DetectedPlayer();
            o.transform.parent.GetComponent<PosessedScript>().Aggroed();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScript : MonsterScript
{
    bool seenState;
    int limbsLost;

    public override void assignTraits()
    {
        seenState = false;
        limbsLost = 0;
        SetAnimationLock(true); // animation states are rigged to be off

        SetHealth(4f);
        SetDamage(4f);
        SetAfterlag(1.25f);
        SetStartup(0f);
        SetBaseSpeed(6f);
        SetSightDistance(4.5f);
        SetLightSensitivity(1.2f);
        SetHearingDistance(10f);
        SetHitstun(0.2f);
        SetStrikeDistance(1.5f);
        SetVisionRange(20f);
        SetPackDistance(8f);
    }

    public override void AddedMethods()
    {
        if (GetPlayerSeen() || seenState)
        {
            seenState = true;
            if (GetHealth() > 3f)
            {
                if (limbsLost < 1)
                {
                    limbsLost = 1;
                    GetGoreMS().SpawnGore(70, transform.position);
                }
                GetAnimatorScript().SetMaterial(2);
            }
            else if (GetHealth() > 2f)
            {
                if (limbsLost < 2)
                {
                    SetDamage(3f);
                    SetBaseSpeed(5f);
                    limbsLost = 2;
                    GetGoreMS().SpawnGore(71, transform.position);
                }
                GetAnimatorScript().SetMaterial(3);
            }
            else if (GetHealth() > 1f)
            {
                if (limbsLost < 3)
                {
                    SetDamage(2f);
                    SetBaseSpeed(4f);
                    limbsLost = 3;
                    GetGoreMS().SpawnGore(72, transform.position);
                }
                GetAnimatorScript().SetMaterial(4);
            }
            else
            {
                if (limbsLost < 4)
                {
                    SetDamage(1f);
                    SetBaseSpeed(3f);
                    limbsLost = 4;
                    GetGoreMS().SpawnGore(73, transform.position);
                }
                GetAnimatorScript().SetMaterial(5);
            }
        }
    }

    public override void Gore()
    {
        for (int i = 0; i < 5; i++)
            GetBloodMS().SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y, transform.position.z + Random.Range(-1.5f, 1.5f)));
        if (!GetDeathMuted())
            GetAudioS().PlayBloodExplosion();
        for (int i = 74; i < 76; i++)
            GetGoreMS().SpawnGore(i, transform.position);
    }

    List<GameObject> tempList = new List<GameObject>();

    public override void PackReaction()
    {
        tempList = GetSameNearby();
        foreach (GameObject o in tempList)
        {
            o.transform.parent.GetComponent<ZombieScript>().DetectedPlayer();
            o.transform.parent.GetComponent<ZombieScript>().Aggroed();
        }
    }
}

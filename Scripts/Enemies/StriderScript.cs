using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StriderScript : MonsterScript
{
    bool strideCooldown;
    const float STRIDE_DELAY = .5f;

    public override void assignTraits()
    {
        strideCooldown = false;

        SetBloodType(1);
        SetHealth(2.1f);
        SetDamage(5f);
        SetAfterlag(0.75f);
        SetStartup(0f);
        SetBaseSpeed(10f); //this does not matter since it is changed in added methods
        SetSightDistance(10f);
        SetLightSensitivity(0.9f); // light slightly blinds striders
        SetHearingDistance(4f);
        SetHitstun(0.3f);
        SetStrikeDistance(1.5f);
        SetVisionRange(20f);
        SetPackDistance(2.5f);
    }

    public override void Gore()
    {
        for (int i = 0; i < 7; i++)
            GetBloodMS().SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y, transform.position.z + Random.Range(-1.5f, 1.5f)), 1);
        if (!GetDeathMuted())
            GetAudioS().PlayBloodExplosion();
        for (int i = 49; i < 58; i++)
            GetGoreMS().SpawnGore(i, transform.position);
    }

    public override void AddedMethods()
    {
        if (!strideCooldown)
        {
            strideCooldown = true;
            SetBaseSpeed(100f);
            StartCoroutine("EndStrideCooldown");
        }
    }

    IEnumerator EndStrideCooldown()
    {
        yield return new WaitForSeconds(0.2f);
        SetBaseSpeed(0.1f);
        yield return new WaitForSeconds(STRIDE_DELAY);
        strideCooldown = false;
    }

    List<GameObject> tempList = new List<GameObject>();

    public override void PackReaction()
    {
        tempList = GetSameNearby();
        foreach (GameObject o in tempList)
        {
            print("the tag: " + o.transform.tag);
            o.transform.parent.GetComponent<StriderScript>().DetectedPlayer();
            o.transform.parent.GetComponent<StriderScript>().Aggroed();
        }
    }

}
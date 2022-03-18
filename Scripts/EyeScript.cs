using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeScript : MonsterScript
{
    float laserDelay;
    float timePassed;
    public LaserScript laserS;
    const float LASER_DAMAGE = 3f;

    public override void assignTraits()
    {
        laserDelay = 1f;
        timePassed = 0;

        SetHealth(5f);
        SetDamage(3f);
        SetAfterlag(0.75f);
        SetStartup(0f);
        SetBaseSpeed(5f);
        SetSightDistance(10f);
        SetLightSensitivity(2f);
        SetHearingDistance(6f);
        SetHitstun(1f);
        SetStrikeDistance(1.5f);
        SetVisionRange(30f);
        SetPackDistance(2f);
    }

    public override void AddedMethods()
    {
        if (GetPlayerSeen())
            CheckLaser();
        else
            timePassed = 0;
    }

    void CheckLaser()
    {
        if (laserDelay < timePassed)
        {
            SetRotationLock(true);
            timePassed = -3f; // longer delay after an actual shot is got off
            laserS.Shoot(transform, LASER_DAMAGE, GetRotation());
            SetBaseSpeed(0);
            CallSetState(2);
            SetAnimationLock(true);

            StartCoroutine("ResetSpeed");
        }
        else
            timePassed += Time.deltaTime;
    }

    IEnumerator ResetSpeed()
    {
        yield return new WaitForSeconds(2f);
        SetBaseSpeed(5f);
        SetRotationLock(false);
        SetAnimationLock(false);
        if (GetAggro())
            CallSetState(3);
        else
            CallSetState(1);
    }

    public override void Gore()
    {
        for (int i = 0; i < 6; i++)
            GetBloodMS().SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y, transform.position.z + Random.Range(-1.5f, 1.5f)));
        for (int i = 0; i < 2; i++)
            GetBloodMS().SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y, transform.position.z + Random.Range(-1.5f, 1.5f)), 1);
        if (!GetDeathMuted())
            GetAudioS().PlayBloodExplosion();
        for (int i = 58; i < 70; i++)
            GetGoreMS().SpawnGore(i, transform.position);
        for (int i = 62; i < 65; i++)
            GetGoreMS().SpawnGore(i, transform.position);
    }

    List<GameObject> tempList = new List<GameObject>();

    public override void PackReaction()
    {
        tempList = GetSameNearby();
        foreach (GameObject o in tempList)
        {
            o.transform.parent.GetComponent<EyeScript>().DetectedPlayer();
            o.transform.parent.GetComponent<EyeScript>().Aggroed();
        }
    }
}

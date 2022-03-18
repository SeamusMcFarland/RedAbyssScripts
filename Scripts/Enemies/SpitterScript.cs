using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitterScript : MonsterScript
{

    private float gooDelay;
    private float timePassed;
    public GooScript gooS;


    public override void assignTraits()
    {
        RandomizeGooTime();
        timePassed = 0f;
        SetHealth(10f);
        SetDamage(4f);
        SetAfterlag(1f);
        SetStartup(0f);
        SetBaseSpeed(5f);
        SetSightDistance(6.5f);
        SetLightSensitivity(1.5f);
        SetHearingDistance(6f);
        SetHitstun(0.01f);
        SetStrikeDistance(1f);
        SetVisionRange(179f);
    }

    public override void Gore()
    {
        for (int i = 0; i < 5; i++)
            GetBloodMS().SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y, transform.position.z + Random.Range(-1.5f, 1.5f)));
        if (!GetDeathMuted())
            GetAudioS().PlayBloodExplosion();
        for (int i = 14; i < 28; i++)
            GetGoreMS().SpawnGore(i, transform.position);
    }

    public override void AddedMethods()
    {
        if(GetPlayerSeen())
            CheckGoo();
    }

    void CheckGoo()
    {
        if (gooDelay < timePassed)
        {
            timePassed = 0;
            gooS.Shoot(transform.position);
        }
        else
            timePassed += Time.deltaTime;
    }

    void RandomizeGooTime()
    {
        gooDelay = Random.Range(0.8f, 1.6f);
    }

}

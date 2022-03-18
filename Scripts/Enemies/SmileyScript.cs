using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmileyScript : MonsterScript
{
    float attackDelay;
    float teleportDelay;
    float timePassed;
    float timePassed2;
    public SmileyStrikeScript smileyStrikeS;
    const float STRIKE_DAMAGE = 3f;
    const float STRIKE_STARTUP = 0.12f;
    GameObject[] warpPoints;
    List<GameObject> potentialWarpPoints = new List<GameObject>();
    GameObject chosenWarpPoint;
    Color savedColor;
    MeshRenderer mr;
    bool teleporting;
    int flashNum;
    bool initialAggro;

    const float MAX_TELEPORT_DISTANCE = 10f;

    public override void assignTraits()
    {
        initialAggro = false;
        mr = GetAnimationObject().GetComponent<MeshRenderer>();
        savedColor = mr.material.color;
        warpPoints = GameObject.FindGameObjectsWithTag("warppoint");
        attackDelay = 0.5f;
        teleportDelay = Random.Range(1f, 4f);
        timePassed = 0;
        timePassed2 = 0;

        SetHealth(3f);
        SetDamage(2f);
        SetAfterlag(0.60f);
        SetStartup(0f);
        SetBaseSpeed(0f);
        SetSightDistance(14f);
        SetLightSensitivity(1f);
        SetHearingDistance(15f);
        SetHitstun(0.2f);
        SetStrikeDistance(1.5f);
        SetVisionRange(20f);
        SetPackDistance(5f);
    }

    public override void AddedMethods()
    {
        if(initialAggro)
            CheckSpecial();
        else
        {
            if (GetPlayerSeen())
                initialAggro = true;
        }
    }

    void CheckSpecial()
    {
        if (teleportDelay < timePassed2)
        {
            teleportDelay = teleportDelay = Random.Range(1f, 4f);
            timePassed2 = 0;
            Teleport();
        }
        else
            timePassed2 += Time.deltaTime;

        if (attackDelay < timePassed && !teleporting && !GetDead() && PlayerWithinSight())
        {
            SetRotationLock(true);
            timePassed = -0.5f; // longer delay after an actual shot is got off
            smileyStrikeS.SmileyStrike(STRIKE_DAMAGE, STRIKE_STARTUP);
            CallSetState(2);
            SetAnimationLock(true);

            StartCoroutine("ResetAnimation");
        }
        else
            timePassed += Time.deltaTime;
    }

    private void Teleport()
    {
        teleporting = true;
        SetRotationLock(true);
        flashNum = 0;
        StartCoroutine("Flashing");
    }

    IEnumerator Flashing()
    {
        mr.material.color = new Color(Random.Range(0, 255f), Random.Range(0, 255f), Random.Range(0, 255f));
        yield return new WaitForSeconds(0.01f);
        if (flashNum < 10)
        {
            flashNum++;
            StartCoroutine("Flashing");
        }
        else
        {
            flashNum = 0;
            mr.material.color = savedColor;
            FinishTeleport();
        }
        transform.position = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y, transform.position.z + Random.Range(-1f, 1f));
    }

    private void FinishTeleport()
    {
        initialAggro = false;
        SetRotationLock(false);

        ChooseWarpPoint();

        GetNavMeshAgent().Warp(new Vector3(chosenWarpPoint.transform.position.x, transform.position.y, chosenWarpPoint.transform.position.z));
        StartCoroutine("EndTeleportVariable");
    }

    private void ChooseWarpPoint()
    {
        potentialWarpPoints.Clear();
        for (int i = 0; i < warpPoints.Length; i++)
            if (Get2DDistance(warpPoints[i].transform, transform) < MAX_TELEPORT_DISTANCE && Get2DDistance(warpPoints[i].transform, transform) > 0.3f) // 0.3 makes sure not going to same spot
                potentialWarpPoints.Add(warpPoints[i]);

        if (potentialWarpPoints.Count < 1) // checks if empty and adds a completely random point if so
            potentialWarpPoints.Add(warpPoints[Random.Range(0, warpPoints.Length)]);

        chosenWarpPoint = potentialWarpPoints[(int)Random.Range(0, potentialWarpPoints.Count)];
    }

    IEnumerator EndTeleportVariable()
    {
        yield return new WaitForSeconds(0.3f);
        teleporting = false;
    }

    IEnumerator ResetAnimation()
    {
        yield return new WaitForSeconds(2f);
        SetRotationLock(false);
        SetAnimationLock(false);
        if (GetAggro())
            CallSetState(3);
        else
            CallSetState(1);
    }

    public override void Gore()
    {
        for (int i = 0; i < 5; i++)
            GetBloodMS().SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y, transform.position.z + Random.Range(-1.5f, 1.5f)));
        if (!GetDeathMuted())
            GetAudioS().PlayBloodExplosion();
        for (int i = 79; i < 87; i++)
            GetGoreMS().SpawnGore(i, transform.position);
    }

    List<GameObject> tempList = new List<GameObject>();

    public override void PackReaction()
    {
        tempList = GetSameNearby();
        foreach (GameObject o in tempList)
        {
            o.transform.parent.GetComponent<SmileyScript>().DetectedPlayer();
            o.transform.parent.GetComponent<SmileyScript>().Aggroed();
        }
    }
}

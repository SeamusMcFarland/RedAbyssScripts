using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultistScript : MonsterScript
{
    const float SHOOT_DISTANCE = 6f;
    const float SHOOT_DELAY = 0.1f;
    const int SHOTS_IN_BURST = 4;
    const float BULLET_DAMAGE = 1f;

    bool shooting;

    RaycastHit hit2;
    BreakableWallScript tempBreakableWallScript;
    public SparkEffectScript sparkES;
    List<GameObject> explodedBarrels = new List<GameObject>();

    bool explodingBarrel;
    bool noisyWall;
    bool noisyExplosion;
    bool noisyBox;
    bool noisyAcid;
    bool noisyBreakableWall;
    bool noisyRubble;
    bool barrelMatch;

    public Material[] materialVar1;
    public Material[] materialVar2;

    public override void assignTraits()
    {
        if (Random.value < 0.33f)
            SkinVariation(1);
        else if (Random.value < 0.5f)
            SkinVariation(2);

        shooting = false;

        SetHealth(4f);
        SetDamage(2f);
        SetAfterlag(0.60f);
        SetStartup(0f);
        SetBaseSpeed(9f);
        SetSightDistance(7f);
        SetLightSensitivity(2f);
        SetHearingDistance(8f);
        SetHitstun(0.2f);
        SetStrikeDistance(0.1f); //essentially disables striking
        SetVisionRange(20f);
        SetPackDistance(8f);
    }

    private void SkinVariation(int v)
    {
        if (v == 1)
            GetAnimatorScript().aMat = materialVar1;
        else if (v == 2)
            GetAnimatorScript().aMat = materialVar2;
    }

    public override void AddedMethods()
    {
        if (GetHealth() > 1f)
        {
            if (!shooting && !GetLocked() && GetPlayerSeen() && Get2DDistance(transform, GetPlayer().transform) < SHOOT_DISTANCE && !GetPlayerScript().GetPlayerDead()) // shooting
                StartCoroutine("DelayShootGun", SHOTS_IN_BURST);
        }
        else // wounded state
        {
            SetAnimationLock(true);
            SetRotationLock(true);
            SetSpeedDisabled(true);
            GetNavMeshAgent().velocity = new Vector3(0, 0, 0);
            GetNavMeshAgent().speed = 0;
            CallSetState(4);
        }
    }

    IEnumerator DelayShootGun(int numTimes)
    {
        if (GetHealth() > 1f)
        {
            SetSpeedDisabled(true);
            SetAnimationLock(true);
            shooting = true;
            CheckDirection();
            SetRotationLock(true);
            GetNavMeshAgent().speed = 0;
            GetNavMeshAgent().velocity = new Vector3(0, 0, 0);
            yield return new WaitForSeconds(SHOOT_DELAY);
            if (!GetLocked() && GetPlayerSeen() && Get2DDistance(transform, GetPlayer().transform) < SHOOT_DISTANCE && numTimes > 0)
            {
                CallSetState(2);

                ShootGun();
                numTimes--;
                StartCoroutine("DelayShootGun", numTimes);
            }
            else
            {
                StartCoroutine("EndShootingState");
                GetNavMeshAgent().speed = 9f;
            }
        }
    }

    IEnumerator EndShootingState()
    {
        yield return new WaitForSeconds(SHOOT_DELAY / 2f);
        shooting = false;
        SetSpeedDisabled(false);
        SetAnimationLock(false);
        if (!GetLocked() && GetPlayerSeen()) // maybe aggroed?
            CallSetState(3);
        if (GetHealth() > 1f)
        {
            SetAnimationLock(false);
            SetRotationLock(false);
        }
    }

    private void ShootGun()
    {
        Bullet();
    }

    public override void Gore()
    {
        for (int i = 0; i < 5; i++)
            GetBloodMS().SpawnBlood(new Vector3(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y, transform.position.z + Random.Range(-1.5f, 1.5f)));
        if(!GetDeathMuted())
            GetAudioS().PlayBloodExplosion();
        for (int i = 96; i < 101; i++)
            GetGoreMS().SpawnGore(i, transform.position);
    }

    List<GameObject> tempList = new List<GameObject>();

    public override void PackReaction()
    {
        tempList = GetSameNearby();
        foreach (GameObject o in tempList)
        {
            o.transform.parent.GetComponent<CultistScript>().DetectedPlayer();
            o.transform.parent.GetComponent<CultistScript>().Aggroed();
        }
    }

    private void Bullet() // handles individual bullets from gunshots
    {
        GetAudioS().PlaySilencer();

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), transform.right, out hit2))
        {
            if (hit2.transform.CompareTag("wall"))
            {
                sparkES.Spark(hit2.point); // angleMod is used for iterator over spark effects

                if (noisyWall == false) // prevents multi-shots from causing it to play multiple times at once.
                    GetAudioS().PlayBulletClang();
                noisyWall = true;
            }
            else if (hit2.transform.CompareTag("player"))
            {
                GetPlayerScript().Hit(BULLET_DAMAGE);
            }
            else if (hit2.transform.CompareTag("explosivebarrel"))
            {
                explodingBarrel = true;
                StartCoroutine("EndExplodingBarrel");

                if (noisyWall == false) // prevents multi-shots from causing it to play multiple times at once.
                    GetAudioS().PlayBulletClang();
                if (noisyExplosion == false)
                {
                    for (int i = 0; i < 2; i++) //volume increase
                        GetAudioS().PlayExplosion();
                }
                noisyWall = true;
                noisyExplosion = true;
                barrelMatch = false;
                foreach (GameObject e in explodedBarrels) // assures not to blow up the same explosive barrel twice
                {
                    if (e == hit2.transform.gameObject)
                        barrelMatch = true;
                }
                if (barrelMatch == false)
                {
                    explodedBarrels.Add(hit2.transform.gameObject);
                    hit2.collider.gameObject.GetComponent<ExplosiveBarrelScript>().Explode();
                }
            }
            else if (hit2.transform.CompareTag("box"))
            {
                if (noisyBox == false) // prevents multi-shots from causing it to play multiple times at once.
                    GetAudioS().PlayBoxBreak();
                noisyBox = true;
                hit2.collider.gameObject.GetComponent<BoxScript>().BreakBox();
            }
            else if (hit2.transform.CompareTag("acidcontainer"))
            {
                if (noisyAcid == false) // prevents multi-shots from causing it to play multiple times at once.
                    GetAudioS().PlayShatter();
                noisyAcid = true;
                hit2.collider.gameObject.GetComponent<AcidContainerScript>().PunctureContainer();
            }
            else if (hit2.transform.CompareTag("breakablewall"))
            {
                tempBreakableWallScript = hit2.transform.GetComponent<BreakableWallScript>();
                tempBreakableWallScript.BreakWall();
                if (tempBreakableWallScript.GetDestroyed() && !noisyBox)
                {
                    GetAudioS().PlayDestroyWall();
                    noisyBox = true;
                }
                if (!noisyBreakableWall)
                {
                    GetAudioS().PlayDamageWall();
                    noisyBreakableWall = true;
                }
            }
            else if (hit2.transform.CompareTag("rubble"))
            {
                Vector3 tempVector = new Vector3(hit2.transform.position.x - transform.position.x, Random.Range(3f, 5f), hit2.transform.position.z - transform.position.z);
                tempVector = new Vector3(14f * tempVector.x / Get2DDistance(transform, hit2.transform), tempVector.y, 14f * tempVector.z / Get2DDistance(transform, hit2.transform));
                hit2.transform.GetComponent<RubbleScript>().Struck(tempVector);
                if (!noisyRubble)
                {
                    GetAudioS().PlayCrumbleHit();
                    noisyRubble = true;
                }
            }


        }
        StartCoroutine("EndNoisy");
    }
    IEnumerator EndNoisy() // turns off light produced by gunshot after 0.05 seconds
    {
        yield return new WaitForSeconds(0.05f);
        noisyWall = false;
        noisyExplosion = false;
        noisyBox = false;
        noisyAcid = false;
        noisyBreakableWall = false;
        noisyRubble = false;
    }

    IEnumerator EndExplodingBarrel()
    {
        yield return new WaitForSeconds(0.2f);
        explodingBarrel = false;
    }
}

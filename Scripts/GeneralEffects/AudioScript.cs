using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    public AudioClip bulletClang;
    public AudioClip gunshot;
    public AudioClip reload;
    public AudioClip bulletStrike;
    public AudioClip bloodExplosion;
    public AudioClip gash;
    public AudioClip strike;
    public AudioClip openDoor;
    public AudioClip acid;
    public AudioClip acidMiss;
    public AudioClip explosion;
    public AudioClip glassBreak;
    public AudioClip glassBreak2;
    public AudioClip glassBreak3;
    public AudioClip burn;
    public AudioClip boxBreak;
    public AudioClip shatter;
    public AudioClip fireball;
    public AudioClip burst;
    public AudioClip zap;
    public AudioClip glitch;
    public AudioClip switchWeapons;
    public AudioClip shotgun;
    public AudioClip bottlePickup;
    public AudioClip click1;
    public AudioClip click2;
    public AudioClip playerWalk;
    public AudioClip metalClick;
    public AudioClip trapClose;
    public AudioClip wood1;
    public AudioClip wood2;
    public AudioClip clanking;
    public AudioClip metalBang;
    public AudioClip paperGlitch;
    public AudioClip snap;
    public AudioClip swoosh;
    public AudioClip wetStep;
    public AudioClip lightSwitch;
    public AudioClip deepBreath;
    public AudioClip metal;
    public AudioClip heavyEquip;
    public AudioClip burnClip;
    public AudioClip fizzClip;
    public AudioClip explosion2;
    public AudioClip silencer;
    public AudioClip crumbleHit;
    public AudioClip moan;
    public AudioClip flash;
    public AudioClip shove;
    public AudioClip elasticHit;
    public AudioClip elasticBreak;
    public AudioClip groundStep;
    public AudioClip fireExplosion;
    public AudioClip spit;
    public AudioClip puddleStep;
    public AudioClip smallerBloodExplosion;
    public AudioClip multiply;
    public AudioClip waterSplash;
    public AudioClip briefEarRing;
    public AudioClip laserBurst;
    public AudioClip knifeSlice;
    public AudioClip crackle;
    public AudioClip music;

    int deathSoundNoisy;

    const float VOLUME_RATE = 0.01f;

    public int audioNum;
    AudioSource MusicSource;

    // Start is called before the first frame update
    void Start()
    {
        MusicSource = this.GetComponent<AudioSource>();
        //MusicSource.clip = bulletClang;
    }

    private void Update()
    {
        deathSoundNoisy = 0;
        if (audioNum == 4 || audioNum == 5)
            MusicSource.volume -= VOLUME_RATE;
        else if(audioNum == 7)
            MusicSource.volume -= VOLUME_RATE / 5f;
    }

    public void IncreaseVolume()
    {
        MusicSource.volume += VOLUME_RATE * 5f;
    }

    public int GetAudioNum()
    {
        return audioNum;
    }

    public void PlayBulletClang()
    {
        MusicSource.PlayOneShot(bulletClang);
    }

    public void PlayGunshot()
    {
        MusicSource.PlayOneShot(gunshot);
    }

    public void PlayReload()
    {
        MusicSource.PlayOneShot(reload);
    }

    public void PlayBulletStrike()
    {
        MusicSource.PlayOneShot(bulletStrike);
    }

    public void PlayBloodExplosion()
    {
        if (deathSoundNoisy < 2)
        {
            deathSoundNoisy++;
            MusicSource.PlayOneShot(bloodExplosion);
        }
    }

    public void PlayGash()
    {
        MusicSource.PlayOneShot(gash);
    }

    public void PlayStrike()
    {
        MusicSource.PlayOneShot(strike);
    }

    public void PlayOpenDoor()
    {
        MusicSource.PlayOneShot(openDoor);
    }

    public void PlayAcid()
    {
        MusicSource.PlayOneShot(acid);
    }

    public void PlayAcidMiss()
    {
        MusicSource.PlayOneShot(acidMiss);
    }

    public void PlayExplosion()
    {
        MusicSource.PlayOneShot(explosion);
    }

    public void PlayGlassBreak()
    {
        MusicSource.PlayOneShot(glassBreak);
    }
    public void PlayGlassBreak2()
    {
        MusicSource.PlayOneShot(glassBreak2);
    }

    public void PlayGlassBreak3()
    {
        MusicSource.PlayOneShot(glassBreak3);
    }

    public void PlayBurn()
    {
        MusicSource.PlayOneShot(burn);
    }

    public void PlayBoxBreak()
    {
        MusicSource.PlayOneShot(boxBreak);
    }

    public void PlayShatter()
    {
        MusicSource.PlayOneShot(shatter);
    }

    public void PlayFireball()
    {
        MusicSource.PlayOneShot(fireball);
    }

    public void PlayBurst()
    {
        MusicSource.PlayOneShot(burst);
    }

    public void PlayZap()
    {
        MusicSource.PlayOneShot(zap);
    }

    public void PlayGlitch()
    {
        MusicSource.PlayOneShot(glitch);
    }

    public void PlaySwitchWeapons()
    {
        MusicSource.PlayOneShot(switchWeapons);
    }
    
    public void PlayShotgun()
    {
        MusicSource.PlayOneShot(shotgun);
    }

    public void PlayBottlePickup()
    {
        MusicSource.PlayOneShot(bottlePickup);
    }

    public void PlayClick1()
    {
        MusicSource.PlayOneShot(click1);
    }

    public void PlayClick2()
    {
        MusicSource.PlayOneShot(click2);
    }

    public void PlayPlayerWalk()
    {
        MusicSource.PlayOneShot(playerWalk);
    }

    public void PlayMetalClick()
    {
        MusicSource.PlayOneShot(metalClick);
    }

    public void PlayTrapClose()
    {
        MusicSource.PlayOneShot(trapClose);
    }

    public void PlayDestroyWall()
    {
        MusicSource.PlayOneShot(boxBreak);
    }

    public void PlayDamageWall()
    {
        MusicSource.PlayOneShot(wood1);
    }

    public void PlayTrapPickup()
    {
        MusicSource.PlayOneShot(clanking);
    }

    public void PlayMetalBang()
    {
        MusicSource.PlayOneShot(metalBang);
    }

    public void PlayPaperGlitch()
    {
        MusicSource.PlayOneShot(paperGlitch);
    }

    public void PlaySnap()
    {
        MusicSource.PlayOneShot(snap);
    }

    public void PlaySwoosh()
    {
        MusicSource.PlayOneShot(swoosh);
    }

    public void PlayWetStep()
    {
        MusicSource.PlayOneShot(wetStep);
    }

    public void PlayLightSwitch()
    {
        MusicSource.PlayOneShot(lightSwitch);
    }
    public void PlayDeepBreath()
    {
        MusicSource.PlayOneShot(deepBreath);
    }

    public void PlayMetal()
    {
        MusicSource.PlayOneShot(metal);
    }

    public void PlayHeavyEquip()
    {
        MusicSource.PlayOneShot(heavyEquip);
    }

    public void PlayBurnClip()
    {
        MusicSource.PlayOneShot(burnClip);
    }

    public void PlayFizzClip()
    {
        MusicSource.PlayOneShot(fizzClip);
    }
    public void PlayExplosion2()
    {
        MusicSource.PlayOneShot(explosion2);
    }

    public void PlaySilencer()
    {
        MusicSource.PlayOneShot(silencer);
    }

    public void PlayCrumbleHit()
    {
        MusicSource.PlayOneShot(crumbleHit);
    }

    public void PlayMoan()
    {
        MusicSource.PlayOneShot(moan);
    }

    public void PlayFlash()
    {
        MusicSource.PlayOneShot(flash);
    }

    public void PlayShove()
    {
        MusicSource.PlayOneShot(shove);
    }

    public void PlayElasticHit()
    {
        MusicSource.PlayOneShot(elasticHit);
    }

    public void PlaySpit()
    {
        MusicSource.PlayOneShot(spit);
    }

    public void PlayElasticBreak()
    {
        MusicSource.PlayOneShot(elasticBreak);
    }

    public void PlayGroundStep()
    {
        MusicSource.PlayOneShot(groundStep);
    }

    public void PlayFireExplosion()
    {
        MusicSource.PlayOneShot(fireExplosion);
    }

    public void PlayPuddleStep()
    {
        MusicSource.PlayOneShot(puddleStep);
    }

    public void PlaySmallerBloodExplosion()
    {
        MusicSource.PlayOneShot(smallerBloodExplosion);
    }

    public void PlayMultiply()
    {
        MusicSource.PlayOneShot(multiply);
    }

    public void PlayWaterSplash()
    {
        MusicSource.PlayOneShot(waterSplash);
    }

    public void PlayBriefEarRing()
    {
        MusicSource.PlayOneShot(briefEarRing);
    }

    public void PlayLaserBurst()
    {
        MusicSource.PlayOneShot(laserBurst);
    }

    public void PlayKnifeSlice()
    {
        MusicSource.PlayOneShot(knifeSlice);
    }

    public void PlayCrackle()
    {
        MusicSource.PlayOneShot(crackle);
    }


    public void PlayBackgroundMusic()
    {
        MusicSource.PlayOneShot(music);
    }
    

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public AudioSource Audio;
    public ParticleSystem gunParticle;
    public AudioClip[] gunSounds;
    Dictionary<string, AudioClip> gunDic;

    int shotCount = 0;
    int overhit = 10;
    int coolingTime = 3;
    int overheatTime = 5;
    bool isOverheat = false;
    bool cooling = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // 딕셔너리 초기화
        gunDic = new Dictionary<string, AudioClip>();
        for (int i = 0; i < gunSounds.Length; i++)
        {
            // 주의: 오디오클립 이름이 "ShotgunSFX", "OverheatSFX"여야 함
            gunDic.Add(gunSounds[i].name, gunSounds[i]);
        }
    }
    public void ShotLogic()

    {

        if (isOverheat)

        {

            //딸깍소리출력

            return;

        }

        if (shotCount >= overhit)

        {

            isOverheat = true;

            Audio.PlayOneShot(gunDic["OverheatSFX"]);

            StartCoroutine(Overheat());

            return;

        }
        if (isOverheat)
            return;
        gunParticle.Play();

        Audio.PlayOneShot(gunDic["ShotgunSFX"]);

        shotCount++;
        StartCoroutine(Cooldown());

    }



    IEnumerator Overheat()

    {

        yield return new WaitForSeconds(overheatTime);

        isOverheat = false;

    }



    IEnumerator Cooldown()

    {

        while (true)
        {
            if (shotCount >= 1 && !cooling)
            {
                cooling = true;

                yield return new WaitForSeconds(coolingTime);

                shotCount--;

                cooling = false;
            }

        }

    }
}

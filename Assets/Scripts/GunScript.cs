using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : BaseGun
{
    /*    public AudioSource audioSource;
        public ParticleSystem[] gunParticles; //0 발사 1, 2 과열 
        public AudioClip[] gunSounds;  => 부모 클래스가 담당 */
    Dictionary<string, AudioClip> gunDic;

    int shotCount = 0;  
    [SerializeField]  int overhit = 10;
    [SerializeField]  int coolingTime = 3;
    [SerializeField] int overheatTime = 5;
    [SerializeField]  bool isOverheat = false;
    bool cooling = false;


    private void Start()
    {
        damage = 8;
        // 딕셔너리 초기화
        gunDic = new Dictionary<string, AudioClip>();
        for (int i = 0; i < gunSounds.Length; i++)
        {
            // 주의: 오디오클립 이름이 "ShotgunSFX", "OverheatSFX"여야 함
            gunDic.Add(gunSounds[i].name, gunSounds[i]);
        }
        StartCoroutine(Cooldown());
    }

    public override void TryFire()
    {
        if (isOverheat)

        {
            Debug.Log("과열로 인해 발사 안됨");
            return;
        }

        if (shotCount >= overhit && !isOverheat)

        {
            Debug.Log("과열됨");
            isOverheat = true;
            gunParticles[1].Play();
            gunParticles[2].Play();
            audioSource.PlayOneShot(gunDic["OverheatSFX"]);

            StartCoroutine(Overheat());

            return;

        }

        gunParticles[0].Play();

        audioSource.PlayOneShot(gunDic["ShotgunSFX"]);

        shotCount++;
        Debug.Log($"{shotCount} 샷 카운트 추가");
    }



    IEnumerator Overheat()

    {

        yield return new WaitForSeconds(overheatTime);
        audioSource.PlayOneShot(gunDic["OverheatShotSFX"]); ;
        isOverheat = false;
        shotCount = 0;

    }

    IEnumerator Cooldown()

    {

        while (true)
        {
            yield return null;
            if (shotCount >= 1)
            {
                cooling = true;

                yield return new WaitForSeconds(coolingTime);

                shotCount--;

                cooling = false;
                
            }

        }

    }
}

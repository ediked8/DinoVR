using MikeNspired.XRIStarterKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GunScript : BaseGun
{
    /*    public AudioSource audioSource;
        public ParticleSystem[] gunParticles; //0 발사 1, 2 과열 
        public AudioClip[] gunSounds;  => 부모 클래스가 담당 */



    Dictionary<string, AudioClip> gunDic;
    public ParticleSystem ps; // 인스펙터에서 할당

    List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    // 충돌 정보를 담을 리스트 (매번 new 하면 메모리 낭비니 재사용)

    int shotCount = 0;  
    [SerializeField]  int overhit = 10;
    [SerializeField]  int coolingTime = 3;
    [SerializeField] int overheatTime = 5;
    [SerializeField]  bool isOverheat = false;
    bool cooling = false;

    


    private void Start()
    {

        // 만약 ps가 할당 안됐으면 컴포넌트 가져오기
        if (ps == null) Debug.Log("발사 파티클 등록 필요");
        

        audioSource = GetComponentInChildren<AudioSource>();

        damage = 8; // 데미지 초기화.
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

    // 파티클이 오브젝트(other)에 부딪혔을 때 유니티가 자동 호출
    public override void OnParticleHit(GameObject other, ParticleSystem senderParticle)
    {
        // 1. 공룡인지 확인
        CustomDinoController target = other.GetComponent<CustomDinoController>();

        if (target != null)
        {
            // 2. 전달받은 파티클 시스템(senderParticle)을 이용해 충돌 이벤트 가져오기
            int numCollisionEvents = senderParticle.GetCollisionEvents(other, collisionEvents);

            if (numCollisionEvents > 0)
            {
                // 저장해둔 데미지 계산 (발사 시점의 스택 데미지 적용)
                int totalDamage = numCollisionEvents * damage; // 혹은 그냥 damage 변수

                Debug.Log($"파티클 {numCollisionEvents}개 명중! 총 데미지: {totalDamage}");

                // 데미지 적용
                target.TakeDamage(totalDamage);
            }
        }
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

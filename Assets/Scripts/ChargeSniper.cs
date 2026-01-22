using MikeNspired.XRIStarterKit;
using System.Collections.Generic;
using UnityEngine;


public class ChargeSniper : BaseGun
{
    [Header("Charge Settings")]
    [SerializeField] float maxChargeTime = 2.0f;
    [SerializeField] int maxChargeStack = 2;
    [SerializeField] int currentChargeStack = 0;
    

    [Header("Damage Settings")]
    [SerializeField] int damagePerStack = 50; // 스택당 추가 데미지
    private int currentShotDamage = 0; // 발사 시점에 확정된 데미지를 저장할 변수

    [Header("AudioSetting")]
    // 차징 사운드 클립 별도 저장 (딕셔너리에서 꺼내 쓰기 번거로움 방지)
    Dictionary<string, AudioClip> gunDic;
    private AudioClip chargingClip;
    private AudioClip chargeCompleteClip;

    public XRKnob knob;
    public float currentvalue; // 디버깅용 public

    // 파티클 충돌 감지용 리스트 (최적화)
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();


    private void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>(); 
        damage = 100;
        // 1. 딕셔너리 초기화 (기존 코드 유지)
        gunDic = new Dictionary<string, AudioClip>();
        for (int i = 0; i < gunSounds.Length; i++)
        {
            gunDic.Add(gunSounds[i].name, gunSounds[i]);
        }

        // 2. 자주 쓰는 클립 미리 캐싱 (최적화 및 코드 간결화)
        if (gunDic.ContainsKey("ChargingSFX"))
            chargingClip = gunDic["ChargingSFX"];

        if (gunDic.ContainsKey("ChargeCompleteSFX")) // 풀차지 소리가 따로 있다면
            chargeCompleteClip = gunDic["ChargeCompleteSFX"];

        // 3. AudioSource 초기 세팅
        audioSource.clip = chargingClip;
        audioSource.loop = false; // 계속 돌리는 동안 끊기지 않게
        audioSource.playOnAwake = false;
    }

    // XR Knob의 OnValueChange 이벤트에 연결하세요.
    public void CheckLeverValue()
    {
        currentvalue = knob.Value;
        Debug.Log("레버 작동확인");
      
        if (currentvalue > 0.01f)
        {
            if (!audioSource.isPlaying && chargingClip != null)
            {
                audioSource.clip = chargingClip;
                audioSource.Play();
            }
        }

        // 2. 차징 완료 (Value가 1 도달)
        if (currentvalue >= 0.99f) // 부동소수점 오차 고려하여 0.99 이상 체크
        {
            CompleteOneStack();
        }
    }

    void CompleteOneStack()
    {
        // 스택 로직
        if (currentChargeStack < maxChargeStack)
        {
            currentChargeStack++;
            Debug.Log($"차지 스택: {currentChargeStack}");
            gunParticles[1].Play(); //CharagedVFX

            // 스택 쌓이는 소리 (딸깍!) - 이건 OneShot으로 겹쳐 들리게
            if (chargeCompleteClip != null)
                audioSource.PlayOneShot(chargingClip);
            else
                Debug.Log("소리없음");
                // 임시
        }
        else
        {
            gunParticles[2].gameObject.SetActive(true);
            AudioSource.PlayClipAtPoint(chargeCompleteClip, transform.position);
            Debug.Log("풀차지 상태입니다.");
        }
        
        // 중요: 값 초기화
        knob.Value = 0;
        currentvalue = 0;


    }

    public override void TryFire()
    {
        // 발사 로직 구현... (스택 소모 등)
        if (currentChargeStack > 0)
        {
            Debug.Log($"스택 {currentChargeStack} 소모하여 발사!");

            // 기본 데미지 + (스택 * 추가데미지)
            currentShotDamage = damage + (currentChargeStack * damagePerStack);

            audioSource.PlayOneShot(gunDic["ChargeShot"]);
            
            //ShotVFX 실행
            gunParticles[0].Play();

            //충전관련 VFX 종료;
            gunParticles[1].Stop();
            gunParticles[2].gameObject.SetActive(false);

            // 스택 및 레버 초기화
            currentChargeStack = 0;
            knob.Value = 0;
            currentvalue = 0;
            
        }
    }

    // [핵심] 파티클이 무언가에 부딪혔을 때 호출됨
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
                int totalDamage = numCollisionEvents * currentShotDamage; // 혹은 그냥 damage 변수

                Debug.Log($"파티클 {numCollisionEvents}개 명중! 총 데미지: {totalDamage}");

                // 데미지 적용
                target.TakeDamage(totalDamage);
            }
        }
    }
}

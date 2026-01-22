using MikeNspired.XRIStarterKit;
using System.Collections.Generic;
using UnityEngine;


public class ChargeSniper : BaseGun
{

    [SerializeField] float maxChargeTime = 2.0f;
    [SerializeField] int maxChargeStack = 2;
    [SerializeField] int currentChargeStack = 0;
    Dictionary<string, AudioClip> gunDic;

    // 차징 사운드 클립 별도 저장 (딕셔너리에서 꺼내 쓰기 번거로움 방지)
    private AudioClip chargingClip;
    private AudioClip chargeCompleteClip;

    public XRKnob knob;
    public float currentvalue; // 디버깅용 public

    // 이전에 값이 변했는지 체크하기 위한 변수
    private float lastKnobValue = -1f;

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
            //스택에 따른 소리 추가해야함.
            currentChargeStack = 0;
            audioSource.PlayOneShot(gunDic["ChargeShot"]);
            //ShotVFX 실행
            gunParticles[0].Play();
            gunParticles[1].Stop();//ChargedVFX 종료;
            gunParticles[2].gameObject.SetActive(false);
            knob.Value = 0;
            currentvalue = 0;
            
        }
    }

    public override void TryDamage(CustomDinoController dino)
    {
        if (dino != null)
        {
            dino.currentHealth -= damage;
        }
    }
}
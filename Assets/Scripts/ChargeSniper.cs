using MikeNspired.XRIStarterKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class ChargeSniper : BaseGun
{
    [Header("Charge Settings")]
    [SerializeField] float maxChargeTime = 2.0f;
    [SerializeField] int maxChargeStack = 3;
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
    private bool isLeverReady;
    public Transform[] Gears;

    // 파티클 충돌 감지용 리스트 (최적화)
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    [Header("Spring Back Settings")]
    [SerializeField] float returnSpeed = 5.0f; // 되돌아가는 속도
    private bool isGrabbed = false; // 현재 잡고 있는지 여부
    private Coroutine returnRoutine;
    

  
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
          for(int i = 0; i < Gears.Length; i++ )
          {
              // 1. 현재 각도를 오일러(도, Degree) 단위로 가져옵니다.
              Vector3 currentRot = Gears[i].localEulerAngles;
              Vector3 currentPos = Gears[i].position;
              // 2. Y축 각도를 Knob 값에 비례하게 설정합니다.
              // 예: Knob가 0~1일 때 기어는 0~360도 회전
              // (더 많이 돌리고 싶으면 360 대신 720, 1080 등을 곱하세요)
              currentRot.x = currentvalue * 360f;

              // 3. 변경된 각도를 다시 넣어줍니다.
              Gears[i].localEulerAngles = currentRot;
              Gears[i].position = currentPos;

          }


        // 1. 소리 재생 로직 (기존과 동일하되, 너무 작은 값 변화는 무시)
        if (currentvalue > 0.05f)
        {
            // ... 소리 재생 코드 ...
        }

        // 2. [핵심 수정] 차징 완료 로직 (Value가 1 도달)
        // 조건: 값이 0.99 이상이고 + "준비된 상태(isLeverReady)"여야 함
        if (currentvalue >= 0.95f && isLeverReady)
        {
            CompleteOneStack();

            // [중요] 스택을 쌓았으니, 레버가 돌아올 때까지 잠금!
            isLeverReady = false;
        }

        // 3. [추가] 레버 복귀 확인 (재장전 준비)
        // 레버 값이 거의 0으로 돌아왔을 때 다시 장전 가능하게 풀어줌
        if (currentvalue <= 0.05f && !isLeverReady)
        {
            isLeverReady = true;
            Debug.Log("레버 복귀 완료 - 재장전 가능");
        }
    }

    void CompleteOneStack()
    {
        // 스택 로직
        if (currentChargeStack < maxChargeStack-1)
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
            currentChargeStack++;
            gunParticles[2].gameObject.SetActive(true);
            AudioSource.PlayClipAtPoint(chargeCompleteClip, transform.position);
            Debug.Log($"{currentChargeStack}스택 풀차지 상태입니다.");
        }
        
        // 중요: 값 초기화
        //knob.Value = 0;
        //currentvalue = 0;


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

    public void OnHandleGrab()
    {
        isGrabbed = true;

        // 되돌아가는 중이었다면 멈춤 (플레이어가 다시 잡았으므로)
        if (returnRoutine != null) StopCoroutine(returnRoutine);
    }

    // Select Exited (놓았을 때) 이벤트에 연결
    public void OnHandleRelease()
    {
        isGrabbed = false;

        // 손을 놓으면 0으로 되돌아가는 코루틴 시작
        if (gameObject.activeInHierarchy) // 비활성화 상태 에러 방지
            returnRoutine = StartCoroutine(SpringBackRoutine());
    }

    // ---------------------------------------------------------
    // [2] 0으로 부드럽게 되돌리는 코루틴
    // ---------------------------------------------------------
    IEnumerator SpringBackRoutine()
    {
        // 값이 0보다 큰 동안 계속 실행
        while (knob.Value > 0.01f)
        {
            // 플레이어가 그 사이에 다시 잡았다면 즉시 중단
            if (isGrabbed) yield break;

            // 값을 부드럽게 0으로 줄임 (Mathf.Lerp)
            knob.Value = Mathf.Lerp(knob.Value, 0f, Time.deltaTime * returnSpeed);

            // Value가 바뀌었으니 CheckLeverValue 로직이 돌 수 있음
            // (필요하다면 여기서 수동으로 오디오 처리 등을 할 수도 있음)

            yield return null; // 한 프레임 대기
        }

        // 확실하게 0으로 마무리
        knob.Value = 0f;
    }
}

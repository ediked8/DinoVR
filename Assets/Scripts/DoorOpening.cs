using UnityEngine;

public class TriggerEventController : MonoBehaviour
{
    [Header("Targeting")]
    public string targetTag = "Player";
    public Animator targetAnimator;
    public string triggerParameterName = "Open";

    [Header("Options")]
    public bool playOnce = false;
    [Tooltip("중복 실행 방지 시간 (초). 문이 열리는 애니메이션 시간만큼 설정하세요.")]
    public float cooldownTime = 6.0f; // 쿨타임 추가

    private bool _hasPlayed = false;
    private float _lastTriggerTime = -5.0f; // 마지막 실행 시간 저장

    private void Awake()
    {
        if (targetAnimator == null) targetAnimator = GetComponent<Animator>();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning($"{gameObject.name}: 안정적인 감지를 위해 Kinematic Rigidbody 추가를 권장합니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playOnce && _hasPlayed) return;

        // 쿨타임 체크: 마지막 실행 후 일정 시간이 안 지났으면 무시
        if (Time.time - _lastTriggerTime < cooldownTime) return;

        if (other.CompareTag(targetTag))
        {
            // [중요] 애니메이터가 이미 '전환 중(Transition)'이라면 중복 실행 방지
            if (targetAnimator != null && targetAnimator.IsInTransition(0))
            {
                return;
            }

            ExecuteTrigger();
        }
    }

    private void ExecuteTrigger()
    {
        if (targetAnimator != null)
        {
            targetAnimator.SetTrigger(triggerParameterName);
            _hasPlayed = true;
            _lastTriggerTime = Time.time; // 실행 시간 기록

            Debug.Log($"[VR Trigger] {gameObject.name} activated by Player.");
        }
    }
}
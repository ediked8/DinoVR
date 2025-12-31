using UnityEngine;

public class DimoHunter : MonoBehaviour
{
    [Header("타겟 설정")]
    public Transform player;       // 추격할 플레이어 (Capsule)
    public float sightRange = 15f; // 플레이어를 발견하는 거리
    public float attackRange = 2f; // 공격 인정 거리

    [Header("방 설정 (중요)")]
    // 큐브 맵의 크기 (이 범위를 벗어나지 않음)
    public Vector3 roomSize = new Vector3(10, 8, 10);
    public Vector3 roomCenter = new Vector3(0, 0, 0);

    [Header("비행 속도")]
    public float patrolSpeed = 4f; // 순찰 속도
    public float chaseSpeed = 8f;  // 추격 속도 (더 빠름!)
    public float turnSpeed = 2f;   // 회전 속도

    // 내부 변수
    private Animator anim;
    private Vector3 currentTarget; // 현재 목적지
    private bool isChasing = false; // 지금 쫓고 있는가?

    void Start()
    {
        anim = GetComponent<Animator>();

        // 공룡에게 "너는 지금 날고 있다"라고 인식시킴 (Dimo.cs 애니메이션 호환)
        anim.SetBool("OnGround", false);
        anim.SetInteger("Move", 2); // 2번이 비행 모드

        GetNewPatrolPoint();
    }

    void Update()
    {
        // 1. 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 2. 상태 결정 (추격 vs 순찰)
        if (distanceToPlayer < sightRange)
        {
            isChasing = true;
            currentTarget = player.position; // 목표를 플레이어로 고정
        }
        else
        {
            isChasing = false;
            // 순찰 중 목표에 도착했으면 새로운 목표 설정
            if (Vector3.Distance(transform.position, currentTarget) < 2.0f)
            {
                GetNewPatrolPoint();
            }
        }

        // 3. 이동 로직 실행
        MoveToTarget();
        UpdateAnimationState();
    }

    void MoveToTarget()
    {
        // 목표 방향 계산
        Vector3 direction = currentTarget - transform.position;
        direction.Normalize();

        // 회전 (부드럽게)
        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSpeed);
        }

        // 이동 (추격 중이면 더 빠르게)
        float currentSpeed = isChasing ? chaseSpeed : patrolSpeed;
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    void UpdateAnimationState()
    {
        // Dimo.cs의 애니메이션 파라미터 제어
        // 공격 거리 안에 들어오면 공격 모션
        float distance = Vector3.Distance(transform.position, currentTarget);
        if (isChasing && distance < attackRange)
        {
            anim.SetBool("Attack", true);
        }
        else
        {
            anim.SetBool("Attack", false);
        }

        // 위/아래를 쳐다보는 각도(Pitch) 설정
        Vector3 targetDir = currentTarget - transform.position;
        float pitch = -targetDir.y * 0.1f; // 높이 차이에 따라 고개 숙임/들기
        anim.SetFloat("Pitch", Mathf.Clamp(pitch, -1f, 1f));
    }

    void GetNewPatrolPoint()
    {
        // 방 안의 랜덤한 좌표 생성
        float x = Random.Range(-roomSize.x / 2, roomSize.x / 2);
        float y = Random.Range(-roomSize.y / 2, roomSize.y / 2);
        float z = Random.Range(-roomSize.z / 2, roomSize.z / 2);

        currentTarget = roomCenter + new Vector3(x, y, z);
    }

    // 에디터에서 방 크기를 눈으로 보여줌
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(roomCenter, roomSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange); // 감지 범위 표시
    }
}
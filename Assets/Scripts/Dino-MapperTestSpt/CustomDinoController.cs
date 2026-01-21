using UnityEngine;
using System.Collections; // ★ 코루틴 사용을 위해 필요

public class CustomDinoController : MonoBehaviour
{
    [Header("공룡 스펙 설정")]
    public float moveSpeed = 0f; // ★ 시작 속도를 0으로 설정
    public float rotSpeed = 2.0f;
    public float attackRange = 2.5f;
    public int maxHealth = 100;
    public float distance;

    // ... (기존 변수 동일)
    private Animator anim;
    private Transform player;
    private bool isDead = false;
    public int currentHealth;

    private readonly string ANI_MOVE = "Move";
    private readonly string ANI_ATTACK = "Attack";
    private readonly string ANI_ONGROUND = "OnGround";
    private readonly string ANI_IDLE = "Idle";

    // ■ [새로 추가] 버튼 클릭 시 외부에서 호출할 함수
    public void StartChasingAfterDelay()
    {
        // 중복 실행 방지 및 사망 시 실행 방지
        if (isDead) return;

        Debug.Log($"공룡 비활성화: 속도 {moveSpeed}으로 변경됨");
        StartCoroutine(MoveRoutine());
    }

    // ■ [새로 추가] 3초 대기 로직
    private IEnumerator MoveRoutine()
    {
        yield return new WaitForSeconds(3.0f); // 3초 대기

        moveSpeed = 6.0f; // 속도 변경
        Debug.Log($"공룡 활성화: 속도 {moveSpeed}으로 변경됨");
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        currentHealth = maxHealth;
        anim.SetBool(ANI_ONGROUND, true);

        // (미리 배치된 공룡 뿐만 아니라, 나중에 소환될 때도 안전하게 처리됨)
        if (DinoUnlockButton.isGameStarted == false)
        {
            moveSpeed = 0f;
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        distance = Vector3.Distance(transform.position, player.position);

        // moveSpeed가 0일 때는 가만히 Idle 애니메이션 유지하도록 체크
        if (moveSpeed <= 0)
        {
            anim.SetInteger(ANI_MOVE, 0);
            return;
        }

        if (distance <= attackRange)
        {
            DoAttack();
        }
        else
        {
            DoChase();
        }
    }

    // ... (DoChase, DoAttack, TakeDamage, Die 함수는 기존과 동일)
    void DoChase()
    {
        anim.SetBool(ANI_ATTACK, false);
        anim.SetInteger(ANI_MOVE, 2);
        anim.SetInteger(ANI_IDLE, 0);

        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotSpeed);
        }

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    void DoAttack()
    {
        anim.SetInteger(ANI_MOVE, 0);
        anim.SetBool(ANI_ATTACK, true);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;
        anim.SetBool(ANI_ATTACK, false);
        anim.SetInteger(ANI_MOVE, 0);
        anim.SetInteger(ANI_IDLE, -1);
        Destroy(gameObject, 3.0f);
    }
}
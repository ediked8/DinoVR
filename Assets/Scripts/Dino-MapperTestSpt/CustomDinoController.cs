using UnityEngine;

public class CustomDinoController : MonoBehaviour
{
    [Header("공룡 스펙 설정")]
    public float moveSpeed = 6.0f;
    public float rotSpeed = 2.0f;
    public float attackRange = 2.5f;
    public int maxHealth = 100;
    public float distance;

    // 내부 변수
    private Animator anim;
    private Transform player;
    private bool isDead = false;
    private int currentHealth;

    // 쥬라기 팩 애니메이션 파라미터 이름
    private readonly string ANI_MOVE = "Move";
    private readonly string ANI_ATTACK = "Attack";
    private readonly string ANI_ONGROUND = "OnGround";
    private readonly string ANI_IDLE = "Idle"; // ★ 추가: 사망 처리를 위한 파라미터

    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        currentHealth = maxHealth;

        // Dimo(익룡) 땅에 붙이기
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

        if (distance <= attackRange)
        {
            DoAttack();
        }
        else
        {
            DoChase();
        }
    }

    void DoChase()
    {
        anim.SetBool(ANI_ATTACK, false);
        anim.SetInteger(ANI_MOVE, 2);
        anim.SetInteger(ANI_IDLE, 0); // ★ 이동 중엔 Idle 상태 0으로

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

        // ★ 수정된 부분: 이름으로 찾는 CrossFade 대신 파라미터 제어 사용
        anim.SetBool(ANI_ATTACK, false); // 공격 끄기
        anim.SetInteger(ANI_MOVE, 0);    // 이동 끄기
        anim.SetInteger(ANI_IDLE, -1);   // ★ 핵심: Idle을 -1로 하면 사망 애니메이션 재생됨

        Destroy(gameObject, 3.0f);
    }
}
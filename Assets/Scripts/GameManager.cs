using UnityEngine;

// 플레이어 혹은 게임 매니저에 붙여주세요
public class GameManager : MonoBehaviour
{
    // 싱글톤으로 만들어서 어디서든 접근 쉽게 설정 (선택사항)
    public static GameManager Instance;

    [Header("플레이어 설정")]
    public int maxHealth = 100;
    public int currentHealth;
    public Transform player;

    private void Awake()
    {
        Instance = this;
        currentHealth = maxHealth;
    }

    // 데미지를 받는 함수
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"플레이어 피격! 데미지: {damageAmount}, 남은 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("플레이어 사망");
        // 여기에 게임 오버 로직 추가
    }
}
using UnityEngine;

public class LifeTimer : MonoBehaviour
{
    [Header("생존 시간 설정 (초)")]
    public float lifeTime = 10f;

    void Start()
    {
        // 설정된 시간(lifeTime)이 지나면 TimeOut 함수 실행
        Invoke("TimeOut", lifeTime);
    }

    void TimeOut()
    {
        Debug.Log($"{gameObject.name} 수명 종료.");

        // 1. 이 공룡의 컨트롤러를 찾습니다.
        CustomDinoController dino = GetComponent<CustomDinoController>();

        if (dino != null)
        {
            // 2. 그냥 Destroy() 하지 말고, '엄청난 데미지'를 줘서 죽게 만듭니다.
            // 그래야 아까 고친 '사망 애니메이션'이 재생되고 나서 사라집니다.
            dino.TakeDamage(99999);
        }
        else
        {
            // 만약 공룡이 아니라 그냥 나무상자 같은 거라면 바로 삭제
            Destroy(gameObject);
        }
    }
}
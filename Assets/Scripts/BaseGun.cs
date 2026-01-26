using UnityEngine;


public class BaseGun : MonoBehaviour
{
    // 모든 총에 다 필요한 변수들
    public AudioSource audioSource;
    public ParticleSystem[] gunParticles;
    public AudioClip[] gunSounds;
    public int damage;



    // 자식들이 각자 알아서 구현해야 할 함수 (virtual)
    public virtual void TryFire()
    {
        // 빈 껍데기. 자식들이 채워 넣음.
    }

    // [추가] 자식들이 공통으로 쓸 파티클 충돌 함수 정의
    // virtual 키워드: 자식이 이 내용을 덮어쓸 수 있음(Override)
    public virtual void OnParticleHit(GameObject other, ParticleSystem senderParticle)
    {
        // 부모는 기본적으로 아무것도 안 하거나, 로그만 찍음
        // 자식 스크립트에서 이 부분을 override해서 각자의 로직을 짬
    }
}

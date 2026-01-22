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

    public virtual void TryDamage(CustomDinoController dino)
    {
        // 빈 껍데기. 자식들이 채워 넣음.
    }
}

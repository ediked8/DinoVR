using UnityEngine;

public class RelayPS : MonoBehaviour
{
    // 본체 스크립트를 연결할 변수
    public BaseGun mainGunScript;
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        // 만약 인스펙터에서 연결 안 했으면 부모에서 자동으로 찾기 시도
        if (mainGunScript == null)
            mainGunScript = GetComponentInParent<BaseGun>();
       
    }

    private void OnParticleCollision(GameObject other)
    {
        // 충돌이 일어나면 본체 스크립트의 함수를 대신 호출해줌!
        // 이때, "누가(ps)" 부딪혔는지 정보도 같이 넘겨줍니다.
        if (mainGunScript != null)
        {
            mainGunScript.OnParticleHit(other, ps);
        }
    }
}
using System.Collections;
using UnityEngine;

public class DinoUnlockButton : MonoBehaviour
{
    [Header("설정")]
    public float startSpeed = 6.0f; // 버튼 누르면 적용될 속도 (기본 6)

    // 게임 시작 여부를 전역에서 알 수 있게 static으로 선언 (선택 사항)
    public static bool isGameStarted = false;
    public float delayT = 6f;
    public CustomDinoController[] allDinos;

    void Start()
    {
        isGameStarted = false;

        // 1. 게임 시작 시: 모든 공룡의 속도를 0으로 만듦
        ChangeDinoSpeed(0);
    }

    // ★ 이 함수를 버튼 이벤트에 연결하세요!
    public void OnPressButton()
    {

        if (isGameStarted) return; // 이미 눌렀으면 무시


        StartCoroutine(DinoStart());
    }

    // 씬에 있는 모든 공룡을 찾아서 속도를 바꾸는 함수
    void ChangeDinoSpeed(float speed)
    {
        // 현재 씬에 있는 모든 CustomDinoController를 찾아옵니다.
        
        foreach (var dino in allDinos)
        {
            dino.moveSpeed = speed; // 공룡 속도 변경

            // (선택) 만약 '추격 중' 상태라면 애니메이션도 갱신해주기 위해
            // 공룡이 멈출 땐 Idle 상태로 보이게 처리할 수도 있습니다.
            // 하지만 moveSpeed만 0이 되어도 제자리 뛰기를 하므로 자연스럽습니다.
        }
    }

    IEnumerator DinoStart()
    {
       yield return new WaitForSeconds(delayT);

        Debug.Log("게임 시작! 공룡들이 움직입니다.");
        isGameStarted = true;

        // 2. 버튼 클릭 시: 모든 공룡의 속도를 원래대로(6) 복구
        ChangeDinoSpeed(startSpeed);
    }
}

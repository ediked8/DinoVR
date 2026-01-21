using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("이동할 씬의 이름을 정확하게 입력하세요.")]
    public string nextSceneName;

    [Header("Time Settings")]
    [Tooltip("몇 초 뒤에 씬을 변경할지 설정하세요.")]
    public float delayTime = 3.0f;

    [Header("Trigger Settings")]
    [Tooltip("체크하면 트리거(충돌) 시 씬이 변경됩니다.")]
    public bool useTrigger = true;
    [Tooltip("트리거를 발동시킬 오브젝트의 태그")]
    public string targetTag = "Player";

    [Tooltip("체크하면 게임 시작 시 자동으로 카운트다운을 시작합니다.")]
    public bool autoStart = false; // 트리거용으로 쓸 때는 false가 좋습니다.

    private bool isLoading = false;

    void Start()
    {
        if (autoStart)
        {
            TriggerSceneChange();
        }
    }

    // 1. 트리거 진입 시 호출 (새로 추가된 부분)
    private void OnTriggerEnter(Collider other)
    {
        // 이미 로딩 중이거나 트리거 모드가 꺼져있으면 무시
        if (isLoading || !useTrigger) return;

        // 부딪힌 물체의 태그가 Player일 때만 실행
        if (other.CompareTag(targetTag))
        {
            TriggerSceneChange();
        }
    }

    // 2. 외부 호출 및 내부 실행용
    public void TriggerSceneChange()
    {
        if (isLoading) return;
        StartCoroutine(LoadSceneAfterDelay());
    }

    IEnumerator LoadSceneAfterDelay()
    {
        isLoading = true;
        Debug.Log(delayTime + "초 뒤에 " + nextSceneName + " 씬으로 이동합니다.");

        yield return new WaitForSeconds(delayTime);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("이동할 씬의 이름이 비어있습니다!");
            isLoading = false; // 이름이 비어있어 실패한 경우 다시 시도 가능하게 리셋
        }
    }
}
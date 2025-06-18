using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectClick : MonoBehaviour
{
    public string sceneName; // 유니티 인스펙터에서 씬 이름을 설정할 수 있음

    // 씬을 전환하는 메서드
    public void LoadScene()
    {
        // 씬을 로드하는 코드 (예시)
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}

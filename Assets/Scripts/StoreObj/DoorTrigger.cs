using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요

public class DoorTrigger : MonoBehaviour
{
    public string sceneToLoad; // 이동할 씬 이름

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 캐릭터가 "Player" 태그를 가지고 있을 경우
        {
            Debug.Log("문과 충돌 발생! 씬 이동 시작.");
            SceneManager.LoadScene(sceneToLoad); // 지정한 씬으로 이동
        }
    }
}
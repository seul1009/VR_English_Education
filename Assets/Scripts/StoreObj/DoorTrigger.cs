using UnityEngine;
using UnityEngine.SceneManagement; // �� ��ȯ�� ���� �ʿ�

public class DoorTrigger : MonoBehaviour
{
    public string sceneToLoad; // �̵��� �� �̸�

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ĳ���Ͱ� "Player" �±׸� ������ ���� ���
        {
            Debug.Log("���� �浹 �߻�! �� �̵� ����.");
            SceneManager.LoadScene(sceneToLoad); // ������ ������ �̵�
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectClick : MonoBehaviour
{
    public string sceneName; // ����Ƽ �ν����Ϳ��� �� �̸��� ������ �� ����

    // ���� ��ȯ�ϴ� �޼���
    public void LoadScene()
    {
        // ���� �ε��ϴ� �ڵ� (����)
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}

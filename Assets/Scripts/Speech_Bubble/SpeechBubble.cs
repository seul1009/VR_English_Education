using UnityEngine;
using WebSocketSharp;
using TMPro;
using System.Threading.Tasks;

public class SpeechBubble: MonoBehaviour
{
    public static SpeechBubble Instance { get; private set; } // �̱��� �ν��Ͻ�

    public TextMeshProUGUI uiText;         
    public RectTransform backgroundRect;  
    public float maxWidth = 500f;

    private string receivedMessage = "";
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetReceivedMessage(string message)
    {
        receivedMessage = message;
    }


    // Unity�� Update �Լ����� ������ �޽����� ó��
    void Update()
    {
        if (!string.IsNullOrEmpty(receivedMessage))
        {
            // �ؽ�Ʈ UI�� ������Ʈ
            uiText.text = receivedMessage;

            // �ؽ�Ʈ ũ�⿡ ���� ��� �̹��� ũ�� ����
            backgroundRect.sizeDelta = new Vector2(
                Mathf.Min(uiText.preferredWidth + 20f, maxWidth + 20f),
                uiText.preferredHeight - 20f/* + 20f*/
            );

            receivedMessage = "";  // ������Ʈ �� �ʱ�ȭ
        }
    }
}

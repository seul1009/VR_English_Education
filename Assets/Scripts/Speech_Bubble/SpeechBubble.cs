using UnityEngine;
using WebSocketSharp;
using TMPro;
using System.Threading.Tasks;

public class SpeechBubble: MonoBehaviour
{
    public static SpeechBubble Instance { get; private set; } // 싱글톤 인스턴스

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


    // Unity의 Update 함수에서 서버의 메시지를 처리
    void Update()
    {
        if (!string.IsNullOrEmpty(receivedMessage))
        {
            // 텍스트 UI를 업데이트
            uiText.text = receivedMessage;

            // 텍스트 크기에 맞춰 배경 이미지 크기 조정
            backgroundRect.sizeDelta = new Vector2(
                Mathf.Min(uiText.preferredWidth + 20f, maxWidth + 20f),
                uiText.preferredHeight - 20f/* + 20f*/
            );

            receivedMessage = "";  // 업데이트 후 초기화
        }
    }
}

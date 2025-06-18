//using System;
//using UnityEngine;
//using UnityEngine.Networking;
//using SocketIOClient;
//using System.Collections;

//public class DBWebSocket : MonoBehaviour
//{
//    private SocketIO client;
//    public string apiKey;
//    private AudioSource audioSource;
//    public static string inputData;

//    private void Start()
//    {
//        audioSource = GetComponent<AudioSource>();
//        client = new SocketIO("ws://localhost:5000");

//        client.OnConnected += async (sender, e) =>
//        {
//            Debug.Log("서버와 연결됨");
//        };

//        // 메시지 수신 처리
//        client.On("new_data", response =>
//        {
//            string receivedData = response.GetValue<string>();
//            Debug.Log("서버로부터 메시지 수신: " + receivedData);
//            ProcessNewData(receivedData);
//        });

//        // 오류 처리
//        client.On("error", response =>
//        {
//            Debug.LogError("Socket.IO 오류: " + response.GetValue<string>());
//        });

//        // 연결 종료 처리
//        client.On("disconnect", response =>
//        {
//            Debug.Log("Socket.IO 연결 종료");
//        });

//        // 서버와 연결 시작
//        client.ConnectAsync().ContinueWith(task =>
//        {
//            if (task.IsCompleted)
//            {
//                Debug.Log("서버와의 연결이 완료되었습니다.");
//            }
//            else
//            {
//                Debug.LogError("서버와의 연결 시도에 실패했습니다.");
//            }
//        });
//    }

//    private void OnDestroy()
//    {
//        // 애플리케이션 종료 시 Socket.IO 클라이언트 연결 종료
//        if (client != null)
//        {
//            client.DisconnectAsync();
//        }
//    }

//    void ProcessNewData(string data)
//    {
//        inputData = data;
//        Debug.Log("새 데이터: " + data);

//        StartCoroutine(SendTextToSpeechRequest(data));
//    }

//    // TTS 요청을 보내는 함수
//    IEnumerator SendTextToSpeechRequest(string inputText)
//    {
//        if (string.IsNullOrEmpty(inputText))
//        {
//            Debug.LogError("입력 텍스트가 비어 있습니다.");
//            yield break;
//        }

//        var requestData = new TextToSpeechRequest
//        {
//            input = new Input { text = inputText },
//            voice = new Voice
//            {
//                languageCode = "en-US",
//                name = "en-US-Wavenet-A"
//            },
//            audioConfig = new AudioConfig
//            {
//                audioEncoding = "LINEAR16",
//                speakingRate = 1.0f
//            }
//        };

//        string jsonRequest = JsonUtility.ToJson(requestData);
//        string url = "https://texttospeech.googleapis.com/v1/text:synthesize?key=" + apiKey;

//        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
//        {
//            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequest);
//            request.SetRequestHeader("Content-Type", "application/json");
//            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
//            request.downloadHandler = new DownloadHandlerBuffer();

//            yield return request.SendWebRequest();

//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                string responseText = request.downloadHandler.text;
//                var jsonResponse = JsonUtility.FromJson<TextToSpeechResponse>(responseText);
//                string audioContent = jsonResponse.audioContent;

//                byte[] pcmBytes = Convert.FromBase64String(audioContent);

//                AudioClip audioClip = ConvertToAudioClip(pcmBytes, sampleRate: 22050, channels: 1);

//                audioSource.clip = audioClip;
//                audioSource.Play();
//                Debug.Log("음성 재생");
//            }
//            else
//            {
//                Debug.LogError("음성 변환 실패: " + request.error);
//                Debug.LogError("Response: " + request.downloadHandler.text);
//            }
//        }
//    }

//    // PCM 데이터를 AudioClip으로 변환하는 함수
//    AudioClip ConvertToAudioClip(byte[] pcmBytes, int sampleRate, int channels)
//    {
//        int dataLength = pcmBytes.Length / 2; // 16비트 샘플
//        float[] audioData = new float[dataLength];

//        for (int i = 0; i < dataLength; i++)
//        {
//            short sample = BitConverter.ToInt16(pcmBytes, i * 2);
//            audioData[i] = sample / 32768f; // [-1.0, 1.0]로 변환
//        }

//        AudioClip audioClip = AudioClip.Create("TTS_AudioClip", audioData.Length, channels, sampleRate, false);
//        audioClip.SetData(audioData, 0);

//        return audioClip;
//    }

//    [System.Serializable]
//    public class TextToSpeechResponse
//    {
//        public string audioContent;
//    }

//    [System.Serializable]
//    public class TextToSpeechRequest
//    {
//        public Input input;
//        public Voice voice;
//        public AudioConfig audioConfig;
//    }

//    [System.Serializable]
//    public class Input
//    {
//        public string text;
//    }

//    [System.Serializable]
//    public class Voice
//    {
//        public string languageCode;
//        public string name;
//    }

//    [System.Serializable]
//    public class AudioConfig
//    {
//        public string audioEncoding;
//        public float speakingRate;
//    }
//}

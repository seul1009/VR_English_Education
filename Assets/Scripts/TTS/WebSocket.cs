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
//            Debug.Log("������ �����");
//        };

//        // �޽��� ���� ó��
//        client.On("new_data", response =>
//        {
//            string receivedData = response.GetValue<string>();
//            Debug.Log("�����κ��� �޽��� ����: " + receivedData);
//            ProcessNewData(receivedData);
//        });

//        // ���� ó��
//        client.On("error", response =>
//        {
//            Debug.LogError("Socket.IO ����: " + response.GetValue<string>());
//        });

//        // ���� ���� ó��
//        client.On("disconnect", response =>
//        {
//            Debug.Log("Socket.IO ���� ����");
//        });

//        // ������ ���� ����
//        client.ConnectAsync().ContinueWith(task =>
//        {
//            if (task.IsCompleted)
//            {
//                Debug.Log("�������� ������ �Ϸ�Ǿ����ϴ�.");
//            }
//            else
//            {
//                Debug.LogError("�������� ���� �õ��� �����߽��ϴ�.");
//            }
//        });
//    }

//    private void OnDestroy()
//    {
//        // ���ø����̼� ���� �� Socket.IO Ŭ���̾�Ʈ ���� ����
//        if (client != null)
//        {
//            client.DisconnectAsync();
//        }
//    }

//    void ProcessNewData(string data)
//    {
//        inputData = data;
//        Debug.Log("�� ������: " + data);

//        StartCoroutine(SendTextToSpeechRequest(data));
//    }

//    // TTS ��û�� ������ �Լ�
//    IEnumerator SendTextToSpeechRequest(string inputText)
//    {
//        if (string.IsNullOrEmpty(inputText))
//        {
//            Debug.LogError("�Է� �ؽ�Ʈ�� ��� �ֽ��ϴ�.");
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
//                Debug.Log("���� ���");
//            }
//            else
//            {
//                Debug.LogError("���� ��ȯ ����: " + request.error);
//                Debug.LogError("Response: " + request.downloadHandler.text);
//            }
//        }
//    }

//    // PCM �����͸� AudioClip���� ��ȯ�ϴ� �Լ�
//    AudioClip ConvertToAudioClip(byte[] pcmBytes, int sampleRate, int channels)
//    {
//        int dataLength = pcmBytes.Length / 2; // 16��Ʈ ����
//        float[] audioData = new float[dataLength];

//        for (int i = 0; i < dataLength; i++)
//        {
//            short sample = BitConverter.ToInt16(pcmBytes, i * 2);
//            audioData[i] = sample / 32768f; // [-1.0, 1.0]�� ��ȯ
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

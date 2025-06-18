using System;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using MyNamespace;

public class test : MonoBehaviour
{
    private WebSocket ws;
    public string apiKey;
    private AudioSource audioSource;
    public static string inputData;
    private bool isConnected = false;
    private static readonly Queue<Action> executeOnMainThread = new Queue<Action>();

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ConnectWebSocket();
    }
    void Update()
    {
        if (executeOnMainThread.Count > 0)
        {
            Debug.Log("���� �����忡�� ���� ��!");
            executeOnMainThread.Dequeue().Invoke();
        }
    }

    private void ConnectWebSocket()
    {
        ws = new WebSocket("ws://127.0.0.1:7000");

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket ���� ���� : TTS");
        };


        ws.OnOpen += (sender, e) =>
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                Debug.Log("WebSocket ���� ���� : STT");
            });
        };

        ws.OnMessage += (sender, e) =>
        {
            var response = JsonUtility.FromJson<ResponseData>(e.Data);
            Debug.Log($"����: {response.Response}, �ǵ��: {response.Feedback}");

            UnityMainThreadDispatcher.Enqueue(() => 
            {
                SpeechBubble.Instance.SetReceivedMessage(response.Feedback);
               
            });

            UnityMainThreadDispatcher.EnqueueCoroutine(SendTextToSpeechRequest(response.Response));

        };

        ws.OnError += (sender, e) =>
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                if (e.Exception != null)
                {
                    Debug.LogError("WebSocket ����: " + e.Exception.Message);
                }
                else
                {
                    Debug.LogError("WebSocket ����: " + e.Message);
                }
            });
        };

        ws.OnClose += (sender, e) =>
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                Debug.Log("WebSocket ���� ����");
            });
        };


        ws.Connect();
    }

    private IEnumerator ReconnectWebSocket()
    {
        yield return new WaitForSeconds(5);
        if (!isConnected)
        {
            Debug.Log("WebSocket �翬�� �õ�...");
            ConnectWebSocket();
        }
    }

    private void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
        }
    }


    private IEnumerator SendTextToSpeechRequest(string data)
    {
        var requestData = new TextToSpeechRequest
        {
            input = new Input { text = data },
            voice = new Voice { languageCode = "en-US", name = "en-US-Wavenet-A" },
            audioConfig = new AudioConfig { audioEncoding = "LINEAR16", speakingRate = 1.0f }
        };

        string jsonRequest = JsonConvert.SerializeObject(requestData);
        string url = "https://texttospeech.googleapis.com/v1/text:synthesize?key=" + apiKey;

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequest);
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;

                var jsonResponse = JsonUtility.FromJson<TextToSpeechResponse>(responseText);
                if (string.IsNullOrEmpty(jsonResponse.audioContent))
                {
                    Debug.LogError("audioContent�� �������");
                    yield break;
                }


                byte[] audioContent = Convert.FromBase64String(jsonResponse.audioContent);
                AudioClip audioClip = ConvertToAudioClip(audioContent, sampleRate: 22050, channels: 1);

                audioSource.clip = audioClip;
                audioSource.Play();
                Debug.Log("���� ���");
            }
            else
            {
                Debug.LogError($"���� ��ȯ ����: {request.error}, ����: {request.downloadHandler.text}");
            }
        }
    }

    AudioClip ConvertToAudioClip(byte[] pcmBytes, int sampleRate, int channels)
    {
        int dataLength = pcmBytes.Length / 2;
        float[] audioData = new float[dataLength];

        for (int i = 0; i < dataLength; i++)
        {
            short sample = BitConverter.ToInt16(pcmBytes, i * 2);
            audioData[i] = sample / 32768f;
        }

        AudioClip audioClip = AudioClip.Create("TTS_AudioClip", audioData.Length, channels, sampleRate, false);
        audioClip.SetData(audioData, 0);

        return audioClip;
    }

    [System.Serializable]
    public class ResponseData
    {
        public string Response; 
        public string Feedback;
    }

    [System.Serializable]
    public class TextToSpeechResponse
    {
        public string audioContent;
    }

    [System.Serializable]
    public class TextToSpeechRequest
    {
        public Input input;
        public Voice voice;
        public AudioConfig audioConfig;
    }

    [System.Serializable]
    public class Input
    {
        public string text;
    }

    [System.Serializable]
    public class Voice
    {
        public string languageCode;
        public string name;
    }

    [System.Serializable]
    public class AudioConfig
    {
        public string audioEncoding;
        public float speakingRate;
    }
}

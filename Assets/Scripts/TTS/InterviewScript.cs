using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Interview_Script : MonoBehaviour
{
    public string serverUrl = "http://localhost:5001/check_new_data_i";  // ���� URL
    public string apiKey;
    private AudioSource audioSource;
    public static string inputData;

    private int lastProcessedId = -1;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(CheckNewData());
    }


    IEnumerator CheckNewData()
    {
        while (true)
        {
            yield return StartCoroutine(SendLongPollingRequest());


            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator SendLongPollingRequest()
    {
        string url = $"{serverUrl}?lastId={lastProcessedId}";  // ���� URL�� lastProcessedId�� ���� �Ķ���ͷ� �����ϴ�.

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("���� ����: " + response);

                try
                {
                    ServerResponse serverResponse = JsonUtility.FromJson<ServerResponse>(response);
                    ProcessNewData(serverResponse.employee_response);  // ó�� �Լ� ȣ��
                }
                catch (Exception ex)
                {
                    Debug.LogError("���� �Ľ� ����: " + ex.Message);
                }
            }
            else
            {
                Debug.Log("���� ��û ����: " + request.error);
            }
        }
    }

    // �� �����͸� ó���ϴ� �Լ�
    void ProcessNewData(string data)
    {
        lastProcessedId++;  // ó���� ������ �������� ID�� ������Ʈ
        inputData = data;
        Debug.Log("�� ������: " + data);

        // �� �����͸� �ؽ�Ʈ ���� ��ȯ(TTS) ��û�� �����մϴ�.
        StartCoroutine(SendTextToSpeechRequest(data));
    }

    // TTS ��û�� ������ �Լ�
    IEnumerator SendTextToSpeechRequest(string inputText)
    {
        if (string.IsNullOrEmpty(inputText))
        {
            Debug.LogError("�Է� �ؽ�Ʈ�� ��� �ֽ��ϴ�.");
            yield break;
        }

        var requestData = new TextToSpeechRequest
        {
            input = new Input { text = inputText },
            voice = new Voice
            {
                languageCode = "en-US",
                name = "en-US-Wavenet-A"

            },
            audioConfig = new AudioConfig
            {
                audioEncoding = "LINEAR16",
                speakingRate = 1.0f    // ���� �ӵ�
            }
        };

        string jsonRequest = JsonUtility.ToJson(requestData);
        string url = "https://texttospeech.googleapis.com/v1/text:synthesize?key=" + apiKey;  // TTS API URL

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
                string audioContent = jsonResponse.audioContent;

                byte[] pcmBytes = System.Convert.FromBase64String(audioContent);

                AudioClip audioClip = ConvertToAudioClip(pcmBytes, sampleRate: 22050, channels: 1);

                audioSource.clip = audioClip;
                audioSource.Play();
                Debug.Log("���� ���");
            }
            else
            {
                Debug.LogError("���� ��ȯ ����: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
        }
    }

    // PCM ����Ʈ �����͸� AudioClip���� ��ȯ�ϴ� �Լ�
    AudioClip ConvertToAudioClip(byte[] pcmBytes, int sampleRate, int channels)
    {
        int dataLength = pcmBytes.Length / 2;  // 16��Ʈ ����, 2����Ʈ��
        float[] audioData = new float[dataLength];

        // PCM ����Ʈ �����͸� float �迭�� ��ȯ
        for (int i = 0; i < dataLength; i++)
        {
            short sample = BitConverter.ToInt16(pcmBytes, i * 2);
            audioData[i] = sample / 32768f;  // [-1.0, 1.0] ������ ��ȯ
        }

        // AudioClip ����
        AudioClip audioClip = AudioClip.Create("TTS_AudioClip", audioData.Length, channels, sampleRate, false);
        audioClip.SetData(audioData, 0);

        return audioClip;
    }

    [System.Serializable]
    public class ServerResponse
    {
        public string employee_response;
        public string corrected_sentence;
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
        public float pitch;
        public float volumeGainDb;
    }
}

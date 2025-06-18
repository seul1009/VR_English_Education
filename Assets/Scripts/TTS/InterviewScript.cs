using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Interview_Script : MonoBehaviour
{
    public string serverUrl = "http://localhost:5001/check_new_data_i";  // 서버 URL
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
        string url = $"{serverUrl}?lastId={lastProcessedId}";  // 서버 URL과 lastProcessedId를 쿼리 파라미터로 보냅니다.

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("서버 응답: " + response);

                try
                {
                    ServerResponse serverResponse = JsonUtility.FromJson<ServerResponse>(response);
                    ProcessNewData(serverResponse.employee_response);  // 처리 함수 호출
                }
                catch (Exception ex)
                {
                    Debug.LogError("응답 파싱 오류: " + ex.Message);
                }
            }
            else
            {
                Debug.Log("서버 요청 실패: " + request.error);
            }
        }
    }

    // 새 데이터를 처리하는 함수
    void ProcessNewData(string data)
    {
        lastProcessedId++;  // 처리한 마지막 데이터의 ID를 업데이트
        inputData = data;
        Debug.Log("새 데이터: " + data);

        // 새 데이터를 텍스트 음성 변환(TTS) 요청에 전달합니다.
        StartCoroutine(SendTextToSpeechRequest(data));
    }

    // TTS 요청을 보내는 함수
    IEnumerator SendTextToSpeechRequest(string inputText)
    {
        if (string.IsNullOrEmpty(inputText))
        {
            Debug.LogError("입력 텍스트가 비어 있습니다.");
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
                speakingRate = 1.0f    // 발음 속도
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
                Debug.Log("음성 재생");
            }
            else
            {
                Debug.LogError("음성 변환 실패: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
        }
    }

    // PCM 바이트 데이터를 AudioClip으로 변환하는 함수
    AudioClip ConvertToAudioClip(byte[] pcmBytes, int sampleRate, int channels)
    {
        int dataLength = pcmBytes.Length / 2;  // 16비트 샘플, 2바이트씩
        float[] audioData = new float[dataLength];

        // PCM 바이트 데이터를 float 배열로 변환
        for (int i = 0; i < dataLength; i++)
        {
            short sample = BitConverter.ToInt16(pcmBytes, i * 2);
            audioData[i] = sample / 32768f;  // [-1.0, 1.0] 범위로 변환
        }

        // AudioClip 생성
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

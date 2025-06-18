using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebSocketSharp;
using SimpleJSON; // SimpleJSON ���̺귯�� �߰�
using MySql.Data.MySqlClient; // MySQL ���̺귯�� �߰�

public class SpeechToText : MonoBehaviour
{
    // ���� speechtotext api key
    public string apiKey;

    // Inspector���� ������ �� �ֵ��� public���� ����
    public string server; // Google Cloud SQL ���� IP �ּ�
    public string database;
    public string uid;
    public string password;
    public string wsServer;
    private WebSocket ws;

    private void Start()
    {
        ConnectWebSocket();
    }
    private void ConnectWebSocket()
    {
        ws = new WebSocket(wsServer);
        ws.Connect();
        //ws.Send("hi");
        try
        {
            if (ws != null && ws.IsAlive) 
            {
                Debug.Log("WebSocket�� Ȱ�� �����Դϴ�.: STT");
            }
            else
            {
                Debug.Log("WebSocket ���� ���� : STT");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"WebSocket ���� Ȯ�� �� ���� �߻�: {ex.Message}");
        }

        ws.OnOpen += (sender, e) =>
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                Debug.Log("WebSocket ���� ���� : STT");
            });
        };

        ws.OnMessage += (sender, e) =>
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                Debug.Log("�����κ��� �޽��� ����: " + e.Data);
            });
        };

        ws.OnError += (sender, e) =>
        {
            // WebSocket ���� �߻� �� ���� ������� ����
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
    }
    public IEnumerator SendAudioToGoogle(byte[] audioData)
    {
        if (audioData == null || audioData.Length == 0)
        {
            Debug.LogError("Audio data is null or empty.");
            yield break;
        }

        string url = $"https://speech.googleapis.com/v1/speech:recognize?key={apiKey}";
        string audioBase64 = System.Convert.ToBase64String(audioData);

        if (string.IsNullOrEmpty(audioBase64))
        {
            Debug.LogError("Audio Base64 is empty.");
            yield break;
        }

        var requestData = new SpeechRecognitionRequest
        {
            config = new RecognitionConfig
            {
                encoding = "LINEAR16",
                sampleRateHertz = 44100,
                languageCode = "en-US"
            },
            audio = new RecognitionAudio
            {
                content = audioBase64
            }
        };

        string jsonData = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, jsonData))
        {
            request.method = "POST";
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
            else
            {
                // JSON ���信�� transcript ����
                string responseText = request.downloadHandler.text;
                var jsonResponse = JSON.Parse(responseText); // SimpleJSON���� �Ľ�
                string transcript = jsonResponse["results"][0]["alternatives"][0]["transcript"]; // transcript ����
                Debug.Log("Transcript: " + transcript); // ����� transcript ���

                // transcript�� �����ͺ��̽��� ����
                SaveTranscriptToDatabase(transcript);
                SendData(transcript);
            }
        }
    }
    private void SendData(string message)
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Send(message);
            Debug.Log("������ ������ ����: " + message);
        }
        else
        {
            Debug.LogError("WebSocket ������ Ȱ��ȭ���� �ʾҽ��ϴ�.");
        }
    }

    private void SaveTranscriptToDatabase(string transcript)
    {
        string connectionString = $"Server={server}; Port=3306; Database={database}; Uid={uid}; Pwd={password};";

        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                //Debug.Log("MySQL ���� ����");

                string query = "INSERT INTO input (ans) VALUES (@transcript)";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@transcript", transcript);
                    cmd.ExecuteNonQuery();
                    Debug.Log("������ ���� ����");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("MySQL ���� ����: " + ex.Message);
        }
    }

    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static readonly Queue<Action> executeOnMainThread = new Queue<Action>();

        public static void Enqueue(Action action)
        {
            lock (executeOnMainThread)
            {
                executeOnMainThread.Enqueue(action);
            }
        }

        void Update()
        {
            while (executeOnMainThread.Count > 0)
            {
                executeOnMainThread.Dequeue().Invoke();
            }
        }
    }

    [System.Serializable]
    public class SpeechRecognitionRequest
    {
        public RecognitionConfig config;
        public RecognitionAudio audio;
    }

    [System.Serializable]
    public class RecognitionConfig
    {
        public string encoding;
        public int sampleRateHertz;
        public string languageCode;
    }

    [System.Serializable]
    public class RecognitionAudio
    {
        public string content;
    }
}

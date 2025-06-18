//using System;
//using UnityEngine;
//using UnityEngine.Networking;
//using System.Collections;
//using SimpleJSON; // SimpleJSON 라이브러리 추가
//using MySql.Data.MySqlClient; // MySQL 라이브러리 추가

//public class SpeechToText : MonoBehaviour
//{
//    // 구글 speechtotext api key
//    public string apiKey;

//    // Inspector에서 설정할 수 있도록 public으로 선언
//    public string server; // Google Cloud SQL 공용 IP 주소
//    public string database;
//    public string uid;
//    public string password;

//    public IEnumerator SendAudioToGoogle(byte[] audioData)
//    {
//        if (audioData == null || audioData.Length == 0)
//        {
//            Debug.LogError("Audio data is null or empty.");
//            yield break;
//        }

//        string url = $"https://speech.googleapis.com/v1/speech:recognize?key={apiKey}";
//        string audioBase64 = System.Convert.ToBase64String(audioData);

//        if (string.IsNullOrEmpty(audioBase64))
//        {
//            Debug.LogError("Audio Base64 is empty.");
//            yield break;
//        }

//        var requestData = new SpeechRecognitionRequest
//        {
//            config = new RecognitionConfig
//            {
//                encoding = "LINEAR16",
//                sampleRateHertz = 44100,
//                languageCode = "en-US"
//            },
//            audio = new RecognitionAudio
//            {
//                content = audioBase64
//            }
//        };

//        string jsonData = JsonUtility.ToJson(requestData);

//        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, jsonData))
//        {
//            request.method = "POST";
//            request.SetRequestHeader("Content-Type", "application/json");
//            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
//            request.downloadHandler = new DownloadHandlerBuffer();

//            yield return request.SendWebRequest();

//            if (request.result != UnityWebRequest.Result.Success)
//            {
//                Debug.LogError("Error: " + request.error);
//                Debug.LogError("Response: " + request.downloadHandler.text);
//            }
//            else
//            {
//                // JSON 응답에서 transcript 추출
//                string responseText = request.downloadHandler.text;
//                var jsonResponse = JSON.Parse(responseText); // SimpleJSON으로 파싱
//                string transcript = jsonResponse["results"][0]["alternatives"][0]["transcript"]; // transcript 추출
//                Debug.Log("Transcript: " + transcript); // 추출된 transcript 출력

//                // transcript를 데이터베이스에 저장
//                SaveTranscriptToDatabase(transcript);
//            }
//        }
//    }

//    private void SaveTranscriptToDatabase(string transcript)
//    {
//        string connectionString = $"Server={server}; Port=3306; Database={database}; Uid={uid}; Pwd={password};";

//        try
//        {
//            using (MySqlConnection connection = new MySqlConnection(connectionString))
//            {
//                connection.Open();
//                Debug.Log("MySQL 연결 성공");

//                // 컬럼 이름을 ans로 변경
//                string query = "INSERT INTO input (ans) VALUES (@transcript)";
//                using (MySqlCommand cmd = new MySqlCommand(query, connection))
//                {
//                    cmd.Parameters.AddWithValue("@transcript", transcript);
//                    cmd.ExecuteNonQuery();
//                    Debug.Log("데이터 저장 성공");
//                }
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError("MySQL 연결 실패: " + ex.Message);
//        }
//    }

//    [System.Serializable]
//    public class SpeechRecognitionRequest
//    {
//        public RecognitionConfig config;
//        public RecognitionAudio audio;
//    }

//    [System.Serializable]
//    public class RecognitionConfig
//    {
//        public string encoding;
//        public int sampleRateHertz;
//        public string languageCode;
//    }

//    [System.Serializable]
//    public class RecognitionAudio
//    {
//        public string content;
//    }
//}

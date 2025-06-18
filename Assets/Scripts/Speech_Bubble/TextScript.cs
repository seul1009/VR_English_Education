using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using UnityEditor.MemoryProfiler;
using TMPro;
using System.Collections;

public class TextScript : MonoBehaviour
{
    public string server;
    public string database;
    public string uid;
    public string password;

    public RectTransform backgroundRect;
    private int lastProcessedId = -1;
    public TextMeshProUGUI uiText;
    public float maxWidth;

    void Start()
    {
        StartCoroutine(CheckForNewData());
    }
    void Update()
    {
        // 텍스트 크기와 위치에 맞춰 배경 이미지 크기 조정
        backgroundRect.sizeDelta = new Vector2(
            Mathf.Min(uiText.preferredWidth + 20f, maxWidth + 20f), // 최대 너비 제한
            uiText.preferredHeight + 20f
        );
    }
    IEnumerator CheckForNewData()
    {
        while (true)
        {
            string connStr = $"Server={server}; Port=3306; Database={database}; Uid={uid}; Pwd={password};";
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string query = "SELECT id, text FROM correction ORDER BY id DESC LIMIT 1;";
                using (MySqlCommand command = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int currentId = reader.GetInt32(0);
                        string ans = reader.GetString(1);
                        if (lastProcessedId == -1)
                        {
                            lastProcessedId = currentId;

                        }
                        else if (currentId > lastProcessedId)
                        {
                            lastProcessedId = currentId;
                            uiText.text = ans;

                        }
                        else
                        {
                            Debug.Log("새 데이터 없음");
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("DB 연결 오류: " + e.Message);
            }
            finally
            {
                conn.Close();
            }

            // 5초마다 확인
            yield return new WaitForSeconds(5f);
        }
    }
}
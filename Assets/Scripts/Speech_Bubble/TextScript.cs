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
        // �ؽ�Ʈ ũ��� ��ġ�� ���� ��� �̹��� ũ�� ����
        backgroundRect.sizeDelta = new Vector2(
            Mathf.Min(uiText.preferredWidth + 20f, maxWidth + 20f), // �ִ� �ʺ� ����
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
                            Debug.Log("�� ������ ����");
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("DB ���� ����: " + e.Message);
            }
            finally
            {
                conn.Close();
            }

            // 5�ʸ��� Ȯ��
            yield return new WaitForSeconds(5f);
        }
    }
}
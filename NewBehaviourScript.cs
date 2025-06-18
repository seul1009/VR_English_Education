using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using System;
using TMPro;
using Unity.VisualScripting;
using Newtonsoft.Json.Linq;  // JSON 데이터 파싱을 위해 추가


public class NewBehaviourScript : MonoBehaviour
{
    public SocketIOUnity socket;

    void Start()
    {
        var uri = new Uri("http://127.0.0.1:3001");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            // git README의 경우 Query를 쓰던데 저는 안써도 통신이 되서 뺐습니다.
            //Query = new Dictionary<string, string>
            //{
            //    { "token", "UNITY" },
            //},

            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        socket.Connect();
        //연결합니다.


        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("연결");
        };
        socket.OnDisconnected += (sender, e) => { Debug.Log("disconnect: " + e); };

        socket.On("0", (data) =>
        {
            Debug.Log(data);
        });

        socket.On("3", (data) =>
        {
            Debug.Log(data);
        });
    }

}
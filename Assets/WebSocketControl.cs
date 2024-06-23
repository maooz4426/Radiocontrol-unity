using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WebSocketControl : MonoBehaviour
{
    private WebSocket ws;

    private Queue commnadQueue;

    [SerializeField] private string URL = "ws://localhost:8000/ws/control/";
    [SerializeField] private float speed = 2;
    void Start()
    {
        commnadQueue = Queue.Synchronized(new Queue());
        
        //websocketのインスタンス生成
        ws = new WebSocket(URL);
        ws.OnOpen += (sender, e) => Debug.Log("WebSocket Open");
        ws.OnMessage += (sender, e) => {
            // Debug.Log("WebSocket Message: " + e.Data);
            try
            {
                //パースする
                CommandData data = JsonUtility.FromJson<CommandData>(e.Data);
                commnadQueue.Enqueue(data.command);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error handling message: " + ex.Message);
            }
        };
        ws.OnError += (sender, e) => Debug.Log("WebSocket Error: " + e.Message);
        ws.OnClose += (sender, e) => Debug.Log("WebSocket Close");
        ws.OnClose += (sender, e) => Debug.LogWarning($"WebSocket Close: {e.Code}, {e.Reason}");

        ws.Connect();
    }

    void Update()
    {
        lock (commnadQueue.SyncRoot)
        {
            while (commnadQueue.Count > 0)
            {
                
                string message = commnadQueue.Dequeue().ToString();
                Debug.Log(message);
                HandleCommand(message);
            }
        }
        
    }

    void HandleCommand(string message)
    {
        Debug.Log("Received message: " + message);
        try
        {
            // CommandData data = JsonUtility.FromJson<CommandData>(message);

            if (message == "forward")
            {
                transform.Translate(Vector3.forward * speed);
                Debug.Log("f");
            }
            else if (message == "backward")
            {
                transform.Translate(Vector3.back * speed);
                Debug.Log("b");
            }else if (message == "rightward")
            {
                transform.Translate(Vector3.right * speed);
                Debug.Log("r");
            }else if (message == "leftward")
            {
                transform.Translate(Vector3.left * speed);
                Debug.Log("l");
            }
            
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing message: " + ex.Message);
            Debug.LogError("Message content: " + message);
        }
    }

    void OnDestroy()
    {
        ws.Close();
    }

    
    [System.Serializable] 
    private class CommandData
    {
        public string command;
    }
}
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;
using ArmInteractionCore;
using System.Collections.Generic;
using System.Linq;

namespace WebSocketLayer
{
    public class WebSocketManager: IWebSocketManager
    {
        private JObject receivedData;

        //const string URL = "ws://192.168.244.197:8765/";
        const string localURL = "ws://127.0.0.1:8765/";

        private readonly WebSocket WebSocket;
        private readonly ICoroutineExecutor coroutineExecutor;

        public event Action<IEnumerable<Vector3>> OnPoseReceived;

        public WebSocketManager(string webSocketURL = localURL)
        {
            WebSocket = new WebSocket(webSocketURL);
            WebSocket.OnMessage += OnMessage;
            WebSocket.OnMessage += TriggerCubeSpawn;

            cubeDirectory = new Dictionary<int, Vector3>();
        }

        private Dictionary<int, Vector3> cubeDirectory;

        private void TriggerCubeSpawn(object sender, MessageEventArgs e)
        {
            cubeDirectory.Clear();

            var o = JObject.Parse(e.Data);
            JToken token;

            if (o.TryGetValue("scene_objects", out token))
            {
                var array = JArray.Parse(token.ToString());
                foreach(JObject item in array)
                {
                    int.TryParse(item.GetValue("id").ToString(), out int index);
                    var blah = item.GetValue("position").Values <float>().ToArray();
                    var position = new Vector3(blah[0], blah[1], blah[2]);
                    cubeDirectory[index] = position;
                }
            }
            else
            {
                Debug.LogError("Error: Cannot find field scene_objects");
            }

            OnPoseReceived?.Invoke(cubeDirectory.Values);
        }

        public void ConnectWebSocket()
        {
            WebSocket?.Connect();
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            Debug.Log("WebSocket says: " + e.Data.ToString());
        }

        public void CleanUp()
        {
            WebSocket.OnMessage -= OnMessage;
            WebSocket.OnMessage -= TriggerCubeSpawn;
            WebSocket.Close();
        }
    }
}

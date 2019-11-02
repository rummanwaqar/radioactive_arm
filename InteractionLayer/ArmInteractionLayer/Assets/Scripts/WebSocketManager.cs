using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;

namespace WebSocketLayer
{
    public class WebSocketManager
    {
        Vector3 rootPosition;
        JObject receivedData;

        const string URL = "ws://192.168.244.197:8765/";

        private readonly WebSocket WebSocket;

        public WebSocketManager(Vector3 rootPosition, JObject receivedData)
        {
            this.rootPosition = rootPosition;
            this.receivedData = receivedData ?? throw new ArgumentNullException(nameof(receivedData));

            WebSocket = new WebSocket(URL);
            WebSocket.OnMessage += OnMessage;
            WebSocket.ConnectAsync();
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            Debug.LogError("WebSocket says: " + e.Data.ToString());
        }

        public void SpawnCube()
        {
            if (WebSocket.IsAlive) WebSocket.Accept();
        }

        public void CleanUp()
        {
            WebSocket.CloseAsync();
        }
    }
}

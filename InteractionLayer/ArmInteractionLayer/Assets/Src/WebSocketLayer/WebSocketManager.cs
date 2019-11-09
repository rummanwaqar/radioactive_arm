using ArmInteractionCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

namespace WebSocketLayer
{
    public class WebSocketManager: IWebSocketManager
    {
        private JObject receivedData;

        const string localURL = "ws://127.0.0.1:8765/";

        private readonly WebSocket WebSocket;
        private readonly ICoroutineExecutor coroutineExecutor;

        public event Action<IEnumerable<Vector3>> OnPoseReceived;
        private readonly GameStateReader gameStateReader;

        public WebSocketManager(string webSocketURL = localURL)
        {
            WebSocket = new WebSocket(webSocketURL);
            WebSocket.OnMessage += OnMessage;
            WebSocket.OnMessage += TriggerCubeSpawn;

            gameStateReader = new GameStateReader();
        }

        private void TriggerCubeSpawn(object sender, MessageEventArgs e)
        {
            if(gameStateReader.TryParseState(e.Data))
            {
                OnPoseReceived?.Invoke(gameStateReader.CubePositions);
                WebSocket.OnMessage -= TriggerCubeSpawn; //TODO: Spawning cubes only one. Would need to make a Cube registry to keep track of all the cubes in the scene
            }
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

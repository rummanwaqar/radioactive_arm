using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ArmInteractionCore
{
    public interface IGameManager : IManager
    {
        void Update();
    }

    public interface IManager
    {
        void CleanUp();
    }

    public interface IWebSocketManager : IManager
    {
        event Action<IEnumerable<Vector3>> OnPoseReceived;
        void ConnectWebSocket();
    }

    public interface ICoroutineExecutor
    {
        Coroutine StartCoroutine(IEnumerator routine);
    }

    public interface ITableObjectSpawner
    {
        GameObject InstantiateOnTable(GameObject go);
    }
}

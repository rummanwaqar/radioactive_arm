using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArmInteractionCore
{
    public interface IWebSocketManager : IManager
    {
        event Action<IEnumerable<Vector3>> OnPoseReceived;
        void ConnectWebSocket();
    }
}

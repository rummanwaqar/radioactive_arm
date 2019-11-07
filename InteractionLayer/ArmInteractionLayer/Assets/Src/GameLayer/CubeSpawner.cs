using UnityEngine;
using ArmInteractionCore;
using System;
using System.Collections.Generic;

namespace GameLayer
{
    public class CubeSpawner
    {
        private readonly Action onSpawnCube;
        private readonly IWebSocketManager webSocketManager;

        private Vector3 cubePosition;
        public IEnumerable<Vector3> CubePositions;

        public CubeSpawner(Action onSpawnCube)
        {
            this.onSpawnCube = onSpawnCube ?? throw new ArgumentNullException(nameof(onSpawnCube));
        }

        public void SpawnCube(IEnumerable<Vector3> relativePositions)
        {
            CubePositions = relativePositions;
            onSpawnCube();
        }
    }

}
using ArmInteractionCore;
using System;
using System.Collections;
using UnityEngine;

namespace GameLayer
{
    public class GameManager : IGameManager
    {
        bool shouldSpawnCube;
        ICoroutineExecutor coroutineExecutor;
        ITableObjectSpawner gameObjectSpawner;
        IWebSocketManager webSocketManager;

        CubeSpawner cubeSpawner;

        public GameManager(ICoroutineExecutor coroutineExecutor,
            ITableObjectSpawner gameObjectSpawner,
            IWebSocketManager webSocketManager,
            Transform rootTransform)
        {
            this.coroutineExecutor = coroutineExecutor ?? throw new ArgumentNullException(nameof(coroutineExecutor));
            this.gameObjectSpawner = gameObjectSpawner ?? throw new ArgumentNullException(nameof(gameObjectSpawner));
            this.webSocketManager = webSocketManager ?? throw new ArgumentNullException(nameof(webSocketManager));

            cubeSpawner = new CubeSpawner(OnCubeSpawned);

            webSocketManager.OnPoseReceived += cubeSpawner.SpawnCube;
        }

        private void OnCubeSpawned()
        {
            shouldSpawnCube = true;
        }

        public void Update()
        {
            if (shouldSpawnCube)
            {
                coroutineExecutor.StartCoroutine(InstantiateAsync());
                shouldSpawnCube = false;
            }
        }

        private IEnumerator InstantiateAsync()
        {
            foreach(var position in cubeSpawner.CubePositions)
            {
                var loadedCube = gameObjectSpawner.InstantiateOnTable(Resources.Load<GameObject>("Prefabs/Cube"));
                yield return null;
                loadedCube.transform.localPosition = position;
            }
        }

        public void CleanUp()
        {
            webSocketManager.OnPoseReceived -= cubeSpawner.SpawnCube;
            cubeSpawner = null;
        }
    }
}
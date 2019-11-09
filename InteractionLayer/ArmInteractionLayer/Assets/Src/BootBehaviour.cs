using ArmInteractionCore;
using GameLayer;
using UnityEngine;
using WebSocketLayer;

public class BootBehaviour : MonoBehaviour, ICoroutineExecutor, ITableObjectSpawner
{
    Transform rootTransform;
    IWebSocketManager webSocketManager;
    IGameManager gameManager;

    void Start()
    {
        rootTransform = this.gameObject.GetComponent<Transform>();
        webSocketManager = new WebSocketManager();

        gameManager = new GameManager(this, this, webSocketManager, rootTransform);

        webSocketManager.ConnectWebSocket();
    }

    void Update()
    {
        gameManager.Update();
    }

    void OnApplicationQuit()
    {
        webSocketManager.CleanUp();
        gameManager.CleanUp();
    }

    public GameObject InstantiateOnTable(GameObject go)
    {
        return Instantiate(go, rootTransform, false) as GameObject;
    }
}
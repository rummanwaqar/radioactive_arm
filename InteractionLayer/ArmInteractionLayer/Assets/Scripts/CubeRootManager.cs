using UnityEngine;
using Newtonsoft.Json.Linq;
using WebSocketLayer;

public class CubeRootManager : MonoBehaviour
{
    Transform rootTransform;
    WebSocketManager cubeSpawner;

    void Start()
    {
        rootTransform = this.gameObject.GetComponent<Transform>();
        cubeSpawner = new WebSocketManager(rootTransform.position, new JObject());
    }

    void OnApplicationQuit()
    {
        cubeSpawner.CleanUp();
    }
}
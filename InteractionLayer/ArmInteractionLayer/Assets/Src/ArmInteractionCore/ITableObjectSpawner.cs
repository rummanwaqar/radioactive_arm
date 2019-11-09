using UnityEngine;

namespace ArmInteractionCore
{
    public interface ITableObjectSpawner
    {
        GameObject InstantiateOnTable(GameObject go);
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace ArmInteractionCore
{
    public interface IGameStateReader
    {
        IEnumerable<Vector3> CubePositions { get; }
        bool TryParseState(string jsonString);
    }
}
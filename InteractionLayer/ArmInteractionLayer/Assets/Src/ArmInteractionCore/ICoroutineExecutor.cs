using System.Collections;
using UnityEngine;

namespace ArmInteractionCore
{
    public interface ICoroutineExecutor
    {
        Coroutine StartCoroutine(IEnumerator routine);
    }
}
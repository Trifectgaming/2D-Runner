using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleQueue<T> : IEnumerable<T> where T : Object
{
    private readonly Queue<T> _internalQueue;
    public RecycleQueue(int count, T prefab, Vector3 position)
    {
        _internalQueue = new Queue<T>(count);
        for (int i = 0; i < count; i++)
        {
            var projectile = Object.Instantiate(prefab, position, Quaternion.identity) as T;
            _internalQueue.Enqueue(projectile);
        }
    }

    public T Next()
    {
        var result = _internalQueue.Dequeue();
        _internalQueue.Enqueue(result);
        return result;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _internalQueue.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
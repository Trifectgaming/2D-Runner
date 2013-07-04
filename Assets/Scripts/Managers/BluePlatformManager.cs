using UnityEngine;
using System.Collections;

public class BluePlatformManager : MonoBehaviour
{
    public Transform[] platformPrefabs;
    public int sharePlatformCount = 8;
    public int distanceOffset;
    private RecycleQueue<Transform>[] _queues;
    private float _lastObstaclePositionEnd;

    private void Awake()
    {
        _queues = new RecycleQueue<Transform>[platformPrefabs.Length];
        for (int i = 0; i < platformPrefabs.Length; i++)
        {
            _queues[i] = new RecycleQueue<Transform>(sharePlatformCount / platformPrefabs.Length, platformPrefabs[i],
                                                      new Vector3(-100, platformPrefabs[i].position.y));
        }
    }
    private void Update()
    {
        if (_lastObstaclePositionEnd + distanceOffset > TK2DRunner.DistanceTraveled) return;
        var batch = _queues[Random.Range(0, _queues.Length)];
        var platform = batch.Next();
        var trans = platform.transform;
        trans.position = new Vector3(_lastObstaclePositionEnd + (distanceOffset*2), trans.position.y);
        _lastObstaclePositionEnd = trans.position.x;
    }
}

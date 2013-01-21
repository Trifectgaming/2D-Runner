using UnityEngine;

public class FlatlineManager : RecyclingBlockManager
{
    protected override void Start()
    {
        base.Start();
        OnGameStart(new GameStartMessage());
    }


    protected override void OnRecycle(Transform block, Vector3 scale, Vector3 position)
    {
        base.OnRecycle(block, scale, position);

        nextPosition.x += scale.x;
    }

    void OnDrawGizmo()
    {
        Gizmos.DrawCube(nextPosition, minsize);
    }
}
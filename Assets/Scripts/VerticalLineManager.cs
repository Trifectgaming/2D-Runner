using UnityEngine;

public class VerticalLineManager : RecyclingBlockManager
{
    public float gap;

    protected override void Start()
    {
        base.Start();
        OnGameStart(new GameStartMessage());
    }
    
    protected override void OnRecycle(Transform block, Vector3 scale, Vector3 position)
    {
        base.OnRecycle(block, scale, position);

        nextPosition.x += (scale.x + gap);

        Debug.Log(nextPosition);
    }
}
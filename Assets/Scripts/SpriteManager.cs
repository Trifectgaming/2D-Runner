using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SpriteManager : MonoBehaviour
{
    public tk2dBaseSprite spritePrefab;
    public int count = 10;
    
    private Transform _transform;
    private Queue<tk2dBaseSprite> _batch;
    private Vector3 nextPosition;

    // Use this for initialization
    private void Start()
    {
        nextPosition = transform.localPosition;
        _batch = new Queue<tk2dBaseSprite>(count);
        for (int i = 0; i < count; i++)
        {
            var sprite = Instantiate(spritePrefab, transform.position, transform.rotation) as tk2dBaseSprite;
            Recycle(sprite);
        }
        _transform = transform;
    }

    // Update is called once per frame
	void Update ()
	{
        var offset = _batch.Peek().transform.localPosition.x + (_batch.Peek().GetBounds().size.x);
        if (offset < Runner.DistanceTraveled)
	    {
            Recycle(_batch.Dequeue());
	    }
	}

    private void Recycle(tk2dBaseSprite o)
    {
        var trans = o.transform;
        var size = o.GetBounds().size;
        var position = nextPosition;
        _batch.Enqueue(o);

        trans.localPosition = position;
        nextPosition.x += size.x;
    }
}

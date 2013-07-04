using UnityEngine;

public class ActionBlock : MonoBehaviour
{
    public InputState InputState;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}

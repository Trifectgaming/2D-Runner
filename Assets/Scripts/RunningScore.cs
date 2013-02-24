using UnityEngine;
using System.Collections;

[RequireComponent(typeof(tk2dTextMesh))]
public class RunningScore : MonoBehaviour
{
    public Vector3 offset;
    private tk2dTextMesh _mesh;

    Transform __transform; // cache transform locally
    Transform _transform
    {
        get
        {
            if (__transform == null) __transform = transform;
            return __transform;
        }
    }

	// Use this for initialization
	void Start ()
	{
	    _mesh = GetComponent<tk2dTextMesh>();
        StartCoroutine(UpdateTransform());
	}

    private IEnumerator UpdateTransform()
    {
        while (true)
        {
            var orthoSize = Camera.mainCamera.orthographicSize;
            var resolution = Camera.mainCamera.GetScreenWidth()/Camera.mainCamera.GetScreenHeight();
            _transform.localPosition = new Vector3(
                -resolution * orthoSize + offset.x,
                orthoSize + offset.y,
                _transform.localPosition.z + offset.z);
            _mesh.text = "Ran: " + Runner.DistanceTraveled.ToString("0000000");
            _mesh.Commit();

            yield return new WaitForSeconds(1f);
        }
    }

    // Update is called once per frame
	void Update ()
	{
        
	}
}

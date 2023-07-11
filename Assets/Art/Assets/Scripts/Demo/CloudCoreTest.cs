using UnityEngine;

public class CloudCoreTest : MonoBehaviour
{
    public float falloffDstHorizontal = 3;
    public float falloffVertical = 1.5f;
    public float maxScale = 1;

    public Vector2 rotSpeedMinMax = new(10, 20);

    [HideInInspector] public Transform myTransform;

    private float rotSpeed;

    private void Start()
    {
        rotSpeed = Random.Range(rotSpeedMinMax.x, rotSpeedMinMax.y);
        myTransform = transform;
    }

    private void Update()
    {
        myTransform.RotateAround(transform.parent.position, Vector3.up, Time.deltaTime * rotSpeed);
    }
}
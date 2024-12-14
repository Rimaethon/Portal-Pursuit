using UnityEngine;

public class PickupAnimation : MonoBehaviour
{
    public float oscillationHeight = 0.5f;
    public float oscillationSpeed = 2.0f;
    public float rotationSpeed = 90.0f;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        transform.localPosition = startPosition + Vector3.up * (oscillationHeight * Mathf.Cos(Time.timeSinceLevelLoad * oscillationSpeed));
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + Vector3.up * (Time.deltaTime * rotationSpeed));
    }
}

using UnityEngine;

public class Car : PortalTraveller
{
    public float maxSpeed = 1;
    private float smoothV;
    private float speed;
    private float targetSpeed;

    private void Start()
    {
        Debug.Log("Press C to stop/start car");
        targetSpeed = maxSpeed;
    }

    private void Update()
    {
        var moveDst = Time.deltaTime * speed;
        transform.position += transform.forward * Time.deltaTime * speed;

        if (Input.GetKeyDown(KeyCode.C)) targetSpeed = targetSpeed == 0 ? maxSpeed : 0;
        speed = Mathf.SmoothDamp(speed, targetSpeed, ref smoothV, .5f);
    }
}
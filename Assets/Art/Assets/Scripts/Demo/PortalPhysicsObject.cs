using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PortalPhysicsObject : PortalTraveller
{
    private static int i;

    public float force = 10;
    public Color[] colors;
    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        graphicsObject.GetComponent<MeshRenderer>().material.color = colors[i];
        i++;
        if (i > colors.Length - 1) i = 0;
    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        base.Teleport(fromPortal, toPortal, pos, rot);
        rigidbody.velocity = toPortal.TransformVector(fromPortal.InverseTransformVector(rigidbody.velocity));
        rigidbody.angularVelocity =
            toPortal.TransformVector(fromPortal.InverseTransformVector(rigidbody.angularVelocity));
    }
}
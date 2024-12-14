using UnityEngine;


public class Projectile : MonoBehaviour
{
    public float projectileSpeed = 3.0f;


    protected virtual void Update()
    {
        transform.position += transform.forward * (projectileSpeed * Time.deltaTime);
    }
}

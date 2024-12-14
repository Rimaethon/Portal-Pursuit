using UnityEngine;

public class TimedObjectDestroyer : MonoBehaviour
{
    public static bool quitting;
    public float lifetime = 5.0f;
    public bool destroyChildrenOnDeath = true;
    private float timeAlive;

    private void Update()
    {
        if (timeAlive > lifetime)
            Destroy(gameObject);
        else
            timeAlive += Time.deltaTime;
    }

    private void OnDestroy()
    {
        if (destroyChildrenOnDeath && !quitting && Application.isPlaying)
        {
            int childCount = transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                GameObject childObject = transform.GetChild(i).gameObject;
                if (childObject != null) Destroy(childObject);
            }
        }
        transform.DetachChildren();
    }

    private void OnApplicationQuit()
    {
        quitting = true;
        DestroyImmediate(gameObject);
    }
}

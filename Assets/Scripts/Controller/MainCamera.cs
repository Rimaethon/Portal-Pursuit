using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private Portal[] portals;

    private void Awake()
    {
        portals = FindObjectsOfType<Portal>();
    }

    private void OnPreCull()
    {
        for (var i = 0; i < portals.Length; i++) portals[i].PrePortalRender();
        for (var i = 0; i < portals.Length; i++) portals[i].Render();

        for (var i = 0; i < portals.Length; i++) portals[i].PostPortalRender();
    }
}
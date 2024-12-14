using UnityEngine;
using UnityEngine.Rendering;

public class MainCamera : MonoBehaviour {

    Portal[] portals;
    void Awake()
    {
        portals = FindObjectsOfType<Portal>();
        RenderPipelineManager.beginCameraRendering += RenderPortal;
    }

    private void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= RenderPortal;
    }

    private void RenderPortal(ScriptableRenderContext context, Camera camera)
    {
        foreach (Portal portal in portals)
        {
            portal.PrePortalRender();
        }

        foreach (Portal portal in portals)
        {
            portal.Render(context);


        }

        foreach (Portal portal in portals)
        {
            portal.PostPortalRender();

        }
    }
}

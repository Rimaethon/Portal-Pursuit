using UnityEngine;
using UnityEngine.Rendering;

public class MainCamera : MonoBehaviour {

    Portal[] portals;
    private Camera mainCam;
    void Awake()
    {
        portals = FindObjectsOfType<Portal>();
        mainCam = GetComponent<Camera>();
        RenderPipelineManager.beginCameraRendering += RenderPortal;
    }

    private void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= RenderPortal;
    }

    private void RenderPortal(ScriptableRenderContext context, Camera camera)
    {
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].PrePortalRender();
        }
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].Render(context);
        }
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].PostPortalRender();
        }
    }

}

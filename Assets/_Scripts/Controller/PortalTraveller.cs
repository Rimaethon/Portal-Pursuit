using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour {

    public GameObject graphicsObject;
    private PlayerAnimationController playerAnimationController;
    public GameObject graphicsClone { get; set; }
    private SkinnedMeshRenderer[] cloneSkinnedMeshRenderer;
    private MeshRenderer[] clonedMeshRenderer;
    private MeshRenderer[] originalMeshRenderer;
    private SkinnedMeshRenderer[] originalSkinnedMeshRenderer;
    public Vector3 previousOffsetFromPortal { get; set; }

    public Material[] originalMaterials { get; set; }
    public Material[] cloneMaterials { get; set; }

    protected virtual void Awake()
    {
        graphicsClone = Instantiate (graphicsObject);
        graphicsClone.transform.parent = graphicsObject.transform.parent;
        graphicsClone.transform.localScale = graphicsObject.transform.localScale;
        playerAnimationController = GetComponent<PlayerAnimationController>();
        originalSkinnedMeshRenderer = graphicsObject.GetComponentsInChildren<SkinnedMeshRenderer> ();
        cloneSkinnedMeshRenderer=graphicsClone.GetComponentsInChildren<SkinnedMeshRenderer>();
        clonedMeshRenderer = graphicsClone.GetComponentsInChildren<MeshRenderer>();
        originalMeshRenderer = graphicsObject.GetComponentsInChildren<MeshRenderer>();
        graphicsClone.transform.localPosition = graphicsObject.transform.localPosition;
        HandleSkinnedMeshRenderers(false);
        originalMaterials = GetMaterials (originalSkinnedMeshRenderer);
        cloneMaterials = GetMaterials (cloneSkinnedMeshRenderer);
    }

    public virtual void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }

    public virtual void EnterPortalThreshold ()
    {
        HandleSkinnedMeshRenderers(true);
        playerAnimationController.ApplyAnimation();
    }

    public virtual void ExitPortalThreshold ()
    {
        HandleSkinnedMeshRenderers(false);
        // Disable slicing
        foreach (Material material in originalMaterials)
        {
            material.SetVector ("sliceNormal", Vector3.zero);
        }
    }

    public void SetSliceOffsetDst (float dst, bool clone) {
        for (int i = 0; i < originalMaterials.Length; i++) {
            if (clone) {
                cloneMaterials[i].SetFloat ("sliceOffsetDst", dst);
            } else {
                originalMaterials[i].SetFloat ("sliceOffsetDst", dst);
            }
        }
    }
    private void HandleSkinnedMeshRenderers(bool enable)
    {
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in cloneSkinnedMeshRenderer)
        {
            skinnedMeshRenderer.enabled = enable;
        }
        foreach (MeshRenderer meshRenderer in clonedMeshRenderer)
        {
            meshRenderer.enabled = enable;
        }
    }


    Material[] GetMaterials (SkinnedMeshRenderer[] skinnedMeshRenderer)
    {
        List<Material> matList = new List<Material> ();

        foreach (var renderer in skinnedMeshRenderer)
        {
            foreach (Material mat in renderer.sharedMaterials)
            {
                if(matList.Contains(mat))
                    continue;
                matList.Add (mat);
            }
        }

        return matList.ToArray ();
    }
}

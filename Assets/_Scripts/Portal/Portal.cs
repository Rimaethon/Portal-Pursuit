using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;
    public MeshRenderer screen;
    public int recursionLimit = 5;

    public float nearClipOffset = 0.05f;
    public float nearClipLimit = 0.2f;

    [SerializeField] private float screenThickness;
    [SerializeField]   private Camera playerCam;
    private RenderTexture viewTexture;
    private Camera portalCam;
    private Material firstRecursionMat;
    private   List<PortalTraveller> trackedTravellers;
    private MeshFilter screenMeshFilter;
    private Vector3 portalCamPos => portalCam.transform.position;

    void Awake ()
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera> ();
        portalCam.enabled = false;
        trackedTravellers = new List<PortalTraveller> ();
        screenMeshFilter = screen.GetComponent<MeshFilter> ();
        screen.material.SetInt ("displayMask", 1);
    }


   void LateUpdate ()
    {
        HandleTravellers ();
    }

    void HandleTravellers ()
    {
        for (int i = 0; i < trackedTravellers.Count; i++) {
            PortalTraveller traveller = trackedTravellers[i];
            Transform travellerT = traveller.transform;
            Matrix4x4 m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

            Vector3 offsetFromPortal = travellerT.position - transform.position;
            int portalSide = System.Math.Sign (Vector3.Dot (offsetFromPortal, transform.forward));
            int portalSideOld = System.Math.Sign (Vector3.Dot (traveller.previousOffsetFromPortal, transform.forward));
            if (portalSide != portalSideOld) {
                Vector3 positionOld = travellerT.position;
                Quaternion rotOld = travellerT.rotation;
                traveller.Teleport (transform, linkedPortal.transform, m.GetColumn (3), m.rotation);
                traveller.graphicsClone.transform.SetPositionAndRotation (positionOld, rotOld);
                linkedPortal.OnTravellerEnterPortal (traveller);
                trackedTravellers.RemoveAt (i);
                i--;
            } else {
                traveller.graphicsClone.transform.SetPositionAndRotation (m.GetColumn (3), m.rotation);
                UpdateSliceParams (traveller);
                traveller.previousOffsetFromPortal = offsetFromPortal;
            }
        }
    }

    public void PrePortalRender ()
    {
        foreach (PortalTraveller traveller in trackedTravellers) {
            UpdateSliceParams (traveller);
        }
    }

    public void Render (ScriptableRenderContext context)
    {
        if (!CameraUtility.VisibleFromCamera (linkedPortal.screen, playerCam)) {
            return;
        }

        CreateViewTexture ();

        Matrix4x4 localToWorldMatrix = playerCam.transform.localToWorldMatrix;
        Vector3[] renderPositions = new Vector3[recursionLimit];
        Quaternion[] renderRotations = new Quaternion[recursionLimit];

        int startIndex = 0;
        portalCam.projectionMatrix = playerCam.projectionMatrix;
        for (int i = 0; i < recursionLimit; i++) {
            if (i > 0) {
                if (!CameraUtility.BoundsOverlap (screenMeshFilter, linkedPortal.screenMeshFilter, portalCam)) {
                    break;
                }
            }
            localToWorldMatrix = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * localToWorldMatrix;
            int renderOrderIndex = recursionLimit - i - 1;
            renderPositions[renderOrderIndex] = localToWorldMatrix.GetColumn (3);
            renderRotations[renderOrderIndex] = localToWorldMatrix.rotation;

            portalCam.transform.SetPositionAndRotation (renderPositions[renderOrderIndex], renderRotations[renderOrderIndex]);
            startIndex = renderOrderIndex;
        }

        screen.shadowCastingMode =ShadowCastingMode.ShadowsOnly;
        linkedPortal.screen.material.SetInt ("displayMask", 0);

        for (int i = startIndex; i < recursionLimit; i++) {
            portalCam.transform.SetPositionAndRotation (renderPositions[i], renderRotations[i]);
            SetNearClipPlane ();
            HandleClipping ();
            UniversalRenderPipeline.RenderSingleCamera(context, portalCam);

            if (i == startIndex) {
                linkedPortal.screen.material.SetInt ("displayMask", 1);
            }
        }

        screen.shadowCastingMode = ShadowCastingMode.On;
    }

    private void HandleClipping ()
    {
        const float hide_distance = -1000;
        const float show_distance = 1000;
        screenThickness = linkedPortal.ProtectScreenFromClipping (portalCam.transform.position);
        foreach (PortalTraveller traveller in trackedTravellers)
        {
            traveller.SetSliceOffsetDst(SameSideOfPortal(traveller.transform.position, portalCamPos) ? hide_distance : show_distance, false);
            int cloneSideOfLinkedPortal = -SideOfPortal (traveller.transform.position);
            bool camSameSideAsClone = linkedPortal.SideOfPortal (portalCamPos) == cloneSideOfLinkedPortal;
            if (camSameSideAsClone)
            {
                traveller.SetSliceOffsetDst (screenThickness, true);
            } else
            {
                traveller.SetSliceOffsetDst (-screenThickness, true);
            }
        }

        foreach (PortalTraveller linkedTraveller in linkedPortal.trackedTravellers)
        {
            Vector3 travellerPos = linkedTraveller.graphicsObject.transform.position;
            bool cloneOnSameSideAsCam = linkedPortal.SideOfPortal (travellerPos) != SideOfPortal (portalCamPos);
            linkedTraveller.SetSliceOffsetDst(cloneOnSameSideAsCam ? hide_distance : show_distance, true);
            bool camSameSideAsTraveller = linkedPortal.SameSideOfPortal (linkedTraveller.transform.position, portalCamPos);
            if (camSameSideAsTraveller)
            {
                linkedTraveller.SetSliceOffsetDst (screenThickness, false);
            } else {
                linkedTraveller.SetSliceOffsetDst (-screenThickness, false);
            }
        }
    }

    public void PostPortalRender ()
    {
        foreach (var traveller in trackedTravellers)
        {
            UpdateSliceParams (traveller);
        }
        ProtectScreenFromClipping (playerCam.transform.position);
    }

    private void CreateViewTexture ()
    {
        if (viewTexture != null && viewTexture.width == Screen.width && viewTexture.height == Screen.height) return;

        if (viewTexture != null)
        {
            viewTexture.Release ();
        }
        viewTexture = new RenderTexture (Screen.width, Screen.height, 0);
        portalCam.targetTexture = viewTexture;
        linkedPortal.screen.material.SetTexture ("_MainTex", viewTexture);
    }

   private float ProtectScreenFromClipping (Vector3 viewPoint)
    {
        float halfHeight = playerCam.nearClipPlane * Mathf.Tan (playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * playerCam.aspect;
        float dstToNearClipPlaneCorner = new Vector3 (halfWidth, halfHeight, playerCam.nearClipPlane).magnitude;
        float screenThickness = dstToNearClipPlaneCorner;

        Transform screenT = screen.transform;
        bool camFacingSameDirAsPortal = Vector3.Dot (transform.forward, transform.position - viewPoint) > 0;
        screenT.localScale = new Vector3 (screenT.localScale.x, screenT.localScale.y, screenThickness);
        screenT.localPosition = Vector3.forward * screenThickness * ((camFacingSameDirAsPortal) ? 0.5f : -0.5f);
        return screenThickness;
    }

    void UpdateSliceParams (PortalTraveller traveller) {
        int side = SideOfPortal (traveller.transform.position);
        Vector3 sliceNormal = transform.forward * -side;
        Vector3 cloneSliceNormal = linkedPortal.transform.forward * side;

        Vector3 slicePos = transform.position;
        Vector3 cloneSlicePos = linkedPortal.transform.position;

        float sliceOffsetDst = 0;
        float cloneSliceOffsetDst = 0;
        float screenThickness = screen.transform.localScale.z;

        bool playerSameSideAsTraveller = SameSideOfPortal (playerCam.transform.position, traveller.transform.position);
        if (!playerSameSideAsTraveller) {
            sliceOffsetDst = -screenThickness;
        }
        bool playerSameSideAsCloneAppearing = side != linkedPortal.SideOfPortal (playerCam.transform.position);
        if (!playerSameSideAsCloneAppearing) {
            cloneSliceOffsetDst = -screenThickness;
        }

        for (int i = 0; i < traveller.originalMaterials.Length; i++)
        {
            traveller.originalMaterials[i].SetVector ("sliceCentre", slicePos);
            traveller.originalMaterials[i].SetVector ("sliceNormal", sliceNormal);
            traveller.originalMaterials[i].SetFloat ("sliceOffsetDst", sliceOffsetDst);
            traveller.cloneMaterials[i].SetVector ("sliceCentre", cloneSlicePos);
            traveller.cloneMaterials[i].SetVector ("sliceNormal", cloneSliceNormal);
            traveller.cloneMaterials[i].SetFloat ("sliceOffsetDst", cloneSliceOffsetDst);
        }

    }
    private void SetNearClipPlane ()
    {
        Transform clipPlane = transform;
        int dot = System.Math.Sign (Vector3.Dot (clipPlane.forward, transform.position - portalCam.transform.position));
        Vector3 camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint (clipPlane.position);
        Vector3 camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector (clipPlane.forward) * dot;
        float camSpaceDst = -Vector3.Dot (camSpacePos, camSpaceNormal) + nearClipOffset;

        if (Mathf.Abs (camSpaceDst) > nearClipLimit)
        {
            Vector4 clipPlaneCameraSpace = new Vector4 (camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);
            portalCam.projectionMatrix = playerCam.CalculateObliqueMatrix (clipPlaneCameraSpace);
        } else
        {
            portalCam.projectionMatrix = playerCam.projectionMatrix;
        }
    }

   private  void OnTravellerEnterPortal (PortalTraveller traveller)
    {
        if (trackedTravellers.Contains(traveller)) return;
        traveller.EnterPortalThreshold ();
        traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
        trackedTravellers.Add (traveller);
    }

    private void OnTriggerEnter (Collider other)
    {
        PortalTraveller traveller = other.GetComponent<PortalTraveller> ();
        if (traveller)
        {
            OnTravellerEnterPortal (traveller);
        }
    }

    private void OnTriggerExit (Collider other)
    {
        PortalTraveller traveller = other.GetComponent<PortalTraveller> ();
        if (!traveller || !trackedTravellers.Contains(traveller))
            return;
        traveller.ExitPortalThreshold ();
        trackedTravellers.Remove (traveller);
    }

    private int SideOfPortal (Vector3 pos)
    {
        return System.Math.Sign (Vector3.Dot (pos - transform.position, transform.forward));
    }

    private  bool SameSideOfPortal (Vector3 posA, Vector3 posB)
    {
        return SideOfPortal (posA) == SideOfPortal (posB);
    }

    void OnValidate ()
    {
        if (linkedPortal != null) {
            linkedPortal.linkedPortal = this;
        }
    }
}

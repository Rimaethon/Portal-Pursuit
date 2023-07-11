using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Portal : MonoBehaviour
{
    [Header("Main Settings")] public Portal linkedPortal;

    public MeshRenderer screen;
    public int recursionLimit = 5;

    [Header("Advanced Settings")] public float nearClipOffset = 0.05f;

    public float nearClipLimit = 0.2f;
    private Material firstRecursionMat;
    private Camera playerCam;
    private Camera portalCam;
    private MeshFilter screenMeshFilter;
    private List<PortalTraveller> trackedTravellers;

    // Private variables
    private RenderTexture viewTexture;

    private Vector3 portalCamPos => portalCam.transform.position;

    private void Awake()
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false;
        trackedTravellers = new List<PortalTraveller>();
        screenMeshFilter = screen.GetComponent<MeshFilter>();
        screen.material.SetInt("displayMask", 1);
    }

    private void LateUpdate()
    {
        HandleTravellers();
    }

    private void OnTriggerEnter(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller) OnTravellerEnterPortal(traveller);
    }

    private void OnTriggerExit(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller && trackedTravellers.Contains(traveller))
        {
            traveller.ExitPortalThreshold();
            trackedTravellers.Remove(traveller);
        }
    }

    private void OnValidate()
    {
        if (linkedPortal != null) linkedPortal.linkedPortal = this;
    }

    private void HandleTravellers()
    {
        for (var i = 0; i < trackedTravellers.Count; i++)
        {
            var traveller = trackedTravellers[i];
            var travellerT = traveller.transform;
            var m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix *
                    travellerT.localToWorldMatrix;

            var offsetFromPortal = travellerT.position - transform.position;
            var portalSide = Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
            var portalSideOld = Math.Sign(Vector3.Dot(traveller.previousOffsetFromPortal, transform.forward));
            // Teleport the traveller if it has crossed from one side of the portal to the other
            if (portalSide != portalSideOld)
            {
                var positionOld = travellerT.position;
                var rotOld = travellerT.rotation;
                traveller.Teleport(transform, linkedPortal.transform, m.GetColumn(3), m.rotation);
                traveller.graphicsClone.transform.SetPositionAndRotation(positionOld, rotOld);
                // Can't rely on OnTriggerEnter/Exit to be called next frame since it depends on when FixedUpdate runs
                linkedPortal.OnTravellerEnterPortal(traveller);
                trackedTravellers.RemoveAt(i);
                i--;
            }
            else
            {
                traveller.graphicsClone.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
                //UpdateSliceParams (traveller);
                traveller.previousOffsetFromPortal = offsetFromPortal;
            }
        }
    }

    // Called before any portal cameras are rendered for the current frame
    public void PrePortalRender()
    {
        foreach (var traveller in trackedTravellers) UpdateSliceParams(traveller);
    }

    // Manually render the camera attached to this portal
    // Called after PrePortalRender, and before PostPortalRender
    public void Render()
    {
        // Skip rendering the view from this portal if player is not looking at the linked portal
        if (!CameraUtility.VisibleFromCamera(linkedPortal.screen, playerCam)) return;

        CreateViewTexture();

        var localToWorldMatrix = playerCam.transform.localToWorldMatrix;
        var renderPositions = new Vector3[recursionLimit];
        var renderRotations = new Quaternion[recursionLimit];

        var startIndex = 0;
        portalCam.projectionMatrix = playerCam.projectionMatrix;
        for (var i = 0; i < recursionLimit; i++)
        {
            if (i > 0)
                // No need for recursive rendering if linked portal is not visible through this portal
                if (!CameraUtility.BoundsOverlap(screenMeshFilter, linkedPortal.screenMeshFilter, portalCam))
                    break;
            localToWorldMatrix = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix *
                                 localToWorldMatrix;
            var renderOrderIndex = recursionLimit - i - 1;
            renderPositions[renderOrderIndex] = localToWorldMatrix.GetColumn(3);
            renderRotations[renderOrderIndex] = localToWorldMatrix.rotation;

            portalCam.transform.SetPositionAndRotation(renderPositions[renderOrderIndex],
                renderRotations[renderOrderIndex]);
            startIndex = renderOrderIndex;
        }

        // Hide screen so that camera can see through portal
        screen.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        linkedPortal.screen.material.SetInt("displayMask", 0);

        for (var i = startIndex; i < recursionLimit; i++)
        {
            portalCam.transform.SetPositionAndRotation(renderPositions[i], renderRotations[i]);
            SetNearClipPlane();
            HandleClipping();
            portalCam.Render();

            if (i == startIndex) linkedPortal.screen.material.SetInt("displayMask", 1);
        }

        // Unhide objects hidden at start of render
        screen.shadowCastingMode = ShadowCastingMode.On;
    }

    private void HandleClipping()
    {
        // There are two main graphical issues when slicing travellers
        // 1. Tiny sliver of mesh drawn on backside of portal
        //    Ideally the oblique clip plane would sort this out, but even with 0 offset, tiny sliver still visible
        // 2. Tiny seam between the sliced mesh, and the rest of the model drawn onto the portal screen
        // This function tries to address these issues by modifying the slice parameters when rendering the view from the portal
        // Would be great if this could be fixed more elegantly, but this is the best I can figure out for now
        const float hideDst = -1000;
        const float showDst = 1000;
        var screenThickness = linkedPortal.ProtectScreenFromClipping(portalCam.transform.position);

        foreach (var traveller in trackedTravellers)
        {
            if (SameSideOfPortal(traveller.transform.position, portalCamPos))
                // Addresses issue 1
                traveller.SetSliceOffsetDst(hideDst, false);
            else
                // Addresses issue 2
                traveller.SetSliceOffsetDst(showDst, false);

            // Ensure clone is properly sliced, in case it's visible through this portal:
            var cloneSideOfLinkedPortal = -SideOfPortal(traveller.transform.position);
            var camSameSideAsClone = linkedPortal.SideOfPortal(portalCamPos) == cloneSideOfLinkedPortal;
            if (camSameSideAsClone)
                traveller.SetSliceOffsetDst(screenThickness, true);
            else
                traveller.SetSliceOffsetDst(-screenThickness, true);
        }

        var offsetFromPortalToCam = portalCamPos - transform.position;
        foreach (var linkedTraveller in linkedPortal.trackedTravellers)
        {
            var travellerPos = linkedTraveller.graphicsObject.transform.position;
            var clonePos = linkedTraveller.graphicsClone.transform.position;
            // Handle clone of linked portal coming through this portal:
            var cloneOnSameSideAsCam = linkedPortal.SideOfPortal(travellerPos) != SideOfPortal(portalCamPos);
            if (cloneOnSameSideAsCam)
                // Addresses issue 1
                linkedTraveller.SetSliceOffsetDst(hideDst, true);
            else
                // Addresses issue 2
                linkedTraveller.SetSliceOffsetDst(showDst, true);

            // Ensure traveller of linked portal is properly sliced, in case it's visible through this portal:
            var camSameSideAsTraveller =
                linkedPortal.SameSideOfPortal(linkedTraveller.transform.position, portalCamPos);
            if (camSameSideAsTraveller)
                linkedTraveller.SetSliceOffsetDst(screenThickness, false);
            else
                linkedTraveller.SetSliceOffsetDst(-screenThickness, false);
        }
    }

    // Called once all portals have been rendered, but before the player camera renders
    public void PostPortalRender()
    {
        foreach (var traveller in trackedTravellers) UpdateSliceParams(traveller);
        ProtectScreenFromClipping(playerCam.transform.position);
    }

    private void CreateViewTexture()
    {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
        {
            if (viewTexture != null) viewTexture.Release();
            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            // Render the view from the portal camera to the view texture
            portalCam.targetTexture = viewTexture;
            // Display the view texture on the screen of the linked portal
            linkedPortal.screen.material.SetTexture("_MainTex", viewTexture);
        }
    }

    // Sets the thickness of the portal screen so as not to clip with camera near plane when player goes through
    private float ProtectScreenFromClipping(Vector3 viewPoint)
    {
        var halfHeight = playerCam.nearClipPlane * Mathf.Tan(playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        var halfWidth = halfHeight * playerCam.aspect;
        var dstToNearClipPlaneCorner = new Vector3(halfWidth, halfHeight, playerCam.nearClipPlane).magnitude;
        var screenThickness = dstToNearClipPlaneCorner;

        var screenT = screen.transform;
        var camFacingSameDirAsPortal = Vector3.Dot(transform.forward, transform.position - viewPoint) > 0;
        screenT.localScale = new Vector3(screenT.localScale.x, screenT.localScale.y, screenThickness);
        screenT.localPosition = Vector3.forward * screenThickness * (camFacingSameDirAsPortal ? 0.5f : -0.5f);
        return screenThickness;
    }

    private void UpdateSliceParams(PortalTraveller traveller)
    {
        // Calculate slice normal
        var side = SideOfPortal(traveller.transform.position);
        var sliceNormal = transform.forward * -side;
        var cloneSliceNormal = linkedPortal.transform.forward * side;

        // Calculate slice centre
        var slicePos = transform.position;
        var cloneSlicePos = linkedPortal.transform.position;

        // Adjust slice offset so that when player standing on other side of portal to the object, the slice doesn't clip through
        float sliceOffsetDst = 0;
        float cloneSliceOffsetDst = 0;
        var screenThickness = screen.transform.localScale.z;

        var playerSameSideAsTraveller = SameSideOfPortal(playerCam.transform.position, traveller.transform.position);
        if (!playerSameSideAsTraveller) sliceOffsetDst = -screenThickness;
        var playerSameSideAsCloneAppearing = side != linkedPortal.SideOfPortal(playerCam.transform.position);
        if (!playerSameSideAsCloneAppearing) cloneSliceOffsetDst = -screenThickness;

        // Apply parameters
        for (var i = 0; i < traveller.originalMaterials.Length; i++)
        {
            traveller.originalMaterials[i].SetVector("sliceCentre", slicePos);
            traveller.originalMaterials[i].SetVector("sliceNormal", sliceNormal);
            traveller.originalMaterials[i].SetFloat("sliceOffsetDst", sliceOffsetDst);

            traveller.cloneMaterials[i].SetVector("sliceCentre", cloneSlicePos);
            traveller.cloneMaterials[i].SetVector("sliceNormal", cloneSliceNormal);
            traveller.cloneMaterials[i].SetFloat("sliceOffsetDst", cloneSliceOffsetDst);
        }
    }

    // Use custom projection matrix to align portal camera's near clip plane with the surface of the portal
    // Note that this affects precision of the depth buffer, which can cause issues with effects like screenspace AO
    private void SetNearClipPlane()
    {
        // Learning resource:
        // http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
        var clipPlane = transform;
        var dot = Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - portalCam.transform.position));

        var camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        var camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        var camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;

        // Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
        if (Mathf.Abs(camSpaceDst) > nearClipLimit)
        {
            var clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            // Update projection based on new clip plane
            // Calculate matrix with player cam so that player camera settings (fov, etc) are used
            portalCam.projectionMatrix = playerCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }
        else
        {
            portalCam.projectionMatrix = playerCam.projectionMatrix;
        }
    }

    private void OnTravellerEnterPortal(PortalTraveller traveller)
    {
        if (!trackedTravellers.Contains(traveller))
        {
            traveller.EnterPortalThreshold();
            traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
            trackedTravellers.Add(traveller);
        }
    }

    /*
     ** Some helper/convenience stuff:
     */

    private int SideOfPortal(Vector3 pos)
    {
        return Math.Sign(Vector3.Dot(pos - transform.position, transform.forward));
    }

    private bool SameSideOfPortal(Vector3 posA, Vector3 posB)
    {
        return SideOfPortal(posA) == SideOfPortal(posB);
    }
}
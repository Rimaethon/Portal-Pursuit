using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class PlayerInteract : MonoBehaviour,IInteractor
{
    [SerializeField] private GameObject interactPopUp;
    private readonly List<IInteractable> interactables=new List<IInteractable>();
    public bool isInteracting { get; set; }
    public Transform interactorTransform => gameObject.transform;
    private bool isPopUpActive;

    private void OnEnable()
    {
        EventManager.Subscribe<InteractEventArgs>( InteractMethod);
    }

    private void OnDisable()
    {
        EventManager.UnSubscribe<InteractEventArgs>( InteractMethod);
    }

    private void InteractMethod(InteractEventArgs args)
    {
        if (isInteracting) return;
        Vector3 nearestInteractable = Vector3.positiveInfinity;
        IInteractable nearestInteractableObject = null;

        foreach (IInteractable interactable in interactables)
        {
            if (!(Vector3.Distance(interactable.Position, interactorTransform.position) < Vector3.Distance(nearestInteractable, interactorTransform.position)))
                continue;
            nearestInteractable = interactable.Position;
            nearestInteractableObject = interactable;
        }

        nearestInteractableObject?.Interact(this);
        interactPopUp.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.TryGetComponent(out IInteractable interactable);
        if(interactable!=null && !interactables.Contains(interactable))
            interactables.Add(interactable);
        if (isInteracting) return;
        if(interactables.Count==1)
            interactPopUp.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isInteracting|| interactables.Count==0) return;
        interactPopUp.SetActive(interactables.Count != 0);
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.TryGetComponent(out IInteractable interactable);
        if(interactable!=null)
            interactables.Remove(interactable);
        if(interactables.Count==0&& interactPopUp!=null)
            interactPopUp.SetActive(false);
    }
}

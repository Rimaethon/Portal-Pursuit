using UnityEngine;

public interface IInteractable
{
	void Interact(IInteractor interactor);
	Vector3 Position { get; }
}

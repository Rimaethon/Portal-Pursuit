using UnityEngine;

public interface IInteractor
{
	bool isInteracting { get; set; }
	Transform interactorTransform { get; }
}

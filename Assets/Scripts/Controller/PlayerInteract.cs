using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public bool ePressed;
    private List<Transform> npclist;

    private void Update()
    {
    }

    public void InteractMethod()
    {
        if (ePressed)
        {
            var interactRange = 2f;
            var colliderArray = Physics.OverlapSphere(transform.position, interactRange);
            foreach (var collider in colliderArray) Debug.Log(collider);
        }
    }
}
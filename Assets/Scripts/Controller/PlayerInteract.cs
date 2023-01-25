using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInteract : MonoBehaviour
{
    private List<Transform> npclist;
    public bool ePressed;

    private void Update()
    {
        


    }
    public void InteractMethod()
    {
        if (ePressed)
        {
            float interactRange = 2f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider collider in colliderArray)
            {

                Debug.Log(collider);
            }

        }
    }
}

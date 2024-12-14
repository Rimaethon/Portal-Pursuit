using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NpcInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private List<string> mySentences;
    [SerializeField] private string NpcName;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI astronautNameText;
    [SerializeField] private GameObject dialoguePanel;
    public Vector3 Position => transform.position;
    private Transform player;
    private bool isInteracted;

    private void Update()
    {
        if (!isInteracted) return;
        Vector3 playerTransform = player.transform.position;
        playerTransform.y = gameObject.transform.position.y;
        transform.LookAt(playerTransform);
    }

    public void Interact(IInteractor interactor)
    {
        StartCoroutine(Dialogue(interactor));
    }

    private IEnumerator Dialogue(IInteractor interactor)
    {
        player = interactor.interactorTransform;
        isInteracted = true;
        interactor.isInteracting = true;
        astronautNameText.text = NpcName;
        dialoguePanel.SetActive(true);

        foreach (string sentence in mySentences)
        {
            messageText.text= sentence;
            yield return new WaitForSeconds(4f);
        }
        dialoguePanel.SetActive(false);
        transform.rotation = Quaternion.Euler(0, 90, 0);
        isInteracted = false;
        interactor.isInteracting = false;
        yield return null;
    }
}

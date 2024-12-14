using System.Collections;
using TMPro;
using UnityEngine;

public class RadioInteractor : MonoBehaviour
{
    public TextMeshProUGUI textRadio;
    public TextMeshProUGUI astronautNameRadio;
    public GameObject RadioBox;
    public string myFirstSentenceRadio;
    public string mySecondSentenceRadio;
    public string whatIsMyNameRadio;

    public bool radioHit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(DialogueRadio());
            radioHit = false;
        }
        else
        {
            radioHit = true;
        }
    }

    public IEnumerator DialogueRadio()
    {
        astronautNameRadio.text = whatIsMyNameRadio;
        textRadio.text = myFirstSentenceRadio;
        RadioBox.SetActive(true);
        yield return new WaitForSeconds(6f);
        textRadio.text = mySecondSentenceRadio;
        yield return new WaitForSeconds(4f);
        RadioBox.SetActive(false);
        yield return null;
    }
}

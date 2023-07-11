using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RadioInteractor : MonoBehaviour
{
    public Text textRadio;
    public string myFirstSentenceRadio;
    public string mySecondSentenceRadio;
    public Text astronoutNameRadio;
    public string whatIsMyNameRadio;
    public GameObject RadioBox;

    public bool radioHit;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(DiolougeRadio());
            radioHit = false;
        }
        else
        {
            radioHit = true;
        }
    }

    public IEnumerator DiolougeRadio()
    {
        astronoutNameRadio.text = whatIsMyNameRadio;
        textRadio.text = myFirstSentenceRadio;
        RadioBox.SetActive(true);


        //Print the time of when the function is first called.

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(6f);
        textRadio.text = mySecondSentenceRadio;
        yield return new WaitForSeconds(4f);
        RadioBox.SetActive(false);
        //After we have waited 5 seconds print the time agaiN
        yield return null;
    }
}
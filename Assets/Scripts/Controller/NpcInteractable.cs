using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class NpcInteractable : MonoBehaviour
{

    public Text text;
    public GameObject diolougeBox;
    public string myFirstSentence;
    public string mySecondSentence;
    public Text astronoutName;
    public string whatIsMyName;
    public GameObject interactBox;
   
    public void InteractBox()
    {
        
        
        
        

    }
    private void Start()
    {
        Quaternion quaternion = Quaternion.identity;
    }
    public IEnumerator Diolouge()
    {
        astronoutName.text=whatIsMyName;
        text.text = myFirstSentence;
        diolougeBox.SetActive(true);
        transform.Rotate(0, 270, 0);
       
        //Print the time of when the function is first called.

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(6f);
        text.text = mySecondSentence;
        yield return new WaitForSeconds(8f);
        diolougeBox.SetActive(false);
        transform.rotation = Quaternion.Euler(0, 90, 0);
        //After we have waited 5 seconds print the time agaiN
        yield return null;
    }
    public IEnumerator InteractInfo()
    {
        if (!diolougeBox.activeSelf)
        {
            interactBox.SetActive(true);
            yield return new WaitForSeconds(1f);
            interactBox.SetActive(false);


        }
        else
        {
            interactBox.SetActive(false);   
        }
        yield return null;
    }

    public IEnumerator DiolougeCloser()
    {
        diolougeBox.SetActive(false);
        yield return null;
    }


}

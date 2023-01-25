using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerResetPosition : MonoBehaviour
{
    // Start is called before the first frame update

    Vector3 positionReset = new Vector3(1, 1, 1);
    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition.y < -300)
        {

            SceneManager.LoadScene("Level2");   
        }   
    }
}

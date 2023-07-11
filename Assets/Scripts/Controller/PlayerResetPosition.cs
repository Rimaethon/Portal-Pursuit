using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerResetPosition : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 positionReset = new(1, 1, 1);

    // Update is called once per frame
    private void Update()
    {
        if (transform.localPosition.y < -300) SceneManager.LoadScene("Level2");
    }
}
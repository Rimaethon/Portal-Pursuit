using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScreenshotUtility : MonoBehaviour
{
    [SerializeField]
    private int imageCount;
    public bool runOnlyInEditor = true;
    public string screenshotKey = "c";
    public int scaleFactor = 1;
    public bool includeImageSizeInFilename = true;
    private const string image_cnt_key = "IMAGE_CNT";
    private static ScreenshotUtility screenShotUtility;

    private void Awake()
    {
        if (screenShotUtility != null)
        {
            Destroy(gameObject);
        }
        else if (runOnlyInEditor && !Application.isEditor)
        {
            Destroy(gameObject);
        }
        else
        {
            screenShotUtility = GetComponent<ScreenshotUtility>();
            DontDestroyOnLoad(gameObject);
            imageCount = PlayerPrefs.GetInt(image_cnt_key);
            if (!Directory.Exists("Screenshots")) Directory.CreateDirectory("Screenshots");
        }
    }

    private void Update()
    {
        if (Keyboard.current.FindKeyOnCurrentKeyboardLayout(screenshotKey).wasPressedThisFrame) TakeScreenshot();
    }

    [ContextMenu("Reset Counter")]
    public void ResetCounter()
    {
        imageCount = 0;
        PlayerPrefs.SetInt(image_cnt_key, imageCount);
    }

    private void TakeScreenshot()
    {
        PlayerPrefs.SetInt(image_cnt_key, ++imageCount);
        int width = Screen.width * scaleFactor;
        int height = Screen.height * scaleFactor;
        string pathname = "Screenshots/Screenshot_";
        if (includeImageSizeInFilename) pathname += width + "x" + height + "_";
        pathname += imageCount + ".png";
        ScreenCapture.CaptureScreenshot(pathname, scaleFactor);
        Debug.Log("Screenshot captured at " + pathname);
    }
}

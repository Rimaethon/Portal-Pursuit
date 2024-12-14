using UnityEngine;

public class ScaleOverTime : MonoBehaviour
{
    public AnimationCurve scaleCurve = new AnimationCurve();
    public float scaleSpeed = 1.0f;
    public bool looping = true;
    private Vector3 baseScale = Vector3.one;
    private float startTime;

    private void Start()
    {
        startTime = Time.timeSinceLevelLoad;
        baseScale = transform.localScale;
        Scale();
    }

    private void Update()
    {
        Scale();
    }

    private void Scale()
    {
        float curveTime = GetCurveTime();
        transform.localScale = baseScale * scaleCurve.Evaluate(curveTime);
    }

    private float GetCurveTime()
    {
        float curveTime = (Time.timeSinceLevelLoad - startTime) * scaleSpeed;
        if (looping) curveTime = curveTime % scaleSpeed;
        curveTime = Mathf.Clamp(curveTime, 0, 1);
        return curveTime;
    }
}

using UnityEngine;

public class CloudBehaviorTest : MonoBehaviour
{
    public Vector2 rotSpeedMinMax = new(10, 20);

    private CloudCoreTest[] cloudCentres;
    private Transform myTransform;

    private float rotSpeed;

    private void Start()
    {
        rotSpeed = Random.Range(rotSpeedMinMax.x, rotSpeedMinMax.y);
        cloudCentres = FindObjectsOfType<CloudCoreTest>();
        myTransform = transform;
    }

    private void Update()
    {
        myTransform.RotateAround(transform.parent.position, Vector3.up, Time.deltaTime * rotSpeed);
        float maxScale = 0;
        for (var i = 0; i < cloudCentres.Length; i++)
        {
            var cloudCentre = cloudCentres[i];
            var offset = myTransform.position - cloudCentre.transform.position;
            var sqrDstHorizontal = offset.x * offset.x + offset.z * offset.z;
            var sqrDstVertical = offset.y * offset.y;
            var tH = 1 - Mathf.Min(1,
                sqrDstHorizontal / (cloudCentre.falloffDstHorizontal * cloudCentre.falloffDstHorizontal));
            var tV = 1 - Mathf.Min(1, sqrDstVertical / (cloudCentre.falloffVertical * cloudCentre.falloffVertical));
            //float t = 1 - Mathf.Min (1, sqrDst / (falloffDst * falloffDst));
            maxScale = Mathf.Max(maxScale, tV * tH * cloudCentre.maxScale);
        }

        myTransform.localScale = Vector3.one * maxScale;
    }
}
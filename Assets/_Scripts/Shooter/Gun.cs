using UnityEngine;


public class Gun : MonoBehaviour
{

    public GameObject raycastFrom;
    public float minimumDistanceToAim = 10f;
    public float maxDistanceToAim = 1000f;
    public bool alwaysShootAtTarget = true;
    public Enemy enemy;
    public GameObject projectileGameObject;
    public bool childProjectileToFireLocation;
    public GameObject fireEffect;
    public Transform fireLocationTransform;
    public float fireDelay = 0.02f;
    public int maximumToFire = 1;
    public float maximumSpreadDegree;
    public bool available;
    public Animator gunAnimator;
    public string shootTriggerName = "Shoot";
    public string idleAnimationName = "Idle";
    private float ableToFireAgainTime;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        AdjustAim();
    }

    private void Setup()
    {
        if (raycastFrom != null) return;
        raycastFrom = gameObject;
    }

    private void AdjustAim()
    {
        if (alwaysShootAtTarget && enemy != null)
        {
            fireLocationTransform.LookAt(enemy.target);
            return;
        }

        Vector3 aimAtPosition = raycastFrom.transform.position + raycastFrom.transform.forward * maxDistanceToAim;
        bool hitSomething = Physics.Raycast(raycastFrom.transform.position, raycastFrom.transform.forward,
                                            out RaycastHit hitInformation);
        if (!hitSomething || hitInformation.distance > maxDistanceToAim || hitInformation.transform.CompareTag("Projectile"))
        {
            fireLocationTransform.LookAt(aimAtPosition);
        }
        else if (hitInformation.distance < minimumDistanceToAim)
        {
            aimAtPosition = raycastFrom.transform.position + raycastFrom.transform.forward * minimumDistanceToAim;
            fireLocationTransform.LookAt(aimAtPosition);
        }
        else
        {
            aimAtPosition = raycastFrom.transform.position + raycastFrom.transform.forward * hitInformation.distance;
            fireLocationTransform.LookAt(aimAtPosition);
        }
    }

    public void Fire()
    {
        bool canFire;

        if (gunAnimator != null)
            canFire = gunAnimator.GetCurrentAnimatorStateInfo(0).IsName(idleAnimationName);
        else
            canFire = ableToFireAgainTime <= Time.time;

        if (!canFire) return;

        if (projectileGameObject != null)
            for (int i = 0; i < maximumToFire; i++)
            {
                float fireDegreeX = Random.Range(-maximumSpreadDegree, maximumSpreadDegree);
                float fireDegreeY = Random.Range(-maximumSpreadDegree, maximumSpreadDegree);
                Vector3 fireRotationInEuler = fireLocationTransform.rotation.eulerAngles +
                                              new Vector3(fireDegreeX, fireDegreeY, 0);
                GameObject projectile = Instantiate(projectileGameObject, fireLocationTransform.position,
                                                    Quaternion.Euler(fireRotationInEuler), null);
                if (childProjectileToFireLocation) projectile.transform.SetParent(fireLocationTransform);
            }

        if (fireEffect != null)
            Instantiate(fireEffect, fireLocationTransform.position, fireLocationTransform.rotation,
                        fireLocationTransform);

        ableToFireAgainTime = Time.time + fireDelay;
        PlayShootAnimation();
    }

    private void PlayShootAnimation()
    {
        if (gunAnimator != null) gunAnimator.SetTrigger(shootTriggerName);
    }
}

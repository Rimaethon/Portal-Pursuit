using System.Collections;
using UnityEngine;

public class EnemyAttackerExplosive : EnemyAttacker
{
    public bool dieOnExplosion = true;
    public GameObject explostionEffect;

    protected override IEnumerator PerformAttack(Vector3 position)
    {
        OnAttackStart();
        float t = 0;
        while (t < attackDuration)
        {
            yield return null;
            t += Time.deltaTime;
        }

        SpawnExplosion();
        OnAttackEnd();
    }

    private void SpawnExplosion()
    {
        if (explostionEffect != null)
        {
            var obj = Instantiate(explostionEffect, transform.position, transform.rotation, null);
        }

        TryDie();
    }

    private void TryDie()
    {
        if (dieOnExplosion) Destroy(gameObject);
    }
}

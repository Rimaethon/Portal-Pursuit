using System.Collections;
using UnityEngine;

public class EnemyAttackerRanged : EnemyAttacker
{
    public Shooter shooter;

    protected override bool AttackPossible()
    {
        return base.AttackPossible() && shooter != null;
    }

    protected override IEnumerator PerformAttack(Vector3 position)
    {
        OnAttackStart();
        shooter.FireEquippedGun();
        float t = 0;
        while (t < attackDuration)
        {
            yield return null;
            t += Time.deltaTime;
        }

        OnAttackEnd();
    }
}

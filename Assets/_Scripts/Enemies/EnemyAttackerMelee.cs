using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackerMelee : EnemyAttacker
{
    public List<Collider> weaponColliders = new List<Collider>();

    protected override IEnumerator PerformAttack(Vector3 position)
    {
        OnAttackStart();
        SetWeaponColliders(true);
        float t = 0;
        while (t < attackDuration)
        {
            yield return null;
            t += Time.deltaTime;
        }

        SetWeaponColliders(false);
        OnAttackEnd();
    }

    protected void SetWeaponColliders(bool activation)
    {
        foreach (Collider collider in weaponColliders)
        {
            if (collider != null)
                collider.enabled = activation;
        }
    }
}

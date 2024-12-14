using System.Collections;
using UnityEngine;

public abstract class EnemyAttacker : MonoBehaviour
{
    public float attackDuration = 0.5f;
    public float cooldownDuration = 1.0f;
    private bool canAttack = true;

    protected virtual bool AttackPossible()
    {
        return canAttack;
    }

    public void Attack(Vector3 position)
    {
        if (AttackPossible()) StartCoroutine(PerformAttack(position));
    }

    protected virtual IEnumerator PerformAttack(Vector3 position)
    {
        OnAttackStart();
        yield return null;
        Debug.Log("Attack Made");
        OnAttackEnd();
    }

    protected IEnumerator Cooldown()
    {
        float t = 0;
        while (t < cooldownDuration)
        {
            yield return null;
            t += Time.deltaTime;
        }

        canAttack = true;
    }

    protected void OnAttackStart()
    {
        canAttack = false;
    }

    protected void OnAttackEnd()
    {
        StartCoroutine(Cooldown());
    }
}

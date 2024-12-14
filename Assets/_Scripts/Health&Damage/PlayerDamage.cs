using UnityEngine;

namespace _Scripts.Health_Damage
{
	public class PlayerDamage: MonoBehaviour
	{
		[SerializeField] private int teamId;
		[SerializeField] private int damageAmount = 1;


		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if(hit.gameObject.transform.position.y>transform.position.y)
				return;
			DealDamage(hit.collider);
		}


		private void DealDamage(Collider collision)
		{
			collision.gameObject.TryGetComponent(out IDamageable damageable);
			if (damageable == null) return;

			if (damageable.TeamId == teamId) return;

			damageable.TakeDamage(damageAmount);

		}

	}
}

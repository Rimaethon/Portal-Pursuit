using Managers;
using UnityEngine;

public class Health : MonoBehaviour,IDamageable
{
    public int TeamId => teamId;
    [SerializeField] private int teamId;
    [SerializeField] private float invincibilityTime = 3f;
    [SerializeField] private int currentLives = 3;
    [SerializeField] private int maximumLives = 5;
    [SerializeField] private float respawnWaitTime = 3f;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject hitEffect;
    private bool isDead;
    private bool isInvincible;
    private Vector3 respawnPosition;
    private float respawnTime;
    private float timeToBecomeDamageAbleAgain;

    private void Start()
    {
        SetRespawnPoint(transform.position);
    }

    private void Update()
    {
        InvincibilityCheck();
        RespawnCheck();
    }

    private void RespawnCheck()
    {
        if (respawnWaitTime == 0 || !isDead || currentLives <= 0) return;

        if (Time.time >= respawnTime)
            Respawn();
    }

    private void InvincibilityCheck()
    {
        if (timeToBecomeDamageAbleAgain <= Time.time) isInvincible = false;
    }

    public void SetRespawnPoint(Vector3 newRespawnPosition)
    {
        respawnPosition = newRespawnPosition;
    }

    private void Respawn()
    {
        CharacterController characterController = GetComponent<CharacterController>();
        if (characterController != null) characterController.enabled = false;
        transform.position = respawnPosition;
        transform.rotation = Quaternion.identity;
        if (characterController != null) characterController.enabled = true;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isInvincible || isDead) return;

        if (hitEffect != null) Instantiate(hitEffect, transform.position, transform.rotation, null);
        timeToBecomeDamageAbleAgain = Time.time + invincibilityTime;
        isInvincible = true;
        currentLives -= 1;
        Die();
    }

    public void AddLives(int bonusLives)
    {
        currentLives += bonusLives;
        if (currentLives > maximumLives)
            currentLives = maximumLives;
    }

    private void Die()
    {
        if (deathEffect != null) Instantiate(deathEffect, transform.position, transform.rotation, null);
        currentLives -= 1;
        if (currentLives > 0)
        {
            if (respawnWaitTime == 0)
                Respawn();
            else
                respawnTime = Time.time + respawnWaitTime;
        }
        else
        {
            if (respawnWaitTime != 0)
                respawnTime = Time.time + respawnWaitTime;
            else
                EventManager.RaiseEvent(new PlayerLoseEventArgs());
        }
    }



}

public interface IDamageable
{
    void TakeDamage(int damageAmount);
    int TeamId { get; }
}

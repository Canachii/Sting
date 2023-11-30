using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy")] public GameObject enemy;
    public int maxHealth = 10;
    protected int Health { get; set; } // 남은 체력
    public ParticleSystem particle;

    [Header("Item Drop")] public GameObject itemPrefab;
    [Range(0, 1)] public float dropChance = 1f;

    protected virtual void Awake()
    {
        // 난이도에 따른 체력 (쉬움 = 0.5x, 보통 = 1.0x, 어려움 = 1.5x)
        maxHealth = GameManager.Difficulty switch
        {
            2 => (int)(maxHealth * 1.5f),
            1 => maxHealth,
            _ => maxHealth / 2
        };
        Health = maxHealth;
    }

    public virtual void TakeDamage()
    {
        Health--;
        GameManager.Score += 10;
        UIController.instance.UpdateScore();

        if (Health > 0) return;

        OnDeath();
    }

    protected virtual void OnDeath()
    {
        // 아이템 드롭, 오브젝트 파괴
        if (itemPrefab != null && Random.value <= dropChance)
        {
            Instantiate(itemPrefab, enemy.transform.position, Quaternion.identity);
        }

        AudioManager.instance.PlaySFX("Destroy");
        particle.transform.position = enemy.transform.position;
        particle.Play();
        GameManager.Score += 100;
        UIController.instance.UpdateScore();

        GetComponent<EnemyMovement>().StopBehavior();

        Destroy(gameObject, particle.main.duration);
        Destroy(enemy);
    }
}
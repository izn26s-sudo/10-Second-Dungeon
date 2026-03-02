using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 3;
    private int currentHP;

    [Header("ノックバック")]
    [SerializeField] private float knockbackPower = 8f;
    [SerializeField] private float knockbackUpForce = 3f;

    private Rigidbody2D rb;

    private void Start()
    {
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage, Vector2 hitPoint)
    {
        Debug.Log($"[ダメージ] {gameObject.name} に {damage} ダメージ");

        currentHP -= damage;
        Debug.Log($"[HP残量] {currentHP}/{maxHP}");

        // 👇 AIにノックバック指示
        SideScrollEnemy enemy = GetComponent<SideScrollEnemy>();
        if (enemy != null)
        {
            enemy.ApplyKnockback(hitPoint);
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

        private void Die()
    {
        Debug.Log($"[死亡] {gameObject.name} が倒された");
        Destroy(gameObject);
    }
}
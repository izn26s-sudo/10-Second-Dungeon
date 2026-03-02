using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 3;
    private int currentHP;

    private Rigidbody2D rb;

    private void Start()
    {
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
    }

    // 外部から呼ぶダメージ処理
    public void TakeDamage(int damage, Vector2 hitPoint)
    {
        // SideScrollEnemy 取得
        SideScrollEnemy enemy = GetComponent<SideScrollEnemy>();

        // ① 死亡中なら何もしない
        if (enemy != null && enemy.isDead) return;

        // ② HP 減算
        currentHP -= damage;
        Debug.Log($"[ダメージ] {gameObject.name} に {damage} ダメージ → HP:{currentHP}/{maxHP}");

        // ③ HP が 0 以下なら死亡処理
        if (currentHP <= 0)
        {
            Die(); // HandleDeath を呼ぶだけ
        }
    }

    private void Die()
    {
        Debug.Log($"[死亡] {gameObject.name} が倒された");

        SideScrollEnemy enemy = GetComponent<SideScrollEnemy>();
        if (enemy != null)
        {
            enemy.HandleDeath();  // 死亡処理（吹っ飛び・削除など）を委譲
        }
    }
}
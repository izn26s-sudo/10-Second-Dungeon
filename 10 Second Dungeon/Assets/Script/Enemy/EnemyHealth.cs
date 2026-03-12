using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 3;
    private int currentHP;

    [Header("無敵時間")]
    [SerializeField] private float invincibleTime = 0.5f;     // 無敵時間
    [SerializeField] private float blinkInterval = 0.1f;      // 点滅間隔
    [Header("カメラシェイク")]
    [SerializeField] private float shakeDuration = 0.1f;
    [SerializeField] private float shakeMagnitude = 0.15f;

    [SerializeField] private GameObject damageTextPrefab;

    private CameraLookAhead cameraLookAhead;
    private bool isInvincible = false;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private void Start()
    {
        cameraLookAhead = Camera.main.GetComponent<CameraLookAhead>();
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int damage, Vector2 hitPoint)
    {
        SideScrollEnemy enemy = GetComponent<SideScrollEnemy>();

        // 死亡中 or 無敵中は無効
        if ((enemy != null && enemy.isDead) || isInvincible)
            return;

        currentHP -= damage;
        Debug.Log($"[ダメージ] {gameObject.name} → HP:{currentHP}/{maxHP}");
        if (cameraLookAhead != null)
        {
            cameraLookAhead.Shake(shakeDuration, shakeMagnitude);
        }
        if (damageTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 1f;
            GameObject obj = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity);
            DamageText dt = obj.GetComponent<DamageText>();

            if (dt == null)
            {
                Debug.LogError("DamageText が取得できていない");
            }
            else
            {
                dt.Setup(damage);
            }
        }
        if (currentHP <= 0)
        {
            Die();
            return;
        }

        // 無敵開始
        StartCoroutine(InvincibleRoutine());
    }

    private IEnumerator InvincibleRoutine()
    {
        isInvincible = true;

        float timer = 0f;
        while (timer < invincibleTime)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        isInvincible = false;
    }

    private void Die()
    {
        SideScrollEnemy enemy = GetComponent<SideScrollEnemy>();
        if (enemy != null)
            enemy.HandleDeath();
    }
}
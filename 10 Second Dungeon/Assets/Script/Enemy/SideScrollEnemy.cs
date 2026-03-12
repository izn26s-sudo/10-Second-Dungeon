using UnityEngine;

public class SideScrollEnemy : MonoBehaviour
{
    public enum EnemyState { Patrol, Chase, Attack, AttackCooldown, Dead }
    private EnemyState currentState;

    [Header("移動")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3f;

    [Header("検知")]
    [SerializeField] private float detectDistance = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask playerLayer;

    [Header("壁検知")]
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheckPoint;

    [Header("崖検知")]
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private Transform groundCheckPoint;

    [Header("攻撃判定")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private int attackDamage = 10;

    [Header("攻撃クールタイム")]
    [SerializeField] private float attackCooldown = 1.0f;
    private float lastAttackTime = -999f;

    [Header("死亡")]
    [SerializeField] private GameObject deathEffectPrefab; // Inspectorで設定可能
    [SerializeField] private float deathKnockbackY = 5f;
    public bool isDead = false;

    [Header("ノックバック")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackUpForce = 3f;
    [SerializeField] private float knockbackTime = 0.2f;
    private bool isKnockback;
    private float knockbackCounter;

    private Rigidbody2D rb;
    private Transform player;
    private int direction = 1;
    private bool canMove = true;
    private bool isAttacking = false;

    [SerializeField] private Animator anim; // 子のAnimator
    private Vector2 prevVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        ChangeState(EnemyState.Patrol);
    }

    void Update()
    {
        if (isDead) return; // 死亡中は何もしない

        // ノックバック処理
        if (isKnockback)
        {
            knockbackCounter -= Time.deltaTime;
            if (knockbackCounter <= 0)
            {
                isKnockback = false;
                canMove = true;
            }
            return;
        }

        // 状態ごとの処理
        switch (currentState)
        {
            case EnemyState.Patrol: Patrol(); break;
            case EnemyState.Chase: Chase(); break;
            case EnemyState.Attack: Attack(); break;
            case EnemyState.AttackCooldown: AttackCooldown(); break;
        }

        UpdateAnimation();
    }

    // ---------------- 状態制御 ----------------
    void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }

    void Patrol()
    {
        if (!canMove) return;

        rb.linearVelocity = new Vector2(moveSpeed * direction, rb.linearVelocity.y);

        if (IsWallAhead() || IsCliffAhead())
            Flip();

        if (DetectPlayer())
            ChangeState(EnemyState.Chase);
    }

    void Chase()
    {
        if (!canMove) return;

        float dir = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(dir * chaseSpeed, rb.linearVelocity.y);

        if (InAttackRange())
            ChangeState(EnemyState.Attack);

        if (!DetectPlayer())
            ChangeState(EnemyState.Patrol);
    }

    void Attack()
    {
        if (!canMove || isKnockback) return;

        if (!InAttackRange())
        {
            isAttacking = false;
            canMove = true;
            ChangeState(EnemyState.Chase);
            return;
        }

        if (Time.time - lastAttackTime < attackCooldown) return;

        if (!isAttacking)
        {
            isAttacking = true;
            canMove = false;
            rb.linearVelocity = Vector2.zero;
            anim.SetTrigger("Attack");
        }
    }

    void AttackCooldown()
    {
        rb.linearVelocity = Vector2.zero;

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (InAttackRange())
                ChangeState(EnemyState.Attack);
            else
                ChangeState(EnemyState.Chase);
        }
    }

    void UpdateAnimation()
    {
        if (isDead) return; // 死亡中は他のアニメを無効化
        anim.SetBool("Run", currentState == EnemyState.Patrol || currentState == EnemyState.Chase);
    }

    // ---------------- 攻撃判定 ----------------
    public void DealDamage()
    {
        if (isDead) return; // 死亡中は攻撃無効

        if (Time.time - lastAttackTime < attackCooldown) return;

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);
        if (hit != null)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(attackDamage);
        }

        lastAttackTime = Time.time;
    }

    public void EndAttack()
    {
        if (isDead) return;

        isAttacking = false;
        lastAttackTime = Time.time;
        canMove = true;
        ChangeState(EnemyState.AttackCooldown);
    }

    // ---------------- ダメージ処理 ----------------
    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        if (isDead) return;

        // ノックバック
        float dir = Mathf.Sign(transform.position.x - attackerPosition.x);
        Vector2 force = new Vector2(dir * knockbackForce, knockbackUpForce);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        isKnockback = true;
        knockbackCounter = knockbackTime;
        canMove = false;
    }

    public void ApplyKnockback(Vector2 attackerPosition)
    {
        if (isDead) return;

        float dir = Mathf.Sign(transform.position.x - attackerPosition.x);
        Vector2 force = new Vector2(dir * knockbackForce, knockbackUpForce);

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        isKnockback = true;
        knockbackCounter = knockbackTime;
        canMove = false;
        isAttacking = false;

        ChangeState(EnemyState.Patrol);
    }

    // ---------------- 死亡処理 ----------------
    public void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        ChangeState(EnemyState.Dead);

        // 移動・攻撃・ノックバック停止
        canMove = false;
        isAttacking = false;
        isKnockback = false;

        // 速度リセット
        rb.linearVelocity = Vector2.zero;

        // 上方向に吹っ飛び
        rb.AddForce(new Vector2(0, deathKnockbackY), ForceMode2D.Impulse);

        // 死亡アニメーション
        if (anim != null)
            anim.SetTrigger("Die");

        // 死亡エフェクト生成（設定があれば）
        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        // 2秒後にオブジェクト削除
        Destroy(gameObject, 2f);
    }

    // ---------------- 判定 ----------------
    bool DetectPlayer()
    {
        Vector2 dir = Vector2.right * direction;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectDistance, playerLayer);
        return hit.collider != null;
    }

    bool InAttackRange()
    {
        return Mathf.Abs(player.position.x - transform.position.x) < attackRange;
    }

    bool IsWallAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(wallCheckPoint.position, Vector2.right * direction, wallCheckDistance, groundLayer);
        return hit.collider != null;
    }

    bool IsCliffAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider == null;
    }

    void Flip()
    {
        direction *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + Vector3.right * direction * wallCheckDistance);
        }
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * groundCheckDistance);
        }
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
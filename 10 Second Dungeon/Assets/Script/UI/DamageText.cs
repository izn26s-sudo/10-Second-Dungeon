using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [Header("動き")]
    [SerializeField] private float minX = -1f;
    [SerializeField] private float maxX = 1f;
    [SerializeField] private float minY = 4f;
    [SerializeField] private float maxY = 6f;
    [SerializeField] private float gravity = 8f;

    [Header("寿命")]
    [SerializeField] private float lifeTime = 1f;

    [Header("拡大消滅")]
    [SerializeField] private float maxScale = 1.8f;

    private TextMeshPro textMesh;
    private Vector3 velocity;
    private float timer;

    void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshPro>();

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        velocity = new Vector3(randomX, randomY, 0f);
    }

    public void Setup(int damage)
    {
        textMesh.text = damage.ToString();
    }

    void Update()
    {
        // 重力
        velocity.y -= gravity * Time.deltaTime;

        // 移動
        transform.position += velocity * Time.deltaTime;

        // タイマー
        timer += Time.deltaTime;

        // 拡大
        float t = timer / lifeTime;
        float scale = Mathf.Lerp(1f, maxScale, t);
        transform.localScale = Vector3.one * scale;

        // 寿命
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
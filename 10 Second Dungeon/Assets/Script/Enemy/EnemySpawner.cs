using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("生成する敵Prefab（複数可）")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("各Prefabの生成数")]
    [SerializeField] private int[] spawnCounts;

    [Header("Raycast設定")]
    [SerializeField] private float raycastDistance = 20f;
    [SerializeField] private LayerMask groundLayer;

    [Header("地面からのオフセット")]
    [SerializeField] private float spawnOffsetY = 1f;

    private void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (enemyPrefabs.Length != spawnCounts.Length)
        {
            Debug.LogError("Prefab数とSpawnCount数が一致していません");
            return;
        }

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            for (int j = 0; j < spawnCounts[i]; j++)
            {
                SpawnOnGround(enemyPrefabs[i]);
            }
        }
    }

    private void SpawnOnGround(GameObject prefab)
    {
        Vector2 rayStart = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(
            rayStart,
            Vector2.down,
            raycastDistance,
            groundLayer
        );

        if (hit.collider != null)
        {
            Vector3 spawnPosition = new Vector3(
                transform.position.x,
                hit.point.y + spawnOffsetY,
                0f
            );

            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("地面が見つかりませんでした");
        }
    }
}
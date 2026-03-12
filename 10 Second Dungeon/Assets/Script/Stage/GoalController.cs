using UnityEngine;

public class GoalController : MonoBehaviour
{
    [SerializeField] private GameObject resultUI; // ★追加：リザルトUI
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Goal!");
            GameClear();
        }
    }

    void GameClear()
    {
        // クリア処理
        Time.timeScale = 0f; 
        resultUI.SetActive(true); // ★追加：リザルト表示
    }
}
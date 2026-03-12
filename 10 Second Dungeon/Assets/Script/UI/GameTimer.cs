using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float startTime = 60f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    private float currentTime;
    private bool isRunning = true;

    void Start()
    {
        currentTime = startTime;
        UpdateTimerUI();
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            // タイムオーバー処理をここに追加予定
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
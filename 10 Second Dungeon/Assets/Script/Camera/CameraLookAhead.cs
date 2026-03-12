using UnityEngine;
using System.Collections;

public class CameraLookAhead : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 baseOffset;
    [SerializeField] private float lookAheadDistance = 2f;
    [SerializeField] private float returnSpeed = 5f;
    [SerializeField] private float followSpeed = 5f;

    [Header("シェイク")]
    [SerializeField] private float defaultShakeDuration = 0.1f;
    [SerializeField] private float defaultShakeMagnitude = 0.15f;

    private Vector3 currentLookAhead;
    private Vector3 shakeOffset;
    private Vector3 smoothVelocity;   // ★ 追加
    private Vector3 basePosition;     // ★ 基準位置保持

    private Coroutine shakeCoroutine;
    void Start()
    {
        basePosition = transform.position;
    }
    void LateUpdate()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector3 targetLookAhead =
            new Vector3(inputX, inputY, 0) * lookAheadDistance;

        currentLookAhead = Vector3.Lerp(
            currentLookAhead,
            targetLookAhead,
            returnSpeed * Time.deltaTime
        );

        Vector3 targetPos =
            target.position + baseOffset + currentLookAhead;

        // ★ transform.positionを基準にしない
        basePosition = Vector3.Lerp(
            basePosition,
            targetPos,
            followSpeed * Time.deltaTime
        );

        // ★ 最終位置
        transform.position = basePosition + shakeOffset;
    }

    public void ShakeDefault()
    {
        Shake(defaultShakeDuration, defaultShakeMagnitude);
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            shakeOffset = new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }
}
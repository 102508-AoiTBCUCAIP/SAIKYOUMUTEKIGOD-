
using UnityEngine;

public class CPUPaddleController : MonoBehaviour
{
    [SerializeField] private Transform ball;
    [SerializeField] private float moveSpeed = 4f;

    [Header("CPUラケットの移動範囲")]
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;

    [SerializeField] private float minY = 1f;
    [SerializeField] private float maxY = 2f;

    [SerializeField] private float minZ = 3.5f;
    [SerializeField] private float maxZ = 6f;

    [Header("卓球台")]
    [SerializeField] private float tableTopY = 1.575f;

    private BoxCollider paddleCollider;

    void Awake()
    {
        paddleCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (ball == null)
            return;

        // ボールの位置を目標にする
        float targetX = Mathf.Clamp(
            ball.position.x, minX, maxX
        );

        float targetY = Mathf.Clamp(
            ball.position.y, minY, maxY
        );

        // CPU側のZ位置は現在の位置を維持
        float targetZ = transform.position.z;

        // ラケットの実際の高さを取得
        float paddleHalfHeight = 0.2f;

        if (paddleCollider != null)
        {
            paddleHalfHeight = paddleCollider.bounds.extents.y;
        }

        // 卓球台に埋まらないようにする
        float safeY = Mathf.Max(
    targetY,
    tableTopY + 0.05f
);

        Vector3 targetPosition = new Vector3(
            targetX,
            safeY,
            targetZ
        );

        // ボールを追いかける
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }
}


using UnityEngine;

public class PlayerPaddleController : MonoBehaviour
{
    [SerializeField] private HandPositionReader handPositionReader;

    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;

    [SerializeField] private float minY = 1.8f;
    [SerializeField] private float maxY = 3.5f;

    [SerializeField] private float minZ = -4f;
    [SerializeField] private float maxZ = 3f;

    [SerializeField] private float moveSpeed = 10f;

    [Header("手の大きさによる前後移動")]
    [SerializeField] private float handSizeFar = 0.08f;
    [SerializeField] private float handSizeNear = 0.25f;

    [Header("台の高さ")]
    [SerializeField] private float tableTopY = 1.575f;

    private Collider paddleCollider;

    void Start()
    {
        paddleCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (handPositionReader == null) return;
        if (!handPositionReader.IsHandDetected) return;

        Vector3 hand = handPositionReader.HandPosition;

        // 左右の移動
        float targetX = Mathf.Lerp(minX, maxX, hand.x);

        // 上下の移動
        float targetY = Mathf.Lerp(maxY, minY, hand.y);
        targetY = Mathf.Max(targetY, minY);

        // 手が大きく映るほど、カメラに近いと判断
        float depth = Mathf.InverseLerp(
            handSizeFar,
            handSizeNear,
            handPositionReader.HandSize
        );

        // 手前はマイナスZ、奥はプラスZ
        float targetZ = Mathf.Lerp(maxZ, minZ, depth);

        // ラケットが台に埋まらないようにする
        float halfHeight = paddleCollider != null
            ? paddleCollider.bounds.extents.y
            : 0.2f;

        float safeY = Mathf.Max(
            targetY,
            tableTopY + halfHeight + 0.02f
        );

        Vector3 targetPosition =
            new Vector3(targetX, safeY, targetZ);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }
}

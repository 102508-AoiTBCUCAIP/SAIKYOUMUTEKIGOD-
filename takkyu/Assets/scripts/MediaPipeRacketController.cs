
using UnityEngine;

public class MediaPipeRacketController : MonoBehaviour
{
    public HandPositionReader handReader;

    [Header("左右")]
    public float minX = -1.5f;
    public float maxX = 1.5f;
    public bool mirrorX = true;

    [Header("上下")]
    public float minY = 0.8f;
    public float maxY = 2.0f;

    [Header("前後")]
    public float minZ = -2.5f;
    public float maxZ = -0.5f;

    public float nearHandSize = 0.35f;
    public float farHandSize = 0.10f;
    public bool reverseDepth = false;

    [Header("動きの滑らかさ")]
    public float smoothSpeed = 10f;

    [Header("ラケットの傾き")]
    public float tiltMultiplier = 1f;
    public float rotationSmoothSpeed = 8f;

    private Quaternion initialRotation;
    private Vector3 previousPosition;

    public float CurrentSpeed { get; private set; }

    void Start()
    {
        previousPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (handReader == null) return;

        Vector3 hand;
        float size;

        if (!handReader.TryGetHand(out hand, out size))
            return;

        float hx = Mathf.Clamp01(hand.x);
        float hy = Mathf.Clamp01(hand.y);

        if (mirrorX)
            hx = 1f - hx;

        float x = Mathf.Lerp(minX, maxX, hx);
        float y = Mathf.Lerp(maxY, minY, hy);

        float depth = Mathf.InverseLerp(
            farHandSize, nearHandSize, size
        );

        if (reverseDepth)
            depth = 1f - depth;

        float z = Mathf.Lerp(minZ, maxZ, depth);

        Vector3 target = new Vector3(x, y, z);

        float t = 1f - Mathf.Exp(
            -smoothSpeed * Time.deltaTime
        );

        transform.position = Vector3.Lerp(
            transform.position, target, t
        );

        CurrentSpeed =
            (transform.position - previousPosition).magnitude
            / Mathf.Max(Time.deltaTime, 0.0001f);

        previousPosition = transform.position;


        // 手の傾きを取得
        Vector3 tilt;

        Quaternion targetRotation = initialRotation;

        if (handReader.TryGetHandTilt(out tilt))
        {
            // 傾きの強さを制限
            float tiltX = Mathf.Clamp(
                -tilt.x * 0.35f,
                -25f,
                25f
            );

            float tiltZ = Mathf.Clamp(
                -tilt.z * 0.35f,
                -25f,
                25f
            );

            // ラケットの目標角度
            targetRotation =
                initialRotation *
                Quaternion.Euler(tiltX, 0f, tiltZ);
        }

        // 滑らかに回転
        float rotationT = 1f - Mathf.Exp(
            -10f * Time.deltaTime
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationT
        );

    }
}



using UnityEngine;

public class RacketTiltController : MonoBehaviour
{
    [Header("傾き設定")]
    [SerializeField] private float maxTiltAngle = 25f;
    [SerializeField] private float tiltSensitivity = 12f;
    [SerializeField] private float smoothSpeed = 8f;

    private Quaternion initialRotation;
    private Vector3 previousPosition;

    void Start()
    {
        // 最初のラケットの角度を保存
        initialRotation = transform.localRotation;

        previousPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        // ラケットの移動量を取得
        Vector3 currentPosition = transform.localPosition;

        Vector3 movement =
            (currentPosition - previousPosition) /
            Mathf.Max(Time.deltaTime, 0.0001f);

        previousPosition = currentPosition;

        // 左右・前後の動きから傾きを計算
        float tiltZ = Mathf.Clamp(
            -movement.x * tiltSensitivity,
            -maxTiltAngle,
            maxTiltAngle
        );

        float tiltX = Mathf.Clamp(
            movement.z * tiltSensitivity,
            -maxTiltAngle,
            maxTiltAngle
        );

        // 目標の角度
        Quaternion targetRotation =
            initialRotation * Quaternion.Euler(tiltX, 0f, tiltZ);

        // なめらかに回転
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * smoothSpeed
        );
    }
}


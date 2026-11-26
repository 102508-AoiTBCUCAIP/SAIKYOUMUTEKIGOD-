
using UnityEngine;

public class PlayerPaddleHit : MonoBehaviour
{
    [SerializeField] private float hitSpeed = 6f;

    [Header("打ち分け")]
    [SerializeField] private float sidePower = 2f;
    [SerializeField] private float upPower = 1.5f;

    [Header("スマッシュ")]
    [SerializeField] private MediaPipeRacketController racketController;
    [SerializeField] private float slowSpeed = 1f;
    [SerializeField] private float fastSpeed = 8f;
    [SerializeField] private float maxPower = 2f;

    [Header("スマッシュエフェクト")]
    [SerializeField] private GameObject smashEffectPrefab;
    [SerializeField] private float smashEffectThreshold = 0.7f;

    [Header("スマッシュ演出")]
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private AudioSource smashAudio;
    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeMagnitude = 0.15f;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ball"))
            return;

        Rigidbody ballRb =
            collision.gameObject.GetComponent<Rigidbody>();

        if (ballRb == null)
            return;

        Vector3 hitPoint = collision.contacts[0].point;

        Vector3 localHitPoint =
            transform.InverseTransformPoint(hitPoint);

        float directionX =
            Mathf.Clamp(localHitPoint.x * sidePower, -1f, 1f);

        float directionY =
            Mathf.Clamp(localHitPoint.y * upPower, -0.5f, 1.5f);

        Vector3 direction = new Vector3(
            directionX,
            0.8f + directionY,
            1f
        ).normalized;

        // ラケットの速度から威力を計算
        float racketSpeed = 0f;

        if (racketController != null)
        {
            racketSpeed = racketController.CurrentSpeed;
        }

        float powerRate = Mathf.InverseLerp(
            slowSpeed,
            fastSpeed,
            racketSpeed
        );

        float finalPower = Mathf.Lerp(
            1f,
            maxPower,
            powerRate
        );

        // スマッシュしたときだけエフェクトを出す
        if (powerRate >= smashEffectThreshold)
        {
            if (smashEffectPrefab != null)
            {
                Instantiate(
                    smashEffectPrefab,
                    hitPoint,
                    Quaternion.identity
                );
            }

            // 画面を揺らす
            if (cameraShake != null)
            {
                cameraShake.Shake(shakeDuration, shakeMagnitude);
            }

            // 効果音を鳴らす
            if (smashAudio != null)
            {
                smashAudio.Play();
            }
        }

        // ボールを打ち返す
        ballRb.velocity = direction * hitSpeed * finalPower;
    }
}

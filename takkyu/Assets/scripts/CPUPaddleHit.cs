
using UnityEngine;

public class CPUPaddleHit : MonoBehaviour
{
    [SerializeField] private float hitSpeed = 6f;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ball"))
            return;

        Rigidbody ballRb =
            collision.gameObject.GetComponent<Rigidbody>();

        if (ballRb == null)
            return;

        // 横方向への動きをなくしてプレイヤー側へ返す
        Vector3 direction = new Vector3(0f, 0.8f, -1f).normalized;
        ballRb.velocity = direction * hitSpeed;
    }
}

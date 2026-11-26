
using UnityEngine;

public class BallOutDetector : MonoBehaviour
{
    private bool scored = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball"))
            return;

        if (scored)
            return;

        scored = true;

        // プレイヤー側（Zがマイナス）に落ちたらCPUの得点
        if (other.transform.position.z < 0)
        {
            ScoreManager.Instance.AddCPUPoint();
        }
        // CPU側（Zがプラス）に落ちたらプレイヤーの得点
        else
        {
            ScoreManager.Instance.AddPlayerPoint();
        }

        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        other.transform.position = new Vector3(0f, 2f, -2f);

        scored = false;
    }
}


using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int playerScore = 0;
    public int cpuScore = 0;

    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI cpuScoreText;

    private void Awake()
    {
        Instance = this;
    }

    public void AddPlayerPoint()
    {
        playerScore++;
        UpdateScore();
        CheckWinner();
    }

    public void AddCPUPoint()
    {
        cpuScore++;
        UpdateScore();
        CheckWinner();
    }

    void UpdateScore()
    {
        playerScoreText.text = playerScore.ToString();
        cpuScoreText.text = cpuScore.ToString();
    }

    void CheckWinner()
    {
        if ((playerScore >= 11 || cpuScore >= 11) &&
            Mathf.Abs(playerScore - cpuScore) >= 2)
        {
            Debug.Log(playerScore > cpuScore ?
                "PLAYER WIN!" : "CPU WIN!");
        }
    }
}

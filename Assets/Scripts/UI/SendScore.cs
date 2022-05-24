using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SendScore : MonoBehaviour
{
    public TMP_InputField inputName;
    public Button sendButton;

    // Start is called before the first frame update
    void Awake()
    {
        inputName = GetComponentInChildren<TMP_InputField>();
        sendButton = GetComponentInChildren<Button>();

        sendButton.onClick.AddListener(SendScoreButton);
    }

    void SendScoreButton()
    {
        LeaderboardController.AddNewScore(inputName.text, Game._score);
        LeaderboardController.SaveLeaderboard();
    }
}

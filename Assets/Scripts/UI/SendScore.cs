using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SendScore : MonoBehaviour
{
    public TMP_InputField inputName;
    public Button sendButton;

    private Game gameIns;

    void Awake()
    {
        inputName = GetComponentInChildren<TMP_InputField>();
        sendButton = GetComponentInChildren<Button>();

        sendButton.onClick.AddListener(SendScoreButton);

        gameIns = GameObject.FindObjectOfType<Game>();
    }

    public void SendScoreButton()
    {
        if(inputName.text.Length == 0)
        {
            //Debug.Log("Complete the name field");
            ShowError();
            inputName.Select();
            return;
        }

        LeaderboardController.AddNewScore(inputName.text, Game._score);
        LeaderboardController.SaveLeaderboard();

        gameObject.SetActive(false);

        gameIns.Restart();
    }

    void ShowError()
    {
        inputName.GetComponent<Image>().color = Color.red;
        Invoke("HideError", 1.5f);
    }

    void HideError()
    {
        inputName.GetComponent<Image>().color = Color.white;
    }

    private void OnEnable()
    {
        inputName.Select();
    }
}

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class LeaderboardController : MonoBehaviour
{
    [System.Serializable]
    public class Score
    {
        public Score(string n, int s)
        {
            name = n;
            score = s;
        }

        public string name;
        public int score;
    };

    [System.Serializable]
    public class Leaderboard
    {
        public float ver = 0.1f;
        public string date = "05/21/2022";
        public List<Score> m_leaderboard = new List<Score>();

        public void Add(string name, int score)
        {
            Score newScore = new Score(name, score);
            m_leaderboard.Add(newScore);
        }
    }

    [SerializeField] static public Leaderboard m_leaderboard = new Leaderboard();

    public GameObject score_ui_element;
    public GameObject viewport_content;

    // Start is called before the first frame update
    void Start()
    {
        /*AddNewScore("Monti2", 64);
        AddNewScore("Francisco", 4800);
        AddNewScore("Francisco", 48);

        SaveLeaderboard();*/
        //ClearLeaderboard();
        LoadLeaderboard();
        UpdateScoreUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadLeaderboard()
    {
        var reader = File.OpenText(Application.persistentDataPath + "/Leaderboard.json");
        string json = "";

        while (!reader.EndOfStream)
        {
            json = reader.ReadLine();
            // Do Something with the input. 
        }

        reader.Close();

        Debug.Log(json);
        JsonUtility.FromJsonOverwrite(json, m_leaderboard);
    }

    public void UpdateScoreUI()
    {
        int index = 1;
        foreach (Score score in m_leaderboard.m_leaderboard)
        {
            var element = Instantiate(score_ui_element, viewport_content.transform);
            element.transform.GetChild(0).GetComponent<TMP_Text>().text = index.ToString();
            element.transform.GetChild(1).GetComponent<TMP_Text>().text = score.name;
            element.transform.GetChild(2).GetComponent<TMP_Text>().text = score.score.ToString();

            RankingCustomColor(element.transform.GetChild(0).GetComponent<TMP_Text>(), index);
            index++;

        }
    }

    private void RankingCustomColor(TMP_Text text, int index)
    {
        if (index == 1)
        {
            text.color = new Color(.99215f, 0.8823f, 0f, 1f);
        } else if(index == 2)
        {
            text.color = new Color(0.7921f, 0.7725f, 0.7921f, 1f);
        } else if(index == 3)
        {
            text.color = new Color(0.6901f, 0.2941f, 0.0823f, 1f);
        }
    }

    static public void AddNewScore(string name, int score)
    {
        m_leaderboard.Add(name, score);
    }

    static int SortByScore(Score p1, Score p2)
    {
        return p1.score.CompareTo(p2.score);
    }

    static public void SaveLeaderboard()
    {
        m_leaderboard.m_leaderboard.Sort(SortByScore);
        m_leaderboard.m_leaderboard.Reverse();

        string data = JsonUtility.ToJson(m_leaderboard);
        File.WriteAllText(Application.persistentDataPath + "/Leaderboard.json", data);
    }

    static public void ClearLeaderboard()
    {
        Leaderboard newLeaderboard = new Leaderboard();

        string data = JsonUtility.ToJson(newLeaderboard);
        File.WriteAllText(Application.persistentDataPath + "/Leaderboard.json", data);
    }

    static public bool UpdateHighScore(int newScore)
    {
        Debug.Log("New score: " + newScore.ToString());
        Debug.Log("Last score: " + Game._highScore.ToString());

        if (newScore >= Game._highScore)
        {
            return true;
        }

        return false;
    }
}

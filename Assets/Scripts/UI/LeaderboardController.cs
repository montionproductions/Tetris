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
        public string date = "05/20/2022";
        public List<Score> m_leaderboard = new List<Score>();

        public void Add(string name, int score)
        {
            Score newScore = new Score(name, score);
            m_leaderboard.Add(newScore);
        }
    }

    [SerializeField] public Leaderboard m_leaderboard = new Leaderboard();

    public GameObject score_ui_element;
    public GameObject viewport_content;

    // Start is called before the first frame update
    void Start()
    {
        //AddNewScore("Monti", 254);
        //AddNewScore("Francisco", 4800);

        //SaveLeaderboard();
        LoadLeaderboard();
        UpdateScoreUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadLeaderboard()
    {
        var reader = File.OpenText(Application.dataPath + "/Leaderboard.json");
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
        foreach (Score score in m_leaderboard.m_leaderboard)
        {
            var element = Instantiate(score_ui_element, viewport_content.transform);
            element.transform.GetChild(0).GetComponent<TMP_Text>().text = "0";
            element.transform.GetChild(1).GetComponent<TMP_Text>().text = score.name;
            element.transform.GetChild(2).GetComponent<TMP_Text>().text = score.score.ToString();

        }
    }

    public void AddNewScore(string name, int score)
    {
        m_leaderboard.Add(name, score);
    }

    public int SortByScore(Score p1, Score p2)
    {
        return p1.score.CompareTo(p2.score);
    }

    public void SaveLeaderboard()
    {
        string data = JsonUtility.ToJson(m_leaderboard);
        File.WriteAllText(Application.dataPath + "/Leaderboard.json", data);
    }
}

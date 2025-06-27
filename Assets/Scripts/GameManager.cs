using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerMode {
    Single,
    Versus
}

public class GameManager : MonoBehaviour {
    public GameObject Player1;
    public GameObject Player2;
    public Slider Player1HealthSlider;
    public Slider Player2HealthSlider;
    private bool isOver = false;
    private PlayerID winner;
    public int score;
    public float scoreMultiplier;
    public MapSectionManager msManager;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    [Header("Difficulty Settings")]
    public float velocity = 15f;
    private float startVelocity, endVelocity;
    public float velocityFactor = 0.02f;
    private float difficulty;
    public int maxVelocityTime = 220;
    public float time;
    void Awake() {
        Time.timeScale = 1;
        score = 0;
        Debug.Log("Awake: Score = " + score);
        difficulty = PlayerPrefs.GetFloat("Pace", 0.5f);
        SetStartVelocity(difficulty);
        SetEndVelocity(difficulty);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        Debug.Log("StarVelocity = " + startVelocity + "\tEndVelocity = " + endVelocity);

        if (MainMenu.mode == PlayerMode.Single) {
            Player2.SetActive(false);
            Player2HealthSlider.gameObject.SetActive(false);
            Player1.GetComponent<Rigidbody>().position = new Vector3(0f, 0.1f, 0f);
        }
        else {
            Player2.SetActive(true);
            Player1.GetComponent<Rigidbody>().position = new Vector3(0f, 0.1f, -2.5f);
            Player2.GetComponent<Rigidbody>().position = new Vector3(0f, 0.1f, 2.5f);
        }
    }

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;
        SetScore();
        SetCurrentVelocity();

        int player1Health = Player1.GetComponent<PlayerHealth>().GetPlayerHealth();
        int player2Health = Player2.GetComponent<PlayerHealth>().GetPlayerHealth();

        Player1HealthSlider.value = player1Health;
        Player2HealthSlider.value = player2Health;

        if (player1Health <= 0 && player2Health <= 0) {
            GameOver();
            winner = PlayerID.None;
        }
        else if (player1Health <= 0) {
            GameOver();
            winner = PlayerID.Player2;
        }
        else if (player2Health <= 0) {
            GameOver();
            winner = PlayerID.Player1;
        }
        else {
            SetScore();
            Debug.Log("Score = " + score);
            scoreText.text = score.ToString();
        }
    }

    private void SetStartVelocity(float difficulty) {
        startVelocity = 5 * difficulty + 15;
    }

    private void SetEndVelocity(float difficulty) {
        endVelocity = 12 * difficulty + 18;
    }

    private void SetCurrentVelocity() {
        float t = time;
        if (t > maxVelocityTime) {
            velocity = endVelocity;
        }
        else {
            velocity = GetAcceleration() * t + startVelocity;
        }
    }

    public float GetCurrentVelocity() {
        return velocity;
    }

    public float GetAcceleration() {
        return (endVelocity - startVelocity) / maxVelocityTime;
    }

    private void GameOver() {
        isOver = true;
        SetHighScore();
    }

    public void SetScore() {
        float t = time;
        float score;
        if (t <= maxVelocityTime) {
            score = (GetAcceleration() / 2 * t + startVelocity) * t;
            Debug.Log("");
        }
        else {
            score = endVelocity * t;
        }
        score *= scoreMultiplier;
        this.score = (int)score;
    }

    private void SetHighScore() {
        if (MainMenu.mode == PlayerMode.Single) {
            if (score > PlayerPrefs.GetInt("HighscoreSingle", 0)) {
                PlayerPrefs.SetInt("HighscoreSingle", score);
                highScoreText.text = "New Single Highscore: " + score.ToString();
            }
            else {
                highScoreText.text = "Current Single Highscore: " + PlayerPrefs.GetInt("HighscoreSingle", 0);
            }
        }
        else {
            if (score > PlayerPrefs.GetInt("HighscoreVersus", 0)) {
                PlayerPrefs.SetInt("HighscoreVersus", score);
                highScoreText.text = "New Versus Highscore: " + score.ToString();
            }
            else {
                highScoreText.text = "Current Versus Highscore: " + PlayerPrefs.GetInt("HighscoreVersus", 0);
            }
        }
        scoreText.text = "Score: " + score;
    }

    public bool IsOver() {
        return isOver;
    }

    public PlayerID GetWinner() {
        return winner;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfGameMenu : MonoBehaviour {
    public GameObject endPanel;
    public GameObject backgroundPanel;
    public TMP_Text playerWonText;
    public GameManager gameManager;
    private bool showEndScreen = true;

    void Update() {
        if (gameManager.IsOver() && showEndScreen) {
            showEndScreen = false;
            End();
        }
    }

    public void End() {
        PlayerID winner = gameManager.GetWinner();
        playerWonText.text = winner == PlayerID.None ? "Draw!" : (winner == PlayerID.Player1 ? "Blue " : "Red ") + "Player won!";
        endPanel.SetActive(true);
        backgroundPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Restart() {
        SceneManager.LoadScene(1);
    }

    public void QuitToMainMenu() {
        SceneManager.LoadScene(0);
    }
}
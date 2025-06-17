using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfGameMenu : MonoBehaviour {
    public GameObject endPanel;
    public GameObject backgroundPanel;
    public TMP_Text playerWonText;
    public GameManager gameManager;

    void Update() {
        if (gameManager.IsOver()) {
            PlayerID winner = gameManager.GetWinner();
            playerWonText.text = winner == PlayerID.None ? "Draw!" : (winner == PlayerID.Player1 ? "Blue " : "Red ") + "Player won!";
            End();
        }
    }

    public void End() {
        if (gameManager.IsOver()) {
            endPanel.SetActive(true);
            backgroundPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void Restart() {
        SceneManager.LoadScene(1);
    }

    public void QuitToMainMenu() {
        SceneManager.LoadScene(0);
    }
}
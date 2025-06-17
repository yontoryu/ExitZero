using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public GameObject Player1;
    public GameObject Player2;
    public Slider Player1HealthSlider;
    public Slider Player2HealthSlider;
    private bool isOver = false;

    private PlayerID winner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update() {
        int player1Health = Player1.GetComponent<PlayerHealth>().GetPlayerHealth();
        int player2Health = Player2.GetComponent<PlayerHealth>().GetPlayerHealth();

        Player1HealthSlider.value = player1Health;
        Player2HealthSlider.value = player2Health;

        if (player1Health <= 0 && player2Health <= 0) {
            isOver = true;
            winner = PlayerID.None;
        }
        else if (player1Health <= 0) {
            isOver = true;
            winner = PlayerID.Player2;
        }
        else if (player2Health <= 0) {
            isOver = true;
            winner = PlayerID.Player1;
        }
    }

    public bool IsOver() {
        return isOver;
    }

    public PlayerID GetWinner() {
        return winner;
    }
}

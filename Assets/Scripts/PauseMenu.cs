using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    public AudioClip mainMenuIntroClip;
    public AudioClip mainMenuLoopClip;
    public GameObject pausePanel;
    public GameObject backgroundPanel;
    private bool isPaused;
    private string inputPause;

    void Awake() {
        inputPause = "Pause";
    }

    void Start() {
        MusicManager.Instance.PlayMusic(mainMenuIntroClip, mainMenuLoopClip);
    }

    void Update() {
        if (!isPaused && Input.GetButtonDown(inputPause)) {
            isPaused = true;
            Pause();
        }
        else if (isPaused && Input.GetButtonDown(inputPause)) {
            isPaused = false;
            Resume();
        }
    }

    public void Pause() {
        pausePanel.SetActive(true);
        backgroundPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume() {
        pausePanel.SetActive(false);
        backgroundPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void QuitToMainMenu() {
        SceneManager.LoadScene(0);
    }
}
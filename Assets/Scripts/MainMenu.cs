using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public static PlayerMode mode = PlayerMode.Versus;

    public void PlaySingle() {
        mode = PlayerMode.Single;
        SceneManager.LoadScene(1);
    }

    public void PlayVersus() {
        mode = PlayerMode.Versus;
        SceneManager.LoadScene(1);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
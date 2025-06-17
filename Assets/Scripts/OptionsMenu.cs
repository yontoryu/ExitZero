using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {
    public AudioMixer mixer;
    public Slider volumeSlider;
    public Slider paceSlider;
    Resolution[] resolutions;
    public TMP_Dropdown resDropdown;

    void Start() {
        SetVolumeOnStart();
        SetPaceOnStart();
        SetResolutionDropdown();
    }

    private void SetResolutionDropdown() {
        resolutions = Screen.resolutions;
        resDropdown.ClearOptions();

        List<string> options = new List<string>();
        int curResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++) {
            Resolution res = resolutions[i];
            string option = res.width + " x " + res.height;
            options.Add(option);

            if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height) {
                curResIndex = i;
            }
        }

        resDropdown.AddOptions(options);
        resDropdown.value = curResIndex;
        resDropdown.RefreshShownValue();
    }

    public void SetResolution(int resIndex) {
        Resolution resolution = resolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void SetVolumeOnStart() {
        float volume = PlayerPrefs.GetFloat("Volume", 0.7f);
        volumeSlider.value = volume;

        mixer.SetFloat("Volume", MusicManager.LinearToDecibel(volume));
    }

    public void SetVolumeSlider(float linearVolume) {
        mixer.SetFloat("Volume", MusicManager.LinearToDecibel(linearVolume));
        PlayerPrefs.SetFloat("Volume", linearVolume);
    }

    private void SetPaceOnStart() {
        float pace = PlayerPrefs.GetFloat("Pace", 0.5f);
        paceSlider.value = pace;
    }

    public void SetPaceSlider(float pace) {
        PlayerPrefs.SetFloat("Pace", pace);
    }

    public void SetFullScreen(bool isFullscreen) {
        Screen.fullScreen = isFullscreen;
    }
}

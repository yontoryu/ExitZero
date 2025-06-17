using System.Collections;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour {
    private static MusicManager Instance;
    public AudioSource introSource;
    public AudioSource loopSource;
    public AudioMixer mixer;

    [Header("Config")]
    public string mixerVolume = "MusicVolume";
    public float fadeInTime = 2f;

    private void Awake() {
        float volume = PlayerPrefs.GetFloat("Volume", 0.7f);
        mixer.SetFloat("Volume", LinearToDecibel(volume));
    }

    void Start() {
        PlayMusic(introSource.clip, loopSource.clip);
    }

    public void PlayMusic(AudioClip introClip, AudioClip loopClip) {
        introSource.clip = introClip;
        loopSource.clip = loopClip;

        introSource.loop = true;
        loopSource.loop = true;

        // loopSource.PlayDelayed(introSource.clip.length);
        introSource.Play();

        StartCoroutine(FadeInAudio(introSource, fadeInTime, 1f));
    }

    private IEnumerator FadeInAudio(AudioSource audio, float duration, float targetVol) {
        float timer = 0f;
        while (timer < duration) {
            timer += Time.deltaTime;
            audio.volume = Mathf.Lerp(0f, targetVol, timer / duration);
            yield return null;
        }
    }

    public void StopMusic() {
        introSource.Stop();
        loopSource.Stop();
    }

    public static float LinearToDecibel(float linearValue) {
        if (linearValue <= 0.0001f) {
            return -80f;
        }

        return Mathf.Log(linearValue) * 20f;
    }

    public static float DecibelToLinear(float dB) {
        return Mathf.Exp(dB / 20f);
    }
}

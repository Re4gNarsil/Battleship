using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    static bool hardMode = true;
	static bool CPUMode = true;
	static float audioLevel = .5f;
	static float levelSpeed = 1f;

	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.volume = audioLevel;
		FindObjectOfType<DayNight>().SetSpeed(levelSpeed);
	}

	public bool SetOpponent() {
        return CPUMode;
    }

	public bool SetDifficulty()
	{
		return hardMode;
	}

	public void LoadLevel(string name) {
        if (name == "Hard") {
			hardMode = true;
            SceneManager.LoadScene("Scene01");

        } else if (name == "Easy") {
			hardMode = false;
            SceneManager.LoadScene("Scene01");
        }
    }

	public void SetAudio(Slider slider)
	{
		audioLevel = slider.value;
		audioSource.volume = audioLevel;
	}

	public void SetSpeed(Slider slider)
	{
		levelSpeed = slider.value;
		FindObjectOfType<DayNight>().SetSpeed(levelSpeed);
	}
}

using UnityEngine;
using System.Collections;

public class stealthSounds : MonoBehaviour {

	public AudioClip audioCalm;
	public AudioClip audioSpotted;
	public AudioClip audioAlarm;
	AudioSource audio;
	alertStatus curState;

	// Use this for initialization
	void Awake()
	{
		audio = this.gameObject.GetComponent<AudioSource>();
        audio.volume = 0.1f;
		curState = alertStatus.calm;
	}

	public void playCalm()
	{
		if (curState != alertStatus.calm)
		{
			audio.clip = audioCalm;
			audio.Play();
			curState = alertStatus.calm;
		}	
	}

	public void playSpotted()
	{
		if (curState != alertStatus.spotted)
		{
			audio.clip = audioSpotted;
			audio.Play();
			curState = alertStatus.spotted;
		}		
	}

	public void playAlarm()
	{
		if (curState != alertStatus.alert)
		{
			audio.clip = audioAlarm;
			audio.Play();
			curState = alertStatus.alert;
		}
	}
}

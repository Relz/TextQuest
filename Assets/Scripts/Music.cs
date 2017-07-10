using UnityEngine;
using System.Collections;

namespace TextQuest
{
	public class Music : MonoBehaviour
	{
		Object[] music;

		void Awake()
		{
			music = Resources.LoadAll("Music", typeof(AudioClip));
			audioSource.clip = music[0] as AudioClip;
		}

		void Start()
		{
			audioSource.Play();
		}
		
		void Update()
		{
			if (!audioSource.isPlaying)
			{
				PlayRandomMusic();
			}
		}

		void PlayRandomMusic()
		{
			audioSource.clip = music[Random.Range(0, music.Length)] as AudioClip;
			audioSource.Play();
		}

		public AudioSource audioSource;
	}
}
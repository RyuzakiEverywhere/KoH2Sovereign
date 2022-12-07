using System;
using UnityEngine;

// Token: 0x02000025 RID: 37
[RequireComponent(typeof(AudioSource))]
public class FlockChildSound : MonoBehaviour
{
	// Token: 0x0600009E RID: 158 RVA: 0x00005E10 File Offset: 0x00004010
	public void Start()
	{
		this._flockChild = base.GetComponent<FlockChild>();
		this._audio = base.GetComponent<AudioSource>();
		base.InvokeRepeating("PlayRandomSound", Random.value + 1f, 1f);
		if (this._scareSounds.Length != 0)
		{
			base.InvokeRepeating("ScareSound", 1f, 0.01f);
		}
	}

	// Token: 0x0600009F RID: 159 RVA: 0x00005E70 File Offset: 0x00004070
	public void PlayRandomSound()
	{
		if (base.gameObject.activeInHierarchy)
		{
			if (!this._audio.isPlaying && this._flightSounds.Length != 0 && this._flightSoundRandomChance > Random.value && !this._flockChild._landing)
			{
				this._audio.clip = this._flightSounds[Random.Range(0, this._flightSounds.Length)];
				this._audio.pitch = Random.Range(this._pitchMin, this._pitchMax);
				this._audio.volume = Random.Range(this._volumeMin, this._volumeMax);
				this._audio.Play();
				return;
			}
			if (!this._audio.isPlaying && this._idleSounds.Length != 0 && this._idleSoundRandomChance > Random.value && this._flockChild._landing)
			{
				this._audio.clip = this._idleSounds[Random.Range(0, this._idleSounds.Length)];
				this._audio.pitch = Random.Range(this._pitchMin, this._pitchMax);
				this._audio.volume = Random.Range(this._volumeMin, this._volumeMax);
				this._audio.Play();
				this._hasLanded = true;
			}
		}
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x00005FC4 File Offset: 0x000041C4
	public void ScareSound()
	{
		if (base.gameObject.activeInHierarchy && this._hasLanded && !this._flockChild._landing && this._idleSoundRandomChance * 2f > Random.value)
		{
			this._audio.clip = this._scareSounds[Random.Range(0, this._scareSounds.Length)];
			this._audio.volume = Random.Range(this._volumeMin, this._volumeMax);
			this._audio.PlayDelayed(Random.value * 0.2f);
			this._hasLanded = false;
		}
	}

	// Token: 0x040000DB RID: 219
	public AudioClip[] _idleSounds;

	// Token: 0x040000DC RID: 220
	public float _idleSoundRandomChance = 0.05f;

	// Token: 0x040000DD RID: 221
	public AudioClip[] _flightSounds;

	// Token: 0x040000DE RID: 222
	public float _flightSoundRandomChance = 0.05f;

	// Token: 0x040000DF RID: 223
	public AudioClip[] _scareSounds;

	// Token: 0x040000E0 RID: 224
	public float _pitchMin = 0.85f;

	// Token: 0x040000E1 RID: 225
	public float _pitchMax = 1f;

	// Token: 0x040000E2 RID: 226
	public float _volumeMin = 0.6f;

	// Token: 0x040000E3 RID: 227
	public float _volumeMax = 0.8f;

	// Token: 0x040000E4 RID: 228
	private FlockChild _flockChild;

	// Token: 0x040000E5 RID: 229
	private AudioSource _audio;

	// Token: 0x040000E6 RID: 230
	private bool _hasLanded;
}

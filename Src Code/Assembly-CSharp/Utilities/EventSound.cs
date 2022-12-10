using System;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000467 RID: 1127
	[Serializable]
	public class EventSound
	{
		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x06003B41 RID: 15169 RVA: 0x001C6776 File Offset: 0x001C4976
		// (set) Token: 0x06003B40 RID: 15168 RVA: 0x001C676D File Offset: 0x001C496D
		public float VolumeWeight
		{
			get
			{
				return this.volumeWeight;
			}
			set
			{
				this.volumeWeight = value;
			}
		}

		// Token: 0x06003B42 RID: 15170 RVA: 0x001C6780 File Offset: 0x001C4980
		public void PlayAudio(AudioSource audio)
		{
			if (audio == null)
			{
				return;
			}
			if (this.Clips == null || this.Clips.Length == 0)
			{
				return;
			}
			audio.spatialBlend = 1f;
			audio.clip = this.Clips[Random.Range(0, this.Clips.Length)];
			audio.pitch *= this.pitch;
			audio.volume = Mathf.Clamp01(this.volume * this.VolumeWeight);
			audio.Play();
		}

		// Token: 0x04002B18 RID: 11032
		public string name = "Name Here";

		// Token: 0x04002B19 RID: 11033
		public AudioClip[] Clips;

		// Token: 0x04002B1A RID: 11034
		public float volume = 1f;

		// Token: 0x04002B1B RID: 11035
		public float pitch = 1f;

		// Token: 0x04002B1C RID: 11036
		protected float volumeWeight = 1f;
	}
}

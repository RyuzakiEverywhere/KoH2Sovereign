using System;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000466 RID: 1126
	public class AnimatorEventSounds : MonoBehaviour
	{
		// Token: 0x06003B3D RID: 15165 RVA: 0x001C665C File Offset: 0x001C485C
		private void Start()
		{
			this.anim = base.GetComponent<Animator>();
			if (this._audioSource == null)
			{
				this._audioSource = base.gameObject.AddComponent<AudioSource>();
			}
			this._audioSource.volume = 0f;
		}

		// Token: 0x06003B3E RID: 15166 RVA: 0x001C669C File Offset: 0x001C489C
		public virtual void PlaySound(AnimationEvent e)
		{
			if ((double)e.animatorClipInfo.weight < 0.1)
			{
				return;
			}
			EventSound eventSound = this.m_EventSound.Find((EventSound item) => item.name == e.stringParameter);
			if (eventSound != null)
			{
				eventSound.VolumeWeight = e.animatorClipInfo.weight;
				if (this.anim)
				{
					this._audioSource.pitch = this.anim.speed;
				}
				if (this._audioSource.isPlaying)
				{
					if (eventSound.VolumeWeight * eventSound.volume > this._audioSource.volume)
					{
						eventSound.PlayAudio(this._audioSource);
						return;
					}
				}
				else
				{
					eventSound.PlayAudio(this._audioSource);
				}
			}
		}

		// Token: 0x04002B15 RID: 11029
		public List<EventSound> m_EventSound;

		// Token: 0x04002B16 RID: 11030
		public AudioSource _audioSource;

		// Token: 0x04002B17 RID: 11031
		protected Animator anim;
	}
}

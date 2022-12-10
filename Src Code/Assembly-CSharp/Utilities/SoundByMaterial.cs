using System;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x0200047A RID: 1146
	public class SoundByMaterial : MonoBehaviour
	{
		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x06003BD3 RID: 15315 RVA: 0x001C8D50 File Offset: 0x001C6F50
		// (set) Token: 0x06003BD4 RID: 15316 RVA: 0x001C8D71 File Offset: 0x001C6F71
		protected AudioSource Audio_Source
		{
			get
			{
				if (!this.audioSource)
				{
					this.audioSource = base.GetComponent<AudioSource>();
				}
				return this.audioSource;
			}
			set
			{
				this.audioSource = value;
			}
		}

		// Token: 0x06003BD5 RID: 15317 RVA: 0x001C8D7C File Offset: 0x001C6F7C
		public virtual void PlayMaterialSound(RaycastHit hitSurface)
		{
			Collider collider = hitSurface.collider;
			if (collider)
			{
				this.PlayMaterialSound(collider.sharedMaterial);
			}
		}

		// Token: 0x06003BD6 RID: 15318 RVA: 0x001C8DA8 File Offset: 0x001C6FA8
		public virtual void PlayMaterialSound(GameObject hitSurface)
		{
			Collider component = hitSurface.GetComponent<Collider>();
			if (component)
			{
				this.PlayMaterialSound(component.sharedMaterial);
			}
		}

		// Token: 0x06003BD7 RID: 15319 RVA: 0x001C8DD0 File Offset: 0x001C6FD0
		public virtual void PlayMaterialSound(Collider hitSurface)
		{
			this.PlayMaterialSound(hitSurface.sharedMaterial);
		}

		// Token: 0x06003BD8 RID: 15320 RVA: 0x001C8DE0 File Offset: 0x001C6FE0
		public virtual void PlayMaterialSound(PhysicMaterial hitSurface)
		{
			if (!this.Audio_Source)
			{
				this.Audio_Source = base.gameObject.AddComponent<AudioSource>();
				this.Audio_Source.spatialBlend = 1f;
			}
			MaterialSound materialSound = this.materialSounds.Find((MaterialSound item) => item.material == hitSurface);
			if (materialSound != null)
			{
				AudioClip clip = materialSound.Sounds[Random.Range(0, materialSound.Sounds.Length)];
				this.Audio_Source.clip = clip;
				this.audioSource.Play();
				return;
			}
			if (this.DefaultSound)
			{
				this.Audio_Source.clip = this.DefaultSound;
				this.audioSource.Play();
			}
		}

		// Token: 0x04002B69 RID: 11113
		public AudioClip DefaultSound;

		// Token: 0x04002B6A RID: 11114
		public List<MaterialSound> materialSounds;

		// Token: 0x04002B6B RID: 11115
		private AudioSource audioSource;
	}
}

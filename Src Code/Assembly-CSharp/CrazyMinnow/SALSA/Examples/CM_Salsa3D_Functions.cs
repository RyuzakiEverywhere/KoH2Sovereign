using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x02000499 RID: 1177
	public class CM_Salsa3D_Functions : MonoBehaviour
	{
		// Token: 0x06003E00 RID: 15872 RVA: 0x001DB0D4 File Offset: 0x001D92D4
		private void Start()
		{
			if (!this.salsa3D)
			{
				this.salsa3D = (Salsa3D)Object.FindObjectOfType(typeof(Salsa3D));
			}
			if (this.audioClips.Length != 0)
			{
				this.salsa3D.SetAudioClip(this.audioClips[this.clipIndex]);
			}
		}

		// Token: 0x06003E01 RID: 15873 RVA: 0x001DB12C File Offset: 0x001D932C
		private void OnGUI()
		{
			this.yPos = 0;
			this.yPos += this.yGap;
			if (GUI.Button(new Rect(20f, (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Play"))
			{
				this.salsa3D.Play();
			}
			this.yPos += this.yGap + this.yHeight;
			if (GUI.Button(new Rect(20f, (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Pause"))
			{
				this.salsa3D.Pause();
			}
			this.yPos += this.yGap + this.yHeight;
			if (GUI.Button(new Rect(20f, (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Stop"))
			{
				this.salsa3D.Stop();
			}
			this.yPos += this.yGap + this.yHeight;
			if (GUI.Button(new Rect(20f, (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Set audio clip"))
			{
				if (this.clipIndex < this.audioClips.Length - 1)
				{
					this.clipIndex++;
					this.salsa3D.SetAudioClip(this.audioClips[this.clipIndex]);
				}
				else
				{
					this.clipIndex = 0;
					this.salsa3D.SetAudioClip(this.audioClips[this.clipIndex]);
				}
			}
			GUI.Label(new Rect((float)(30 + this.xWidth), (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Clip " + this.audioClips[this.clipIndex].name);
		}

		// Token: 0x04002C12 RID: 11282
		public Salsa3D salsa3D;

		// Token: 0x04002C13 RID: 11283
		public AudioClip[] audioClips;

		// Token: 0x04002C14 RID: 11284
		private int clipIndex;

		// Token: 0x04002C15 RID: 11285
		private int yPos;

		// Token: 0x04002C16 RID: 11286
		private int yGap = 10;

		// Token: 0x04002C17 RID: 11287
		private int xWidth = 150;

		// Token: 0x04002C18 RID: 11288
		private int yHeight = 35;
	}
}

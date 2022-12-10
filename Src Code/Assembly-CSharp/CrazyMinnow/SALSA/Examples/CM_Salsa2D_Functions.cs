using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x02000498 RID: 1176
	public class CM_Salsa2D_Functions : MonoBehaviour
	{
		// Token: 0x06003DFD RID: 15869 RVA: 0x001DAE78 File Offset: 0x001D9078
		private void Start()
		{
			if (!this.salsa2D)
			{
				this.salsa2D = (Salsa2D)Object.FindObjectOfType(typeof(Salsa2D));
			}
			if (this.audioClips.Length != 0)
			{
				this.salsa2D.SetAudioClip(this.audioClips[this.clipIndex]);
			}
		}

		// Token: 0x06003DFE RID: 15870 RVA: 0x001DAED0 File Offset: 0x001D90D0
		private void OnGUI()
		{
			this.yPos = 0;
			this.yPos += this.yGap;
			if (GUI.Button(new Rect(20f, (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Play"))
			{
				this.salsa2D.Play();
			}
			this.yPos += this.yGap + this.yHeight;
			if (GUI.Button(new Rect(20f, (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Pause"))
			{
				this.salsa2D.Pause();
			}
			this.yPos += this.yGap + this.yHeight;
			if (GUI.Button(new Rect(20f, (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Stop"))
			{
				this.salsa2D.Stop();
			}
			this.yPos += this.yGap + this.yHeight;
			if (GUI.Button(new Rect(20f, (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Set audio clip"))
			{
				if (this.clipIndex < this.audioClips.Length - 1)
				{
					this.clipIndex++;
					this.salsa2D.SetAudioClip(this.audioClips[this.clipIndex]);
				}
				else
				{
					this.clipIndex = 0;
					this.salsa2D.SetAudioClip(this.audioClips[this.clipIndex]);
				}
			}
			GUI.Label(new Rect((float)(30 + this.xWidth), (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Clip " + this.audioClips[this.clipIndex].name);
		}

		// Token: 0x04002C0B RID: 11275
		public Salsa2D salsa2D;

		// Token: 0x04002C0C RID: 11276
		public AudioClip[] audioClips;

		// Token: 0x04002C0D RID: 11277
		private int clipIndex;

		// Token: 0x04002C0E RID: 11278
		private int yPos;

		// Token: 0x04002C0F RID: 11279
		private int yGap = 5;

		// Token: 0x04002C10 RID: 11280
		private int xWidth = 150;

		// Token: 0x04002C11 RID: 11281
		private int yHeight = 30;
	}
}

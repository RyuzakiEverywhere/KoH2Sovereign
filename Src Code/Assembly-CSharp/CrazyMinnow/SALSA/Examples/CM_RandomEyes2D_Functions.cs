using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x02000493 RID: 1171
	public class CM_RandomEyes2D_Functions : MonoBehaviour
	{
		// Token: 0x06003DED RID: 15853 RVA: 0x001D9F94 File Offset: 0x001D8194
		private void Start()
		{
			if (!this.randomEyes2D)
			{
				this.randomEyes2D = (RandomEyes2D)Object.FindObjectOfType(typeof(RandomEyes2D));
			}
			if (!this.mainCam)
			{
				this.mainCam = (Camera)Object.FindObjectOfType(typeof(Camera));
			}
			this.targetPosHome = this.target.transform.position;
		}

		// Token: 0x06003DEE RID: 15854 RVA: 0x001DA008 File Offset: 0x001D8208
		private void OnGUI()
		{
			this.xPos = Screen.width - 20 - this.xWidth;
			this.yPos = 0;
			this.yPos += this.yGap;
			if (GUI.Button(new Rect((float)this.xPos, (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Toggle Blink"))
			{
				if (this.randomEyes2D.randomBlink)
				{
					this.randomEyes2D.SetBlink(false);
				}
				else
				{
					this.randomEyes2D.SetBlink(true);
				}
			}
			if (this.randomEyes2D.randomBlink)
			{
				GUI.Label(new Rect((float)(this.xPos - 120), (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Random Blink On");
			}
			else
			{
				GUI.Label(new Rect((float)(this.xPos - 120), (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Random Blink Off");
			}
			if (!this.randomEyes2D.randomBlink)
			{
				this.yPos += this.yGap + this.yHeight;
				if (GUI.Button(new Rect((float)this.xPos, (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Blink"))
				{
					this.randomEyes2D.Blink(0.075f);
				}
			}
			this.yPos += this.yGap + this.yHeight;
			if (GUI.Button(new Rect((float)this.xPos, (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Toggle Affinity"))
			{
				if (this.affinity)
				{
					this.affinity = false;
				}
				else
				{
					this.affinity = true;
				}
				this.affinitySet = true;
			}
			if (this.affinity)
			{
				GUI.Label(new Rect((float)(this.xPos - 120), (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Affinity On: " + this.randomEyes2D.targetAffinityPercentage + "%");
			}
			else
			{
				GUI.Label(new Rect((float)(this.xPos - 120), (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Affinity Off");
			}
			if (this.affinitySet)
			{
				if (this.affinity)
				{
					this.randomEyes2D.SetTargetAffinity(true);
					this.randomEyes2D.SetLookTarget(this.target.gameObject);
				}
				else
				{
					this.randomEyes2D.SetTargetAffinity(false);
				}
				this.affinitySet = false;
			}
			this.yPos += this.yGap + this.yHeight;
			if (GUI.Button(new Rect((float)this.xPos, (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Toggle Tracking"))
			{
				if (this.track)
				{
					this.track = false;
				}
				else
				{
					this.track = true;
				}
				this.trackSet = true;
			}
			if (this.track)
			{
				GUI.Label(new Rect((float)(this.xPos - 120), (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Tracking On");
			}
			else
			{
				GUI.Label(new Rect((float)(this.xPos - 120), (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Tracking Off");
			}
			this.yPos += this.yGap + this.yHeight;
			if (GUI.Button(new Rect((float)this.xPos, (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Toggle RandomEyes"))
			{
				if (this.random)
				{
					this.randomEyes2D.SetRandomEyes(false);
					this.random = false;
				}
				else
				{
					this.randomEyes2D.SetRandomEyes(true);
					this.random = true;
				}
			}
			if (this.random)
			{
				GUI.Label(new Rect((float)(this.xPos - 120), (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Random Eyes On");
			}
			else
			{
				GUI.Label(new Rect((float)(this.xPos - 120), (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Random Eyes Off");
			}
			if (this.track)
			{
				this.targetPos = Input.mousePosition;
				this.targetPos.z = -this.mainCam.transform.position.z - -this.target.transform.position.z;
				this.target.transform.position = new Vector3(this.mainCam.ScreenToWorldPoint(this.targetPos).x, this.mainCam.ScreenToWorldPoint(this.targetPos).y, -0.5f);
			}
			else
			{
				this.target.transform.position = this.targetPosHome;
			}
			if (this.trackSet)
			{
				if (this.track)
				{
					this.randomEyes2D.SetLookTarget(this.target.gameObject);
				}
				else
				{
					this.randomEyes2D.SetLookTarget(null);
				}
				this.trackSet = false;
			}
			if (!this.affinity && !this.track)
			{
				this.randomEyes2D.SetLookTarget(null);
			}
		}

		// Token: 0x04002BE6 RID: 11238
		public RandomEyes2D randomEyes2D;

		// Token: 0x04002BE7 RID: 11239
		public Camera mainCam;

		// Token: 0x04002BE8 RID: 11240
		public SpriteRenderer target;

		// Token: 0x04002BE9 RID: 11241
		private bool random = true;

		// Token: 0x04002BEA RID: 11242
		private bool affinity;

		// Token: 0x04002BEB RID: 11243
		private bool affinitySet = true;

		// Token: 0x04002BEC RID: 11244
		private bool track;

		// Token: 0x04002BED RID: 11245
		private bool trackSet = true;

		// Token: 0x04002BEE RID: 11246
		private Vector3 targetPosHome;

		// Token: 0x04002BEF RID: 11247
		private Vector3 targetPos;

		// Token: 0x04002BF0 RID: 11248
		private int xPos;

		// Token: 0x04002BF1 RID: 11249
		private int yPos;

		// Token: 0x04002BF2 RID: 11250
		private int yGap = 5;

		// Token: 0x04002BF3 RID: 11251
		private int xWidth = 150;

		// Token: 0x04002BF4 RID: 11252
		private int yHeight = 30;
	}
}

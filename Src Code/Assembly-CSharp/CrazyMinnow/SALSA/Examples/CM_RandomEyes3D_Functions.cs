using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x02000494 RID: 1172
	public class CM_RandomEyes3D_Functions : MonoBehaviour
	{
		// Token: 0x06003DF0 RID: 15856 RVA: 0x001DA578 File Offset: 0x001D8778
		private void Start()
		{
			if (!this.randomEyes3D)
			{
				this.randomEyes3D = (RandomEyes3D)Object.FindObjectOfType(typeof(RandomEyes3D));
			}
			if (!this.mainCam)
			{
				this.mainCam = (Camera)Object.FindObjectOfType(typeof(Camera));
			}
			this.targetPosHome = this.target.transform.position;
		}

		// Token: 0x06003DF1 RID: 15857 RVA: 0x001DA5EC File Offset: 0x001D87EC
		private void OnGUI()
		{
			this.xPos = Screen.width - 20 - this.xWidth;
			this.yPos = 0;
			this.yPos += this.yGap;
			if (GUI.Button(new Rect((float)this.xPos, (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Toggle Blink"))
			{
				if (this.randomEyes3D.randomBlink)
				{
					this.randomEyes3D.SetBlink(false);
				}
				else
				{
					this.randomEyes3D.SetBlink(true);
				}
			}
			if (this.randomEyes3D.randomBlink)
			{
				GUI.Label(new Rect((float)(this.xPos - 120), (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Random Blink On");
			}
			else
			{
				GUI.Label(new Rect((float)(this.xPos - 120), (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Random Blink Off");
			}
			if (!this.randomEyes3D.randomBlink)
			{
				this.yPos += this.yGap + this.yHeight;
				if (GUI.Button(new Rect((float)this.xPos, (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Blink"))
				{
					this.randomEyes3D.Blink(0.075f);
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
				GUI.Label(new Rect((float)(this.xPos - 120), (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Affinity On: " + this.randomEyes3D.targetAffinityPercentage + "%");
			}
			else
			{
				GUI.Label(new Rect((float)(this.xPos - 120), (float)this.yPos, (float)this.xWidth, (float)this.yHeight), "Affinity Off");
			}
			if (this.affinitySet)
			{
				if (this.affinity)
				{
					this.randomEyes3D.SetTargetAffinity(true);
					this.randomEyes3D.SetLookTarget(this.target.gameObject);
				}
				else
				{
					this.randomEyes3D.SetTargetAffinity(false);
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
					this.randomEyes3D.SetRandomEyes(false);
					this.random = false;
				}
				else
				{
					this.randomEyes3D.SetRandomEyes(true);
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
				this.target.transform.position = new Vector3(this.mainCam.ScreenToWorldPoint(this.targetPos).x, this.mainCam.ScreenToWorldPoint(this.targetPos).y, -2f);
			}
			else
			{
				this.target.transform.position = this.targetPosHome;
			}
			if (this.trackSet)
			{
				if (this.track)
				{
					this.randomEyes3D.SetLookTarget(this.target.gameObject);
				}
				else
				{
					this.randomEyes3D.SetLookTarget(null);
				}
				this.trackSet = false;
			}
			if (!this.affinity && !this.track)
			{
				this.randomEyes3D.SetLookTarget(null);
			}
		}

		// Token: 0x04002BF5 RID: 11253
		public RandomEyes3D randomEyes3D;

		// Token: 0x04002BF6 RID: 11254
		public Camera mainCam;

		// Token: 0x04002BF7 RID: 11255
		public SpriteRenderer target;

		// Token: 0x04002BF8 RID: 11256
		private bool random = true;

		// Token: 0x04002BF9 RID: 11257
		private bool affinity;

		// Token: 0x04002BFA RID: 11258
		private bool affinitySet = true;

		// Token: 0x04002BFB RID: 11259
		private bool track;

		// Token: 0x04002BFC RID: 11260
		private bool trackSet = true;

		// Token: 0x04002BFD RID: 11261
		private Vector3 targetPosHome;

		// Token: 0x04002BFE RID: 11262
		private Vector3 targetPos;

		// Token: 0x04002BFF RID: 11263
		private int xPos;

		// Token: 0x04002C00 RID: 11264
		private int yPos;

		// Token: 0x04002C01 RID: 11265
		private int yGap = 5;

		// Token: 0x04002C02 RID: 11266
		private int xWidth = 150;

		// Token: 0x04002C03 RID: 11267
		private int yHeight = 30;
	}
}

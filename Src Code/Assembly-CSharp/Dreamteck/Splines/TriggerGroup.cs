using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004D1 RID: 1233
	[Serializable]
	public class TriggerGroup
	{
		// Token: 0x06004193 RID: 16787 RVA: 0x001F2F08 File Offset: 0x001F1108
		public void Check(double start, double end, SplineUser user = null)
		{
			for (int i = 0; i < this.triggers.Length; i++)
			{
				if (this.triggers[i] != null && this.triggers[i].Check(start, end))
				{
					this.triggers[i].Invoke(user);
				}
			}
		}

		// Token: 0x06004194 RID: 16788 RVA: 0x001F2F54 File Offset: 0x001F1154
		public void Reset()
		{
			for (int i = 0; i < this.triggers.Length; i++)
			{
				this.triggers[i].Reset();
			}
		}

		// Token: 0x04002DA4 RID: 11684
		public bool enabled = true;

		// Token: 0x04002DA5 RID: 11685
		public string name = "";

		// Token: 0x04002DA6 RID: 11686
		public Color color = Color.white;

		// Token: 0x04002DA7 RID: 11687
		public SplineTrigger[] triggers = new SplineTrigger[0];
	}
}

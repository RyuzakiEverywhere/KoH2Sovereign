using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003FF RID: 1023
	public class Pivots : MonoBehaviour
	{
		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06003855 RID: 14421 RVA: 0x001BB838 File Offset: 0x001B9A38
		public Vector3 GetPivot
		{
			get
			{
				return base.transform.position;
			}
		}

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06003856 RID: 14422 RVA: 0x001BB845 File Offset: 0x001B9A45
		public float Y
		{
			get
			{
				return base.transform.position.y;
			}
		}

		// Token: 0x06003857 RID: 14423 RVA: 0x001BB858 File Offset: 0x001B9A58
		private void OnDrawGizmos()
		{
			if (this.debug)
			{
				Gizmos.color = this.DebugColor;
				Gizmos.DrawWireSphere(this.GetPivot, this.debugSize);
				if (this.drawRay)
				{
					Gizmos.DrawRay(this.GetPivot, -base.transform.up * this.multiplier * base.transform.root.localScale.y);
				}
			}
		}

		// Token: 0x0400285D RID: 10333
		public float multiplier = 1f;

		// Token: 0x0400285E RID: 10334
		public bool debug = true;

		// Token: 0x0400285F RID: 10335
		public float debugSize = 0.03f;

		// Token: 0x04002860 RID: 10336
		public Color DebugColor = Color.blue;

		// Token: 0x04002861 RID: 10337
		public bool drawRay = true;
	}
}

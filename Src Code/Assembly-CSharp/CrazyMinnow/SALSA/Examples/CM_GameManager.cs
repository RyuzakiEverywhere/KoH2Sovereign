using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x02000492 RID: 1170
	public class CM_GameManager : MonoBehaviour
	{
		// Token: 0x06003DEB RID: 15851 RVA: 0x001D9F3C File Offset: 0x001D813C
		private void OnGUI()
		{
			if (GUI.Button(new Rect(20f, (float)(Screen.height - 55), 75f, 35f), "Reset"))
			{
				this.spiderWaypoints.ResetSalsaWaypoints();
				this.boxheadWaypoints.ResetSalsaWaypoints();
				this.dialogSystem.ResetDialog();
			}
		}

		// Token: 0x04002BE3 RID: 11235
		public CM_DialogSystem dialogSystem;

		// Token: 0x04002BE4 RID: 11236
		public CM_SalsaWaypoints spiderWaypoints;

		// Token: 0x04002BE5 RID: 11237
		public CM_SalsaWaypoints boxheadWaypoints;
	}
}

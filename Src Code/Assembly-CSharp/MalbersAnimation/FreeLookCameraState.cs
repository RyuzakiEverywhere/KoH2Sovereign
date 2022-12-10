using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003E6 RID: 998
	[CreateAssetMenu(menuName = "Malbers Animations/Camera/FreeLook Camera State")]
	public class FreeLookCameraState : ScriptableObject
	{
		// Token: 0x060037A2 RID: 14242 RVA: 0x001B8954 File Offset: 0x001B6B54
		public FreeLookCameraState()
		{
			this.CamFOV = 45f;
			this.PivotPos = new Vector3(0f, 1f, 0f);
			this.CamPos = new Vector3(0f, 0f, -4.45f);
		}

		// Token: 0x040027D7 RID: 10199
		public Vector3 PivotPos;

		// Token: 0x040027D8 RID: 10200
		public Vector3 CamPos;

		// Token: 0x040027D9 RID: 10201
		public float CamFOV = 45f;
	}
}

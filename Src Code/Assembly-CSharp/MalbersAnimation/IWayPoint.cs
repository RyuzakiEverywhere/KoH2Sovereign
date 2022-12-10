using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003F4 RID: 1012
	internal interface IWayPoint
	{
		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06003800 RID: 14336
		float StoppingDistance { get; }

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x06003801 RID: 14337
		Transform NextTarget { get; }

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x06003802 RID: 14338
		float WaitTime { get; }

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06003803 RID: 14339
		WayPointType PointType { get; }
	}
}

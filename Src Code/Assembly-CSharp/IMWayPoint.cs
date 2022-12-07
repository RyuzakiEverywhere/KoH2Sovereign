using System;
using UnityEngine;

// Token: 0x02000052 RID: 82
internal interface IMWayPoint
{
	// Token: 0x17000020 RID: 32
	// (get) Token: 0x060001FD RID: 509
	Transform NextTarget { get; }

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x060001FE RID: 510
	float StoppinDistance { get; }
}

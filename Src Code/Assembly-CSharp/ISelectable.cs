using System;
using UnityEngine;

// Token: 0x020001BA RID: 442
public interface ISelectable
{
	// Token: 0x06001A30 RID: 6704
	void RegisterSelectable();

	// Token: 0x06001A31 RID: 6705
	void UnregisterSelectable();

	// Token: 0x06001A32 RID: 6706
	float RayCast(BattleViewUI ui, Ray ray);

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x06001A33 RID: 6707
	// (set) Token: 0x06001A34 RID: 6708
	bool Selected { get; set; }

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06001A35 RID: 6709
	// (set) Token: 0x06001A36 RID: 6710
	bool PreSelected { get; set; }

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x06001A37 RID: 6711
	Transform transform { get; }
}

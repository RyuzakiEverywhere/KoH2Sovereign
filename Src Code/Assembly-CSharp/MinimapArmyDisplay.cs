using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200023C RID: 572
public class MinimapArmyDisplay : MonoBehaviour
{
	// Token: 0x06002336 RID: 9014 RVA: 0x0013F123 File Offset: 0x0013D323
	private void Awake()
	{
		this.minimap = base.transform.GetComponent<MiniMap>();
	}

	// Token: 0x06002337 RID: 9015 RVA: 0x0013F136 File Offset: 0x0013D336
	public void RegisterArmy(Army army)
	{
		if (this.minimap == null)
		{
			return;
		}
		if (((army != null) ? army.logic : null) == null)
		{
			return;
		}
		this.armies.Add(army);
	}

	// Token: 0x06002338 RID: 9016 RVA: 0x0013F162 File Offset: 0x0013D362
	public void UnregisterArmy(Army army)
	{
		if (this.minimap == null)
		{
			return;
		}
		this.minimap.DelObj(army.logic);
	}

	// Token: 0x06002339 RID: 9017 RVA: 0x0013F184 File Offset: 0x0013D384
	private void LateUpdate()
	{
		if (this.armies.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.armies.Count; i++)
		{
			this.minimap.AddObj(this.armies[i].logic);
		}
		this.armies.Clear();
	}

	// Token: 0x0400179C RID: 6044
	private MiniMap minimap;

	// Token: 0x0400179D RID: 6045
	private List<Army> armies = new List<Army>();
}

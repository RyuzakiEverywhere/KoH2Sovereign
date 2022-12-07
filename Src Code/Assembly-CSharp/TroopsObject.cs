using System;
using UnityEngine;

// Token: 0x020000D7 RID: 215
public class TroopsObject : MonoBehaviour, Troops.ITroopObject
{
	// Token: 0x06000AA7 RID: 2727 RVA: 0x0007CE56 File Offset: 0x0007B056
	public bool Initted()
	{
		return this.data_id >= 0;
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void AddToTroops()
	{
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x0007CE64 File Offset: 0x0007B064
	public int GetID()
	{
		return this.data_id;
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x0007CE6C File Offset: 0x0007B06C
	public void SetID(int id)
	{
		this.data_id = id;
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x0007CE75 File Offset: 0x0007B075
	public virtual bool ReadyToAdd()
	{
		return Troops.Initted;
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x0007CE7C File Offset: 0x0007B07C
	private void Update()
	{
		if (this.Initted())
		{
			this.OnUpdate();
			return;
		}
		if (!this.ReadyToAdd())
		{
			return;
		}
		this.AddToTroops();
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void OnUpdate()
	{
	}

	// Token: 0x0400086B RID: 2155
	private int data_id = -1;
}

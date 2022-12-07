using System;
using UnityEngine;

// Token: 0x02000127 RID: 295
public class BattleViewGeometryOptimizations : MonoBehaviour
{
	// Token: 0x06000DB3 RID: 3507 RVA: 0x00099B9D File Offset: 0x00097D9D
	private void OnEnable()
	{
		SettlementBV.OnGenerationComplete = (SettlementBV.OnGenerationEvent)Delegate.Combine(SettlementBV.OnGenerationComplete, new SettlementBV.OnGenerationEvent(this.Optimize));
		this.cutout_shaders_setup.enabled = true;
		this.batcher.enabled = true;
	}

	// Token: 0x06000DB4 RID: 3508 RVA: 0x00099BD7 File Offset: 0x00097DD7
	private void OnDisable()
	{
		SettlementBV.OnGenerationComplete = (SettlementBV.OnGenerationEvent)Delegate.Remove(SettlementBV.OnGenerationComplete, new SettlementBV.OnGenerationEvent(this.Optimize));
		this.cutout_shaders_setup.enabled = false;
		this.batcher.enabled = false;
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x00099C11 File Offset: 0x00097E11
	private void Optimize()
	{
		if (this.isOptimized || !BattleViewGeometryOptimizations.Enabled)
		{
			return;
		}
		this.cutout_shaders_setup.FixCutoutShaders();
		this.batcher.GenerateStaticBatches();
		this.isOptimized = true;
	}

	// Token: 0x04000A77 RID: 2679
	public static bool Enabled = true;

	// Token: 0x04000A78 RID: 2680
	[SerializeField]
	private SetupCutoutShaders cutout_shaders_setup;

	// Token: 0x04000A79 RID: 2681
	[SerializeField]
	private RuntimeStaticBatching batcher;

	// Token: 0x04000A7A RID: 2682
	private bool isOptimized;
}

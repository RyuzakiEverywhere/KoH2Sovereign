using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001D7 RID: 471
public class UIBattleViewSquadEffects : MonoBehaviour, IListener
{
	// Token: 0x17000177 RID: 375
	// (get) Token: 0x06001C09 RID: 7177 RVA: 0x00109E47 File Offset: 0x00108047
	// (set) Token: 0x06001C0A RID: 7178 RVA: 0x00109E4F File Offset: 0x0010804F
	public Logic.Squad SquadLogic { get; private set; }

	// Token: 0x17000178 RID: 376
	// (get) Token: 0x06001C0B RID: 7179 RVA: 0x00109E58 File Offset: 0x00108058
	// (set) Token: 0x06001C0C RID: 7180 RVA: 0x00109E60 File Offset: 0x00108060
	public global::Squad Visuals { get; private set; }

	// Token: 0x17000179 RID: 377
	// (get) Token: 0x06001C0D RID: 7181 RVA: 0x00109E69 File Offset: 0x00108069
	// (set) Token: 0x06001C0E RID: 7182 RVA: 0x00109E71 File Offset: 0x00108071
	public BattleSimulation.Squad SimulationSquadLogic { get; private set; }

	// Token: 0x06001C0F RID: 7183 RVA: 0x00109E7A File Offset: 0x0010807A
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06001C10 RID: 7184 RVA: 0x00109E84 File Offset: 0x00108084
	public void Setup(BattleSimulation.Squad logic)
	{
		this.Initialize();
		this.SimulationSquadLogic = logic;
		this.SquadLogic = this.SimulationSquadLogic.squad;
		Logic.Squad squadLogic = this.SquadLogic;
		if (((squadLogic != null) ? squadLogic.visuals : null) != null)
		{
			this.Visuals = (this.SquadLogic.visuals as global::Squad);
		}
		this.Refresh();
	}

	// Token: 0x06001C11 RID: 7185 RVA: 0x00109EE0 File Offset: 0x001080E0
	public void OnMessage(object obj, string message, object param)
	{
		if (this == null)
		{
			return;
		}
		if (base.gameObject == null)
		{
			return;
		}
		if (message == "defeated")
		{
			this.Refresh();
			return;
		}
		if (!(message == "troop_died"))
		{
			return;
		}
		this.Refresh();
	}

	// Token: 0x06001C12 RID: 7186 RVA: 0x00109F2E File Offset: 0x0010812E
	private void Initialize()
	{
		if (this.m_isInitialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_windowDef == null)
		{
			this.m_windowDef = global::Defs.GetDefField("UIBattleViewSquad", null);
		}
		this.m_isInitialized = true;
	}

	// Token: 0x06001C13 RID: 7187 RVA: 0x000023FD File Offset: 0x000005FD
	private void Refresh()
	{
	}

	// Token: 0x04001239 RID: 4665
	[UIFieldTarget("id_SquadIcon")]
	private Image m_SquadIcon;

	// Token: 0x0400123A RID: 4666
	[UIFieldTarget("id_Border")]
	private Image m_SquadBorder;

	// Token: 0x0400123B RID: 4667
	[UIFieldTarget("id_SquadIconDead")]
	private Image m_SquadIconDead;

	// Token: 0x0400123C RID: 4668
	[UIFieldTarget("id_RetreatIcon")]
	private Image m_RetreatIcon;

	// Token: 0x0400123D RID: 4669
	[UIFieldTarget("id_FleeingIcon")]
	private Image m_FleeingIcon;

	// Token: 0x0400123E RID: 4670
	[UIFieldTarget("id_SelectionFrame")]
	private TweenAlpha m_SelectionFrame;

	// Token: 0x0400123F RID: 4671
	[UIFieldTarget("id_DamageEffect")]
	private TweenAlpha m_DamageEffect;

	// Token: 0x04001243 RID: 4675
	public bool mouse_in;

	// Token: 0x04001244 RID: 4676
	private DT.Field m_windowDef;

	// Token: 0x04001245 RID: 4677
	private DT.Field m_stateDef;

	// Token: 0x04001246 RID: 4678
	public bool isSelected;

	// Token: 0x04001247 RID: 4679
	private bool m_isInitialized;
}

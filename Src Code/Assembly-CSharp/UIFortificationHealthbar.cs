using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001DB RID: 475
public class UIFortificationHealthbar : WorldToScreenObject
{
	// Token: 0x06001C43 RID: 7235 RVA: 0x0010B1CC File Offset: 0x001093CC
	private void LateUpdate()
	{
		this.SetHealth();
	}

	// Token: 0x06001C44 RID: 7236 RVA: 0x0010B1D4 File Offset: 0x001093D4
	public void Setup(Logic.Fortification logic)
	{
		this.logic = logic;
		if (logic != null)
		{
			global::Fortification fortification = logic.visuals as global::Fortification;
			this._g = ((fortification != null) ? fortification.gameObject : null);
		}
		if (!this._init)
		{
			this._init = true;
			UICommon.FindComponents(this, false);
			this.RefreshDefField();
			this.UpdateVisibilityFromView(ViewMode.AllowedFigures.None);
			this.UpdateVisibilityFilter();
		}
		if (this.logic != null)
		{
			this.m_GateContainer.SetActive(this.logic.gate != null);
		}
		this.SetupBarColor();
	}

	// Token: 0x06001C45 RID: 7237 RVA: 0x0010B258 File Offset: 0x00109458
	public override void UpdateVisibilityFromView(ViewMode.AllowedFigures allowedFiguresFromViewMode)
	{
		this.visibility_from_view = true;
		base.UpdateVisibility();
	}

	// Token: 0x06001C46 RID: 7238 RVA: 0x0010B267 File Offset: 0x00109467
	public override void UpdateVisibilityFilter()
	{
		this.visibility_from_filter = true;
		base.UpdateVisibility();
	}

	// Token: 0x06001C47 RID: 7239 RVA: 0x0010B276 File Offset: 0x00109476
	public override void RefreshDefField()
	{
		if (base.InitDef())
		{
			this.m_definition = global::Defs.GetDefField("UIFortificationHealthBar", null);
			this.LoadDefs(this.m_definition);
		}
		base.RefreshDefField();
	}

	// Token: 0x06001C48 RID: 7240 RVA: 0x0010B2A3 File Offset: 0x001094A3
	protected override string DefKey(bool refresh = false)
	{
		if (refresh)
		{
			this.def_key = "fortification_health_bar";
		}
		return base.DefKey(refresh);
	}

	// Token: 0x06001C49 RID: 7241 RVA: 0x0010B2BC File Offset: 0x001094BC
	private void SetupBarColor()
	{
		if (this.m_HealthFill != null)
		{
			this.m_HealthFill.color = WorldToScreenObject.def_params[this.DefKey(false)].color;
		}
		if (this.m_GateHealthFill != null)
		{
			this.m_GateHealthFill.color = WorldToScreenObject.def_params[this.DefKey(false)].secondary_color;
		}
	}

	// Token: 0x06001C4A RID: 7242 RVA: 0x0010B328 File Offset: 0x00109528
	public void SetHealth()
	{
		if (this.m_HealthFill != null)
		{
			this.m_HealthFill.fillAmount = this.logic.health / this.logic.max_health;
		}
		if (this.m_GateHealthFill != null && this.logic.gate != null)
		{
			this.m_GateHealthFill.fillAmount = this.logic.gate.health / this.logic.max_gate_health;
		}
	}

	// Token: 0x06001C4B RID: 7243 RVA: 0x0010B3A8 File Offset: 0x001095A8
	public override Vector3 GetDesiredPosition(bool is_pv)
	{
		global::Fortification fortification = this.logic.visuals as global::Fortification;
		if (fortification == null)
		{
			return base.GetDesiredPosition(is_pv);
		}
		return fortification.snapped_position + new Vector3(0f, fortification.bounds.extents.y, 0f) + WorldToScreenObject.def_params[this.DefKey(false)].offset_3d;
	}

	// Token: 0x0400126D RID: 4717
	[UIFieldTarget("id_HealthFill")]
	private Image m_HealthFill;

	// Token: 0x0400126E RID: 4718
	[UIFieldTarget("id_Gate")]
	private GameObject m_GateContainer;

	// Token: 0x0400126F RID: 4719
	[UIFieldTarget("id_GateHealthFill")]
	private Image m_GateHealthFill;

	// Token: 0x04001270 RID: 4720
	public Logic.Fortification logic;

	// Token: 0x04001271 RID: 4721
	private DT.Field m_definition;

	// Token: 0x04001272 RID: 4722
	private bool _init;
}

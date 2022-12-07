using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001DE RID: 478
public class UISquadStatus : MonoBehaviour
{
	// Token: 0x06001C59 RID: 7257 RVA: 0x000DF44F File Offset: 0x000DD64F
	private void Awake()
	{
		UICommon.FindComponents(this, false);
	}

	// Token: 0x06001C5A RID: 7258 RVA: 0x0010BB83 File Offset: 0x00109D83
	public void SetDef(SquadBuff buff, DT.Field icon_settings)
	{
		this.buff = buff;
		if (buff == null)
		{
			return;
		}
		this.SetDef(buff.field, icon_settings);
	}

	// Token: 0x06001C5B RID: 7259 RVA: 0x0010BBA0 File Offset: 0x00109DA0
	private void SetDef(DT.Field def, DT.Field icon_settings)
	{
		this.vars = new Vars();
		this.vars.Set<DT.Field>("caption", def.FindChild("caption", null, true, true, true, '.'));
		this.vars.Set<DT.Field>("description", def.FindChild("description", null, true, true, true, '.'));
		this.vars.Set<DT.Field>("effects", def.FindChild("effects_text", null, true, true, true, '.'));
		this.vars.Set<bool>("has_effects", def.GetBool("has_effects", null, false, true, true, true, '.'));
		this.CheckTooltipToggleChanged(true);
		if (icon_settings != null)
		{
			Sprite obj = global::Defs.GetObj<Sprite>(icon_settings, "background" + ((!this.pinnable) ? "_nameplate" : ""), null);
			Sprite obj2 = global::Defs.GetObj<Sprite>(icon_settings, "icon", null);
			this.SetSprites(obj, obj2);
		}
	}

	// Token: 0x06001C5C RID: 7260 RVA: 0x0010BC84 File Offset: 0x00109E84
	private void UpdateVars()
	{
		if (this.buff == null)
		{
			return;
		}
		this.UpdateVar("CTH", this.buff.GetCTH());
		this.UpdateVar("CTH_cavalry", this.buff.getCTHCavalry());
		this.UpdateVar("CTH_shoot_mod", this.buff.GetCTHShootMod());
		this.UpdateVar("defense", this.buff.GetDefense());
		this.UpdateVar("defense_against_ranged", this.buff.GetDefenseAgainstRanged());
		this.UpdateVar("resilience_flat", this.buff.GetResilienceFlat());
		this.UpdateVar("cth_against_me", this.buff.getCTHAgainstMe());
		this.UpdateVar("movement_speed", this.buff.GetMoveSpeed());
	}

	// Token: 0x06001C5D RID: 7261 RVA: 0x0010BD4A File Offset: 0x00109F4A
	private void UpdateVar(string key, float val)
	{
		if (val == 0f)
		{
			this.vars.Set<Value>(key, Value.Unknown);
			return;
		}
		this.vars.Set<float>(key, val);
	}

	// Token: 0x06001C5E RID: 7262 RVA: 0x0010BD73 File Offset: 0x00109F73
	public void SetSprites(Sprite background, Sprite icon)
	{
		this.m_StatusBackground.sprite = background;
		this.m_StatusIcon.sprite = icon;
	}

	// Token: 0x06001C5F RID: 7263 RVA: 0x0010BD8D File Offset: 0x00109F8D
	private void Update()
	{
		if (!this.pinnable)
		{
			this.CheckTooltipToggleChanged(false);
			if (this.last_tooltip_enabled != 1)
			{
				return;
			}
		}
		this.UpdateVars();
	}

	// Token: 0x06001C60 RID: 7264 RVA: 0x0010BDB0 File Offset: 0x00109FB0
	public void CheckTooltipToggleChanged(bool force = false)
	{
		SquadBuff squadBuff = this.buff;
		Logic.Squad squad = (squadBuff != null) ? squadBuff.squad : null;
		global::Squad squad2 = ((squad != null) ? squad.visuals : null) as global::Squad;
		if (squad2 == null)
		{
			return;
		}
		bool nameplateTooltipFilter = squad2.m_NameplateTooltipFilter;
		bool flag = this.last_tooltip_enabled == 1;
		if (nameplateTooltipFilter == flag && this.last_tooltip_enabled != -1 && !force)
		{
			return;
		}
		this.last_tooltip_enabled = (nameplateTooltipFilter ? 1 : 0);
		this.ToggleTooltip(nameplateTooltipFilter);
	}

	// Token: 0x06001C61 RID: 7265 RVA: 0x0010BE24 File Offset: 0x0010A024
	public void ToggleTooltip(bool isFilterOn)
	{
		if (this.pinnable)
		{
			Tooltip.Get(base.gameObject, true).SetDef("SquadBuffTooltip", this.vars);
			return;
		}
		if (isFilterOn)
		{
			Tooltip.Get(base.gameObject, true).SetDef("SquadBuffTooltipUnpinnable", this.vars);
			return;
		}
		Tooltip tooltip = Tooltip.Get(base.gameObject, false);
		if (tooltip != null)
		{
			global::Common.DestroyObj(tooltip);
		}
	}

	// Token: 0x0400128E RID: 4750
	[UIFieldTarget("id_StatusBackground")]
	[HideInInspector]
	public Image m_StatusBackground;

	// Token: 0x0400128F RID: 4751
	[UIFieldTarget("id_StatusIcon")]
	private Image m_StatusIcon;

	// Token: 0x04001290 RID: 4752
	private Vars vars;

	// Token: 0x04001291 RID: 4753
	private SquadBuff buff;

	// Token: 0x04001292 RID: 4754
	public bool pinnable = true;

	// Token: 0x04001293 RID: 4755
	private int last_tooltip_enabled = -1;
}

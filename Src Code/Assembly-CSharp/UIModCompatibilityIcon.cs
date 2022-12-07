using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000246 RID: 582
public class UIModCompatibilityIcon : ObjectIcon
{
	// Token: 0x06002377 RID: 9079 RVA: 0x00140288 File Offset: 0x0013E488
	private void Init()
	{
		if (this.m_Initiazlied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		Tooltip.Get(base.gameObject, true).SetDef("ModCompatibilityIconTooltip", new Vars(this));
		ModManager modManager = ModManager.Get(false);
		if (modManager != null)
		{
			ModManager modManager2 = modManager;
			modManager2.onActiveModChanged = (Action)Delegate.Combine(modManager2.onActiveModChanged, new Action(this.OnModeChanged));
		}
		this.m_Initiazlied = false;
	}

	// Token: 0x06002378 RID: 9080 RVA: 0x001402F4 File Offset: 0x0013E4F4
	public override void SetObject(object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		this.SetData(obj);
	}

	// Token: 0x06002379 RID: 9081 RVA: 0x00140305 File Offset: 0x0013E505
	private void SetData(object target)
	{
		this.Init();
		if (this.def == null)
		{
			this.def = global::Defs.GetDefField("ModCompatibilityIcon", null);
		}
		this.m_Target = target;
		this.Refresh();
	}

	// Token: 0x0600237A RID: 9082 RVA: 0x00140334 File Offset: 0x0013E534
	private void Refresh()
	{
		ModManager modManager = ModManager.Get(false);
		if (modManager == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		if (this.m_Icon != null)
		{
			string state = this.GetState(this.GetModId(this.m_Target), modManager.GetActiveMod());
			bool flag = !string.IsNullOrEmpty(state);
			if (flag)
			{
				this.m_Icon.overrideSprite = global::Defs.GetObj<Sprite>(this.def, "icons." + state, null);
			}
			this.m_Icon.gameObject.SetActive(flag);
		}
	}

	// Token: 0x0600237B RID: 9083 RVA: 0x001403CC File Offset: 0x0013E5CC
	private string GetState(string targetModId, Mod activeMod)
	{
		bool flag = !string.IsNullOrEmpty(targetModId);
		bool flag2 = activeMod != null;
		bool flag3 = ModManager.Get(false).GetMod(targetModId) != null;
		if (!flag && !flag2)
		{
			return string.Empty;
		}
		if (flag2 && targetModId == activeMod.mod_id)
		{
			return "compatible";
		}
		if (flag && !flag3)
		{
			return "missing";
		}
		if (flag && flag3)
		{
			return "not_active";
		}
		if (!flag && flag2)
		{
			return "not_active";
		}
		return string.Empty;
	}

	// Token: 0x0600237C RID: 9084 RVA: 0x00140444 File Offset: 0x0013E644
	private string GetModId(object target)
	{
		if (target is SaveGame.Info)
		{
			return (target as SaveGame.Info).mod_id;
		}
		if (target is Campaign)
		{
			return (target as Campaign).GetModID();
		}
		if (target is string)
		{
			return (string)target;
		}
		return string.Empty;
	}

	// Token: 0x0600237D RID: 9085 RVA: 0x00140482 File Offset: 0x0013E682
	private string GetTargetTypeKey()
	{
		if (this.m_Target is SaveGame.Info)
		{
			return "save";
		}
		Campaign campaign = this.m_Target as Campaign;
		return "campaign";
	}

	// Token: 0x0600237E RID: 9086 RVA: 0x001404A8 File Offset: 0x0013E6A8
	private string GetModName(string mod_id)
	{
		if (string.IsNullOrEmpty(mod_id))
		{
			return global::Defs.Localize("TargetPicker.none_text", null, null, true, true);
		}
		string[] array = mod_id.Split(new char[]
		{
			'/'
		});
		if (array.Length != 3)
		{
			return global::Defs.Localize("TargetPicker.none_text", null, null, true, true);
		}
		return array[0];
	}

	// Token: 0x0600237F RID: 9087 RVA: 0x001404F6 File Offset: 0x0013E6F6
	private string GetModName(Mod mod)
	{
		if (mod == null)
		{
			return global::Defs.Localize("TargetPicker.none_text", null, null, true, true);
		}
		return mod.name;
	}

	// Token: 0x06002380 RID: 9088 RVA: 0x00140510 File Offset: 0x0013E710
	private string GetModStateDescription()
	{
		ModManager modManager = ModManager.Get(false);
		if (modManager == null)
		{
			return "";
		}
		string state = this.GetState(this.GetModId(this.m_Target), modManager.GetActiveMod());
		string a = this.GetTargetTypeKey() + "_" + state;
		if (a == "campaign_compatible")
		{
			return global::Defs.Localize("SaveLoadMenuWindow.campaign_same_mod_tooltip", this, null, true, true);
		}
		if (a == "campaign_not_active")
		{
			return global::Defs.Localize("SaveLoadMenuWindow.campaign_different_mod_tooltip", this, null, true, true);
		}
		if (a == "campaign_missing")
		{
			return global::Defs.Localize("SaveLoadMenuWindow.campaign_unknown_mod_tooltip", this, null, true, true);
		}
		if (a == "save_compatible")
		{
			return global::Defs.Localize("SaveLoadMenuWindow.save_same_mod_tooltip", this, null, true, true);
		}
		if (a == "save_not_active")
		{
			return global::Defs.Localize("SaveLoadMenuWindow.save_different_mod_tooltip", this, null, true, true);
		}
		if (!(a == "save_missing"))
		{
			return "";
		}
		return global::Defs.Localize("SaveLoadMenuWindow.save_unknown_mod_tooltip", this, null, true, true);
	}

	// Token: 0x06002381 RID: 9089 RVA: 0x00140607 File Offset: 0x0013E807
	private void OnModeChanged()
	{
		if (this == null)
		{
			return;
		}
		this.def = global::Defs.GetDefField("ModCompatibilityIcon", null);
		this.Refresh();
	}

	// Token: 0x06002382 RID: 9090 RVA: 0x0014062A File Offset: 0x0013E82A
	public override void OnClick(PointerEventData e)
	{
		this.UpdateHighlight();
	}

	// Token: 0x06002383 RID: 9091 RVA: 0x0014062A File Offset: 0x0013E82A
	public override void OnDoubleClick(PointerEventData e)
	{
		this.UpdateHighlight();
	}

	// Token: 0x06002384 RID: 9092 RVA: 0x00140632 File Offset: 0x0013E832
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002385 RID: 9093 RVA: 0x00140641 File Offset: 0x0013E841
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002386 RID: 9094 RVA: 0x000023FD File Offset: 0x000005FD
	public void UpdateHighlight()
	{
	}

	// Token: 0x06002387 RID: 9095 RVA: 0x00140650 File Offset: 0x0013E850
	private void OnDestroy()
	{
		ModManager modManager = ModManager.Get(false);
		if (modManager != null)
		{
			ModManager modManager2 = modManager;
			modManager2.onActiveModChanged = (Action)Delegate.Remove(modManager2.onActiveModChanged, new Action(this.OnModeChanged));
		}
	}

	// Token: 0x06002388 RID: 9096 RVA: 0x0014068C File Offset: 0x0013E88C
	public override Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		if (key == "mod_name")
		{
			return "#" + this.GetModName(this.GetModId(this.m_Target));
		}
		if (key == "current_mod")
		{
			string str = "#";
			ModManager modManager = ModManager.Get(false);
			return str + this.GetModName((modManager != null) ? modManager.GetActiveMod() : null);
		}
		if (!(key == "description"))
		{
			return base.GetVar(key, vars, as_value);
		}
		return "#" + this.GetModStateDescription();
	}

	// Token: 0x040017C9 RID: 6089
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x040017CA RID: 6090
	private DT.Field def;

	// Token: 0x040017CB RID: 6091
	private bool m_Initiazlied;

	// Token: 0x040017CC RID: 6092
	private object m_Target;
}

using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002BD RID: 701
public class UISettlementTypeIcon : Hotspot
{
	// Token: 0x06002BFD RID: 11261 RVA: 0x00171650 File Offset: 0x0016F850
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialized = true;
	}

	// Token: 0x06002BFE RID: 11262 RVA: 0x00171669 File Offset: 0x0016F869
	public void SetDef(DT.Field def, Logic.Realm realm)
	{
		this.Init();
		this.m_Realm = realm;
		this.Def = def;
		this.Refresh();
	}

	// Token: 0x06002BFF RID: 11263 RVA: 0x00171688 File Offset: 0x0016F888
	private void Refresh()
	{
		Vars vars = new Vars(this.Def);
		vars.Set<Logic.Realm>("realm", this.m_Realm);
		DT.Field field = this.Def.FindChild("type", null, true, true, true, '.');
		string type = (field != null) ? field.String(null, "") : null;
		int settlementCount = this.m_Realm.GetSettlementCount(type, "realm", false);
		if (this.m_Icon != null)
		{
			this.m_Icon.overrideSprite = global::Defs.GetObj<Sprite>(this.Def, "icon", vars);
		}
		if (this.m_IconContainer != null)
		{
			this.m_IconContainer.gameObject.SetActive(settlementCount > 0);
		}
		if (this.m_Count != null)
		{
			UIText.SetText(this.m_Count, settlementCount.ToString());
		}
		Tooltip.Get(base.gameObject, true).SetDef("SettlementTypeTooltip", vars);
	}

	// Token: 0x06002C00 RID: 11264 RVA: 0x00171774 File Offset: 0x0016F974
	public static UISettlementTypeIcon GetIcon(DT.Field def, Logic.Realm realm, RectTransform parent)
	{
		GameObject obj = global::Defs.GetObj<GameObject>("UISettlementTypeIcon", "window_prefab", null);
		if (obj == null)
		{
			return null;
		}
		UISettlementTypeIcon component = global::Common.Spawn(obj, parent, false, "").GetComponent<UISettlementTypeIcon>();
		if (component != null)
		{
			component.SetDef(def, realm);
		}
		return component;
	}

	// Token: 0x04001DF1 RID: 7665
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x04001DF2 RID: 7666
	[UIFieldTarget("id_CountContainer")]
	private GameObject m_IconContainer;

	// Token: 0x04001DF3 RID: 7667
	[UIFieldTarget("id_Count")]
	private TextMeshProUGUI m_Count;

	// Token: 0x04001DF4 RID: 7668
	public DT.Field Def;

	// Token: 0x04001DF5 RID: 7669
	private Logic.Realm m_Realm;

	// Token: 0x04001DF6 RID: 7670
	private bool m_Initialized;
}

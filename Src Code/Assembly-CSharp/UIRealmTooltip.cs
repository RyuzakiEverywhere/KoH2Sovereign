using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002BA RID: 698
public class UIRealmTooltip : MonoBehaviour, Tooltip.IHandler
{
	// Token: 0x06002BD8 RID: 11224 RVA: 0x00170974 File Offset: 0x0016EB74
	private void Init()
	{
		if (this.m_Initialzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.def = global::Defs.GetDefField("RealmTooltip", null);
		if (this.m_StatusIconPrototype != null)
		{
			this.m_StatusIconPrototype.SetActive(false);
		}
		if (this.m_RealmProvinceFeatures != null)
		{
			this.m_ProvinceFeatures = this.m_RealmProvinceFeatures.AddComponent<UICastleWindow.UIProvinceFeatures>();
		}
		if (this.m_ProvinceFeaturesLabel != null)
		{
			UIText.SetTextKey(this.m_ProvinceFeaturesLabel, "RealmTooltip.settlements_and_features_label", null, null);
		}
		this.m_Initialzed = true;
	}

	// Token: 0x06002BD9 RID: 11225 RVA: 0x00170A04 File Offset: 0x0016EC04
	public bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		Logic.Realm realm = null;
		if (tooltip.vars != null)
		{
			realm = tooltip.vars.obj.Get<Logic.Realm>();
		}
		return this.HandleTooltip(realm, tooltip.vars, ui, evt);
	}

	// Token: 0x06002BDA RID: 11226 RVA: 0x00170A3C File Offset: 0x0016EC3C
	public bool HandleTooltip(Logic.Realm realm, Vars vars, BaseUI ui, Tooltip.Event evt)
	{
		this.Init();
		if (evt != Tooltip.Event.Fill && evt != Tooltip.Event.Update)
		{
			return false;
		}
		if (realm == null)
		{
			TooltipInstance component = base.GetComponent<TooltipInstance>();
			if (component != null)
			{
				component.Close(null);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			return true;
		}
		this.Data = realm;
		this.Refresh(evt != Tooltip.Event.Update);
		return true;
	}

	// Token: 0x06002BDB RID: 11227 RVA: 0x00170A9A File Offset: 0x0016EC9A
	private void Refresh(bool full = true)
	{
		this.Init();
		this.PoulateHeader();
		this.BuildStatusicons();
		this.PoplulatefeaturesAndSettlements();
	}

	// Token: 0x06002BDC RID: 11228 RVA: 0x00170AB4 File Offset: 0x0016ECB4
	private void PoulateHeader()
	{
		bool flag = this.Data.IsOccupied();
		if (this.m_OwnerCrest != null)
		{
			this.m_OwnerCrest.SetObject(this.Data.GetKingdom(), null);
		}
		if (this.m_OccupierCrest != null)
		{
			this.m_OccupierCrest.gameObject.SetActive(flag);
			if (flag)
			{
				this.m_OccupierCrest.SetObject(this.Data.GetControllingKingdom(), null);
			}
		}
		if (this.m_SpacingHack != null)
		{
			this.m_SpacingHack.gameObject.SetActive(!flag);
		}
		if (this.m_RealmName != null)
		{
			UIText.SetText(this.m_RealmName, global::Defs.Localize(this.def, "realm_name", this.Data, null, true, true));
		}
		if (this.m_Owner != null)
		{
			string text;
			if (flag)
			{
				text = global::Defs.Localize(this.def, "realm_subtitle_occupied", this.Data, null, true, true);
			}
			else
			{
				text = global::Defs.Localize(this.def, "realm_subtitle_not_occupied", this.Data, null, true, true);
			}
			UIText.SetText(this.m_Owner, text);
		}
	}

	// Token: 0x06002BDD RID: 11229 RVA: 0x00170BD4 File Offset: 0x0016EDD4
	private void BuildStatusicons()
	{
		if (this.m_StatusIconsContainer == null)
		{
			return;
		}
		UICommon.DeleteActiveChildren(this.m_StatusIconsContainer);
		if (this.m_StatusIconPrototype == null)
		{
			return;
		}
		if (this.Data == null)
		{
			return;
		}
		DT.Field field = this.def.FindChild("icons", null, true, true, true, '.');
		List<DT.Field> list = (field != null) ? field.children : null;
		if (list == null || list.Count == 0)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			DT.Field field2 = list[i];
			if (!string.IsNullOrEmpty(field2.key))
			{
				this.<BuildStatusicons>g__PopulateIcon|18_0(field2, this.m_StatusIconsContainer, this.m_StatusIconPrototype);
			}
		}
	}

	// Token: 0x06002BDE RID: 11230 RVA: 0x00170C7B File Offset: 0x0016EE7B
	private void PoplulatefeaturesAndSettlements()
	{
		if (this.m_ProvinceFeatures != null)
		{
			this.m_ProvinceFeatures.SetObject(this.Data);
		}
	}

	// Token: 0x06002BE0 RID: 11232 RVA: 0x00170C9C File Offset: 0x0016EE9C
	[CompilerGenerated]
	private void <BuildStatusicons>g__PopulateIcon|18_0(DT.Field f, RectTransform container, GameObject prototype)
	{
		GameObject gameObject = global::Common.Spawn(prototype, container, false, "");
		Image image = global::Common.FindChildComponent<Image>(gameObject, "id_StatusIcon");
		if (image != null)
		{
			image.overrideSprite = global::Defs.GetObj<Sprite>(f, "icon", this.Data);
		}
		TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(gameObject, "id_StatusText");
		if (textMeshProUGUI != null)
		{
			UIText.SetText(textMeshProUGUI, global::Defs.Localize(f, "text", this.Data, null, true, true));
		}
		gameObject.gameObject.SetActive(true);
	}

	// Token: 0x04001DCF RID: 7631
	[UIFieldTarget("id_OwnerCrest")]
	private UIKingdomIcon m_OwnerCrest;

	// Token: 0x04001DD0 RID: 7632
	[UIFieldTarget("id_OccupierCrest")]
	private UIKingdomIcon m_OccupierCrest;

	// Token: 0x04001DD1 RID: 7633
	[UIFieldTarget("id_SpacingHack")]
	private GameObject m_SpacingHack;

	// Token: 0x04001DD2 RID: 7634
	[UIFieldTarget("id_RealmName")]
	private TextMeshProUGUI m_RealmName;

	// Token: 0x04001DD3 RID: 7635
	[UIFieldTarget("id_Owner")]
	private TextMeshProUGUI m_Owner;

	// Token: 0x04001DD4 RID: 7636
	[UIFieldTarget("id_StatusIconsContainer")]
	private RectTransform m_StatusIconsContainer;

	// Token: 0x04001DD5 RID: 7637
	[UIFieldTarget("id_StatusIconPrototype")]
	private GameObject m_StatusIconPrototype;

	// Token: 0x04001DD6 RID: 7638
	[UIFieldTarget("id_RealmProvinceFeatures")]
	private GameObject m_RealmProvinceFeatures;

	// Token: 0x04001DD7 RID: 7639
	[UIFieldTarget("id_ProvinceFeaturesLabel")]
	private TextMeshProUGUI m_ProvinceFeaturesLabel;

	// Token: 0x04001DD8 RID: 7640
	private Logic.Realm Data;

	// Token: 0x04001DD9 RID: 7641
	private DT.Field def;

	// Token: 0x04001DDA RID: 7642
	private UICastleWindow.UIProvinceFeatures m_ProvinceFeatures;

	// Token: 0x04001DDB RID: 7643
	private bool m_Initialzed;
}

using System;
using System.Collections.Generic;
using System.Text;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002AD RID: 685
public class UICastleRegionInfo : MonoBehaviour
{
	// Token: 0x17000218 RID: 536
	// (get) Token: 0x06002AE5 RID: 10981 RVA: 0x0016B2F5 File Offset: 0x001694F5
	// (set) Token: 0x06002AE6 RID: 10982 RVA: 0x0016B2FD File Offset: 0x001694FD
	public Logic.Realm Realm { get; private set; }

	// Token: 0x17000219 RID: 537
	// (get) Token: 0x06002AE7 RID: 10983 RVA: 0x0016B306 File Offset: 0x00169506
	// (set) Token: 0x06002AE8 RID: 10984 RVA: 0x0016B30E File Offset: 0x0016950E
	public Castle Castle { get; private set; }

	// Token: 0x06002AE9 RID: 10985 RVA: 0x0016B317 File Offset: 0x00169517
	public void SetObject(Castle castle)
	{
		this.Init();
		this.Castle = castle;
		this.Realm = this.Castle.GetRealm();
		this.Refresh();
	}

	// Token: 0x06002AEA RID: 10986 RVA: 0x0016B340 File Offset: 0x00169540
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_ProvinceSettlementsContainer != null && this.m_SettlementIconPrototype != null)
		{
			this.m_SettlementIconPrototype.gameObject.SetActive(false);
			for (int i = 0; i < 6; i++)
			{
				UICastleRegionInfo.SettlementData orAddComponent = UnityEngine.Object.Instantiate<GameObject>(this.m_SettlementIconPrototype, this.m_ProvinceSettlementsContainer, false).GetOrAddComponent<UICastleRegionInfo.SettlementData>();
				this.m_Settelments.Add(orAddComponent);
			}
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06002AEB RID: 10987 RVA: 0x0016B3C1 File Offset: 0x001695C1
	private void Refresh()
	{
		this.PopulateSettlementIcons();
		this.PopulateGoods();
		this.PopulateProvinceFeatures();
		this.UpdateLabels();
	}

	// Token: 0x06002AEC RID: 10988 RVA: 0x0016B3DC File Offset: 0x001695DC
	private void PopulateSettlementIcons()
	{
		if (this.m_ProvinceSettlementsContainer == null)
		{
			return;
		}
		if (this.m_SettlementIconPrototype == null)
		{
			return;
		}
		List<string> list = new List<string>();
		if (this.Realm != null && this.Realm.settlements != null)
		{
			for (int i = 1; i < this.Realm.settlements.Count; i++)
			{
				Logic.Settlement settlement = this.Realm.settlements[i];
				if (!(settlement.def.id == "Castle") && !list.Contains(settlement.def.id) && settlement.IsActiveSettlement())
				{
					list.Add(settlement.def.id);
				}
			}
		}
		for (int j = 0; j < this.m_Settelments.Count; j++)
		{
			string text = (list.Count > j) ? list[j] : null;
			if (text == null)
			{
				this.m_Settelments[j].SetData(null, null);
				this.m_Settelments[j].gameObject.SetActive(false);
			}
			else
			{
				this.m_Settelments[j].SetData(this.Realm, text);
				this.m_Settelments[j].gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06002AED RID: 10989 RVA: 0x0016B520 File Offset: 0x00169720
	private void UpdateLabels()
	{
		if (this.m_ProvinceFeatures != null)
		{
			UIText.SetTextKey(this.m_ProvinceFeatures.gameObject, "id_Caption", "Castle.Window.province_features", null, null);
		}
		if (this.m_GoodsProduction != null)
		{
			UIText.SetTextKey(this.m_GoodsProduction.gameObject, "id_Caption", "Castle.Window.produce", null, null);
		}
		if (this.m_ImprotedGoods != null)
		{
			UIText.SetTextKey(this.m_ImprotedGoods.gameObject, "id_Caption", "Castle.Window.import", null, null);
		}
	}

	// Token: 0x06002AEE RID: 10990 RVA: 0x0016B5AC File Offset: 0x001697AC
	private void PopulateGoods()
	{
		if (this.Castle == null)
		{
			return;
		}
		if (this.m_GoodsContainer == null)
		{
			return;
		}
		Logic.Realm realm = this.Castle.GetRealm();
		if (realm == null)
		{
			return;
		}
		DT.Field defField = global::Defs.GetDefField("Resource", null);
		if (defField == null || defField.def == null || defField.def.defs == null)
		{
			return;
		}
		List<DT.Def> defs = defField.def.defs;
		UICommon.DeleteChildren(this.m_GoodsContainer);
		for (int i = 0; i < defs.Count; i++)
		{
			DT.Def def = defs[i];
			if (def != null && realm.HasTag(def.field.key, 1))
			{
				UIGoodsIcon.GetIcon(def.field.key, null, this.m_GoodsContainer);
			}
		}
	}

	// Token: 0x06002AEF RID: 10991 RVA: 0x0016B66C File Offset: 0x0016986C
	private void PopulateProvinceFeatures()
	{
		if (this.Castle == null)
		{
			return;
		}
		if (this.m_ProvinceFeaturesContainer == null)
		{
			return;
		}
		Logic.Realm realm = this.Castle.GetRealm();
		if (realm == null)
		{
			return;
		}
		UICommon.DeleteChildren(this.m_ProvinceFeaturesContainer);
		Game game = GameLogic.Get(true);
		if (game != null)
		{
			for (int i = 0; i < realm.features.Count; i++)
			{
				string id = realm.features[i];
				ProvinceFeature.Def def = game.defs.Get<ProvinceFeature.Def>(id);
				if (def != null && def.show_in_political_view)
				{
					UIProvinceFeature.GetIcon(realm.features[i], null, this.m_ProvinceFeaturesContainer);
				}
			}
		}
	}

	// Token: 0x06002AF0 RID: 10992 RVA: 0x0016B70D File Offset: 0x0016990D
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "structures_changed")
		{
			this.Refresh();
		}
	}

	// Token: 0x04001D13 RID: 7443
	[UIFieldTarget("id_ProvinceFeatures")]
	private RectTransform m_ProvinceFeatures;

	// Token: 0x04001D14 RID: 7444
	[UIFieldTarget("id_ProvinceFeaturesContainer")]
	private RectTransform m_ProvinceFeaturesContainer;

	// Token: 0x04001D15 RID: 7445
	[UIFieldTarget("id_GoodsProduction")]
	private RectTransform m_GoodsProduction;

	// Token: 0x04001D16 RID: 7446
	[UIFieldTarget("id_GoodsContainer")]
	private RectTransform m_GoodsContainer;

	// Token: 0x04001D17 RID: 7447
	[UIFieldTarget("id_ImprotedGoods")]
	private RectTransform m_ImprotedGoods;

	// Token: 0x04001D18 RID: 7448
	[UIFieldTarget("id_ProvinceSettlementsContainer")]
	private RectTransform m_ProvinceSettlementsContainer;

	// Token: 0x04001D19 RID: 7449
	[UIFieldTarget("id_SettlementIconPrototype")]
	private GameObject m_SettlementIconPrototype;

	// Token: 0x04001D1A RID: 7450
	private List<UICastleRegionInfo.SettlementData> m_Settelments = new List<UICastleRegionInfo.SettlementData>();

	// Token: 0x04001D1B RID: 7451
	private List<UIRealmIcon> m_RealmIcons = new List<UIRealmIcon>();

	// Token: 0x04001D1E RID: 7454
	private bool m_Initialzied;

	// Token: 0x04001D1F RID: 7455
	private const int MaxSettlementIcons = 6;

	// Token: 0x02000808 RID: 2056
	internal class SettlementRegionData : MonoBehaviour
	{
		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x06004F56 RID: 20310 RVA: 0x002350CB File Offset: 0x002332CB
		// (set) Token: 0x06004F57 RID: 20311 RVA: 0x002350D3 File Offset: 0x002332D3
		public Logic.Realm Realm { get; private set; }

		// Token: 0x06004F58 RID: 20312 RVA: 0x002350DC File Offset: 0x002332DC
		private void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initialzied = true;
		}

		// Token: 0x06004F59 RID: 20313 RVA: 0x002350F5 File Offset: 0x002332F5
		public void SetRealm(Logic.Realm realm)
		{
			this.Init();
			this.Realm = realm;
			this.Refresh();
		}

		// Token: 0x06004F5A RID: 20314 RVA: 0x0023510C File Offset: 0x0023330C
		private void Refresh()
		{
			this.UpdateGeneric();
			this.BuildToolip();
			string a = this.type;
			if (a == "Farm")
			{
				this.PopulateFarms();
				return;
			}
			if (a == "Village")
			{
				this.PopulateVillage();
				return;
			}
			if (a == "StockFarm")
			{
				this.PopulateStockFarm();
				return;
			}
			if (a == "Monastery")
			{
				this.PopulateMonastery();
				return;
			}
			if (!(a == "Keep"))
			{
				return;
			}
			this.PopulateKeep();
		}

		// Token: 0x06004F5B RID: 20315 RVA: 0x00235190 File Offset: 0x00233390
		private void UpdateGeneric()
		{
			int num;
			int num2;
			this.GetNeighboringSettlementCountsByType(this.Realm, this.type, out num, out num2);
			if (this.m_Count != null)
			{
				UIText.SetText(this.m_Count, string.Format("{0}/{1}", num2, num));
			}
			if (this.m_Icon != null)
			{
				DT.Field defField = global::Defs.GetDefField(this.type, null);
				this.m_Icon.overrideSprite = global::Defs.GetObj<Sprite>(defField, "icon", null);
			}
		}

		// Token: 0x06004F5C RID: 20316 RVA: 0x00235214 File Offset: 0x00233414
		private void BuildToolip()
		{
			DT.Field defField = global::Defs.GetDefField(this.type, null);
			Vars vars = new Vars(this.Realm);
			vars.Set<string>("caption", "#" + global::Defs.Localize("Castle.Region." + this.type, vars, null, true, true));
			UICastleRegionInfo.SettlementRegionData.stringBuilder.Clear();
			UICastleRegionInfo.SettlementRegionData.relevantRealms.Clear();
			UICastleRegionInfo.SettlementRegionData.relevantRealms.Add(this.Realm);
			UICastleRegionInfo.SettlementRegionData.relevantRealms.AddRange(this.Realm.logicNeighborsAll);
			for (int i = 0; i < UICastleRegionInfo.SettlementRegionData.relevantRealms.Count; i++)
			{
				int num = 0;
				Logic.Realm realm = UICastleRegionInfo.SettlementRegionData.relevantRealms[i];
				for (int j = 0; j < realm.settlements.Count; j++)
				{
					if (realm.settlements[j].type == this.type)
					{
						num++;
					}
				}
				if (i == 0)
				{
					UICastleRegionInfo.SettlementRegionData.stringBuilder.Append("<b>");
				}
				UICastleRegionInfo.SettlementRegionData.stringBuilder.Append(global::Defs.Localize(realm.GetNameKey(null, ""), null, null, true, true));
				UICastleRegionInfo.SettlementRegionData.stringBuilder.Append(": ");
				UICastleRegionInfo.SettlementRegionData.stringBuilder.Append(num);
				if (i == 0)
				{
					UICastleRegionInfo.SettlementRegionData.stringBuilder.Append("</b>");
				}
				if (i % 2 == 0)
				{
					UICastleRegionInfo.SettlementRegionData.stringBuilder.Append("<pos=45%>");
				}
				else
				{
					UICastleRegionInfo.SettlementRegionData.stringBuilder.Append(Environment.NewLine);
				}
			}
			vars.Set<string>("body", "#" + UICastleRegionInfo.SettlementRegionData.stringBuilder);
			vars.Set<Sprite>("icon", global::Defs.GetObj<Sprite>(defField, "icon", null));
			Tooltip.Get(base.gameObject, true).SetDef("RegionSettlementsTooltip", vars);
		}

		// Token: 0x06004F5D RID: 20317 RVA: 0x000023FD File Offset: 0x000005FD
		private void PopulateFarms()
		{
		}

		// Token: 0x06004F5E RID: 20318 RVA: 0x000023FD File Offset: 0x000005FD
		private void PopulateVillage()
		{
		}

		// Token: 0x06004F5F RID: 20319 RVA: 0x000023FD File Offset: 0x000005FD
		private void PopulateStockFarm()
		{
		}

		// Token: 0x06004F60 RID: 20320 RVA: 0x000023FD File Offset: 0x000005FD
		private void PopulateMonastery()
		{
		}

		// Token: 0x06004F61 RID: 20321 RVA: 0x000023FD File Offset: 0x000005FD
		private void PopulateKeep()
		{
		}

		// Token: 0x06004F62 RID: 20322 RVA: 0x002353E4 File Offset: 0x002335E4
		private void GetNeighboringSettlementCountsByType(Logic.Realm realm, string sType, out int total, out int inrealm)
		{
			inrealm = 0;
			total = 0;
			for (int i = 0; i < this.Realm.logicNeighborsAll.Count; i++)
			{
				Logic.Realm realm2 = this.Realm.logicNeighborsAll[i];
				for (int j = 0; j < realm2.settlements.Count; j++)
				{
					if (realm2.settlements[j].type == sType)
					{
						total++;
					}
				}
			}
			for (int k = 0; k < this.Realm.settlements.Count; k++)
			{
				if (this.Realm.settlements[k].type == sType)
				{
					total++;
					inrealm++;
				}
			}
		}

		// Token: 0x04003D72 RID: 15730
		[UIFieldTarget("id_Icon")]
		private Image m_Icon;

		// Token: 0x04003D73 RID: 15731
		[UIFieldTarget("id_Count")]
		private TextMeshProUGUI m_Count;

		// Token: 0x04003D74 RID: 15732
		public string type;

		// Token: 0x04003D76 RID: 15734
		private bool m_Initialzied;

		// Token: 0x04003D77 RID: 15735
		private static StringBuilder stringBuilder = new StringBuilder();

		// Token: 0x04003D78 RID: 15736
		private static List<Logic.Realm> relevantRealms = new List<Logic.Realm>();
	}

	// Token: 0x02000809 RID: 2057
	internal class SettlementData : MonoBehaviour
	{
		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x06004F65 RID: 20325 RVA: 0x002354B7 File Offset: 0x002336B7
		// (set) Token: 0x06004F66 RID: 20326 RVA: 0x002354BF File Offset: 0x002336BF
		public Logic.Realm Realm { get; private set; }

		// Token: 0x06004F67 RID: 20327 RVA: 0x002354C8 File Offset: 0x002336C8
		private void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initialzied = true;
		}

		// Token: 0x06004F68 RID: 20328 RVA: 0x002354E1 File Offset: 0x002336E1
		public void SetData(Logic.Realm realm, string type)
		{
			this.Init();
			this.Realm = realm;
			this.Type = type;
			this.Refresh();
		}

		// Token: 0x06004F69 RID: 20329 RVA: 0x002354FD File Offset: 0x002336FD
		private void Refresh()
		{
			this.UpdateGeneric();
			this.BuildToolip();
		}

		// Token: 0x06004F6A RID: 20330 RVA: 0x0023550C File Offset: 0x0023370C
		private void UpdateGeneric()
		{
			if (this.m_Count != null)
			{
				UIText.SetText(this.m_Count, this.GetCount(this.Type).ToString());
			}
			if (this.m_Icon != null)
			{
				DT.Field defField = global::Defs.GetDefField(this.Type, null);
				this.m_Icon.overrideSprite = global::Defs.GetObj<Sprite>(defField, "icon", null);
			}
		}

		// Token: 0x06004F6B RID: 20331 RVA: 0x00235578 File Offset: 0x00233778
		private void BuildToolip()
		{
			DT.Field defField = global::Defs.GetDefField(this.Type, null);
			Vars vars = new Vars(defField);
			vars.Set<Sprite>("icon", global::Defs.GetObj<Sprite>(defField, "icon", null));
			Tooltip.Get(base.gameObject, true).SetDef("SettlementTooltip", vars);
		}

		// Token: 0x06004F6C RID: 20332 RVA: 0x002355C8 File Offset: 0x002337C8
		private int GetCount(string type)
		{
			int num = 0;
			if (this.Realm == null)
			{
				return num;
			}
			if (this.Realm.settlements == null)
			{
				return num;
			}
			for (int i = 0; i < this.Realm.settlements.Count; i++)
			{
				if (this.Realm.settlements[i].type == type)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x04003D79 RID: 15737
		[UIFieldTarget("id_Icon")]
		private Image m_Icon;

		// Token: 0x04003D7A RID: 15738
		[UIFieldTarget("id_Count")]
		private TextMeshProUGUI m_Count;

		// Token: 0x04003D7B RID: 15739
		public string Type;

		// Token: 0x04003D7D RID: 15741
		private bool m_Initialzied;
	}
}

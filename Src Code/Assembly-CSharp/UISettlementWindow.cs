using System;
using System.Runtime.CompilerServices;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002BE RID: 702
public class UISettlementWindow : ObjectWindow, IPoolable
{
	// Token: 0x1700022F RID: 559
	// (get) Token: 0x06002C02 RID: 11266 RVA: 0x001717C2 File Offset: 0x0016F9C2
	// (set) Token: 0x06002C03 RID: 11267 RVA: 0x001717CA File Offset: 0x0016F9CA
	public Village Data { get; protected set; }

	// Token: 0x06002C04 RID: 11268 RVA: 0x001717D3 File Offset: 0x0016F9D3
	private void Start()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x06002C05 RID: 11269 RVA: 0x001717DB File Offset: 0x0016F9DB
	private void OnEnable()
	{
		if (this.m_Initialzied)
		{
			this.ExtractLogicObject();
		}
	}

	// Token: 0x06002C06 RID: 11270 RVA: 0x001717EC File Offset: 0x0016F9EC
	private void ExtractLogicObject()
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		if (worldUI.selected_obj == null)
		{
			return;
		}
		global::Settlement component = worldUI.selected_obj.GetComponent<global::Settlement>();
		if (component != null)
		{
			this.SetObject(component.logic, new Vars(component.logic));
		}
	}

	// Token: 0x06002C07 RID: 11271 RVA: 0x00171849 File Offset: 0x0016FA49
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x06002C08 RID: 11272 RVA: 0x00171862 File Offset: 0x0016FA62
	protected override void Update()
	{
		base.Update();
		this.UpdateUpgrade();
	}

	// Token: 0x06002C09 RID: 11273 RVA: 0x00171870 File Offset: 0x0016FA70
	public override void SetObject(Logic.Object obj, Vars vars = null)
	{
		this.Init();
		this.RemoveListeners();
		base.SetObject(obj, vars);
		UICommon.FindComponents(this, false);
		this.Data = (obj as Village);
		this.AddListeners();
		this.Refresh();
	}

	// Token: 0x06002C0A RID: 11274 RVA: 0x001718A5 File Offset: 0x0016FAA5
	public override void Refresh()
	{
		if (this.Data == null)
		{
			return;
		}
		this.UpdateStatics();
		this.BuildIllustration();
		this.UpdateResources();
	}

	// Token: 0x06002C0B RID: 11275 RVA: 0x001718C4 File Offset: 0x0016FAC4
	private void UpdateStatics()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_KingdomIcon != null && this.m_KingdomIcon.Length != 0)
		{
			UIKingdomIcon uikingdomIcon = this.m_KingdomIcon[0];
			if (uikingdomIcon != null)
			{
				uikingdomIcon.SetObject(this.Data, null);
			}
			if (this.m_KingdomIcon.Length > 1)
			{
				UIKingdomIcon uikingdomIcon2 = this.m_KingdomIcon[1];
				if (uikingdomIcon2 != null)
				{
					uikingdomIcon2.SetObject(this.Data.GetController(), null);
				}
			}
		}
		if (this.m_Province != null)
		{
			TMP_Text province = this.m_Province;
			string key = "Settlement.realm_castle";
			Village data = this.Data;
			UIText.SetTextKey(province, key, new Vars((data != null) ? data.GetRealm() : null), null);
		}
		if (this.m_Caption != null)
		{
			UIText.SetText(this.m_Caption, global::Defs.Localize(this.Data.def.field, "caption", this.Data, null, true, true));
		}
		if (this.m_Body != null)
		{
			UIText.SetText(this.m_Body, this.Data.def.field, "description", new Vars(this.Data), null);
		}
	}

	// Token: 0x06002C0C RID: 11276 RVA: 0x001719E8 File Offset: 0x0016FBE8
	private void BuildIllustration()
	{
		string text = this.Data.game.GetClimateZone(this.Data.position).ToString();
		bool @bool = this.Data.GetRealm().def.GetBool("has_mountains", null, false, true, true, true, '.');
		bool flag = this.Data.game.GetTerrainType(this.Data.position) == TerrainType.Hills;
		bool bool2 = this.Data.GetRealm().def.GetBool("has_rivers", null, false, true, true, true, '.');
		bool coastal = this.Data.coastal;
		bool forest = this.Data.forest;
		bool flag2 = this.Data.level == 0;
		if (this.m_Settlement != null)
		{
			Sprite illustration = this.GetIllustration(this.Data, text, flag2);
			this.m_Settlement.gameObject.SetActive(illustration != null);
			if (illustration != null)
			{
				this.m_Settlement.sprite = illustration;
			}
		}
		if (this.m_PillageMood != null)
		{
			this.m_PillageMood.gameObject.SetActive(flag2);
		}
		UISettlementWindow.<BuildIllustration>g__UpdateFeatureSprite|36_0(true, this.m_Sky, text, "sky");
		UISettlementWindow.<BuildIllustration>g__UpdateFeatureSprite|36_0(@bool, this.m_Mountain, text, "mountains");
		UISettlementWindow.<BuildIllustration>g__UpdateFeatureSprite|36_0(@bool && !coastal, this.m_Mountain_Left, text, "mountains_left");
		UISettlementWindow.<BuildIllustration>g__UpdateFeatureSprite|36_0(flag, this.m_Hills, text, "hills");
		UISettlementWindow.<BuildIllustration>g__UpdateFeatureSprite|36_0(flag && !coastal, this.m_Hills_Left, text, "hills_left");
		UISettlementWindow.<BuildIllustration>g__UpdateFeatureSprite|36_0(true, this.m_TerrainBase, text, "base");
		UISettlementWindow.<BuildIllustration>g__UpdateFeatureSprite|36_0(forest, this.m_Forest, text, "forest");
		UISettlementWindow.<BuildIllustration>g__UpdateFeatureSprite|36_0(forest && !coastal, this.m_Forest_Left, text, "forest_left");
		UISettlementWindow.<BuildIllustration>g__UpdateFeatureSprite|36_0(coastal, this.m_Ocean, text, "ocean");
		UISettlementWindow.<BuildIllustration>g__UpdateFeatureSprite|36_0(bool2, this.m_River, text, "river");
	}

	// Token: 0x06002C0D RID: 11277 RVA: 0x00171BF4 File Offset: 0x0016FDF4
	private Sprite GetIllustration(Village village, string climateZome, bool isPilleged)
	{
		global::Settlement settlement = ((village != null) ? village.visuals : null) as global::Settlement;
		string str = (settlement != null) ? settlement.houses_architecture : "";
		string text = "settlements." + village.type + (isPilleged ? "_Pillaged" : "");
		string text2 = text + "." + str;
		string key = text2 + "." + climateZome;
		Sprite obj = global::Defs.GetObj<Sprite>("SettlementIllustrationSettings", key, null);
		if (obj == null)
		{
			obj = global::Defs.GetObj<Sprite>("SettlementIllustrationSettings", text2, null);
		}
		if (obj == null)
		{
			obj = global::Defs.GetObj<Sprite>("SettlementIllustrationSettings", text, null);
		}
		return obj;
	}

	// Token: 0x06002C0E RID: 11278 RVA: 0x000023FD File Offset: 0x000005FD
	private void UpdateResources()
	{
	}

	// Token: 0x06002C0F RID: 11279 RVA: 0x00171CA8 File Offset: 0x0016FEA8
	private void UpdateUpgrade()
	{
		if (this.Data == null)
		{
			return;
		}
		bool active = false;
		if (this.Data.settlementUpgrade != null && this.Data.settlementUpgrade.upgrading)
		{
			active = true;
			if (this.m_ProgressBarForeground != null)
			{
				this.m_ProgressBarForeground.fillAmount = this.Data.settlementUpgrade.progress;
			}
		}
		GameObject upgradeProgress = this.m_UpgradeProgress;
		if (upgradeProgress != null)
		{
			upgradeProgress.gameObject.SetActive(active);
		}
		GameObject upgradeSpeed = this.m_UpgradeSpeed;
		if (upgradeSpeed == null)
		{
			return;
		}
		upgradeSpeed.gameObject.SetActive(active);
	}

	// Token: 0x06002C10 RID: 11280 RVA: 0x00171D37 File Offset: 0x0016FF37
	public override void AddListeners()
	{
		base.AddListeners();
		Village data = this.Data;
		if (data == null)
		{
			return;
		}
		Logic.Realm realm = data.GetRealm();
		if (realm == null)
		{
			return;
		}
		realm.AddListener(this);
	}

	// Token: 0x06002C11 RID: 11281 RVA: 0x00171D5A File Offset: 0x0016FF5A
	public override void RemoveListeners()
	{
		base.RemoveListeners();
		Village data = this.Data;
		if (data == null)
		{
			return;
		}
		Logic.Realm realm = data.GetRealm();
		if (realm == null)
		{
			return;
		}
		realm.DelListener(this);
	}

	// Token: 0x06002C12 RID: 11282 RVA: 0x00171D80 File Offset: 0x0016FF80
	protected override void HandleLogicMessage(object obj, string message, object param)
	{
		base.HandleLogicMessage(obj, message, param);
		if (message == "destroying" || message == "finishing")
		{
			this.RemoveListeners();
			return;
		}
		if (message == "level_changed")
		{
			this.UpdateResources();
			this.BuildIllustration();
			return;
		}
		if (!(message == "religion_changed"))
		{
			return;
		}
		this.BuildIllustration();
	}

	// Token: 0x06002C13 RID: 11283 RVA: 0x00171DE5 File Offset: 0x0016FFE5
	public override void Release()
	{
		base.Release();
		this.Data = null;
	}

	// Token: 0x06002C14 RID: 11284 RVA: 0x001717D3 File Offset: 0x0016F9D3
	public override void ValidateSelectionObject()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x06002C15 RID: 11285 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolSpawned()
	{
	}

	// Token: 0x06002C16 RID: 11286 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolActivated()
	{
	}

	// Token: 0x06002C17 RID: 11287 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x06002C18 RID: 11288 RVA: 0x00171DF4 File Offset: 0x0016FFF4
	public void OnPoolDeactivated()
	{
		this.Data = null;
		this.OnDestroy();
	}

	// Token: 0x06002C1A RID: 11290 RVA: 0x00171E04 File Offset: 0x00170004
	[CompilerGenerated]
	internal static void <BuildIllustration>g__UpdateFeatureSprite|36_0(bool available, Image i, string cz, string feature)
	{
		if (i == null)
		{
			return;
		}
		i.gameObject.SetActive(available);
		if (!available)
		{
			return;
		}
		Sprite obj = global::Defs.GetObj<Sprite>("SettlementIllustrationSettings", "terrain." + feature + "." + cz, null);
		if (obj == null)
		{
			obj = global::Defs.GetObj<Sprite>("SettlementIllustrationSettings", "terrain." + feature, null);
		}
		i.gameObject.SetActive(obj != null);
		if (obj != null)
		{
			i.overrideSprite = obj;
		}
	}

	// Token: 0x04001DF7 RID: 7671
	[UIFieldTarget("id_KingdomIcon")]
	private UIKingdomIcon[] m_KingdomIcon;

	// Token: 0x04001DF8 RID: 7672
	[UIFieldTarget("id_Caption")]
	protected TextMeshProUGUI m_Caption;

	// Token: 0x04001DF9 RID: 7673
	[UIFieldTarget("id_Body")]
	protected TextMeshProUGUI m_Body;

	// Token: 0x04001DFA RID: 7674
	[UIFieldTarget("id_Province")]
	protected TextMeshProUGUI m_Province;

	// Token: 0x04001DFB RID: 7675
	[UIFieldTarget("id_Icon")]
	protected Image m_Icon;

	// Token: 0x04001DFC RID: 7676
	[UIFieldTarget("id_Illustration")]
	protected Image m_Illustration;

	// Token: 0x04001DFD RID: 7677
	[UIFieldTarget("id_ResourcesContainer")]
	protected RectTransform m_ResourcesContainer;

	// Token: 0x04001DFE RID: 7678
	[UIFieldTarget("id_ResourcesPrototype")]
	protected GameObject m_ResourcePrototype;

	// Token: 0x04001DFF RID: 7679
	[UIFieldTarget("id_ProgressBar")]
	protected GameObject m_UpgradeProgress;

	// Token: 0x04001E00 RID: 7680
	[UIFieldTarget("id_ProgressBarForeground")]
	protected Image m_ProgressBarForeground;

	// Token: 0x04001E01 RID: 7681
	[UIFieldTarget("id_UpgradeSpeed")]
	protected GameObject m_UpgradeSpeed;

	// Token: 0x04001E02 RID: 7682
	[UIFieldTarget("id_Sky")]
	protected Image m_Sky;

	// Token: 0x04001E03 RID: 7683
	[UIFieldTarget("id_Mountain")]
	protected Image m_Mountain;

	// Token: 0x04001E04 RID: 7684
	[UIFieldTarget("id_Mountain_Left")]
	protected Image m_Mountain_Left;

	// Token: 0x04001E05 RID: 7685
	[UIFieldTarget("id_Hills")]
	protected Image m_Hills;

	// Token: 0x04001E06 RID: 7686
	[UIFieldTarget("id_Hills_Left")]
	protected Image m_Hills_Left;

	// Token: 0x04001E07 RID: 7687
	[UIFieldTarget("id_TerrainBase")]
	protected Image m_TerrainBase;

	// Token: 0x04001E08 RID: 7688
	[UIFieldTarget("id_Ocean")]
	protected Image m_Ocean;

	// Token: 0x04001E09 RID: 7689
	[UIFieldTarget("id_River")]
	protected Image m_River;

	// Token: 0x04001E0A RID: 7690
	[UIFieldTarget("id_Forest")]
	protected Image m_Forest;

	// Token: 0x04001E0B RID: 7691
	[UIFieldTarget("id_Forest_Left")]
	protected Image m_Forest_Left;

	// Token: 0x04001E0C RID: 7692
	[UIFieldTarget("id_Settlement")]
	protected Image m_Settlement;

	// Token: 0x04001E0D RID: 7693
	[UIFieldTarget("id_PillageMood")]
	protected Image m_PillageMood;

	// Token: 0x04001E0F RID: 7695
	private bool m_Initialzied;
}

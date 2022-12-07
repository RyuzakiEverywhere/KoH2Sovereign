using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000222 RID: 546
public class UIGreatPowersWindow : UIWindow, IListener
{
	// Token: 0x06002107 RID: 8455 RVA: 0x0012C65E File Offset: 0x0012A85E
	public override string GetDefId()
	{
		return UIGreatPowersWindow.def_id;
	}

	// Token: 0x170001AB RID: 427
	// (get) Token: 0x06002108 RID: 8456 RVA: 0x0012C665 File Offset: 0x0012A865
	// (set) Token: 0x06002109 RID: 8457 RVA: 0x0012C66D File Offset: 0x0012A86D
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x0600210A RID: 8458 RVA: 0x0012C676 File Offset: 0x0012A876
	// (set) Token: 0x0600210B RID: 8459 RVA: 0x0012C67E File Offset: 0x0012A87E
	public GreatPowers Data { get; private set; }

	// Token: 0x0600210C RID: 8460 RVA: 0x0012C688 File Offset: 0x0012A888
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_ButtonClose != null)
		{
			this.m_ButtonClose.onClick = new BSGButton.OnClick(this.HandleOnClose);
		}
		if (this.m_ButtonVictory != null)
		{
			this.m_ButtonVictory.onClick = new BSGButton.OnClick(this.HandleOnVicotryButton);
		}
		if (this.m_KingdomSlotPrototype != null)
		{
			this.m_KingdomSlotPrototype.SetActive(false);
		}
		if (this.m_Player != null)
		{
			this.m_Player.onClick = new BSGButton.OnClick(this.HandleSelectPlayer);
		}
		if (this.m_PlayerKing != null)
		{
			this.m_PlayerKing.OnSelect += new Action<UICharacterIcon>(this.HandleSelectPlayer);
			UIKingdomIcon uikingdomIcon = global::Common.FindChildComponent<UIKingdomIcon>(this.m_PlayerKing.gameObject, "_Crest");
			KingdomShield kingdomShield = (uikingdomIcon != null) ? uikingdomIcon.GetPrimary() : null;
			if (kingdomShield != null)
			{
				kingdomShield.onClick = new KingdomShield.OnShieldClick(this.HandleShieldClick);
			}
		}
		this.AllocateRankingIcons();
		this.tooltipVars = new Vars();
		if (this.m_AdditinalFameSourcesContianer != null)
		{
			this.m_additnalFameSources = this.m_AdditinalFameSourcesContianer.AddComponent<UIGreatPowersWindow.AdditnalFameSources>();
		}
		this.windowDef = global::Defs.GetDefField(this.GetDefId(), null);
		this.clamLabelMaterialNormal = global::Defs.GetObj<Material>(this.windowDef, "claim_vitory_button.material_normal", null);
		this.clamLabelMaterialDisabled = global::Defs.GetObj<Material>(this.windowDef, "claim_vitory_button.material_disabled", null);
		this.m_Initialized = true;
	}

	// Token: 0x0600210D RID: 8461 RVA: 0x0012C808 File Offset: 0x0012AA08
	public void SetObject(GreatPowers ranking)
	{
		this.Init();
		this.Data = ranking;
		if (this.Kingdom != null)
		{
			this.Kingdom.DelListener(this);
		}
		this.Kingdom = BaseUI.LogicKingdom();
		if (this.Kingdom != null)
		{
			this.Kingdom.game.AddListener(this);
			this.Kingdom.AddListener(this);
		}
		this.tooltipVars.obj = this.Kingdom;
		if (this.m_ButtonVictory != null)
		{
			Tooltip.Get(this.m_ButtonVictory.gameObject, true).SetDef("FameVictoryTooltip", this.tooltipVars);
		}
		if (this.m_PlayerFameContainer != null)
		{
			Tooltip.Get(this.m_PlayerFameContainer.gameObject, true).SetDef("FameTooltip", this.tooltipVars);
		}
		if (this.m_KingdomShield != null)
		{
			this.m_KingdomShield.SetObject(this.Kingdom, null);
		}
		if (this.m_additnalFameSources != null)
		{
			this.m_additnalFameSources.SetData(this.Kingdom, this.windowDef);
		}
		this.SelectKingdom(null);
	}

	// Token: 0x0600210E RID: 8462 RVA: 0x0012C928 File Offset: 0x0012AB28
	private void SelectKingdom(Logic.Kingdom kingdom)
	{
		this.selectedKingdom = kingdom;
		for (int i = 0; i < this.m_TopRankingKingdoms.Count; i++)
		{
			UIGreatPowersWindow.KingdomSlot kingdomSlot = this.m_TopRankingKingdoms[i];
			kingdomSlot.SetFocused(this.selectedKingdom != null && kingdomSlot.Kingdom == this.selectedKingdom);
		}
		if (this.m_PlayerSelected != null)
		{
			this.m_PlayerSelected.gameObject.SetActive(this.selectedKingdom == null);
		}
		this.tooltipVars.Set<bool>("hide_sources", this.selectedKingdom == null);
		this.Refresh();
	}

	// Token: 0x0600210F RID: 8463 RVA: 0x0012C9C4 File Offset: 0x0012ABC4
	private void Refresh()
	{
		this.PopulateLabes();
		this.UpdatePlayerKing();
		this.PopulateAdvatages();
		this.UpdateVictoryButton();
		this.UpdateRanking();
	}

	// Token: 0x06002110 RID: 8464 RVA: 0x0012C9E4 File Offset: 0x0012ABE4
	protected override void Update()
	{
		base.Update();
		if (this.m_PlayerFame != null && this.Kingdom.fame != this.m_LastFame)
		{
			this.m_LastFame = this.Kingdom.fame;
			this.m_PlayerFame.text = this.m_LastFame.ToString();
			this.UpdateVictoryButton();
		}
	}

	// Token: 0x06002111 RID: 8465 RVA: 0x0012CA48 File Offset: 0x0012AC48
	private void AllocateRankingIcons()
	{
		if (this.m_KingdomSlotPrototype == null)
		{
			return;
		}
		if (this.m_GreathPowersContianer == null)
		{
			return;
		}
		int num = 9;
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = global::Common.Spawn(this.m_KingdomSlotPrototype, this.m_GreathPowersContianer, false, "");
			UIGreatPowersWindow.KingdomSlot kingdomSlot = gameObject.AddComponent<UIGreatPowersWindow.KingdomSlot>();
			UIGreatPowersWindow.KingdomSlot kingdomSlot2 = kingdomSlot;
			kingdomSlot2.OnSelect = (Action<UIGreatPowersWindow.KingdomSlot>)Delegate.Combine(kingdomSlot2.OnSelect, new Action<UIGreatPowersWindow.KingdomSlot>(this.HandleOnKingdomSelect));
			kingdomSlot.SetData(this.Kingdom);
			gameObject.SetActive(true);
			this.m_TopRankingKingdoms.Add(kingdomSlot);
			LayoutElement orAddComponent = gameObject.GetOrAddComponent<LayoutElement>();
			if (i % 2 == 0)
			{
				gameObject.transform.SetAsFirstSibling();
			}
			else
			{
				gameObject.transform.SetAsLastSibling();
			}
			if (i != 0 && i < 3)
			{
				orAddComponent.preferredWidth *= this.m_FirstTierScale;
				orAddComponent.preferredHeight *= this.m_FirstTierScale;
			}
			else if (i >= 3)
			{
				orAddComponent.preferredWidth *= this.m_SecoundTierScale;
				orAddComponent.preferredHeight *= this.m_SecoundTierScale;
			}
		}
	}

	// Token: 0x06002112 RID: 8466 RVA: 0x0012CB6C File Offset: 0x0012AD6C
	private void UpdateRanking()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		Logic.Kingdom kingdom = this.Kingdom;
		List<Logic.Kingdom> list;
		if (kingdom == null)
		{
			list = null;
		}
		else
		{
			Game game = kingdom.game;
			if (game == null)
			{
				list = null;
			}
			else
			{
				GreatPowers great_powers = game.great_powers;
				list = ((great_powers != null) ? great_powers.TopKingdoms(false) : null);
			}
		}
		List<Logic.Kingdom> list2 = list;
		if (this.selectedKingdom != null)
		{
			if (this.selectedKingdom.IsDefeated())
			{
				this.SelectKingdom(null);
				return;
			}
			if (!list2.Contains(this.selectedKingdom))
			{
				this.SelectKingdom(null);
				return;
			}
		}
		for (int i = 0; i < this.m_TopRankingKingdoms.Count; i++)
		{
			Logic.Kingdom data = (i < list2.Count) ? list2[i] : null;
			this.m_TopRankingKingdoms[i].SetData(data);
		}
	}

	// Token: 0x06002113 RID: 8467 RVA: 0x0012CC1D File Offset: 0x0012AE1D
	private void UpdatePlayerKing()
	{
		if (this.m_PlayerKing != null)
		{
			this.m_PlayerKing.SetObject(this.Kingdom.GetKing(), null);
		}
	}

	// Token: 0x06002114 RID: 8468 RVA: 0x0012CC44 File Offset: 0x0012AE44
	private void UpdatePlayerSelection()
	{
		if (this.m_PlayerFame != null)
		{
			this.m_PlayerFame.text = this.Kingdom.fame.ToString();
		}
	}

	// Token: 0x06002115 RID: 8469 RVA: 0x0012CC80 File Offset: 0x0012AE80
	private void PopulateLabes()
	{
		UIText.SetTextKey(this.m_Caption, "FameWindow.caption", new Vars(this.Kingdom), null);
		UIText.SetTextKey(this.m_LabelVictory, "FameWindow.claim_victory", new Vars(this.Kingdom), null);
	}

	// Token: 0x06002116 RID: 8470 RVA: 0x0012CCD0 File Offset: 0x0012AED0
	private void PopulateAdvatages()
	{
		if (this.m_CategoriesContainer == null)
		{
			return;
		}
		Logic.Kingdom kingdom = this.selectedKingdom ?? this.Kingdom;
		if (kingdom == null)
		{
			return;
		}
		List<KingdomRankingCategory> list;
		if (kingdom == null)
		{
			list = null;
		}
		else
		{
			KingdomRankingCategories rankingCategories = kingdom.rankingCategories;
			list = ((rankingCategories != null) ? rankingCategories.categories : null);
		}
		List<KingdomRankingCategory> list2 = list;
		if (list2 == null)
		{
			return;
		}
		this.m_CategorySlots = this.m_CategoriesContainer.GetComponentsInChildren<UIKingdomRankingCategory>();
		if (this.m_CategorySlots != null && this.m_CategorySlots.Length != 0)
		{
			for (int i = 0; i < this.m_CategorySlots.Length; i++)
			{
				KingdomRankingCategory @object = (list2 != null && list2.Count > i) ? list2[i] : null;
				this.m_CategorySlots[i].DisableTooltip(true);
				this.m_CategorySlots[i].SetObject(@object);
			}
		}
		if (this.m_additnalFameSources != null)
		{
			this.m_additnalFameSources.SetData(kingdom, this.windowDef);
		}
	}

	// Token: 0x06002117 RID: 8471 RVA: 0x0012CDA8 File Offset: 0x0012AFA8
	private void UpdateAdvantages()
	{
		if (this.m_CategoriesContainer == null)
		{
			return;
		}
		if (this.m_CategorySlots == null || this.m_CategorySlots.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this.m_CategorySlots.Length; i++)
		{
			if (!(this.m_CategorySlots[i] == null))
			{
				this.m_CategorySlots[i].Refresh();
			}
		}
	}

	// Token: 0x06002118 RID: 8472 RVA: 0x0012CE08 File Offset: 0x0012B008
	private void UpdateVictoryButton()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		bool flag = this.Data.game.emperorOfTheWorld.ValidateVote(this.Kingdom);
		if (this.m_ButtonVictory != null)
		{
			this.m_ButtonVictory.Enable(flag, false);
		}
		if (this.m_LabelVictoryProgress != null)
		{
			UIText.SetTextKey(this.m_LabelVictoryProgress, "FameWindow.claim_victory_progress", this.Kingdom, null);
		}
		if (this.clamLabelMaterialNormal != null && this.clamLabelMaterialDisabled != null)
		{
			if (this.m_LabelVictory != null)
			{
				this.m_LabelVictory.fontSharedMaterial = (flag ? this.clamLabelMaterialNormal : this.clamLabelMaterialDisabled);
			}
			if (this.m_LabelVictoryProgress != null)
			{
				this.m_LabelVictoryProgress.fontSharedMaterial = (flag ? this.clamLabelMaterialNormal : this.clamLabelMaterialDisabled);
			}
		}
	}

	// Token: 0x06002119 RID: 8473 RVA: 0x0012CEEA File Offset: 0x0012B0EA
	private bool HandleShieldClick(PointerEventData e, KingdomShield s)
	{
		this.SelectKingdom(null);
		return true;
	}

	// Token: 0x0600211A RID: 8474 RVA: 0x0012CEF4 File Offset: 0x0012B0F4
	private void HandleSelectPlayer(object b)
	{
		this.SelectKingdom(null);
	}

	// Token: 0x0600211B RID: 8475 RVA: 0x0012CEFD File Offset: 0x0012B0FD
	private void HandleOnKingdomSelect(UIGreatPowersWindow.KingdomSlot slot)
	{
		if (slot.Kingdom == this.selectedKingdom)
		{
			this.SelectKingdom(null);
			return;
		}
		this.SelectKingdom(slot.Kingdom);
	}

	// Token: 0x0600211C RID: 8476 RVA: 0x0012CF21 File Offset: 0x0012B121
	private void HandleOnVicotryButton(BSGButton btn)
	{
		MessageWnd.Create("EmperorOfTheWorldConfirmationMessage", null, null, delegate(MessageWnd wnd, string btn_id)
		{
			wnd.Close(false);
			if (btn_id == "ok")
			{
				this.Close(false);
				this.Data.game.emperorOfTheWorld.StartVote(BaseUI.LogicKingdom());
			}
			return true;
		});
	}

	// Token: 0x0600211D RID: 8477 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleOnClose(BSGButton bnt)
	{
		this.Close(false);
	}

	// Token: 0x0600211E RID: 8478 RVA: 0x0012CF3C File Offset: 0x0012B13C
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "ranking_updated")
		{
			this.UpdateRanking();
			return;
		}
		if (message == "royal_new_sovereign")
		{
			this.UpdatePlayerKing();
			return;
		}
		if (!(message == "game_pause_changed"))
		{
			return;
		}
		this.UpdateVictoryButton();
	}

	// Token: 0x0600211F RID: 8479 RVA: 0x0012CF7C File Offset: 0x0012B17C
	protected override void OnDestroy()
	{
		if (UIGreatPowersWindow.instance == this)
		{
			UIGreatPowersWindow.instance = null;
		}
		Logic.Kingdom kingdom = this.Kingdom;
		if (kingdom != null)
		{
			kingdom.DelListener(this);
		}
		Logic.Kingdom kingdom2 = this.Kingdom;
		if (kingdom2 != null)
		{
			Game game = kingdom2.game;
			if (game != null)
			{
				game.DelListener(this);
			}
		}
		base.OnDestroy();
	}

	// Token: 0x06002120 RID: 8480 RVA: 0x0012CFD1 File Offset: 0x0012B1D1
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIGreatPowersWindow.def_id, null);
	}

	// Token: 0x06002121 RID: 8481 RVA: 0x0012CFE0 File Offset: 0x0012B1E0
	public static void ToggleOpen(GreatPowers great_powers)
	{
		if (great_powers == null)
		{
			if (UIGreatPowersWindow.current != null)
			{
				UIGreatPowersWindow.current.Close(false);
			}
			return;
		}
		if (UIGreatPowersWindow.current != null)
		{
			UIGreatPowersWindow uigreatPowersWindow = UIGreatPowersWindow.current;
			if (((uigreatPowersWindow != null) ? uigreatPowersWindow.Data : null) == great_powers)
			{
				UIGreatPowersWindow.current.Close(false);
				return;
			}
			UIGreatPowersWindow.current.SetObject(great_powers);
			return;
		}
		else
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return;
			}
			GameObject prefab = UIGreatPowersWindow.GetPrefab();
			if (prefab == null)
			{
				return;
			}
			GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
			if (gameObject != null)
			{
				UICommon.DeleteChildren(gameObject.transform, typeof(UIGreatPowersWindow));
				UIGreatPowersWindow.current = UIGreatPowersWindow.Create(great_powers, prefab, gameObject.transform as RectTransform);
			}
			return;
		}
	}

	// Token: 0x06002122 RID: 8482 RVA: 0x0012D0AB File Offset: 0x0012B2AB
	public static bool IsActive()
	{
		return UIGreatPowersWindow.current != null;
	}

	// Token: 0x06002123 RID: 8483 RVA: 0x0012D0B8 File Offset: 0x0012B2B8
	public static UIGreatPowersWindow Create(GreatPowers great_powers, GameObject prototype, RectTransform parent)
	{
		if (prototype == null)
		{
			return null;
		}
		if (great_powers == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UIGreatPowersWindow orAddComponent = global::Common.Spawn(prototype, parent, false, "").GetOrAddComponent<UIGreatPowersWindow>();
		orAddComponent.SetObject(great_powers);
		orAddComponent.on_close = (UIWindow.OnClose)Delegate.Combine(orAddComponent.on_close, new UIWindow.OnClose(delegate(UIWindow _)
		{
			UIGreatPowersWindow.current = null;
		}));
		orAddComponent.Open();
		return orAddComponent;
	}

	// Token: 0x04001607 RID: 5639
	private static string def_id = "GreatPowersWindow";

	// Token: 0x04001608 RID: 5640
	[UIFieldTarget("id_Player")]
	private BSGButton m_Player;

	// Token: 0x04001609 RID: 5641
	[UIFieldTarget("id_PlayerKing")]
	private UICharacterIcon m_PlayerKing;

	// Token: 0x0400160A RID: 5642
	[UIFieldTarget("id_PlayerFameContainer")]
	private GameObject m_PlayerFameContainer;

	// Token: 0x0400160B RID: 5643
	[UIFieldTarget("id_PlayerFame")]
	private TextMeshProUGUI m_PlayerFame;

	// Token: 0x0400160C RID: 5644
	[UIFieldTarget("id_PlayerSelected")]
	private GameObject m_PlayerSelected;

	// Token: 0x0400160D RID: 5645
	[UIFieldTarget("id_GreathPowersLabel")]
	private TextMeshProUGUI m_GreathPowersLabel;

	// Token: 0x0400160E RID: 5646
	[UIFieldTarget("id_GreathPowersContianer")]
	private RectTransform m_GreathPowersContianer;

	// Token: 0x0400160F RID: 5647
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001610 RID: 5648
	[UIFieldTarget("id_Crest")]
	private UIKingdomIcon m_KingdomShield;

	// Token: 0x04001611 RID: 5649
	[UIFieldTarget("id_SelecteeCrest")]
	private UIKingdomIcon m_SelecteeShield;

	// Token: 0x04001612 RID: 5650
	[UIFieldTarget("id_VictoryCrest")]
	private UIKingdomIcon m_VictoryCrest;

	// Token: 0x04001613 RID: 5651
	[UIFieldTarget("id_RankingCategoriesContainer")]
	private GameObject m_CategoriesContainer;

	// Token: 0x04001614 RID: 5652
	[UIFieldTarget("id_AdditinalFameSources")]
	private GameObject m_AdditinalFameSourcesContianer;

	// Token: 0x04001615 RID: 5653
	[UIFieldTarget("id_KingdomSlotPrototype")]
	private GameObject m_KingdomSlotPrototype;

	// Token: 0x04001616 RID: 5654
	[UIFieldTarget("id_ButtonVictory")]
	private BSGButton m_ButtonVictory;

	// Token: 0x04001617 RID: 5655
	[UIFieldTarget("id_LabelVictory")]
	private TextMeshProUGUI m_LabelVictory;

	// Token: 0x04001618 RID: 5656
	[UIFieldTarget("id_LabelVictoryProgress")]
	private TextMeshProUGUI m_LabelVictoryProgress;

	// Token: 0x04001619 RID: 5657
	[UIFieldTarget("id_Button_Close")]
	private BSGButton m_ButtonClose;

	// Token: 0x0400161A RID: 5658
	[SerializeField]
	private float m_FirstTierScale = 0.8f;

	// Token: 0x0400161B RID: 5659
	[SerializeField]
	private float m_SecoundTierScale = 0.72f;

	// Token: 0x0400161C RID: 5660
	[SerializeField]
	public float m_RowSpace = 26f;

	// Token: 0x0400161F RID: 5663
	public static UIGreatPowersWindow instance = null;

	// Token: 0x04001620 RID: 5664
	private Logic.Kingdom selectedKingdom;

	// Token: 0x04001621 RID: 5665
	private List<UIGreatPowersWindow.KingdomSlot> m_TopRankingKingdoms = new List<UIGreatPowersWindow.KingdomSlot>();

	// Token: 0x04001622 RID: 5666
	private UIGreatPowersWindow.AdditnalFameSources m_additnalFameSources;

	// Token: 0x04001623 RID: 5667
	private Vars tooltipVars;

	// Token: 0x04001624 RID: 5668
	private DT.Field windowDef;

	// Token: 0x04001625 RID: 5669
	private Material clamLabelMaterialNormal;

	// Token: 0x04001626 RID: 5670
	private Material clamLabelMaterialDisabled;

	// Token: 0x04001627 RID: 5671
	private float m_LastFame;

	// Token: 0x04001628 RID: 5672
	private UIKingdomRankingCategory[] m_CategorySlots;

	// Token: 0x04001629 RID: 5673
	private static UIGreatPowersWindow current;

	// Token: 0x02000770 RID: 1904
	internal class KingdomSlot : MonoBehaviour
	{
		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x06004BFC RID: 19452 RVA: 0x00228061 File Offset: 0x00226261
		// (set) Token: 0x06004BFD RID: 19453 RVA: 0x00228069 File Offset: 0x00226269
		public Logic.Kingdom Kingdom { get; private set; }

		// Token: 0x06004BFE RID: 19454 RVA: 0x00228074 File Offset: 0x00226274
		private void Init()
		{
			if (this.m_Initalized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_Icon != null)
			{
				KingdomShield primary = this.m_Icon.GetPrimary();
				if (primary != null)
				{
					KingdomShield kingdomShield = primary;
					kingdomShield.onClick = (KingdomShield.OnShieldClick)Delegate.Combine(kingdomShield.onClick, new KingdomShield.OnShieldClick(this.HandleShieldClick));
				}
			}
			if (this.m_Selected != null)
			{
				this.m_Selected.gameObject.SetActive(false);
			}
			if (this.m_Icon != null)
			{
				this.m_Icon.gameObject.SetActive(false);
			}
			this.m_TooltipVars = new Vars();
			this.m_Initalized = true;
		}

		// Token: 0x06004BFF RID: 19455 RVA: 0x00228126 File Offset: 0x00226326
		private bool HandleShieldClick(PointerEventData e, KingdomShield s)
		{
			this.OnSelect(this);
			return true;
		}

		// Token: 0x06004C00 RID: 19456 RVA: 0x00228138 File Offset: 0x00226338
		public void SetData(Logic.Kingdom k)
		{
			this.Init();
			this.Kingdom = k;
			this.m_TooltipVars.obj = this.Kingdom;
			this.m_TooltipVars.Set<bool>("is_player", BaseUI.LogicKingdom() == k);
			this.m_TooltipVars.Set<bool>("hide_sources", this.m_Focused);
			if (this.m_FameContainer != null)
			{
				this.m_FameContainer.SetActive(this.Kingdom != null);
			}
			if (this.m_FameContainer != null)
			{
				Tooltip.Get(this.m_FameContainer.gameObject, true).SetDef("FameTooltip", this.m_TooltipVars);
			}
			this.Refresh();
		}

		// Token: 0x06004C01 RID: 19457 RVA: 0x002281F0 File Offset: 0x002263F0
		private void Refresh()
		{
			this.m_Empty.gameObject.SetActive(this.Kingdom == null);
			this.m_Icon.SetObject(this.Kingdom, null);
			this.m_Icon.gameObject.SetActive(this.Kingdom != null);
			this.UpdateHighlight();
			this.UpdateFame();
			this.UpdateTooltip();
		}

		// Token: 0x06004C02 RID: 19458 RVA: 0x00228254 File Offset: 0x00226454
		private void UpdateFame()
		{
			if (this.m_Fame != null && this.Kingdom != null)
			{
				this.m_Fame.text = this.Kingdom.fame.ToString();
			}
		}

		// Token: 0x06004C03 RID: 19459 RVA: 0x00228295 File Offset: 0x00226495
		public void SetFocused(bool focused)
		{
			this.m_Focused = focused;
			this.m_TooltipVars.Set<bool>("hide_sources", this.m_Focused);
			this.UpdateHighlight();
		}

		// Token: 0x06004C04 RID: 19460 RVA: 0x002282BC File Offset: 0x002264BC
		private void UpdateHighlight()
		{
			if (this.m_Selected != null)
			{
				this.m_Selected.gameObject.SetActive(this.m_Focused);
			}
			if (this.m_SelectedLine != null)
			{
				this.m_SelectedLine.gameObject.SetActive(this.m_Focused);
			}
			if (this.m_SlantBackground != null)
			{
				Logic.Kingdom kingdom = BaseUI.LogicKingdom();
				Logic.Kingdom kingdom2 = this.Kingdom;
				List<Logic.Kingdom> list;
				if (kingdom2 == null)
				{
					list = null;
				}
				else
				{
					Game game = kingdom2.game;
					if (game == null)
					{
						list = null;
					}
					else
					{
						GreatPowers great_powers = game.great_powers;
						list = ((great_powers != null) ? great_powers.TopKingdoms(false) : null);
					}
				}
				List<Logic.Kingdom> list2 = list;
				bool flag = list2 != null && list2.Contains(kingdom);
				this.m_SlantBackground.gameObject.SetActive(flag && this.Kingdom != kingdom);
				if (flag)
				{
					EmperorOfTheWorld emperorOfTheWorld = this.Kingdom.game.emperorOfTheWorld;
					EmperorOfTheWorld.Slant slant = emperorOfTheWorld.CalcSlant(this.Kingdom, kingdom, -1);
					this.m_SlantBackground.overrideSprite = global::Defs.GetObj<Sprite>(emperorOfTheWorld.def.field, "SlantBackgrounds.GreatPowers." + slant.ToString(), null);
				}
			}
		}

		// Token: 0x06004C05 RID: 19461 RVA: 0x002283DC File Offset: 0x002265DC
		private void UpdateTooltip()
		{
			Logic.Kingdom kingdom = this.Kingdom;
			List<Logic.Kingdom> list;
			if (kingdom == null)
			{
				list = null;
			}
			else
			{
				Game game = kingdom.game;
				if (game == null)
				{
					list = null;
				}
				else
				{
					GreatPowers great_powers = game.great_powers;
					list = ((great_powers != null) ? great_powers.TopKingdoms(false) : null);
				}
			}
			List<Logic.Kingdom> list2 = list;
			Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
			if (this.Kingdom != null && this.m_IconPrimary != null && this.Kingdom != kingdom2 && list2 != null && list2.Contains(kingdom2))
			{
				Vars vars = new Vars();
				vars.Set<Logic.Kingdom>("voter", this.Kingdom);
				vars.Set<Logic.Kingdom>("candidate", kingdom2);
				vars.Set<float>("vote_weight", this.Kingdom.game.emperorOfTheWorld.CalcVoteWeightBase(this.Kingdom, kingdom2));
				vars.Set<string>("slant_text", UIEmperorOfTheWorldWindow.GetSlantText(this.Kingdom, kingdom2, "GreatPowerEmperorVoteTooltip.SlantTexts"));
				vars.Set<string>("pro_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.Kingdom, kingdom2, "reason_to_them", true));
				vars.Set<string>("con_texts", UIEmperorOfTheWorldWindow.GetProConTooltipText(this.Kingdom, kingdom2, "reason_to_them", false));
				Tooltip.Get(this.m_IconPrimary, true).SetDef("GreatPowerEmperorVoteTooltip", vars);
			}
		}

		// Token: 0x04003A8D RID: 14989
		[UIFieldTarget("id_KIngdomCrest")]
		private UIKingdomIcon m_Icon;

		// Token: 0x04003A8E RID: 14990
		[UIFieldTarget("id_Primary")]
		private GameObject m_IconPrimary;

		// Token: 0x04003A8F RID: 14991
		[UIFieldTarget("id_Empty")]
		private GameObject m_Empty;

		// Token: 0x04003A90 RID: 14992
		[UIFieldTarget("id_Selected")]
		private GameObject m_Selected;

		// Token: 0x04003A91 RID: 14993
		[UIFieldTarget("id_FameContainer")]
		private GameObject m_FameContainer;

		// Token: 0x04003A92 RID: 14994
		[UIFieldTarget("id_SelectedLine")]
		private GameObject m_SelectedLine;

		// Token: 0x04003A93 RID: 14995
		[UIFieldTarget("id_SlantBackground")]
		private Image m_SlantBackground;

		// Token: 0x04003A94 RID: 14996
		[UIFieldTarget("id_Fame")]
		private TextMeshProUGUI m_Fame;

		// Token: 0x04003A96 RID: 14998
		public Action<UIGreatPowersWindow.KingdomSlot> OnSelect;

		// Token: 0x04003A97 RID: 14999
		private bool m_Initalized;

		// Token: 0x04003A98 RID: 15000
		private bool m_Focused;

		// Token: 0x04003A99 RID: 15001
		private Vars m_TooltipVars;
	}

	// Token: 0x02000771 RID: 1905
	internal class AdditnalFameSources : MonoBehaviour
	{
		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x06004C07 RID: 19463 RVA: 0x0022850C File Offset: 0x0022670C
		// (set) Token: 0x06004C08 RID: 19464 RVA: 0x00228514 File Offset: 0x00226714
		public Logic.Kingdom Kingdom { get; private set; }

		// Token: 0x06004C09 RID: 19465 RVA: 0x0022851D File Offset: 0x0022671D
		private void Init()
		{
			if (this.m_Initalized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_FameSourcePrototype != null)
			{
				this.m_FameSourcePrototype.gameObject.SetActive(false);
			}
			this.m_Initalized = true;
		}

		// Token: 0x06004C0A RID: 19466 RVA: 0x00228555 File Offset: 0x00226755
		public void SetData(Logic.Kingdom k, DT.Field def)
		{
			this.Init();
			this.m_WindowDef = def;
			this.Kingdom = k;
			UIText.SetTextKey(this.m_Caption, "KingdomAdvantage.other_fame_soruces_label", null, null);
			this.AllocateSources();
			this.UpdateSources();
		}

		// Token: 0x06004C0B RID: 19467 RVA: 0x00228589 File Offset: 0x00226789
		private void Update()
		{
			if (this.lastRefreshTime + this.refreshInterval < UnityEngine.Time.time)
			{
				this.UpdateSources();
				this.lastRefreshTime = UnityEngine.Time.time;
			}
		}

		// Token: 0x06004C0C RID: 19468 RVA: 0x002285B0 File Offset: 0x002267B0
		private void AllocateSources()
		{
			for (int i = 0; i < this.m_Rows.Count; i++)
			{
				global::Common.DestroyObj(this.m_Rows[i].gameObject);
			}
			this.m_Rows.Clear();
			if (this.m_WindowDef == null)
			{
				return;
			}
			if (this.m_FameSourcePrototype == null)
			{
				return;
			}
			if (this.m_SourcesContainer == null)
			{
				return;
			}
			UICommon.DeleteActiveChildren(this.m_SourcesContainer.transform);
			DT.Field field = this.m_WindowDef.FindChild("fame_sources", null, true, true, true, '.');
			List<DT.Field> list = (field != null) ? field.Children() : null;
			if (list == null)
			{
				return;
			}
			for (int j = 0; j < list.Count; j++)
			{
				DT.Field field2 = list[j];
				if (field2 != null && !(field2.key == string.Empty))
				{
					UIGreatPowersWindow.AdditnalFameSources.FameSourceRow fameSourceRow = new UIGreatPowersWindow.AdditnalFameSources.FameSourceRow();
					fameSourceRow.gameObject = global::Common.Spawn(this.m_FameSourcePrototype, this.m_SourcesContainer.transform, false, "");
					fameSourceRow.def = field2;
					fameSourceRow.kingdom = this.Kingdom;
					fameSourceRow.UpdateName();
					this.m_Rows.Add(fameSourceRow);
				}
			}
		}

		// Token: 0x06004C0D RID: 19469 RVA: 0x002286D4 File Offset: 0x002268D4
		private void UpdateSources()
		{
			float num = 0f;
			for (int i = 0; i < this.m_Rows.Count; i++)
			{
				UIGreatPowersWindow.AdditnalFameSources.FameSourceRow fameSourceRow = this.m_Rows[i];
				float num2 = fameSourceRow.UpdateValue();
				fameSourceRow.gameObject.SetActive(num2 != 0f);
				num += num2;
			}
			if (this.m_TotalFame != null)
			{
				this.m_TotalFame.text = num.ToString();
			}
		}

		// Token: 0x04003A9A RID: 15002
		[UIFieldTarget("id_Caption")]
		private TextMeshProUGUI m_Caption;

		// Token: 0x04003A9B RID: 15003
		[UIFieldTarget("id_TotalFame")]
		private TextMeshProUGUI m_TotalFame;

		// Token: 0x04003A9C RID: 15004
		[UIFieldTarget("id_FameSourcePrototype")]
		private GameObject m_FameSourcePrototype;

		// Token: 0x04003A9D RID: 15005
		[UIFieldTarget("id_SourcesContainer")]
		private GameObject m_SourcesContainer;

		// Token: 0x04003A9F RID: 15007
		private DT.Field m_WindowDef;

		// Token: 0x04003AA0 RID: 15008
		private List<UIGreatPowersWindow.AdditnalFameSources.FameSourceRow> m_Rows = new List<UIGreatPowersWindow.AdditnalFameSources.FameSourceRow>();

		// Token: 0x04003AA1 RID: 15009
		private bool m_Initalized;

		// Token: 0x04003AA2 RID: 15010
		private float refreshInterval = 0.5f;

		// Token: 0x04003AA3 RID: 15011
		private float lastRefreshTime;

		// Token: 0x02000A18 RID: 2584
		private class FameSourceRow
		{
			// Token: 0x06005579 RID: 21881 RVA: 0x002498C0 File Offset: 0x00247AC0
			public void UpdateName()
			{
				if (this.def == null)
				{
					return;
				}
				if (this.gameObject == null)
				{
					return;
				}
				TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(this.gameObject, "id_SourceName");
				if (textMeshProUGUI == null)
				{
					return;
				}
				UIText.SetText(textMeshProUGUI, global::Defs.Localize(this.def, "name", null, null, true, true));
			}

			// Token: 0x0600557A RID: 21882 RVA: 0x0024991C File Offset: 0x00247B1C
			public float UpdateValue()
			{
				if (this.def == null)
				{
					return 0f;
				}
				if (this.gameObject == null)
				{
					return 0f;
				}
				TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(this.gameObject, "id_SourceValue");
				if (textMeshProUGUI == null)
				{
					return 0f;
				}
				float @float = this.def.GetFloat("bonus_value", this.kingdom, 0f, true, true, true, '.');
				UIText.SetText(textMeshProUGUI, "+" + @float.ToString());
				return @float;
			}

			// Token: 0x04004679 RID: 18041
			public GameObject gameObject;

			// Token: 0x0400467A RID: 18042
			public DT.Field def;

			// Token: 0x0400467B RID: 18043
			public Logic.Kingdom kingdom;
		}
	}
}

using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002CD RID: 717
public class UICampaignKingdomBrowser : UIWindow, RemoteVars.IListener
{
	// Token: 0x06002CE7 RID: 11495 RVA: 0x0017590F File Offset: 0x00173B0F
	public override string GetDefId()
	{
		return UICampaignKingdomBrowser.def_id;
	}

	// Token: 0x06002CE8 RID: 11496 RVA: 0x00175918 File Offset: 0x00173B18
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.KingdomNameplatePrototype != null)
		{
			this.KingdomNameplatePrototype.gameObject.SetActive(false);
		}
		if (this.m_CloseKingdomBrowser != null)
		{
			this.m_CloseKingdomBrowser.onClick = new BSGButton.OnClick(this.HandelOnClose);
		}
		this.m_Initialized = true;
	}

	// Token: 0x06002CE9 RID: 11497 RVA: 0x00175980 File Offset: 0x00173B80
	public void Open(Campaign campaign, Action<Logic.Kingdom> onSelectCallBack, Func<Logic.Kingdom, bool> validateConditon)
	{
		this.Init();
		Campaign campaign2 = this.m_Campaign;
		if (campaign2 != null)
		{
			campaign2.DelVarsListener(this);
		}
		this.m_Campaign = campaign;
		Campaign campaign3 = this.m_Campaign;
		if (campaign3 != null)
		{
			campaign3.AddVarsListener(this);
		}
		this.onSelect = onSelectCallBack;
		this.validateMemberCondition = validateConditon;
		base.gameObject.SetActive(true);
		this.Show();
		this.Populate();
		this.LocalizeStaticTexts();
	}

	// Token: 0x06002CEA RID: 11498 RVA: 0x001759EC File Offset: 0x00173BEC
	public void UpdateMode()
	{
		this.mode = UICampaignKingdomBrowser.Mode.Kingdom;
		if (this.m_Campaign == null)
		{
			return;
		}
		if (this.m_Campaign.GetVar("pick_kingdom", null, true) == "province")
		{
			this.mode = UICampaignKingdomBrowser.Mode.Realm;
		}
		this.LocalizeStaticTexts();
	}

	// Token: 0x06002CEB RID: 11499 RVA: 0x00175A3C File Offset: 0x00173C3C
	private void Populate()
	{
		if (this.m_KingdomNamesContainer == null)
		{
			return;
		}
		if (this.KingdomNameplatePrototype == null)
		{
			return;
		}
		this.UpdateMode();
		UICommon.DeleteActiveChildren(this.m_KingdomNamesContainer.transform);
		this.m_Labels.Clear();
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		List<Logic.Kingdom> list = new List<Logic.Kingdom>();
		if (this.mode == UICampaignKingdomBrowser.Mode.Kingdom)
		{
			CampaignUtils.GetKingdoms(game, list);
		}
		else
		{
			CampaignUtils.GetRealmKingdoms(game, list);
		}
		list.Sort((Logic.Kingdom x, Logic.Kingdom y) => x.Name.CompareTo(y.Name));
		int constraintCount = this.m_KingdomNamesContainer.constraintCount;
		for (int i = 0; i < list.Count; i++)
		{
			UICampaignKingdomBrowser.KingdomLabel kingdomLabel = this.AddKingdom(list[i], this.m_KingdomNamesContainer.transform as RectTransform, this.validateMemberCondition);
			bool shown = i / constraintCount % 2 == 0;
			kingdomLabel.ShowBackground(shown);
		}
		if (this.m_ScrollView != null && this.m_ScrollView.verticalScrollbar != null)
		{
			this.m_ScrollView.verticalScrollbar.value = 1f;
		}
	}

	// Token: 0x06002CEC RID: 11500 RVA: 0x00175B60 File Offset: 0x00173D60
	private void RefreshLabels()
	{
		for (int i = 0; i < this.m_Labels.Count; i++)
		{
			this.m_Labels[i].UpdateTeam();
		}
	}

	// Token: 0x06002CED RID: 11501 RVA: 0x00175B94 File Offset: 0x00173D94
	private void LocalizeStaticTexts()
	{
		if (this.m_Caption != null)
		{
			string key = UICampaignKingdomBrowser.def_id + "." + ((this.mode == UICampaignKingdomBrowser.Mode.Kingdom) ? "kingdom_list" : "province_list");
			UIText.SetTextKey(this.m_Caption, key, null, null);
		}
	}

	// Token: 0x06002CEE RID: 11502 RVA: 0x00175BE4 File Offset: 0x00173DE4
	private void Select(Logic.Kingdom k)
	{
		this.selected = k;
		for (int i = 0; i < this.m_Labels.Count; i++)
		{
			UICampaignKingdomBrowser.KingdomLabel kingdomLabel = this.m_Labels[i];
			kingdomLabel.Select(kingdomLabel.Data == k);
		}
	}

	// Token: 0x06002CEF RID: 11503 RVA: 0x00175C28 File Offset: 0x00173E28
	private UICampaignKingdomBrowser.KingdomLabel AddKingdom(Logic.Kingdom kingdom, RectTransform parent, Func<Logic.Kingdom, bool> validateConditon)
	{
		UICampaignKingdomBrowser.KingdomLabel kingdomLabel = UICampaignKingdomBrowser.KingdomLabel.Create(this.m_Campaign, kingdom, this.KingdomNameplatePrototype, parent, validateConditon);
		UICampaignKingdomBrowser.KingdomLabel kingdomLabel2 = kingdomLabel;
		kingdomLabel2.OnSelect = (Action<UICampaignKingdomBrowser.KingdomLabel>)Delegate.Combine(kingdomLabel2.OnSelect, new Action<UICampaignKingdomBrowser.KingdomLabel>(this.HandleKingdomSelected));
		this.m_Labels.Add(kingdomLabel);
		return kingdomLabel;
	}

	// Token: 0x06002CF0 RID: 11504 RVA: 0x00175C79 File Offset: 0x00173E79
	protected void LateUpdate()
	{
		if (this.m_InvalidateLabels)
		{
			this.RefreshLabels();
			this.m_InvalidateLabels = false;
		}
		if (this.m_InvalidateKingdoms)
		{
			this.Populate();
			this.m_InvalidateKingdoms = false;
		}
	}

	// Token: 0x06002CF1 RID: 11505 RVA: 0x00175CA5 File Offset: 0x00173EA5
	private void HandleKingdomSelected(UICampaignKingdomBrowser.KingdomLabel label)
	{
		this.Select(label.Data);
		if (this.onSelect != null)
		{
			this.onSelect(this.selected);
		}
	}

	// Token: 0x06002CF2 RID: 11506 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	public void HandelOnClose(BSGButton b)
	{
		this.Close(false);
	}

	// Token: 0x06002CF3 RID: 11507 RVA: 0x00175CCC File Offset: 0x00173ECC
	private void HandleSelectedAndConfirm(UICampaignKingdomBrowser.KingdomLabel label)
	{
		this.Select(label.Data);
		if (this.onSelect != null)
		{
			this.onSelect(this.selected);
		}
		this.HandelOnClose(null);
	}

	// Token: 0x06002CF4 RID: 11508 RVA: 0x00175CFA File Offset: 0x00173EFA
	private void HandleOnCancelSelection(BSGButton b)
	{
		UserInteractionLogger.LogNewLine(b, null);
		this.HandelOnClose(null);
	}

	// Token: 0x06002CF5 RID: 11509 RVA: 0x00175D0C File Offset: 0x00173F0C
	public void OnVarChanged(RemoteVars vars, string key, Value old_val, Value new_val)
	{
		if (key == "kingdom_name" || key == "team" || key == "team_size")
		{
			this.m_InvalidateLabels = true;
			return;
		}
		if (!(key == "start_period") && !(key == "pick_kingdom"))
		{
			return;
		}
		this.m_InvalidateKingdoms = true;
	}

	// Token: 0x06002CF6 RID: 11510 RVA: 0x00175D6A File Offset: 0x00173F6A
	public override void Close(bool silent = false)
	{
		this.Clean();
		if (this.on_close != null)
		{
			this.on_close(this);
		}
		this.Hide(silent);
		if (base.gameObject != null)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002CF7 RID: 11511 RVA: 0x00175DA7 File Offset: 0x00173FA7
	private void Clean()
	{
		Campaign campaign = this.m_Campaign;
		if (campaign != null)
		{
			campaign.DelVarsListener(this);
		}
		this.m_Campaign = null;
	}

	// Token: 0x04001EA2 RID: 7842
	private static string def_id = "CampaignsKingdomBrowserWindow";

	// Token: 0x04001EA3 RID: 7843
	[UIFieldTarget("id_Caption")]
	private TMP_Text m_Caption;

	// Token: 0x04001EA4 RID: 7844
	[UIFieldTarget("id_KingdomNamesContainer")]
	private GridLayoutGroup m_KingdomNamesContainer;

	// Token: 0x04001EA5 RID: 7845
	[UIFieldTarget("id_KingdomNameplatePrototype")]
	private GameObject KingdomNameplatePrototype;

	// Token: 0x04001EA6 RID: 7846
	[UIFieldTarget("id_CloseKingdomBrowser")]
	private BSGButton m_CloseKingdomBrowser;

	// Token: 0x04001EA7 RID: 7847
	[UIFieldTarget("id_Ok")]
	private BSGButton m_Ok;

	// Token: 0x04001EA8 RID: 7848
	[UIFieldTarget("id_Cancel")]
	private BSGButton m_Cancel;

	// Token: 0x04001EA9 RID: 7849
	[UIFieldTarget("id_ScrollView")]
	private ScrollRect m_ScrollView;

	// Token: 0x04001EAA RID: 7850
	private UICampaignKingdomBrowser.Mode mode;

	// Token: 0x04001EAB RID: 7851
	private Logic.Kingdom selected;

	// Token: 0x04001EAC RID: 7852
	private Campaign m_Campaign;

	// Token: 0x04001EAD RID: 7853
	private List<UICampaignKingdomBrowser.KingdomLabel> m_Labels = new List<UICampaignKingdomBrowser.KingdomLabel>();

	// Token: 0x04001EAE RID: 7854
	private Action<Logic.Kingdom> onSelect;

	// Token: 0x04001EAF RID: 7855
	private Func<Logic.Kingdom, bool> validateMemberCondition;

	// Token: 0x04001EB0 RID: 7856
	private bool m_InvalidateLabels;

	// Token: 0x04001EB1 RID: 7857
	private bool m_InvalidateKingdoms;

	// Token: 0x02000828 RID: 2088
	public enum Mode
	{
		// Token: 0x04003E0C RID: 15884
		Kingdom,
		// Token: 0x04003E0D RID: 15885
		Realm
	}

	// Token: 0x02000829 RID: 2089
	internal class KingdomLabel : Hotspot
	{
		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x06005007 RID: 20487 RVA: 0x00237FE6 File Offset: 0x002361E6
		// (set) Token: 0x06005008 RID: 20488 RVA: 0x00237FEE File Offset: 0x002361EE
		public Logic.Kingdom Data { get; private set; }

		// Token: 0x06005009 RID: 20489 RVA: 0x00237FF7 File Offset: 0x002361F7
		private void Init()
		{
			if (this.m_Initiazlied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.def = global::Defs.GetDefField("CampaignsKingdomBrowserWindow", null);
			this.m_Initiazlied = true;
		}

		// Token: 0x0600500A RID: 20490 RVA: 0x00238021 File Offset: 0x00236221
		public void SetKigdom(Campaign campaign, Logic.Kingdom kingdom, Func<Logic.Kingdom, bool> validateMethod)
		{
			this.m_Campaign = campaign;
			this.validateConditons = validateMethod;
			this.Init();
			this.Data = kingdom;
			this.Refresh();
		}

		// Token: 0x0600500B RID: 20491 RVA: 0x00238044 File Offset: 0x00236244
		public void ShowBackground(bool shown)
		{
			this.m_ShowBackground = shown;
			this.UpdateHighlight();
		}

		// Token: 0x0600500C RID: 20492 RVA: 0x00238054 File Offset: 0x00236254
		public void UpdateTeam()
		{
			int kingdomsOwningPlayerIndex = CampaignUtils.GetKingdomsOwningPlayerIndex(this.m_Campaign, this.Data);
			if (kingdomsOwningPlayerIndex >= 0)
			{
				if (this.m_Team != null)
				{
					this.m_Team.gameObject.SetActive(true);
				}
				int team = CampaignUtils.GetTeam(this.m_Campaign, kingdomsOwningPlayerIndex);
				string text = (team >= 0) ? UICommon.IntToRomanNumber(team + 1) : "";
				if (this.m_TeamIndex)
				{
					bool flag = CampaignUtils.IsAutoTeams(this.m_Campaign);
					this.m_TeamIndex.gameObject.SetActive(!flag);
					if (!flag)
					{
						UIText.SetText(this.m_TeamIndex, text);
					}
				}
				if (this.m_TeamBackground != null)
				{
					string key = "team.team_" + (team + 1);
					this.m_TeamBackground.overrideSprite = global::Defs.GetObj<Sprite>("LobbyPlayerWindow", key, null);
				}
				if (this.m_TeamBanner != null)
				{
					string key2 = "team_banner.team_" + (team + 1);
					this.m_TeamBanner.overrideSprite = global::Defs.GetObj<Sprite>("LobbyPlayerWindow", key2, null);
				}
			}
			else if (this.m_Team != null)
			{
				this.m_Team.gameObject.SetActive(false);
			}
			if (this.m_SinglePlayerIcon != null)
			{
				this.m_SinglePlayerIcon.gameObject.SetActive(!this.m_Campaign.IsMultiplayerCampaign());
			}
		}

		// Token: 0x0600500D RID: 20493 RVA: 0x002381B7 File Offset: 0x002363B7
		private void Update()
		{
			this.UpdateHighlight();
		}

		// Token: 0x0600500E RID: 20494 RVA: 0x002381C0 File Offset: 0x002363C0
		private void Refresh()
		{
			if (this.Data == null)
			{
				return;
			}
			if (this.m_KingdomName != null)
			{
				UIText.SetTextKey(this.m_KingdomName, "TitleScreen.SinglePlayer.Kingdom.name_only", this.Data, null);
			}
			if (this.m_KingdomSize != null)
			{
				this.m_KingdomSize.text = this.Data.realms.Count.ToString();
			}
			if (this.m_KingdomIcon != null)
			{
				this.m_KingdomIcon.SetObject(this.Data, null);
			}
			if (this.m_ReligionIcon != null && this.Data.religion != null)
			{
				this.m_ReligionIcon.sprite = global::Defs.GetObj<Sprite>(this.Data.religion.def.field, global::Religions.GetRelgionIconKey(this.Data), null);
			}
			this.UpdateTeam();
			this.UpdateHighlight();
		}

		// Token: 0x0600500F RID: 20495 RVA: 0x002382A4 File Offset: 0x002364A4
		public void Select(bool selected)
		{
			if (this.m_Selected == selected)
			{
				return;
			}
			this.m_Selected = selected;
			this.UpdateHighlight();
		}

		// Token: 0x06005010 RID: 20496 RVA: 0x002382C0 File Offset: 0x002364C0
		public void UpdateHighlight()
		{
			Image background = this.m_Background;
			if (background != null)
			{
				background.gameObject.SetActive(this.m_ShowBackground && !this.m_Selected && !this.mouse_in);
			}
			Image backgroundHover = this.m_BackgroundHover;
			if (backgroundHover != null)
			{
				backgroundHover.gameObject.SetActive(!this.m_Selected && this.mouse_in);
			}
			Image backgroundSelected = this.m_BackgroundSelected;
			if (backgroundSelected != null)
			{
				backgroundSelected.gameObject.SetActive(this.m_Selected);
			}
			bool flag = this.IsBalckListed();
			if (this.m_Selected)
			{
				Image background_Blacklisted = this.m_Background_Blacklisted;
				if (background_Blacklisted != null)
				{
					background_Blacklisted.gameObject.SetActive(false);
				}
			}
			else
			{
				Image background_Blacklisted2 = this.m_Background_Blacklisted;
				if (background_Blacklisted2 != null)
				{
					background_Blacklisted2.gameObject.SetActive(flag);
				}
			}
			if (this.m_KingdomName != null && this.def != null)
			{
				this.m_KingdomName.color = global::Defs.GetColor(this.def, flag ? "kingdom_name_blacklisted" : "kingdom_name_eligable", null);
			}
		}

		// Token: 0x06005011 RID: 20497 RVA: 0x002383BD File Offset: 0x002365BD
		private bool IsBalckListed()
		{
			return this.validateConditons != null && !this.validateConditons(this.Data);
		}

		// Token: 0x06005012 RID: 20498 RVA: 0x002383DD File Offset: 0x002365DD
		public override void OnClick(PointerEventData e)
		{
			this.UpdateHighlight();
			if (this.OnSelect != null)
			{
				this.OnSelect(this);
			}
		}

		// Token: 0x06005013 RID: 20499 RVA: 0x002383F9 File Offset: 0x002365F9
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			this.UpdateHighlight();
		}

		// Token: 0x06005014 RID: 20500 RVA: 0x00238408 File Offset: 0x00236608
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this.UpdateHighlight();
		}

		// Token: 0x06005015 RID: 20501 RVA: 0x00238418 File Offset: 0x00236618
		public static UICampaignKingdomBrowser.KingdomLabel Create(Campaign campaign, Logic.Kingdom kingdom, GameObject prototype, RectTransform parent, Func<Logic.Kingdom, bool> validateConditon)
		{
			if (kingdom == null)
			{
				return null;
			}
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, parent, false);
			UICampaignKingdomBrowser.KingdomLabel kingdomLabel = gameObject.AddComponent<UICampaignKingdomBrowser.KingdomLabel>();
			kingdomLabel.SetKigdom(campaign, kingdom, validateConditon);
			gameObject.SetActive(true);
			return kingdomLabel;
		}

		// Token: 0x04003E0E RID: 15886
		[UIFieldTarget("id_KingdomName")]
		private TMP_Text m_KingdomName;

		// Token: 0x04003E0F RID: 15887
		[UIFieldTarget("id_KingdomSize")]
		private TMP_Text m_KingdomSize;

		// Token: 0x04003E10 RID: 15888
		[UIFieldTarget("id_KingdomIcon")]
		private UIKingdomIcon m_KingdomIcon;

		// Token: 0x04003E11 RID: 15889
		[UIFieldTarget("id_ReligionIcon")]
		private Image m_ReligionIcon;

		// Token: 0x04003E12 RID: 15890
		[UIFieldTarget("id_Background")]
		private Image m_Background;

		// Token: 0x04003E13 RID: 15891
		[UIFieldTarget("id_Background_Hover")]
		private Image m_BackgroundHover;

		// Token: 0x04003E14 RID: 15892
		[UIFieldTarget("id_Background_Selected")]
		private Image m_BackgroundSelected;

		// Token: 0x04003E15 RID: 15893
		[UIFieldTarget("id_Background_Blacklisted")]
		private Image m_Background_Blacklisted;

		// Token: 0x04003E16 RID: 15894
		[UIFieldTarget("id_Team")]
		private RectTransform m_Team;

		// Token: 0x04003E17 RID: 15895
		[UIFieldTarget("id_TeamBanner")]
		private Image m_TeamBanner;

		// Token: 0x04003E18 RID: 15896
		[UIFieldTarget("id_TeamBackground")]
		private Image m_TeamBackground;

		// Token: 0x04003E19 RID: 15897
		[UIFieldTarget("id_TeamIndex")]
		private TMP_Text m_TeamIndex;

		// Token: 0x04003E1A RID: 15898
		[UIFieldTarget("id_SinglePlayerIcon")]
		private Image m_SinglePlayerIcon;

		// Token: 0x04003E1C RID: 15900
		public Action<UICampaignKingdomBrowser.KingdomLabel> OnSelect;

		// Token: 0x04003E1D RID: 15901
		private Campaign m_Campaign;

		// Token: 0x04003E1E RID: 15902
		private DT.Field def;

		// Token: 0x04003E1F RID: 15903
		private bool m_Initiazlied;

		// Token: 0x04003E20 RID: 15904
		private bool m_Selected;

		// Token: 0x04003E21 RID: 15905
		private bool m_ShowBackground = true;

		// Token: 0x04003E22 RID: 15906
		private Func<Logic.Kingdom, bool> validateConditons;
	}
}

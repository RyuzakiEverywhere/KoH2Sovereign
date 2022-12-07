using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000215 RID: 533
public class UIKingdomWindow : ObjectWindow
{
	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x0600204D RID: 8269 RVA: 0x0012864B File Offset: 0x0012684B
	// (set) Token: 0x0600204E RID: 8270 RVA: 0x00128653 File Offset: 0x00126853
	public global::Kingdom SelectedKingdom { get; private set; }

	// Token: 0x0600204F RID: 8271 RVA: 0x0012865C File Offset: 0x0012685C
	private void Start()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x06002050 RID: 8272 RVA: 0x00128664 File Offset: 0x00126864
	private void OnEnable()
	{
		if (this.m_Initialzied)
		{
			this.ExtractLogicObject();
		}
	}

	// Token: 0x06002051 RID: 8273 RVA: 0x00128674 File Offset: 0x00126874
	private void ExtractLogicObject()
	{
		global::Kingdom selected_kingdom = WorldUI.Get().selected_kingdom;
		if (selected_kingdom != null && selected_kingdom.logic != null)
		{
			this.SetObject(selected_kingdom.logic, null);
		}
	}

	// Token: 0x06002052 RID: 8274 RVA: 0x001286A4 File Offset: 0x001268A4
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Audience != null)
		{
			this.m_Audience.onClick = new BSGButton.OnClick(this.InitiateAudience);
			this.m_Audience.SetAudioSet("DefaultAudioSetPaper");
		}
		if (this.m_KingomdReligion != null)
		{
			this.m_KingomdReligion.KeepAlive = true;
		}
		if (this.m_RoyalFamilyContainer != null)
		{
			this.m_RoyalFamily = this.m_RoyalFamilyContainer.AddComponent<UIKingdomWindow.RoyalFamily>();
		}
		if (this.m_CardinalsContainer != null)
		{
			this.m_Cardinals = this.m_CardinalsContainer.AddComponent<UIKingdomWindow.Cardinals>();
		}
		if (this.m_RoyalCourtContainer != null)
		{
			this.m_Court = this.m_RoyalCourtContainer.AddComponent<UIKingdomWindow.Court>();
		}
		if (this.m_PoliticsContainer != null)
		{
			this.m_Politics = this.m_PoliticsContainer.AddComponent<UIKingdomWindow.Politics>();
		}
		if (this.m_TraditionsContainer != null)
		{
			this.m_Traditions = this.m_TraditionsContainer.AddComponent<UIKingdomWindow.Traditions>();
		}
		if (this.m_TabFamily != null)
		{
			this.m_TabFamily.onClick = new BSGButton.OnClick(this.HandleOnTabFamily);
			this.m_TabFamily.AllowSelection(true);
		}
		if (this.m_TabCardinals != null)
		{
			this.m_TabCardinals.onClick = new BSGButton.OnClick(this.HandleOnTabCardinals);
			this.m_TabCardinals.AllowSelection(true);
		}
		if (this.m_TabCourt != null)
		{
			this.m_TabCourt.onClick = new BSGButton.OnClick(this.HandleOnTabCourt);
			this.m_TabCourt.AllowSelection(true);
		}
		if (this.m_TabPolitics != null)
		{
			this.m_TabPolitics.onClick = new BSGButton.OnClick(this.HandleOnTabPolitics);
			this.m_TabPolitics.AllowSelection(true);
		}
		if (this.m_TabTraditions != null)
		{
			this.m_TabTraditions.onClick = new BSGButton.OnClick(this.HandleOnTabTraditions);
			this.m_TabTraditions.AllowSelection(true);
		}
		this.LocalizeStaticLabels();
		this.m_Initialzied = true;
	}

	// Token: 0x06002053 RID: 8275 RVA: 0x001288B0 File Offset: 0x00126AB0
	private void LocalizeStaticLabels()
	{
		if (this.m_AudienceLabel != null)
		{
			UIText.SetTextKey(this.m_AudienceLabel, "KingdomWindow.Audience", null, null);
		}
		if (this.m_TabCardinalsLabel != null)
		{
			UIText.SetTextKey(this.m_TabCardinalsLabel, "KingdomWindow.cardinals", null, null);
		}
		if (this.m_TabFamilyLabel != null)
		{
			UIText.SetTextKey(this.m_TabFamilyLabel, "KingdomWindow.family", null, null);
		}
		if (this.m_TabCourtLabel != null)
		{
			UIText.SetTextKey(this.m_TabCourtLabel, "KingdomWindow.court", null, null);
		}
		if (this.m_TabTraditionsLabel != null)
		{
			UIText.SetTextKey(this.m_TabTraditionsLabel, "KingdomWindow.traditions", null, null);
		}
		if (this.m_TabPoliticsLabel != null)
		{
			UIText.SetTextKey(this.m_TabPoliticsLabel, "KingdomWindow.politics", null, null);
		}
		if (this.m_OwnKingdomDescription != null)
		{
			UIText.SetTextKey(this.m_OwnKingdomDescription, "KingdomWindow.own_kingdom_description", null, null);
		}
	}

	// Token: 0x06002054 RID: 8276 RVA: 0x0012899D File Offset: 0x00126B9D
	public override void SetObject(Logic.Object obj, Vars vars = null)
	{
		this.Init();
		base.SetObject(obj, vars);
		if (obj != null)
		{
			this.SelectedKingdom = (obj.visuals as global::Kingdom);
		}
		this.Refresh();
	}

	// Token: 0x06002055 RID: 8277 RVA: 0x001289C7 File Offset: 0x00126BC7
	public void Close()
	{
		BaseUI.Get().SelectObj(null, false, true, true, true);
	}

	// Token: 0x06002056 RID: 8278 RVA: 0x001289D8 File Offset: 0x00126BD8
	public override void Refresh()
	{
		if (this.SelectedKingdom == null)
		{
			return;
		}
		if (this.m_DiplomaticInfo != null)
		{
			this.m_DiplomaticInfo.AddComponent<DiplomaticInfo>().SetKingdoms(BaseUI.LogicKingdom(), this.SelectedKingdom.logic);
		}
		if (this.m_Relations != null)
		{
			this.m_Relations.SetData(BaseUI.LogicKingdom(), this.SelectedKingdom.logic);
		}
		this.PopulateStatic();
		this.PopulateKing();
		this.UpdateTabSelections();
		if (this.m_InfuenceContianer != null)
		{
			bool flag = this.SelectedKingdom.logic != BaseUI.LogicKingdom();
			if (flag)
			{
				Tooltip.Get(this.m_InfuenceContianer, true).SetDef("InfluenceTooltip", new Vars(this.SelectedKingdom.logic));
			}
			this.m_InfuenceContianer.SetActive(flag);
		}
		if (this.m_DefeatedLabel != null)
		{
			string key = (this.SelectedKingdom.logic.defeated_by != null) ? "KingdomWindow.Defeated.defeated_by" : "KingdomWindow.Defeated.inactive";
			UIText.SetTextKey(this.m_DefeatedLabel, key, this.SelectedKingdom.logic, null);
		}
	}

	// Token: 0x06002057 RID: 8279 RVA: 0x00128AFC File Offset: 0x00126CFC
	private void PopulateStatic()
	{
		if (this.m_Shield != null)
		{
			this.m_Shield.SetObject(this.SelectedKingdom.logic, null);
		}
		if (this.m_Audience != null)
		{
			this.m_Audience.gameObject.SetActive(this.SelectedKingdom.logic != BaseUI.LogicKingdom());
		}
		if (this.m_RequestPending != null)
		{
			MessageIcon pendingOfferIcon = this.GetPendingOfferIcon();
			if (pendingOfferIcon != null)
			{
				BSGButton btn = pendingOfferIcon.GetComponent<BSGButton>();
				this.m_RequestPending.gameObject.SetActive(true);
				this.m_RequestPending.onClick = delegate(BSGButton b)
				{
					btn.onClick(btn);
				};
			}
			else
			{
				this.m_RequestPending.gameObject.SetActive(false);
			}
		}
		if (this.m_KingdomName)
		{
			UIText.SetTextKey(this.m_KingdomName, (this.SelectedKingdom == null) ? "" : "Kingdom.name_only", new Vars(this.SelectedKingdom.logic), null);
		}
		if (this.m_KingomdReligion != null)
		{
			this.m_KingomdReligion.SetData(this.logicObject as Logic.Kingdom);
		}
	}

	// Token: 0x06002058 RID: 8280 RVA: 0x00128C34 File Offset: 0x00126E34
	private void PopulateKing()
	{
		Logic.Character sovereign = this.SelectedKingdom.logic.royalFamily.Sovereign;
		if (this.m_KingIcon != null)
		{
			this.m_KingIcon.SetObject(sovereign, null);
			this.m_KingIcon.OnSelect -= this.InitiateAudience;
			this.m_KingIcon.OnSelect += this.InitiateAudience;
			this.m_KingIcon.ShowCrest(false);
			this.m_KingIcon.ShowStatus(false);
			this.m_KingIcon.ShowArmyBanner(false);
			this.m_KingIcon.ShowPrisonKingdomCrest(true);
		}
		if (this.m_KingName != null)
		{
			if (sovereign != null)
			{
				UIText.SetTextKey(this.m_KingName, "Character.title_name", new Vars(sovereign), null);
			}
			this.m_KingName.gameObject.SetActive(sovereign != null);
		}
		if (this.m_KingAge != null)
		{
			if (sovereign != null)
			{
				UIText.SetTextKey(this.m_KingAge, "Character.age." + sovereign.age.ToString(), null, null);
			}
			this.m_KingAge.gameObject.SetActive(sovereign != null);
		}
	}

	// Token: 0x06002059 RID: 8281 RVA: 0x00128D60 File Offset: 0x00126F60
	protected override void Update()
	{
		base.Update();
		if (this.m_Infuence != null)
		{
			global::Kingdom selectedKingdom = this.SelectedKingdom;
			if (((selectedKingdom != null) ? selectedKingdom.logic : null) != null)
			{
				UIText.SetText(this.m_Infuence, Mathf.RoundToInt(BaseUI.LogicKingdom().GetInfluenceIn(this.SelectedKingdom.logic)).ToString());
			}
		}
	}

	// Token: 0x0600205A RID: 8282 RVA: 0x00128DC4 File Offset: 0x00126FC4
	private void UpdateTabSelections()
	{
		bool flag = this.SelectedKingdom.logic.game.religions.catholic.hq_kingdom == this.SelectedKingdom.logic;
		if (this.m_RoyalFamily != null)
		{
			UIKingdomWindow.RoyalFamily royalFamily = this.m_RoyalFamily;
			global::Kingdom selectedKingdom = this.SelectedKingdom;
			Logic.RoyalFamily data;
			if (selectedKingdom == null)
			{
				data = null;
			}
			else
			{
				Logic.Kingdom logic = selectedKingdom.logic;
				data = ((logic != null) ? logic.royalFamily : null);
			}
			royalFamily.SetData(data);
			this.m_RoyalFamily.gameObject.SetActive(this.m_ShowFamilyOrCradinals && !flag);
		}
		if (this.m_Cardinals != null)
		{
			if (flag)
			{
				this.m_Cardinals.SetData(this.SelectedKingdom.logic);
			}
			this.m_Cardinals.gameObject.SetActive(this.m_ShowFamilyOrCradinals && flag);
		}
		if (this.m_Court != null)
		{
			this.m_Court.SetData(this.SelectedKingdom.logic);
			this.m_Court.gameObject.SetActive(this.m_ShowCourt);
		}
		if (this.m_Politics != null)
		{
			this.m_Politics.SetData(this.SelectedKingdom.logic);
			this.m_Politics.gameObject.SetActive(this.m_ShowPolitics);
		}
		if (this.m_Traditions != null)
		{
			this.m_Traditions.SetData(this.SelectedKingdom.logic);
			this.m_Traditions.gameObject.SetActive(this.m_ShowTradisions);
		}
		if (this.m_TabFamily != null)
		{
			this.m_TabFamily.SetSelected(this.m_ShowFamilyOrCradinals && !flag, false);
			this.m_TabFamily.gameObject.SetActive(!flag);
		}
		if (this.m_TabCardinals != null)
		{
			this.m_TabCardinals.SetSelected(this.m_ShowFamilyOrCradinals && flag, false);
			this.m_TabCardinals.gameObject.SetActive(flag);
		}
		if (this.m_TabCourt != null)
		{
			this.m_TabCourt.SetSelected(this.m_ShowCourt, false);
		}
		if (this.m_TabPolitics != null)
		{
			this.m_TabPolitics.SetSelected(this.m_ShowPolitics, false);
		}
		if (this.m_TabTraditions != null)
		{
			this.m_TabTraditions.SetSelected(this.m_ShowTradisions, false);
		}
	}

	// Token: 0x0600205B RID: 8283 RVA: 0x00129011 File Offset: 0x00127211
	private void InitiateAudience(BSGButton obj)
	{
		this.InitiateAudience();
	}

	// Token: 0x0600205C RID: 8284 RVA: 0x00129019 File Offset: 0x00127219
	private void InitiateAudience(UICharacterIcon obj)
	{
		if (UICommon.GetKey(KeyCode.LeftShift, false) || UICommon.GetKey(KeyCode.RightShift, false))
		{
			obj.ExecuteDefaultSelectAction();
			return;
		}
		this.InitiateAudience();
	}

	// Token: 0x0600205D RID: 8285 RVA: 0x00129044 File Offset: 0x00127244
	private MessageIcon GetPendingOfferIcon()
	{
		List<MessageIcon> pendingIcons = MessageIcon.pendingIcons;
		for (int i = 0; i < pendingIcons.Count; i++)
		{
			Offer offer = pendingIcons[i].vars.Get<Offer>("offer", null);
			if (offer.from == this.SelectedKingdom.logic || offer.to == this.SelectedKingdom.logic)
			{
				return pendingIcons[i];
			}
		}
		return null;
	}

	// Token: 0x0600205E RID: 8286 RVA: 0x001290AF File Offset: 0x001272AF
	private void InitiateAudience()
	{
		AudienceWindow.Create(this.SelectedKingdom, "Main", null);
		AudienceWindow.BringToFront();
	}

	// Token: 0x0600205F RID: 8287 RVA: 0x0012865C File Offset: 0x0012685C
	public override void ValidateSelectionObject()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x06002060 RID: 8288 RVA: 0x001290C8 File Offset: 0x001272C8
	private void HandleOnTabTraditions(BSGButton btn)
	{
		this.m_ShowPolitics = false;
		this.m_ShowTradisions = true;
		this.UpdateTabSelections();
	}

	// Token: 0x06002061 RID: 8289 RVA: 0x001290DE File Offset: 0x001272DE
	private void HandleOnTabPolitics(BSGButton btn)
	{
		this.m_ShowPolitics = true;
		this.m_ShowTradisions = false;
		this.UpdateTabSelections();
	}

	// Token: 0x06002062 RID: 8290 RVA: 0x001290F4 File Offset: 0x001272F4
	private void HandleOnTabCourt(BSGButton btn)
	{
		this.m_ShowFamilyOrCradinals = false;
		this.m_ShowCourt = true;
		this.UpdateTabSelections();
	}

	// Token: 0x06002063 RID: 8291 RVA: 0x0012910A File Offset: 0x0012730A
	private void HandleOnTabCardinals(BSGButton btn)
	{
		this.m_ShowFamilyOrCradinals = true;
		this.m_ShowCourt = false;
		this.UpdateTabSelections();
	}

	// Token: 0x06002064 RID: 8292 RVA: 0x0012910A File Offset: 0x0012730A
	private void HandleOnTabFamily(BSGButton btn)
	{
		this.m_ShowFamilyOrCradinals = true;
		this.m_ShowCourt = false;
		this.UpdateTabSelections();
	}

	// Token: 0x06002065 RID: 8293 RVA: 0x00129120 File Offset: 0x00127320
	protected override void HandleLogicMessage(object obj, string message, object param)
	{
		base.HandleLogicMessage(obj, message, param);
		if (message == "royal_new_sovereign")
		{
			this.PopulateKing();
			return;
		}
		if (!(message == "religion_changed"))
		{
			return;
		}
		this.PopulateStatic();
	}

	// Token: 0x0400156E RID: 5486
	[UIFieldTarget("id_KingIcon")]
	private UICharacterIcon m_KingIcon;

	// Token: 0x0400156F RID: 5487
	[UIFieldTarget("id_KingName")]
	private TextMeshProUGUI m_KingName;

	// Token: 0x04001570 RID: 5488
	[UIFieldTarget("id_KingAge")]
	private TextMeshProUGUI m_KingAge;

	// Token: 0x04001571 RID: 5489
	[UIFieldTarget("id_KingdomName")]
	private TextMeshProUGUI m_KingdomName;

	// Token: 0x04001572 RID: 5490
	[UIFieldTarget("id_OwnKingdomDescription")]
	private TextMeshProUGUI m_OwnKingdomDescription;

	// Token: 0x04001573 RID: 5491
	[UIFieldTarget("id_DiplomaticInfo")]
	private GameObject m_DiplomaticInfo;

	// Token: 0x04001574 RID: 5492
	[UIFieldTarget("id_Religion")]
	private UIReligion m_KingomdReligion;

	// Token: 0x04001575 RID: 5493
	[UIFieldTarget("id_Audience")]
	private BSGButton m_Audience;

	// Token: 0x04001576 RID: 5494
	[UIFieldTarget("id_AudienceLabel")]
	private TextMeshProUGUI m_AudienceLabel;

	// Token: 0x04001577 RID: 5495
	[UIFieldTarget("id_RequestPending")]
	private BSGButton m_RequestPending;

	// Token: 0x04001578 RID: 5496
	[UIFieldTarget("id_Relations")]
	private UIKingdomRelations m_Relations;

	// Token: 0x04001579 RID: 5497
	[UIFieldTarget("id_Shield")]
	private UIKingdomIcon m_Shield;

	// Token: 0x0400157A RID: 5498
	[UIFieldTarget("id_InfuenceContianer")]
	private GameObject m_InfuenceContianer;

	// Token: 0x0400157B RID: 5499
	[UIFieldTarget("id_Infuence")]
	private TextMeshProUGUI m_Infuence;

	// Token: 0x0400157C RID: 5500
	[UIFieldTarget("id_RoyalFamily")]
	private GameObject m_RoyalFamilyContainer;

	// Token: 0x0400157D RID: 5501
	[UIFieldTarget("id_RoyalCourt")]
	private GameObject m_RoyalCourtContainer;

	// Token: 0x0400157E RID: 5502
	[UIFieldTarget("id_Cardinals")]
	private GameObject m_CardinalsContainer;

	// Token: 0x0400157F RID: 5503
	[UIFieldTarget("id_Politics")]
	private GameObject m_PoliticsContainer;

	// Token: 0x04001580 RID: 5504
	[UIFieldTarget("id_Traditions")]
	private GameObject m_TraditionsContainer;

	// Token: 0x04001581 RID: 5505
	[UIFieldTarget("id_DefeatedLabel")]
	private TextMeshProUGUI m_DefeatedLabel;

	// Token: 0x04001582 RID: 5506
	[UIFieldTarget("id_TabFamily")]
	private BSGButton m_TabFamily;

	// Token: 0x04001583 RID: 5507
	[UIFieldTarget("id_TabFamilyLabel")]
	private TextMeshProUGUI m_TabFamilyLabel;

	// Token: 0x04001584 RID: 5508
	[UIFieldTarget("id_TabCourt")]
	private BSGButton m_TabCourt;

	// Token: 0x04001585 RID: 5509
	[UIFieldTarget("id_TabCourtLabel")]
	private TextMeshProUGUI m_TabCourtLabel;

	// Token: 0x04001586 RID: 5510
	[UIFieldTarget("id_TabCardinals")]
	private BSGButton m_TabCardinals;

	// Token: 0x04001587 RID: 5511
	[UIFieldTarget("id_TabCardinalsLabel")]
	private TextMeshProUGUI m_TabCardinalsLabel;

	// Token: 0x04001588 RID: 5512
	[UIFieldTarget("id_TabPolitics")]
	private BSGButton m_TabPolitics;

	// Token: 0x04001589 RID: 5513
	[UIFieldTarget("id_TabPoliticsLabel")]
	private TextMeshProUGUI m_TabPoliticsLabel;

	// Token: 0x0400158A RID: 5514
	[UIFieldTarget("id_TabTraditions")]
	private BSGButton m_TabTraditions;

	// Token: 0x0400158B RID: 5515
	[UIFieldTarget("id_TabTraditionsLabel")]
	private TextMeshProUGUI m_TabTraditionsLabel;

	// Token: 0x0400158C RID: 5516
	private UIKingdomWindow.RoyalFamily m_RoyalFamily;

	// Token: 0x0400158D RID: 5517
	private UIKingdomWindow.Court m_Court;

	// Token: 0x0400158E RID: 5518
	private UIKingdomWindow.Cardinals m_Cardinals;

	// Token: 0x0400158F RID: 5519
	private UIKingdomWindow.Politics m_Politics;

	// Token: 0x04001590 RID: 5520
	private UIKingdomWindow.Traditions m_Traditions;

	// Token: 0x04001591 RID: 5521
	private bool m_ShowFamilyOrCradinals = true;

	// Token: 0x04001592 RID: 5522
	private bool m_ShowCourt;

	// Token: 0x04001593 RID: 5523
	private bool m_ShowPolitics = true;

	// Token: 0x04001594 RID: 5524
	private bool m_ShowTradisions;

	// Token: 0x04001595 RID: 5525
	private bool m_Initialzied;

	// Token: 0x02000750 RID: 1872
	internal class Politics : MonoBehaviour, IListener
	{
		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x06004AA5 RID: 19109 RVA: 0x002218CB File Offset: 0x0021FACB
		// (set) Token: 0x06004AA6 RID: 19110 RVA: 0x002218D3 File Offset: 0x0021FAD3
		public Logic.Kingdom Data { get; private set; }

		// Token: 0x06004AA7 RID: 19111 RVA: 0x002218DC File Offset: 0x0021FADC
		private void Init()
		{
			if (this.m_Initiallized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.LocalizeStaticLabels();
			if (this.m_Allies != null)
			{
				UICommon.DeleteChildren(this.m_Allies);
			}
			if (this.m_Friends != null)
			{
				UICommon.DeleteChildren(this.m_Friends);
			}
			if (this.m_Vassals != null)
			{
				UICommon.DeleteChildren(this.m_Vassals);
			}
			if (this.m_Enemies != null)
			{
				UICommon.DeleteChildren(this.m_Enemies);
			}
			if (this.m_Threats != null)
			{
				UICommon.DeleteChildren(this.m_Threats);
			}
			if (this.m_Kinship != null)
			{
				UICommon.DeleteChildren(this.m_Kinship);
			}
			this.m_Initiallized = true;
		}

		// Token: 0x06004AA8 RID: 19112 RVA: 0x0022199C File Offset: 0x0021FB9C
		private void LocalizeStaticLabels()
		{
			if (this.m_LabelPolitics != null)
			{
				UIText.SetTextKey(this.m_LabelPolitics, "KingdomWindow.Politics.header", null, null);
			}
			if (this.m_AlliesLabel != null)
			{
				UIText.SetTextKey(this.m_AlliesLabel, "KingdomWindow.Politics.allies", null, null);
			}
			if (this.m_FriendsLabel != null)
			{
				UIText.SetTextKey(this.m_FriendsLabel, "KingdomWindow.Politics.friends", null, null);
			}
			if (this.m_EnemiesLabel != null)
			{
				UIText.SetTextKey(this.m_EnemiesLabel, "KingdomWindow.Politics.enemies", null, null);
			}
			if (this.m_ThreatsLabel != null)
			{
				UIText.SetTextKey(this.m_ThreatsLabel, "KingdomWindow.Politics.threats", null, null);
			}
			if (this.m_KinshipLabel != null)
			{
				UIText.SetTextKey(this.m_KinshipLabel, "KingdomWindow.Politics.kinship", null, null);
			}
		}

		// Token: 0x06004AA9 RID: 19113 RVA: 0x00221A69 File Offset: 0x0021FC69
		public void SetData(Logic.Kingdom k)
		{
			this.Init();
			Logic.Kingdom data = this.Data;
			if (data != null)
			{
				data.DelListener(this);
			}
			this.Data = k;
			Logic.Kingdom data2 = this.Data;
			if (data2 != null)
			{
				data2.AddListener(this);
			}
			this.Refresh();
		}

		// Token: 0x06004AAA RID: 19114 RVA: 0x00221AA4 File Offset: 0x0021FCA4
		private void Refresh()
		{
			this.UpdateCategory(this.m_Allies, this.GetAllies(), this.m_AlliesIconList);
			this.UpdateCategory(this.m_Friends, this.GetFriends(), this.m_FriendsIconList);
			this.UpdateCategory(this.m_Vassals, this.GetVassals(), this.m_VassalsIconList);
			this.UpdateCategory(this.m_Enemies, this.GetEnemies(), this.m_EmemiesIconList);
			this.UpdateCategory(this.m_Threats, this.GetThreads(), this.m_ThreatsIconList);
			this.UpdateCategory(this.m_Kinship, this.GetKinship(), this.m_KinshipIconList);
			if (this.Data.IsVassal())
			{
				UIText.SetTextKey(this.m_VassalageLabel, "KingdomWindow.Politics.vassal_of", null, null);
				return;
			}
			UIText.SetTextKey(this.m_VassalageLabel, "KingdomWindow.Politics.vassals", null, null);
		}

		// Token: 0x06004AAB RID: 19115 RVA: 0x00221B73 File Offset: 0x0021FD73
		private void Update()
		{
			if (this.m_Invalidate)
			{
				this.Refresh();
				this.m_Invalidate = false;
			}
		}

		// Token: 0x06004AAC RID: 19116 RVA: 0x00221B8C File Offset: 0x0021FD8C
		private void UpdateCategory(RectTransform container, List<Logic.Kingdom> kingdoms, List<UIKingdomIcon> icons)
		{
			if (container == null)
			{
				return;
			}
			if (kingdoms == null)
			{
				return;
			}
			if (icons == null)
			{
				icons = new List<UIKingdomIcon>();
			}
			int num = Mathf.Max(kingdoms.Count, icons.Count);
			for (int i = 0; i < num; i++)
			{
				UIKingdomIcon uikingdomIcon = (icons.Count > i) ? icons[i] : null;
				Logic.Kingdom kingdom = (kingdoms.Count > i) ? kingdoms[i] : null;
				if (kingdom != null && uikingdomIcon != null)
				{
					uikingdomIcon.SetObject(kingdom, null);
				}
				else if (kingdom != null && uikingdomIcon == null)
				{
					uikingdomIcon = this.AddKingdomIcon(kingdom, container);
					icons.Add(uikingdomIcon);
				}
				else if (kingdom == null && uikingdomIcon != null)
				{
					uikingdomIcon.SetObject(null, null);
				}
				uikingdomIcon.gameObject.SetActive(kingdom != null);
			}
			StackableIconsContainer component = container.GetComponent<StackableIconsContainer>();
			if (component == null)
			{
				return;
			}
			component.Refresh();
		}

		// Token: 0x06004AAD RID: 19117 RVA: 0x00221C64 File Offset: 0x0021FE64
		private List<Logic.Kingdom> GetFriends()
		{
			return UIWarsOverviewWindow.UIOverview.GetFriends(this.Data);
		}

		// Token: 0x06004AAE RID: 19118 RVA: 0x00221C74 File Offset: 0x0021FE74
		private List<Logic.Kingdom> GetVassals()
		{
			this.tmp_KingdomList.Clear();
			if (this.Data.IsVassal())
			{
				this.tmp_KingdomList.Add(this.Data.sovereignState);
			}
			else if (this.Data.vassalStates != null && this.Data.vassalStates.Count > 0)
			{
				for (int i = 0; i < this.Data.vassalStates.Count; i++)
				{
					Logic.Kingdom kingdom = this.Data.vassalStates[i];
					if (kingdom != null && !kingdom.IsDefeated())
					{
						this.tmp_KingdomList.Add(kingdom);
					}
				}
			}
			return this.tmp_KingdomList;
		}

		// Token: 0x06004AAF RID: 19119 RVA: 0x00221D1C File Offset: 0x0021FF1C
		private List<Logic.Kingdom> GetAllies()
		{
			this.m_TempKingdomSet.Clear();
			for (int i = 0; i < this.Data.wars.Count; i++)
			{
				War war = this.Data.wars[i];
				List<Logic.Kingdom> kingdoms = war.GetKingdoms(war.GetSide(this.Data));
				for (int j = 0; j < kingdoms.Count; j++)
				{
					this.m_TempKingdomSet.Add(kingdoms[j]);
				}
			}
			this.m_TempKingdomSet.Remove(this.Data);
			List<Logic.Kingdom> result = new List<Logic.Kingdom>(this.m_TempKingdomSet);
			this.m_TempKingdomSet.Clear();
			return result;
		}

		// Token: 0x06004AB0 RID: 19120 RVA: 0x00221DC0 File Offset: 0x0021FFC0
		private List<Logic.Kingdom> GetEnemies()
		{
			this.m_TempKingdomSet.Clear();
			for (int i = 0; i < this.Data.wars.Count; i++)
			{
				War war = this.Data.wars[i];
				List<Logic.Kingdom> kingdoms = war.GetKingdoms((war.GetSide(this.Data) == 0) ? 1 : 0);
				for (int j = 0; j < kingdoms.Count; j++)
				{
					this.m_TempKingdomSet.Add(kingdoms[j]);
				}
			}
			this.m_TempKingdomSet.Remove(this.Data);
			List<Logic.Kingdom> result = new List<Logic.Kingdom>(this.m_TempKingdomSet);
			this.m_TempKingdomSet.Clear();
			return result;
		}

		// Token: 0x06004AB1 RID: 19121 RVA: 0x00221E68 File Offset: 0x00220068
		private List<Logic.Kingdom> GetThreads()
		{
			return UIWarsOverviewWindow.UIOverview.GetThreads(this.Data);
		}

		// Token: 0x06004AB2 RID: 19122 RVA: 0x00221E78 File Offset: 0x00220078
		private List<Logic.Kingdom> GetKinship()
		{
			this.tmp_KingdomList.Clear();
			List<Marriage> marriages = this.Data.marriages;
			if (marriages == null || marriages.Count == 0)
			{
				return this.tmp_KingdomList;
			}
			for (int i = 0; i < marriages.Count; i++)
			{
				Marriage marriage = marriages[i];
				if (KingdomAndKingdomRelation.GetMarriage(marriage.kingdom_husband, marriage.wife.GetOriginalKingdom()))
				{
					Logic.Kingdom item = (marriage.kingdom_husband == this.Data) ? marriage.wife.GetOriginalKingdom() : marriage.kingdom_husband;
					if (!this.tmp_KingdomList.Contains(item))
					{
						this.tmp_KingdomList.Add(item);
					}
				}
			}
			return this.tmp_KingdomList;
		}

		// Token: 0x06004AB3 RID: 19123 RVA: 0x00221F24 File Offset: 0x00220124
		private UIKingdomIcon AddKingdomIcon(Logic.Kingdom k, RectTransform container)
		{
			if (k == null)
			{
				return null;
			}
			if (container == null)
			{
				return null;
			}
			Vars vars = new Vars(k);
			vars.Set<string>("variant", "compact");
			GameObject icon = ObjectIcon.GetIcon(k, vars, container);
			UIKingdomIcon uikingdomIcon = (icon != null) ? icon.GetComponent<UIKingdomIcon>() : null;
			if (uikingdomIcon != null)
			{
				LayoutElement orAddComponent = uikingdomIcon.GetOrAddComponent<LayoutElement>();
				orAddComponent.preferredWidth = 26f;
				orAddComponent.preferredHeight = 34f;
			}
			return uikingdomIcon;
		}

		// Token: 0x06004AB4 RID: 19124 RVA: 0x00221F9C File Offset: 0x0022019C
		public void OnMessage(object obj, string message, object param)
		{
			if (message == "vassals_changed" || message == "sovereign_set" || message == "sovereign_removed" || message == "new_marriage" || message == "stance_changed" || message == "relation_modified")
			{
				this.m_Invalidate = true;
			}
		}

		// Token: 0x04003992 RID: 14738
		[UIFieldTarget("id_Allies")]
		private RectTransform m_Allies;

		// Token: 0x04003993 RID: 14739
		[UIFieldTarget("id_AlliesLabel")]
		private TextMeshProUGUI m_AlliesLabel;

		// Token: 0x04003994 RID: 14740
		[UIFieldTarget("id_Friends")]
		private RectTransform m_Friends;

		// Token: 0x04003995 RID: 14741
		[UIFieldTarget("id_FriendsLabel")]
		private TextMeshProUGUI m_FriendsLabel;

		// Token: 0x04003996 RID: 14742
		[UIFieldTarget("id_Vassals")]
		private RectTransform m_Vassals;

		// Token: 0x04003997 RID: 14743
		[UIFieldTarget("id_VassalageLabel")]
		private TextMeshProUGUI m_VassalageLabel;

		// Token: 0x04003998 RID: 14744
		[UIFieldTarget("id_Enemies")]
		private RectTransform m_Enemies;

		// Token: 0x04003999 RID: 14745
		[UIFieldTarget("id_EnemiesLabel")]
		private TextMeshProUGUI m_EnemiesLabel;

		// Token: 0x0400399A RID: 14746
		[UIFieldTarget("id_Threats")]
		private RectTransform m_Threats;

		// Token: 0x0400399B RID: 14747
		[UIFieldTarget("id_ThreatsLabel")]
		private TextMeshProUGUI m_ThreatsLabel;

		// Token: 0x0400399C RID: 14748
		[UIFieldTarget("id_Kinship")]
		private RectTransform m_Kinship;

		// Token: 0x0400399D RID: 14749
		[UIFieldTarget("id_KinshipLabel")]
		private TextMeshProUGUI m_KinshipLabel;

		// Token: 0x0400399E RID: 14750
		[UIFieldTarget("id_LabelPolitics")]
		private TextMeshProUGUI m_LabelPolitics;

		// Token: 0x040039A0 RID: 14752
		private List<UIKingdomIcon> m_AlliesIconList = new List<UIKingdomIcon>();

		// Token: 0x040039A1 RID: 14753
		private List<UIKingdomIcon> m_FriendsIconList = new List<UIKingdomIcon>();

		// Token: 0x040039A2 RID: 14754
		private List<UIKingdomIcon> m_VassalsIconList = new List<UIKingdomIcon>();

		// Token: 0x040039A3 RID: 14755
		private List<UIKingdomIcon> m_EmemiesIconList = new List<UIKingdomIcon>();

		// Token: 0x040039A4 RID: 14756
		private List<UIKingdomIcon> m_ThreatsIconList = new List<UIKingdomIcon>();

		// Token: 0x040039A5 RID: 14757
		private List<UIKingdomIcon> m_KinshipIconList = new List<UIKingdomIcon>();

		// Token: 0x040039A6 RID: 14758
		private HashSet<Logic.Kingdom> m_TempKingdomSet = new HashSet<Logic.Kingdom>();

		// Token: 0x040039A7 RID: 14759
		private List<Logic.Kingdom> tmp_KingdomList = new List<Logic.Kingdom>();

		// Token: 0x040039A8 RID: 14760
		private bool m_Invalidate;

		// Token: 0x040039A9 RID: 14761
		private bool m_Initiallized;
	}

	// Token: 0x02000751 RID: 1873
	internal class RoyalFamily : MonoBehaviour, IListener
	{
		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x06004AB6 RID: 19126 RVA: 0x0022206B File Offset: 0x0022026B
		// (set) Token: 0x06004AB7 RID: 19127 RVA: 0x00222073 File Offset: 0x00220273
		public Logic.RoyalFamily Data { get; private set; }

		// Token: 0x06004AB8 RID: 19128 RVA: 0x0022207C File Offset: 0x0022027C
		private void Init()
		{
			if (this.m_Initiallized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initiallized = true;
			this.LocalizeStaticLabels();
		}

		// Token: 0x06004AB9 RID: 19129 RVA: 0x0022209B File Offset: 0x0022029B
		private void LocalizeStaticLabels()
		{
			if (this.m_LabelFamily != null)
			{
				UIText.SetTextKey(this.m_LabelFamily, "KingdomWindow.RoyalFamily.header", null, null);
			}
		}

		// Token: 0x06004ABA RID: 19130 RVA: 0x002220BD File Offset: 0x002202BD
		public void SetData(Logic.RoyalFamily royalFamilily)
		{
			this.Init();
			this.RemoveListeners();
			this.Data = royalFamilily;
			this.AddListeners();
			this.Refresh();
		}

		// Token: 0x06004ABB RID: 19131 RVA: 0x002220DE File Offset: 0x002202DE
		private void Refresh()
		{
			if (this.Data == null)
			{
				return;
			}
			this.UpdateSpouse();
			this.PopulateChildren();
		}

		// Token: 0x06004ABC RID: 19132 RVA: 0x002220F5 File Offset: 0x002202F5
		private void Update()
		{
			if (this.m_Invalidate)
			{
				this.Refresh();
				this.m_Invalidate = false;
			}
		}

		// Token: 0x06004ABD RID: 19133 RVA: 0x0022210C File Offset: 0x0022030C
		private void UpdateSpouse()
		{
			Logic.RoyalFamily data = this.Data;
			Logic.Character character;
			if (data == null)
			{
				character = null;
			}
			else
			{
				Logic.Kingdom kingdom = data.GetKingdom();
				character = ((kingdom != null) ? kingdom.royalFamily.Spouse : null);
			}
			Logic.Character character2 = character;
			if (this.m_SpouseIcon != null)
			{
				this.m_SpouseIcon.SetObject(character2, null);
				this.m_SpouseIcon.ShowCrest(false);
			}
			if (this.m_QueenOriginKingdom != null)
			{
				this.m_QueenOriginKingdom.gameObject.SetActive(character2 != null && character2.original_kingdom_id != character2.GetKingdom().id);
				ObjectIcon queenOriginKingdom = this.m_QueenOriginKingdom;
				object obj;
				if (character2 == null)
				{
					obj = null;
				}
				else
				{
					Game game = character2.game;
					obj = ((game != null) ? game.GetKingdom(character2.original_kingdom_id) : null);
				}
				queenOriginKingdom.SetObject(obj, null);
			}
			if (this.m_SpouseName != null)
			{
				this.m_SpouseName.gameObject.SetActive(character2 != null);
				if (character2 != null)
				{
					UIText.SetTextKey(this.m_SpouseName, "Character.title_name", character2, null);
				}
			}
			if (this.m_SpouseTitle != null)
			{
				this.m_SpouseTitle.gameObject.SetActive(character2 != null);
				if (character2 != null)
				{
					UIText.SetText(this.m_SpouseTitle, global::Defs.Localize(character2.GetTitle(), null, null, true, true));
				}
			}
			if (this.m_SpouseNameOnly != null)
			{
				this.m_SpouseNameOnly.gameObject.SetActive(character2 != null);
				if (character2 != null)
				{
					UIText.SetTextKey(this.m_SpouseNameOnly, "Character.name_only", character2, null);
				}
			}
			if (this.m_SpouseOriginTitle != null)
			{
				this.m_SpouseOriginTitle.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004ABE RID: 19134 RVA: 0x00222294 File Offset: 0x00220494
		private void PopulateChildren()
		{
			if (this.m_ChildrenContainer == null)
			{
				return;
			}
			Logic.RoyalFamily data = this.Data;
			int? num;
			if (data == null)
			{
				num = null;
			}
			else
			{
				Logic.Kingdom kingdom = data.GetKingdom();
				num = ((kingdom != null) ? new int?(kingdom.royalFamily.MaxChildren()) : null);
			}
			int? num2 = num;
			if (num2 == null)
			{
				num2 = new int?(4);
			}
			Logic.RoyalFamily data2 = this.Data;
			List<Logic.Character> list;
			if (data2 == null)
			{
				list = null;
			}
			else
			{
				Logic.Kingdom kingdom2 = data2.GetKingdom();
				list = ((kingdom2 != null) ? kingdom2.royalFamily.Children : null);
			}
			List<Logic.Character> list2 = list;
			UICharacterIcon[] componentsInChildren = this.m_ChildrenContainer.GetComponentsInChildren<UICharacterIcon>(true);
			GameObject childrenSpouseContianer = this.m_ChildrenSpouseContianer;
			UICharacterIcon[] array = (childrenSpouseContianer != null) ? childrenSpouseContianer.GetComponentsInChildren<UICharacterIcon>(true) : null;
			for (int i = 0; i < num2.Value; i++)
			{
				Logic.Character character = (list2.Count > i) ? list2[i] : null;
				UICharacterIcon uicharacterIcon;
				if (componentsInChildren != null && componentsInChildren.Length > i)
				{
					uicharacterIcon = componentsInChildren[i];
					uicharacterIcon.SetObject(character, null);
				}
				else if (character != null)
				{
					GameObject icon = ObjectIcon.GetIcon(character, null, this.m_ChildrenContainer.transform as RectTransform);
					uicharacterIcon = ((icon != null) ? icon.GetComponent<UICharacterIcon>() : null);
				}
				else
				{
					Vars vars = new Vars();
					vars.Set<string>("variant", "royal_family");
					GameObject icon2 = ObjectIcon.GetIcon("Character", vars, this.m_ChildrenContainer.transform as RectTransform);
					uicharacterIcon = ((icon2 != null) ? icon2.GetComponent<UICharacterIcon>() : null);
				}
				if (uicharacterIcon != null)
				{
					uicharacterIcon.ShowStatus(false);
				}
				if (uicharacterIcon != null)
				{
					uicharacterIcon.ShowArmyBanner(false);
				}
				if (uicharacterIcon != null)
				{
					uicharacterIcon.ShowCrest(false);
				}
				if (array != null && i < array.Length)
				{
					Logic.Character character2 = (character != null) ? character.GetSpouse() : null;
					if (character2 != null)
					{
						array[i].SetObject(character2, null);
						array[i].ShowStatus(false);
						array[i].ShowMissonKingdomCrest(false);
						array[i].ShowPrisonKingdomCrest(false);
						UIKingdomIcon component = global::Common.FindChildByName(array[i].gameObject, "_Crest", true, true).GetComponent<UIKingdomIcon>();
						if (character2.IsRoyalChild() || character2.sex == Logic.Character.Sex.Male)
						{
							component.SetObject(character2.GetOriginalKingdom(), null);
						}
						else
						{
							component.gameObject.SetActive(false);
						}
					}
					else
					{
						array[i].SetObject(null, null);
					}
					if (character == null || (character2 == null && !character.CanMarry()))
					{
						global::Common.FindChildByName(array[i].gameObject, "Group_Empty", true, true).SetActive(false);
						global::Common.FindChildByName(array[i].gameObject, "Group_Populated", true, true).SetActive(false);
					}
				}
			}
		}

		// Token: 0x06004ABF RID: 19135 RVA: 0x00222518 File Offset: 0x00220718
		private void AddListeners()
		{
			Logic.RoyalFamily data = this.Data;
			if (data != null)
			{
				data.AddListener(this);
			}
			Logic.RoyalFamily data2 = this.Data;
			if (data2 == null)
			{
				return;
			}
			Logic.Kingdom kingdom = data2.GetKingdom();
			if (kingdom == null)
			{
				return;
			}
			kingdom.AddListener(this);
		}

		// Token: 0x06004AC0 RID: 19136 RVA: 0x00222547 File Offset: 0x00220747
		private void RemoveListeners()
		{
			Logic.RoyalFamily data = this.Data;
			if (data != null)
			{
				data.DelListener(this);
			}
			Logic.RoyalFamily data2 = this.Data;
			if (data2 == null)
			{
				return;
			}
			Logic.Kingdom kingdom = data2.GetKingdom();
			if (kingdom == null)
			{
				return;
			}
			kingdom.DelListener(this);
		}

		// Token: 0x06004AC1 RID: 19137 RVA: 0x00222578 File Offset: 0x00220778
		public void OnMessage(object obj, string message, object param)
		{
			if (obj is Logic.Kingdom)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
				if (num <= 2158144290U)
				{
					if (num <= 810626458U)
					{
						if (num != 190343346U)
						{
							if (num != 810626458U)
							{
								goto IL_118;
							}
							if (!(message == "stance_changed"))
							{
								goto IL_118;
							}
						}
						else if (!(message == "royal_new_sovereign"))
						{
							goto IL_118;
						}
					}
					else if (num != 1065923593U)
					{
						if (num != 2158144290U)
						{
							goto IL_118;
						}
						if (!(message == "new_marriage"))
						{
							goto IL_118;
						}
					}
					else if (!(message == "royal_new_born"))
					{
						goto IL_118;
					}
				}
				else if (num <= 2626030674U)
				{
					if (num != 2418603302U)
					{
						if (num != 2626030674U)
						{
							goto IL_118;
						}
						if (!(message == "royal_ties_broken"))
						{
							goto IL_118;
						}
					}
					else if (!(message == "princess_becomes_queen"))
					{
						goto IL_118;
					}
				}
				else if (num != 3002834028U)
				{
					if (num != 3568541980U)
					{
						if (num != 3694414769U)
						{
							goto IL_118;
						}
						if (!(message == "king_changed"))
						{
							goto IL_118;
						}
					}
					else if (!(message == "new_queen"))
					{
						goto IL_118;
					}
				}
				else if (!(message == "charcter_divorce"))
				{
					goto IL_118;
				}
				this.m_Invalidate = true;
			}
			IL_118:
			if (obj is Logic.RoyalFamily)
			{
				this.m_Invalidate = true;
			}
		}

		// Token: 0x06004AC2 RID: 19138 RVA: 0x002226AC File Offset: 0x002208AC
		private void OnDestroy()
		{
			this.RemoveListeners();
		}

		// Token: 0x040039AA RID: 14762
		[UIFieldTarget("id_SpouseIcon")]
		private UICharacterIcon m_SpouseIcon;

		// Token: 0x040039AB RID: 14763
		[UIFieldTarget("id_QueenOriginKingdom")]
		private UIKingdomIcon m_QueenOriginKingdom;

		// Token: 0x040039AC RID: 14764
		[UIFieldTarget("id_SpouseName")]
		private TextMeshProUGUI m_SpouseName;

		// Token: 0x040039AD RID: 14765
		[UIFieldTarget("id_SpouseTitle")]
		private TextMeshProUGUI m_SpouseTitle;

		// Token: 0x040039AE RID: 14766
		[UIFieldTarget("id_SpouseNameOnly")]
		private TextMeshProUGUI m_SpouseNameOnly;

		// Token: 0x040039AF RID: 14767
		[UIFieldTarget("id_SpouseOriginTitle")]
		private TextMeshProUGUI m_SpouseOriginTitle;

		// Token: 0x040039B0 RID: 14768
		[UIFieldTarget("id_Child", true)]
		private GameObject[] Children;

		// Token: 0x040039B1 RID: 14769
		[UIFieldTarget("id_ChildrenContainer")]
		private GameObject m_ChildrenContainer;

		// Token: 0x040039B2 RID: 14770
		[UIFieldTarget("id_ChildrenSpouseContianer")]
		private GameObject m_ChildrenSpouseContianer;

		// Token: 0x040039B3 RID: 14771
		[UIFieldTarget("id_LabelFamily")]
		private TextMeshProUGUI m_LabelFamily;

		// Token: 0x040039B5 RID: 14773
		private bool m_Initiallized;

		// Token: 0x040039B6 RID: 14774
		private bool m_Invalidate;
	}

	// Token: 0x02000752 RID: 1874
	internal class Cardinals : MonoBehaviour, IListener
	{
		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x06004AC4 RID: 19140 RVA: 0x002226B4 File Offset: 0x002208B4
		// (set) Token: 0x06004AC5 RID: 19141 RVA: 0x002226BC File Offset: 0x002208BC
		public Logic.Kingdom Papacy { get; private set; }

		// Token: 0x06004AC6 RID: 19142 RVA: 0x002226C5 File Offset: 0x002208C5
		private void Init()
		{
			if (this.m_Initiallized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initiallized = true;
			this.LocalizeStaticLabels();
		}

		// Token: 0x06004AC7 RID: 19143 RVA: 0x002226E4 File Offset: 0x002208E4
		private void LocalizeStaticLabels()
		{
			if (this.m_LabelCardinals != null)
			{
				UIText.SetTextKey(this.m_LabelCardinals, "KingdomWindow.Cardinals.header", null, null);
			}
		}

		// Token: 0x06004AC8 RID: 19144 RVA: 0x00222706 File Offset: 0x00220906
		public void SetData(Logic.Kingdom papacy)
		{
			this.Init();
			this.RemoveListeners();
			this.Papacy = papacy;
			this.AddListeners();
			this.Refresh();
		}

		// Token: 0x06004AC9 RID: 19145 RVA: 0x00222727 File Offset: 0x00220927
		private void Refresh()
		{
			if (this.Papacy == null)
			{
				return;
			}
			this.PopulateCardinals();
		}

		// Token: 0x06004ACA RID: 19146 RVA: 0x00222738 File Offset: 0x00220938
		private void Update()
		{
			if (this.m_Invalidate)
			{
				this.Refresh();
				this.m_Invalidate = false;
			}
		}

		// Token: 0x06004ACB RID: 19147 RVA: 0x00222750 File Offset: 0x00220950
		private void PopulateCardinals()
		{
			if (this.m_CardinalsContainer == null)
			{
				return;
			}
			Logic.Kingdom papacy = this.Papacy;
			int? num = (papacy != null) ? new int?(papacy.game.religions.catholic.max_cardinals) : null;
			Logic.Kingdom papacy2 = this.Papacy;
			List<Logic.Character> list;
			if (papacy2 == null)
			{
				list = null;
			}
			else
			{
				Game game = papacy2.game;
				if (game == null)
				{
					list = null;
				}
				else
				{
					Logic.Religions religions = game.religions;
					if (religions == null)
					{
						list = null;
					}
					else
					{
						Catholic catholic = religions.catholic;
						list = ((catholic != null) ? catholic.cardinals : null);
					}
				}
			}
			List<Logic.Character> list2 = list;
			UICharacterIcon[] componentsInChildren = this.m_CardinalsContainer.GetComponentsInChildren<UICharacterIcon>();
			for (int i = 0; i < num.Value; i++)
			{
				Logic.Character character = (list2 != null && list2.Count > i) ? list2[i] : null;
				UICharacterIcon uicharacterIcon;
				if (componentsInChildren != null && componentsInChildren.Length > i)
				{
					uicharacterIcon = componentsInChildren[i];
					uicharacterIcon.SetObject(character, null);
				}
				else if (character != null)
				{
					GameObject icon = ObjectIcon.GetIcon(character, null, this.m_CardinalsContainer.transform as RectTransform);
					uicharacterIcon = ((icon != null) ? icon.GetComponent<UICharacterIcon>() : null);
				}
				else
				{
					Vars vars = new Vars();
					vars.Set<string>("variant", "cardinal");
					GameObject icon2 = ObjectIcon.GetIcon("Character", vars, this.m_CardinalsContainer.transform as RectTransform);
					uicharacterIcon = ((icon2 != null) ? icon2.GetComponent<UICharacterIcon>() : null);
				}
				if (uicharacterIcon != null)
				{
					uicharacterIcon.ShowStatus(false);
				}
				if (uicharacterIcon != null)
				{
					uicharacterIcon.ShowArmyBanner(false);
				}
				if (uicharacterIcon != null)
				{
					uicharacterIcon.ShowCrest(false);
				}
			}
		}

		// Token: 0x06004ACC RID: 19148 RVA: 0x002228CA File Offset: 0x00220ACA
		private void AddListeners()
		{
			Logic.Kingdom papacy = this.Papacy;
			if (papacy != null)
			{
				papacy.AddListener(this);
			}
			Logic.Kingdom papacy2 = this.Papacy;
			if (papacy2 == null)
			{
				return;
			}
			Game game = papacy2.game;
			if (game == null)
			{
				return;
			}
			Logic.Religions religions = game.religions;
			if (religions == null)
			{
				return;
			}
			religions.AddListener(this);
		}

		// Token: 0x06004ACD RID: 19149 RVA: 0x00222903 File Offset: 0x00220B03
		private void RemoveListeners()
		{
			Logic.Kingdom papacy = this.Papacy;
			if (papacy != null)
			{
				papacy.DelListener(this);
			}
			Logic.Kingdom papacy2 = this.Papacy;
			if (papacy2 == null)
			{
				return;
			}
			Game game = papacy2.game;
			if (game == null)
			{
				return;
			}
			Logic.Religions religions = game.religions;
			if (religions == null)
			{
				return;
			}
			religions.DelListener(this);
		}

		// Token: 0x06004ACE RID: 19150 RVA: 0x0022293C File Offset: 0x00220B3C
		public void OnMessage(object obj, string message, object param)
		{
			if (message == "cardinals_changed")
			{
				this.m_Invalidate = true;
			}
		}

		// Token: 0x06004ACF RID: 19151 RVA: 0x00222952 File Offset: 0x00220B52
		private void OnDestroy()
		{
			this.RemoveListeners();
		}

		// Token: 0x040039B7 RID: 14775
		[UIFieldTarget("id_CardinalIcons")]
		private GameObject m_CardinalsContainer;

		// Token: 0x040039B8 RID: 14776
		[UIFieldTarget("id_LabelCardinals")]
		private TextMeshProUGUI m_LabelCardinals;

		// Token: 0x040039BA RID: 14778
		private bool m_Initiallized;

		// Token: 0x040039BB RID: 14779
		private bool m_Invalidate;
	}

	// Token: 0x02000753 RID: 1875
	internal class Court : MonoBehaviour, IListener
	{
		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x06004AD1 RID: 19153 RVA: 0x0022295A File Offset: 0x00220B5A
		// (set) Token: 0x06004AD2 RID: 19154 RVA: 0x00222962 File Offset: 0x00220B62
		public Logic.Kingdom Kingdom { get; private set; }

		// Token: 0x06004AD3 RID: 19155 RVA: 0x0022296C File Offset: 0x00220B6C
		private void Init()
		{
			if (this.m_Initiallized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			UICharacterIcon[] componentsInChildren = this.m_CourtContainer.GetComponentsInChildren<UICharacterIcon>();
			if (componentsInChildren != null && componentsInChildren.Length != 0)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].OnPointerEvent += this.OnCharacterPointerEvent;
				}
			}
			this.m_Initiallized = true;
		}

		// Token: 0x06004AD4 RID: 19156 RVA: 0x002229C5 File Offset: 0x00220BC5
		public void SetData(Logic.Kingdom k)
		{
			this.Init();
			this.RemoveListeners();
			this.Kingdom = k;
			this.AddListeners();
			this.Refresh();
		}

		// Token: 0x06004AD5 RID: 19157 RVA: 0x002229E6 File Offset: 0x00220BE6
		private void Refresh()
		{
			if (this.Kingdom == null)
			{
				return;
			}
			this.PopulateCourt();
		}

		// Token: 0x06004AD6 RID: 19158 RVA: 0x002229F7 File Offset: 0x00220BF7
		private void Update()
		{
			if (this.m_Invalidate)
			{
				this.Refresh();
				this.m_Invalidate = false;
			}
		}

		// Token: 0x06004AD7 RID: 19159 RVA: 0x00222A10 File Offset: 0x00220C10
		private void PopulateCourt()
		{
			if (this.m_CourtContainer == null)
			{
				return;
			}
			if (this.Kingdom == null)
			{
				return;
			}
			int count = this.Kingdom.court.Count;
			UICharacterIcon[] componentsInChildren = this.m_CourtContainer.GetComponentsInChildren<UICharacterIcon>();
			for (int i = 0; i < count - 1; i++)
			{
				Logic.Character courtOrSpecialCourtMember = this.Kingdom.GetCourtOrSpecialCourtMember(i + 1);
				UICharacterIcon uicharacterIcon;
				if (componentsInChildren != null && componentsInChildren.Length > i)
				{
					uicharacterIcon = componentsInChildren[i];
					uicharacterIcon.SetObject(courtOrSpecialCourtMember, null);
				}
				else
				{
					if (courtOrSpecialCourtMember != null)
					{
						GameObject icon = ObjectIcon.GetIcon(courtOrSpecialCourtMember, null, this.m_CourtContainer.transform as RectTransform);
						uicharacterIcon = ((icon != null) ? icon.GetComponent<UICharacterIcon>() : null);
					}
					else
					{
						Vars vars = new Vars();
						vars.Set<string>("variant", "enemy_court");
						GameObject icon2 = ObjectIcon.GetIcon("Character", vars, this.m_CourtContainer.transform as RectTransform);
						uicharacterIcon = ((icon2 != null) ? icon2.GetComponent<UICharacterIcon>() : null);
					}
					uicharacterIcon.OnPointerEvent += this.OnCharacterPointerEvent;
				}
				if (uicharacterIcon != null)
				{
					uicharacterIcon.ShowStatus(false);
				}
				if (uicharacterIcon != null)
				{
					uicharacterIcon.ShowArmyBanner(false);
				}
				if (uicharacterIcon != null)
				{
					uicharacterIcon.ShowCrest(false);
				}
			}
		}

		// Token: 0x06004AD8 RID: 19160 RVA: 0x00222B3C File Offset: 0x00220D3C
		private void OnCharacterPointerEvent(Hotspot h, EventTriggerType et, PointerEventData e)
		{
			if (et != EventTriggerType.PointerUp)
			{
				return;
			}
			if (this.delayClick == null)
			{
				this.delayClick = base.StartCoroutine(this.Delay(BaseUI.Get().dblclk_delay / 2f, h));
				return;
			}
			base.StopCoroutine(this.delayClick);
			this.OnCharacterClickFinish(true, h);
		}

		// Token: 0x06004AD9 RID: 19161 RVA: 0x00222B90 File Offset: 0x00220D90
		private void OnCharacterClickFinish(bool double_click, Hotspot h)
		{
			this.delayClick = null;
			UICharacterIcon uicharacterIcon = h as UICharacterIcon;
			object obj;
			if (uicharacterIcon == null)
			{
				obj = null;
			}
			else
			{
				Logic.Character data = uicharacterIcon.Data;
				if (data == null)
				{
					obj = null;
				}
				else
				{
					Logic.Army army = data.GetArmy();
					obj = ((army != null) ? army.visuals : null);
				}
			}
			global::Army army2 = obj as global::Army;
			if (((army2 != null) ? army2.gameObject : null) != null)
			{
				GameObject selectionObj = BaseUI.Get().GetSelectionObj(army2.gameObject);
				if (selectionObj == null)
				{
					return;
				}
				BaseUI.Get().SelectObj(selectionObj, false, true, true, true);
				if (double_click)
				{
					BaseUI.Get().LookAt(selectionObj.transform.position, false);
				}
			}
		}

		// Token: 0x06004ADA RID: 19162 RVA: 0x00222C2B File Offset: 0x00220E2B
		private IEnumerator Delay(float delay, Hotspot h)
		{
			yield return new WaitForSecondsRealtime(delay);
			this.OnCharacterClickFinish(false, h);
			yield break;
		}

		// Token: 0x06004ADB RID: 19163 RVA: 0x00222C48 File Offset: 0x00220E48
		private void AddListeners()
		{
			Logic.Kingdom kingdom = this.Kingdom;
			if (kingdom == null)
			{
				return;
			}
			kingdom.AddListener(this);
		}

		// Token: 0x06004ADC RID: 19164 RVA: 0x00222C5B File Offset: 0x00220E5B
		private void RemoveListeners()
		{
			Logic.Kingdom kingdom = this.Kingdom;
			if (kingdom == null)
			{
				return;
			}
			kingdom.DelListener(this);
		}

		// Token: 0x06004ADD RID: 19165 RVA: 0x00222C6E File Offset: 0x00220E6E
		public void OnMessage(object obj, string message, object param)
		{
			if (message == "court_changed")
			{
				this.m_Invalidate = true;
			}
		}

		// Token: 0x06004ADE RID: 19166 RVA: 0x00222C84 File Offset: 0x00220E84
		private void OnDestroy()
		{
			this.RemoveListeners();
		}

		// Token: 0x040039BC RID: 14780
		[UIFieldTarget("id_CourtIcons")]
		private GameObject m_CourtContainer;

		// Token: 0x040039BE RID: 14782
		private bool m_Initiallized;

		// Token: 0x040039BF RID: 14783
		private bool m_Invalidate;

		// Token: 0x040039C0 RID: 14784
		private UnityEngine.Coroutine delayClick;
	}

	// Token: 0x02000754 RID: 1876
	internal class Traditions : MonoBehaviour, IListener
	{
		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x06004AE0 RID: 19168 RVA: 0x00222C8C File Offset: 0x00220E8C
		// (set) Token: 0x06004AE1 RID: 19169 RVA: 0x00222C94 File Offset: 0x00220E94
		public Logic.Kingdom Kingdom { get; private set; }

		// Token: 0x06004AE2 RID: 19170 RVA: 0x00222C9D File Offset: 0x00220E9D
		private void Init()
		{
			if (this.m_Initiallized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initiallized = true;
		}

		// Token: 0x06004AE3 RID: 19171 RVA: 0x00222CB6 File Offset: 0x00220EB6
		public void SetData(Logic.Kingdom k)
		{
			this.Init();
			this.RemoveListeners();
			this.Kingdom = k;
			this.AddListeners();
			this.Refresh();
		}

		// Token: 0x06004AE4 RID: 19172 RVA: 0x00222CD7 File Offset: 0x00220ED7
		private void Refresh()
		{
			if (this.Kingdom == null)
			{
				return;
			}
			this.Populate();
		}

		// Token: 0x06004AE5 RID: 19173 RVA: 0x00222CE8 File Offset: 0x00220EE8
		private void Update()
		{
			if (this.m_Invalidate)
			{
				this.UpdateTraditions();
				this.m_Invalidate = false;
			}
		}

		// Token: 0x06004AE6 RID: 19174 RVA: 0x00222D00 File Offset: 0x00220F00
		private void Populate()
		{
			if (this.m_TraditionsContainer == null)
			{
				return;
			}
			if (this.Kingdom == null)
			{
				return;
			}
			for (int i = 0; i < this.Kingdom.tradition_slots_types.Length; i++)
			{
				GameObject gameObject = (this.m_TraditionSlotObjects != null && this.m_TraditionSlotObjects.Length > i) ? this.m_TraditionSlotObjects[i] : null;
				if (gameObject == null)
				{
					break;
				}
				UIDynastyTradition orAddComponent = gameObject.GetOrAddComponent<UIDynastyTradition>();
				Tradition.Def def;
				if (this.Kingdom.traditions == null || this.Kingdom.traditions.Count <= i)
				{
					def = null;
				}
				else
				{
					Tradition tradition = this.Kingdom.traditions[i];
					def = ((tradition != null) ? tradition.def : null);
				}
				Tradition.Def def2 = def;
				orAddComponent.SetData(this.Kingdom, def2, false);
				gameObject.SetActive(true);
				this.m_TraditionSlots.Add(orAddComponent);
			}
		}

		// Token: 0x06004AE7 RID: 19175 RVA: 0x00222DD8 File Offset: 0x00220FD8
		private void UpdateTraditions()
		{
			if (this.Kingdom == null)
			{
				return;
			}
			if (this.m_TraditionSlotObjects == null || this.m_TraditionSlots.Count == 0)
			{
				return;
			}
			if (this.Kingdom.tradition_slots_types == null || this.Kingdom.tradition_slots_types.Length == 0)
			{
				return;
			}
			for (int i = 0; i < this.Kingdom.tradition_slots_types.Length; i++)
			{
				UIDynastyTradition uidynastyTradition = (this.m_TraditionSlots.Count > i) ? this.m_TraditionSlots[i] : null;
				if (uidynastyTradition == null)
				{
					break;
				}
				Tradition.Def def;
				if (this.Kingdom.traditions == null || this.Kingdom.traditions.Count <= i)
				{
					def = null;
				}
				else
				{
					Tradition tradition = this.Kingdom.traditions[i];
					def = ((tradition != null) ? tradition.def : null);
				}
				Tradition.Def def2 = def;
				uidynastyTradition.SetData(this.Kingdom, def2, true);
			}
		}

		// Token: 0x06004AE8 RID: 19176 RVA: 0x00222EB1 File Offset: 0x002210B1
		private void AddListeners()
		{
			Logic.Kingdom kingdom = this.Kingdom;
			if (kingdom == null)
			{
				return;
			}
			kingdom.AddListener(this);
		}

		// Token: 0x06004AE9 RID: 19177 RVA: 0x00222EC4 File Offset: 0x002210C4
		private void RemoveListeners()
		{
			Logic.Kingdom kingdom = this.Kingdom;
			if (kingdom == null)
			{
				return;
			}
			kingdom.DelListener(this);
		}

		// Token: 0x06004AEA RID: 19178 RVA: 0x00222ED7 File Offset: 0x002210D7
		public void OnMessage(object obj, string message, object param)
		{
			if (message == "traditions_changed")
			{
				this.m_Invalidate = true;
			}
		}

		// Token: 0x06004AEB RID: 19179 RVA: 0x00222EED File Offset: 0x002210ED
		private void OnDestroy()
		{
			this.RemoveListeners();
		}

		// Token: 0x040039C1 RID: 14785
		[UIFieldTarget("id_TraditionsIcons")]
		private GameObject m_TraditionsContainer;

		// Token: 0x040039C2 RID: 14786
		[UIFieldTarget("id_Tradition")]
		private GameObject[] m_TraditionSlotObjects;

		// Token: 0x040039C4 RID: 14788
		private List<UIDynastyTradition> m_TraditionSlots = new List<UIDynastyTradition>();

		// Token: 0x040039C5 RID: 14789
		private bool m_Initiallized;

		// Token: 0x040039C6 RID: 14790
		private bool m_Invalidate;
	}
}

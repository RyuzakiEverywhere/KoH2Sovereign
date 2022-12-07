using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000224 RID: 548
public class UIKingdomAdvantagesWindow : UIWindow, IListener
{
	// Token: 0x0600213C RID: 8508 RVA: 0x0012D9FE File Offset: 0x0012BBFE
	public override string GetDefId()
	{
		return UIKingdomAdvantagesWindow.def_id;
	}

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x0600213D RID: 8509 RVA: 0x0012DA05 File Offset: 0x0012BC05
	// (set) Token: 0x0600213E RID: 8510 RVA: 0x0012DA0D File Offset: 0x0012BC0D
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x0600213F RID: 8511 RVA: 0x0012DA18 File Offset: 0x0012BC18
	private void Init()
	{
		if (this.m_Initiazlied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.windowDef = global::Defs.GetDefField(this.GetDefId(), null);
		if (this.m_GoodIconPrototype != null)
		{
			this.m_GoodIconPrototype.gameObject.SetActive(false);
		}
		if (this.m_ButtonClose != null)
		{
			this.m_ButtonClose.onClick = new BSGButton.OnClick(this.HandleOnClose);
		}
		this.clamLabelMaterialNormal = global::Defs.GetObj<Material>(this.windowDef, "claim_vitory_button.material_normal", null);
		this.clamLabelMaterialDisabled = global::Defs.GetObj<Material>(this.windowDef, "claim_vitory_button.material_disabled", null);
		UIEndGameWindow.onShow = (Action<UIEndGameWindow>)Delegate.Combine(UIEndGameWindow.onShow, new Action<UIEndGameWindow>(this.HandleOnEndGame));
		this.m_Initiazlied = true;
	}

	// Token: 0x06002140 RID: 8512 RVA: 0x0012DAE0 File Offset: 0x0012BCE0
	public void SetObject(Logic.Kingdom kingdom)
	{
		this.Init();
		if (this.Kingdom != null)
		{
			this.Kingdom.DelListener(this);
		}
		this.Kingdom = kingdom;
		if (this.Kingdom != null)
		{
			this.Kingdom.AddListener(this);
			this.Kingdom.game.AddListener(this);
			this.Kingdom.RefreshAdvantages(true);
		}
		this.Populate();
		this.Refresh();
	}

	// Token: 0x06002141 RID: 8513 RVA: 0x0012DB4C File Offset: 0x0012BD4C
	private Vars GetGoodsTotalsVars()
	{
		int num;
		int num2;
		int val;
		int num3;
		this.GetOwnedTotalResources(out num, out num2, out val, out num3);
		Vars vars = new Vars();
		vars.Set<int>("all_count", num2);
		vars.Set<int>("produces_count", num);
		vars.Set<int>("missing_count", num2 - num);
		vars.Set<int>("imported_count", val);
		vars.Set<int>("produced_in_team_count", num3);
		vars.Set<int>("missing_in_team_count", num2 - num3);
		string currentFilter = UIKingdomAdvantagesWindow.m_CurrentFilter;
		if (!(currentFilter == "All"))
		{
			if (!(currentFilter == "Produces"))
			{
				if (!(currentFilter == "Missing"))
				{
					if (!(currentFilter == "Imported"))
					{
						if (!(currentFilter == "ProducedInTeam"))
						{
							if (currentFilter == "MissingInTeam")
							{
								vars.Set<int>("current_filter_num", num2 - num3);
							}
						}
						else
						{
							vars.Set<int>("current_filter_num", num3);
						}
					}
					else
					{
						vars.Set<int>("current_filter_num", val);
					}
				}
				else
				{
					vars.Set<int>("current_filter_num", num2 - num);
				}
			}
			else
			{
				vars.Set<int>("current_filter_num", num);
			}
		}
		else
		{
			vars.Set<int>("current_filter_num", num2);
		}
		return vars;
	}

	// Token: 0x06002142 RID: 8514 RVA: 0x0012DC7C File Offset: 0x0012BE7C
	protected UIActionIcon BuildAction(string actionKey, GameObject button)
	{
		for (int i = 0; i < this.Kingdom.actions.Count; i++)
		{
			Action action = this.Kingdom.actions[i];
			if (!(action.def.field.key != actionKey))
			{
				Vars vars = new Vars(this.Kingdom);
				UIActionIcon uiactionIcon = UIActionIcon.Possess(action.visuals as ActionVisuals, button, vars);
				if (uiactionIcon != null)
				{
					uiactionIcon.SetSkin("Neutral");
				}
				return uiactionIcon;
			}
		}
		return null;
	}

	// Token: 0x06002143 RID: 8515 RVA: 0x0012DD0C File Offset: 0x0012BF0C
	private void Populate()
	{
		this.InitAdvantages();
		if (this.m_Filter != null)
		{
			this.m_Filter.onValueChanged.AddListener(new UnityAction<int>(this.HandleFilterChange));
			this.m_Filter.ClearOptions();
			Vars goodsTotalsVars = this.GetGoodsTotalsVars();
			for (int i = 0; i < this.m_filterOptions.Length; i++)
			{
				string filter = this.m_filterOptions[i];
				if (this.IsValidFilter(filter))
				{
					this.m_Filter.options.Add(new TMP_Dropdown.OptionData(global::Defs.Localize("KingdomAdvantages.filter." + this.m_filterOptions[i], goodsTotalsVars, null, true, true)));
				}
			}
		}
		this.victoryIcon = this.BuildAction("AdvantagesClaimVictoryAction", this.m_Victory);
		if (this.m_ButtonVictory != null)
		{
			this.m_ButtonVictory.onEvent = new BSGButton.OnEvent(this.HandleVictoryClick);
		}
		this.victoryIcon.ShowIfNotActive = true;
		UIText.SetTextKey(this.m_VictoryLabel, "KingdomAdvantages.claimVictory", null, null);
		UIText.SetTextKey(this.m_Sort, "KingdomAdvantages.sort_by", null, null);
		if (this.m_ButtonVictory != null)
		{
			Tooltip.Get(this.m_ButtonVictory.gameObject, true).SetDef("AdvantagesVictoryTooltip", new Vars(this.Kingdom));
		}
		this.BuildGoods();
		this.BuildGoodsDictionary();
	}

	// Token: 0x06002144 RID: 8516 RVA: 0x0012DE64 File Offset: 0x0012C064
	private bool IsValidFilter(string filter)
	{
		return filter == "All" || filter == "Produces" || filter == "Missing" || filter == "Imported" || ((filter == "ProducedInTeam" || filter == "MissingInTeam") && this.Kingdom.game.IsMultiplayer() && !CampaignUtils.IsFFA(this.Kingdom.game.campaign));
	}

	// Token: 0x06002145 RID: 8517 RVA: 0x0012DEF1 File Offset: 0x0012C0F1
	public void HandleVictoryClick(BSGButton btn, BSGButton.Event e, PointerEventData eventData)
	{
		if (e == BSGButton.Event.Down && eventData.button == PointerEventData.InputButton.Left)
		{
			this.victoryIcon.OnClick(eventData);
		}
	}

	// Token: 0x06002146 RID: 8518 RVA: 0x0012DF0C File Offset: 0x0012C10C
	private void InitAdvantages()
	{
		this.m_Advantages.Clear();
		if (this.m_AdvantagesSlotsContainer == null)
		{
			return;
		}
		int childCount = this.m_AdvantagesSlotsContainer.transform.childCount;
		Logic.Kingdom kingdom = this.Kingdom;
		List<KingdomAdvantage> list;
		if (kingdom == null)
		{
			list = null;
		}
		else
		{
			KingdomAdvantages advantages = kingdom.advantages;
			list = ((advantages != null) ? advantages.advantages : null);
		}
		List<KingdomAdvantage> list2 = list;
		if (list2 == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < childCount; i++)
		{
			if (i >= childCount)
			{
				return;
			}
			UIKingdomAdvantage component = this.m_AdvantagesSlotsContainer.transform.GetChild(i).GetComponent<UIKingdomAdvantage>();
			if (!(component == null))
			{
				if (num >= list2.Count)
				{
					component.gameObject.SetActive(false);
				}
				else
				{
					KingdomAdvantage kingdomAdvantage = list2[num++];
					if (!kingdomAdvantage.CheckHardRequirements())
					{
						i--;
					}
					else
					{
						component.SetObject(kingdomAdvantage);
						this.m_Advantages.Add(component);
					}
				}
			}
		}
	}

	// Token: 0x06002147 RID: 8519 RVA: 0x0012DFE8 File Offset: 0x0012C1E8
	private void BuildGoods()
	{
		if (this.m_GoodIconPrototype == null)
		{
			return;
		}
		if (this.m_GoodsContainer == null)
		{
			return;
		}
		DT.Field defField = global::Defs.GetDefField("Resource", null);
		if (defField == null || defField.def == null || defField.def.defs == null)
		{
			return;
		}
		this.m_ResourcesDefs = defField.def.defs;
		UICommon.DeleteActiveChildren(this.m_GoodsContainer);
		this.m_GoodIcons.Clear();
		for (int i = 0; i < this.m_ResourcesDefs.Count; i++)
		{
			DT.Def def = this.m_ResourcesDefs[i];
			if (def != null)
			{
				GameObject gameObject = global::Common.Spawn(this.m_GoodIconPrototype, this.m_GoodsContainer, false, "");
				gameObject.SetActive(true);
				UIKingdomAdvantagesWindow.GoodsSlot orAddComponent = gameObject.GetOrAddComponent<UIKingdomAdvantagesWindow.GoodsSlot>();
				orAddComponent.SetDef(def, this.Kingdom);
				this.m_GoodIcons.Add(orAddComponent);
			}
		}
	}

	// Token: 0x06002148 RID: 8520 RVA: 0x0012E0C4 File Offset: 0x0012C2C4
	private void HanndleOnSlotHovered(UIKingdomAdvantagesWindow.GoodsSlot s, PointerEventData e)
	{
		List<UIKingdomAdvantagesWindow.GoodsSlot> list = this.goodIconsPerType[s.Def.field.key];
		for (int i = 0; i < list.Count; i++)
		{
			UIKingdomAdvantagesWindow.GoodsSlot goodsSlot = list[i];
			if (!(goodsSlot == s))
			{
				goodsSlot.mouse_in = s.mouse_in;
				goodsSlot.UpdateHighlight();
			}
		}
	}

	// Token: 0x06002149 RID: 8521 RVA: 0x0012E124 File Offset: 0x0012C324
	private void BuildGoodsDictionary(UIKingdomAdvantagesWindow.GoodsSlot gs)
	{
		gs.OnHover = new Action<UIKingdomAdvantagesWindow.GoodsSlot, PointerEventData>(this.HanndleOnSlotHovered);
		string key = gs.Def.field.key;
		List<UIKingdomAdvantagesWindow.GoodsSlot> list;
		if (!this.goodIconsPerType.TryGetValue(key, out list))
		{
			this.goodIconsPerType.Add(key, new List<UIKingdomAdvantagesWindow.GoodsSlot>(1));
		}
		this.goodIconsPerType[key].Add(gs);
	}

	// Token: 0x0600214A RID: 8522 RVA: 0x0012E188 File Offset: 0x0012C388
	private void BuildGoodsDictionary()
	{
		this.goodIconsPerType.Clear();
		for (int i = 0; i < this.m_GoodIcons.Count; i++)
		{
			UIKingdomAdvantagesWindow.GoodsSlot gs = this.m_GoodIcons[i];
			this.BuildGoodsDictionary(gs);
		}
		for (int j = 0; j < this.m_Advantages.Count; j++)
		{
			UIKingdomAdvantage uikingdomAdvantage = this.m_Advantages[j];
			for (int k = 0; k < uikingdomAdvantage.m_Requirements.Count; k++)
			{
				UIKingdomAdvantagesWindow.GoodsSlot gs2 = uikingdomAdvantage.m_Requirements[k];
				this.BuildGoodsDictionary(gs2);
			}
		}
	}

	// Token: 0x0600214B RID: 8523 RVA: 0x0012E220 File Offset: 0x0012C420
	private void GetOwnedTotalResources(out int owned, out int total, out int imported, out int teamProduces)
	{
		owned = 0;
		total = 0;
		imported = 0;
		teamProduces = 0;
		if (this.m_ResourcesDefs == null)
		{
			return;
		}
		this.Kingdom.UpdateRealmTags(false);
		total = this.m_ResourcesDefs.Count;
		Logic.Kingdom kingdom = this.Kingdom;
		List<Game.Player> list;
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
				Game.Teams teams = game.teams;
				if (teams == null)
				{
					list = null;
				}
				else
				{
					Game.Team team = teams.Get(this.Kingdom);
					list = ((team != null) ? team.players : null);
				}
			}
		}
		List<Game.Player> list2 = list;
		for (int i = 0; i < total; i++)
		{
			bool flag = false;
			if (this.Kingdom.GetRealmTag(this.m_ResourcesDefs[i].field.key) > 0)
			{
				owned++;
				teamProduces++;
				flag = true;
			}
			if (this.Kingdom.goods_imported.ContainsKey(this.m_ResourcesDefs[i].field.key))
			{
				imported++;
			}
			if (!flag && list2 != null)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					Logic.Kingdom kingdom2 = this.Kingdom.game.GetKingdom(list2[j].kingdom_id);
					if (kingdom2 != this.Kingdom && kingdom2.GetRealmTag(this.m_ResourcesDefs[i].field.key) > 0)
					{
						teamProduces++;
						break;
					}
				}
			}
		}
	}

	// Token: 0x0600214C RID: 8524 RVA: 0x0012E37C File Offset: 0x0012C57C
	private void UpdateLabels()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		Vars vars = new Vars();
		vars.Set<int>("met_advantages", this.Kingdom.advantages.NumActiveAdvantages());
		vars.Set<int>("total_advantages", this.Kingdom.advantages.MaxActiveAdvantages());
		UIText.SetTextKey(this.m_caption, "KingdomAdvantages.caption", vars, null);
		Vars goodsTotalsVars = this.GetGoodsTotalsVars();
		if (this.m_Owned != null)
		{
			UIText.SetText(this.m_Owned, global::Defs.Localize("KingdomAdvantages.owned", goodsTotalsVars, null, true, true));
		}
		if (this.m_Filter != null)
		{
			this.m_Filter.captionText.text = global::Defs.Localize("KingdomAdvantages.filter." + this.m_filterOptions[this.m_Filter.value], goodsTotalsVars, null, true, true);
			for (int i = 0; i < this.m_Filter.options.Count; i++)
			{
				this.m_Filter.options[i].text = global::Defs.Localize("KingdomAdvantages.filter." + this.m_filterOptions[i], goodsTotalsVars, null, true, true);
			}
			this.m_Filter.RefreshShownValue();
		}
	}

	// Token: 0x0600214D RID: 8525 RVA: 0x0012E4AC File Offset: 0x0012C6AC
	private void Refresh()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		this.RefreshAdvantages();
		this.ApplyFilter();
		this.UpdateVictoryButton();
		float fame = this.Kingdom.fame;
		float num = global::Defs.GetFloat("Fame", "max", null, 0f) * global::Defs.GetFloat("Fame", "min_fame_victory_perc", null, 0f) / 100f;
		for (int i = 0; i < this.m_GoodIcons.Count; i++)
		{
			this.m_GoodIcons[i].UpdateState();
		}
	}

	// Token: 0x0600214E RID: 8526 RVA: 0x0012E53C File Offset: 0x0012C73C
	private void UpdateVictoryButton()
	{
		UIActionIcon uiactionIcon = this.victoryIcon;
		bool flag = ((uiactionIcon != null) ? uiactionIcon.Data.logic.Validate(true) : null) == "ok";
		if (this.m_ButtonVictory != null)
		{
			this.m_ButtonVictory.Enable(flag, false);
		}
		if (this.clamLabelMaterialNormal != null && this.clamLabelMaterialDisabled != null && this.m_VictoryLabel != null)
		{
			this.m_VictoryLabel.fontSharedMaterial = (flag ? this.clamLabelMaterialNormal : this.clamLabelMaterialDisabled);
		}
	}

	// Token: 0x0600214F RID: 8527 RVA: 0x0012E5D4 File Offset: 0x0012C7D4
	private void RefreshAdvantages()
	{
		if (this.m_Advantages == null)
		{
			return;
		}
		for (int i = 0; i < this.m_Advantages.Count; i++)
		{
			UIKingdomAdvantage uikingdomAdvantage = this.m_Advantages[i];
			if (!(uikingdomAdvantage == null))
			{
				uikingdomAdvantage.Invalidate();
			}
		}
	}

	// Token: 0x06002150 RID: 8528 RVA: 0x0012E61C File Offset: 0x0012C81C
	private bool IsEligable(DT.Def def, string filter)
	{
		if (filter == "All")
		{
			return true;
		}
		if (filter == "Produces")
		{
			Resource.Def def2;
			return this.Kingdom.GetRealmTag(def.field.key) > 0 && !this.Kingdom.goods_imported.TryGetValue(def.field.key, out def2);
		}
		if (filter == "Missing")
		{
			return this.Kingdom.GetRealmTag(def.field.key) == 0;
		}
		if (filter == "Imported")
		{
			Resource.Def def3;
			return this.Kingdom.goods_imported.TryGetValue(def.field.key, out def3);
		}
		if (!(filter == "ProducedInTeam"))
		{
			if (!(filter == "MissingInTeam"))
			{
				return false;
			}
			Logic.Kingdom kingdom = this.Kingdom;
			List<Game.Player> list;
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
					Game.Teams teams = game.teams;
					if (teams == null)
					{
						list = null;
					}
					else
					{
						Game.Team team = teams.Get(this.Kingdom);
						list = ((team != null) ? team.players : null);
					}
				}
			}
			List<Game.Player> list2 = list;
			if (list2 == null)
			{
				return false;
			}
			for (int i = 0; i < list2.Count; i++)
			{
				if (this.Kingdom.game.GetKingdom(list2[i].kingdom_id).GetRealmTag(def.field.key) > 0)
				{
					return false;
				}
			}
			return true;
		}
		else
		{
			Logic.Kingdom kingdom2 = this.Kingdom;
			List<Game.Player> list3;
			if (kingdom2 == null)
			{
				list3 = null;
			}
			else
			{
				Game game2 = kingdom2.game;
				if (game2 == null)
				{
					list3 = null;
				}
				else
				{
					Game.Teams teams2 = game2.teams;
					if (teams2 == null)
					{
						list3 = null;
					}
					else
					{
						Game.Team team2 = teams2.Get(this.Kingdom);
						list3 = ((team2 != null) ? team2.players : null);
					}
				}
			}
			List<Game.Player> list4 = list3;
			if (list4 == null)
			{
				return false;
			}
			for (int j = 0; j < list4.Count; j++)
			{
				if (this.Kingdom.game.GetKingdom(list4[j].kingdom_id).GetRealmTag(def.field.key) > 0)
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x06002151 RID: 8529 RVA: 0x0012E810 File Offset: 0x0012CA10
	private void ApplyFilter()
	{
		if (this.m_filterOptions[this.m_Filter.value] != UIKingdomAdvantagesWindow.m_CurrentFilter)
		{
			int valueWithoutNotify = 0;
			for (int i = 0; i < this.m_filterOptions.Length; i++)
			{
				if (this.m_filterOptions[i] == UIKingdomAdvantagesWindow.m_CurrentFilter)
				{
					valueWithoutNotify = i;
					break;
				}
			}
			this.m_Filter.SetValueWithoutNotify(valueWithoutNotify);
		}
		for (int j = 0; j < this.m_GoodIcons.Count; j++)
		{
			UIKingdomAdvantagesWindow.GoodsSlot goodsSlot = this.m_GoodIcons[j];
			goodsSlot.gameObject.SetActive(this.IsEligable(goodsSlot.Def, UIKingdomAdvantagesWindow.m_CurrentFilter));
		}
		this.UpdateLabels();
	}

	// Token: 0x06002152 RID: 8530 RVA: 0x0012E8B9 File Offset: 0x0012CAB9
	private void HandleFilterChange(int index)
	{
		if (this.m_filterOptions == null)
		{
			return;
		}
		if (index < 0 || index >= this.m_filterOptions.Length)
		{
			return;
		}
		UIKingdomAdvantagesWindow.m_CurrentFilter = this.m_filterOptions[Mathf.Clamp(index, 0, this.m_filterOptions.Length - 1)];
		this.ApplyFilter();
	}

	// Token: 0x06002153 RID: 8531 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleOnClose(BSGButton bnt)
	{
		this.Close(false);
	}

	// Token: 0x06002154 RID: 8532 RVA: 0x0012E8F7 File Offset: 0x0012CAF7
	private void HandleOnEndGame(UIEndGameWindow w)
	{
		this.Close(true);
	}

	// Token: 0x06002155 RID: 8533 RVA: 0x0012E900 File Offset: 0x0012CB00
	protected override void OnDestroy()
	{
		Logic.Kingdom kingdom = this.Kingdom;
		if (kingdom != null)
		{
			kingdom.DelListener(this);
		}
		List<UIKingdomAdvantagesWindow.GoodsSlot> goodIcons = this.m_GoodIcons;
		if (goodIcons != null)
		{
			goodIcons.Clear();
		}
		base.OnDestroy();
	}

	// Token: 0x06002156 RID: 8534 RVA: 0x0012E92B File Offset: 0x0012CB2B
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "religion_changed")
		{
			this.InitAdvantages();
			this.Refresh();
			return;
		}
		if (!(message == "refresh_tags"))
		{
			return;
		}
		this.Refresh();
	}

	// Token: 0x06002157 RID: 8535 RVA: 0x0012E95B File Offset: 0x0012CB5B
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIKingdomAdvantagesWindow.def_id, null);
	}

	// Token: 0x06002158 RID: 8536 RVA: 0x0012E968 File Offset: 0x0012CB68
	public static void ToggleOpen(Logic.Kingdom kingdom)
	{
		if (kingdom == null)
		{
			if (UIKingdomAdvantagesWindow.current != null)
			{
				UIKingdomAdvantagesWindow.current.Close(false);
				UIKingdomAdvantagesWindow.current = null;
			}
			return;
		}
		if (UIKingdomAdvantagesWindow.current != null)
		{
			UIKingdomAdvantagesWindow uikingdomAdvantagesWindow = UIKingdomAdvantagesWindow.current;
			if (((uikingdomAdvantagesWindow != null) ? uikingdomAdvantagesWindow.Kingdom : null) == kingdom)
			{
				UIKingdomAdvantagesWindow.current.Close(false);
				UIKingdomAdvantagesWindow.current = null;
				return;
			}
			UIKingdomAdvantagesWindow.current.SetObject(kingdom);
			return;
		}
		else
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return;
			}
			if (UIKingdomAdvantagesWindow.GetPrefab() == null)
			{
				return;
			}
			GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
			if (gameObject != null)
			{
				UICommon.DeleteChildren(gameObject.transform, typeof(UIKingdomAdvantagesWindow));
				UIKingdomAdvantagesWindow.current = UIKingdomAdvantagesWindow.Create(kingdom, gameObject.transform as RectTransform);
			}
			return;
		}
	}

	// Token: 0x06002159 RID: 8537 RVA: 0x0012EA3C File Offset: 0x0012CC3C
	public static UIKingdomAdvantagesWindow Create(Logic.Kingdom kingdom, Transform parent)
	{
		if (kingdom == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		GameObject prefab = UIKingdomAdvantagesWindow.GetPrefab();
		if (prefab == null)
		{
			return null;
		}
		UIKingdomAdvantagesWindow orAddComponent = global::Common.Spawn(prefab, parent, false, "").GetOrAddComponent<UIKingdomAdvantagesWindow>();
		orAddComponent.SetObject(kingdom);
		orAddComponent.Open();
		DT.Field soundsDef = BaseUI.soundsDef;
		BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("open_kingdom_advantages_window", null, "", true, true, true, '.') : null, null);
		return orAddComponent;
	}

	// Token: 0x0600215A RID: 8538 RVA: 0x0012EAB0 File Offset: 0x0012CCB0
	public static bool IsActive()
	{
		return UIKingdomAdvantagesWindow.current != null;
	}

	// Token: 0x0600215B RID: 8539 RVA: 0x0012EABD File Offset: 0x0012CCBD
	public override void OnPoolDeactivated()
	{
		base.OnPoolDeactivated();
		if (UIKingdomAdvantagesWindow.current == this)
		{
			UIKingdomAdvantagesWindow.current = null;
		}
	}

	// Token: 0x04001639 RID: 5689
	private static string def_id = "KingdomAdvantagesWindow";

	// Token: 0x0400163A RID: 5690
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_caption;

	// Token: 0x0400163B RID: 5691
	[UIFieldTarget("id_OwnedGoods")]
	private TextMeshProUGUI m_Owned;

	// Token: 0x0400163C RID: 5692
	[UIFieldTarget("id_KingdomAdvantage")]
	private GameObject m_AdvantagesSlotsContainer;

	// Token: 0x0400163D RID: 5693
	[UIFieldTarget("id_Button_Close")]
	private BSGButton m_ButtonClose;

	// Token: 0x0400163E RID: 5694
	[UIFieldTarget("id_Victory")]
	private GameObject m_Victory;

	// Token: 0x0400163F RID: 5695
	[UIFieldTarget("id_ButtonVictory")]
	private BSGButton m_ButtonVictory;

	// Token: 0x04001640 RID: 5696
	[UIFieldTarget("id_LabelVictory")]
	private TextMeshProUGUI m_VictoryLabel;

	// Token: 0x04001641 RID: 5697
	[UIFieldTarget("id_GoodsContainer")]
	private RectTransform m_GoodsContainer;

	// Token: 0x04001642 RID: 5698
	[UIFieldTarget("id_GoodIconPrototype")]
	private GameObject m_GoodIconPrototype;

	// Token: 0x04001643 RID: 5699
	[UIFieldTarget("id_Filter")]
	private TMP_Dropdown m_Filter;

	// Token: 0x04001644 RID: 5700
	[UIFieldTarget("id_Sort")]
	private TextMeshProUGUI m_Sort;

	// Token: 0x04001645 RID: 5701
	private UIActionIcon victoryIcon;

	// Token: 0x04001647 RID: 5703
	private List<DT.Def> m_ResourcesDefs;

	// Token: 0x04001648 RID: 5704
	private List<UIKingdomAdvantage> m_Advantages = new List<UIKingdomAdvantage>();

	// Token: 0x04001649 RID: 5705
	private List<UIKingdomAdvantagesWindow.GoodsSlot> m_GoodIcons = new List<UIKingdomAdvantagesWindow.GoodsSlot>();

	// Token: 0x0400164A RID: 5706
	private Dictionary<string, List<UIKingdomAdvantagesWindow.GoodsSlot>> goodIconsPerType = new Dictionary<string, List<UIKingdomAdvantagesWindow.GoodsSlot>>();

	// Token: 0x0400164B RID: 5707
	private static string m_CurrentFilter = "All";

	// Token: 0x0400164C RID: 5708
	private string[] m_filterOptions = new string[]
	{
		"All",
		"Produces",
		"Missing",
		"Imported",
		"ProducedInTeam",
		"MissingInTeam"
	};

	// Token: 0x0400164D RID: 5709
	private DT.Field windowDef;

	// Token: 0x0400164E RID: 5710
	private Material clamLabelMaterialNormal;

	// Token: 0x0400164F RID: 5711
	private Material clamLabelMaterialDisabled;

	// Token: 0x04001650 RID: 5712
	private bool m_Initiazlied;

	// Token: 0x04001651 RID: 5713
	private static UIKingdomAdvantagesWindow current;

	// Token: 0x02000774 RID: 1908
	public class GoodsSlot : Hotspot
	{
		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x06004C12 RID: 19474 RVA: 0x0022877B File Offset: 0x0022697B
		// (set) Token: 0x06004C13 RID: 19475 RVA: 0x00228783 File Offset: 0x00226983
		public DT.Def Def { get; private set; }

		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x06004C14 RID: 19476 RVA: 0x0022878C File Offset: 0x0022698C
		// (set) Token: 0x06004C15 RID: 19477 RVA: 0x00228794 File Offset: 0x00226994
		public Logic.Kingdom Kingdom { get; private set; }

		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x06004C16 RID: 19478 RVA: 0x0022879D File Offset: 0x0022699D
		// (set) Token: 0x06004C17 RID: 19479 RVA: 0x002287A5 File Offset: 0x002269A5
		public UIKingdomAdvantagesWindow.GoodsSlot.State state { get; private set; }

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x06004C18 RID: 19480 RVA: 0x002287AE File Offset: 0x002269AE
		// (set) Token: 0x06004C19 RID: 19481 RVA: 0x002287B6 File Offset: 0x002269B6
		public DT.Field state_def { get; private set; }

		// Token: 0x06004C1A RID: 19482 RVA: 0x002287BF File Offset: 0x002269BF
		private void Init()
		{
			if (this.m_Initialzed)
			{
				return;
			}
			this.ui_def = global::Defs.GetDefField("ResourcesSlot", null);
			UICommon.FindComponents(this, false);
			this.m_Initialzed = true;
		}

		// Token: 0x06004C1B RID: 19483 RVA: 0x002287EC File Offset: 0x002269EC
		public void SetDef(DT.Def def, Logic.Kingdom kingodm)
		{
			this.Init();
			this.Def = def;
			this.Kingdom = kingodm;
			if (this.m_ResourceIcon != null)
			{
				this.m_ResourceIcon.SetObject(this.Def, null);
			}
			this.UpdateState();
			this.UpdateHighlight();
		}

		// Token: 0x06004C1C RID: 19484 RVA: 0x00228839 File Offset: 0x00226A39
		public void SetKingdom(Logic.Kingdom kingodm)
		{
			this.Kingdom = kingodm;
			this.UpdateState();
			this.UpdateHighlight();
		}

		// Token: 0x06004C1D RID: 19485 RVA: 0x00228850 File Offset: 0x00226A50
		public void SetState(UIKingdomAdvantagesWindow.GoodsSlot.State state)
		{
			if (this.state == state && this.state_def != null)
			{
				return;
			}
			this.state = state;
			if (this.ui_def == null)
			{
				this.ui_def = global::Defs.GetDefField("ResourcesSlot", null);
			}
			if (this.ui_def != null)
			{
				this.state_def = this.ui_def.FindChild(state.ToString(), null, true, true, true, '.');
				if (this.state_def == null)
				{
					Debug.LogWarning(string.Format("{0}: undefined state '{1}'", this, state));
					this.state_def = this.ui_def.FindChild("State", null, true, true, true, '.');
					return;
				}
			}
			else
			{
				this.state_def = null;
			}
		}

		// Token: 0x06004C1E RID: 19486 RVA: 0x00228900 File Offset: 0x00226B00
		public UIKingdomAdvantagesWindow.GoodsSlot.State DecideState()
		{
			if (this.Def == null)
			{
				return UIKingdomAdvantagesWindow.GoodsSlot.State.Available;
			}
			ResourceInfo resourceInfo = this.Kingdom.GetResourceInfo(this.Def.field.key, true, true);
			if (resourceInfo != null)
			{
				ResourceInfo.Availability own_availability = resourceInfo.own_availability;
				if (resourceInfo.availability == ResourceInfo.Availability.DirectlyObtainable)
				{
					return UIKingdomAdvantagesWindow.GoodsSlot.State.CanProduce;
				}
				if (resourceInfo.availability == ResourceInfo.Availability.IndirectlyObtainable)
				{
					return UIKingdomAdvantagesWindow.GoodsSlot.State.CanProduce;
				}
				if (resourceInfo.availability == ResourceInfo.Availability.Available && own_availability != ResourceInfo.Availability.Available)
				{
					return UIKingdomAdvantagesWindow.GoodsSlot.State.Imported;
				}
			}
			Logic.Kingdom kingdom = this.Kingdom;
			if (((kingdom != null) ? kingdom.GetRealmTag(this.Def.field.key) : 0) <= 0)
			{
				return UIKingdomAdvantagesWindow.GoodsSlot.State.Missing;
			}
			return UIKingdomAdvantagesWindow.GoodsSlot.State.Available;
		}

		// Token: 0x06004C1F RID: 19487 RVA: 0x0022898C File Offset: 0x00226B8C
		public void UpdateState()
		{
			UIKingdomAdvantagesWindow.GoodsSlot.State state = this.DecideState();
			this.SetState(state);
			this.UpdateVisualState();
		}

		// Token: 0x06004C20 RID: 19488 RVA: 0x002289B0 File Offset: 0x00226BB0
		private void UpdateVisualState()
		{
			if (this.state_def == null)
			{
				return;
			}
			if (this.m_Icon != null)
			{
				this.m_Icon.color = global::Defs.GetColor(this.state_def, "icon_color", null);
			}
			if (this.m_Available != null)
			{
				this.m_Available.gameObject.SetActive(this.state_def.GetBool("show_available_border", null, false, true, true, true, '.'));
			}
			if (this.m_CanProduce != null)
			{
				this.m_CanProduce.gameObject.SetActive(this.state_def.GetBool("show_can_produce_border", null, false, true, true, true, '.'));
			}
			if (this.m_Imported != null)
			{
				this.m_Imported.gameObject.SetActive(this.state_def.GetBool("show_imported", null, false, true, true, true, '.'));
			}
		}

		// Token: 0x06004C21 RID: 19489 RVA: 0x00228A8F File Offset: 0x00226C8F
		public void UpdateHighlight()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			GameObject focused = this.m_Focused;
			if (focused == null)
			{
				return;
			}
			focused.SetActive(this.mouse_in);
		}

		// Token: 0x06004C22 RID: 19490 RVA: 0x00228AAF File Offset: 0x00226CAF
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			this.UpdateHighlight();
			Action<UIKingdomAdvantagesWindow.GoodsSlot, PointerEventData> onHover = this.OnHover;
			if (onHover == null)
			{
				return;
			}
			onHover(this, eventData);
		}

		// Token: 0x06004C23 RID: 19491 RVA: 0x00228AD0 File Offset: 0x00226CD0
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this.UpdateHighlight();
			Action<UIKingdomAdvantagesWindow.GoodsSlot, PointerEventData> onHover = this.OnHover;
			if (onHover == null)
			{
				return;
			}
			onHover(this, eventData);
		}

		// Token: 0x06004C24 RID: 19492 RVA: 0x00228AF1 File Offset: 0x00226CF1
		private void Clear()
		{
			this.Def = null;
			this.Kingdom = null;
		}

		// Token: 0x04003AA9 RID: 15017
		[UIFieldTarget("id_ResourceIcon")]
		private UIGoodsIcon m_ResourceIcon;

		// Token: 0x04003AAA RID: 15018
		[UIFieldTarget("id_Icon")]
		private Image m_Icon;

		// Token: 0x04003AAB RID: 15019
		[UIFieldTarget("id_Available")]
		private GameObject m_Available;

		// Token: 0x04003AAC RID: 15020
		[UIFieldTarget("id_Focused")]
		private GameObject m_Focused;

		// Token: 0x04003AAD RID: 15021
		[UIFieldTarget("id_CanProduce")]
		private GameObject m_CanProduce;

		// Token: 0x04003AAE RID: 15022
		[UIFieldTarget("id_Imported")]
		private GameObject m_Imported;

		// Token: 0x04003AB1 RID: 15025
		[HideInInspector]
		public DT.Field ui_def;

		// Token: 0x04003AB4 RID: 15028
		public Action<UIKingdomAdvantagesWindow.GoodsSlot, PointerEventData> OnHover;

		// Token: 0x04003AB5 RID: 15029
		private bool m_Initialzed;

		// Token: 0x02000A19 RID: 2585
		public enum State
		{
			// Token: 0x0400467D RID: 18045
			Available,
			// Token: 0x0400467E RID: 18046
			Imported,
			// Token: 0x0400467F RID: 18047
			CanProduce,
			// Token: 0x04004680 RID: 18048
			Missing
		}
	}
}

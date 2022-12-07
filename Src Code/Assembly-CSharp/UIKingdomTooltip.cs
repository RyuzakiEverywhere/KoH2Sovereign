using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Logic;
using Logic.ExtensionMethods;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000213 RID: 531
public class UIKingdomTooltip : MonoBehaviour, Tooltip.IHandler
{
	// Token: 0x06002027 RID: 8231 RVA: 0x001274EC File Offset: 0x001256EC
	private void Init()
	{
		if (this.m_Initialzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.def = global::Defs.GetDefField("KingdomTooltip", null);
		if (this.m_RelationBarContainer != null)
		{
			this.m_RelationBar = this.m_RelationBarContainer.AddComponent<UIKingdomTooltip.RelationsBar>();
		}
		if (this.m_StancesContainer != null)
		{
			this.m_Stances = this.m_StancesContainer.AddComponent<UIKingdomTooltip.Stances>();
		}
		this.m_Initialzed = true;
	}

	// Token: 0x06002028 RID: 8232 RVA: 0x00127560 File Offset: 0x00125760
	public bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		Logic.Kingdom kingdom = null;
		if (tooltip.vars != null)
		{
			kingdom = tooltip.vars.obj.Get<Logic.Kingdom>();
		}
		return this.HandleTooltip(kingdom, tooltip.vars, ui, evt);
	}

	// Token: 0x06002029 RID: 8233 RVA: 0x00127598 File Offset: 0x00125798
	public bool HandleTooltip(Logic.Kingdom kingdom, Vars vars, BaseUI ui, Tooltip.Event evt)
	{
		this.Init();
		if (evt != Tooltip.Event.Fill && evt != Tooltip.Event.Update)
		{
			return false;
		}
		if (kingdom == null)
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
		this.SetKingdom(kingdom, vars);
		this.Refresh(evt != Tooltip.Event.Update);
		return true;
	}

	// Token: 0x0600202A RID: 8234 RVA: 0x001275F8 File Offset: 0x001257F8
	private void SetKingdom(Logic.Kingdom kingdom, Vars vars = null)
	{
		if (this.Data == kingdom)
		{
			return;
		}
		this.Data = kingdom;
		this.vars = vars;
		if (vars == null)
		{
			vars = new Vars(this.Data);
		}
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		bool active = kingdom2 != null && !kingdom2.IsDefeated() && this.Data.type == Logic.Kingdom.Type.Regular && kingdom2 != this.Data && !this.Data.IsDefeated();
		Logic.Multiplayer.PlayerData playerData = (!kingdom.is_local_player) ? Logic.Multiplayer.CurrentPlayers.GetByKingdom(kingdom.id) : null;
		this.FillVars(vars, playerData);
		if (this.m_RelationBar != null)
		{
			this.m_RelationBar.SetKingdom(this.Data);
			this.m_RelationBar.gameObject.SetActive(this.Data.type == Logic.Kingdom.Type.Regular && kingdom2 != null && !kingdom2.IsDefeated() && kingdom2 != null && this.Data != kingdom2);
		}
		if (this.m_Stances != null)
		{
			this.m_Stances.SetKingdom(this.Data);
		}
		if (this.m_StancesContainer != null)
		{
			this.m_StancesContainer.gameObject.SetActive(active);
		}
		if (this.m_StancesSpacer != null)
		{
			this.m_StancesSpacer.gameObject.SetActive(kingdom2 != null);
		}
		if (this.m_Religion != null)
		{
			this.m_Religion.SetData(this.Data);
			this.m_Religion.gameObject.SetActive(this.Data.type == Logic.Kingdom.Type.Regular);
		}
		if (this.m_SpacingHack != null)
		{
			this.m_SpacingHack.gameObject.SetActive(this.Data.type > Logic.Kingdom.Type.Regular);
		}
		if (this.m_HeaderDescription != null)
		{
			UIText.SetText(this.m_HeaderDescription, this.def, this.GetKingdomSubTitleKey(), vars, null);
		}
		if (this.m_Description != null)
		{
			string key;
			bool descriptionKey = this.GetDescriptionKey(out key);
			if (descriptionKey)
			{
				UIText.SetText(this.m_Description, this.def, key, vars, null);
			}
			this.m_Description.gameObject.SetActive(descriptionKey);
		}
		if (this.m_PlayerDescription != null)
		{
			bool flag = playerData != null;
			this.m_PlayerDescription.gameObject.SetActive(flag);
			if (flag)
			{
				UIText.SetText(this.m_PlayerDescription, this.def, "player_description", vars, null);
			}
		}
		if (this.m_PlayerDescriptionSpacer != null)
		{
			this.m_PlayerDescriptionSpacer.SetActive(playerData != null);
		}
	}

	// Token: 0x0600202B RID: 8235 RVA: 0x00127874 File Offset: 0x00125A74
	private void FillVars(Vars vars, Logic.Multiplayer.PlayerData playerData)
	{
		if (vars == null)
		{
			return;
		}
		Game game = GameLogic.Get(true);
		Crusade crusade = game.religions.catholic.crusade;
		if (crusade != null)
		{
			vars.Set<Crusade>("crusade", game.religions.catholic.crusade);
			vars.Set<Logic.Character>("owner", crusade.leader);
			vars.Set<Logic.Army>("army", crusade.leader.GetArmy());
		}
		Logic.Character character = vars.Get<Logic.Character>("character", null);
		if (character == null)
		{
			vars.obj.Get<Logic.Character>();
		}
		if (character != null)
		{
			vars.Set<Logic.Character>("character", character);
		}
		Mercenary mercenary = vars.Get<Mercenary>("mercenary", null);
		if (mercenary == null)
		{
			Mercenary mercenary2;
			if (character == null)
			{
				mercenary2 = null;
			}
			else
			{
				Logic.Army army = character.GetArmy();
				mercenary2 = ((army != null) ? army.mercenary : null);
			}
			mercenary = mercenary2;
		}
		if (mercenary != null)
		{
			vars.Set<Mercenary>("mercenary", mercenary);
		}
		Rebellion rebellion = vars.Get<Rebellion>("rebellion", null);
		if (rebellion == null)
		{
			Rebellion rebellion2;
			if (character == null)
			{
				rebellion2 = null;
			}
			else
			{
				Logic.Army army2 = character.GetArmy();
				if (army2 == null)
				{
					rebellion2 = null;
				}
				else
				{
					Logic.Rebel rebel = army2.rebel;
					rebellion2 = ((rebel != null) ? rebel.rebellion : null);
				}
			}
			rebellion = rebellion2;
		}
		if (rebellion == null)
		{
			rebellion = vars.obj.Get<Rebellion>();
		}
		if (rebellion != null)
		{
			vars.Set<Rebellion>("rebellion", rebellion);
		}
		if (playerData != null)
		{
			vars.Set<string>("player_name", "#" + UIMultiplayerUtils.ExtractPlayerName(playerData.id));
			if (!CampaignUtils.IsFFA(game.campaign))
			{
				vars.Set<Game.Team>("team", game.teams[playerData.team]);
				vars.Set<string>("team_color", "#" + ColorUtility.ToHtmlStringRGB(global::Defs.GetColor(playerData.team, "LobbyPlayerWindow", "team_color")));
			}
		}
	}

	// Token: 0x0600202C RID: 8236 RVA: 0x00127A1C File Offset: 0x00125C1C
	private void Update()
	{
		if (!this.Validate())
		{
			TooltipInstance component = base.GetComponent<TooltipInstance>();
			if (component != null)
			{
				component.Close(null);
			}
			return;
		}
	}

	// Token: 0x0600202D RID: 8237 RVA: 0x00127A4C File Offset: 0x00125C4C
	private bool Validate()
	{
		if (this.vars != null)
		{
			Mercenary mercenary = this.vars.Get("mercenary", true).obj_val as Mercenary;
			if (mercenary != null && !mercenary.IsValid())
			{
				return false;
			}
			Rebellion rebellion = this.vars.Get("rebellion", true).obj_val as Rebellion;
			if (rebellion != null && !rebellion.IsValid())
			{
				return false;
			}
			if (this.Data.type == Logic.Kingdom.Type.Crusade)
			{
				Game game = GameLogic.Get(true);
				Crusade crusade;
				if (game == null)
				{
					crusade = null;
				}
				else
				{
					Logic.Religions religions = game.religions;
					if (religions == null)
					{
						crusade = null;
					}
					else
					{
						Catholic catholic = religions.catholic;
						crusade = ((catholic != null) ? catholic.crusade : null);
					}
				}
				Crusade crusade2 = crusade;
				if (crusade2 == null || !crusade2.IsValid())
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x0600202E RID: 8238 RVA: 0x00127B00 File Offset: 0x00125D00
	private string GetKingdomTitleKey()
	{
		if (this.Data.type == Logic.Kingdom.Type.Regular)
		{
			return "kingdom_name";
		}
		if (this.Data.type == Logic.Kingdom.Type.Crusade)
		{
			return "crusade_name";
		}
		if (this.Data.Name == "MercenaryFaction")
		{
			Vars vars = this.vars;
			Mercenary mercenary = (vars != null) ? vars.Get<Mercenary>("mercenary", null) : null;
			if (mercenary == null)
			{
				return "generic_mercenary_name";
			}
			if (mercenary.army.IsHeadless())
			{
				return "abandoned_army_name";
			}
			return "mercenaries_name";
		}
		else
		{
			if (this.Data.type != Logic.Kingdom.Type.RebelFaction && this.Data.type != Logic.Kingdom.Type.LoyalistsFaction)
			{
				return "kingdom_name";
			}
			Vars vars2 = this.vars;
			if (((vars2 != null) ? vars2.Get<Rebellion>("rebellion", null) : null) != null)
			{
				return "rebellion_name";
			}
			return "generic_rebellion_name";
		}
	}

	// Token: 0x0600202F RID: 8239 RVA: 0x00127BD0 File Offset: 0x00125DD0
	private string GetKingdomSubTitleKey()
	{
		if (this.Data.Name == "MercenaryFaction")
		{
			Vars vars = this.vars;
			Mercenary mercenary = (vars != null) ? vars.Get<Mercenary>("mercenary", null) : null;
			if (mercenary == null)
			{
				return "generic_mercenary_subtitle";
			}
			if (mercenary.army.IsHeadless())
			{
				return "abandoned_army_subtitle";
			}
			if (mercenary.IsHired())
			{
				return "mercenaries_hired_subtitle";
			}
			return "mercenaries_regular_subtitle";
		}
		else
		{
			if (this.Data.type == Logic.Kingdom.Type.Crusade)
			{
				return "crusade_subtitle";
			}
			if (this.Data.IsDefeated() && this.Data.type == Logic.Kingdom.Type.Regular && GameLogic.Get(true).campaign.state == Campaign.State.Started)
			{
				return "destroyed_kingdom_subtitle";
			}
			if (this.Data.type == Logic.Kingdom.Type.RebelFaction || this.Data.type == Logic.Kingdom.Type.LoyalistsFaction)
			{
				Vars vars2 = this.vars;
				if (((vars2 != null) ? vars2.Get<Rebellion>("rebellion", null) : null) != null)
				{
					return "rebellion_subtitle";
				}
				return "generic_rebellion_subtitle";
			}
			else
			{
				if (this.Data.type == Logic.Kingdom.Type.Exile)
				{
					return "exlie_subtitle";
				}
				return "own_and_foreign_kingdom_subtitle";
			}
		}
	}

	// Token: 0x06002030 RID: 8240 RVA: 0x00127CE0 File Offset: 0x00125EE0
	private bool GetDescriptionKey(out string key)
	{
		key = null;
		if (this.Data == null)
		{
			return false;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null || kingdom.IsDefeated())
		{
			return false;
		}
		if (this.Data == BaseUI.LogicKingdom())
		{
			key = "own_kingdom_flavor";
			return true;
		}
		if (this.Data.type == Logic.Kingdom.Type.Regular && this.Data.IsDefeated())
		{
			key = "destroyed_kingdom_flavor";
			return true;
		}
		if (this.Data.type == Logic.Kingdom.Type.RebelFaction)
		{
			Vars vars = this.vars;
			if (((vars != null) ? vars.Get<Rebellion>("rebellion", null) : null) != null)
			{
				key = "rebellion_flavor";
				return true;
			}
			key = "generic_rebellion_flavor";
			return true;
		}
		else
		{
			if (this.Data.Name == "MercenaryFaction")
			{
				Vars vars2 = this.vars;
				Mercenary mercenary = (vars2 != null) ? vars2.Get<Mercenary>("mercenary", null) : null;
				if (mercenary != null)
				{
					if (mercenary.army.IsHeadless())
					{
						key = "abandoned_army_flavor";
						return true;
					}
					if (mercenary.IsHired())
					{
						key = "mercenaries_hired_flavor";
						return true;
					}
				}
				key = "mercenaries_regular_flavor";
				return true;
			}
			if (this.Data.type == Logic.Kingdom.Type.Crusade)
			{
				key = "crusade_flavor";
				return true;
			}
			if (this.Data.type == Logic.Kingdom.Type.Regular && !this.HasActiveRelations())
			{
				key = "no_active_stances_flavor";
				return true;
			}
			if (this.Data.type == Logic.Kingdom.Type.Exile)
			{
				key = "exile_flavor";
				return true;
			}
			return false;
		}
	}

	// Token: 0x06002031 RID: 8241 RVA: 0x00127E30 File Offset: 0x00126030
	private void Refresh(bool full = true)
	{
		this.Init();
		if (full)
		{
			this.PoulateHeader();
		}
		this.UpdateStanceColor();
	}

	// Token: 0x06002032 RID: 8242 RVA: 0x00127E48 File Offset: 0x00126048
	private void PoulateHeader()
	{
		if (this.m_Crest != null)
		{
			this.m_Crest.SetObject(this.Data, null);
			KingdomShield primary = this.m_Crest.GetPrimary();
			if (primary != null)
			{
				primary.DisbaleTooltip(true);
			}
		}
		if (this.m_KingdomName != null)
		{
			UIText.SetText(this.m_KingdomName, this.def, this.GetKingdomTitleKey(), this.vars, null);
		}
	}

	// Token: 0x06002033 RID: 8243 RVA: 0x00127EB8 File Offset: 0x001260B8
	private void UpdateStanceColor()
	{
		UIKingdomTooltip.<>c__DisplayClass30_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (this.m_StanceColor == null)
		{
			return;
		}
		CS$<>8__locals1.playerKingdom = BaseUI.LogicKingdom();
		if (CS$<>8__locals1.playerKingdom == null)
		{
			return;
		}
		this.m_StanceColor.color = this.<UpdateStanceColor>g__GetColor|30_0(ref CS$<>8__locals1);
	}

	// Token: 0x06002034 RID: 8244 RVA: 0x00127F04 File Offset: 0x00126104
	private bool HasActiveRelations()
	{
		if (this.Data == null)
		{
			return false;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		return kingdom != null && (kingdom.HasStance(this.Data, RelationUtils.Stance.Alliance) || kingdom.HasStance(this.Data, RelationUtils.Stance.AnyVassalage) || kingdom.HasStance(this.Data, RelationUtils.Stance.NonAggression) || kingdom.HasStance(this.Data, RelationUtils.Stance.Marriage) || kingdom.HasStance(this.Data, RelationUtils.Stance.Trade) || kingdom.HasStance(this.Data, RelationUtils.Stance.War));
	}

	// Token: 0x06002036 RID: 8246 RVA: 0x00127F98 File Offset: 0x00126198
	[CompilerGenerated]
	private Color <UpdateStanceColor>g__GetColor|30_0(ref UIKingdomTooltip.<>c__DisplayClass30_0 A_1)
	{
		if (this.Data == A_1.playerKingdom)
		{
			return global::Defs.GetColor(this.def, "stance_colors.own", null);
		}
		if (this.Data.type == Logic.Kingdom.Type.RebelFaction)
		{
			return global::Defs.GetColor(this.def, "stance_colors.rebel", null);
		}
		if (this.Data.Name == "MercenaryFaction")
		{
			return global::Defs.GetColor(this.def, "stance_colors.mercenary", null);
		}
		if (this.Data.IsDefeated())
		{
			return global::Defs.GetColor(this.def, "stance_colors.defeated", null);
		}
		if (A_1.playerKingdom.IsEnemy(this.Data))
		{
			return global::Defs.GetColor(this.def, "stance_colors.enemy", null);
		}
		if (A_1.playerKingdom.HasStance(this.Data, RelationUtils.Stance.Alliance))
		{
			return global::Defs.GetColor(this.def, "stance_colors.ally", null);
		}
		if (A_1.playerKingdom.IsFriend(this.Data))
		{
			return global::Defs.GetColor(this.def, "stance_colors.friend", null);
		}
		return global::Defs.GetColor(this.def, "stance_colors.neutral", null);
	}

	// Token: 0x04001552 RID: 5458
	[UIFieldTarget("id_StanceColor")]
	private Image m_StanceColor;

	// Token: 0x04001553 RID: 5459
	[UIFieldTarget("id_Crest")]
	private UIKingdomIcon m_Crest;

	// Token: 0x04001554 RID: 5460
	[UIFieldTarget("id_SpacingHack")]
	private GameObject m_SpacingHack;

	// Token: 0x04001555 RID: 5461
	[UIFieldTarget("id_KingdomName")]
	private TextMeshProUGUI m_KingdomName;

	// Token: 0x04001556 RID: 5462
	[UIFieldTarget("id_HeaderDescription")]
	private TextMeshProUGUI m_HeaderDescription;

	// Token: 0x04001557 RID: 5463
	[UIFieldTarget("id_RelationBar")]
	private GameObject m_RelationBarContainer;

	// Token: 0x04001558 RID: 5464
	[UIFieldTarget("id_Religion")]
	private UIReligion m_Religion;

	// Token: 0x04001559 RID: 5465
	[UIFieldTarget("id_StancesSpacer")]
	private GameObject m_StancesSpacer;

	// Token: 0x0400155A RID: 5466
	[UIFieldTarget("id_StancesContainer")]
	private GameObject m_StancesContainer;

	// Token: 0x0400155B RID: 5467
	[UIFieldTarget("id_Description")]
	private TextMeshProUGUI m_Description;

	// Token: 0x0400155C RID: 5468
	[UIFieldTarget("id_PlayerDescription")]
	private TextMeshProUGUI m_PlayerDescription;

	// Token: 0x0400155D RID: 5469
	[UIFieldTarget("id_PlayerDescriptionSpacer")]
	private GameObject m_PlayerDescriptionSpacer;

	// Token: 0x0400155E RID: 5470
	private Logic.Kingdom Data;

	// Token: 0x0400155F RID: 5471
	private DT.Field def;

	// Token: 0x04001560 RID: 5472
	private UIKingdomTooltip.RelationsBar m_RelationBar;

	// Token: 0x04001561 RID: 5473
	private UIKingdomTooltip.Stances m_Stances;

	// Token: 0x04001562 RID: 5474
	private Vars vars;

	// Token: 0x04001563 RID: 5475
	private bool m_Initialzed;

	// Token: 0x0200074C RID: 1868
	internal class Stances : MonoBehaviour
	{
		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x06004A7F RID: 19071 RVA: 0x00220CC9 File Offset: 0x0021EEC9
		// (set) Token: 0x06004A80 RID: 19072 RVA: 0x00220CD1 File Offset: 0x0021EED1
		public Logic.Kingdom Data { get; private set; }

		// Token: 0x06004A81 RID: 19073 RVA: 0x00220CDC File Offset: 0x0021EEDC
		private void Init()
		{
			if (this.m_Initilazed)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.vars = new Vars();
			if (this.m_IconPrototype != null)
			{
				this.m_IconPrototype.gameObject.SetActive(false);
			}
			this.m_Initilazed = true;
		}

		// Token: 0x06004A82 RID: 19074 RVA: 0x00220D2C File Offset: 0x0021EF2C
		public void SetKingdom(Logic.Kingdom kingdom)
		{
			this.Init();
			this.Data = kingdom;
			this.vars.Clear();
			Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
			this.vars.Set<Logic.Kingdom>("kingdom", kingdom2);
			this.vars.Set<Logic.Kingdom>("target_kigdom", this.Data);
			if (this.Data.type != Logic.Kingdom.Type.Regular || this.Data == kingdom2)
			{
				base.gameObject.SetActive(false);
				return;
			}
			base.gameObject.SetActive(true);
			this.BuildIcons();
			this.Refresh();
		}

		// Token: 0x06004A83 RID: 19075 RVA: 0x00220DBC File Offset: 0x0021EFBC
		private void BuildIcons()
		{
			if (this.icons.Count > 0)
			{
				return;
			}
			this.<BuildIcons>g__AddIcon|15_0("war_stance");
			this.<BuildIcons>g__AddIcon|15_0("vassalage");
			this.<BuildIcons>g__AddIcon|15_0("non-aggression");
			this.<BuildIcons>g__AddIcon|15_0("alliance");
			this.<BuildIcons>g__AddIcon|15_0("marriage");
			this.<BuildIcons>g__AddIcon|15_0("trade");
		}

		// Token: 0x06004A84 RID: 19076 RVA: 0x00220E1A File Offset: 0x0021F01A
		private void Update()
		{
			this.Refresh();
		}

		// Token: 0x06004A85 RID: 19077 RVA: 0x00220E22 File Offset: 0x0021F022
		private void Refresh()
		{
			this.UpdateWarStance();
			this.UpdateNonAggression();
			this.UpdateAlliance();
			this.UpdateVassalage();
			this.UpdateMarriage();
			this.UpdateTrade();
			this.UpdatePacts();
		}

		// Token: 0x06004A86 RID: 19078 RVA: 0x00220E50 File Offset: 0x0021F050
		private void UpdateWarStance()
		{
			UIKingdomTooltip.Stances.Icon icon;
			if (!this.icons.TryGetValue("war_stance", out icon))
			{
				return;
			}
			if (this.Data == null)
			{
				return;
			}
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (this.Data.IsDefeated() || kingdom == null || this.Data == kingdom)
			{
				icon.gameObject.SetActive(false);
				return;
			}
			icon.gameObject.SetActive(true);
			RelationUtils.Stance warStance = kingdom.GetWarStance(this.Data);
			string str;
			if (warStance.IsPeace() && kingdom.IsInTruceWith(this.Data))
			{
				str = "Truce";
			}
			else if (warStance.IsAlliance())
			{
				str = "Peace";
			}
			else
			{
				str = warStance.ToString();
			}
			string key = "Stance." + str + ".icon";
			string key2 = "Kingdom.Stance." + str + ".short_description";
			icon.GetIcon().overrideSprite = global::Defs.GetObj<Sprite>("Kingdom", key, null);
			UIText.SetTextKey(icon.GetDescription(), key2, null, null);
		}

		// Token: 0x06004A87 RID: 19079 RVA: 0x00220F48 File Offset: 0x0021F148
		private void UpdateVassalage()
		{
			UIKingdomTooltip.Stances.Icon icon;
			if (!this.icons.TryGetValue("vassalage", out icon))
			{
				return;
			}
			if (this.Data == null)
			{
				return;
			}
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom == null || this.Data == kingdom)
			{
				icon.gameObject.SetActive(false);
				return;
			}
			bool flag = kingdom.HasStance(this.Data, RelationUtils.Stance.Vassal);
			bool flag2 = kingdom.HasStance(this.Data, RelationUtils.Stance.Sovereign);
			if (!flag && !flag2)
			{
				icon.gameObject.SetActive(false);
				return;
			}
			icon.gameObject.SetActive(true);
			string key = (!flag) ? "Stance.Vassalage.Vassal.icon" : "Stance.Vassalage.Liege.icon";
			string key2 = (!flag) ? "Kingdom.Stance.Vassalage.Vassal.short_description" : "Kingdom.Stance.Vassalage.Liege.short_description";
			icon.GetIcon().overrideSprite = global::Defs.GetObj<Sprite>("Kingdom", key, null);
			UIText.SetTextKey(icon.GetDescription(), key2, null, null);
		}

		// Token: 0x06004A88 RID: 19080 RVA: 0x00221018 File Offset: 0x0021F218
		private void UpdateNonAggression()
		{
			UIKingdomTooltip.Stances.Icon icon;
			if (!this.icons.TryGetValue("non-aggression", out icon))
			{
				return;
			}
			if (this.Data == null)
			{
				return;
			}
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom == null || this.Data == kingdom)
			{
				icon.gameObject.SetActive(false);
				return;
			}
			if (!kingdom.HasStance(this.Data, RelationUtils.Stance.NonAggression))
			{
				icon.gameObject.SetActive(false);
				return;
			}
			icon.gameObject.SetActive(true);
			string key = "Stance.NonAggression.icon";
			string key2 = "Kingdom.Stance.NonAggression.short_description";
			icon.GetIcon().overrideSprite = global::Defs.GetObj<Sprite>("Kingdom", key, null);
			UIText.SetTextKey(icon.GetDescription(), key2, null, null);
		}

		// Token: 0x06004A89 RID: 19081 RVA: 0x002210BC File Offset: 0x0021F2BC
		private void UpdateAlliance()
		{
			UIKingdomTooltip.Stances.Icon icon;
			if (!this.icons.TryGetValue("alliance", out icon))
			{
				return;
			}
			if (this.Data == null)
			{
				return;
			}
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom == null || this.Data == kingdom)
			{
				icon.gameObject.SetActive(false);
				return;
			}
			if (!kingdom.HasStance(this.Data, RelationUtils.Stance.Alliance))
			{
				icon.gameObject.SetActive(false);
				return;
			}
			icon.gameObject.SetActive(true);
			string key = "Stance.Alliance.icon";
			string key2 = "Kingdom.Stance.Alliance.short_description";
			icon.GetIcon().overrideSprite = global::Defs.GetObj<Sprite>("Kingdom", key, null);
			UIText.SetTextKey(icon.GetDescription(), key2, null, null);
		}

		// Token: 0x06004A8A RID: 19082 RVA: 0x00221160 File Offset: 0x0021F360
		private void UpdateMarriage()
		{
			UIKingdomTooltip.Stances.Icon icon;
			if (!this.icons.TryGetValue("marriage", out icon))
			{
				return;
			}
			if (this.Data == null)
			{
				return;
			}
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom == null && this.Data == kingdom)
			{
				icon.gameObject.SetActive(false);
				return;
			}
			if (!KingdomAndKingdomRelation.GetMarriage(kingdom, this.Data))
			{
				icon.gameObject.SetActive(false);
				return;
			}
			icon.gameObject.SetActive(true);
			string key = "Stance.Marriage.Active.icon";
			string key2 = "Kingdom.Stance.Marriage.Active.short_description";
			icon.GetIcon().overrideSprite = global::Defs.GetObj<Sprite>("Kingdom", key, null);
			UIText.SetTextKey(icon.GetDescription(), key2, null, null);
		}

		// Token: 0x06004A8B RID: 19083 RVA: 0x00221204 File Offset: 0x0021F404
		private void UpdateTrade()
		{
			UIKingdomTooltip.Stances.Icon icon;
			if (!this.icons.TryGetValue("trade", out icon))
			{
				return;
			}
			if (this.Data == null)
			{
				return;
			}
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom == null || this.Data == kingdom)
			{
				icon.gameObject.SetActive(false);
				return;
			}
			if (!kingdom.HasTradeAgreement(this.Data))
			{
				icon.gameObject.SetActive(false);
				return;
			}
			icon.gameObject.SetActive(true);
			string key = "Stance.Trade.Trade.icon";
			string key2 = "Kingdom.Stance.Trade.Trade.short_description";
			icon.GetIcon().overrideSprite = global::Defs.GetObj<Sprite>("Kingdom", key, null);
			UIText.SetTextKey(icon.GetDescription(), key2, null, null);
		}

		// Token: 0x06004A8C RID: 19084 RVA: 0x002212A8 File Offset: 0x0021F4A8
		private void UpdatePacts()
		{
			if (this.m_Pacts == null)
			{
				return;
			}
			if (this.Data == null)
			{
				return;
			}
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom == null || this.Data == kingdom)
			{
				this.m_Pacts.gameObject.SetActive(false);
				return;
			}
			this.m_Pacts.gameObject.SetActive(true);
			int positivePactsWith = kingdom.GetPositivePactsWith(this.Data);
			int negativePactsWith = kingdom.GetNegativePactsWith(this.Data);
			if (positivePactsWith == 0 && negativePactsWith == 0)
			{
				this.m_Pacts.gameObject.SetActive(false);
				return;
			}
			this.m_Pacts.gameObject.SetActive(true);
			this.vars.Set<int>("pacts_with_us_amount", positivePactsWith);
			this.vars.Set<bool>("multiple_friendly_pacts", positivePactsWith > 1);
			this.vars.Set<bool>("pacts_with_us", positivePactsWith > 0);
			this.vars.Set<int>("pacts_against_us_amount", negativePactsWith);
			this.vars.Set<bool>("multiple_hostile_pacts", negativePactsWith > 1);
			this.vars.Set<bool>("pacts_against_us", negativePactsWith > 0);
			if (this.m_PositivePacts != null)
			{
				this.m_PositivePacts.text = positivePactsWith.ToString();
			}
			if (this.m_NegativePacts != null)
			{
				this.m_NegativePacts.text = negativePactsWith.ToString();
			}
			if (this.m_PactsDescription != null)
			{
				UIText.SetTextKey(this.m_PactsDescription, "KingdomTooltip.pacts", this.vars, null);
			}
		}

		// Token: 0x06004A8D RID: 19085 RVA: 0x0022141C File Offset: 0x0021F61C
		private bool HasNonDefaultStance()
		{
			if (this.Data == null)
			{
				return false;
			}
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom == null)
			{
				return false;
			}
			RelationUtils.Stance warStance = kingdom.GetWarStance(this.Data);
			return warStance.IsAlliance() || warStance.IsWar() || warStance.IsAnyVassalage() || warStance.IsNonAgression() || (warStance.IsPeace() && kingdom.IsInTruceWith(this.Data));
		}

		// Token: 0x06004A8F RID: 19087 RVA: 0x002214A0 File Offset: 0x0021F6A0
		[CompilerGenerated]
		private void <BuildIcons>g__AddIcon|15_0(string id)
		{
			UIKingdomTooltip.Stances.Icon icon = UIKingdomTooltip.Stances.Icon.Create(this.m_IconPrototype, base.transform);
			if (icon != null)
			{
				this.icons.Add(id, icon);
			}
		}

		// Token: 0x0400397A RID: 14714
		[UIFieldTarget("id_IconPrototype")]
		private GameObject m_IconPrototype;

		// Token: 0x0400397B RID: 14715
		[UIFieldTarget("id_Pacts")]
		private GameObject m_Pacts;

		// Token: 0x0400397C RID: 14716
		[UIFieldTarget("id_PositivePacts")]
		private TextMeshProUGUI m_PositivePacts;

		// Token: 0x0400397D RID: 14717
		[UIFieldTarget("id_NegativePacts")]
		private TextMeshProUGUI m_NegativePacts;

		// Token: 0x0400397E RID: 14718
		[UIFieldTarget("id_PactsDescription")]
		private TextMeshProUGUI m_PactsDescription;

		// Token: 0x04003980 RID: 14720
		private Dictionary<string, UIKingdomTooltip.Stances.Icon> icons = new Dictionary<string, UIKingdomTooltip.Stances.Icon>();

		// Token: 0x04003981 RID: 14721
		private Vars vars;

		// Token: 0x04003982 RID: 14722
		private bool m_Initilazed;

		// Token: 0x02000A0F RID: 2575
		private class Icon : MonoBehaviour
		{
			// Token: 0x0600555C RID: 21852 RVA: 0x00249539 File Offset: 0x00247739
			private void Init()
			{
				if (this.m_Initilazed)
				{
					return;
				}
				UICommon.FindComponents(this, false);
				this.m_Initilazed = true;
			}

			// Token: 0x0600555D RID: 21853 RVA: 0x00249552 File Offset: 0x00247752
			public Image GetIcon()
			{
				this.Init();
				return this.m_Icon;
			}

			// Token: 0x0600555E RID: 21854 RVA: 0x00249560 File Offset: 0x00247760
			public TextMeshProUGUI GetDescription()
			{
				this.Init();
				return this.m_Description;
			}

			// Token: 0x0600555F RID: 21855 RVA: 0x0024956E File Offset: 0x0024776E
			public static UIKingdomTooltip.Stances.Icon Create(GameObject prototype, Transform parent)
			{
				if (prototype == null)
				{
					return null;
				}
				if (parent == null)
				{
					return null;
				}
				UIKingdomTooltip.Stances.Icon icon = global::Common.Spawn(prototype, parent, false, "").AddComponent<UIKingdomTooltip.Stances.Icon>();
				icon.Init();
				return icon;
			}

			// Token: 0x04004661 RID: 18017
			[UIFieldTarget("id_Icon")]
			private Image m_Icon;

			// Token: 0x04004662 RID: 18018
			[UIFieldTarget("id_Description")]
			private TextMeshProUGUI m_Description;

			// Token: 0x04004663 RID: 18019
			private bool m_Initilazed;
		}
	}

	// Token: 0x0200074D RID: 1869
	internal class RelationsBar : MonoBehaviour
	{
		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x06004A90 RID: 19088 RVA: 0x002214D5 File Offset: 0x0021F6D5
		// (set) Token: 0x06004A91 RID: 19089 RVA: 0x002214DD File Offset: 0x0021F6DD
		public Logic.Kingdom Data { get; private set; }

		// Token: 0x06004A92 RID: 19090 RVA: 0x002214E6 File Offset: 0x0021F6E6
		private void Init()
		{
			if (this.m_Initilazed)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_RectTransform = (base.transform as RectTransform);
			this.vars = new Vars();
			this.m_Initilazed = true;
		}

		// Token: 0x06004A93 RID: 19091 RVA: 0x0022151C File Offset: 0x0021F71C
		public void SetKingdom(Logic.Kingdom kingdom)
		{
			this.Init();
			this.Data = kingdom;
			this.vars.Clear();
			this.vars.Set<Logic.Kingdom>("kingdom", BaseUI.LogicKingdom());
			this.vars.Set<Logic.Kingdom>("target_kigdom", kingdom);
			Tooltip.Get(base.gameObject, true).SetDef("KingdomRelationship", this.vars);
		}

		// Token: 0x06004A94 RID: 19092 RVA: 0x00221583 File Offset: 0x0021F783
		private void Update()
		{
			if (this.Data != null)
			{
				this.UpdateRelationBar();
			}
		}

		// Token: 0x06004A95 RID: 19093 RVA: 0x00221594 File Offset: 0x0021F794
		private void UpdateRelationBar()
		{
			if (this.m_Tumb == null)
			{
				return;
			}
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom == null)
			{
				return;
			}
			float relationship = KingdomAndKingdomRelation.Get(kingdom, this.Data, true, false).GetRelationship();
			this.vars.Set<int>("relationship", (int)relationship);
			this.vars.Set<string>("relationship_key", global::Kingdom.GetKingdomRelationsKey(kingdom, this.Data));
			float num = (relationship / RelationUtils.Def.maxRelationship + 1f) / 2f;
			float num2 = this.m_RectTransform.rect.width - 6f;
			this.m_Tumb.localPosition = new Vector3(num2 * num - num2 / 2f, this.m_Tumb.localPosition.y, 0f);
		}

		// Token: 0x04003983 RID: 14723
		[UIFieldTarget("id_Tumb")]
		private RectTransform m_Tumb;

		// Token: 0x04003985 RID: 14725
		private RectTransform m_RectTransform;

		// Token: 0x04003986 RID: 14726
		private bool m_Initilazed;

		// Token: 0x04003987 RID: 14727
		private Vars vars;
	}
}

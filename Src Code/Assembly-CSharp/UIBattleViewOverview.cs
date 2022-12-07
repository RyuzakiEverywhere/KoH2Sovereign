using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020001D2 RID: 466
public class UIBattleViewOverview : MonoBehaviour, IListener
{
	// Token: 0x06001B73 RID: 7027 RVA: 0x00106490 File Offset: 0x00104690
	private bool Populate(Logic.Battle battle)
	{
		this.logic = battle;
		this.RecalcLeaders();
		this.RecalculateKingdoms();
		if (this.estimationBar != null)
		{
			this.estimationBar.SetObject(battle);
			UIBattleWindow.SetEstimationTooltip(this.estimationBar.gameObject, battle);
		}
		if (this.m_battle_name != null)
		{
			UIBattleWindow.SetEstimationTooltip(this.m_battle_name.gameObject, battle);
		}
		return true;
	}

	// Token: 0x06001B74 RID: 7028 RVA: 0x001064FC File Offset: 0x001046FC
	private void RecalcLeaders()
	{
		Logic.Army army = this.logic.GetArmy(0);
		Logic.Army army2 = this.logic.GetArmy(1);
		this.leaders[0] = ((army == null) ? null : army.leader);
		this.leaders[1] = ((army2 == null) ? null : army2.leader);
	}

	// Token: 0x06001B75 RID: 7029 RVA: 0x0010654B File Offset: 0x0010474B
	private void RecalculateKingdoms()
	{
		this.kingdoms[0] = this.logic.GetSideKingdom(0);
		this.kingdoms[1] = this.logic.GetSideKingdom(1);
	}

	// Token: 0x06001B76 RID: 7030 RVA: 0x00106578 File Offset: 0x00104778
	private void Start()
	{
		UICommon.FindComponents(this, false);
		this.estimationBar = base.GetComponentInChildren<UIBattleEstimationBar>();
		this.ui = BattleViewUI.Get();
		if (this.ui == null)
		{
			return;
		}
		if (BattleMap.battle != null)
		{
			this.Populate(BattleMap.battle);
			this.root = base.gameObject;
			if (this.logic.attacker_kingdom.id == this.ui.kingdom)
			{
				this.logic_sides[0] = 0;
				this.logic_sides[1] = 1;
			}
			else
			{
				this.logic_sides[0] = 1;
				this.logic_sides[1] = 0;
			}
			this.UpdateBattleName();
			this.UpdateName();
			this.UpdateSides();
			this.InitProgress();
			this.AddListeners();
			return;
		}
	}

	// Token: 0x06001B77 RID: 7031 RVA: 0x0010663A File Offset: 0x0010483A
	private void Update()
	{
		this.UpdateProgress();
		if (this.refresh_sides)
		{
			this.refresh_sides = false;
			this.UpdateSides();
		}
	}

	// Token: 0x06001B78 RID: 7032 RVA: 0x00106657 File Offset: 0x00104857
	private void OnDestroy()
	{
		this.RemoveListeners();
	}

	// Token: 0x06001B79 RID: 7033 RVA: 0x0010665F File Offset: 0x0010485F
	private void AddListeners()
	{
		if (this.logic != null)
		{
			this.logic.AddListener(this);
		}
	}

	// Token: 0x06001B7A RID: 7034 RVA: 0x00106675 File Offset: 0x00104875
	private void RemoveListeners()
	{
		if (this.logic != null)
		{
			this.logic.DelListener(this);
		}
	}

	// Token: 0x06001B7B RID: 7035 RVA: 0x0010668C File Offset: 0x0010488C
	private void InitProgress()
	{
		GameObject gameObject = global::Common.FindChildByName(this.root, "id_Progress", true, true);
		if (gameObject == null)
		{
			return;
		}
		if (this.logic.stage != Logic.Battle.Stage.Preparing)
		{
			gameObject.SetActive(false);
			return;
		}
		this.progress = global::Common.FindChildByName(gameObject, "id_progress", true, true);
		if (this.progress == null)
		{
			return;
		}
		this.progress.transform.localScale = Vector3.one;
	}

	// Token: 0x06001B7C RID: 7036 RVA: 0x00106704 File Offset: 0x00104904
	private void UpdateProgress()
	{
		if (this.progress == null)
		{
			return;
		}
		float preparation_time_cached = this.logic.preparation_time_cached;
		if (preparation_time_cached <= 0f)
		{
			this.progress.SetActive(false);
			this.progress = null;
			return;
		}
		float num = this.logic.game.time - this.logic.stage_time;
		float x = 1f - Mathf.Clamp01(num / preparation_time_cached);
		this.progress.transform.localScale = new Vector3(x, 1f, 1f);
	}

	// Token: 0x06001B7D RID: 7037 RVA: 0x00106798 File Offset: 0x00104998
	private void UpdateName()
	{
		global::Battle battle = this.logic.visuals as global::Battle;
		Vars vars = (battle != null) ? battle.Vars() : null;
		TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(this.root, "id_Name");
		if (vars != null)
		{
			UIText.SetTextKey(textMeshProUGUI, vars.Get<string>("BATTLE", null), vars, null);
		}
		TextMeshProUGUI textMeshProUGUI2 = global::Common.FindChildComponent<TextMeshProUGUI>(this.root, "id_State");
		if (textMeshProUGUI2 != null)
		{
			if (this.logic.IsFinishing())
			{
				bool flag = this.logic.winner == this.logic_sides[0];
				UIText.SetText(textMeshProUGUI2, flag ? "Victory" : "Defeat");
				textMeshProUGUI2.color = global::Battle.GetEstimationColor(flag ? "winning_decisively" : "losing_badly");
				if (textMeshProUGUI != null)
				{
					textMeshProUGUI2.fontSize = textMeshProUGUI.fontSize;
					return;
				}
			}
			else
			{
				UIText.SetText(textMeshProUGUI2, this.logic.stage.ToString());
			}
		}
	}

	// Token: 0x06001B7E RID: 7038 RVA: 0x00106898 File Offset: 0x00104A98
	private void UpdateSides()
	{
		for (int i = 0; i <= 1; i++)
		{
			int num = this.logic_sides[i];
			Logic.Character character = this.leaders[num];
			GameObject gameObject = global::Common.FindChildByName(this.root, (i == 0) ? "id_kingdom1" : "id_kingdom2", true, true);
			if (gameObject != null)
			{
				UIKingdomIcon component = gameObject.GetComponent<UIKingdomIcon>();
				if (component != null)
				{
					component.SetObject(character, null);
					component.ShowOverlord(true);
				}
			}
			GameObject gameObject2 = global::Common.FindChildByName(this.root, (i == 0) ? "id_character1" : "id_character2", true, true);
			if (!(gameObject2 == null))
			{
				UIBattleViewCharacterIcon icon_supporter = global::Common.FindChildComponent<UIBattleViewCharacterIcon>(gameObject2, "icon");
				GameObject rebelIcon = global::Common.FindChildByName(gameObject2, "id_LeaderlessRebel", true, true);
				this.SetupSupporter(icon_supporter, character, rebelIcon, num);
				UIBattleViewCharacterIcon icon_supporter2 = global::Common.FindChildComponent<UIBattleViewCharacterIcon>(gameObject2, "icon_supporter");
				GameObject rebelIcon2 = global::Common.FindChildByName(gameObject2, "id_LeaderlessRebelSupport", true, true);
				this.SetupSupporter(icon_supporter2, character, rebelIcon2, num + 2);
				UIBattleOverallMorale uibattleOverallMorale = global::Common.FindChildComponent<UIBattleOverallMorale>(gameObject2, "id_morale");
				if (uibattleOverallMorale != null)
				{
					uibattleOverallMorale.SetData(this.logic, num);
				}
				UIBattleInitiative uibattleInitiative = global::Common.FindChildComponent<UIBattleInitiative>(gameObject2, "initiative");
				if (this.logic.initiative == null || this.logic.initiative_side != num)
				{
					uibattleInitiative.gameObject.SetActive(false);
				}
				else
				{
					uibattleInitiative.gameObject.SetActive(true);
					uibattleInitiative.SetData(this.logic);
				}
				UIText.SetText(gameObject2, "side", (num == 0) ? "Attacker" : "Defender");
			}
		}
		this.UpdateEstimation();
	}

	// Token: 0x06001B7F RID: 7039 RVA: 0x00106A2C File Offset: 0x00104C2C
	private string GetStance(Logic.Character character)
	{
		string result = string.Empty;
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom.IsEnemy(character))
		{
			Logic.Kingdom kingdom2 = this.logic.GetKingdom();
			War war = kingdom.FindWarWith(this.logic.GetKingdom());
			if (war != null && !war.IsLeader(kingdom2))
			{
				result = "EnemySupporter";
			}
			else
			{
				result = "EnemyLeader";
			}
		}
		else if (kingdom.IsOwnStance(character))
		{
			result = "Mine";
		}
		else if (kingdom.IsNeutral(character))
		{
			result = "Neutral";
		}
		else if (kingdom.IsAlly(character))
		{
			result = "Ally";
		}
		return result;
	}

	// Token: 0x06001B80 RID: 7040 RVA: 0x00106ABA File Offset: 0x00104CBA
	public void ToggleVisibility()
	{
		base.gameObject.SetActive(!base.gameObject.activeSelf);
	}

	// Token: 0x06001B81 RID: 7041 RVA: 0x00106AD8 File Offset: 0x00104CD8
	private void SetupSupporter(UIBattleViewCharacterIcon icon_supporter, Logic.Character leader, GameObject rebelIcon, int reinf_id)
	{
		if (icon_supporter != null)
		{
			icon_supporter.Icon.OnSelect -= this.OpenAvailableReinforcementsWindow;
			Vars vars = new Vars();
			vars.Set<Logic.Character>("side_leader", leader);
			vars.Set<bool>("side_has_leader", leader != null);
			int num = reinf_id % 2;
			vars.Set<Logic.Kingdom>("side", this.logic.GetSideKingdom(num));
			List<Logic.Character> list = this.logic.FindValidReinforcements(num);
			vars.Set<bool>("can_add_reinforcements", this.logic.GetSideKingdom(num).IsOwnStance(BaseUI.LogicKingdom()) && list != null && list.Count > 0);
			int num2 = reinf_id;
			Logic.Army army;
			if (reinf_id < 2)
			{
				army = this.logic.GetArmy(reinf_id);
			}
			else
			{
				army = this.logic.GetSupporter(num);
			}
			if (army != null)
			{
				Logic.Rebel rebel = army.rebel;
				if (rebel != null && rebel.IsRegular())
				{
					icon_supporter.gameObject.SetActive(false);
					if (rebelIcon != null)
					{
						rebelIcon.SetActive(true);
					}
				}
				else
				{
					icon_supporter.gameObject.SetActive(true);
					icon_supporter.SetObject(null, vars);
					icon_supporter.SetObject((army != null) ? army.leader : null, vars);
					UICharacterIcon icon = icon_supporter.Icon;
					if (icon != null)
					{
						icon.ShowCrest(true);
					}
					UICharacterIcon icon2 = icon_supporter.Icon;
					if (icon2 != null)
					{
						icon2.ShowStatus(false);
					}
					UICharacterIcon icon3 = icon_supporter.Icon;
					if (icon3 != null)
					{
						icon3.EnableClassLevel(true);
					}
					if (rebelIcon != null)
					{
						rebelIcon.SetActive(false);
					}
				}
			}
			else
			{
				Logic.Army army2 = this.logic.reinforcements[reinf_id].army;
				if ((army2 == null && reinf_id >= 2 && this.logic.GetArmy(reinf_id % 2) != null) || (reinf_id >= 2 && army2 != null && this.logic.GetArmy(reinf_id % 2) == army2))
				{
					num2 = reinf_id % 2;
				}
				army2 = this.logic.reinforcements[num2].army;
				Logic.Character character = (army2 != null) ? army2.leader : null;
				if (character != null && character == leader)
				{
					character = null;
				}
				vars.Set<bool>("is_reinforcement", true);
				vars.Set<int>("battle_side", reinf_id);
				vars.Set<Logic.Battle>("battle", this.logic);
				if (character != null)
				{
					vars.Set<Logic.Character>("obj", character);
				}
				if (this.logic.GetSideKingdom(num).IsOwnStance(BaseUI.LogicKingdom()))
				{
					vars.Set<bool>("is_player_side", true);
					icon_supporter.Icon.OnSelect += this.OpenAvailableReinforcementsWindow;
				}
				else
				{
					vars.Set<bool>("is_player_side", false);
				}
				vars.Set<int>("reinf_id", num2);
				if (rebelIcon != null)
				{
					rebelIcon.SetActive(false);
				}
				icon_supporter.SetObject(null, vars);
				icon_supporter.SetObject(character, vars);
			}
			this.supporter_icons[reinf_id] = icon_supporter;
		}
	}

	// Token: 0x06001B82 RID: 7042 RVA: 0x00106D8C File Offset: 0x00104F8C
	private void OpenAvailableReinforcementsWindow(UICharacterIcon icon)
	{
		int num = icon.vars.Get<int>("battle_side", 0);
		List<Logic.Character> list = this.logic.FindValidReinforcements(num);
		if (list == null || list.Count == 0)
		{
			return;
		}
		List<Value> list2 = new List<Value>();
		List<Vars> list3 = new List<Vars>();
		Vars vars;
		for (int i = 0; i < list.Count; i++)
		{
			Logic.Character character = list[i];
			vars = new Vars();
			Logic.Battle battle = this.logic;
			Logic.Character character2 = character;
			float val = battle.CalcReinforcementTime((character2 != null) ? character2.GetArmy() : null);
			vars.Set<int>("battle_side", num);
			vars.Set<float>("estimation_time", val);
			vars.Set<string>("rightTextKey", "Battle.reinforcement_estimation_text");
			list2.Add(character);
			list3.Add(vars);
		}
		vars = new Vars();
		vars.Set<string>("localization_key", "TargetPicker.none_text");
		List<TargetPickerData> list4 = TargetPickerData.Create(list2, list3, null);
		list4.Insert(0, new TargetPickerData
		{
			Target = "None",
			Vars = vars
		});
		if (num > 1)
		{
			UITargetSelectWindow.ShowDialog(list4, list[0], new Action<Value>(this.OnSelectSecondReinforcementTarget), null, null, null, null, null, "", "");
			return;
		}
		UITargetSelectWindow.ShowDialog(list4, list[0], new Action<Value>(this.OnSelectMainReinforcementTarget), null, null, null, null, null, "", "");
	}

	// Token: 0x06001B83 RID: 7043 RVA: 0x00106EF4 File Offset: 0x001050F4
	private void OnSelectMainReinforcementTarget(Value value)
	{
		Logic.Character character = value.Get<Logic.Character>();
		if (character == null)
		{
			int reinf_id = -1;
			if (this.logic.GetSideKingdom(0).IsOwnStance(BaseUI.LogicKingdom()))
			{
				reinf_id = 0;
			}
			else if (this.logic.GetSideKingdom(1).IsOwnStance(BaseUI.LogicKingdom()))
			{
				reinf_id = 1;
			}
			this.logic.SetReinforcements(null, reinf_id, -1f, false, true);
		}
		else
		{
			Logic.Army army = character.GetArmy();
			int joinSide = this.logic.GetJoinSide(army, true);
			this.logic.AddIntendedReinforcement(army, false);
			this.logic.SetReinforcements(army, joinSide, this.logic.CalcReinforcementTime(army), false, true);
		}
		if (BattleMap.battle != null)
		{
			this.Populate(BattleMap.battle);
		}
		this.UpdateSides();
	}

	// Token: 0x06001B84 RID: 7044 RVA: 0x00106FB0 File Offset: 0x001051B0
	private void OnSelectSecondReinforcementTarget(Value value)
	{
		Logic.Character character = value.Get<Logic.Character>();
		if (character == null)
		{
			int num = -1;
			if (this.logic.GetSideKingdom(0).IsOwnStance(BaseUI.LogicKingdom()))
			{
				num = 0;
			}
			else if (this.logic.GetSideKingdom(1).IsOwnStance(BaseUI.LogicKingdom()))
			{
				num = 1;
			}
			this.logic.SetReinforcements(null, num + 2, -1f, false, true);
		}
		else
		{
			Logic.Army army = character.GetArmy();
			int joinSide = this.logic.GetJoinSide(army, true);
			this.logic.AddIntendedReinforcement(army, false);
			this.logic.SetReinforcements(army, joinSide + 2, this.logic.CalcReinforcementTime(army), false, true);
		}
		if (BattleMap.battle != null)
		{
			this.Populate(BattleMap.battle);
		}
		this.UpdateSides();
	}

	// Token: 0x06001B85 RID: 7045 RVA: 0x00107070 File Offset: 0x00105270
	private void UpdateEstimation()
	{
		if (this.logic.simulation == null)
		{
			return;
		}
		float num;
		if (this.logic.winner < 0)
		{
			num = this.logic.simulation.estimation;
			if (this.logic_sides[0] != 0)
			{
				num = 1f - num;
			}
		}
		else
		{
			num = ((this.logic_sides[0] == this.logic.winner) ? 1f : 0f);
		}
		for (int i = 0; i <= 1; i++)
		{
			GameObject gameObject = global::Common.FindChildByName(this.root, (i == 0) ? "id_character1" : "id_character2", true, true);
			if (!(gameObject == null))
			{
				TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(gameObject, "estimation");
				if (textMeshProUGUI != null)
				{
					string key;
					if (this.logic.winner >= 0)
					{
						key = ((this.logic_sides[i] == this.logic.winner) ? "won" : "lost");
					}
					else
					{
						key = global::Battle.GetEstimationKey((i == 0) ? (1f - num) : num);
					}
					UIText.SetText(textMeshProUGUI, global::Battle.GetEstimationText(key));
					textMeshProUGUI.color = global::Battle.GetEstimationColor(key);
				}
			}
		}
	}

	// Token: 0x06001B86 RID: 7046 RVA: 0x00107190 File Offset: 0x00105390
	private void UpdateBattleName()
	{
		if (this.m_battle_name == null)
		{
			return;
		}
		string text = string.Empty;
		switch (this.logic.type)
		{
		default:
			text = "Battle.fieldBattleCaption";
			break;
		case Logic.Battle.Type.Plunder:
			text = "Battle.pillageDefenseCaption";
			break;
		case Logic.Battle.Type.Siege:
		case Logic.Battle.Type.Assault:
		case Logic.Battle.Type.BreakSiege:
		{
			Logic.Settlement settlement = this.logic.settlement;
			text = ((((settlement != null) ? settlement.type : null) == "Keep") ? "Battle.castleSiegeBattleCaption" : "Battle.townSiegeBattleCaption");
			break;
		}
		case Logic.Battle.Type.Naval:
			throw new NotImplementedException();
		case Logic.Battle.Type.PlunderInterrupt:
			text = "Battle.pillageAttackCaption";
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			UIText.SetTextKey(this.m_battle_name, text, this.GetBattleVars(this.logic), null);
			return;
		}
		this.m_battle_name.text = this.logic.ToString();
	}

	// Token: 0x06001B87 RID: 7047 RVA: 0x00107268 File Offset: 0x00105468
	protected Vars GetBattleVars(Logic.Battle b)
	{
		Vars vars = b.Vars();
		vars.Set<Logic.Realm>("realm", GameLogic.Get(true).GetRealm(this.logic.realm_id));
		vars.Set<Logic.Settlement>("settlement", b.settlement);
		return vars;
	}

	// Token: 0x06001B88 RID: 7048 RVA: 0x001072A4 File Offset: 0x001054A4
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "changed")
		{
			this.UpdateEstimation();
			return;
		}
		if (message == "armies_changed")
		{
			this.refresh_sides = true;
			this.RecalcLeaders();
			return;
		}
		if (message == "reinforcements_changed")
		{
			this.refresh_sides = true;
			this.RecalcLeaders();
			return;
		}
	}

	// Token: 0x040011DB RID: 4571
	[UIFieldTarget("id_battle_name")]
	private TextMeshProUGUI m_battle_name;

	// Token: 0x040011DC RID: 4572
	[UIFieldTarget("id_kingdom1")]
	private UIKingdomIcon m_kingdom1;

	// Token: 0x040011DD RID: 4573
	[UIFieldTarget("id_kingdom2")]
	private UIKingdomIcon m_kingdo2;

	// Token: 0x040011DE RID: 4574
	private Logic.Battle logic;

	// Token: 0x040011DF RID: 4575
	private int[] logic_sides = new int[2];

	// Token: 0x040011E0 RID: 4576
	private Logic.Character[] leaders = new Logic.Character[2];

	// Token: 0x040011E1 RID: 4577
	private Logic.Kingdom[] kingdoms = new Logic.Kingdom[2];

	// Token: 0x040011E2 RID: 4578
	private GameObject root;

	// Token: 0x040011E3 RID: 4579
	private GameObject progress;

	// Token: 0x040011E4 RID: 4580
	private int player_kingdom_id;

	// Token: 0x040011E5 RID: 4581
	private BattleViewUI ui;

	// Token: 0x040011E6 RID: 4582
	private UIBattleEstimationBar estimationBar;

	// Token: 0x040011E7 RID: 4583
	private UIBattleViewCharacterIcon[] supporter_icons = new UIBattleViewCharacterIcon[4];

	// Token: 0x040011E8 RID: 4584
	private bool refresh_sides;
}

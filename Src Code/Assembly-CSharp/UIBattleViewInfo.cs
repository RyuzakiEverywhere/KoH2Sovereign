using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020001CE RID: 462
public class UIBattleViewInfo : MonoBehaviour, IListener
{
	// Token: 0x06001B26 RID: 6950 RVA: 0x001048D0 File Offset: 0x00102AD0
	private bool Populate(Logic.Battle battle)
	{
		this.logic = battle;
		Logic.Army army = this.logic.GetArmy(0);
		Logic.Army army2 = this.logic.GetArmy(1);
		this.leaders[0] = ((army == null) ? null : army.leader);
		this.leaders[1] = ((army2 == null) ? null : army2.leader);
		return true;
	}

	// Token: 0x06001B27 RID: 6951 RVA: 0x00104927 File Offset: 0x00102B27
	private void Awake()
	{
		Debug.LogWarning(base.GetType() + " is Obsolete!", base.gameObject);
	}

	// Token: 0x06001B28 RID: 6952 RVA: 0x00104944 File Offset: 0x00102B44
	private void Start()
	{
		this.ui = BattleViewUI.Get();
		if (this.ui == null)
		{
			return;
		}
		if (BattleMap.battle != null)
		{
			this.Populate(BattleMap.battle);
			if (this.logic.attacker_kingdom.id == this.ui.kingdom)
			{
				this.logic_sides[0] = 0;
				this.logic_sides[1] = 1;
			}
			else
			{
				this.logic_sides[0] = 1;
				this.logic_sides[1] = 0;
				Logic.Character character = this.leaders[0];
				this.leaders[0] = this.leaders[1];
				this.leaders[1] = character;
			}
			this.root = global::Common.FindChildByName(base.gameObject, "id_Ongoing", true, true);
			this.UpdateName();
			this.UpdateLeaders();
			this.CreateUnitFrames();
			this.InitProgress();
			this.CreateButtons();
			this.AddListeners();
			return;
		}
	}

	// Token: 0x06001B29 RID: 6953 RVA: 0x00104A27 File Offset: 0x00102C27
	private void Update()
	{
		this.UpdateProgress();
	}

	// Token: 0x06001B2A RID: 6954 RVA: 0x00104A2F File Offset: 0x00102C2F
	private void OnDestroy()
	{
		this.RemoveListeners();
	}

	// Token: 0x06001B2B RID: 6955 RVA: 0x00104A38 File Offset: 0x00102C38
	private void AddListeners()
	{
		if (this.logic != null)
		{
			this.logic.AddListener(this);
		}
		if (this.btn_retreat != null)
		{
			BSGButton bsgbutton = this.btn_retreat;
			bsgbutton.onClick = (BSGButton.OnClick)Delegate.Combine(bsgbutton.onClick, new BSGButton.OnClick(this.Retreat));
		}
		if (this.btn_leave_battle != null)
		{
			BSGButton bsgbutton2 = this.btn_leave_battle;
			bsgbutton2.onClick = (BSGButton.OnClick)Delegate.Combine(bsgbutton2.onClick, new BSGButton.OnClick(this.LeaveBattle));
		}
	}

	// Token: 0x06001B2C RID: 6956 RVA: 0x00104AC4 File Offset: 0x00102CC4
	private void RemoveListeners()
	{
		if (this.logic != null)
		{
			this.logic.DelListener(this);
		}
		if (this.btn_retreat != null)
		{
			BSGButton bsgbutton = this.btn_retreat;
			bsgbutton.onClick = (BSGButton.OnClick)Delegate.Remove(bsgbutton.onClick, new BSGButton.OnClick(this.Retreat));
		}
		if (this.btn_leave_battle != null)
		{
			BSGButton bsgbutton2 = this.btn_leave_battle;
			bsgbutton2.onClick = (BSGButton.OnClick)Delegate.Remove(bsgbutton2.onClick, new BSGButton.OnClick(this.LeaveBattle));
		}
	}

	// Token: 0x06001B2D RID: 6957 RVA: 0x00104B4F File Offset: 0x00102D4F
	private void ShowBtn(BSGButton btn, bool show)
	{
		if (btn == null)
		{
			return;
		}
		btn.gameObject.SetActive(show);
	}

	// Token: 0x06001B2E RID: 6958 RVA: 0x00104B68 File Offset: 0x00102D68
	private BSGButton CreateButton(GameObject parent, string name, string text_key)
	{
		GameObject gameObject = global::Common.Spawn(this.ButtonPrefab, false, false);
		if (gameObject == null)
		{
			return null;
		}
		gameObject.name = name;
		if (parent != null)
		{
			gameObject.transform.SetParent(parent.transform, false);
		}
		UIText.SetTextKey(gameObject.GetComponentInChildren<TextMeshProUGUI>(), text_key, null, null);
		return gameObject.GetComponent<BSGButton>();
	}

	// Token: 0x06001B2F RID: 6959 RVA: 0x00104BC4 File Offset: 0x00102DC4
	private void CreateButtons()
	{
		this.btn_retreat = global::Common.FindChildComponent<BSGButton>(this.root, "id_retreat");
		this.btn_leave_battle = global::Common.FindChildComponent<BSGButton>(this.root, "id_leave");
	}

	// Token: 0x06001B30 RID: 6960 RVA: 0x00104BF4 File Offset: 0x00102DF4
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

	// Token: 0x06001B31 RID: 6961 RVA: 0x00104C6C File Offset: 0x00102E6C
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

	// Token: 0x06001B32 RID: 6962 RVA: 0x00104D00 File Offset: 0x00102F00
	private void Retreat(int side)
	{
		if (side == 1 && this.logic.is_siege)
		{
			return;
		}
		global::Army army = global::Army.Get(this.logic.GetArmy(side));
		this.logic.DoAction("retreat", side, "");
		if (army != null)
		{
			this.ui.SelectObj(army.gameObject, false, true, true, true);
		}
	}

	// Token: 0x06001B33 RID: 6963 RVA: 0x00104D68 File Offset: 0x00102F68
	private void RetreatSupporters(int side)
	{
		global::Army army = global::Army.Get(this.logic.GetArmy(side));
		this.logic.DoAction("retreat_supporters", side, "");
		if (army != null)
		{
			this.ui.SelectObj(army.gameObject, false, true, true, true);
		}
	}

	// Token: 0x06001B34 RID: 6964 RVA: 0x00104DBC File Offset: 0x00102FBC
	private void Retreat(BSGButton button)
	{
		if (this.ui == null || this.logic == null)
		{
			return;
		}
		if (this.ui.kingdom == this.logic.attacker_kingdom.id)
		{
			this.Retreat(0);
			return;
		}
		if (this.ui.kingdom == this.logic.defender_kingdom.id)
		{
			this.Retreat(1);
			return;
		}
		int num = this.ui.kingdom;
		Logic.Army attacker_support = this.logic.attacker_support;
		if (num == ((attacker_support != null) ? attacker_support.GetKingdom().id : 0))
		{
			this.RetreatSupporters(0);
			return;
		}
		int num2 = this.ui.kingdom;
		Logic.Army defender_support = this.logic.defender_support;
		if (num2 == ((defender_support != null) ? defender_support.GetKingdom().id : 0))
		{
			this.RetreatSupporters(1);
		}
	}

	// Token: 0x06001B35 RID: 6965 RVA: 0x00104EA0 File Offset: 0x001030A0
	private void LeaveBattle(BSGButton button)
	{
		if (this.ui == null || this.logic == null)
		{
			return;
		}
		if (this.ui.kingdom == this.logic.attacker_kingdom.id)
		{
			this.LeaveBattle(0);
			return;
		}
		if (this.ui.kingdom == this.logic.defender_kingdom.id)
		{
			this.LeaveBattle(1);
			return;
		}
		int num = this.ui.kingdom;
		Logic.Army attacker_support = this.logic.attacker_support;
		if (num == ((attacker_support != null) ? attacker_support.GetKingdom().id : 0))
		{
			this.RetreatSupporters(0);
			return;
		}
		int num2 = this.ui.kingdom;
		Logic.Army defender_support = this.logic.defender_support;
		if (num2 == ((defender_support != null) ? defender_support.GetKingdom().id : 0))
		{
			this.RetreatSupporters(1);
		}
	}

	// Token: 0x06001B36 RID: 6966 RVA: 0x00104F83 File Offset: 0x00103183
	private void LeaveBattle(int side)
	{
		Debug.Log("battle leave  action call");
		this.logic.DoAction("leave_battle", side, "");
	}

	// Token: 0x06001B37 RID: 6967 RVA: 0x00104FA8 File Offset: 0x001031A8
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

	// Token: 0x06001B38 RID: 6968 RVA: 0x001050A8 File Offset: 0x001032A8
	private void UpdateLeaders()
	{
		for (int i = 0; i <= 1; i++)
		{
			GameObject gameObject = global::Common.FindChildByName(this.root, (i == 0) ? "id_character1" : "id_character2", true, true);
			if (!(gameObject == null))
			{
				Logic.Character character = this.leaders[i];
				if (character == null)
				{
					gameObject.SetActive(false);
				}
				else
				{
					UICharacterIcon uicharacterIcon = global::Common.FindChildComponent<UICharacterIcon>(gameObject, "icon");
					if (uicharacterIcon != null)
					{
						uicharacterIcon.SetObject(character, null);
					}
					UIText.SetText(gameObject, "side", (this.logic_sides[i] == 0) ? "Attacker" : "Defender");
				}
			}
		}
		this.UpdateEstimation();
	}

	// Token: 0x06001B39 RID: 6969 RVA: 0x00105144 File Offset: 0x00103344
	private void UpdateEstimation()
	{
		if (this.logic.simulation == null)
		{
			return;
		}
		float num;
		if (this.logic.winner < 0)
		{
			this.logic.simulation.CalcTotals(false, false);
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

	// Token: 0x06001B3A RID: 6970 RVA: 0x00105278 File Offset: 0x00103478
	private void CreateUnitFrames()
	{
		this.UnitFramePrefab = UICommon.GetPrefab("UnitSlot", null);
		for (int i = 0; i <= 1; i++)
		{
			GameObject gameObject = global::Common.FindChildByName(this.root, "id_units" + (i + 1), true, true);
			if (!(gameObject == null))
			{
				UICommon.DeleteChildren(gameObject.transform);
				if (!(this.UnitFramePrefab == null))
				{
					for (int j = 0; j < 5; j++)
					{
						for (int k = 0; k < Logic.Army.battle_cols; k++)
						{
							GameObject gameObject2 = global::Common.Spawn(this.UnitFramePrefab, false, false);
							gameObject2.name = string.Concat(new object[]
							{
								"unit_",
								j,
								"_",
								k
							});
							gameObject2.transform.SetParent(gameObject.transform, false);
						}
					}
				}
			}
		}
		this.UpdateUnitFrames();
	}

	// Token: 0x06001B3B RID: 6971 RVA: 0x00105368 File Offset: 0x00103568
	private void UpdateUnitFrames()
	{
		for (int i = 0; i <= 1; i++)
		{
			GameObject gameObject = global::Common.FindChildByName(this.root, "id_units" + (i + 1), true, true);
			if (!(gameObject == null))
			{
				Logic.Army army = this.logic.GetArmy(this.logic_sides[i]);
				for (int j = 0; j < 5; j++)
				{
					for (int k = 0; k < Logic.Army.battle_cols; k++)
					{
						UIUnitSlot uiunitSlot = global::Common.FindChildComponent<UIUnitSlot>(gameObject, string.Concat(new object[]
						{
							"unit_",
							j,
							"_",
							k
						}));
						if (!(uiunitSlot == null))
						{
							Logic.Unit unit = (army == null) ? null : army.GetUnitAtBattlePos(j, k);
							uiunitSlot.SetUnitInstance(unit, -1, army, null);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001B3C RID: 6972 RVA: 0x00105444 File Offset: 0x00103644
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "changed")
		{
			this.UpdateEstimation();
			return;
		}
	}

	// Token: 0x040011AA RID: 4522
	public GameObject ButtonPrefab;

	// Token: 0x040011AB RID: 4523
	private BattleViewUI ui;

	// Token: 0x040011AC RID: 4524
	private GameObject UnitFramePrefab;

	// Token: 0x040011AD RID: 4525
	[NonSerialized]
	public Logic.Battle logic;

	// Token: 0x040011AE RID: 4526
	private int[] logic_sides = new int[2];

	// Token: 0x040011AF RID: 4527
	private Logic.Character[] leaders = new Logic.Character[2];

	// Token: 0x040011B0 RID: 4528
	private GameObject root;

	// Token: 0x040011B1 RID: 4529
	private BSGButton btn_retreat;

	// Token: 0x040011B2 RID: 4530
	private BSGButton btn_leave_battle;

	// Token: 0x040011B3 RID: 4531
	private GameObject progress;

	// Token: 0x040011B4 RID: 4532
	private global::Battle battle;

	// Token: 0x040011B5 RID: 4533
	private int player_kingdom_id;
}

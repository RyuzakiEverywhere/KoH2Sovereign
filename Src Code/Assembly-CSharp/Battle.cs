using System;
using System.Collections.Generic;
using FMODUnity;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000BA RID: 186
[Serializable]
public class Battle : GameLogic.Behaviour, VisibilityDetector.IVisibilityChanged
{
	// Token: 0x060007A5 RID: 1957 RVA: 0x0005091C File Offset: 0x0004EB1C
	public bool IsVisible()
	{
		return this.visible;
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x00050924 File Offset: 0x0004EB24
	public override Logic.Object GetLogic()
	{
		return this.logic;
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x0005092C File Offset: 0x0004EB2C
	public void UpdateUnitVisibility()
	{
		this._needs_update_unit_visibility = true;
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x00050935 File Offset: 0x0004EB35
	public void RefreshUnits()
	{
		this._needs_refresh_units = true;
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x0005093E File Offset: 0x0004EB3E
	public Vars Vars()
	{
		if (this.logic == null)
		{
			return null;
		}
		return this.logic.Vars();
	}

	// Token: 0x060007AA RID: 1962 RVA: 0x00050958 File Offset: 0x0004EB58
	public string OngoingMessageDefId()
	{
		if (this.logic.is_siege)
		{
			return "Ongoing" + this.logic.type.ToString() + this.logic.settlement.type;
		}
		if (this.logic.type == Logic.Battle.Type.PlunderInterrupt)
		{
			return "Ongoing" + Logic.Battle.Type.Plunder.ToString();
		}
		return "Ongoing" + this.logic.type.ToString();
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x000509EB File Offset: 0x0004EBEB
	private void Update()
	{
		this.UpdateUnits();
		if (this.must_fix_siege_label)
		{
			this.FixSiegeLabel();
		}
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x00050A04 File Offset: 0x0004EC04
	public void SetSelected(bool bSelected, bool bPrimaryselection = true)
	{
		this.selected = bSelected;
		this.primarySelection = bPrimaryselection;
		if (this.selected)
		{
			this.CreateSelection();
		}
		else
		{
			this.DestroySelection();
		}
		BaseUI.LogicKingdom();
		if (this.selected)
		{
			if (global::Battle.PlayerIsAttacker(this.logic, true))
			{
				string key = "SelPlayerBattleTrigger";
				Logic.Kingdom defender_kingdom = this.logic.defender_kingdom;
				string religion;
				if (defender_kingdom == null)
				{
					religion = null;
				}
				else
				{
					Religion religion2 = defender_kingdom.religion;
					religion = ((religion2 != null) ? religion2.name : null);
				}
				BackgroundMusic.OnTrigger(key, religion);
			}
			else if (global::Battle.PlayerIsDefender(this.logic, true))
			{
				string key2 = "SelPlayerBattleTrigger";
				Logic.Kingdom attacker_kingdom = this.logic.attacker_kingdom;
				string religion3;
				if (attacker_kingdom == null)
				{
					religion3 = null;
				}
				else
				{
					Religion religion4 = attacker_kingdom.religion;
					religion3 = ((religion4 != null) ? religion4.name : null);
				}
				BackgroundMusic.OnTrigger(key2, religion3);
			}
		}
		if (this.ui_pvFigure != null)
		{
			this.ui_pvFigure.UpdateIcon();
		}
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x00050AD4 File Offset: 0x0004ECD4
	public bool IsSelected()
	{
		return this.selected;
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x00050ADC File Offset: 0x0004ECDC
	public void RecreateSelectionUI()
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null)
		{
			baseUI.RefreshSelection(base.gameObject);
		}
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x00050B04 File Offset: 0x0004ED04
	public void CreateSelection()
	{
		if (this.selection != null)
		{
			return;
		}
		if (this.logic == null || !this.logic.started)
		{
			return;
		}
		if (this.logic.settlement != null)
		{
			global::Settlement settlement = this.logic.settlement.visuals as global::Settlement;
			if (settlement != null && settlement.visible)
			{
				this.selection = settlement.CreateSelection(true);
			}
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null || !baseUI.SelectionShown())
		{
			return;
		}
		this.selection = MeshUtils.CreateSelectionCircle(base.gameObject, this.logic.GetRadius(), baseUI.GetSelectionMaterial(base.gameObject, null, this.primarySelection, false), 0.25f);
		MeshUtils.SnapSelectionToTerrain(this.selection, null);
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x00050BD1 File Offset: 0x0004EDD1
	public void DestroySelection()
	{
		if (this.selection == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(this.selection.gameObject);
		this.selection = null;
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x00050BFC File Offset: 0x0004EDFC
	private void UpdateSelectionUI()
	{
		this.UpdateBar();
		this.UpdateStatus();
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null)
		{
			worldUI.RefreshSelection(base.gameObject);
		}
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x00050C30 File Offset: 0x0004EE30
	private void CreateAftermathMessage(int winner)
	{
		if (WorldUI.Get() == null)
		{
			return;
		}
		Vars vars = this.Vars();
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		bool flag = false;
		string str = "";
		bool flag2 = false;
		Logic.Settlement settlement = this.logic.settlement;
		Game game = this.logic.game;
		object obj;
		if (game == null)
		{
			obj = null;
		}
		else
		{
			Logic.Religions religions = game.religions;
			if (religions == null)
			{
				obj = null;
			}
			else
			{
				Catholic catholic = religions.catholic;
				if (catholic == null)
				{
					obj = null;
				}
				else
				{
					Logic.Realm hq_realm = catholic.hq_realm;
					obj = ((hq_realm != null) ? hq_realm.castle : null);
				}
			}
		}
		if (settlement == obj)
		{
			return;
		}
		if (kingdom == this.logic.defender_kingdom)
		{
			flag = false;
		}
		else if (kingdom == this.logic.attacker_kingdom)
		{
			flag = true;
		}
		else
		{
			Logic.Settlement settlement2 = this.logic.settlement;
			if (((settlement2 != null) ? settlement2.GetKingdom() : null) != null && this.logic.settlement.GetKingdom().IsOwnStance(kingdom) && this.logic.attacker_kingdom.type != Logic.Kingdom.Type.Crusade)
			{
				flag = true;
				if (this.logic.settlement.GetKingdom().IsEnemy(this.logic.attacker))
				{
					str = "ThirdEnemy";
				}
				else
				{
					str = "ThirdParty";
				}
			}
			else
			{
				for (int i = 0; i < 2; i++)
				{
					List<Logic.Army> armies = this.logic.GetArmies(i);
					for (int j = 0; j < armies.Count; j++)
					{
						if (armies[j].GetKingdom() == kingdom)
						{
							if (i == 0)
							{
								flag = true;
							}
							flag2 = true;
							break;
						}
						if (flag2)
						{
							break;
						}
					}
				}
				if (!flag2)
				{
					return;
				}
			}
		}
		bool flag3 = winner >= 0;
		bool flag4 = winner == 0;
		bool flag5 = flag == flag4;
		string text = str + (flag5 ? "Won" : "Lost");
		switch (this.logic.victory_reason)
		{
		case Logic.Battle.VictoryReason.Combat:
		case Logic.Battle.VictoryReason.CapturePoints:
		case Logic.Battle.VictoryReason.LeaderKilled:
			switch (this.logic.type)
			{
			case Logic.Battle.Type.OpenField:
				text += "OpenField";
				break;
			case Logic.Battle.Type.Plunder:
			case Logic.Battle.Type.PlunderInterrupt:
				text = text + (flag ? "Enemy" : "Own") + "Plunder";
				break;
			case Logic.Battle.Type.Siege:
			case Logic.Battle.Type.Assault:
			case Logic.Battle.Type.BreakSiege:
				if (!flag)
				{
					Castle castle = this.logic.settlement as Castle;
					if (castle != null)
					{
						if (castle.sacked && this.logic.settlement.IsOwnStance(this.logic.defender))
						{
							text = "SackedOwnCastle";
						}
						else if (castle.GetRealm().GetKingdom().IsOwnStance(kingdom))
						{
							text += "OwnCastle";
							if (this.logic.attacker_kingdom.IsRebelKingdom())
							{
								text += "ToRebels";
							}
						}
						else
						{
							text += "EnemyCastleOccupiedByOwn";
						}
					}
					else
					{
						text += "OwnKeep";
					}
				}
				else
				{
					Logic.Kingdom kingdom2 = vars.Get<Logic.Kingdom>("settlement_owner", null);
					bool flag6 = vars.Get<bool>("was_occupied", false);
					if (!flag6)
					{
						text += "Enemy";
					}
					else if (kingdom2.IsOwnStance(kingdom))
					{
						text += "Own";
					}
					else if (kingdom2.IsEnemy(kingdom))
					{
						text += "Enemy";
					}
					else
					{
						text += "ThirdParty";
					}
					if (this.logic.settlement is Castle)
					{
						text += "Castle";
					}
					else
					{
						text += "Keep";
					}
					if (flag6 && !kingdom2.IsEnemy(kingdom))
					{
						text += "OccupiedByEnemy";
					}
				}
				break;
			case Logic.Battle.Type.Naval:
				text += "Naval";
				break;
			}
			break;
		case Logic.Battle.VictoryReason.Retreat:
			if (flag5)
			{
				text = "BattleRetreat";
			}
			break;
		case Logic.Battle.VictoryReason.LiftSiege:
			if (!flag)
			{
				if (this.logic.type == Logic.Battle.Type.Siege)
				{
					text = "BattleLiftSiege";
				}
				else if (this.logic.type == Logic.Battle.Type.Plunder)
				{
					text = "BattleStopPlundering";
				}
			}
			break;
		case Logic.Battle.VictoryReason.CounterBattle:
			if (!flag)
			{
				Logic.Settlement settlement3 = this.logic.settlement;
				if (!flag3 && settlement3 != null && settlement3.type == "Castle")
				{
					text = "CounterBattleWonOwnCastle";
				}
				else if (!flag3 && settlement3 != null && settlement3.type == "Keep")
				{
					text = "CounterBattleWonOwnKeep";
				}
				else
				{
					text = "CounterBattleDefender";
				}
			}
			else
			{
				text = "CounterBattleAttacker";
			}
			break;
		case Logic.Battle.VictoryReason.WarOver:
			text = "BattleWarOver";
			break;
		case Logic.Battle.VictoryReason.RealmChange:
			text = "BattleRealmChange";
			break;
		case Logic.Battle.VictoryReason.Surrender:
			text += "BattleSurrender";
			break;
		}
		if (this.aftermath_message_created)
		{
			Debug.LogWarning("Trying to create an aftermath message for second time! def_id=" + text);
			return;
		}
		Vars voiceVars;
		this.PlayBattleOutcomeSounds(flag5, flag, flag2, out voiceVars);
		MessageIcon x = null;
		if (flag2)
		{
			x = MessageIcon.Create(text + "Supporter", vars, true, voiceVars);
		}
		if (x == null)
		{
			x = MessageIcon.Create(text, vars, true, voiceVars);
		}
		if (x != null)
		{
			this.aftermath_message_created = true;
		}
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x00051150 File Offset: 0x0004F350
	private void PlayBattleOutcomeSounds(bool won, bool is_attacker, bool is_supporter, out Vars voice_vars)
	{
		voice_vars = null;
		if (this.logic.winner == -1)
		{
			return;
		}
		string text = null;
		string text2 = null;
		if (this.logic.victory_reason == Logic.Battle.VictoryReason.RealmChange)
		{
			return;
		}
		if (this.logic.victory_reason == Logic.Battle.VictoryReason.WarOver)
		{
			War war = this.logic.Vars().Get<War>("war", this.logic.attacker_kingdom.FindWarWith(this.logic.defender_kingdom));
			int num = this.logic.Vars().Get<int>("war_winner", -1);
			if (war == null || num < 0)
			{
				return;
			}
			if (num == war.GetSide(this.logic.attacker_kingdom))
			{
				return;
			}
		}
		else if (this.logic.victory_reason == Logic.Battle.VictoryReason.LiftSiege)
		{
			if (won)
			{
				text = "LiftSiege";
			}
		}
		else if (this.logic.victory_reason == Logic.Battle.VictoryReason.Retreat)
		{
			Logic.Character character;
			Logic.Character character2;
			if (is_attacker)
			{
				character = this.Vars().Get<Logic.Character>("defender_leader", null);
				if (is_supporter)
				{
					character2 = this.Vars().Get<Logic.Character>("attacker_support_leader", null);
				}
				else
				{
					character2 = this.Vars().Get<Logic.Character>("attacker_leader", null);
				}
			}
			else
			{
				character = this.Vars().Get<Logic.Character>("attacker_leader", null);
				if (is_supporter)
				{
					character2 = this.Vars().Get<Logic.Character>("defender_support_leader", null);
				}
				else
				{
					character2 = this.Vars().Get<Logic.Character>("defender_leader", null);
				}
			}
			if (character2 != null && character2.IsValid() && character != null && character.IsValid() && !character.IsPrisoner())
			{
				if (won)
				{
					text = "WonRetreated";
					voice_vars = new Vars();
					voice_vars.Set<Logic.Character>("character", character2);
				}
				else if (this.logic.battle_view_victory_reason != Logic.Battle.VictoryReason.Retreat)
				{
					text = "LostRetreated";
					voice_vars = new Vars();
					voice_vars.Set<Logic.Character>("character", character2);
				}
			}
		}
		else if (won)
		{
			if (this.logic.type == Logic.Battle.Type.OpenField || this.logic.type == Logic.Battle.Type.Naval || this.logic.type == Logic.Battle.Type.PlunderInterrupt)
			{
				voice_vars = new Vars();
				string text3 = null;
				if (this.logic.killed_nobles.Count == 0)
				{
					if (!is_supporter)
					{
						for (int i = 0; i < this.logic.imprisoned_at_end_of_battle.Count; i++)
						{
							string a = this.logic.imprisoned_at_end_of_battle[i];
							if (this.logic.imprisoned_at_end_of_battle_characters.Count > 0)
							{
								Logic.Character val = this.Vars().Get<Logic.Character>(is_attacker ? "attacker_leader" : "defender_leader", null);
								voice_vars.Set<Logic.Character>("character", val);
							}
							if (a == "King")
							{
								voice_vars.SetVar("is_king", true);
								text3 = "ImprisonedEnemyKing";
							}
							else if (a == "Prince")
							{
								voice_vars.SetVar("is_prince", true);
								text3 = "ImprisonedEnemyPrince";
							}
							else
							{
								text3 = "ImprisonedEnemyKnight";
							}
						}
					}
				}
				else
				{
					Logic.Army army = this.logic.GetArmy(this.logic.winner);
					Logic.Character character3 = (army != null) ? army.leader : null;
					if (character3 != null)
					{
						voice_vars.SetVar("character", character3);
						for (int j = 0; j < this.logic.killed_nobles.Count; j++)
						{
							string a2 = this.logic.killed_nobles[j];
							if (a2 == "King")
							{
								text3 = "KilledEnemyKing";
								voice_vars.SetVar("is_king", true);
							}
							else if (a2 == "Prince")
							{
								voice_vars.SetVar("is_prince", true);
								text3 = "KilledEnemyPrince";
							}
							else if (a2 != "Rebel")
							{
								text3 = "KilledEnemyKnight";
							}
						}
					}
					else
					{
						text3 = "KilledEnemyWeHaveNoLeader";
					}
				}
				if (text3 == null)
				{
					text3 = "WonBattle";
				}
				text = text3;
				text2 = "WonBattleSFX";
			}
			else if (this.logic.settlement != null)
			{
				if (this.logic.is_siege)
				{
					if (this.logic.settlement.type == "Castle")
					{
						if (global::Battle.PlayerIsAttacker(this.logic, true))
						{
							if (this.logic.attacker_kingdom == BaseUI.LogicKingdom())
							{
								if (this.logic.GetRealm().IsTradeCenter())
								{
									text = "WonEnemyCastleTradeCenter";
								}
								else
								{
									text = "WonEnemyCastle";
								}
							}
							else
							{
								text = "WonBattle";
							}
						}
						else if (this.logic.defender_kingdom == BaseUI.LogicKingdom())
						{
							text = "WonOwnCastle";
						}
						else
						{
							text = "WonAllyCastle";
						}
					}
					text2 = "WonSiegeSFX";
				}
				else if (global::Battle.PlayerIsAttacker(this.logic, true))
				{
					text = "WonEnemyPlunder";
					text2 = "WonEnemyPlunderSFX";
				}
			}
		}
		else
		{
			voice_vars = new Vars();
			string text4 = null;
			if (this.logic.imprisoned_at_end_of_battle.Count > 0)
			{
				text2 = "ImprisonedSFX";
			}
			if (this.logic.killed_nobles.Count <= 0)
			{
				for (int k = 0; k < this.logic.imprisoned_at_end_of_battle.Count; k++)
				{
					string a3 = this.logic.imprisoned_at_end_of_battle[k];
					if (this.logic.imprisoned_at_end_of_battle_characters.Count > 0)
					{
						voice_vars.Set<Logic.Character>("character", this.logic.imprisoned_at_end_of_battle_characters[0]);
					}
					if (a3 == "King")
					{
						voice_vars.Set<bool>("is_king", true);
						text4 = "ImprisonedOwnKing";
					}
					else if (a3 == "Prince")
					{
						text4 = "ImprisonedOwnPrince";
						voice_vars.Set<bool>("is_prince", true);
					}
					else
					{
						text4 = "ImprisonedOwnKnight";
					}
				}
			}
			else
			{
				for (int l = 0; l < this.logic.killed_nobles.Count; l++)
				{
					string a4 = this.logic.killed_nobles[l];
					if (a4 == "King")
					{
						text4 = "KilledOwnKing";
						voice_vars.Set<bool>("is_king", true);
					}
					else if (a4 == "Prince")
					{
						text4 = "KilledOwnPrince";
						voice_vars.Set<bool>("is_prince", true);
					}
				}
			}
			if (text4 != null)
			{
				text = text4;
			}
			else if (this.logic.type == Logic.Battle.Type.OpenField || this.logic.type == Logic.Battle.Type.Naval)
			{
				text = "LostBattle";
				if (text2 == null)
				{
					text2 = "LostBattleSFX";
				}
			}
			else if (this.logic.settlement != null)
			{
				if (this.logic.is_plunder)
				{
					if (text2 == null)
					{
						text2 = "LostBattleSFX";
					}
					if (is_attacker)
					{
						text = "LostEnemySettlement";
					}
					else
					{
						text = "LostOwnSettlement";
					}
				}
				else if (this.logic.is_siege)
				{
					Castle castle = this.logic.settlement as Castle;
					if (this.logic.settlement.type == "Castle")
					{
						if (is_attacker)
						{
							if (castle.GetKingdom().IsOwnStance(BaseUI.LogicKingdom()) && castle.IsOccupied())
							{
								text = "LostOwnCastleOccupiedByEnemy";
							}
							else
							{
								text = "LostEnemyCastle";
							}
						}
						else if (castle != null && castle.sacked && this.logic.settlement.IsOwnStance(this.logic.defender))
						{
							text = "SackedOwnCastle";
						}
						else if (this.logic.attacker_kingdom.IsRebelKingdom())
						{
							text = "LostOwnCastleToRebels";
						}
						else
						{
							text = "LostOwnCastle";
						}
					}
					if (text2 == null)
					{
						text2 = "LostSiegeSFX";
					}
				}
			}
		}
		if (text != null)
		{
			DT.Field field = this.logic.def.field.FindChild(text, null, true, true, true, '.');
			if (field == null)
			{
				field = BaseUI.soundsDef.FindChild(text, null, true, true, true, '.');
			}
			BaseUI.PlayVoiceEvent(field, voice_vars);
		}
		if (text2 != null)
		{
			DT.Field field2 = this.logic.def.field.FindChild(text2, null, true, true, true, '.');
			if (field2 == null)
			{
				field2 = BaseUI.soundsDef.FindChild(text2, null, true, true, true, '.');
			}
			BaseUI.PlaySound(field2);
		}
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x0005197B File Offset: 0x0004FB7B
	public void UpdatePVFigureVisiblity(bool visible)
	{
		if (this.ui_pvFigure != null)
		{
			this.ui_pvFigure.UpdateVisibilityFromObject(visible);
		}
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x00051998 File Offset: 0x0004FB98
	public void UpdateStatusBarVisibility(bool visible)
	{
		if (this.statusBar != null)
		{
			this.statusBar.UpdateVisibilityFromObject(visible);
			if (ViewMode.IsPoliticalView())
			{
				this.statusBar.UpdateVisibilityFromView(ViewMode.current.allowedFigures);
				return;
			}
			this.statusBar.UpdateVisibilityFromView((ViewMode.AllowedFigures)(-1));
		}
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x000519E8 File Offset: 0x0004FBE8
	public void UpdatePVFigureVisiblity(ViewMode.AllowedFigures allowedFigures)
	{
		if (this.ui_pvFigure != null)
		{
			this.ui_pvFigure.UpdateVisibilityFromView(allowedFigures);
		}
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x00051A04 File Offset: 0x0004FC04
	private void UpdatePVFigure()
	{
		if (this.ui_pvFigure == null)
		{
			GameObject obj = global::Defs.GetObj<GameObject>(global::Defs.GetDefField("PoliticalView", "pv_figures.Battle"), "ui_figure_prefab", null);
			GameObject prefab = obj;
			WorldUI worldUI = WorldUI.Get();
			GameObject gameObject = global::Common.Spawn(prefab, (worldUI != null) ? worldUI.m_statusBar : null, false, "");
			this.ui_pvFigure = ((gameObject != null) ? gameObject.GetComponent<UIPVFigureBattle>() : null);
		}
		UIPVFigureBattle uipvfigureBattle = this.ui_pvFigure;
		if (uipvfigureBattle == null)
		{
			return;
		}
		uipvfigureBattle.SetBattle(this);
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x00051A7A File Offset: 0x0004FC7A
	public static GameObject StatusBarPrefab()
	{
		return global::Defs.GetObj<GameObject>(global::Defs.GetDefField("BattleStatusBar", null), "window_prefab", null);
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x00051A92 File Offset: 0x0004FC92
	public static GameObject ParticlesPrefab()
	{
		return global::Defs.GetObj<GameObject>(global::Defs.GetDefField("Battle", null), "particles", null);
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x00051AAC File Offset: 0x0004FCAC
	private void UpdateStatusBar()
	{
		if (this.logic != null && this.logic.battle_map_only)
		{
			return;
		}
		if (this.statusBar == null)
		{
			GameObject prefab = global::Battle.StatusBarPrefab();
			WorldUI worldUI = WorldUI.Get();
			GameObject gameObject = global::Common.Spawn(prefab, (worldUI != null) ? worldUI.m_statusBar : null, false, "");
			this.statusBar = ((gameObject != null) ? gameObject.GetComponent<UIBattleStatusBar>() : null);
			if (this.statusBar != null)
			{
				this.statusBar.transform.position = new Vector3(-1000f, 0f, -1000f);
			}
		}
		if (this.statusBar != null)
		{
			this.statusBar.SetBattle(this.logic);
		}
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x00051B64 File Offset: 0x0004FD64
	public static void CreateVisuals(Logic.Object logic_obj)
	{
		Logic.Battle battle = logic_obj as Logic.Battle;
		if (battle == null)
		{
			return;
		}
		GameObject gameObject = new GameObject(battle.type.ToString());
		global::Common.SetObjectParent(gameObject, GameLogic.instance.transform, "Battles");
		Point pt = battle.position;
		gameObject.transform.position = global::Common.SnapToTerrain(pt, 0f, null, -1f, false);
		global::Battle battle2 = gameObject.AddComponent<global::Battle>();
		battle2.logic = battle;
		battle2.logic.visuals = battle2;
		GameObject obj = global::Defs.GetObj<GameObject>(battle.def.field, "prefab", null);
		if (obj != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(obj, gameObject.transform);
			if (battle.is_siege && battle.settlement is Castle)
			{
				battle2.FixSiegeLabel();
			}
			gameObject2.transform.localPosition = Vector3.zero;
		}
		if (battle2.battle_sound_emitter == null)
		{
			battle2.battle_sound_emitter = gameObject.gameObject.AddComponent<StudioEventEmitter>();
			battle2.battle_sound_emitter.PlayEvent = EmitterGameEvent.ObjectEnable;
			battle2.battle_sound_emitter.StopEvent = EmitterGameEvent.ObjectDisable;
			battle2.UpdateSoundLoop();
		}
		battle2.Init();
		battle2.CreateProgressIcon();
		battle2.UpdatePVFigure();
		battle2.UpdateStatusBar();
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x00051CA0 File Offset: 0x0004FEA0
	private bool particlesValid()
	{
		return this.logic.type != Logic.Battle.Type.Naval && this.logic.stage == Logic.Battle.Stage.Ongoing && this.visible;
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x00051CCB File Offset: 0x0004FECB
	public void RefreshParticles(bool simulate)
	{
		if (this.particlesValid())
		{
			this.CreateParticles(simulate);
			return;
		}
		this.StopParticles(false);
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x00051CE4 File Offset: 0x0004FEE4
	private void CreateParticles(bool simulate)
	{
		if (this.particles == null)
		{
			this.particles = global::Common.Spawn(global::Battle.ParticlesPrefab(), base.transform.parent, false, "");
			if (this.particles == null)
			{
				return;
			}
			this.particles.transform.position = base.transform.position;
			this.particles.transform.localScale = Vector3.one;
		}
		if (this.particles == null)
		{
			return;
		}
		ParticleSystem[] componentsInChildren = this.particles.GetComponentsInChildren<ParticleSystem>();
		float num = UnityEngine.Time.time - this.last_visible_time;
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			if (simulate)
			{
				particleSystem.Simulate(num + particleSystem.time);
			}
			if (!particleSystem.isPlaying)
			{
				particleSystem.Play();
			}
		}
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x00051DB8 File Offset: 0x0004FFB8
	private void StopParticles(bool instant)
	{
		if (this.particles == null)
		{
			return;
		}
		ParticleSystem[] componentsInChildren = this.particles.GetComponentsInChildren<ParticleSystem>();
		float num = 0f;
		if (!instant)
		{
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				ParticleSystem.MainModule main = particleSystem.main;
				particleSystem.subEmitters.enabled = false;
				if (main.duration > num)
				{
					num = main.duration;
				}
				particleSystem.Stop();
			}
		}
		UnityEngine.Object.Destroy(this.particles, num);
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x00051E34 File Offset: 0x00050034
	private void CreateProgressIcon()
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null || this.logic.attacker_kingdom == null || this.logic.defender_kingdom == null)
		{
			return;
		}
		int num = worldUI.kingdom;
		if (num != this.logic.attacker_kingdom.id && num != this.logic.defender_kingdom.id)
		{
			int num2 = num;
			Logic.Army attacker_support = this.logic.attacker_support;
			if (num2 != ((attacker_support != null) ? attacker_support.GetKingdom().id : 0))
			{
				int num3 = num;
				Logic.Army defender_support = this.logic.defender_support;
				if (num3 != ((defender_support != null) ? defender_support.GetKingdom().id : 0))
				{
					return;
				}
			}
		}
		MessageIcon.Create(this, true);
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x00051EE4 File Offset: 0x000500E4
	public void FixSiegeLabel()
	{
		if (this.logic.settlement == null || this.logic.settlement.visuals == null)
		{
			this.must_fix_siege_label = true;
			return;
		}
		GameObject gameObject = base.transform.GetChild(0).gameObject;
		if (gameObject == null)
		{
			this.must_fix_siege_label = true;
			return;
		}
		global::Settlement settlement = this.logic.settlement.visuals as global::Settlement;
		if (settlement.name_label == null || !settlement.visible)
		{
			this.must_fix_siege_label = true;
			return;
		}
		this.must_fix_siege_label = false;
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			Billboard component = gameObject.transform.GetChild(i).GetComponent<Billboard>();
			if (!(component == null))
			{
				component.parent_object = settlement.name_label;
				component.offset = Vector3.up * 6f;
			}
		}
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x00051FC8 File Offset: 0x000501C8
	public static bool PlayerIsDefender(Logic.Battle battle, bool merc_too = true)
	{
		return battle != null && battle.defender_kingdom != null && !(WorldUI.Get() == null) && (global::Battle.GetPlayerDefendingArmy(battle, merc_too) != null || battle.defender_kingdom == BaseUI.LogicKingdom());
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x00051FFE File Offset: 0x000501FE
	public static bool PlayerIsAttacker(Logic.Battle battle, bool merc_too = true)
	{
		return global::Battle.GetPlayerAttackingArmy(battle, merc_too) != null;
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x0005200C File Offset: 0x0005020C
	public static Logic.Army GetPlayerAttackingArmy(Logic.Battle battle, bool merc_too = true)
	{
		if (battle == null)
		{
			return null;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return null;
		}
		List<Logic.Army> armies = battle.GetArmies(0);
		for (int i = 0; i < armies.Count; i++)
		{
			if (armies[i].kingdom_id == baseUI.kingdom.id && (!armies[i].IsHiredMercenary() || merc_too))
			{
				return armies[i];
			}
		}
		return null;
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x0005207C File Offset: 0x0005027C
	public static Logic.Army GetPlayerDefendingArmy(Logic.Battle battle, bool merc_too = true)
	{
		if (battle == null)
		{
			return null;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return null;
		}
		List<Logic.Army> armies = battle.GetArmies(1);
		for (int i = 0; i < armies.Count; i++)
		{
			if (armies[i].kingdom_id == baseUI.kingdom.id && (!armies[i].IsHiredMercenary() || merc_too))
			{
				return armies[i];
			}
		}
		return null;
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x000520EC File Offset: 0x000502EC
	public static void CalcSides(Logic.Battle logic, out int side_left, out int side_right, bool use_x_location = false)
	{
		Logic.Kingdom k = BaseUI.LogicKingdom();
		RelationUtils.Stance stance = logic.attacker_kingdom.GetStance(k);
		RelationUtils.Stance stance2 = logic.defender_kingdom.GetStance(k);
		side_left = 0;
		side_right = 1;
		if (use_x_location)
		{
			float num = float.MaxValue;
			side_left = 0;
			for (int i = 0; i <= 1; i++)
			{
				Logic.Army army = logic.GetArmy(i);
				if (army != null)
				{
					float x = army.position.x;
					if (x < num)
					{
						num = x;
						side_left = i;
					}
				}
			}
			side_right = 1 - side_left;
			return;
		}
		if (global::Battle.PlayerIsAttacker(logic, true))
		{
			side_left = 0;
			side_right = 1;
			return;
		}
		if (global::Battle.PlayerIsDefender(logic, true))
		{
			side_left = 1;
			side_right = 0;
			return;
		}
		if ((stance & RelationUtils.Stance.Own) != RelationUtils.Stance.None)
		{
			side_left = 0;
			side_right = 1;
			return;
		}
		if ((stance2 & RelationUtils.Stance.Own) != RelationUtils.Stance.None)
		{
			side_left = 1;
			side_right = 0;
			return;
		}
		if ((stance & RelationUtils.Stance.War) != RelationUtils.Stance.None && (stance2 & RelationUtils.Stance.War) == RelationUtils.Stance.None)
		{
			side_left = 1;
			side_right = 0;
		}
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x000521BA File Offset: 0x000503BA
	private bool FillBar()
	{
		this.UpdateEstimation();
		return true;
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x000521C3 File Offset: 0x000503C3
	public static string GetEstimationKey(float estimation)
	{
		if (estimation < 0.35f)
		{
			return "losing_badly";
		}
		if (estimation < 0.45f)
		{
			return "losing";
		}
		if (estimation < 0.55f)
		{
			return "balanced";
		}
		if (estimation < 0.65f)
		{
			return "winning";
		}
		return "winning_decisively";
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x00052202 File Offset: 0x00050402
	public static string GetEstimationText(string key)
	{
		return global::Defs.Localize("Battle.estimation_texts." + key, null, null, true, true);
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x00052218 File Offset: 0x00050418
	public static Color GetEstimationColor(string key)
	{
		return global::Defs.GetColor("Battle", "estimation_colors." + key);
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x0005222F File Offset: 0x0005042F
	public static Color GetEstimationBarColor(string key)
	{
		return global::Defs.GetColor("Battle", "estimation_bar_colors." + key);
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x00052246 File Offset: 0x00050446
	public static Color GetMoraleColor(string key)
	{
		return global::Defs.GetColor("Battle", "morale_colors." + key);
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x00052260 File Offset: 0x00050460
	public static Color GetMoraleColor(Logic.Object obj, string key)
	{
		Logic.Kingdom obj2 = BaseUI.LogicKingdom();
		if (obj.IsEnemy(obj2))
		{
			return global::Battle.GetMoraleColor(key + "_enemy");
		}
		return global::Battle.GetMoraleColor(key + "_player");
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x000522A0 File Offset: 0x000504A0
	public static void SetEstimationColor(GameObject obj, float estimation)
	{
		if (obj == null)
		{
			return;
		}
		Color estimationColor = global::Battle.GetEstimationColor(global::Battle.GetEstimationKey(estimation));
		Image component = obj.GetComponent<Image>();
		if (component != null)
		{
			component.color = estimationColor;
			return;
		}
		Renderer componentInChildren = obj.GetComponentInChildren<Renderer>();
		if (componentInChildren != null)
		{
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			materialPropertyBlock.SetColor("_Color", estimationColor);
			componentInChildren.SetPropertyBlock(materialPropertyBlock);
			return;
		}
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x00052308 File Offset: 0x00050508
	public static void UpdateEstimationBar(GameObject bar, float estimation)
	{
		if (bar == null)
		{
			return;
		}
		GameObject gameObject = global::Common.FindChildByName(bar, "id_left_estimation", true, true);
		GameObject gameObject2 = global::Common.FindChildByName(bar, "id_right_estimation", true, true);
		if (gameObject != null)
		{
			gameObject.transform.localScale = new Vector3(1f - estimation, 1f, 1f);
		}
		if (gameObject2 != null)
		{
			gameObject2.transform.localScale = new Vector3(estimation, 1f, 1f);
		}
		if (estimation >= 0.4f && estimation < 0.5f)
		{
			estimation = 0.3f;
		}
		else if (estimation >= 0.5f && estimation < 0.6f)
		{
			estimation = 0.7f;
		}
		global::Battle.SetEstimationColor(gameObject, 1f - estimation);
		global::Battle.SetEstimationColor(gameObject2, estimation);
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x000523D0 File Offset: 0x000505D0
	private void UpdateEstimation()
	{
		if (this.logic == null || this.logic.simulation == null)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		Logic.Army attacker_support = this.logic.attacker_support;
		bool flag = ((attacker_support != null) ? attacker_support.GetKingdom().id : 0) == kingdom.id;
		Logic.Army defender_support = this.logic.defender_support;
		bool flag2 = ((defender_support != null) ? defender_support.GetKingdom().id : 0) == kingdom.id;
		Logic.Kingdom attacker_kingdom = this.logic.attacker_kingdom;
		bool flag3 = ((attacker_kingdom != null) ? attacker_kingdom.id : 0) == kingdom.id || flag;
		Logic.Kingdom defender_kingdom = this.logic.defender_kingdom;
		bool flag4 = ((defender_kingdom != null) ? defender_kingdom.id : 0) == kingdom.id || flag;
		if (flag3 || flag4)
		{
			float num = this.logic.simulation.estimation - this.logic.simulation.initial_estimation;
			float num2 = this.logic.simulation.attacker_totals.last_count / this.logic.simulation.attacker_totals.initial_count;
			float num3 = this.logic.simulation.defender_totals.last_count / this.logic.simulation.defender_totals.initial_count;
			float num4 = this.logic.simulation.attacker_totals.count / this.logic.simulation.attacker_totals.initial_count;
			float num5 = this.logic.simulation.defender_totals.count / this.logic.simulation.defender_totals.initial_count;
			bool flag5 = false;
			bool flag6 = false;
			if (num2 > 0.5f && num4 <= 0.5f && num > 0.1f)
			{
				if (flag3)
				{
					flag5 = true;
				}
				else
				{
					flag6 = true;
				}
			}
			else if (num3 > 0.5f && num5 <= 0.5f && num < -0.1f)
			{
				if (flag4)
				{
					flag5 = true;
				}
				else
				{
					flag6 = true;
				}
			}
			if (flag5 || flag6)
			{
				Logic.Character vars = null;
				if (flag3)
				{
					Logic.Character character;
					if (!flag)
					{
						Logic.Army attacker = this.logic.attacker;
						character = ((attacker != null) ? attacker.leader : null);
					}
					else
					{
						Logic.Army attacker_support2 = this.logic.attacker_support;
						character = ((attacker_support2 != null) ? attacker_support2.leader : null);
					}
					vars = character;
				}
				else if (flag2)
				{
					Logic.Army defender_support2 = this.logic.defender_support;
					vars = ((defender_support2 != null) ? defender_support2.leader : null);
				}
				else
				{
					MapObject defender = this.logic.defender;
					if (defender != null)
					{
						Logic.Army army;
						if ((army = (defender as Logic.Army)) == null)
						{
							Castle castle;
							if ((castle = (defender as Castle)) != null)
							{
								Logic.Army army2 = castle.army;
								vars = ((army2 != null) ? army2.leader : null);
							}
						}
						else
						{
							vars = army.leader;
						}
					}
				}
				if (flag5)
				{
					BaseUI.PlayVoiceEvent(this.logic.def.field.FindChild("HeavyLosses", null, true, true, true, '.'), vars);
					return;
				}
				BaseUI.PlayVoiceEvent(this.logic.def.field.FindChild("HeavyLossesEnemy", null, true, true, true, '.'), vars);
			}
		}
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x000526C0 File Offset: 0x000508C0
	private void UpdateBar()
	{
		this.FillBar();
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x000526C9 File Offset: 0x000508C9
	public bool IsPreparing()
	{
		return this.logic.stage == Logic.Battle.Stage.Preparing;
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x000526D9 File Offset: 0x000508D9
	public bool IsOngoing()
	{
		return this.logic.stage == Logic.Battle.Stage.Ongoing && this.logic.type != Logic.Battle.Type.Plunder;
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x000526FC File Offset: 0x000508FC
	public bool IsOngoingPlunder()
	{
		return this.logic.stage == Logic.Battle.Stage.Ongoing && this.logic.type == Logic.Battle.Type.Plunder;
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x0005271C File Offset: 0x0005091C
	private void UpdateStatus()
	{
		if (this.logic == null)
		{
			return;
		}
		UIPVFigureBattle uipvfigureBattle = this.ui_pvFigure;
		if (uipvfigureBattle == null)
		{
			return;
		}
		uipvfigureBattle.UpdateStatus();
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x00052738 File Offset: 0x00050938
	public void Init()
	{
		Logic.Battle battle = this.logic;
		if (((battle != null) ? battle.simulation : null) != null)
		{
			this.logic.simulation.CalcTotals(false, false);
		}
		this.UpdateBar();
		this.UpdateStatus();
		this.last_visible_time = UnityEngine.Time.time;
		this.visibility_index = VisibilityDetector.Add(base.transform.position, 20f, null, this, base.gameObject.layer);
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x000527AC File Offset: 0x000509AC
	private void UpdateSoundLoop()
	{
		if (BattleMap.battle != null && BattleMap.battle.battle_map_only)
		{
			return;
		}
		if (this.battle_sound_emitter != null)
		{
			string text = null;
			if (this.visible && this.logic.stage == Logic.Battle.Stage.Ongoing)
			{
				text = this.logic.def.field.GetString("ongoing_sound", null, "", true, true, true, '.');
			}
			this.battle_sound_emitter.Stop();
			this.battle_sound_emitter.Event = text;
			if (!this.visible)
			{
				return;
			}
			if (text != null)
			{
				this.battle_sound_emitter.Play();
			}
		}
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x00052848 File Offset: 0x00050A48
	private void OnDestroy()
	{
		this.DestroySelection();
		this.ClearUnits();
		if (this.logic != null)
		{
			Logic.Battle battle = this.logic;
			if (BattleMap.battle == battle)
			{
				BattleMap.SetBattle(null, 0);
			}
			this.logic = null;
			battle.Destroy(false);
		}
		this.StopParticles(Game.isLoadingSaveGame);
		if (this.statusBar != null)
		{
			this.statusBar.SetBattle(null);
			global::Common.DestroyObj(this.statusBar.gameObject);
		}
		if (this.ui_pvFigure != null)
		{
			global::Common.DestroyObj(this.ui_pvFigure.gameObject);
		}
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x000528E0 File Offset: 0x00050AE0
	public override void OnMessage(object obj, string message, object param)
	{
		if (this.logic == null)
		{
			return;
		}
		Vars vars = this.Vars();
		if (message == "broken_battle_report")
		{
			GameCapture.Capture(null, "Broken_Battle_" + this.logic + "_host", null, null, null, null, false);
			return;
		}
		if (message == "started")
		{
			this.UpdateSelectionUI();
			this.RefreshUnits();
			return;
		}
		if (message == "stage_changed")
		{
			this.UpdateUnitVisibility();
			this.UpdateSoundLoop();
			this.UpdateSelectionUI();
			this.RefreshParticles(false);
			return;
		}
		if (message == "type_changed")
		{
			this.UpdateSoundLoop();
			this.UpdateSelectionUI();
			this.RefreshUnits();
			Logic.Battle.Type type = (Logic.Battle.Type)param;
			if (BaseUI.LogicKingdom() == null)
			{
				return;
			}
			Logic.Army playerAttackingArmy = global::Battle.GetPlayerAttackingArmy(this.logic, true);
			if (playerAttackingArmy != null && type == Logic.Battle.Type.BreakSiege && this.logic.type == Logic.Battle.Type.Siege)
			{
				MessageIcon.Create("DefendersRetreatedIntoCastle", vars, true, playerAttackingArmy.leader);
			}
			return;
		}
		else if (message == "armies_changed")
		{
			this.UpdateBar();
			this.UpdateSelectionUI();
			this.RefreshUnits();
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom == null)
			{
				return;
			}
			BattleMap battleMap = BattleMap.Get();
			if (battleMap != null)
			{
				battleMap.AssignColors();
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = BattleMap.Get() == null;
			Logic.Army army = param as Logic.Army;
			float time = UnityEngine.Time.time;
			float num = this.last_join_time;
			this.last_join_time = UnityEngine.Time.time;
			UIPVFigureBattle uipvfigureBattle = this.ui_pvFigure;
			if (uipvfigureBattle != null)
			{
				uipvfigureBattle.Refresh();
			}
			if (army != null)
			{
				vars = new Vars(vars);
				vars.Set<Logic.Army>("joined_army", army);
				vars.Set<Logic.Character>("character", army.leader);
				if ((!this.logic.is_siege || this.logic.attackers.Count > 1) && this.logic.attackers.Contains(army))
				{
					flag = true;
				}
				else if ((!this.logic.is_siege || this.logic.defenders.Count > 1) && this.logic.defenders.Contains(army))
				{
					flag2 = true;
				}
				if ((flag2 || flag) && army.kingdom_id != kingdom.id && !army.IsEnemy(kingdom))
				{
					flag3 = true;
				}
				if (army.kingdom_id == kingdom.id && army.battle == this.logic && ((army.kingdom_id != this.logic.attacker_kingdom.id && army.IsOwnStance(this.logic.attacker_support)) || (army.kingdom_id != this.logic.defender_kingdom.id && army.IsOwnStance(this.logic.defender_support))))
				{
					this.CreateProgressIcon();
				}
			}
			if (kingdom.id == this.logic.attacker_kingdom.id)
			{
				if (flag)
				{
					if (flag3)
					{
						if (!flag4)
						{
							MessageIcon.Create("JoinedBattleOursAlly", vars, true, null);
						}
						else
						{
							BaseUI.PlayVoiceEvent(this.logic.def.field.GetString("JoinedBattleOursAlly", null, "", true, true, true, '.'), null);
						}
					}
					else if (this.logic.attackers.Count == 2)
					{
						DT.Field soundsDef = BaseUI.soundsDef;
						BaseUI.PlayVoiceEvent((soundsDef != null) ? soundsDef.GetString("player_reinforcements_arrived", null, "", true, true, true, '.') : null, null);
					}
				}
				if (flag2)
				{
					if (this.logic.is_plunder && this.logic.defenders.Count == 1)
					{
						if (!flag4)
						{
							MessageIcon.Create("JoinedBattleTheirsPlunder", vars, true, null);
							return;
						}
						BaseUI.PlayVoiceEvent(this.logic.def.field.GetString("JoinedBattleTheirsPlunder", null, "", true, true, true, '.'), vars);
						return;
					}
					else
					{
						if (!flag4)
						{
							MessageIcon.Create("JoinedBattleTheirs", vars, true, null);
							return;
						}
						BaseUI.PlayVoiceEvent(this.logic.def.field.GetString("JoinedBattleTheirs", null, "", true, true, true, '.'), vars);
						return;
					}
				}
			}
			else if (kingdom.id == this.logic.defender_kingdom.id)
			{
				if (flag)
				{
					if (!flag4)
					{
						MessageIcon.Create("JoinedBattleTheirs", vars, true, null);
					}
					else
					{
						BaseUI.PlayVoiceEvent(this.logic.def.field.GetString("JoinedBattleTheirs", null, "", true, true, true, '.'), vars);
					}
				}
				if (flag2)
				{
					if (flag3)
					{
						if (!flag4)
						{
							MessageIcon.Create("JoinedBattleOursAlly", vars, true, null);
							return;
						}
						BaseUI.PlayVoiceEvent(this.logic.def.field.GetString("JoinedBattleOursAlly", null, "", true, true, true, '.'), vars);
						return;
					}
					else if (this.logic.defenders.Count == 2)
					{
						DT.Field soundsDef2 = BaseUI.soundsDef;
						BaseUI.PlayVoiceEvent((soundsDef2 != null) ? soundsDef2.GetString("player_reinforcements_arrived", null, "", true, true, true, '.') : null, vars);
					}
				}
			}
			return;
		}
		else
		{
			if (message == "vars_changed")
			{
				if (this.logic.IsFinishing())
				{
					this.CreateAftermathMessage(this.logic.winner);
				}
				return;
			}
			if (message == "changed")
			{
				this.UpdateEstimation();
				return;
			}
			if (message == "break_siege")
			{
				this.UpdateSelectionUI();
				Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
				if (kingdom2 != null)
				{
					if (kingdom2.id == this.logic.attacker_kingdom.id)
					{
						if (this.logic.defender_support != null || this.logic.defenders.Count == 2)
						{
							MessageIcon.Create("BreakSiegeTheirsJoin", vars, true, null);
						}
						else
						{
							MessageIcon.Create("BreakSiegeTheirs", vars, true, null);
						}
					}
					else if (kingdom2.id == this.logic.defender_kingdom.id)
					{
						BackgroundMusic.OnTrigger("SiegePreparationTrigger", null);
						Logic.Army defender_support = this.logic.defender_support;
						if (((defender_support != null) ? defender_support.GetKingdom() : null) != null)
						{
							Logic.Army defender_support2 = this.logic.defender_support;
							int? num2 = (defender_support2 != null) ? new int?(defender_support2.GetKingdom().id) : null;
							int id = kingdom2.id;
							if (!(num2.GetValueOrDefault() == id & num2 != null))
							{
								MessageIcon.Create("BreakSiegeOursJoin", vars, true, null);
							}
						}
					}
					List<Logic.Army> armies = this.logic.GetArmies(1);
					if (armies != null && armies.Count > 0)
					{
						Logic.Character leader = armies[armies.Count - 1].leader;
						if (leader != null && kingdom2.IsOwnStance(leader))
						{
							vars = new Vars();
							vars.Set<Logic.Character>("character", leader);
							vars.Set<Logic.Battle>("battle", this.logic);
							Logic.Battle.BreakSiegeFrom breakSiegeFrom = (Logic.Battle.BreakSiegeFrom)param;
							if (breakSiegeFrom == Logic.Battle.BreakSiegeFrom.Inside)
							{
								BaseUI.PlayVoiceEvent(this.logic.attacker.leader.GetVoiceLine("BreakSiegeFromInside"), leader);
								return;
							}
							if (breakSiegeFrom != Logic.Battle.BreakSiegeFrom.Outside)
							{
								return;
							}
							BaseUI.PlayVoiceEvent(this.logic.attacker.leader.GetVoiceLine("BreakSiegeFromOutside"), leader);
						}
					}
				}
				return;
			}
			if (message == "can_assault")
			{
				Logic.Kingdom kingdom3 = BaseUI.LogicKingdom();
				if (kingdom3 != null && kingdom3.id == this.logic.attacker_kingdom.id)
				{
					DT.Field soundsDef3 = BaseUI.soundsDef;
					BaseUI.PlayVoiceEvent((soundsDef3 != null) ? soundsDef3.GetString("narrator_assault_can_start", null, "", true, true, true, '.') : null, null);
				}
			}
			if (message == "assault")
			{
				this.UpdateSelectionUI();
				Logic.Kingdom kingdom4 = BaseUI.LogicKingdom();
				if (kingdom4 != null)
				{
					if (kingdom4.id == this.logic.defender_kingdom.id)
					{
						MessageIcon.Create("AssaultOurs", vars, true, null);
						return;
					}
					if (kingdom4.id == this.logic.attacker_kingdom.id)
					{
						BackgroundMusic.OnTrigger("SiegePreparationTrigger", null);
						if (this.logic.attacker.leader != null)
						{
							BaseUI.PlayVoiceEvent(this.logic.attacker.leader.GetVoiceLine("AssaultTown"), this.logic.attacker.leader);
						}
					}
				}
				return;
			}
			if (message == "enter_battle")
			{
				if (WorldUI.Get() == null)
				{
					return;
				}
				Logic.Battle battle = this.logic;
				if (((battle != null) ? battle.settlement : null) != null)
				{
					BattleMap.settlement_id = this.logic.settlement.GetNid(true);
				}
				else
				{
					BattleMap.settlement_id = -1;
				}
				BattleMap.wv_battle_pos = this.logic.position;
				BattleViewLoader.LoadBattle(this.logic, BaseUI.LogicKingdom().id);
				return;
			}
			else if (message == "gold_won")
			{
				Vars vars2 = param as Vars;
				Logic.Kingdom kingdom5 = vars2.Get<Logic.Kingdom>("kingdom", null);
				Logic.Kingdom kingdom6 = BaseUI.LogicKingdom();
				if (kingdom5 != kingdom6)
				{
					return;
				}
				FloatingText.Create(base.gameObject, "FloatingTexts.BattleWealthGained", "battle_wealth_gained", vars2, false);
				return;
			}
			else
			{
				if (message == "destroying" || message == "finishing")
				{
					this.UnregisterToMinimap();
					this.logic.visuals = null;
					this.logic.DelListener(this);
					this.logic = null;
					if (this.selected)
					{
						WorldUI worldUI = WorldUI.Get();
						if (worldUI != null)
						{
							worldUI.SelectObj((worldUI.select_target == base.gameObject) ? null : worldUI.select_target, false, true, true, true);
						}
					}
					UnityEngine.Object.DestroyImmediate(base.gameObject);
					return;
				}
				if (!(message == "army_retreat"))
				{
					return;
				}
				Logic.Kingdom kingdom7 = BaseUI.LogicKingdom();
				Logic.Army army2 = (Logic.Army)param;
				if (army2 == null)
				{
					return;
				}
				Logic.Kingdom kingdom8 = army2.GetKingdom();
				if (kingdom7 == kingdom8)
				{
					if (army2.leader != null)
					{
						BaseUI.PlayVoiceEvent(this.logic.def.field.GetString("OurArmyRetreatedVoice", null, "", true, true, true, '.'), army2.leader);
					}
					BaseUI.PlaySoundEvent(this.logic.def.field.GetString("OurArmyRetreatedSFX", null, "", true, true, true, '.'), null);
				}
				return;
			}
		}
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x000532CE File Offset: 0x000514CE
	public void VisibilityChanged(bool visible)
	{
		this.inCameraView = visible;
		this.UpdateVisibility();
		this.UpdateSoundLoop();
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x000532E4 File Offset: 0x000514E4
	public bool CanBeSelected()
	{
		Logic.Battle battle = this.logic;
		if (((battle != null) ? battle.game : null) == null)
		{
			return false;
		}
		Logic.Kingdom k = BaseUI.LogicKingdom();
		Logic.Realm realm = this.logic.game.GetRealm(this.logic.realm_id);
		return realm == null || realm.CalcVisibleBy(k, true) > 0;
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x0005333C File Offset: 0x0005153C
	public void UpdateVisibility()
	{
		bool flag = this.visible;
		this.visible = true;
		Logic.Battle battle = this.logic;
		if (((battle != null) ? battle.game : null) == null)
		{
			return;
		}
		Logic.Realm realm = this.logic.game.GetRealm(this.logic.realm_id);
		if (realm == null)
		{
			this.UpdateMinimapIconVisibility(this.visible);
			this.UpdatePVFigureVisiblity(this.visible);
			this.UpdateStatusBarVisibility(this.visible);
			return;
		}
		Logic.Kingdom k = BaseUI.LogicKingdom();
		if (realm.CalcVisibleBy(k, true) <= 0)
		{
			this.visible = false;
		}
		MapObject mapObject = BaseUI.Get().selected_logic_obj as MapObject;
		if (mapObject == this.logic || this.logic.attackers.Contains(mapObject as Logic.Army) || this.logic.defenders.Contains(mapObject as Logic.Army) || this.logic.defender == mapObject)
		{
			GameObject selectionObj = BaseUI.Get().GetSelectionObj(base.gameObject);
			if (selectionObj != BaseUI.Get().selected_obj)
			{
				BaseUI.Get().SelectObj(selectionObj, false, true, true, true);
			}
		}
		this.UpdateMinimapIconVisibility(this.visible);
		UIPVFigureBattle uipvfigureBattle = this.ui_pvFigure;
		if (uipvfigureBattle != null)
		{
			uipvfigureBattle.UpdateVisibilityFilter();
		}
		this.UpdatePVFigureVisiblity(this.visible);
		this.UpdateStatusBarVisibility(this.visible && (this.inCameraView || ViewMode.IsPoliticalView()));
		this.visible &= this.inCameraView;
		if (!this.visible && flag)
		{
			this.last_visible_time = UnityEngine.Time.time;
		}
		if (base.gameObject.activeSelf != this.visible)
		{
			base.gameObject.SetActive(this.visible);
		}
		if (this.visible != flag && this.logic.settlement != null)
		{
			global::Settlement settlement = this.logic.settlement.visuals as global::Settlement;
			if (settlement != null)
			{
				settlement.CreateLabel(true, true);
			}
			this.UpdateUnitVisibility();
		}
		this.RefreshParticles(this.visible != flag);
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x00053545 File Offset: 0x00051745
	private void UpdateMinimapIconVisibility(bool visible)
	{
		if (visible)
		{
			this.RegisterToMinimap();
			return;
		}
		this.UnregisterToMinimap();
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x00053558 File Offset: 0x00051758
	private void RegisterToMinimap()
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null || baseUI.minimap == null)
		{
			return;
		}
		baseUI.minimap.AddObj(this.logic);
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x00053594 File Offset: 0x00051794
	private void UnregisterToMinimap()
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null || baseUI.minimap == null)
		{
			return;
		}
		baseUI.minimap.DelObj(this.logic);
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x000535D0 File Offset: 0x000517D0
	private void MoveUnitsBattle(bool instant = false)
	{
		if (this.logic == null || !global::Army.move_units_in_battle)
		{
			return;
		}
		for (int i = 0; i < this.units.Count; i++)
		{
			global::Unit unit = this.units[i];
			if (unit.logic.simulation != null)
			{
				if (unit.logic.simulation.state == BattleSimulation.Squad.State.Idle || instant)
				{
					unit.dest_pos = Vector3.zero;
					unit.dest_speed = 0f;
				}
				else
				{
					unit.MoveTowards(unit.logic.simulation.world_tgt_pos, unit.logic.def.move_speed * unit.logic.simulation.mod_move_speed, true);
				}
			}
		}
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x00053694 File Offset: 0x00051894
	private void UpdateUnits()
	{
		this.MoveUnitsBattle(false);
		for (int i = 0; i < this.units.Count; i++)
		{
			this.units[i].Update();
		}
		if (this._needs_refresh_units)
		{
			this.ApplyRefreshUnits();
		}
		if (this._needs_update_unit_visibility)
		{
			this.ApplyUpdateUnitVisibility();
		}
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x000536EC File Offset: 0x000518EC
	private void ApplyRefreshUnits()
	{
		this._needs_refresh_units = false;
		this.ClearUnits();
		Logic.Battle battle = this.logic;
		bool flag;
		if (battle == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Settlement settlement = battle.settlement;
			if (settlement == null)
			{
				flag = (null != null);
			}
			else
			{
				Garrison garrison = settlement.garrison;
				flag = (((garrison != null) ? garrison.units : null) != null);
			}
		}
		if (flag)
		{
			this.CreateUnits();
			this.MoveUnitsBattle(true);
		}
		this.UpdateUnitVisibility();
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x00053748 File Offset: 0x00051948
	public int GetRealmID()
	{
		if (this.logic != null)
		{
			return this.logic.realm_id;
		}
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return 0;
		}
		return worldMap.RealmIDAt(base.transform.position.x, base.transform.position.z);
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x000537A0 File Offset: 0x000519A0
	public static bool TentsVisible(Logic.Battle battle)
	{
		bool result = false;
		Logic.Realm realm = battle.GetRealm();
		global::Realm realm2 = ((realm != null) ? realm.visuals : null) as global::Realm;
		if ((realm2 == null || realm2.visibility > 0) && (battle.type == Logic.Battle.Type.Siege || battle.stage == Logic.Battle.Stage.Preparing))
		{
			result = true;
		}
		return result;
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x000537E8 File Offset: 0x000519E8
	private void ApplyUpdateUnitVisibility()
	{
		this._needs_update_unit_visibility = false;
		bool enabled = false;
		if (!global::Battle.TentsVisible(this.logic))
		{
			enabled = true;
		}
		for (int i = 0; i < this.units.Count; i++)
		{
			this.units[i].Enable(enabled, false);
		}
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x00053838 File Offset: 0x00051A38
	private void ClearUnits()
	{
		for (int i = this.units.Count - 1; i >= 0; i--)
		{
			this.units[i].SetLogic(null);
		}
		this.units.Clear();
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0005387C File Offset: 0x00051A7C
	private void CreateUnits()
	{
		WorldMap worldMap = WorldMap.Get();
		for (int i = 0; i < this.logic.settlement.garrison.units.Count; i++)
		{
			Logic.Unit unit = this.logic.settlement.garrison.units[i];
			global::Unit unit2 = new global::Unit(worldMap.texture_baker, GameLogic.instance.transform);
			this.units.Add(unit2);
			unit2.SetLogic(unit);
			unit2.SetPosition(unit.simulation.world_pos);
		}
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x00053910 File Offset: 0x00051B10
	public static int PlayerBattleSide()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Logic.Battle battle = BattleMap.battle;
		if (battle == null)
		{
			return -1;
		}
		int result = -1;
		if (battle.attacker_kingdom != kingdom)
		{
			Logic.Army attacker_support = battle.attacker_support;
			if (((attacker_support != null) ? attacker_support.GetKingdom() : null) != kingdom)
			{
				if (battle.defender_kingdom != kingdom)
				{
					Logic.Army defender_support = battle.defender_support;
					if (((defender_support != null) ? defender_support.GetKingdom() : null) != kingdom)
					{
						return result;
					}
				}
				return 1;
			}
		}
		result = 0;
		return result;
	}

	// Token: 0x04000640 RID: 1600
	public Logic.Battle logic;

	// Token: 0x04000641 RID: 1601
	private int visibility_index = -1;

	// Token: 0x04000642 RID: 1602
	private bool visible = true;

	// Token: 0x04000643 RID: 1603
	private bool inCameraView = true;

	// Token: 0x04000644 RID: 1604
	private float last_visible_time;

	// Token: 0x04000645 RID: 1605
	private bool selected;

	// Token: 0x04000646 RID: 1606
	private bool primarySelection = true;

	// Token: 0x04000647 RID: 1607
	private MeshRenderer selection;

	// Token: 0x04000648 RID: 1608
	public UIPVFigureBattle ui_pvFigure;

	// Token: 0x04000649 RID: 1609
	private UIBattleStatusBar statusBar;

	// Token: 0x0400064A RID: 1610
	private GameObject particles;

	// Token: 0x0400064B RID: 1611
	private StudioEventEmitter battle_sound_emitter;

	// Token: 0x0400064C RID: 1612
	private List<global::Unit> units = new List<global::Unit>();

	// Token: 0x0400064D RID: 1613
	private bool must_fix_siege_label;

	// Token: 0x0400064E RID: 1614
	private float last_join_time;

	// Token: 0x0400064F RID: 1615
	private bool aftermath_message_created;

	// Token: 0x04000650 RID: 1616
	private bool _needs_update_unit_visibility;

	// Token: 0x04000651 RID: 1617
	private bool _needs_refresh_units;
}

using System;
using System.Collections;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001B7 RID: 439
public class BattleViewOutcomeUI : MonoBehaviour, IListener
{
	// Token: 0x06001A08 RID: 6664 RVA: 0x000FC374 File Offset: 0x000FA574
	private void OnDestroy()
	{
		if (this.m_ContinueButton != null)
		{
			this.m_ContinueButton.onClick = null;
		}
		if (this.m_RestartBattleview != null)
		{
			this.m_RestartBattleview.onClick = null;
		}
		if (this.m_battleLogic != null)
		{
			this.m_battleLogic.DelListener(this);
		}
	}

	// Token: 0x06001A09 RID: 6665 RVA: 0x000FC3C9 File Offset: 0x000FA5C9
	private IEnumerator Start()
	{
		while (BattleMap.battle == null)
		{
			yield return null;
		}
		UICommon.FindComponents(this, false);
		if (this.m_ContinueButton != null)
		{
			this.m_ContinueButton.onClick = new BSGButton.OnClick(this.ContinueButton_OnClick);
		}
		if (this.m_RestartBattleview != null)
		{
			this.m_RestartBattleview.onClick = new BSGButton.OnClick(this.RestartButton_OnClick);
		}
		this.m_battleLogic = BattleMap.battle;
		this.m_battleLogic.AddListener(this);
		this.snapshot = new FMODWrapper.Snapshot("battleview_outcome_screen");
		this.m_windowDefinition = global::Defs.GetDefField("BattleViewOutcomeUI", null);
		if (this.m_Container != null)
		{
			this.m_Container.gameObject.SetActive(false);
		}
		if (this.m_FadeAnimation != null)
		{
			this.m_FadeAnimation.gameObject.SetActive(false);
		}
		yield break;
	}

	// Token: 0x06001A0A RID: 6666 RVA: 0x000FC3D8 File Offset: 0x000FA5D8
	private void OnDisable()
	{
		Tutorial.SupressTutorials(false);
	}

	// Token: 0x06001A0B RID: 6667 RVA: 0x000FC3E0 File Offset: 0x000FA5E0
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "battle_view_finished")
		{
			if (this.m_battleLogic.battle_map_finished)
			{
				this.FadeIn();
			}
			return;
		}
	}

	// Token: 0x06001A0C RID: 6668 RVA: 0x000FC404 File Offset: 0x000FA604
	private void FadeIn()
	{
		FMODVoiceProvider.ClearAllVoices();
		this.m_winnerSide = this.m_battleLogic.winner;
		Logic.Army army = this.m_battleLogic.GetArmy(1 - this.m_battleLogic.winner);
		bool flag = (this.m_winnerSide == 0 && global::Battle.PlayerIsAttacker(this.m_battleLogic, true)) || (this.m_winnerSide == 1 && global::Battle.PlayerIsDefender(this.m_battleLogic, true));
		FMODWrapper.Snapshot snapshot = this.snapshot;
		if (snapshot != null)
		{
			snapshot.StartSnapshot();
		}
		string text;
		if (flag)
		{
			text = "WonBattleview";
		}
		else
		{
			text = "LostBattleview";
		}
		Logic.Battle.VictoryReason battle_view_victory_reason = this.m_battleLogic.battle_view_victory_reason;
		if (battle_view_victory_reason != Logic.Battle.VictoryReason.Retreat)
		{
			switch (battle_view_victory_reason)
			{
			case Logic.Battle.VictoryReason.Surrender:
				break;
			case Logic.Battle.VictoryReason.CapturePoints:
				text += "CapturePoints";
				goto IL_D4;
			case Logic.Battle.VictoryReason.LeaderKilled:
				text += "LeaderDied";
				goto IL_D4;
			default:
				goto IL_D4;
			}
		}
		text += "Retreated";
		IL_D4:
		Vars vars = new Vars();
		if (flag)
		{
			BaseUI.PlaySound(this.m_battleLogic.def.field.FindChild("WonBattleview", null, true, true, true, '.'));
		}
		else
		{
			BaseUI.PlaySound(this.m_battleLogic.def.field.FindChild("LostBattleview", null, true, true, true, '.'));
			if (((army != null) ? army.leader : null) != null && !army.IsEnemy(BaseUI.LogicKingdom()) && this.m_battleLogic.battle_view_victory_reason == Logic.Battle.VictoryReason.Surrender)
			{
				vars.Set<Logic.Character>("character", army.leader);
				if (army.leader.IsKing())
				{
					text = "OurKingFled";
				}
				else if (army.leader.IsPrince())
				{
					text = "OurPrinceFled";
				}
				else
				{
					text = "OurKnightFled";
				}
			}
		}
		BaseUI.PlayVoiceEvent(this.m_battleLogic.def.field.FindChild(text, null, true, true, true, '.'), vars);
		if (this.m_FadeAnimation == null)
		{
			this.HandleBattleEnd(this.m_winnerSide);
		}
		else if (this.m_Container != null)
		{
			this.m_Container.gameObject.SetActive(false);
			Tutorial.SupressTutorials(false);
		}
		if (this.m_FadeAnimation != null)
		{
			this.m_FadeAnimation.gameObject.SetActive(true);
			this.m_FadeAnimation.duration = 1.25f;
			this.m_FadeAnimation.onFinished.AddListener(new UnityAction(this.FadeAnimation_FadeIn_OnFinished));
			this.m_FadeAnimation.PlayForward();
		}
	}

	// Token: 0x06001A0D RID: 6669 RVA: 0x000FC660 File Offset: 0x000FA860
	private void FadeAnimation_FadeIn_OnFinished()
	{
		this.m_FadeAnimation.onFinished.RemoveListener(new UnityAction(this.FadeAnimation_FadeIn_OnFinished));
		this.HandleBattleEnd(this.m_winnerSide);
		this.FadeOut();
	}

	// Token: 0x06001A0E RID: 6670 RVA: 0x000FC690 File Offset: 0x000FA890
	private void FadeOut()
	{
		this.m_FadeAnimation.onFinished.AddListener(new UnityAction(this.FadeAnimation_FadeOut_OnFinished));
		this.m_FadeAnimation.duration = 0.8f;
		this.m_FadeAnimation.PlayReverse();
	}

	// Token: 0x06001A0F RID: 6671 RVA: 0x000FC6C9 File Offset: 0x000FA8C9
	private void FadeAnimation_FadeOut_OnFinished()
	{
		this.m_FadeAnimation.onFinished.RemoveListener(new UnityAction(this.FadeAnimation_FadeOut_OnFinished));
		this.m_FadeAnimation.gameObject.SetActive(false);
	}

	// Token: 0x06001A10 RID: 6672 RVA: 0x000FC6F8 File Offset: 0x000FA8F8
	private void HandleBattleEnd(int winnerSide)
	{
		if (this.m_Container == null)
		{
			return;
		}
		this.m_Container.SetActive(true);
		Tutorial.SupressTutorials(true);
		TooltipInstance.RemovePinnedTooltips();
		int num = (BattleMap.KingdomId == BattleMap.battle.attacker_kingdom.id) ? 0 : 1;
		bool victory = winnerSide == num;
		bool isAttacker = BattleMap.KingdomId == BattleMap.battle.attacker_kingdom.id;
		this.UpdateTitle(victory);
		this.UpdateBattleName();
		this.UpdateDescriptionAndBackground(victory, isAttacker);
		this.UpdateButtons();
	}

	// Token: 0x06001A11 RID: 6673 RVA: 0x000FC781 File Offset: 0x000FA981
	private void UpdateTitle(bool victory)
	{
		if (this.m_Title == null)
		{
			return;
		}
		UIText.SetText(this.m_Title, global::Defs.Localize(this.m_windowDefinition, victory ? "victory_caption" : "defeat_caption", null, null, true, true));
	}

	// Token: 0x06001A12 RID: 6674 RVA: 0x000FC7BC File Offset: 0x000FA9BC
	private void UpdateBattleName()
	{
		if (this.m_BattleName == null)
		{
			return;
		}
		string text = string.Empty;
		switch (this.m_battleLogic.type)
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
			Logic.Settlement settlement = this.m_battleLogic.settlement;
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
			UIText.SetTextKey(this.m_BattleName, text, this.m_battleLogic.Vars(), null);
			return;
		}
		this.m_BattleName.text = this.m_battleLogic.ToString();
	}

	// Token: 0x06001A13 RID: 6675 RVA: 0x000FC894 File Offset: 0x000FAA94
	private void UpdateDescriptionAndBackground(bool victory, bool isAttacker)
	{
		string text = victory ? "Victory" : "Defeat";
		string text2 = "description";
		string text3 = string.Empty;
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		switch (this.m_battleLogic.type)
		{
		default:
			text3 = "OpenField." + text;
			break;
		case Logic.Battle.Type.Siege:
		case Logic.Battle.Type.Assault:
		case Logic.Battle.Type.BreakSiege:
			if (this.m_battleLogic.settlement != null)
			{
				Logic.Settlement settlement = this.m_battleLogic.settlement;
				bool flag = ((settlement != null) ? settlement.type : null) == "Keep";
				string text4 = flag ? "Castle" : "Town";
				bool flag2 = false;
				if (this.m_battleLogic.battle_view_victory_reason == Logic.Battle.VictoryReason.Retreat && !isAttacker)
				{
					for (int i = 0; i < this.m_battleLogic.settlement.garrison.units.Count; i++)
					{
						if (!this.m_battleLogic.settlement.garrison.units[i].IsDefeated())
						{
							flag2 = true;
						}
					}
				}
				string text5;
				if (flag2)
				{
					text = "RetreatedSiegeContinue";
					text5 = "DefenderOurs";
				}
				else if (this.m_battleLogic.battle_view_victory_reason == Logic.Battle.VictoryReason.IdleLeaveBattle)
				{
					text = "Idle";
					if (isAttacker)
					{
						text5 = "Attacker";
					}
					else
					{
						text5 = "Defender";
					}
					if (this.m_battleLogic.initiative_side == global::Battle.PlayerBattleSide())
					{
						text5 += "Ours";
					}
					else
					{
						text5 += "Theirs";
					}
				}
				else if (isAttacker)
				{
					if (victory)
					{
						if (kingdom.occupiedKeeps.Contains(this.m_battleLogic.settlement) || !flag)
						{
							text5 = "AttackerOvertaken";
						}
						else
						{
							text5 = "AttackerFreed";
						}
					}
					else
					{
						text5 = "Attacker";
					}
				}
				else if (victory)
				{
					text5 = "Defender";
				}
				else
				{
					Logic.Object controller = this.m_battleLogic.GetRealm().controller;
					bool flag3 = false;
					if (controller != null)
					{
						flag3 = (controller is Rebellion);
					}
					if (flag3)
					{
						text5 = "DefenderRebels";
					}
					else
					{
						text5 = "Defender";
					}
				}
				text3 = string.Concat(new string[]
				{
					text4,
					".",
					text,
					".",
					text5
				});
			}
			break;
		case Logic.Battle.Type.Naval:
			throw new NotImplementedException();
		}
		string key;
		string key2;
		if (!string.IsNullOrEmpty(text3))
		{
			key = text3 + "." + text2;
			key2 = text3 + ".background";
		}
		else
		{
			key = "Default." + text + "." + text2;
			key2 = "Default." + text + ".background";
		}
		if (this.m_Description != null)
		{
			string str = global::Defs.Localize(this.m_windowDefinition, key, this.m_battleLogic.Vars(), null, true, true);
			string str2 = global::Defs.Localize(this.m_windowDefinition, this.GetBattleEndReason(victory), this.m_battleLogic.Vars(), null, true, true);
			string text6 = str + " " + str2;
			UIText.SetText(this.m_Description, text6);
		}
		if (this.m_Illustration != null)
		{
			this.m_Illustration.sprite = global::Defs.GetObj<Sprite>(this.m_windowDefinition, key2, null);
		}
	}

	// Token: 0x06001A14 RID: 6676 RVA: 0x000FCBB4 File Offset: 0x000FADB4
	private string GetBattleEndReason(bool victory)
	{
		if (this.m_battleLogic.battle_view_victory_reason == Logic.Battle.VictoryReason.IdleLeaveBattle)
		{
			return "VictoryReasons.Idle." + ((this.m_battleLogic.initiative_side == global::Battle.PlayerBattleSide()) ? "Ours" : "Theirs");
		}
		if (this.m_battleLogic.battle_view_victory_reason == Logic.Battle.VictoryReason.Retreat || this.m_battleLogic.battle_view_victory_reason == Logic.Battle.VictoryReason.Surrender)
		{
			return "VictoryReasons.Fled." + (victory ? "Victory" : "Defeat");
		}
		if (this.m_battleLogic.battle_view_victory_reason == Logic.Battle.VictoryReason.CapturePoints)
		{
			return "VictoryReasons.PointsCaptured." + (victory ? "Victory" : "Defeat");
		}
		if (this.m_battleLogic.battle_view_victory_reason == Logic.Battle.VictoryReason.LeaderKilled || this.m_battleLogic.battle_view_victory_reason == Logic.Battle.VictoryReason.Combat)
		{
			return "VictoryReasons.LeaderKilled." + (victory ? "Victory" : "Defeat");
		}
		return "VictoryReasons.Fled." + (victory ? "Victory" : "Defeat");
	}

	// Token: 0x06001A15 RID: 6677 RVA: 0x000FCCA8 File Offset: 0x000FAEA8
	private void UpdateButtons()
	{
		if (this.m_ContinueLabel != null)
		{
			UIText.SetText(this.m_ContinueLabel, global::Defs.Localize(this.m_windowDefinition, "continue_label", null, null, true, true));
		}
		if (this.m_ContinueLabel != null)
		{
			UIText.SetText(this.m_RestartBattleLabel, global::Defs.Localize(this.m_windowDefinition, "restart_label", this.m_battleLogic.Vars(), null, true, true));
		}
	}

	// Token: 0x06001A16 RID: 6678 RVA: 0x000FCD1C File Offset: 0x000FAF1C
	private void ContinueButton_OnClick(BSGButton btn)
	{
		if (BattleMap.battle != null)
		{
			int side = (BattleMap.KingdomId == BattleMap.battle.attacker_kingdom.id) ? 0 : 1;
			BattleViewLoader.SetLoadingScreenSprite(this.m_Illustration.sprite);
			BattleViewLoader.DisableFadeOut();
			if (BattleViewLoader.WorldView.isLoaded)
			{
				BattleMap.battle.SetStage(Logic.Battle.Stage.Ongoing, true, 0f);
				BattleMap.battle.CheckVictory(true, true);
				BattleMap.battle.DoAction("leave_battle", side, "");
			}
		}
		FMODWrapper.Snapshot snapshot = this.snapshot;
		if (snapshot != null)
		{
			snapshot.EndSnapshot();
		}
		this.m_Container.SetActive(false);
		Tutorial.SupressTutorials(false);
	}

	// Token: 0x06001A17 RID: 6679 RVA: 0x000FCDC4 File Offset: 0x000FAFC4
	private void RestartButton_OnClick(BSGButton btn)
	{
		FMODWrapper.Snapshot snapshot = this.snapshot;
		if (snapshot != null)
		{
			snapshot.EndSnapshot();
		}
		if (this.m_Container != null)
		{
			this.m_Container.SetActive(false);
		}
		Tutorial.SupressTutorials(false);
		BattleViewLoader.SetLoadingScreenSprite(this.m_Illustration.sprite);
		BattleViewLoader.DisableFadeOut();
		BattleMap.battle.Restart();
	}

	// Token: 0x040010B5 RID: 4277
	[UIFieldTarget("id_Title")]
	private TextMeshProUGUI m_Title;

	// Token: 0x040010B6 RID: 4278
	[UIFieldTarget("id_BattleType")]
	private TextMeshProUGUI m_BattleName;

	// Token: 0x040010B7 RID: 4279
	[UIFieldTarget("id_Illustration")]
	private Image m_Illustration;

	// Token: 0x040010B8 RID: 4280
	[UIFieldTarget("id_Description")]
	private TextMeshProUGUI m_Description;

	// Token: 0x040010B9 RID: 4281
	[UIFieldTarget("id_ContinueButton")]
	private BSGButton m_ContinueButton;

	// Token: 0x040010BA RID: 4282
	[UIFieldTarget("id_RestartBattleButton")]
	private BSGButton m_RestartBattleview;

	// Token: 0x040010BB RID: 4283
	[UIFieldTarget("id_ContinueLabel")]
	private TextMeshProUGUI m_ContinueLabel;

	// Token: 0x040010BC RID: 4284
	[UIFieldTarget("id_RestartBattleLabel")]
	private TextMeshProUGUI m_RestartBattleLabel;

	// Token: 0x040010BD RID: 4285
	[UIFieldTarget("id_Container")]
	private GameObject m_Container;

	// Token: 0x040010BE RID: 4286
	[UIFieldTarget("id_FadeAnimation")]
	private TweenAlpha m_FadeAnimation;

	// Token: 0x040010BF RID: 4287
	private Logic.Battle m_battleLogic;

	// Token: 0x040010C0 RID: 4288
	private DT.Field m_windowDefinition;

	// Token: 0x040010C1 RID: 4289
	private int m_winnerSide = -1;

	// Token: 0x040010C2 RID: 4290
	private FMODWrapper.Snapshot snapshot;
}

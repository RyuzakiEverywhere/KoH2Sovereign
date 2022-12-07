using System;
using System.Collections.Generic;
using Logic;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020000D0 RID: 208
public class SquadsTargetPreview : MonoBehaviour
{
	// Token: 0x06000A1B RID: 2587 RVA: 0x000742E1 File Offset: 0x000724E1
	private void Start()
	{
		this.Initialize();
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x000742E9 File Offset: 0x000724E9
	private void Initialize()
	{
		if (BattleMap.battle != null)
		{
			this.battle = BattleMap.battle;
			this.InitBattleSide();
			this.InitSquadsLists();
		}
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x0007430C File Offset: 0x0007250C
	private void InitBattleSide()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (this.battle.attacker_kingdom.id != kingdom.id)
		{
			Logic.Army attacker_support = this.battle.attacker_support;
			if (((attacker_support != null) ? attacker_support.GetKingdom().id : 0) != kingdom.id)
			{
				if (this.battle.defender_kingdom.id != kingdom.id)
				{
					Logic.Army defender_support = this.battle.defender_support;
					if (((defender_support != null) ? defender_support.GetKingdom().id : 0) != kingdom.id)
					{
						return;
					}
				}
				this.battle_side = 1;
				return;
			}
		}
		this.battle_side = 0;
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x000743A8 File Offset: 0x000725A8
	private void InitSquadsLists()
	{
		BaseUI.LogicKingdom();
		List<Logic.Squad> list = this.battle.squads.Get(this.battle_side);
		this.squads = new List<global::Squad>();
		this.not_owned_squads = new List<global::Squad>();
		foreach (Logic.Squad squad in list)
		{
			if (squad.is_main_squad)
			{
				if (!squad.IsAlly(squad))
				{
					global::Squad squad2 = squad.visuals as global::Squad;
					if (squad2 != null)
					{
						this.squads.Add(squad2);
					}
				}
				else
				{
					global::Squad squad3 = squad.visuals as global::Squad;
					if (squad3 != null)
					{
						this.not_owned_squads.Add(squad3);
					}
				}
			}
		}
		foreach (Logic.Squad squad4 in this.battle.squads.Get(1 - this.battle_side))
		{
			if (squad4.is_main_squad)
			{
				global::Squad squad5 = squad4.visuals as global::Squad;
				if (squad5 != null)
				{
					this.not_owned_squads.Add(squad5);
				}
			}
		}
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x000744F4 File Offset: 0x000726F4
	public void OnPreviewStart()
	{
		this.Initialize();
		foreach (global::Squad squad in this.squads)
		{
			squad.Previewed = true;
			squad.PreviewDirty = true;
		}
		foreach (global::Squad squad2 in this.not_owned_squads)
		{
			squad2.Previewed = true;
		}
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x00074594 File Offset: 0x00072794
	public unsafe void SetPreviewPositions()
	{
		foreach (global::Squad squad in this.squads)
		{
			if (squad.InDrag || squad.logic.movement == null || squad.logic.movement.path == null || squad.logic.target != null || squad.logic.is_fleeing)
			{
				squad.TargetPreviewed = false;
			}
			else
			{
				Formation formation = squad.previewFormation;
				if (!squad.InDrag)
				{
					formation = squad.previewFormationConfirmed;
				}
				if (formation != squad.last_previewed_formation)
				{
					squad.last_previewed_formation = formation;
					squad.PreviewDirty = true;
				}
				if (squad.PreviewDirty)
				{
					if (formation == null)
					{
						squad.TargetPreviewed = false;
					}
					else
					{
						int num = 0;
						Troops.Troop troop = squad.data->FirstTroop;
						while (troop <= squad.data->LastTroop)
						{
							if (troop.HasFlags(Troops.Troop.Flags.Dead) || troop.HasFlags(Troops.Troop.Flags.Destroyed))
							{
								troop.preview_position = 0;
							}
							else
							{
								PPos pt = formation.positions[num];
								float y = pt.Height(squad.logic.game, global::Common.GetHeight(pt, null, -1f, false), 0f);
								troop.preview_position = new float3(pt.x, y, pt.y);
								num++;
							}
							troop = ++troop;
						}
						squad.TargetPreviewed = true;
						squad.PreviewDirty = false;
					}
				}
			}
		}
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x00074750 File Offset: 0x00072950
	public void OnPreviewEnd()
	{
		foreach (global::Squad squad in this.squads)
		{
			squad.TargetPreviewed = false;
			squad.Previewed = false;
		}
		foreach (global::Squad squad2 in this.not_owned_squads)
		{
			squad2.Previewed = false;
		}
	}

	// Token: 0x04000822 RID: 2082
	private Logic.Battle battle;

	// Token: 0x04000823 RID: 2083
	private int battle_side;

	// Token: 0x04000824 RID: 2084
	private List<global::Squad> squads;

	// Token: 0x04000825 RID: 2085
	private List<global::Squad> not_owned_squads;
}

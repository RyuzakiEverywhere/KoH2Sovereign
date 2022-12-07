using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001AF RID: 431
public class UIBattleEstimationBar : MonoBehaviour, IListener
{
	// Token: 0x0600198A RID: 6538 RVA: 0x000F8D60 File Offset: 0x000F6F60
	private void Start()
	{
		this.Init();
	}

	// Token: 0x0600198B RID: 6539 RVA: 0x000F8D68 File Offset: 0x000F6F68
	private void Update()
	{
		if (this._invalidate)
		{
			this._invalidate = false;
			this.Refresh();
		}
	}

	// Token: 0x0600198C RID: 6540 RVA: 0x000F8D7F File Offset: 0x000F6F7F
	private void OnDestroy()
	{
		if (this.logic != null)
		{
			this.logic.DelListener(this);
		}
	}

	// Token: 0x0600198D RID: 6541 RVA: 0x000F8D95 File Offset: 0x000F6F95
	public void SetObject(Logic.Battle battle)
	{
		if (this.logic != null)
		{
			this.logic.DelListener(this);
		}
		this.logic = battle;
		if (this.logic != null)
		{
			this.logic.AddListener(this);
		}
		this.Init();
		this.Refresh();
	}

	// Token: 0x0600198E RID: 6542 RVA: 0x000F8DD4 File Offset: 0x000F6FD4
	private void Init()
	{
		if (this._initialized)
		{
			return;
		}
		this._initialized = true;
		UICommon.FindComponents(this, false);
		GameObject gameObject = this.left_estimation_bar;
		this.left_estimation_bar_image = ((gameObject != null) ? gameObject.GetComponent<Image>() : null);
		GameObject gameObject2 = this.right_estimation_bar;
		this.right_estimation_bar_image = ((gameObject2 != null) ? gameObject2.GetComponent<Image>() : null);
	}

	// Token: 0x0600198F RID: 6543 RVA: 0x000F8E28 File Offset: 0x000F7028
	private void Refresh()
	{
		if (this.logic == null || this.logic.simulation == null)
		{
			return;
		}
		this._invalidate = true;
		if (Game.isLoadingSaveGame)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		if (!(kingdom.visuals is global::Kingdom) && !this.logic.battle_map_only)
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null && worldUI.m_InvalidateUpdateKingdom)
		{
			return;
		}
		this._invalidate = false;
		float num = this.logic.simulation.GetEstimation();
		global::Battle.CalcSides(this.logic, out this.visual_sides[0], out this.visual_sides[1], this.use_x_location);
		global::Battle.CalcSides(this.logic, out this.logic_sides[0], out this.logic_sides[1], false);
		bool flag = this.visual_sides[0] != 0;
		if (flag)
		{
			num = 1f - num;
		}
		if (num < 0.1f)
		{
			num = 0.1f;
		}
		if (num > 0.9f)
		{
			num = 0.9f;
		}
		if (this.left_estimation_bar != null)
		{
			this.left_estimation_bar.transform.localScale = new Vector3(1f - num, 1f, 1f);
		}
		if (this.right_estimation_bar != null)
		{
			this.right_estimation_bar.transform.localScale = new Vector3(num, 1f, 1f);
		}
		if (global::Battle.PlayerIsAttacker(this.logic, true))
		{
			if (!flag)
			{
				if (this.left_estimation_bar_image != null)
				{
					this.left_estimation_bar_image.color = global::Battle.GetEstimationColor("player");
				}
				if (this.right_estimation_bar_image != null)
				{
					this.right_estimation_bar_image.color = global::Battle.GetEstimationColor("player_enemy");
					return;
				}
			}
			else
			{
				if (this.right_estimation_bar_image != null)
				{
					this.right_estimation_bar_image.color = global::Battle.GetEstimationColor("player");
				}
				if (this.left_estimation_bar_image != null)
				{
					this.left_estimation_bar_image.color = global::Battle.GetEstimationColor("player_enemy");
					return;
				}
			}
		}
		else if (global::Battle.PlayerIsDefender(this.logic, true))
		{
			if (!flag)
			{
				if (this.right_estimation_bar_image != null)
				{
					this.right_estimation_bar_image.color = global::Battle.GetEstimationColor("player");
				}
				if (this.left_estimation_bar_image != null)
				{
					this.left_estimation_bar_image.color = global::Battle.GetEstimationColor("player_enemy");
					return;
				}
			}
			else
			{
				if (this.left_estimation_bar_image != null)
				{
					this.left_estimation_bar_image.color = global::Battle.GetEstimationColor("player");
				}
				if (this.right_estimation_bar_image != null)
				{
					this.right_estimation_bar_image.color = global::Battle.GetEstimationColor("player_enemy");
					return;
				}
			}
		}
		else
		{
			bool flag2 = this.logic_sides[0] != 0 != flag;
			if (flag)
			{
				if (this.left_estimation_bar_image != null)
				{
					this.left_estimation_bar_image.color = UIBattleEstimationBar.GetEstimationColor(this.logic.defender_kingdom, this.logic.defender, flag2 ? "right" : "left");
				}
				if (this.right_estimation_bar_image != null)
				{
					this.right_estimation_bar_image.color = UIBattleEstimationBar.GetEstimationColor(this.logic.attacker_kingdom, this.logic.attacker, flag2 ? "left" : "right");
					return;
				}
			}
			else
			{
				if (this.left_estimation_bar_image != null)
				{
					this.left_estimation_bar_image.color = UIBattleEstimationBar.GetEstimationColor(this.logic.attacker_kingdom, this.logic.attacker, flag2 ? "right" : "left");
				}
				if (this.right_estimation_bar_image != null)
				{
					this.right_estimation_bar_image.color = UIBattleEstimationBar.GetEstimationColor(this.logic.defender_kingdom, this.logic.defender, flag2 ? "left" : "right");
				}
			}
		}
	}

	// Token: 0x06001990 RID: 6544 RVA: 0x000F9210 File Offset: 0x000F7410
	private static Color GetEstimationColor(Logic.Kingdom k, Logic.Object obj, string side)
	{
		RelationUtils.Stance stance;
		if (obj != null && obj.IsValid())
		{
			stance = obj.GetStance(BaseUI.LogicKingdom());
		}
		else
		{
			stance = k.GetStance(BaseUI.LogicKingdom());
		}
		if ((stance & RelationUtils.Stance.Own) != RelationUtils.Stance.None)
		{
			return global::Battle.GetEstimationColor("player_" + side);
		}
		if ((stance & RelationUtils.Stance.Alliance) != RelationUtils.Stance.None)
		{
			return global::Battle.GetEstimationColor("ally_" + side);
		}
		if ((stance & RelationUtils.Stance.War) != RelationUtils.Stance.None)
		{
			return global::Battle.GetEstimationColor("enemy_" + side);
		}
		return global::Battle.GetEstimationColor("neutral_" + side);
	}

	// Token: 0x06001991 RID: 6545 RVA: 0x000F9298 File Offset: 0x000F7498
	public void OnMessage(object obj, string message, object param)
	{
		if (this.logic == null)
		{
			return;
		}
		if (message == "changed" || message == "armies_changed")
		{
			this.Refresh();
			return;
		}
		if (!(message == "destroying") && !(message == "finishing"))
		{
			return;
		}
		if (this.logic != null)
		{
			this.logic.DelListener(this);
		}
	}

	// Token: 0x04001070 RID: 4208
	[NonSerialized]
	public bool use_x_location;

	// Token: 0x04001071 RID: 4209
	[UIFieldTarget("id_left_estimation")]
	private GameObject left_estimation_bar;

	// Token: 0x04001072 RID: 4210
	[UIFieldTarget("id_right_estimation")]
	private GameObject right_estimation_bar;

	// Token: 0x04001073 RID: 4211
	private Image left_estimation_bar_image;

	// Token: 0x04001074 RID: 4212
	private Image right_estimation_bar_image;

	// Token: 0x04001075 RID: 4213
	private Logic.Battle logic;

	// Token: 0x04001076 RID: 4214
	private bool _initialized;

	// Token: 0x04001077 RID: 4215
	private int[] visual_sides = new int[2];

	// Token: 0x04001078 RID: 4216
	private int[] logic_sides = new int[2];

	// Token: 0x04001079 RID: 4217
	private bool _invalidate;
}

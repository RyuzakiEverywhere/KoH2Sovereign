using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000240 RID: 576
public class BattleMinimapIcon : MinimapIcon
{
	// Token: 0x06002357 RID: 9047 RVA: 0x0013F8AE File Offset: 0x0013DAAE
	private static void ExtractSettings()
	{
		string def_id = "Minimap";
		BattleMinimapIcon.min_scale = global::Defs.GetFloat(def_id, "worldview_battle_icon_scale_min", null, 0f);
		BattleMinimapIcon.max_scale = global::Defs.GetFloat(def_id, "worldview_battle_icon_scale_max", null, 0f);
	}

	// Token: 0x06002358 RID: 9048 RVA: 0x0013F8E0 File Offset: 0x0013DAE0
	public static void ValidateDefData()
	{
		if (global::Defs.Version != MinimapIcon.defVersion)
		{
			BattleMinimapIcon.ExtractSettings();
		}
	}

	// Token: 0x06002359 RID: 9049 RVA: 0x0013F8F4 File Offset: 0x0013DAF4
	private BattleMinimapIcon.State GetState(Logic.Battle battle, Logic.Kingdom playerKingdom)
	{
		global::Battle battle2 = (global::Battle)battle.visuals;
		this.selected = (battle2 != null && battle2.IsSelected());
		bool is_siege = battle.is_siege;
		if (battle.IsKingdomParticipant(playerKingdom))
		{
			if (is_siege)
			{
				if (!this.selected)
				{
					return BattleMinimapIcon.State.our_battle_in_castle;
				}
				return BattleMinimapIcon.State.our_battle_in_castle_selected;
			}
			else
			{
				if (!this.selected)
				{
					return BattleMinimapIcon.State.our_battle_normal;
				}
				return BattleMinimapIcon.State.our_battle_selected;
			}
		}
		else if (is_siege)
		{
			if (!this.selected)
			{
				return BattleMinimapIcon.State.battle_in_castle;
			}
			return BattleMinimapIcon.State.battle_in_castle_selected;
		}
		else
		{
			if (!this.selected)
			{
				return BattleMinimapIcon.State.battle_normal;
			}
			return BattleMinimapIcon.State.battle_selected;
		}
	}

	// Token: 0x0600235A RID: 9050 RVA: 0x0013F96C File Offset: 0x0013DB6C
	public override Sprite GetSprite()
	{
		if (this.target == null || this.minimap == null)
		{
			return null;
		}
		string key = this.state.ToString();
		Sprite obj = global::Defs.GetObj<Sprite>(this.minimap.def_field, key, null);
		base.CheckNullSprite(obj);
		return obj;
	}

	// Token: 0x0600235B RID: 9051 RVA: 0x0013F9BE File Offset: 0x0013DBBE
	public override float GetMinScale()
	{
		return BattleMinimapIcon.min_scale;
	}

	// Token: 0x0600235C RID: 9052 RVA: 0x0013F9C5 File Offset: 0x0013DBC5
	public override float GetMaxScale()
	{
		return BattleMinimapIcon.max_scale;
	}

	// Token: 0x0600235D RID: 9053 RVA: 0x0013F9CC File Offset: 0x0013DBCC
	public override void UpdateSprite(bool force = false)
	{
		if (this.image == null)
		{
			return;
		}
		if (this.target == null)
		{
			return;
		}
		BattleMinimapIcon.State state = this.GetState(this.target as Logic.Battle, BaseUI.LogicKingdom());
		if (state == this.state && !force)
		{
			return;
		}
		this.state = state;
		this.image.overrideSprite = this.GetSprite();
		if (this.selected)
		{
			this.targetTransform.SetAsLastSibling();
		}
	}

	// Token: 0x0600235E RID: 9054 RVA: 0x0013FA40 File Offset: 0x0013DC40
	public static MinimapIcon Create(MapObject target, MiniMap minimap)
	{
		BattleMinimapIcon.ValidateDefData();
		Image image = new GameObject().AddComponent<Image>();
		BattleMinimapIcon battleMinimapIcon = new BattleMinimapIcon();
		battleMinimapIcon.target = target;
		battleMinimapIcon.minimap = minimap;
		battleMinimapIcon.targetTransform = ((global::Battle)target.visuals).transform;
		battleMinimapIcon.image = image;
		Transform parent = (minimap.icons_container != null) ? minimap.icons_container : minimap.transform;
		image.transform.SetParent(parent);
		image.transform.localScale = Vector3.one;
		image.preserveAspect = true;
		battleMinimapIcon.UpdateSprite(true);
		battleMinimapIcon.UpdateSpriteSize();
		return battleMinimapIcon;
	}

	// Token: 0x040017A8 RID: 6056
	private BattleMinimapIcon.State state;

	// Token: 0x040017A9 RID: 6057
	private static float min_scale;

	// Token: 0x040017AA RID: 6058
	private static float max_scale;

	// Token: 0x0200079F RID: 1951
	public enum State
	{
		// Token: 0x04003B5D RID: 15197
		battle_normal,
		// Token: 0x04003B5E RID: 15198
		battle_selected,
		// Token: 0x04003B5F RID: 15199
		battle_in_castle,
		// Token: 0x04003B60 RID: 15200
		battle_in_castle_selected,
		// Token: 0x04003B61 RID: 15201
		our_battle_normal,
		// Token: 0x04003B62 RID: 15202
		our_battle_selected,
		// Token: 0x04003B63 RID: 15203
		our_battle_in_castle,
		// Token: 0x04003B64 RID: 15204
		our_battle_in_castle_selected
	}
}

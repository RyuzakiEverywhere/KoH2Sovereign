using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200023D RID: 573
public class ArmyMinimapIcon : MinimapIcon
{
	// Token: 0x0600233B RID: 9019 RVA: 0x0013F1EF File Offset: 0x0013D3EF
	private static void ExtractSettings()
	{
		string def_id = "Minimap";
		ArmyMinimapIcon.min_scale = global::Defs.GetFloat(def_id, "worldview_units_icon_scale_min", null, 0f);
		ArmyMinimapIcon.max_scale = global::Defs.GetFloat(def_id, "worldview_units_icon_scale_max", null, 0f);
	}

	// Token: 0x0600233C RID: 9020 RVA: 0x0013F221 File Offset: 0x0013D421
	public static void ValidateDefData()
	{
		if (global::Defs.Version != MinimapIcon.defVersion)
		{
			ArmyMinimapIcon.ExtractSettings();
		}
	}

	// Token: 0x0600233D RID: 9021 RVA: 0x0013F234 File Offset: 0x0013D434
	private ArmyMinimapIcon.State GetState(Logic.Army army, Logic.Kingdom playerKingdom)
	{
		global::Army army2 = (global::Army)army.visuals;
		Castle castle = army.castle;
		this.selected = (army2 != null && army2.IsSelected());
		bool flag = army.kingdom_id == playerKingdom.id;
		bool flag2 = castle != null;
		if (flag)
		{
			if (flag2)
			{
				if (!((global::Settlement)castle.visuals).IsSelected())
				{
					return ArmyMinimapIcon.State.our_army_in_castle;
				}
				return ArmyMinimapIcon.State.our_army_in_castle_selected;
			}
			else
			{
				if (!this.selected)
				{
					return ArmyMinimapIcon.State.primary_friend_normal;
				}
				return ArmyMinimapIcon.State.primary_friend_selected;
			}
		}
		else if (army.IsAlly(playerKingdom))
		{
			if (!this.selected)
			{
				return ArmyMinimapIcon.State.secondary_friend_normal;
			}
			return ArmyMinimapIcon.State.secondary_friend_selected;
		}
		else
		{
			bool flag3 = army.IsEnemy(playerKingdom);
			bool is_supporter = army.is_supporter;
			if (flag3 && is_supporter)
			{
				if (!this.selected)
				{
					return ArmyMinimapIcon.State.secondary_enemy_normal;
				}
				return ArmyMinimapIcon.State.secondary_enemy_selected;
			}
			else if (flag3 && !is_supporter)
			{
				if (!this.selected)
				{
					return ArmyMinimapIcon.State.primary_enemy_normal;
				}
				return ArmyMinimapIcon.State.primary_enemy_selected;
			}
			else
			{
				if (!this.selected)
				{
					return ArmyMinimapIcon.State.primary_neutral_normal;
				}
				return ArmyMinimapIcon.State.primary_neutral_selected;
			}
		}
	}

	// Token: 0x0600233E RID: 9022 RVA: 0x0013F2FE File Offset: 0x0013D4FE
	public override float GetMinScale()
	{
		return ArmyMinimapIcon.min_scale;
	}

	// Token: 0x0600233F RID: 9023 RVA: 0x0013F305 File Offset: 0x0013D505
	public override float GetMaxScale()
	{
		return ArmyMinimapIcon.max_scale;
	}

	// Token: 0x06002340 RID: 9024 RVA: 0x0013F30C File Offset: 0x0013D50C
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
		Logic.Army army = this.target as Logic.Army;
		if (army == null)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		ArmyMinimapIcon.State state = this.GetState(army, kingdom);
		if (state == this.state && !force)
		{
			return;
		}
		this.state = state;
		this.image.overrideSprite = this.GetSprite();
	}

	// Token: 0x06002341 RID: 9025 RVA: 0x0013F37C File Offset: 0x0013D57C
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

	// Token: 0x06002342 RID: 9026 RVA: 0x0013F3D0 File Offset: 0x0013D5D0
	public static MinimapIcon Create(MapObject target, MiniMap minimap)
	{
		ArmyMinimapIcon.ValidateDefData();
		Image image = new GameObject().AddComponent<Image>();
		ArmyMinimapIcon armyMinimapIcon = new ArmyMinimapIcon();
		armyMinimapIcon.target = target;
		armyMinimapIcon.minimap = minimap;
		armyMinimapIcon.targetTransform = ((global::Army)target.visuals).transform;
		armyMinimapIcon.image = image;
		Transform parent = (minimap.icons_container != null) ? minimap.icons_container : minimap.transform;
		image.transform.SetParent(parent);
		image.transform.localScale = Vector3.one;
		image.preserveAspect = true;
		armyMinimapIcon.UpdateSprite(true);
		armyMinimapIcon.UpdateSpriteSize();
		return armyMinimapIcon;
	}

	// Token: 0x0400179E RID: 6046
	private ArmyMinimapIcon.State state;

	// Token: 0x0400179F RID: 6047
	private static float min_scale;

	// Token: 0x040017A0 RID: 6048
	private static float max_scale;

	// Token: 0x0200079C RID: 1948
	public enum State
	{
		// Token: 0x04003B47 RID: 15175
		primary_enemy_normal,
		// Token: 0x04003B48 RID: 15176
		primary_enemy_selected,
		// Token: 0x04003B49 RID: 15177
		primary_friend_normal,
		// Token: 0x04003B4A RID: 15178
		primary_friend_selected,
		// Token: 0x04003B4B RID: 15179
		primary_neutral_normal,
		// Token: 0x04003B4C RID: 15180
		primary_neutral_selected,
		// Token: 0x04003B4D RID: 15181
		secondary_enemy_normal,
		// Token: 0x04003B4E RID: 15182
		secondary_enemy_selected,
		// Token: 0x04003B4F RID: 15183
		secondary_friend_normal,
		// Token: 0x04003B50 RID: 15184
		secondary_friend_selected,
		// Token: 0x04003B51 RID: 15185
		secondary_neutral_normal,
		// Token: 0x04003B52 RID: 15186
		secondary_neutral_selected,
		// Token: 0x04003B53 RID: 15187
		our_army_in_castle,
		// Token: 0x04003B54 RID: 15188
		our_army_in_castle_selected
	}
}

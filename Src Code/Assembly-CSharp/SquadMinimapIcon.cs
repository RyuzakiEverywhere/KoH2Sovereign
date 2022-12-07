using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200023E RID: 574
public class SquadMinimapIcon : MinimapIcon
{
	// Token: 0x06002344 RID: 9028 RVA: 0x0013F472 File Offset: 0x0013D672
	private static void ExtractSettings()
	{
		string def_id = "Minimap";
		SquadMinimapIcon.min_scale = global::Defs.GetFloat(def_id, "battleview_icon_scale_min", null, 0f);
		SquadMinimapIcon.max_scale = global::Defs.GetFloat(def_id, "battleview_icon_scale_max", null, 0f);
	}

	// Token: 0x06002345 RID: 9029 RVA: 0x0013F4A4 File Offset: 0x0013D6A4
	public static void ValidateDefData()
	{
		if (global::Defs.Version != MinimapIcon.defVersion)
		{
			SquadMinimapIcon.ExtractSettings();
		}
	}

	// Token: 0x06002346 RID: 9030 RVA: 0x0013F4B8 File Offset: 0x0013D6B8
	private SquadMinimapIcon.State GetState(Logic.Squad squad, Logic.Kingdom playerKingdom)
	{
		BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
		if (battleViewUI == null || battleViewUI.minimap == null)
		{
			return SquadMinimapIcon.State.minimap_icon_enemy;
		}
		switch (battleViewUI.GetStanceType(squad))
		{
		case BattleViewUI.BattleViewStance.Own:
			return SquadMinimapIcon.State.minimap_icon_own;
		case BattleViewUI.BattleViewStance.Supporter:
		case BattleViewUI.BattleViewStance.Neutral:
			return SquadMinimapIcon.State.minimap_icon_ally;
		default:
			return SquadMinimapIcon.State.minimap_icon_enemy;
		}
	}

	// Token: 0x06002347 RID: 9031 RVA: 0x0013F50C File Offset: 0x0013D70C
	public override Sprite GetSprite()
	{
		if (this.target == null || this.minimap == null)
		{
			return null;
		}
		string key = this.state.ToString();
		Sprite obj = global::Defs.GetObj<Sprite>(this.squad.logic.def.field, key, null);
		base.CheckNullSprite(obj);
		return obj;
	}

	// Token: 0x06002348 RID: 9032 RVA: 0x0013F568 File Offset: 0x0013D768
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
		SquadMinimapIcon.State state = this.GetState(this.target as Logic.Squad, BaseUI.LogicKingdom());
		if (state == this.state && !force)
		{
			return;
		}
		this.state = state;
		this.image.overrideSprite = this.GetSprite();
	}

	// Token: 0x06002349 RID: 9033 RVA: 0x0013F5C9 File Offset: 0x0013D7C9
	public override float GetMinScale()
	{
		return SquadMinimapIcon.min_scale;
	}

	// Token: 0x0600234A RID: 9034 RVA: 0x0013F5D0 File Offset: 0x0013D7D0
	public override float GetMaxScale()
	{
		return SquadMinimapIcon.max_scale;
	}

	// Token: 0x0600234B RID: 9035 RVA: 0x0013F5D7 File Offset: 0x0013D7D7
	public unsafe override Vector3 GetPosition()
	{
		if (this.squad.GetID() < 0)
		{
			return base.GetPosition();
		}
		return this.squad.data->banner_pos;
	}

	// Token: 0x0600234C RID: 9036 RVA: 0x0013F604 File Offset: 0x0013D804
	public static MinimapIcon Create(MapObject target, MiniMap minimap)
	{
		SquadMinimapIcon.ValidateDefData();
		Image image = new GameObject().AddComponent<Image>();
		SquadMinimapIcon squadMinimapIcon = new SquadMinimapIcon();
		squadMinimapIcon.squad = (target.visuals as global::Squad);
		squadMinimapIcon.target = target;
		squadMinimapIcon.minimap = minimap;
		squadMinimapIcon.targetTransform = ((global::Squad)target.visuals).transform;
		squadMinimapIcon.image = image;
		Transform parent = (minimap.icons_container != null) ? minimap.icons_container : minimap.transform;
		image.transform.SetParent(parent);
		image.transform.localScale = Vector3.one;
		image.preserveAspect = true;
		squadMinimapIcon.UpdateSprite(true);
		squadMinimapIcon.UpdateSpriteSize();
		return squadMinimapIcon;
	}

	// Token: 0x040017A1 RID: 6049
	private SquadMinimapIcon.State state;

	// Token: 0x040017A2 RID: 6050
	private global::Squad squad;

	// Token: 0x040017A3 RID: 6051
	private static float min_scale;

	// Token: 0x040017A4 RID: 6052
	private static float max_scale;

	// Token: 0x0200079D RID: 1949
	private enum State
	{
		// Token: 0x04003B56 RID: 15190
		minimap_icon_own,
		// Token: 0x04003B57 RID: 15191
		minimap_icon_ally,
		// Token: 0x04003B58 RID: 15192
		minimap_icon_enemy
	}
}

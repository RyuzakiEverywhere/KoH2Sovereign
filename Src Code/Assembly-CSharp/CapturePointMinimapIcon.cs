using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200023F RID: 575
public class CapturePointMinimapIcon : MinimapIcon
{
	// Token: 0x0600234E RID: 9038 RVA: 0x0013F6AF File Offset: 0x0013D8AF
	private static void ExtractSettings()
	{
		string def_id = "Minimap";
		CapturePointMinimapIcon.min_scale = global::Defs.GetFloat(def_id, "battleview_icon_scale_min", null, 0f);
		CapturePointMinimapIcon.max_scale = global::Defs.GetFloat(def_id, "battleview_icon_scale_max", null, 0f);
	}

	// Token: 0x0600234F RID: 9039 RVA: 0x0013F6E1 File Offset: 0x0013D8E1
	public static void ValidateDefData()
	{
		if (global::Defs.Version != MinimapIcon.defVersion)
		{
			CapturePointMinimapIcon.ExtractSettings();
		}
	}

	// Token: 0x06002350 RID: 9040 RVA: 0x0013F6F4 File Offset: 0x0013D8F4
	private CapturePointMinimapIcon.State GetState(Logic.CapturePoint capturePoint, Logic.Kingdom playerKingdom)
	{
		BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
		if (battleViewUI == null || battleViewUI.minimap == null)
		{
			return CapturePointMinimapIcon.State.minimap_icon_enemy;
		}
		BattleViewUI.BattleViewStance stanceType = battleViewUI.GetStanceType(capturePoint.battle.GetSideKingdom(capturePoint.battle_side));
		if (stanceType > BattleViewUI.BattleViewStance.Supporter)
		{
			int num = stanceType - BattleViewUI.BattleViewStance.EnemySupporter;
			return CapturePointMinimapIcon.State.minimap_icon_enemy;
		}
		return CapturePointMinimapIcon.State.minimap_icon_own;
	}

	// Token: 0x06002351 RID: 9041 RVA: 0x0013F748 File Offset: 0x0013D948
	public override Sprite GetSprite()
	{
		if (this.target == null || this.minimap == null)
		{
			return null;
		}
		string key = this.state.ToString();
		Sprite obj = global::Defs.GetObj<Sprite>((this.target as Logic.CapturePoint).def.field, key, null);
		base.CheckNullSprite(obj);
		return obj;
	}

	// Token: 0x06002352 RID: 9042 RVA: 0x0013F7A4 File Offset: 0x0013D9A4
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
		CapturePointMinimapIcon.State state = this.GetState(this.target as Logic.CapturePoint, BaseUI.LogicKingdom());
		if (state == this.state && !force)
		{
			return;
		}
		this.state = state;
		this.image.overrideSprite = this.GetSprite();
	}

	// Token: 0x06002353 RID: 9043 RVA: 0x0013F805 File Offset: 0x0013DA05
	public override float GetMinScale()
	{
		return CapturePointMinimapIcon.min_scale;
	}

	// Token: 0x06002354 RID: 9044 RVA: 0x0013F80C File Offset: 0x0013DA0C
	public override float GetMaxScale()
	{
		return CapturePointMinimapIcon.max_scale;
	}

	// Token: 0x06002355 RID: 9045 RVA: 0x0013F814 File Offset: 0x0013DA14
	public static MinimapIcon Create(MapObject target, MiniMap minimap)
	{
		CapturePointMinimapIcon.ValidateDefData();
		Image image = new GameObject().AddComponent<Image>();
		CapturePointMinimapIcon capturePointMinimapIcon = new CapturePointMinimapIcon();
		capturePointMinimapIcon.target = target;
		capturePointMinimapIcon.minimap = minimap;
		capturePointMinimapIcon.targetTransform = ((global::CapturePoint)target.visuals).transform;
		capturePointMinimapIcon.image = image;
		Transform parent = (minimap.icons_container != null) ? minimap.icons_container : minimap.transform;
		image.transform.SetParent(parent);
		image.transform.localScale = Vector3.one;
		image.preserveAspect = true;
		capturePointMinimapIcon.UpdateSprite(true);
		capturePointMinimapIcon.UpdateSpriteSize();
		return capturePointMinimapIcon;
	}

	// Token: 0x040017A5 RID: 6053
	private CapturePointMinimapIcon.State state;

	// Token: 0x040017A6 RID: 6054
	private static float min_scale;

	// Token: 0x040017A7 RID: 6055
	private static float max_scale;

	// Token: 0x0200079E RID: 1950
	private enum State
	{
		// Token: 0x04003B5A RID: 15194
		minimap_icon_own,
		// Token: 0x04003B5B RID: 15195
		minimap_icon_enemy
	}
}

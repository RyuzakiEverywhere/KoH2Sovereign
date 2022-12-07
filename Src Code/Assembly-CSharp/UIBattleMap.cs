using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001B1 RID: 433
public class UIBattleMap : MonoBehaviour
{
	// Token: 0x0600199E RID: 6558 RVA: 0x000F95C0 File Offset: 0x000F77C0
	public void BuildMap(Logic.Battle battle)
	{
		this.logic = battle;
		if (this.logic == null || this.logic.tt_grid == null)
		{
			return;
		}
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Map", true, true);
		if (gameObject == null)
		{
			return;
		}
		RawImage component = gameObject.GetComponent<RawImage>();
		if (component == null)
		{
			return;
		}
		RenderTexture renderTexture = TerrainTypesRender.Render(new Vector2Int(this.logic.tt_x, this.logic.tt_y), this.logic.tt_grid, null, 512, 512, 64, false);
		Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false, false);
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), 0, 0, false);
		RenderTexture.active = null;
		texture2D.Apply();
		component.texture = texture2D;
		renderTexture.Release();
		RectTransform component2 = gameObject.GetComponent<RectTransform>();
		this.PlaceSquadsOnMap(component2);
		this.PlaceArrowsAndStatuses(component2);
	}

	// Token: 0x0600199F RID: 6559 RVA: 0x000F96C8 File Offset: 0x000F78C8
	private void PlaceSquadsOnMap(RectTransform rtMap)
	{
		if (this.logic.simulation == null)
		{
			return;
		}
		if (this.logic.IsFinishing())
		{
			this.placed_squads = new List<UIBattleMap.PlacedSquad>[2];
			this.placed_squads[0] = new List<UIBattleMap.PlacedSquad>();
			this.placed_squads[1] = new List<UIBattleMap.PlacedSquad>();
		}
		for (int i = 0; i <= 1; i++)
		{
			List<BattleSimulation.Squad> squads = this.logic.simulation.GetSquads(i);
			if (squads != null && squads.Count >= 1)
			{
				for (int j = 0; j < squads.Count; j++)
				{
					BattleSimulation.Squad squad = squads[j];
					Color clr = (i == 0) ? this.AttackerColor : this.DefenderColor;
					if (squad.IsDefeated())
					{
						clr.a = 0.5f;
					}
					RectTransform rectTransform = UIBattleMap.PlaceSquadOnMap(squad, rtMap, this.logic.tt_grid, this.logic.game.terrain_types.tile_size, clr);
					if (!(rectTransform == null) && this.placed_squads != null)
					{
						clr.a = 1f;
						UIBattleMap.PlacedSquad placedSquad = new UIBattleMap.PlacedSquad();
						placedSquad.logic = squad;
						placedSquad.rt = rectTransform;
						placedSquad.clr = clr;
						placedSquad.tgt_pos = (placedSquad.org_pos = new Vector2(rectTransform.localPosition.x, rectTransform.localPosition.y));
						this.placed_squads[i].Add(placedSquad);
					}
				}
			}
		}
	}

	// Token: 0x060019A0 RID: 6560 RVA: 0x000F9840 File Offset: 0x000F7A40
	private void PlaceArrowsAndStatuses(RectTransform rtMap)
	{
		if (this.placed_squads == null)
		{
			return;
		}
		for (int i = 0; i <= 1; i++)
		{
			List<UIBattleMap.PlacedSquad> squads = this.placed_squads[i];
			this.PlaceArrowsAndStatuses(rtMap, squads, i == 0);
		}
	}

	// Token: 0x060019A1 RID: 6561 RVA: 0x000F9878 File Offset: 0x000F7A78
	private void PlaceArrowsAndStatuses(RectTransform rtMap, List<UIBattleMap.PlacedSquad> squads, bool attacker)
	{
		if (squads == null)
		{
			return;
		}
		squads.Sort(delegate(UIBattleMap.PlacedSquad sq1, UIBattleMap.PlacedSquad sq2)
		{
			int battle_row = sq1.logic.battle_row;
			int battle_row2 = sq2.logic.battle_row;
			int num3 = sq1.logic.battle_col - Logic.Army.battle_cols / 2;
			int num4 = sq1.logic.battle_col - Logic.Army.battle_cols / 2;
			int num5 = battle_row * battle_row + num3 * num3;
			int value = battle_row2 * battle_row2 + num4 * num4;
			return num5.CompareTo(value);
		});
		int num;
		int num2;
		if (attacker)
		{
			num = Random.Range(2, 4);
			num2 = this.PlaceAttackArrows(squads, BattleSimulation.Squad.State.Attacking, num);
			if (num2 < num)
			{
				num2 += this.PlaceAttackArrows(squads, BattleSimulation.Squad.State.Retreating, num - num2);
			}
			if (num2 < num)
			{
				num2 += this.PlaceAttackArrows(squads, BattleSimulation.Squad.State.Dead, num - num2);
			}
			if (num2 < num)
			{
				num2 += this.PlaceAttackArrows(squads, BattleSimulation.Squad.State.Fled, num - num2);
			}
		}
		num = Random.Range(3, 6);
		num2 = this.PlaceRetreatArrows(squads, BattleSimulation.Squad.State.Left, int.MaxValue);
		num2 += this.PlaceRetreatArrows(squads, BattleSimulation.Squad.State.Fled, int.MaxValue);
		if (num2 < num)
		{
			num2 += this.PlaceRetreatArrows(squads, BattleSimulation.Squad.State.Dead, num - num2);
		}
		if (num2 < num)
		{
			num2 += this.PlaceRetreatArrows(squads, BattleSimulation.Squad.State.Retreating, num - num2);
		}
		this.PlaceStatusIcons(rtMap, squads);
	}

	// Token: 0x060019A2 RID: 6562 RVA: 0x000F994C File Offset: 0x000F7B4C
	private int PlaceAttackArrows(List<UIBattleMap.PlacedSquad> squads, BattleSimulation.Squad.State state, int max = 2147483647)
	{
		int num = 0;
		for (int i = 0; i < squads.Count; i++)
		{
			UIBattleMap.PlacedSquad placedSquad = squads[i];
			if (placedSquad.logic.state == state && this.PlaceAttackArrow(placedSquad) && ++num >= max)
			{
				break;
			}
		}
		return num;
	}

	// Token: 0x060019A3 RID: 6563 RVA: 0x000F9994 File Offset: 0x000F7B94
	private bool CalcAttackPos(UIBattleMap.PlacedSquad squad)
	{
		Point point = squad.tgt_pos;
		Point point2 = Point.UnitRight.GetRotated(squad.logic.heading + Random.Range(-30f, 30f));
		point2 *= Random.Range(30f, 45f);
		point += point2;
		squad.tgt_pos = point;
		return true;
	}

	// Token: 0x060019A4 RID: 6564 RVA: 0x000F99F8 File Offset: 0x000F7BF8
	private bool PlaceAttackArrow(UIBattleMap.PlacedSquad squad)
	{
		Point tgt_pos = squad.tgt_pos;
		if (!this.CalcAttackPos(squad))
		{
			return false;
		}
		Sprite obj = global::Defs.GetObj<Sprite>("Battle", "arrows.attack", null);
		return !(obj == null) && this.PlaceArrow(squad, obj, tgt_pos, 1f);
	}

	// Token: 0x060019A5 RID: 6565 RVA: 0x000F9A44 File Offset: 0x000F7C44
	private int PlaceRetreatArrows(List<UIBattleMap.PlacedSquad> squads, BattleSimulation.Squad.State state, int max = 2147483647)
	{
		int num = 0;
		for (int i = squads.Count - 1; i >= 0; i--)
		{
			UIBattleMap.PlacedSquad placedSquad = squads[i];
			if (placedSquad.logic.state == state && this.PlaceRetreatArrow(placedSquad) && ++num >= max)
			{
				break;
			}
		}
		return num;
	}

	// Token: 0x060019A6 RID: 6566 RVA: 0x000F9A90 File Offset: 0x000F7C90
	private static bool IsInsideMap(RectTransform rtMap, Point pt, float r = 0f)
	{
		Rect worldRect = UICommon.GetWorldRect(rtMap.parent as RectTransform);
		Rect worldRect2 = UICommon.GetWorldRect(rtMap);
		Vector3 vector = new Vector3(pt.x, pt.y, 0f);
		vector = rtMap.TransformPoint(vector);
		r += worldRect2.xMin - worldRect.xMin;
		return vector.x - r > worldRect.xMin && vector.x + r < worldRect.xMax && vector.y - r > worldRect.yMin && vector.y + r < worldRect.yMax;
	}

	// Token: 0x060019A7 RID: 6567 RVA: 0x000F9B38 File Offset: 0x000F7D38
	private bool CalcRetreatPos(UIBattleMap.PlacedSquad squad)
	{
		Point point = squad.org_pos;
		Point point2 = point.GetNormalized();
		point2 *= 65f;
		point2.x += Random.Range(-15f, 15f);
		point2.y += Random.Range(-15f, 15f);
		point += point2;
		if (!UIBattleMap.IsInsideMap(squad.rt.parent as RectTransform, point, -30f))
		{
			return false;
		}
		squad.tgt_pos = point;
		return true;
	}

	// Token: 0x060019A8 RID: 6568 RVA: 0x000F9BC4 File Offset: 0x000F7DC4
	private bool PlaceRetreatArrow(UIBattleMap.PlacedSquad squad)
	{
		Point tgt_pos = squad.tgt_pos;
		if (!this.CalcRetreatPos(squad))
		{
			return false;
		}
		string str = "retreat";
		float num = tgt_pos.Dist(squad.tgt_pos);
		if (num < 45f)
		{
			str = "retreat_short";
		}
		else if (num > 75f)
		{
			str = "retreat_long";
		}
		Sprite obj = global::Defs.GetObj<Sprite>("Battle", "arrows." + str, null);
		if (obj == null)
		{
			return false;
		}
		Image component = squad.rt.GetComponent<Image>();
		if (component != null)
		{
			Color clr = squad.clr;
			clr.a = 0.5f;
			component.color = clr;
		}
		squad.has_retreat_arrow = this.PlaceArrow(squad, obj, tgt_pos, (Random.Range(0f, 1f) < 0.5f) ? 1f : -1f);
		return squad.has_retreat_arrow;
	}

	// Token: 0x060019A9 RID: 6569 RVA: 0x000F9CA4 File Offset: 0x000F7EA4
	private bool PlaceArrow(UIBattleMap.PlacedSquad squad, Sprite sprite, Point pos, float xscale = 1f)
	{
		GameObject gameObject = new GameObject(sprite.name);
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		rectTransform.SetParent(squad.rt.parent, false);
		rectTransform.pivot = new Vector2(0.5f, 0f);
		rectTransform.localPosition = new Vector3(pos.x, pos.y, 0f);
		rectTransform.SetParent(squad.rt, true);
		Point point = squad.tgt_pos - pos;
		float num = point.Heading();
		float num2 = point.Length();
		rectTransform.eulerAngles = new Vector3(0f, 0f, num - 90f);
		rectTransform.localScale = new Vector3(xscale, 1f, 1f);
		float num3 = sprite.rect.width / sprite.rect.height;
		float y = num2;
		float x = num2 * num3;
		rectTransform.sizeDelta = new Vector2(x, y);
		Image image = gameObject.AddComponent<Image>();
		image.sprite = sprite;
		Color clr = squad.clr;
		clr.a = 0.75f;
		image.color = clr;
		return true;
	}

	// Token: 0x060019AA RID: 6570 RVA: 0x000F9DC0 File Offset: 0x000F7FC0
	private void PlaceStatusIcons(RectTransform rtMap, List<UIBattleMap.PlacedSquad> squads)
	{
		for (int i = 0; i < squads.Count; i++)
		{
			UIBattleMap.PlacedSquad squad = squads[i];
			this.PlaceStatusIcon(rtMap, squad);
		}
	}

	// Token: 0x060019AB RID: 6571 RVA: 0x000F9DF0 File Offset: 0x000F7FF0
	private void PlaceStatusIcon(RectTransform rtMap, UIBattleMap.PlacedSquad squad)
	{
		int state = (int)squad.logic.state;
		Logic.Unit.Def def = squad.logic.def;
		if (state == 8)
		{
			Sprite obj = global::Defs.GetObj<Sprite>(def.field, "symbol_dead", null);
			if (obj != null)
			{
				Color clr = squad.clr;
				clr.a = 0.75f;
				if (squad.tgt_pos == squad.org_pos)
				{
					Image component = squad.rt.GetComponent<Image>();
					if (component != null)
					{
						component.sprite = obj;
						component.color = clr;
						return;
					}
				}
				else
				{
					RectTransform rectTransform = UIBattleMap.PlaceSpriteOnMap("dead", obj, rtMap, squad.tgt_pos, 0f, squad.rt.rect.width, clr);
					if (rectTransform != null)
					{
						rectTransform.SetParent(squad.rt, true);
						float num = Random.Range(-30f, 30f);
						if (squad.has_retreat_arrow)
						{
							num += 180f;
						}
						rectTransform.localEulerAngles = new Vector3(0f, 0f, num);
					}
				}
			}
		}
	}

	// Token: 0x060019AC RID: 6572 RVA: 0x000F9F08 File Offset: 0x000F8108
	public static RectTransform PlaceSquadOnMap(BattleSimulation.Squad squad, RectTransform rtMap, TerrainType[,] tt_grid, float tile_size, Color clr)
	{
		if (tt_grid == null)
		{
			return null;
		}
		Logic.Unit.Def def = squad.def;
		Sprite obj = global::Defs.GetObj<Sprite>(def.field, "symbol", null);
		if (obj == null)
		{
			return null;
		}
		float width = rtMap.rect.width;
		float height = rtMap.rect.height;
		int length = tt_grid.GetLength(0);
		float length2 = (float)tt_grid.GetLength(1);
		float num = (float)length * tile_size;
		float num2 = length2 * tile_size;
		Point position = squad.position;
		position.x = position.x * width / num;
		position.y = position.y * height / num2;
		float width2 = 1.25f * width / (float)(length * Logic.Army.battle_cols);
		RectTransform rectTransform = UIBattleMap.PlaceSpriteOnMap(def.name, obj, rtMap, position, squad.heading - 90f, width2, clr);
		if (rectTransform == null)
		{
			return null;
		}
		Vars vars = new Vars(squad);
		vars.Set<DT.Field>("name", squad.def.field.FindChild("name", null, true, true, true, '.'));
		Tooltip.Get(rectTransform.gameObject, true).SetDef("SquadSymbolTooltip", vars);
		return rectTransform;
	}

	// Token: 0x060019AD RID: 6573 RVA: 0x000FA034 File Offset: 0x000F8234
	public static RectTransform PlaceSpriteOnMap(string name, Sprite sprite, RectTransform rtMap, Point pos, float rot, float width, Color clr)
	{
		float num = sprite.rect.width / sprite.rect.height;
		float num2 = width / num;
		float r = Mathf.Max(width, num2) / 2f;
		if (!UIBattleMap.IsInsideMap(rtMap, pos, r))
		{
			return null;
		}
		GameObject gameObject = new GameObject(name);
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		rectTransform.SetParent(rtMap, false);
		rectTransform.localPosition = new Vector3(pos.x, pos.y, 0f);
		rectTransform.localEulerAngles = new Vector3(0f, 0f, rot);
		rectTransform.localScale = Vector3.one;
		rectTransform.sizeDelta = new Vector2(width, num2);
		Image image = gameObject.AddComponent<Image>();
		image.sprite = sprite;
		image.color = clr;
		return rectTransform;
	}

	// Token: 0x0400107E RID: 4222
	public Color AttackerColor;

	// Token: 0x0400107F RID: 4223
	public Color DefenderColor;

	// Token: 0x04001080 RID: 4224
	[NonSerialized]
	public Logic.Battle logic;

	// Token: 0x04001081 RID: 4225
	private List<UIBattleMap.PlacedSquad>[] placed_squads;

	// Token: 0x0200070C RID: 1804
	private class PlacedSquad
	{
		// Token: 0x04003800 RID: 14336
		public BattleSimulation.Squad logic;

		// Token: 0x04003801 RID: 14337
		public RectTransform rt;

		// Token: 0x04003802 RID: 14338
		public Color clr;

		// Token: 0x04003803 RID: 14339
		public Point org_pos;

		// Token: 0x04003804 RID: 14340
		public Point tgt_pos;

		// Token: 0x04003805 RID: 14341
		public bool has_retreat_arrow;
	}
}

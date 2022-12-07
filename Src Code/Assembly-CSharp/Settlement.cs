using System;
using System.Collections.Generic;
using FMODUnity;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x0200017B RID: 379
public class Settlement : GameLogic.Behaviour, VisibilityDetector.IVisibilityChanged
{
	// Token: 0x06001443 RID: 5187 RVA: 0x000CB163 File Offset: 0x000C9363
	public static global::Settlement First()
	{
		return global::Settlement.first;
	}

	// Token: 0x06001444 RID: 5188 RVA: 0x000CB16A File Offset: 0x000C936A
	public global::Settlement Next()
	{
		return this.next;
	}

	// Token: 0x06001445 RID: 5189 RVA: 0x000CB172 File Offset: 0x000C9372
	public static global::Settlement Get(GameObject go)
	{
		if (go == null)
		{
			return null;
		}
		return go.GetComponent<global::Settlement>();
	}

	// Token: 0x06001446 RID: 5190 RVA: 0x000CB185 File Offset: 0x000C9385
	public static global::Settlement Get(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		return t.GetComponent<global::Settlement>();
	}

	// Token: 0x06001447 RID: 5191 RVA: 0x000CB198 File Offset: 0x000C9398
	public int GetLevel()
	{
		return this.level;
	}

	// Token: 0x06001448 RID: 5192 RVA: 0x000CB1A0 File Offset: 0x000C93A0
	public void SetLevel(int lvl, bool refresh)
	{
		Profile.BeginSection("Settlement.SetLevel");
		this.UpdateHouseLevels(lvl, refresh);
		this.CalcBounds();
		this.batch = true;
		Profile.EndSection("Settlement.SetLevel");
	}

	// Token: 0x06001449 RID: 5193 RVA: 0x000CB1CC File Offset: 0x000C93CC
	public int CalcLevel(Castle c, string settlement_type, out int logic_level, out int logic_max_level, out int logic_wall_level, out int logic_wall_max_level, out int logic_citadel_level, out int logic_citadel_max_level)
	{
		if (c == null)
		{
			logic_level = 0;
			logic_max_level = 0;
			logic_wall_level = 0;
			logic_wall_max_level = 0;
			logic_citadel_level = 0;
			logic_citadel_max_level = 0;
			return 0;
		}
		if (settlement_type == "Castle")
		{
			logic_wall_level = Mathf.Max(1, c.GetWallLevel());
			logic_wall_max_level = Mathf.Max(logic_wall_level, c.GetMaxWallLevel());
			logic_citadel_level = c.GetCitadelLevel();
			logic_citadel_max_level = c.GetCitadelMaxLevel();
		}
		else
		{
			logic_wall_level = 0;
			logic_wall_max_level = 0;
			logic_citadel_level = 0;
			logic_citadel_max_level = 0;
		}
		logic_level = c.CalcLevel();
		logic_max_level = c.GetMaxLevel();
		int maxLevel = this.GetMaxLevel();
		return global::Common.map(logic_level, 1, logic_max_level, 1, maxLevel, true);
	}

	// Token: 0x0600144A RID: 5194 RVA: 0x000CB26C File Offset: 0x000C946C
	public void UpdatePVFigureVisiblity(bool visible)
	{
		UIPVFigureSettlement uipvfigureSettlement = this.ui_pvFigure;
		if (uipvfigureSettlement == null)
		{
			return;
		}
		uipvfigureSettlement.UpdateVisibilityFromObject(visible);
	}

	// Token: 0x0600144B RID: 5195 RVA: 0x000CB27F File Offset: 0x000C947F
	public void UpdatePVFigureVisiblity(ViewMode.AllowedFigures allowedFigures)
	{
		UIPVFigureSettlement uipvfigureSettlement = this.ui_pvFigure;
		if (uipvfigureSettlement == null)
		{
			return;
		}
		uipvfigureSettlement.UpdateVisibilityFromView(allowedFigures);
	}

	// Token: 0x0600144C RID: 5196 RVA: 0x000CB294 File Offset: 0x000C9494
	public void UpdateLevel(bool refresh = true, bool update_settlements = true, bool instant = false)
	{
		if (!instant)
		{
			this.level_dirty = true;
			return;
		}
		this.level_dirty = false;
		using (Game.Profile("Settlement.UpdateLevel", false, 0f, null))
		{
			string name = SceneManager.GetActiveScene().name;
			Logic.Settlement settlement = this.logic;
			string value;
			if (settlement == null)
			{
				value = null;
			}
			else
			{
				Game game = settlement.game;
				value = ((game != null) ? game.map_name : null);
			}
			if (!name.Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				this.was_visible = false;
			}
			else if (this.fix_level)
			{
				this.SetLevel(this.level, refresh);
			}
			else if (this.logic == null)
			{
				this.CalcLevelFromHouses();
			}
			else
			{
				Logic.Settlement settlement2 = this.logic;
				Logic.Realm realm = (settlement2 != null) ? settlement2.GetRealm() : null;
				Castle c = (realm != null) ? realm.castle : null;
				Profile.BeginSection("Settlement.UpdateLevel.CalcLevel");
				int num2;
				int num3;
				int v;
				int vmax;
				int v2;
				int vmax2;
				int num = this.CalcLevel(c, this.setType, out num2, out num3, out v, out vmax, out v2, out vmax2);
				Profile.EndSection("Settlement.UpdateLevel.CalcLevel");
				this.SetLevel(num, refresh);
				if (this.citadel != null)
				{
					num = global::Common.map(v2, 1, vmax2, 1, this.citadel.GetMaxLevel(), false);
					bool flag = this.citadel.cur_level != num;
					this.citadel.set_level = num;
					if (flag && refresh && (this.was_visible || !Application.isPlaying))
					{
						Profile.BeginSection("Settlement.UpdateLevel.Refresh citadel");
						this.citadel.Refresh(true, true);
						this.UpdateCrest();
						Profile.EndSection("Settlement.UpdateLevel.Refresh citadel");
					}
				}
				if (this.wall != null)
				{
					num = global::Common.map(v, 1, vmax, 1, this.wall.GetMaxLevel(), false);
					bool flag2 = this.wall.level != num;
					this.wall.level = num;
					if (flag2 && refresh && (this.was_visible || !Application.isPlaying))
					{
						Profile.BeginSection("Settlement.UpdateLevel.Refresh wall");
						this.wall.Refresh(false);
						Profile.EndSection("Settlement.UpdateLevel.Refresh wall");
					}
				}
				if (update_settlements && this.IsCastle() && realm != null)
				{
					Profile.BeginSection("Settlement.UpdateLevel.Refresh settlements");
					for (int i = 0; i < realm.settlements.Count; i++)
					{
						Logic.Settlement settlement3 = realm.settlements[i];
						if (!(settlement3 is Castle))
						{
							global::Settlement settlement4 = ((settlement3 != null) ? settlement3.visuals : null) as global::Settlement;
							if (settlement4 != null)
							{
								settlement4.UpdateLevel(refresh, false, true);
							}
						}
					}
					Profile.EndSection("Settlement.UpdateLevel.Refresh settlements");
				}
			}
		}
	}

	// Token: 0x0600144D RID: 5197 RVA: 0x0002C53B File Offset: 0x0002A73B
	public int GetMinLevel()
	{
		return 1;
	}

	// Token: 0x0600144E RID: 5198 RVA: 0x000CB51C File Offset: 0x000C971C
	public int GetMaxLevel()
	{
		int num = 1;
		for (int i = 0; i < this.houses.Count; i++)
		{
			PrefabGrid prefabGrid = this.houses[i];
			if (prefabGrid.set_level <= 0)
			{
				int maxLevel = prefabGrid.GetMaxLevel();
				if (maxLevel > 0)
				{
					num += maxLevel - 1;
				}
			}
		}
		return num;
	}

	// Token: 0x0600144F RID: 5199 RVA: 0x000CB569 File Offset: 0x000C9769
	public bool IsCastle()
	{
		return this.Name != "";
	}

	// Token: 0x06001450 RID: 5200 RVA: 0x000CB57C File Offset: 0x000C977C
	public global::Army GetArmy()
	{
		Castle castle = this.logic as Castle;
		if (castle == null || castle.army == null)
		{
			return null;
		}
		return castle.army.visuals as global::Army;
	}

	// Token: 0x06001451 RID: 5201 RVA: 0x000CB5B4 File Offset: 0x000C97B4
	public global::Character GetGovernor()
	{
		Castle castle = this.logic as Castle;
		if (castle == null || castle.governor == null)
		{
			return null;
		}
		return castle.governor.visuals as global::Character;
	}

	// Token: 0x06001452 RID: 5202 RVA: 0x000CB5EA File Offset: 0x000C97EA
	public global::Battle GetBattle()
	{
		if (this.logic == null || this.logic.battle == null)
		{
			return null;
		}
		return this.logic.battle.visuals as global::Battle;
	}

	// Token: 0x06001453 RID: 5203 RVA: 0x000CB618 File Offset: 0x000C9818
	public override string ToString()
	{
		return global::Common.ToString<global::Settlement>(this);
	}

	// Token: 0x06001454 RID: 5204 RVA: 0x000CB620 File Offset: 0x000C9820
	public void Log(string msg)
	{
		global::Common.Log<global::Settlement>(this, msg, null);
	}

	// Token: 0x06001455 RID: 5205 RVA: 0x000CB62C File Offset: 0x000C982C
	public void Register()
	{
		if (this.registered)
		{
			return;
		}
		this.registered = true;
		this.prev = global::Settlement.last;
		if (global::Settlement.last != null)
		{
			global::Settlement.last.next = this;
		}
		else
		{
			global::Settlement.first = this;
		}
		global::Settlement.last = this;
	}

	// Token: 0x06001456 RID: 5206 RVA: 0x000CB67C File Offset: 0x000C987C
	public void Unregister()
	{
		if (!this.registered)
		{
			return;
		}
		this.registered = false;
		if (this.next != null)
		{
			this.next.prev = this.prev;
		}
		else
		{
			global::Settlement.last = this.prev;
		}
		if (this.prev != null)
		{
			this.prev.next = this.next;
		}
		else
		{
			global::Settlement.first = this.next;
		}
		this.prev = (this.next = null);
	}

	// Token: 0x06001457 RID: 5207 RVA: 0x000CB704 File Offset: 0x000C9904
	private void DelChildrenByName(string name)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (object obj in base.transform)
		{
			GameObject gameObject = ((Transform)obj).gameObject;
			if (gameObject.name == name)
			{
				list.Add(gameObject);
			}
		}
		foreach (GameObject gameObject2 in list)
		{
			gameObject2.hideFlags = HideFlags.DontSave;
			UnityEngine.Object.DestroyImmediate(gameObject2);
		}
	}

	// Token: 0x06001458 RID: 5208 RVA: 0x000CB7BC File Offset: 0x000C99BC
	private void SnapLabel(Transform label = null)
	{
		if (label == null)
		{
			label = base.transform.Find("_label");
			if (label == null)
			{
				return;
			}
		}
		float num = (this.Name == "") ? 2f : 2f;
		Vector3 localPosition = new Vector3(this.aabb.center.x, this.aabb.max.y + num, this.aabb.center.z) - base.transform.position;
		if (this.citadel != null)
		{
			localPosition.x = this.citadel.transform.localPosition.x;
			localPosition.z = this.citadel.transform.localPosition.z;
		}
		label.transform.localPosition = localPosition;
	}

	// Token: 0x06001459 RID: 5209 RVA: 0x000CB8A8 File Offset: 0x000C9AA8
	private void CalcTextWidthHeight(Text text, out float width, out float height)
	{
		float num = 0f;
		float num2 = 0f;
		CharacterInfo characterInfo = default(CharacterInfo);
		foreach (char ch in text.text.ToCharArray())
		{
			text.font.GetCharacterInfo(ch, out characterInfo, text.fontSize);
			num += (float)characterInfo.glyphHeight;
			num2 = Mathf.Max(num2, (float)characterInfo.glyphHeight);
		}
		width = num * text.gameObject.transform.localScale.x;
		height = num2 * text.gameObject.transform.localScale.y;
	}

	// Token: 0x0600145A RID: 5210 RVA: 0x000CB950 File Offset: 0x000C9B50
	public void UpdateLabelVisibility()
	{
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "_label", true, true);
		if (gameObject == null)
		{
			return;
		}
		GameObject gameObject2 = global::Common.FindChildByName(gameObject, "_name", true, true);
		if (gameObject2 != null)
		{
			gameObject2.SetActive(!global::Settlement.hide_labels);
		}
		GameObject gameObject3 = global::Common.FindChildByName(gameObject, "_shield", true, true);
		if (gameObject3 != null)
		{
			gameObject3.SetActive(!global::Settlement.hide_labels);
		}
	}

	// Token: 0x0600145B RID: 5211 RVA: 0x000CB9C4 File Offset: 0x000C9BC4
	public static void HideAllLabels(bool hide)
	{
		global::Settlement.hide_labels = hide;
		global::Settlement settlement = global::Settlement.First();
		while (settlement != null)
		{
			settlement.UpdateLabelVisibility();
			settlement = settlement.Next();
		}
	}

	// Token: 0x0600145C RID: 5212 RVA: 0x000CB9F8 File Offset: 0x000C9BF8
	public void CreateLabel(bool force_recreate, bool recalc_res_icons = true)
	{
		if (force_recreate)
		{
			this.DelChildrenByName("_label");
		}
		else if (base.transform.Find("_label"))
		{
			return;
		}
		GameObject gameObject = global::Common.SpawnTemplate("SettlementLabel", "_label", base.transform, true, new Type[]
		{
			typeof(BillBoard)
		});
		gameObject.layer = LayerMask.NameToLayer("Labels");
		gameObject.hideFlags = HideFlags.DontSave;
		this.SnapLabel(gameObject.transform);
		this.name_label = gameObject.transform;
		Vector3 zero = Vector3.zero;
		if (recalc_res_icons)
		{
			this.res_icons = null;
		}
		GameObject gameObject2 = ResourceBar.Create(this, "_resources", gameObject.transform);
		if (gameObject2 != null)
		{
			gameObject2.layer = gameObject.layer;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.identity;
			float size = WV_Scale.GetSize(WV_Scale.Object_Type.SettlementResources);
			gameObject2.transform.localScale *= size;
			zero.y += ResourceBar.def.cell_height * size;
		}
		float num = 0f;
		float num2 = 1f;
		GameObject gameObject3 = null;
		if (this.Name != "" && this.NamePrefab != null)
		{
			gameObject3 = global::Common.Spawn(this.NamePrefab, gameObject.transform, false, "");
			gameObject3.SetActive(!global::Settlement.hide_labels);
			gameObject3.name = "_name";
			gameObject3.layer = gameObject.layer;
			gameObject3.transform.localPosition = zero;
			gameObject3.transform.localRotation = Quaternion.identity;
			gameObject3.transform.localScale *= WV_Scale.GetSize(WV_Scale.Object_Type.TownNameplate);
			TextMeshPro component = gameObject3.GetComponent<TextMeshPro>();
			string key = "tn_" + (string.IsNullOrEmpty(this.TownName) ? this.Name : this.TownName);
			if (component != null)
			{
				UIText.SetTextKey(component, key, null, null);
				component.ForceMeshUpdate(true);
				TMP_TextInfo textInfo = component.textInfo;
				if (textInfo.lineCount > 0)
				{
					RectTransform rectTransform = gameObject3.transform as RectTransform;
					if (rectTransform != null)
					{
						num2 = rectTransform.rect.height;
					}
					else
					{
						num2 = textInfo.lineInfo[0].lineHeight;
					}
					num = textInfo.lineInfo[0].lineExtents.max.x - textInfo.lineInfo[0].lineExtents.min.x;
					num *= WV_Scale.GetSize(WV_Scale.Object_Type.TownNameplate);
				}
			}
		}
		float num3 = 1.6f;
		float num4 = -0.25f * WV_Scale.GetSize(WV_Scale.Object_Type.SettlementResources);
		GameObject gameObject4 = null;
		GameObject gameObject5 = null;
		if ((this.logic != null && this.logic.type == "Castle") || this.IsCastle())
		{
			float num5 = num2 * num3;
			float num6 = num / 2f + num5 / 2f + num4;
			float y = num2 * 0.69f;
			gameObject4 = this.CreateShield(gameObject, zero + new Vector3(-num6, y, 0f), num5, num5, -1);
			if (gameObject4 != null)
			{
				gameObject4.SetActive(!global::Settlement.hide_labels);
			}
			this.UpdateShield();
		}
		if (this.logic != null && this.logic.battle != null && this.logic.battle.is_siege && this.logic.battle.visuals != null)
		{
			(this.logic.battle.visuals as global::Battle).FixSiegeLabel();
		}
		float num7 = -0.25f * num3;
		float num8 = 0f;
		num8 += ((gameObject4 != null) ? (num2 / 2f * num3 + num4 / 2f + num7) : 0f);
		num8 -= ((gameObject5 != null) ? (num2 / 2f * num3 + num4 + num7) : 0f);
		if (gameObject3 != null)
		{
			gameObject3.transform.Translate(num8, 0f, 0f, Space.Self);
		}
		if (gameObject4 != null)
		{
			gameObject4.transform.Translate(num8, 0f, 0f, Space.Self);
		}
		if (gameObject5 != null)
		{
			gameObject5.transform.Translate(num8, 0f, 0f, Space.Self);
		}
	}

	// Token: 0x0600145D RID: 5213 RVA: 0x000CBE64 File Offset: 0x000CA064
	public void UpdateCrest()
	{
		CrestObject[] componentsInChildren = base.gameObject.GetComponentsInChildren<CrestObject>(true);
		if (componentsInChildren != null && componentsInChildren.Length != 0)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (!(componentsInChildren[i].shieldMode != "flag"))
				{
					componentsInChildren[i].RefreshCrest();
					componentsInChildren[i].transform.rotation = Quaternion.Euler(0f, -90f, 0f);
				}
			}
		}
	}

	// Token: 0x0600145E RID: 5214 RVA: 0x000CBED0 File Offset: 0x000CA0D0
	public Vector3 GetCenter()
	{
		return this.aabb.center;
	}

	// Token: 0x0600145F RID: 5215 RVA: 0x000CBEDD File Offset: 0x000CA0DD
	public float GetRadius()
	{
		if (this.logic != null)
		{
			return this.logic.GetRadius();
		}
		return this.CalcRadius();
	}

	// Token: 0x06001460 RID: 5216 RVA: 0x000CBEF9 File Offset: 0x000CA0F9
	public float CalcRadius()
	{
		return Mathf.Max(this.aabb.extents.x, this.aabb.extents.z);
	}

	// Token: 0x06001461 RID: 5217 RVA: 0x000CBF20 File Offset: 0x000CA120
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

	// Token: 0x06001462 RID: 5218 RVA: 0x000CBF78 File Offset: 0x000CA178
	public global::Realm GetRealm()
	{
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return null;
		}
		return worldMap.RealmAt(base.transform.position.x, base.transform.position.z);
	}

	// Token: 0x06001463 RID: 5219 RVA: 0x000CBFBC File Offset: 0x000CA1BC
	public int GetKingdomID()
	{
		if (this.logic == null)
		{
			return 0;
		}
		return this.logic.kingdom_id;
	}

	// Token: 0x06001464 RID: 5220 RVA: 0x000CBFD4 File Offset: 0x000CA1D4
	public int GetControllerKingdomID()
	{
		Logic.Settlement settlement = this.logic;
		int? num;
		if (settlement == null)
		{
			num = null;
		}
		else
		{
			Logic.Object controller = settlement.GetController();
			if (controller == null)
			{
				num = null;
			}
			else
			{
				Logic.Kingdom kingdom = controller.GetKingdom();
				num = ((kingdom != null) ? new int?(kingdom.id) : null);
			}
		}
		int? num2 = num;
		if (num2 == null)
		{
			return 0;
		}
		return num2.GetValueOrDefault();
	}

	// Token: 0x06001465 RID: 5221 RVA: 0x000CC03B File Offset: 0x000CA23B
	private static void AddSelectionPoint(Vector3 pt, Vector3 origin)
	{
		pt = global::Common.SnapToTerrain(pt, 0.1f, null, -1f, false);
		pt -= origin;
		global::Settlement.tmp_selection_points.Add(pt);
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x000CC068 File Offset: 0x000CA268
	public List<Vector3> GetWallSelectionPoints(float radd = 0f, Vector3 origin = default(Vector3))
	{
		Wall wall = this.wall;
		List<WallCorner> list = (wall != null) ? wall.GetCorners() : null;
		if (list == null)
		{
			return null;
		}
		global::Settlement.tmp_selection_points.Clear();
		Vector3 center = this.GetCenter();
		radd += (this.wall.gameObject.activeSelf ? 1f : 0f);
		center.y = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			WallCorner wallCorner = list[i];
			Vector3 vector = wallCorner.transform.position;
			vector.y = 0f;
			Vector3 a = vector - center;
			float magnitude = a.magnitude;
			if (magnitude > 0.001f)
			{
				vector = center + a * (1f + radd / magnitude);
			}
			WallCorner wallCorner2 = list[(i + 1) % list.Count];
			Vector3 vector2 = wallCorner2.transform.position;
			vector2.y = 0f;
			Vector3 a2 = vector2 - center;
			float magnitude2 = a2.magnitude;
			if (magnitude2 > 0.001f)
			{
				vector2 = center + a2 * (1f + radd / magnitude2);
			}
			Vector3 vector3 = vector2 - vector;
			float magnitude3 = vector3.magnitude;
			Keyframe[] array = new Keyframe[2];
			array[0] = new Keyframe(0f, 0f);
			array[0].outTangent = wallCorner.OutCurve * 0.017453292f;
			array[1] = new Keyframe(magnitude3, 0f);
			array[1].inTangent = -wallCorner2.InCurve * 0.017453292f;
			AnimationCurve animationCurve = new AnimationCurve(array);
			global::Settlement.AddSelectionPoint(vector, origin);
			float num = 0f;
			float num2 = 1f;
			int num3 = (int)(magnitude3 / num2);
			Vector3 rightVector = global::Common.GetRightVector(vector3, 1f);
			vector3 /= magnitude3;
			for (int j = 0; j < num3 - 1; j++)
			{
				num += num2;
				Vector3 a3 = vector + vector3 * num;
				float d = animationCurve.Evaluate(num) * this.wall.Curveness;
				global::Settlement.AddSelectionPoint(a3 + rightVector * d, origin);
			}
			global::Settlement.AddSelectionPoint(vector2, origin);
		}
		return global::Settlement.tmp_selection_points;
	}

	// Token: 0x06001467 RID: 5223 RVA: 0x000CC2B0 File Offset: 0x000CA4B0
	public List<Vector3> GetCircleSelectionPoints(float radd = 0f, Vector3 origin = default(Vector3))
	{
		global::Settlement.tmp_selection_points.Clear();
		Vector3 center = this.GetCenter();
		float num = this.GetRadius() + radd;
		float num2 = 0f;
		float num3 = 6.2831855f / (float)this.selection_vertices;
		for (int i = 0; i <= this.selection_vertices; i++)
		{
			float num4 = Mathf.Sin(num2);
			float num5 = Mathf.Cos(num2);
			global::Settlement.AddSelectionPoint(new Vector3(center.x + num5 * num * base.transform.localScale.x, center.y, center.z + num4 * num * base.transform.localScale.z), origin);
			num2 += num3;
		}
		return global::Settlement.tmp_selection_points;
	}

	// Token: 0x06001468 RID: 5224 RVA: 0x000CC364 File Offset: 0x000CA564
	public List<Vector3> GetSelectionPoints(float radd = 0f, Vector3 origin = default(Vector3))
	{
		List<Vector3> wallSelectionPoints = this.GetWallSelectionPoints(radd, origin);
		if (wallSelectionPoints != null)
		{
			return wallSelectionPoints;
		}
		return this.GetCircleSelectionPoints(radd, origin);
	}

	// Token: 0x06001469 RID: 5225 RVA: 0x000CC38C File Offset: 0x000CA58C
	public void CreatePickerCollider()
	{
		if (this.picker_collider != null)
		{
			return;
		}
		Vector3 origin = global::Common.SnapToTerrain(base.transform.position, 0.1f, null, -1f, false);
		Mesh sharedMesh = MeshUtils.CreateTriangleFanMesh(this.GetSelectionPoints(0f, origin));
		MeshCollider orAddComponent = base.gameObject.GetOrAddComponent<MeshCollider>();
		orAddComponent.sharedMesh = sharedMesh;
		this.picker_collider = orAddComponent;
	}

	// Token: 0x0600146A RID: 5226 RVA: 0x000CC3F4 File Offset: 0x000CA5F4
	public MeshRenderer CreateSelection(bool refresh = true)
	{
		this.m_InvalidateSelection = false;
		if (refresh)
		{
			this.DestroySelection();
		}
		if (this.selection != null)
		{
			return this.selection;
		}
		if (this.ui == null)
		{
			this.ui = WorldUI.Get();
			if (this.ui == null)
			{
				return null;
			}
		}
		if (!this.ui.SelectionShown())
		{
			return null;
		}
		float @float = global::Defs.GetFloat("Settlement", "selection_thickness", null, 0.5f);
		List<Vector3> selectionPoints = this.GetSelectionPoints(0f, default(Vector3));
		this.currentMaterial = this.ui.GetSelectionMaterial(base.gameObject, null, this.primarySelection, false);
		GameObject gameObject = MeshUtils.CreateLinesObject(this.currentMaterial, selectionPoints, @float, -1f, false, 2, true, false, null);
		gameObject.name = "_Selection";
		gameObject.layer = base.gameObject.layer;
		gameObject.transform.SetParent(base.transform, true);
		this.selection = gameObject.GetComponent<MeshRenderer>();
		if (this.logic.keep_effects != null && this.selection != null && this.logic.keep_effects.CanAttack())
		{
			MeshRenderer meshRenderer = MeshUtils.CreateSelectionCircle(base.gameObject, this.logic.keep_effects.def.max_distance, this.ui.selectionSettings.attritionRangeMaterial, 0.5f);
			MeshUtils.AdjustObjToTerrain(meshRenderer.gameObject, 0.3f, null, true);
			meshRenderer.name = "_Attrition_Range";
			meshRenderer.gameObject.layer = base.gameObject.layer;
			meshRenderer.transform.SetParent(this.selection.transform, true);
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			this.shoot_range_selection = meshRenderer;
		}
		return this.selection;
	}

	// Token: 0x0600146B RID: 5227 RVA: 0x000CC5CC File Offset: 0x000CA7CC
	public void DestroySelection()
	{
		if (this.shoot_range_selection != null)
		{
			MeshFilter component = this.shoot_range_selection.GetComponent<MeshFilter>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component.mesh);
			}
			UnityEngine.Object.Destroy(this.shoot_range_selection.gameObject);
			this.shoot_range_selection = null;
		}
		if (this.currentMaterial != null)
		{
			UnityEngine.Object.Destroy(this.currentMaterial);
			this.currentMaterial = null;
		}
		if (this.selection != null)
		{
			MeshFilter component2 = this.selection.GetComponent<MeshFilter>();
			if (component2 != null)
			{
				UnityEngine.Object.Destroy(component2.mesh);
			}
			UnityEngine.Object.Destroy(this.selection.gameObject);
			this.selection = null;
		}
	}

	// Token: 0x0600146C RID: 5228 RVA: 0x000CC683 File Offset: 0x000CA883
	private void UpdateSelection()
	{
		if (this.selection == null)
		{
			return;
		}
		this.selection.sharedMaterial = this.ui.GetSelectionMaterial(base.gameObject, this.selection.sharedMaterial, this.primarySelection, false);
	}

	// Token: 0x0600146D RID: 5229 RVA: 0x000CC6C2 File Offset: 0x000CA8C2
	public void SetSelected(bool bSelected, bool bPrimaryselection = true)
	{
		this.selected = bSelected;
		this.primarySelection = bPrimaryselection;
		this.m_InvalidateSelection = true;
		if (this.ui_pvFigure != null)
		{
			this.ui_pvFigure.UpdateIcon();
		}
	}

	// Token: 0x0600146E RID: 5230 RVA: 0x000CC6F2 File Offset: 0x000CA8F2
	private void LateUpdate()
	{
		if (this.m_InvalidateSelection)
		{
			this.ValidateSelection();
			this.m_InvalidateSelection = false;
		}
	}

	// Token: 0x0600146F RID: 5231 RVA: 0x000CC709 File Offset: 0x000CA909
	private void ValidateSelection()
	{
		if (!this.visible)
		{
			return;
		}
		if (this.selected && this.logic.def.is_active_settlement)
		{
			this.CreateSelection(true);
			return;
		}
		this.DestroySelection();
	}

	// Token: 0x06001470 RID: 5232 RVA: 0x000CC740 File Offset: 0x000CA940
	public bool IsSelected()
	{
		if (this.selected)
		{
			return true;
		}
		Logic.Settlement settlement = this.logic;
		object obj;
		if (settlement == null)
		{
			obj = null;
		}
		else
		{
			Logic.Battle battle = settlement.battle;
			obj = ((battle != null) ? battle.visuals : null);
		}
		global::Battle battle2 = obj as global::Battle;
		return battle2 != null && battle2.IsSelected();
	}

	// Token: 0x06001471 RID: 5233 RVA: 0x000CC78F File Offset: 0x000CA98F
	public void RecreateSelectionUI()
	{
		if (this.ui != null)
		{
			this.ui.RefreshSelection(base.gameObject);
		}
	}

	// Token: 0x06001472 RID: 5234 RVA: 0x000CC7B0 File Offset: 0x000CA9B0
	public void Refresh(global::Settlement.RefreshMode mode = global::Settlement.RefreshMode.Spawn, bool instantly = false)
	{
		if (!instantly)
		{
			if (mode > this.refresh)
			{
				this.refresh = mode;
			}
			return;
		}
		if (mode < this.refresh)
		{
			mode = this.refresh;
		}
		this.refresh = global::Settlement.RefreshMode.None;
		if (mode >= global::Settlement.RefreshMode.Randomize && this.setType == "Random")
		{
			this.curType = null;
		}
		if (this.UpdateType(false) && mode < global::Settlement.RefreshMode.Label)
		{
			mode = global::Settlement.RefreshMode.Label;
		}
		if (mode >= global::Settlement.RefreshMode.Respawn)
		{
			global::Common.SnapToTerrain(base.transform, 0f, null, -1f);
		}
		Profile.BeginSection("Settlement.UpdateLevel");
		this.UpdateLevel(false, false, true);
		Profile.EndSection("Settlement.UpdateLevel");
		Profile.BeginSection("Settlement.RefreshHouses");
		this.RefreshHouses(mode >= global::Settlement.RefreshMode.Randomize, mode >= global::Settlement.RefreshMode.FullRandomize);
		Profile.EndSection("Settlement.RefreshHouses");
		Profile.BeginSection("Settlement.CreateLabel");
		this.CreateLabel(mode >= global::Settlement.RefreshMode.Label, true);
		Profile.EndSection("Settlement.CreateLabel");
		this.CalcBounds();
		if (mode >= global::Settlement.RefreshMode.Spawn && this.wall != null)
		{
			Profile.BeginSection("Settlement.Refresh wall");
			this.wall.Refresh(false);
			Profile.EndSection("Settlement.Refresh wall");
		}
		if (Application.isPlaying)
		{
			this.CreatePickerCollider();
		}
	}

	// Token: 0x06001473 RID: 5235 RVA: 0x000CC8DC File Offset: 0x000CAADC
	public void Snap()
	{
		if (this.citadel != null)
		{
			this.citadel.Snap();
		}
		for (int i = 0; i < this.houses.Count; i++)
		{
			this.houses[i].Snap();
		}
	}

	// Token: 0x06001474 RID: 5236 RVA: 0x000CC929 File Offset: 0x000CAB29
	public void Fix()
	{
		this.Refresh(global::Settlement.RefreshMode.Respawn, true);
	}

	// Token: 0x06001475 RID: 5237 RVA: 0x000CC934 File Offset: 0x000CAB34
	public static void SpawnAll()
	{
		global::Settlement settlement = global::Settlement.First();
		while (settlement != null)
		{
			if (settlement.refresh != global::Settlement.RefreshMode.None)
			{
				settlement.Refresh(settlement.refresh, true);
			}
			settlement = settlement.Next();
		}
	}

	// Token: 0x06001476 RID: 5238 RVA: 0x000CC970 File Offset: 0x000CAB70
	public static void RefreshAll(global::Settlement.RefreshMode mode)
	{
		int num = 0;
		global::Settlement settlement = global::Settlement.First();
		while (settlement != null)
		{
			num++;
			settlement = settlement.Next();
		}
		int num2 = 0;
		global::Settlement settlement2 = global::Settlement.First();
		while (settlement2 != null)
		{
			num2++;
			if (!global::Common.EditorProgress("Refreshing", settlement2.gameObject.name, (float)num2 / (float)num, true))
			{
				BuildTools.cancelled = true;
				break;
			}
			settlement2.Refresh(mode, true);
			settlement2 = settlement2.Next();
		}
		global::Common.EditorProgress(null, null, 0f, false);
	}

	// Token: 0x06001477 RID: 5239 RVA: 0x000CC9F4 File Offset: 0x000CABF4
	public void CalcBounds()
	{
		Profile.BeginSection("Settlement.CalcBounds");
		Vector3 position = base.transform.position;
		position.y = global::Common.GetHeight(position, null, -1f, false);
		this.aabb = new Bounds(position, Vector3.zero);
		base.GetComponentsInChildren<Renderer>(global::Settlement.s_renderers);
		for (int i = 0; i < global::Settlement.s_renderers.Count; i++)
		{
			Renderer renderer = global::Settlement.s_renderers[i];
			if (renderer.enabled && !(renderer is SkinnedMeshRenderer) && !(renderer is ParticleSystemRenderer) && (renderer.name.Length <= 0 || renderer.name[0] != '_') && (renderer.bounds.center - position).magnitude <= 15f)
			{
				if (this.aabb.size.sqrMagnitude > 0f)
				{
					this.aabb.Encapsulate(renderer.bounds);
				}
				else
				{
					this.aabb = renderer.bounds;
				}
			}
		}
		global::Settlement.s_renderers.Clear();
		if (this.aabb.size == Vector3.zero)
		{
			float num = this.IsCastle() ? 15f : 5f;
			this.aabb.size = new Vector3(num, 5f, num);
		}
		this.aabb.size = new Vector3(this.aabb.size.x, this.IsCastle() ? 7f : 3f, this.aabb.size.z);
		this.SnapLabel(null);
		Profile.EndSection("Settlement.CalcBounds");
	}

	// Token: 0x06001478 RID: 5240 RVA: 0x000CCBB0 File Offset: 0x000CADB0
	private void RandomRotate(PrefabGrid house)
	{
		if (house.set_variant > 0)
		{
			return;
		}
		float num = (float)(Random.Range(0, 6) * 60);
		num = global::Common.NormalizeAngle360(house.transform.eulerAngles.y + num);
		house.transform.eulerAngles = new Vector3(0f, num, 0f);
	}

	// Token: 0x06001479 RID: 5241 RVA: 0x000CCC08 File Offset: 0x000CAE08
	public void RefreshHouses(bool randomize_variant, bool randomize_level)
	{
		for (int i = 0; i < this.houses.Count; i++)
		{
			PrefabGrid prefabGrid = this.houses[i];
			if (randomize_variant && !Application.isPlaying)
			{
				prefabGrid.cur_variant = 0;
				this.RandomRotate(prefabGrid);
			}
			if (randomize_level)
			{
				prefabGrid.cur_level = 0;
			}
			prefabGrid.Refresh(false, true);
		}
		if (this.citadel != null)
		{
			this.citadel.Refresh(false, true);
		}
	}

	// Token: 0x0600147A RID: 5242 RVA: 0x000CCC80 File Offset: 0x000CAE80
	public void CalcLevelFromHouses()
	{
		if (this.houses.Count <= 0)
		{
			return;
		}
		Profile.BeginSection("Settlement.CalcLevelFromHouses");
		this.level = 1;
		for (int i = 0; i < this.houses.Count; i++)
		{
			PrefabGrid prefabGrid = this.houses[i];
			prefabGrid.UpdateInfo(false);
			prefabGrid.DecideLevel();
			if (prefabGrid.set_level <= 0 && prefabGrid.cur_level > 1)
			{
				this.level += prefabGrid.cur_level - 1;
			}
		}
		Profile.EndSection("Settlement.CalcLevelFromHouses");
	}

	// Token: 0x0600147B RID: 5243 RVA: 0x000CCD10 File Offset: 0x000CAF10
	public PrefabGrid ChooseHouseToUpgrade()
	{
		Profile.BeginSection("Settlement.ChooseHouseToUpgrade");
		int num = -1;
		PrefabGrid prefabGrid = null;
		for (int i = 0; i < this.houses.Count; i++)
		{
			PrefabGrid prefabGrid2 = this.houses[i];
			if (prefabGrid2.set_level <= 0)
			{
				int num2 = prefabGrid2.cur_level;
				if (num2 < prefabGrid2.GetMaxLevel())
				{
					if (num2 < 1)
					{
						num2 = 1;
					}
					if (!(prefabGrid != null) || num > num2)
					{
						prefabGrid = prefabGrid2;
						num = num2;
					}
				}
			}
		}
		Profile.EndSection("Settlement.ChooseHouseToUpgrade");
		return prefabGrid;
	}

	// Token: 0x0600147C RID: 5244 RVA: 0x000CCD90 File Offset: 0x000CAF90
	public PrefabGrid ChooseHouseToDegrade()
	{
		Profile.BeginSection("Settlement.ChooseHouseToDegrade");
		int num = -1;
		PrefabGrid prefabGrid = null;
		for (int i = 0; i < this.houses.Count; i++)
		{
			PrefabGrid prefabGrid2 = this.houses[i];
			if (prefabGrid2.set_level <= 0)
			{
				int num2 = prefabGrid2.cur_level;
				if (num2 > 1)
				{
					int maxLevel = prefabGrid2.GetMaxLevel();
					if (num2 > maxLevel)
					{
						num2 = maxLevel;
					}
					if (!(prefabGrid != null) || num < num2)
					{
						prefabGrid = prefabGrid2;
						num = num2;
					}
				}
			}
		}
		Profile.EndSection("Settlement.ChooseHouseToDegrade");
		return prefabGrid;
	}

	// Token: 0x0600147D RID: 5245 RVA: 0x000CCE14 File Offset: 0x000CB014
	public void UpdateHouseLevels(int lvl, bool refresh)
	{
		if (this.houses.Count <= 0)
		{
			return;
		}
		Profile.BeginSection("Settlement.UpdateHouseLevels");
		this.CalcLevelFromHouses();
		while (this.level < lvl)
		{
			PrefabGrid prefabGrid = this.ChooseHouseToUpgrade();
			if (prefabGrid == null)
			{
				Profile.EndSection("Settlement.UpdateHouseLevels");
				return;
			}
			prefabGrid.cur_level++;
			if (refresh && (this.was_visible || !Application.isPlaying))
			{
				prefabGrid.Refresh(true, true);
			}
			this.level++;
			this.batch = true;
		}
		while (this.level > lvl)
		{
			PrefabGrid prefabGrid2 = this.ChooseHouseToDegrade();
			if (prefabGrid2 == null)
			{
				Profile.EndSection("Settlement.UpdateHouseLevels");
				return;
			}
			prefabGrid2.cur_level--;
			if (refresh && (this.was_visible || !Application.isPlaying))
			{
				prefabGrid2.Refresh(true, true);
			}
			this.level--;
			this.batch = true;
		}
		Profile.EndSection("Settlement.UpdateHouseLevels");
	}

	// Token: 0x0600147E RID: 5246 RVA: 0x000CCF10 File Offset: 0x000CB110
	private void FindPrefabGrids(Transform parent, List<PrefabGrid> pgs)
	{
		if (parent == null)
		{
			return;
		}
		if (!parent.gameObject.activeSelf)
		{
			return;
		}
		PrefabGrid component = parent.GetComponent<PrefabGrid>();
		if (component != null)
		{
			pgs.Add(component);
			return;
		}
		foreach (object obj in parent)
		{
			Transform parent2 = (Transform)obj;
			this.FindPrefabGrids(parent2, pgs);
		}
	}

	// Token: 0x0600147F RID: 5247 RVA: 0x000CCF98 File Offset: 0x000CB198
	private void OnEnable()
	{
		this.Register();
	}

	// Token: 0x06001480 RID: 5248 RVA: 0x000CCFA0 File Offset: 0x000CB1A0
	private void OnDestroy()
	{
		if (this.visibility_idx >= 0)
		{
			VisibilityDetector.Del(this.visibility_idx);
			this.visibility_idx = -1;
		}
		if (this.logic != null)
		{
			Logic.Settlement settlement = this.logic;
			this.logic = null;
			if (settlement.IsValid())
			{
				settlement.Destroy(false);
			}
		}
		if (this.OccupiedDecal != null)
		{
			global::Common.DestroyObj(this.OccupiedDecal);
		}
	}

	// Token: 0x06001481 RID: 5249 RVA: 0x000CD006 File Offset: 0x000CB206
	private void OnDisable()
	{
		this.Unregister();
	}

	// Token: 0x06001482 RID: 5250 RVA: 0x000CD010 File Offset: 0x000CB210
	private void Start()
	{
		this.ui = WorldUI.Get();
		if (Application.isPlaying)
		{
			this.batch = true;
			this.additional_emitters = base.GetComponentsInChildren<StudioEventEmitter>();
			if (this.AudioPrefab != null)
			{
				this.audio = global::Common.Spawn(this.AudioPrefab, false, false);
				this.audio.transform.SetParent(base.transform, false);
				this.audio.transform.localPosition = Vector3.zero;
				this.sound_emitter = this.audio.GetComponent<StudioEventEmitter>();
			}
			this.CreateSmoke();
			if (this.logic != null)
			{
				this.UpdateCrest();
			}
		}
		if (this.logic != null && this.IsCastle())
		{
			Logic.Realm realm = this.logic.GetRealm();
			if (realm != null)
			{
				realm.AddListener(this);
			}
			this.UpdateGovernor();
		}
	}

	// Token: 0x06001483 RID: 5251 RVA: 0x000CD0E4 File Offset: 0x000CB2E4
	public void VisibilityChanged(bool is_visible)
	{
		this.visible = is_visible;
		if (this.visible)
		{
			if (this.level_dirty)
			{
				this.UpdateLevel(true, true, true);
			}
			Profile.BeginSection("Settlement.UpdateCrest");
			this.UpdateCrest();
			Profile.EndSection("Settlement.UpdateCrest");
			Profile.BeginSection("Settlement.UpdateFX");
			this.UpdateFX(true);
			Profile.EndSection("Settlement.UpdateFX");
			Profile.BeginSection("Settlement.UpdateGovernor");
			this.UpdateGovernor();
			Profile.EndSection("Settlement.UpdateGovernor");
		}
		Profile.BeginSection("Settlement.UpdateProjectileVisibility");
		this.UpdateProjectileVisibility(is_visible);
		Profile.EndSection("Settlement.UpdateProjectileVisibility");
		if (!this.visible && this.IsSelected())
		{
			this.DestroySelection();
		}
		if (!this.visible || this.was_visible)
		{
			if (this.visible && this.IsSelected())
			{
				this.CreateSelection(true);
			}
			return;
		}
		this.was_visible = true;
		Profile.BeginSection("Settlement.LoadFromLogicDef");
		this.LoadFromLogicDef();
		Profile.EndSection("Settlement.LoadFromLogicDef");
		Profile.BeginSection("Settlement.Refresh on visible");
		this.Refresh(global::Settlement.RefreshMode.Spawn, true);
		Profile.EndSection("Settlement.Refresh on visible");
		if (this.logic.level == 0)
		{
			this.SetRazed(true);
		}
		if (this.IsSelected())
		{
			this.CreateSelection(true);
		}
	}

	// Token: 0x06001484 RID: 5252 RVA: 0x000CD21C File Offset: 0x000CB41C
	private void UpdateProjectileVisibility(bool is_visible)
	{
		if (this.projectile != null)
		{
			Logic.Settlement settlement = this.logic;
			if (settlement.keep_effects != null)
			{
				float num = (UnityEngine.Time.time - this.shoot_time) / settlement.keep_effects.def.shoot_time;
				if (num >= 0f && num <= 1f && is_visible)
				{
					this.projectile.SetActive(true);
					return;
				}
			}
			this.projectile.SetActive(false);
		}
	}

	// Token: 0x06001485 RID: 5253 RVA: 0x000CD297 File Offset: 0x000CB497
	public override Logic.Object GetLogic()
	{
		return this.logic;
	}

	// Token: 0x06001486 RID: 5254 RVA: 0x000CD2A0 File Offset: 0x000CB4A0
	public static void CreateVisuals(Logic.Object logic_obj)
	{
		if (BattleMap.creating_fake_battle)
		{
			return;
		}
		Logic.Settlement settlement = logic_obj as Logic.Settlement;
		if (settlement == null)
		{
			return;
		}
		Castle castle = settlement as Castle;
		string text = (castle != null) ? castle.name : "";
		if (string.IsNullOrEmpty(text))
		{
			string arg = Logic.Object.TypeToStr(settlement.GetType());
			Logic.Realm realm = settlement.GetRealm();
			if (realm != null)
			{
				text = string.Format("{0} {1} in {2}", arg, settlement.GetNid(true), realm.name);
			}
			else
			{
				text = string.Format("{0} {1}", arg, settlement.GetNid(true));
			}
		}
		GameObject gameObject = new GameObject(text);
		gameObject.layer = LayerMask.NameToLayer("Settlements");
		gameObject.SetActive(false);
		global::Common.SetObjectParent(gameObject, GameLogic.instance.transform, "Settlements");
		gameObject.transform.position = global::Common.SnapToTerrain(settlement.position, 0f, null, -1f, false);
		global::Settlement settlement2 = gameObject.AddComponent<global::Settlement>();
		settlement2.logic = settlement;
		settlement.visuals = settlement2;
		if (castle != null)
		{
			settlement2.Name = castle.name;
		}
		settlement2.setType = settlement.type;
		DT.Field field = settlement.field;
		float @float = field.GetFloat("rotation", null, 0f, true, true, true, '.');
		if (@float != 0f)
		{
			gameObject.transform.localEulerAngles = new Vector3(0f, @float, 0f);
		}
		field.GetString("culture", null, "", true, true, true, '.');
		settlement2.NamePrefab = global::Settlement.GetNamePrefabLocalzied(field);
		settlement2.ShieldPrefab = global::Defs.GetObj<GameObject>(field, "shield_prefab", null);
		settlement2.resourceMaterial = global::Defs.GetObj<Material>(field, "resources_material", null);
		if (settlement2.IsCastle())
		{
			settlement2.AudioPrefab = global::Defs.GetObj<GameObject>(field, "audio_prefab", null);
			settlement2.SmokePrefab = global::Defs.GetObj<GameObject>(field, "smoke_prefab", null);
		}
		settlement2.ArrowsPrefab = global::Defs.GetObj<GameObject>(field, "arrows_prefab", null);
		settlement2.CannonBallsPrefab = global::Defs.GetObj<GameObject>(field, "cannons_prefab", null);
		settlement2.visibility_idx = VisibilityDetector.Add(settlement2.transform.position, 15f, gameObject, settlement2, -1);
		settlement2.UpdatePVFigure();
		settlement2.UpdateOccupiedDecal();
	}

	// Token: 0x06001487 RID: 5255 RVA: 0x000CD4E0 File Offset: 0x000CB6E0
	private static GameObject GetNamePrefabLocalzied(DT.Field field)
	{
		GameObject gameObject = null;
		if (!string.IsNullOrEmpty(global::Defs.Language))
		{
			gameObject = global::Defs.GetObj<GameObject>(field, "name_prefab." + global::Defs.Language, null);
		}
		if (gameObject == null)
		{
			gameObject = global::Defs.GetObj<GameObject>(field, "name_prefab", null);
		}
		return gameObject;
	}

	// Token: 0x06001488 RID: 5256 RVA: 0x000CD52C File Offset: 0x000CB72C
	public void LoadFromLogicDef()
	{
		Logic.Settlement settlement = this.logic;
		DT.Field field = (settlement != null) ? settlement.field : null;
		if (field == null)
		{
			return;
		}
		int @int = field.GetInt("level", null, 0, true, true, true, '.');
		if (@int > 0)
		{
			this.fix_level = true;
			this.level = @int;
		}
		else
		{
			this.fix_level = false;
		}
		this.LoadCitadel(field);
		this.LoadHouses(field);
		this.LoadWall(field);
		this.LoadBVDetails(field);
	}

	// Token: 0x06001489 RID: 5257 RVA: 0x000CD59C File Offset: 0x000CB79C
	public void LoadCitadel(DT.Field field)
	{
		if (this.citadel != null)
		{
			UnityEngine.Object.Destroy(this.citadel.gameObject);
		}
		this.citadel = PrefabGrid.Load(field.FindChild("citadel", null, true, true, true, '.'), base.transform, "Citadels", "", 0, 0, true, true);
		if (this.citadel != null)
		{
			this.citadel.SetParent(this, false);
			this.citadel_architecture = this.citadel.architecture;
		}
	}

	// Token: 0x0600148A RID: 5258 RVA: 0x000CD624 File Offset: 0x000CB824
	public void LoadHouses(DT.Field field)
	{
		Transform transform = base.transform.Find("Houses");
		if (transform != null)
		{
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		this.houses.Clear();
		DT.Field field2 = field.FindChild("houses", null, true, true, true, '.');
		if (field2 == null)
		{
			return;
		}
		transform = new GameObject("Houses").transform;
		transform.SetParent(base.transform);
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		Logic.Settlement settlement = this.logic;
		string housesType = global::Settlement.GetHousesType((settlement != null) ? settlement.type : null);
		this.houses_architecture = DT.Unquote(field2.value_str);
		if (this.logic.type != "Castle")
		{
			if (global::Settlement.dummy_house_field == null)
			{
				global::Settlement.dummy_house_field = new DT.Field(null);
				global::Settlement.dummy_house_field.key = "1";
			}
			PrefabGrid prefabGrid = PrefabGrid.Load(global::Settlement.dummy_house_field, transform, housesType, "Auto", 0, 0, true, true);
			prefabGrid.SetParent(this, false);
			this.RandomRotate(prefabGrid);
			this.houses.Add(prefabGrid);
			return;
		}
		if (field2.children == null || field2.children.Count == 0)
		{
			return;
		}
		for (int i = 0; i < field2.children.Count; i++)
		{
			DT.Field field3 = field2.children[i];
			if (!string.IsNullOrEmpty(field3.key))
			{
				PrefabGrid prefabGrid2 = PrefabGrid.Load(field3, transform, housesType, "Auto", 0, 0, true, true);
				prefabGrid2.SetParent(this, false);
				this.RandomRotate(prefabGrid2);
				this.houses.Add(prefabGrid2);
			}
		}
	}

	// Token: 0x0600148B RID: 5259 RVA: 0x000CD7BC File Offset: 0x000CB9BC
	public void LoadBVDetails(DT.Field field)
	{
		Transform transform = base.transform.Find("BV_Details");
		if (transform != null)
		{
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		DT.Field field2 = field.FindChild("bv_details", null, true, true, true, '.');
		if (field2 == null || field2.children == null || field2.children.Count == 0)
		{
			return;
		}
		transform = new GameObject("BV_Details").transform;
		transform.SetParent(base.transform);
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		for (int i = 0; i < field2.children.Count; i++)
		{
			DT.Field field3 = field2.children[i];
			if (!string.IsNullOrEmpty(field3.key))
			{
				PrefabGrid item = PrefabGrid.Load(field3, transform, "Houses", "Auto", 0, 0, true, true);
				this.details.Add(item);
			}
		}
	}

	// Token: 0x0600148C RID: 5260 RVA: 0x000CD8A0 File Offset: 0x000CBAA0
	public static string GetHousesType(string settlement_type)
	{
		if (string.IsNullOrEmpty(settlement_type))
		{
			return "Houses";
		}
		string @string = global::Defs.GetString(settlement_type, "architecture_type", null, "");
		if (!string.IsNullOrEmpty(@string))
		{
			return @string;
		}
		if (settlement_type == "Castle")
		{
			return "Houses";
		}
		if (PrefabGrid.ArchitectureExists(settlement_type))
		{
			return settlement_type;
		}
		settlement_type += "s";
		if (PrefabGrid.ArchitectureExists(settlement_type))
		{
			return settlement_type;
		}
		return "Houses";
	}

	// Token: 0x0600148D RID: 5261 RVA: 0x000CD910 File Offset: 0x000CBB10
	public bool UpdateType(bool refresh)
	{
		if (this.setType == "Random")
		{
			if (this.curType != null)
			{
				return false;
			}
			if (this.GetRealm() == null)
			{
				Debug.LogWarning(string.Format("Invalid Settlement {0} at position {1}! Missing Realm.", base.name, base.transform.position), base.gameObject);
				return false;
			}
			this.curType = Logic.Settlement.GetRandomType(global::Defs.Get(false).dt, this.GetRealm().logic);
		}
		else
		{
			if (this.curType == this.setType)
			{
				return false;
			}
			this.curType = this.setType;
		}
		if (refresh)
		{
			this.CreateLabel(true, true);
		}
		if (this.curType == "Castle")
		{
			this.curType = this.setType;
			return true;
		}
		if (this.houses.Count == 0)
		{
			return true;
		}
		string housesType = global::Settlement.GetHousesType(this.curType);
		if (housesType == null)
		{
			return true;
		}
		for (int i = 0; i < this.houses.Count; i++)
		{
			PrefabGrid component = this.houses[i].GetComponent<PrefabGrid>();
			component.type = housesType;
			if (refresh)
			{
				component.Refresh(true, true);
			}
		}
		return true;
	}

	// Token: 0x0600148E RID: 5262 RVA: 0x000CDA38 File Offset: 0x000CBC38
	public void UpdatePVFigure()
	{
		if (!this.IsCastle())
		{
			return;
		}
		if (this.ui_pvFigure == null)
		{
			GameObject obj = global::Defs.GetObj<GameObject>(global::Defs.GetDefField("PoliticalView", "pv_figures.Settlement"), "ui_figure_prefab", null);
			GameObject prefab = obj;
			WorldUI worldUI = WorldUI.Get();
			GameObject gameObject = global::Common.Spawn(prefab, (worldUI != null) ? worldUI.m_statusBar : null, false, "");
			this.ui_pvFigure = ((gameObject != null) ? gameObject.GetComponent<UIPVFigureSettlement>() : null);
		}
		UIPVFigureSettlement uipvfigureSettlement = this.ui_pvFigure;
		if (uipvfigureSettlement == null)
		{
			return;
		}
		uipvfigureSettlement.SetSettlement(this);
	}

	// Token: 0x0600148F RID: 5263 RVA: 0x000CDAB8 File Offset: 0x000CBCB8
	public void LoadWall(DT.Field field)
	{
		if (this.wall != null)
		{
			UnityEngine.Object.Destroy(this.wall.gameObject);
		}
		this.wall = Wall.Load(field.FindChild("wall", null, true, true, true, '.'), base.transform);
	}

	// Token: 0x06001490 RID: 5264 RVA: 0x000CDB08 File Offset: 0x000CBD08
	public void GovernorIconCallback(Sprite sp, Logic.Character c)
	{
		Transform transform = base.transform.Find("_label/_governor");
		if (transform == null)
		{
			return;
		}
		Castle castle = this.logic as Castle;
		Village village = this.logic as Village;
		Logic.Character character = (castle == null) ? ((village == null) ? null : village.famous_person) : castle.governor;
		if (this.ui == null || character == null)
		{
			transform.gameObject.SetActive(false);
			return;
		}
		SpriteRenderer component = transform.GetComponent<SpriteRenderer>();
		Sprite icon = global::Character.GetIcon(character, 64f);
		if (icon != null)
		{
			component.sprite = icon;
			transform.gameObject.SetActive(true);
			return;
		}
		transform.gameObject.SetActive(false);
	}

	// Token: 0x06001491 RID: 5265 RVA: 0x000CDBC0 File Offset: 0x000CBDC0
	private GameObject CreateShield(GameObject parent, Vector3 position, float width, float height, int kingodmId = -1)
	{
		if ((this.logic == null || !(this.logic.type == "Castle")) && !this.IsCastle())
		{
			return null;
		}
		if (this.ShieldPrefab == null)
		{
			this.ShieldPrefab = global::Defs.GetObj<GameObject>("Settlement", "shield_prefab", null);
		}
		if (this.ShieldPrefab == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ShieldPrefab, parent.transform, false);
		gameObject.name = "_shield";
		gameObject.layer = parent.layer;
		gameObject.transform.localPosition = position;
		gameObject.transform.rotation = Quaternion.identity;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = new Vector3(width, height, 1f);
		if (kingodmId != -1)
		{
			CrestObject crestObject = gameObject.GetComponent<CrestObject>();
			if (crestObject == null)
			{
				crestObject = gameObject.GetComponentInChildren<CrestObject>();
			}
			if (crestObject != null)
			{
				crestObject.SetKingdomId(kingodmId);
				gameObject.name = "_visitor_shield";
			}
		}
		return gameObject;
	}

	// Token: 0x06001492 RID: 5266 RVA: 0x000CDCD4 File Offset: 0x000CBED4
	public void UpdateShield()
	{
		if ((this.logic == null || !(this.logic.type == "Castle")) && !this.IsCastle())
		{
			return;
		}
		Transform transform = base.transform.Find("_label/_shield");
		if (transform == null)
		{
			return;
		}
		CrestObject component = transform.GetComponent<CrestObject>();
		if (component == null)
		{
			return;
		}
		Logic.Settlement settlement = this.logic;
		Logic.Kingdom kingdom = (settlement != null) ? settlement.GetKingdom() : null;
		if (kingdom != null)
		{
			component.SetKingdomId(kingdom.id);
		}
	}

	// Token: 0x06001493 RID: 5267 RVA: 0x000CDD58 File Offset: 0x000CBF58
	public void UpdateGovernor()
	{
		Transform transform = base.transform.Find("_label/_governor");
		if (transform == null)
		{
			return;
		}
		Castle castle = this.logic as Castle;
		Village village = this.logic as Village;
		Logic.Character character = (castle == null) ? ((village == null) ? null : village.famous_person) : castle.governor;
		if (this.visible)
		{
			this.ui = WorldUI.Get();
		}
		if (this.ui == null || character == null)
		{
			transform.gameObject.SetActive(false);
			return;
		}
		SpriteRenderer component = transform.GetComponent<SpriteRenderer>();
		if (!(component != null))
		{
			transform.gameObject.SetActive(false);
			return;
		}
		Sprite icon = global::Character.GetIcon(character, 64f);
		if (icon != null)
		{
			component.sprite = icon;
			transform.gameObject.SetActive(true);
			return;
		}
		transform.gameObject.SetActive(false);
	}

	// Token: 0x06001494 RID: 5268 RVA: 0x000CDE3C File Offset: 0x000CC03C
	private bool CreateSmoke()
	{
		if (this.smoke != null)
		{
			return true;
		}
		if (this.SmokePrefab != null)
		{
			this.smoke = global::Common.Spawn(this.SmokePrefab, false, false);
			this.smoke.transform.SetParent(base.transform, false);
			this.smoke.transform.localPosition = Vector3.zero;
			ParticleSystem[] componentsInChildren = this.smoke.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].subEmitters.enabled = false;
				componentsInChildren[i].Stop();
			}
		}
		return this.smoke != null;
	}

	// Token: 0x06001495 RID: 5269 RVA: 0x000CDEE8 File Offset: 0x000CC0E8
	private bool CreateProjectile()
	{
		if (this.logic.keep_effects == null)
		{
			return false;
		}
		bool flag = this.logic.GetRealm().HasTag("keep_cannons", 1);
		if (this.projectile != null)
		{
			if (this.cannons == flag)
			{
				float num = (UnityEngine.Time.time - this.shoot_time) / this.logic.keep_effects.def.shoot_time;
				if (num >= 0f && num <= 1f && this.visible)
				{
					this.projectile.SetActive(true);
					ParticleSystem[] componentsInChildren = this.projectile.GetComponentsInChildren<ParticleSystem>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].subEmitters.enabled = true;
						componentsInChildren[i].Play();
					}
				}
				return true;
			}
			this.cannons = flag;
			UnityEngine.Object.Destroy(this.projectile.gameObject);
		}
		this.cannons = flag;
		GameObject gameObject;
		if (this.cannons)
		{
			gameObject = this.CannonBallsPrefab;
		}
		else
		{
			gameObject = this.ArrowsPrefab;
		}
		if (gameObject != null)
		{
			this.projectile = global::Common.Spawn(gameObject, false, false);
			this.projectile.transform.SetParent(base.transform, false);
			this.projectile.transform.localPosition = Vector3.zero;
			this.projectile.SetActive(this.visible);
			this.projectile.name = "_Projectile";
			ParticleSystem[] componentsInChildren2 = this.projectile.GetComponentsInChildren<ParticleSystem>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].subEmitters.enabled = true;
				componentsInChildren2[j].Play();
			}
		}
		return this.projectile != null;
	}

	// Token: 0x06001496 RID: 5270 RVA: 0x000CE0A8 File Offset: 0x000CC2A8
	private void UpdateFX(bool pre_warm = true)
	{
		Castle castle = this.logic as Castle;
		if (castle != null && this.CreateSmoke())
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			ParticleSystem[] componentsInChildren = this.smoke.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].main.prewarm = pre_warm;
				if (castle.sacked && !componentsInChildren[i].isPlaying)
				{
					componentsInChildren[i].subEmitters.enabled = true;
					componentsInChildren[i].Play();
				}
				else if (!castle.sacked)
				{
					componentsInChildren[i].subEmitters.enabled = false;
					componentsInChildren[i].Stop();
				}
			}
		}
		if (this.projectile != null)
		{
			Logic.Settlement settlement = this.logic;
			float num = (UnityEngine.Time.time - this.shoot_time) / settlement.keep_effects.def.shoot_time;
			if (num >= 0f && num <= 1f && this.visible)
			{
				ParticleSystem[] componentsInChildren2 = this.projectile.GetComponentsInChildren<ParticleSystem>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					componentsInChildren2[j].subEmitters.enabled = true;
					componentsInChildren2[j].Simulate(1f, true, true);
				}
			}
		}
	}

	// Token: 0x06001497 RID: 5271 RVA: 0x000CE1F4 File Offset: 0x000CC3F4
	private void RemoveFromControlGroup()
	{
		if (this.logic == null)
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null)
		{
			baseUI.ClearFromControlGroup(this.logic);
		}
	}

	// Token: 0x06001498 RID: 5272 RVA: 0x000CE225 File Offset: 0x000CC425
	public void OnLocalPlayerChanged()
	{
		this.UpdateOccupiedDecal();
	}

	// Token: 0x06001499 RID: 5273 RVA: 0x000CE230 File Offset: 0x000CC430
	public void UpdateOccupiedDecal()
	{
		this.m_InvalidateOccupiedDecal = true;
		if (this.logic == null)
		{
			return;
		}
		if (Game.isLoadingSaveGame)
		{
			return;
		}
		if (!this.visible)
		{
			return;
		}
		this.m_InvalidateOccupiedDecal = false;
		if (this.logic.IsOccupied())
		{
			if (this.OccupiedDecalPrefab == null)
			{
				this.OccupiedDecalPrefab = global::Defs.GetObj<GameObject>(this.logic.field, "occupied_decal_prefab", null);
				if (this.OccupiedDecalPrefab == null)
				{
					return;
				}
			}
			if (this.OccupiedDecal == null)
			{
				this.OccupiedDecal = global::Common.SpawnPooled(this.OccupiedDecalPrefab, base.transform, false, "").GetComponent<MeshRenderer>();
				this.OccupiedDecal.transform.position = base.transform.position;
				this.OccupiedDecal.transform.eulerAngles = new Vector3(0f, 0f, 0f);
				this.OccupiedDecal.transform.localScale = Vector3.one;
			}
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			Logic.Object controller = this.logic.keep_effects.GetController();
			RelationUtils.Stance stance = kingdom.GetStance(controller);
			string key;
			if ((stance & RelationUtils.Stance.War) != RelationUtils.Stance.None)
			{
				key = "occupied_enemy";
				Logic.Kingdom kingdom2 = controller as Logic.Kingdom;
				if (kingdom2 != null)
				{
					War war = kingdom.FindWarWith(kingdom2);
					if (war != null && war.GetSupporters(war.GetSide(kingdom2)).Contains(kingdom2))
					{
						key = "occupied_enemy_supporter";
					}
				}
			}
			else if ((stance & RelationUtils.Stance.Alliance) != RelationUtils.Stance.None)
			{
				key = "occupied_ally";
			}
			else if ((stance & RelationUtils.Stance.Own) != RelationUtils.Stance.None)
			{
				key = "occupied_own";
			}
			else
			{
				key = "occupied_neutral";
			}
			Color color = global::Defs.GetColor("Keep", key);
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			materialPropertyBlock.SetColor("_Color", color);
			this.OccupiedDecal.SetPropertyBlock(materialPropertyBlock);
			return;
		}
		if (this.OccupiedDecal != null)
		{
			global::Common.DestroyObj(this.OccupiedDecal);
			this.OccupiedDecal = null;
		}
	}

	// Token: 0x0600149A RID: 5274 RVA: 0x000CE40C File Offset: 0x000CC60C
	public override void OnMessage(object obj, string message, object param)
	{
		if (this.logic == null)
		{
			return;
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 2162786432U)
		{
			if (num > 810626458U)
			{
				if (num <= 1248572439U)
				{
					if (num != 925884271U)
					{
						if (num != 1211309691U)
						{
							if (num != 1248572439U)
							{
								return;
							}
							if (!(message == "build_canceled"))
							{
								return;
							}
						}
						else
						{
							if (!(message == "destroying"))
							{
								return;
							}
							goto IL_6D7;
						}
					}
					else
					{
						if (!(message == "controlling_obj_changed"))
						{
							return;
						}
						goto IL_338;
					}
				}
				else if (num != 1649643086U)
				{
					if (num != 2078899078U)
					{
						if (num != 2162786432U)
						{
							return;
						}
						if (!(message == "build_removed"))
						{
							return;
						}
					}
					else
					{
						if (!(message == "disorder_changed"))
						{
							return;
						}
						if (this.ui_pvFigure != null)
						{
							this.ui_pvFigure.RefreshSettlementType();
							this.ui_pvFigure.RefreshDefField();
							this.ui_pvFigure.Refresh();
						}
						return;
					}
				}
				else
				{
					if (!(message == "finishing"))
					{
						return;
					}
					goto IL_6D7;
				}
				if (this.logic != null && BaseUI.LogicKingdom() == this.logic.GetKingdom())
				{
					DT.Field soundsDef = BaseUI.soundsDef;
					BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("building_demolished", null, "", true, true, true, '.') : null, null);
				}
				return;
				IL_6D7:
				if (this.visibility_idx >= 0)
				{
					VisibilityDetector.Del(this.visibility_idx);
					this.visibility_idx = -1;
				}
				if (this.ui_pvFigure != null)
				{
					global::Common.DestroyObj(this.ui_pvFigure.gameObject);
					this.ui_pvFigure = null;
				}
				this.logic.DelListener(this);
				this.logic = null;
				UnityEngine.Object.DestroyImmediate(base.gameObject);
				return;
			}
			if (num <= 107764623U)
			{
				if (num != 24823524U)
				{
					if (num != 107764623U)
					{
						return;
					}
					if (!(message == "structures_sacked"))
					{
						return;
					}
					PrefabGrid.ChooserContext.RefreshRealmTags(this.logic as Castle);
					this.UpdateFX(false);
					return;
				}
				else
				{
					if (!(message == "structures_changed"))
					{
						return;
					}
					this.UpdateLevel(true, true, false);
					return;
				}
			}
			else if (num != 424646424U)
			{
				if (num != 434548633U)
				{
					if (num != 810626458U)
					{
						return;
					}
					if (!(message == "stance_changed"))
					{
						return;
					}
					this.UpdateOccupiedDecal();
					if (this.ui != null)
					{
						this.ui.RefreshSelection(base.gameObject);
					}
					return;
				}
				else
				{
					if (!(message == "build_started"))
					{
						return;
					}
					if (obj is Logic.Settlement && BaseUI.LogicKingdom() == (obj as Logic.Settlement).GetKingdom())
					{
						if (param != null && param is Building.Def)
						{
							MessageIcon.Create(BaseUI.LogicKingdom(), this.logic as Castle, param as Building.Def, true);
						}
						DT.Field soundsDef2 = BaseUI.soundsDef;
						BaseUI.PlaySoundEvent((soundsDef2 != null) ? soundsDef2.GetString("building_started", null, "", true, true, true, '.') : null, null);
						string key = "BuildStartedTrigger";
						Religion religion = this.logic.GetRealm().religion;
						BackgroundMusic.OnTrigger(key, (religion != null) ? religion.name : null);
					}
					return;
				}
			}
			else
			{
				if (!(message == "governor_changed"))
				{
					return;
				}
				if ((bool)param)
				{
					this.CreateLabel(true, true);
				}
				this.UpdateGovernor();
				return;
			}
		}
		else if (num <= 3226836303U)
		{
			if (num <= 2789345666U)
			{
				if (num != 2203898121U)
				{
					if (num != 2512362562U)
					{
						if (num != 2789345666U)
						{
							return;
						}
						if (!(message == "repair_started"))
						{
							return;
						}
						if (obj is Logic.Settlement && BaseUI.LogicKingdom() == (obj as Logic.Settlement).GetKingdom())
						{
							MessageIcon.Create(this.logic as Castle, true);
						}
						return;
					}
					else
					{
						if (!(message == "build_finished"))
						{
							return;
						}
						if (this.logic != null && BaseUI.LogicKingdom() == this.logic.GetKingdom())
						{
							DT.Field soundsDef3 = BaseUI.soundsDef;
							BaseUI.PlayVoiceEvent((soundsDef3 != null) ? soundsDef3.FindChild("narrator_building_completed", null, true, true, true, '.') : null, null);
							DT.Field soundsDef4 = BaseUI.soundsDef;
							BaseUI.PlaySoundEvent((soundsDef4 != null) ? soundsDef4.GetString("building_completed", null, "", true, true, true, '.') : null, null);
							string key2 = "BuildFinishedTrigger";
							Religion religion2 = this.logic.GetRealm().religion;
							BackgroundMusic.OnTrigger(key2, (religion2 != null) ? religion2.name : null);
						}
						return;
					}
				}
				else
				{
					if (!(message == "keep_auto_control_change"))
					{
						return;
					}
					MessageIcon.Create("KeepControllerChangedAutomaticallyMessage", param as Vars, true, null);
					return;
				}
			}
			else
			{
				if (num != 2790208658U)
				{
					if (num != 2824722882U)
					{
						if (num != 3226836303U)
						{
							return;
						}
						if (!(message == "fortification_changed"))
						{
							return;
						}
					}
					else if (!(message == "level_changed"))
					{
						return;
					}
					this.UpdateLevel(true, true, false);
					return;
				}
				if (!(message == "type_changed"))
				{
					return;
				}
				this.setType = this.logic.type;
				if (this.houses.Count > 0)
				{
					this.UpdateType(this.was_visible);
				}
				return;
			}
		}
		else if (num <= 3780989157U)
		{
			if (num != 3378079542U)
			{
				if (num != 3529413660U)
				{
					if (num != 3780989157U)
					{
						return;
					}
					if (!(message == "kingdom_changed"))
					{
						return;
					}
				}
				else
				{
					if (!(message == "battle_changed"))
					{
						return;
					}
					if (this.ui != null)
					{
						this.ui.RefreshSelection(base.gameObject);
					}
					this.CreateLabel(true, true);
					return;
				}
			}
			else
			{
				if (!(message == "tier_changed"))
				{
					return;
				}
				if (this.logic != null && BaseUI.LogicKingdom() == this.logic.GetKingdom())
				{
					DT.Field soundsDef5 = BaseUI.soundsDef;
					BaseUI.PlaySoundEvent((soundsDef5 != null) ? soundsDef5.GetString("province_expanded", null, "", true, true, true, '.') : null, null);
				}
				return;
			}
		}
		else if (num != 3911294831U)
		{
			if (num != 4108006923U)
			{
				if (num != 4278202659U)
				{
					return;
				}
				if (!(message == "razed"))
				{
					return;
				}
				if (this.setType != "Castle")
				{
					bool razed = (bool)param;
					this.SetRazed(razed);
				}
				return;
			}
			else
			{
				if (!(message == "attrition_shoot"))
				{
					return;
				}
				Logic.Settlement settlement = this.logic;
				Logic.Realm realm = (settlement != null) ? settlement.GetRealm() : null;
				if (realm == null)
				{
					return;
				}
				global::Realm realm2 = realm.visuals as global::Realm;
				if (realm2 == null || realm2.visibility == 0)
				{
					return;
				}
				Logic.Army army = param as Logic.Army;
				if (((army != null) ? army.visuals : null) == null)
				{
					return;
				}
				this.shoot_time = UnityEngine.Time.time;
				if (this.CreateProjectile())
				{
					this.arrow_target = (army.visuals as global::Army).transform.position;
				}
				return;
			}
		}
		else
		{
			if (!(message == "famous_person_changed"))
			{
				return;
			}
			Village village = this.logic as Village;
			this.ui = WorldUI.Get();
			if (village != null && this.ui != null && this.ui.kingdom == village.kingdom_id)
			{
				Vars vars = new Vars(village);
				Logic.Character character = param as Logic.Character;
				if (character != null)
				{
					vars.Set<Logic.Character>("famous_person", character);
					if (village.famous_person != null)
					{
						MessageIcon.Create("FamousPersonSpawned", vars, true, null);
					}
					else
					{
						MessageIcon.Create("FamousPersonLeft", vars, true, null);
					}
				}
			}
			this.CreateLabel(false, true);
			this.UpdateGovernor();
			return;
		}
		IL_338:
		this.UpdateOccupiedDecal();
		this.RemoveFromControlGroup();
		this.UpdateCrest();
		this.CreateLabel(true, true);
		this.UpdatePVFigure();
		if (this.ui != null)
		{
			this.ui.RefreshSelection(base.gameObject);
		}
	}

	// Token: 0x0600149B RID: 5275 RVA: 0x000CEB90 File Offset: 0x000CCD90
	private void SetRazed(bool razed)
	{
		if (razed)
		{
			List<string> list = PrefabGrid.EnumTypes(true);
			for (int i = 0; i < this.houses.Count; i++)
			{
				PrefabGrid component = this.houses[i].GetComponent<PrefabGrid>();
				string text = component.type;
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].IndexOf("Destroyed", StringComparison.OrdinalIgnoreCase) == 0)
					{
						text = list[j];
						break;
					}
				}
				if (!(text == component.type))
				{
					component.type = text;
					component.cur_variant = 0;
					component.Refresh(true, true);
				}
			}
		}
		else
		{
			string housesType = global::Settlement.GetHousesType(this.setType);
			if (housesType == null)
			{
				return;
			}
			for (int k = 0; k < this.houses.Count; k++)
			{
				PrefabGrid component2 = this.houses[k].GetComponent<PrefabGrid>();
				if (!(component2.type == housesType))
				{
					component2.type = housesType;
					component2.Refresh(true, true);
				}
			}
		}
		this.UpdateLevel(true, true, false);
	}

	// Token: 0x0600149C RID: 5276 RVA: 0x000CECA4 File Offset: 0x000CCEA4
	private void SendParameters()
	{
		Logic.Realm realm = this.logic.GetRealm();
		global::Realm realm2 = ((realm != null) ? realm.visuals : null) as global::Realm;
		if (realm2 != null && realm2.visibility == -1)
		{
			realm2.RecalcFow(false);
		}
		bool logic_visible = realm2 != null && realm2.visibility != 0;
		for (int i = 0; i < this.additional_emitters.Length; i++)
		{
			this.additional_emitters[i].logic_visible = logic_visible;
		}
		if (this.sound_emitter != null)
		{
			Castle castle = this.logic as Castle;
			float value = 0f;
			this.sound_emitter.logic_visible = logic_visible;
			if (this.sound_emitter.logic_visible)
			{
				if (this.logic.battle != null && this.logic.battle.stage == Logic.Battle.Stage.Ongoing)
				{
					if (this.logic.battle.type == Logic.Battle.Type.Siege)
					{
						value = 1f;
					}
					else if (this.logic.battle.type == Logic.Battle.Type.BreakSiege)
					{
						value = 2f;
					}
					else if (this.logic.battle.type == Logic.Battle.Type.Assault)
					{
						value = 3f;
					}
					else if (this.logic.battle.is_plunder)
					{
						value = 4f;
					}
				}
				else if (this.logic.razed)
				{
					value = 8f;
				}
				else if (castle != null)
				{
					if (castle.sacked)
					{
						value = 7f;
					}
					else if (castle.structure_build.current_building_def != null)
					{
						value = 5f;
					}
				}
			}
			this.sound_emitter.EventInstance.setParameterByName("SettlementState", value, false);
		}
	}

	// Token: 0x0600149D RID: 5277 RVA: 0x000CEE42 File Offset: 0x000CD042
	public void MarkFroBatch()
	{
		this.batch = true;
	}

	// Token: 0x0600149E RID: 5278 RVA: 0x000CEE4C File Offset: 0x000CD04C
	public void RefreshSelection()
	{
		using (Game.Profile("Settlement.RefreshSelection", false, 0f, null))
		{
			Logic.Realm realm = this.logic.GetRealm();
			if (realm != null && realm.settlements != null)
			{
				for (int i = 0; i < realm.settlements.Count; i++)
				{
					global::Settlement settlement = realm.settlements[i].visuals as global::Settlement;
					if (!settlement.visible && settlement.IsSelected())
					{
						settlement.DestroySelection();
					}
					if ((!settlement.visible || settlement.was_visible) && settlement.visible && settlement.IsSelected())
					{
						settlement.CreateSelection(true);
					}
				}
			}
		}
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x000CEF10 File Offset: 0x000CD110
	public bool ShouldBatch(MeshFilter mf)
	{
		Transform transform = mf.transform;
		while (transform != null && transform != base.transform)
		{
			if (transform.CompareTag("NonStatic"))
			{
				return false;
			}
			transform = transform.parent;
		}
		return true;
	}

	// Token: 0x060014A0 RID: 5280 RVA: 0x0002C53B File Offset: 0x0002A73B
	public bool Batch()
	{
		return true;
	}

	// Token: 0x060014A1 RID: 5281 RVA: 0x000CEF54 File Offset: 0x000CD154
	public string MeshName()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			if (!(transform.name == "_label") && !(transform.name == "Wall"))
			{
				foreach (MeshFilter meshFilter in transform.GetComponentsInChildren<MeshFilter>())
				{
					if (!meshFilter.CompareTag("NonStatic"))
					{
						Mesh mesh = meshFilter.sharedMesh ?? meshFilter.mesh;
						if (!(mesh == null))
						{
							return mesh.name;
						}
					}
				}
			}
		}
		return null;
	}

	// Token: 0x060014A2 RID: 5282 RVA: 0x000CF028 File Offset: 0x000CD228
	private void UpdateProjectile()
	{
		if (this.projectile != null)
		{
			Logic.Settlement settlement = this.logic;
			if (settlement.keep_effects != null)
			{
				float num = (UnityEngine.Time.time - this.shoot_time) / settlement.keep_effects.def.shoot_time;
				if (num >= 0f && num <= 1f && this.visible)
				{
					Vector3 b = Vector3.Lerp(base.transform.position, this.arrow_target, 0.5f) + Vector3.up * settlement.keep_effects.def.shoot_height;
					Vector3 vector = Vector3.Lerp(Vector3.Lerp(base.transform.position, b, num), this.arrow_target, num);
					if (num > 0f && UnityEngine.Time.timeScale != 0f)
					{
						Vector3 vector2 = vector - this.projectile.transform.position;
						if (vector2 != Vector3.zero)
						{
							this.projectile.transform.rotation = Quaternion.LookRotation(vector2);
						}
					}
					this.projectile.transform.position = vector;
					return;
				}
				ParticleSystem[] componentsInChildren = this.projectile.GetComponentsInChildren<ParticleSystem>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].subEmitters.enabled = false;
					componentsInChildren[i].Stop();
				}
			}
		}
	}

	// Token: 0x060014A3 RID: 5283 RVA: 0x000CF194 File Offset: 0x000CD394
	private void Update()
	{
		if (this.m_InvalidateOccupiedDecal)
		{
			this.UpdateOccupiedDecal();
		}
		this.UpdateProjectile();
		if (!this.was_visible)
		{
			return;
		}
		if (this.visible && this.level_dirty)
		{
			this.UpdateLevel(true, true, true);
		}
		ResourceBar.Refresh(this);
		if (this.batch)
		{
			this.Batch();
			this.batch = false;
		}
		this.SendParameters();
	}

	// Token: 0x060014A4 RID: 5284 RVA: 0x000CF1FC File Offset: 0x000CD3FC
	public string GetLocalziedCastle()
	{
		if (!this.IsCastle())
		{
			return string.Empty;
		}
		if (this.localizedCastleName == null)
		{
			this.localizedCastleName = global::Defs.Localize(this.logic.GetNameKey(null, ""), this.logic, null, true, true);
		}
		return this.localizedCastleName;
	}

	// Token: 0x04000D14 RID: 3348
	public const bool SHOW_GOVERNOR = false;

	// Token: 0x04000D15 RID: 3349
	[Space]
	public string Name = "";

	// Token: 0x04000D16 RID: 3350
	public string TownName = "";

	// Token: 0x04000D17 RID: 3351
	[Space]
	public GameObject NamePrefab;

	// Token: 0x04000D18 RID: 3352
	public GameObject ShieldPrefab;

	// Token: 0x04000D19 RID: 3353
	public GameObject OccupiedDecalPrefab;

	// Token: 0x04000D1A RID: 3354
	[HideInInspector]
	public string setType;

	// Token: 0x04000D1B RID: 3355
	[NonSerialized]
	public string curType;

	// Token: 0x04000D1C RID: 3356
	[HideInInspector]
	public string citadel_architecture = "";

	// Token: 0x04000D1D RID: 3357
	[HideInInspector]
	public string houses_architecture = "";

	// Token: 0x04000D1E RID: 3358
	[NonSerialized]
	public Transform name_label;

	// Token: 0x04000D1F RID: 3359
	public Material resourceMaterial;

	// Token: 0x04000D20 RID: 3360
	public GameObject AudioPrefab;

	// Token: 0x04000D21 RID: 3361
	public GameObject SmokePrefab;

	// Token: 0x04000D22 RID: 3362
	public GameObject ArrowsPrefab;

	// Token: 0x04000D23 RID: 3363
	public GameObject CannonBallsPrefab;

	// Token: 0x04000D24 RID: 3364
	public MeshRenderer OccupiedDecal;

	// Token: 0x04000D25 RID: 3365
	[HideInInspector]
	public GameObject goldIncomeLabel;

	// Token: 0x04000D26 RID: 3366
	public int level;

	// Token: 0x04000D27 RID: 3367
	public bool fix_level;

	// Token: 0x04000D28 RID: 3368
	[HideInInspector]
	public int GrowRealmBorders;

	// Token: 0x04000D29 RID: 3369
	[NonSerialized]
	public bool visible;

	// Token: 0x04000D2A RID: 3370
	[NonSerialized]
	public bool was_visible;

	// Token: 0x04000D2B RID: 3371
	[NonSerialized]
	public int defs_version;

	// Token: 0x04000D2C RID: 3372
	private bool registered;

	// Token: 0x04000D2D RID: 3373
	private static global::Settlement first = null;

	// Token: 0x04000D2E RID: 3374
	private static global::Settlement last = null;

	// Token: 0x04000D2F RID: 3375
	private global::Settlement prev;

	// Token: 0x04000D30 RID: 3376
	private global::Settlement next;

	// Token: 0x04000D31 RID: 3377
	private GameObject audio;

	// Token: 0x04000D32 RID: 3378
	private StudioEventEmitter sound_emitter;

	// Token: 0x04000D33 RID: 3379
	private StudioEventEmitter[] additional_emitters;

	// Token: 0x04000D34 RID: 3380
	public UIPVFigureSettlement ui_pvFigure;

	// Token: 0x04000D35 RID: 3381
	private GameObject smoke;

	// Token: 0x04000D36 RID: 3382
	private GameObject projectile;

	// Token: 0x04000D37 RID: 3383
	private float shoot_time;

	// Token: 0x04000D38 RID: 3384
	private Vector3 arrow_target;

	// Token: 0x04000D39 RID: 3385
	private bool cannons;

	// Token: 0x04000D3A RID: 3386
	private bool m_InvalidateSelection;

	// Token: 0x04000D3B RID: 3387
	private bool m_InvalidateOccupiedDecal;

	// Token: 0x04000D3C RID: 3388
	private global::Settlement.RefreshMode refresh;

	// Token: 0x04000D3D RID: 3389
	[NonSerialized]
	public List<ResourceBar.IconInfo> res_icons;

	// Token: 0x04000D3E RID: 3390
	[NonSerialized]
	public List<PrefabGrid> houses = new List<PrefabGrid>();

	// Token: 0x04000D3F RID: 3391
	[NonSerialized]
	public PrefabGrid citadel;

	// Token: 0x04000D40 RID: 3392
	[NonSerialized]
	public Wall wall;

	// Token: 0x04000D41 RID: 3393
	[NonSerialized]
	public Transform details_transform;

	// Token: 0x04000D42 RID: 3394
	[NonSerialized]
	public List<PrefabGrid> details = new List<PrefabGrid>();

	// Token: 0x04000D43 RID: 3395
	[SerializeField]
	[HideInInspector]
	private Bounds aabb;

	// Token: 0x04000D44 RID: 3396
	private bool level_dirty;

	// Token: 0x04000D45 RID: 3397
	private static bool hide_labels = false;

	// Token: 0x04000D46 RID: 3398
	private WorldUI ui;

	// Token: 0x04000D47 RID: 3399
	private bool selected;

	// Token: 0x04000D48 RID: 3400
	private bool primarySelection = true;

	// Token: 0x04000D49 RID: 3401
	private MeshRenderer selection;

	// Token: 0x04000D4A RID: 3402
	private MeshRenderer shoot_range_selection;

	// Token: 0x04000D4B RID: 3403
	private int selection_vertices = 32;

	// Token: 0x04000D4C RID: 3404
	private static List<Vector3> tmp_selection_points = new List<Vector3>();

	// Token: 0x04000D4D RID: 3405
	private Collider picker_collider;

	// Token: 0x04000D4E RID: 3406
	private Material currentMaterial;

	// Token: 0x04000D4F RID: 3407
	private static List<Renderer> s_renderers = new List<Renderer>(128);

	// Token: 0x04000D50 RID: 3408
	public Logic.Settlement logic;

	// Token: 0x04000D51 RID: 3409
	public int visibility_idx = -1;

	// Token: 0x04000D52 RID: 3410
	private static DT.Field dummy_house_field = null;

	// Token: 0x04000D53 RID: 3411
	private bool batch;

	// Token: 0x04000D54 RID: 3412
	private string localizedCastleName;

	// Token: 0x020006AA RID: 1706
	public enum SettlementState
	{
		// Token: 0x04003649 RID: 13897
		Default,
		// Token: 0x0400364A RID: 13898
		Siege,
		// Token: 0x0400364B RID: 13899
		BreakSiege,
		// Token: 0x0400364C RID: 13900
		Assault,
		// Token: 0x0400364D RID: 13901
		Plunder,
		// Token: 0x0400364E RID: 13902
		Building,
		// Token: 0x0400364F RID: 13903
		Repairing,
		// Token: 0x04003650 RID: 13904
		Sacked,
		// Token: 0x04003651 RID: 13905
		Razed
	}

	// Token: 0x020006AB RID: 1707
	public enum RefreshMode
	{
		// Token: 0x04003653 RID: 13907
		None,
		// Token: 0x04003654 RID: 13908
		Spawn,
		// Token: 0x04003655 RID: 13909
		Label,
		// Token: 0x04003656 RID: 13910
		Respawn,
		// Token: 0x04003657 RID: 13911
		Randomize,
		// Token: 0x04003658 RID: 13912
		FullRandomize
	}
}

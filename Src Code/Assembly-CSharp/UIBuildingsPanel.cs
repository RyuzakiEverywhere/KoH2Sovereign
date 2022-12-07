using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002A5 RID: 677
public class UIBuildingsPanel : MonoBehaviour
{
	// Token: 0x1700020E RID: 526
	// (get) Token: 0x06002A65 RID: 10853 RVA: 0x00161BCA File Offset: 0x0015FDCA
	public Game game
	{
		get
		{
			return GameLogic.Get(false);
		}
	}

	// Token: 0x06002A66 RID: 10854 RVA: 0x00168198 File Offset: 0x00166398
	public void Init(District.Def def, Logic.Kingdom kingdom, Castle castle)
	{
		this.def = def;
		this.kingdom = (kingdom ?? ((castle != null) ? castle.GetKingdom() : null));
		this.castle = castle;
		this.defs_version = global::Defs.Version;
		Vector2 vector = this.SpawnBuildings();
		this.SpawnArrows();
		LayoutElement component = base.GetComponent<LayoutElement>();
		if (component != null)
		{
			component.preferredWidth = vector.x;
			component.preferredHeight = vector.y;
			return;
		}
		RectTransform component2 = base.GetComponent<RectTransform>();
		component2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, vector.x);
		component2.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, vector.y);
	}

	// Token: 0x06002A67 RID: 10855 RVA: 0x0016822C File Offset: 0x0016642C
	private Vector2 SpawnBuildings()
	{
		Transform transform = base.transform.Find("buildings");
		RectTransform rectTransform = (transform != null) ? transform.GetComponent<RectTransform>() : null;
		if (rectTransform == null)
		{
			rectTransform = new GameObject("buildings")
			{
				hideFlags = HideFlags.DontSave
			}.AddComponent<RectTransform>();
			rectTransform.SetParent(base.transform, false);
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
		}
		else
		{
			UICommon.DeleteChildren(rectTransform);
		}
		District.Def def = this.def;
		if (((def != null) ? def.buildings : null) == null || this.def.buildings.Count <= 0)
		{
			return Vector2.zero;
		}
		bool flag = this.def.IsCommon() || this.def.IsPF();
		Vector2 result = this.def.panel_size;
		rectTransform.gameObject.SetActive(false);
		int num = 0;
		for (int i = 0; i < this.def.buildings.Count; i++)
		{
			District.Def.BuildingInfo buildingInfo = this.def.buildings[i];
			if (!flag || this.castle == null || this.castle.MayBuildBuilding(buildingInfo.def, true))
			{
				UIBuildingSlot uibuildingSlot = UIBuildingSlot.Create(buildingInfo.def, this.kingdom, this.castle, this.def, rectTransform, null);
				if (!(uibuildingSlot == null))
				{
					if (uibuildingSlot != null)
					{
						uibuildingSlot.OnSelected += this.HandleSlotClick;
					}
					GameObject gameObject = uibuildingSlot.gameObject;
					gameObject.name = buildingInfo.id;
					gameObject.hideFlags = HideFlags.DontSave;
					RectTransform component = gameObject.GetComponent<RectTransform>();
					if (!(component == null))
					{
						if (this.kingdom != null && !this.kingdom.CheckReligionRequirements(buildingInfo.def))
						{
							gameObject.SetActive(false);
						}
						else if (!flag)
						{
							component.anchorMin = buildingInfo.anchors_min;
							component.anchorMax = buildingInfo.anchors_max;
							component.offsetMin = Vector2.zero;
							component.offsetMax = Vector2.zero;
						}
						else
						{
							int num2 = (this.MaxColumns > 0) ? (num / this.MaxColumns) : 0;
							int num3 = (this.MaxColumns > 0) ? (num % this.MaxColumns) : num;
							component.pivot = Vector2.zero;
							component.anchorMin = Vector2.zero;
							component.anchorMax = Vector2.zero;
							component.offsetMin = new Vector2((float)num3 * this.def.grid_cell_size.x + this.def.icon_spacing.x / 2f, (float)num2 * this.def.grid_cell_size.y + this.def.icon_spacing.y / 2f);
							component.offsetMax = component.offsetMin + this.def.icon_size;
							num++;
						}
					}
				}
			}
		}
		if (flag)
		{
			if (num > 0)
			{
				num--;
			}
			int num4 = (this.MaxColumns > 0) ? (num / this.MaxColumns) : 0;
			int num5 = (num4 > 0) ? this.MaxColumns : num;
			result.x = (float)(num5 + 1) * this.def.grid_cell_size.x;
			result.y = (float)(num4 + 1) * this.def.grid_cell_size.y;
		}
		rectTransform.gameObject.SetActive(true);
		return result;
	}

	// Token: 0x06002A68 RID: 10856 RVA: 0x001685C0 File Offset: 0x001667C0
	public int GetRelevatSettlementCount()
	{
		if (this.def == null)
		{
			return 0;
		}
		DT.Field field = this.def.field.FindChild("caption_count", null, true, true, true, '.');
		if (field == null)
		{
			return 0;
		}
		if (this.castle != null)
		{
			DT.Field field2 = field;
			Game game = this.game;
			return field2.Int((game != null) ? game.GetRealm(this.castle.realm_id) : null, 0);
		}
		if (this.kingdom == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < this.kingdom.realms.Count; i++)
		{
			Logic.Realm vars = this.kingdom.realms[i];
			int num2 = field.Int(vars, 0);
			num += num2;
		}
		return num;
	}

	// Token: 0x06002A69 RID: 10857 RVA: 0x00168670 File Offset: 0x00166870
	private void SpawnArrows()
	{
		Transform transform = base.transform.Find("arrows");
		if (transform == null)
		{
			GameObject gameObject = new GameObject("arrows");
			gameObject.hideFlags = HideFlags.DontSave;
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			transform = gameObject.transform;
			transform.SetParent(base.transform, false);
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
		}
		else
		{
			UICommon.DeleteChildren(transform);
		}
		District.Def def = this.def;
		if (((def != null) ? def.arrows : null) == null || this.def.arrows.Count <= 0)
		{
			return;
		}
		Vars vars = null;
		transform.gameObject.SetActive(false);
		int i = 0;
		while (i < this.def.arrows.Count)
		{
			District.Def.ArrowInfo arrowInfo = this.def.arrows[i];
			if (arrowInfo.condition_field == null)
			{
				goto IL_145;
			}
			if (vars == null)
			{
				vars = new Vars(this.def);
				vars.Set<Castle>("castle", this.castle);
				Vars vars2 = vars;
				string key = "realm";
				Castle castle = this.castle;
				vars2.Set<Logic.Realm>(key, (castle != null) ? castle.GetRealm() : null);
				Vars vars3 = vars;
				string key2 = "kingdom";
				Castle castle2 = this.castle;
				vars3.Set<Logic.Kingdom>(key2, (castle2 != null) ? castle2.GetKingdom() : null);
			}
			if (arrowInfo.condition_field.Bool(vars, false))
			{
				goto IL_145;
			}
			IL_1F4:
			i++;
			continue;
			IL_145:
			GameObject obj = global::Defs.GetObj<GameObject>(arrowInfo.prefab_field, null);
			if (obj == null)
			{
				goto IL_1F4;
			}
			GameObject gameObject2 = global::Common.Spawn(obj, false, false);
			if (gameObject2 == null)
			{
				break;
			}
			gameObject2.name = arrowInfo.field.key;
			gameObject2.hideFlags = HideFlags.DontSave;
			gameObject2.transform.SetParent(transform, false);
			RectTransform component = gameObject2.GetComponent<RectTransform>();
			if (!(component == null))
			{
				component.anchorMin = arrowInfo.anchors_min;
				component.anchorMax = arrowInfo.anchors_max;
				component.offsetMin = Vector2.zero;
				component.offsetMax = Vector2.zero;
				goto IL_1F4;
			}
			goto IL_1F4;
		}
		transform.gameObject.SetActive(true);
	}

	// Token: 0x06002A6A RID: 10858 RVA: 0x00168897 File Offset: 0x00166A97
	public bool IsEmpty()
	{
		District.Def def = this.def;
		return ((def != null) ? def.buildings : null) == null || this.def.buildings.Count == 0;
	}

	// Token: 0x06002A6B RID: 10859 RVA: 0x001688C4 File Offset: 0x00166AC4
	private void HandleSlotClick(UIBuildingSlot slot, PointerEventData e)
	{
		Castle castle = ((slot != null) ? slot.Castle : null) ?? this.castle;
		if (UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "cheatmode building slot", true))
		{
			if (castle != null)
			{
				castle.BuildBuilding(slot.Def, -1, true);
			}
			return;
		}
		if (this.OnSelected != null)
		{
			this.OnSelected(slot, e);
			return;
		}
		int slot_idx = UICastleBuildWindow.TargetSlotIndex();
		if (castle != null)
		{
			castle.BuildBuilding(slot.Def, slot_idx, false);
		}
	}

	// Token: 0x06002A6C RID: 10860 RVA: 0x00168944 File Offset: 0x00166B44
	private void OnEnable()
	{
		if (base.GetComponents<TooltipBlocker>() != null)
		{
			TooltipPlacement.AddBlocker(base.gameObject, base.transform);
		}
	}

	// Token: 0x06002A6D RID: 10861 RVA: 0x0016895F File Offset: 0x00166B5F
	private void OnDisable()
	{
		if (base.GetComponents<TooltipBlocker>() != null)
		{
			TooltipPlacement.DelBlocker(base.gameObject);
		}
	}

	// Token: 0x04001CBB RID: 7355
	public District.Def def;

	// Token: 0x04001CBC RID: 7356
	public Logic.Kingdom kingdom;

	// Token: 0x04001CBD RID: 7357
	public Castle castle;

	// Token: 0x04001CBE RID: 7358
	public Action<UIBuildingSlot, PointerEventData> OnSelected;

	// Token: 0x04001CBF RID: 7359
	public string preview_def_id = "TestDistrict1";

	// Token: 0x04001CC0 RID: 7360
	public int MaxColumns = 6;

	// Token: 0x04001CC1 RID: 7361
	private int defs_version;
}

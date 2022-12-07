using System;
using System.Collections.Generic;
using FMODUnity;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001B6 RID: 438
public class UIBattleWindow : MonoBehaviour, IListener, BaseUI.ISelectionPanel
{
	// Token: 0x17000164 RID: 356
	// (get) Token: 0x060019E1 RID: 6625 RVA: 0x000FB309 File Offset: 0x000F9509
	// (set) Token: 0x060019E2 RID: 6626 RVA: 0x000FB310 File Offset: 0x000F9510
	public static Camera troops_camera { get; private set; }

	// Token: 0x060019E3 RID: 6627 RVA: 0x000FB318 File Offset: 0x000F9518
	private void SetupTroopsCamera()
	{
		if (UIBattleWindow.troops_container == null)
		{
			UIBattleWindow.troops_render_texture = new RenderTexture(this.render_tex_width, this.render_tex_height, 0, RenderTextureFormat.ARGB32);
			UIBattleWindow.troops_container = new GameObject("RenderTroopsContainer").transform;
			UIBattleWindow.troops_container.position = new Vector3(0f, -200f, 0f);
			UIBattleWindow.troops_container.eulerAngles = new Vector3(0f, 180f, 0f);
			GameObject gameObject = global::Common.SpawnTemplate("TroopsCamera", "TroopsCamera", UIBattleWindow.troops_container, true, new Type[]
			{
				typeof(Camera)
			});
			UIBattleWindow.troops_camera = gameObject.GetComponent<Camera>();
			UIBattleWindow.troops_camera.targetTexture = UIBattleWindow.troops_render_texture;
			UIBattleWindow.troops_camera.clearFlags = CameraClearFlags.Color;
			UIBattleWindow.troops_camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
			UIBattleWindow.troops_camera.cullingMask = LayerMask.GetMask(new string[]
			{
				"Armies"
			});
			gameObject.transform.localPosition = this.cam_position;
			gameObject.transform.localEulerAngles = this.cam_rotation;
			UIBattleWindow.troops_camera.fieldOfView = this.cam_fov;
			UIBattleWindow.troops_camera.renderingPath = RenderingPath.Forward;
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.shadow_plane_prefab, UIBattleWindow.troops_container);
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localScale = Vector3.one;
			gameObject2.transform.localRotation = Quaternion.identity;
			UnityEngine.Object.DontDestroyOnLoad(UIBattleWindow.troops_container.gameObject);
		}
		bool active = this.logic.type != Logic.Battle.Type.Naval;
		UIBattleWindow.troops_container.gameObject.SetActive(active);
	}

	// Token: 0x060019E4 RID: 6628 RVA: 0x000FB4D8 File Offset: 0x000F96D8
	private void OnDisable()
	{
		this.ClearTroops();
		if (UIBattleWindow.troops_container != null)
		{
			UIBattleWindow.troops_container.gameObject.SetActive(false);
		}
	}

	// Token: 0x060019E5 RID: 6629 RVA: 0x000FB4FD File Offset: 0x000F96FD
	public static void SetEstimationTooltip(GameObject go, Logic.Battle logic)
	{
		Tooltip.Get(go, true).SetDef("BattleEstimationTooltip", new Vars(logic));
	}

	// Token: 0x060019E6 RID: 6630 RVA: 0x000FB51C File Offset: 0x000F971C
	private void AddTroop(Logic.Unit unit, int side)
	{
		if (((unit != null) ? unit.simulation : null) == null)
		{
			return;
		}
		string text = unit.def.field.key;
		if (unit.simulation.state == BattleSimulation.Squad.State.Fled)
		{
			text = "surrendered";
		}
		UIBattleWindow.TroopModels troopModels;
		if (!UIBattleWindow.troops.TryGetValue(text, out troopModels))
		{
			troopModels = default(UIBattleWindow.TroopModels);
			troopModels.models = new List<UnitColliderContainer>();
			troopModels.occupied = 0;
			troopModels.def = unit.def;
			UIBattleWindow.troops[text] = troopModels;
		}
		for (int i = 0; i < this.troops_per_unit; i++)
		{
			global::Unit unit2 = new global::Unit(WorldMap.Get().texture_baker_ui, UIBattleWindow.troops_container);
			unit2.can_move = false;
			if (text != "surrendered")
			{
				unit2.onFlee = new global::Unit.OnFlee(this.ForceRefreshTroops);
			}
			else
			{
				unit2.onFlee = null;
			}
			unit2.SetLogic(unit);
			unit2.Enable(true, false);
			UnitColliderContainer item = UnitColliderContainer.Create(unit2, UIBattleWindow.troops_container);
			troopModels.models.Add(item);
			troopModels.occupied++;
			if (side == 0)
			{
				troopModels.left++;
			}
			else
			{
				troopModels.right++;
			}
		}
		UIBattleWindow.troops[text] = troopModels;
	}

	// Token: 0x060019E7 RID: 6631 RVA: 0x000FB65C File Offset: 0x000F985C
	public void ForceRefreshTroops()
	{
		UIBattleWindow.refresh_troops = true;
	}

	// Token: 0x060019E8 RID: 6632 RVA: 0x000FB664 File Offset: 0x000F9864
	private void SetupTroops()
	{
		Random.State state = Random.state;
		Random.InitState((int)(this.logic.position.x + this.logic.position.y));
		this.ClearTroops();
		if (this.logic.type == Logic.Battle.Type.Naval)
		{
			return;
		}
		int[] array = new int[2];
		int[] array2 = new int[2];
		global::Battle.CalcSides(this.logic, out array2[0], out array2[1], false);
		for (int i = 0; i < 2; i++)
		{
			List<BattleSimulation.Squad> squads = this.logic.simulation.GetSquads(array2[i]);
			for (int j = 0; j < squads.Count; j++)
			{
				BattleSimulation.Squad squad = squads[j];
				this.AddTroop(squad.unit, i);
				array[i]++;
			}
		}
		float[] array3 = new float[]
		{
			this.dist_between_sides,
			this.dist_between_sides
		};
		float[] array4 = new float[2];
		this.PositionModels(array3, array4, 0.8f, new Logic.Unit.Type[]
		{
			Logic.Unit.Type.Cavalry
		});
		float[] idx = array3;
		float[] max_dist = array4;
		float dist_between_units = 0.5f;
		Logic.Unit.Type[] array5 = new Logic.Unit.Type[3];
		array5[0] = Logic.Unit.Type.Defense;
		array5[1] = Logic.Unit.Type.Infantry;
		this.PositionModels(idx, max_dist, dist_between_units, array5);
		this.PositionModels(array3, array4, 0.5f, new Logic.Unit.Type[]
		{
			Logic.Unit.Type.Ranged
		});
		this.PositionModels(array3, array4, 0.8f, new Logic.Unit.Type[]
		{
			Logic.Unit.Type.Noble
		});
		this.PositionSurrenderModels(array3, array4, 0.5f);
		this.ScalePositions(array4);
		Random.state = state;
	}

	// Token: 0x060019E9 RID: 6633 RVA: 0x000FB7E0 File Offset: 0x000F99E0
	private void ScalePositions(float[] max_dist)
	{
		foreach (KeyValuePair<string, UIBattleWindow.TroopModels> keyValuePair in UIBattleWindow.troops)
		{
			UIBattleWindow.TroopModels value = keyValuePair.Value;
			if (value.occupied != 0)
			{
				for (int i = 0; i < value.left; i++)
				{
					UnitColliderContainer unitColliderContainer = value.models[i];
					Vector3 localPosition = unitColliderContainer.transform.localPosition;
					localPosition.x *= this.max_space_for_troops / max_dist[0];
					unitColliderContainer.SetPosition(localPosition);
				}
				for (int j = value.left; j < value.left + value.right; j++)
				{
					UnitColliderContainer unitColliderContainer2 = value.models[j];
					Vector3 localPosition2 = unitColliderContainer2.transform.localPosition;
					localPosition2.x *= this.max_space_for_troops / max_dist[1];
					unitColliderContainer2.SetPosition(localPosition2);
				}
			}
		}
	}

	// Token: 0x060019EA RID: 6634 RVA: 0x000FB8E4 File Offset: 0x000F9AE4
	private void PositionModels(float[] idx, float[] max_dist, float dist_between_units, params Logic.Unit.Type[] types)
	{
		int[] array = new int[2];
		foreach (KeyValuePair<string, UIBattleWindow.TroopModels> keyValuePair in UIBattleWindow.troops)
		{
			UIBattleWindow.TroopModels value = keyValuePair.Value;
			if (value.occupied != 0)
			{
				Logic.Unit.Def def = value.def;
				bool flag = false;
				for (int i = 0; i < types.Length; i++)
				{
					if (types[i] == def.type)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					this.PositionModels(def, value, 0, idx, max_dist, dist_between_units, array);
					this.PositionModels(def, value, 1, idx, max_dist, dist_between_units, array);
				}
			}
		}
		if (array[0] != 0)
		{
			idx[0] += this.dist_between_rows;
		}
		if (array[1] != 0)
		{
			idx[1] += this.dist_between_rows;
		}
	}

	// Token: 0x060019EB RID: 6635 RVA: 0x000FB9C4 File Offset: 0x000F9BC4
	private void PositionSurrenderModels(float[] idx, float[] max_dist, float dist_between_units)
	{
		int[] array = new int[2];
		UIBattleWindow.TroopModels troopModels;
		if (!UIBattleWindow.troops.TryGetValue("surrendered", out troopModels))
		{
			return;
		}
		if (troopModels.occupied == 0)
		{
			return;
		}
		Logic.Unit.Def def = troopModels.def;
		this.PositionModels(def, troopModels, 0, idx, max_dist, dist_between_units, array);
		this.PositionModels(def, troopModels, 1, idx, max_dist, dist_between_units, array);
		if (array[0] != 0)
		{
			idx[0] += this.dist_between_rows;
		}
		if (array[1] != 0)
		{
			idx[1] += this.dist_between_rows;
		}
	}

	// Token: 0x060019EC RID: 6636 RVA: 0x000FBA44 File Offset: 0x000F9C44
	private void PositionModels(Logic.Unit.Def def, UIBattleWindow.TroopModels info, int side, float[] idx, float[] max_dist, float dist_between_units, int[] cnt)
	{
		int num = 0;
		int num2 = info.left;
		if (side == 1)
		{
			num = info.left;
			num2 = info.left + info.right;
		}
		int num3 = -(1 - side * 2);
		for (int i = num; i < num2; i++)
		{
			UnitColliderContainer unitColliderContainer = info.models[i];
			float x = idx[side] * (float)num3 + Random.Range(-dist_between_units, dist_between_units);
			unitColliderContainer.SetPosition(new Vector3(x, 0f, (float)(this.max_units_per_row - cnt[side]) * dist_between_units));
			cnt[side]++;
			unitColliderContainer.SetRotation(new Vector3(0f, (float)(-90 * num3), 0f));
			float num4 = Mathf.Abs(unitColliderContainer.transform.localPosition.x);
			if (num4 > max_dist[side])
			{
				max_dist[side] = num4;
			}
			idx[side] += dist_between_units;
			if (cnt[side] > this.max_units_per_row)
			{
				cnt[side] = 0;
				idx[side] += this.dist_between_rows;
			}
		}
	}

	// Token: 0x060019ED RID: 6637 RVA: 0x000FBB50 File Offset: 0x000F9D50
	private void Awake()
	{
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Preparation", true, true);
		if (gameObject != null)
		{
			this.preparation = gameObject.AddComponent<UIBattleWindow.Preparation>();
		}
		GameObject gameObject2 = global::Common.FindChildByName(base.gameObject, "id_Ongoing", true, true);
		if (gameObject2 != null)
		{
			this.ongoing = gameObject2.AddComponent<UIBattleWindow.Ongoing>();
		}
		GameObject gameObject3 = global::Common.FindChildByName(base.gameObject, "id_Outcome", true, true);
		if (gameObject3 != null)
		{
			this.outcome = gameObject3.AddComponent<UIBattleWindow.Outcome>();
		}
		GameObject gameObject4 = global::Common.FindChildByName(base.gameObject, "id_Siege", true, true);
		if (gameObject4 != null)
		{
			this.siege = gameObject4.AddComponent<UIBattleWindow.Siege>();
		}
	}

	// Token: 0x060019EE RID: 6638 RVA: 0x000FBC00 File Offset: 0x000F9E00
	private void ClearTroops()
	{
		foreach (KeyValuePair<string, UIBattleWindow.TroopModels> keyValuePair in UIBattleWindow.troops)
		{
			for (int i = 0; i < keyValuePair.Value.models.Count; i++)
			{
				UnitColliderContainer unitColliderContainer = keyValuePair.Value.models[i];
				if (!(unitColliderContainer == null))
				{
					unitColliderContainer.unit.Enable(false, false);
					global::Common.DestroyObj(unitColliderContainer.gameObject);
				}
			}
		}
		WorldMap worldMap = WorldMap.Get();
		if (((worldMap != null) ? worldMap.texture_baker_ui : null) != null)
		{
			worldMap.texture_baker_ui.ClearSkinningBuffers();
		}
		UIBattleWindow.troops = new Dictionary<string, UIBattleWindow.TroopModels>();
	}

	// Token: 0x060019EF RID: 6639 RVA: 0x000FBCC8 File Offset: 0x000F9EC8
	private void OnDestroy()
	{
		this.RemoveListeners();
		this.logic = null;
	}

	// Token: 0x060019F0 RID: 6640 RVA: 0x000FBCD8 File Offset: 0x000F9ED8
	private void Start()
	{
		this.RemoveListeners();
		this.logic = this.GetBatlleFromMessageWnd();
		if (this.logic == null)
		{
			this.logic = this.GetBattleFromSelection();
		}
		if (this.logic == null)
		{
			return;
		}
		this.SetupTroopsCamera();
		this.SetupTroops();
		this.DecideState();
		if (this.currentState == null)
		{
			return;
		}
		this.AddListeners();
	}

	// Token: 0x060019F1 RID: 6641 RVA: 0x000FBD3C File Offset: 0x000F9F3C
	private Logic.Battle GetBatlleFromMessageWnd()
	{
		MessageWnd component = base.GetComponent<MessageWnd>();
		if (component == null || component.vars == null)
		{
			return null;
		}
		return component.vars.Get<Logic.Battle>("battle", null);
	}

	// Token: 0x060019F2 RID: 6642 RVA: 0x000FBD74 File Offset: 0x000F9F74
	private Logic.Battle GetBattleFromSelection()
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null || worldUI.selected_obj == null)
		{
			return null;
		}
		this.battle = worldUI.selected_obj.GetComponent<global::Battle>();
		if (this.battle == null || this.battle.logic == null)
		{
			return null;
		}
		return this.battle.logic;
	}

	// Token: 0x060019F3 RID: 6643 RVA: 0x000FBDDC File Offset: 0x000F9FDC
	private void DecideState()
	{
		if (this.logic == null)
		{
			Debug.Log("Battle logic data not avalilabe! FallBack?");
			return;
		}
		if ((this.logic.stage == Logic.Battle.Stage.Preparing || this.logic.stage == Logic.Battle.Stage.EnteringBattle) && UIBattleWindow.PlayerHasControl(this.battle.logic))
		{
			this.currentState = this.preparation;
		}
		else if (this.logic.IsFinishing() && UIBattleWindow.PlayerInvolved(this.battle.logic))
		{
			this.currentState = this.outcome;
		}
		else if (this.logic.type == Logic.Battle.Type.Siege)
		{
			this.currentState = this.siege;
		}
		else
		{
			this.currentState = this.ongoing;
		}
		this.currentState.Activate(true);
		this.currentState.SetBattle(this.logic, null);
		this.preparation.Activate(this.preparation == this.currentState);
		this.outcome.Activate(this.outcome == this.currentState);
		this.siege.Activate(this.siege == this.currentState);
		this.ongoing.Activate(this.ongoing == this.currentState);
	}

	// Token: 0x060019F4 RID: 6644 RVA: 0x000FBF1C File Offset: 0x000FA11C
	private void Update()
	{
		if (this.totals_dirty)
		{
			this.totals_dirty = false;
			global::Battle battle = this.battle;
			bool flag;
			if (battle == null)
			{
				flag = (null != null);
			}
			else
			{
				Logic.Battle battle2 = battle.logic;
				flag = (((battle2 != null) ? battle2.simulation : null) != null);
			}
			if (!flag)
			{
				return;
			}
			this.battle.logic.simulation.CalcTotals(false, false);
		}
		if (UIBattleWindow.refresh_troops)
		{
			UIBattleWindow.refresh_troops = false;
			this.SetupTroops();
		}
		foreach (KeyValuePair<string, UIBattleWindow.TroopModels> keyValuePair in UIBattleWindow.troops)
		{
			for (int i = 0; i < keyValuePair.Value.models.Count; i++)
			{
				keyValuePair.Value.models[i].unit.Update();
			}
		}
		this.currentState.UpdateUnitTooltip();
	}

	// Token: 0x060019F5 RID: 6645 RVA: 0x000023FD File Offset: 0x000005FD
	public void Refresh()
	{
	}

	// Token: 0x060019F6 RID: 6646 RVA: 0x000FC004 File Offset: 0x000FA204
	private void AddListeners()
	{
		if (this.logic != null)
		{
			this.logic.AddListener(this);
		}
	}

	// Token: 0x060019F7 RID: 6647 RVA: 0x000FC01A File Offset: 0x000FA21A
	private void RemoveListeners()
	{
		if (this.logic != null)
		{
			this.logic.DelListener(this);
		}
	}

	// Token: 0x060019F8 RID: 6648 RVA: 0x000FC030 File Offset: 0x000FA230
	public void Close()
	{
		MessageWnd component = base.GetComponent<MessageWnd>();
		if (component != null)
		{
			component.CloseAndDismiss(true);
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null && this.battle != null && worldUI.IsSelected(this.battle.gameObject))
		{
			worldUI.SelectObj(null, false, true, true, true);
			return;
		}
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x060019F9 RID: 6649 RVA: 0x000FC09C File Offset: 0x000FA29C
	public void OnMessage(object obj, string message, object param)
	{
		if (this.IsDestoryed())
		{
			Logic.Battle battle = obj as Logic.Battle;
			if (battle == null)
			{
				return;
			}
			battle.DelListener(this);
			return;
		}
		else if (message == "changed")
		{
			this.totals_dirty = this.battle.logic.simulation.totals_dirty;
			UIBattleWindow.BattleStage battleStage = this.currentState;
			if (battleStage == null)
			{
				return;
			}
			battleStage.UpdateState();
			return;
		}
		else
		{
			if (message == "reinforcements_changed")
			{
				UIBattleWindow.BattleStage battleStage2 = this.currentState;
				if (battleStage2 != null)
				{
					battleStage2.OnArmiesChanged();
				}
			}
			if (message == "armies_changed")
			{
				this.DecideState();
				this.SetupTroops();
				UIBattleWindow.BattleStage battleStage3 = this.currentState;
				if (battleStage3 == null)
				{
					return;
				}
				battleStage3.OnArmiesChanged();
				return;
			}
			else
			{
				if (message == "type_changed")
				{
					this.DecideState();
					return;
				}
				if (message == "stage_changed")
				{
					this.DecideState();
					return;
				}
				return;
			}
		}
	}

	// Token: 0x060019FA RID: 6650 RVA: 0x000FC16C File Offset: 0x000FA36C
	protected static bool PlayerInvolved(Logic.Battle b)
	{
		return BaseUI.LogicKingdom() != null && b != null && (global::Battle.PlayerIsAttacker(b, true) || global::Battle.PlayerIsDefender(b, true));
	}

	// Token: 0x060019FB RID: 6651 RVA: 0x000FC18C File Offset: 0x000FA38C
	public static bool PlayerHasControl(Logic.Battle b)
	{
		if (b == null)
		{
			return false;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		for (int i = 0; i < 2; i++)
		{
			List<Logic.Army> armies = b.GetArmies(i);
			if (armies.Count >= 1)
			{
				Logic.Army army = armies[0];
				if (army.IsOwnStance(kingdom) && !army.IsHiredMercenary())
				{
					return true;
				}
			}
		}
		if (b.defender_kingdom == kingdom)
		{
			Logic.Settlement settlement = b.settlement;
			if (((settlement != null) ? settlement.keep_effects : null) != null && b.settlement.keep_effects.GetController() != kingdom)
			{
				return false;
			}
			Logic.Settlement settlement2 = b.settlement;
			if (((settlement2 != null) ? settlement2.garrison : null) != null && b.settlement.garrison.GetManPower() > 0 && !b.is_plunder)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060019FC RID: 6652 RVA: 0x000FC240 File Offset: 0x000FA440
	public bool IsDestoryed()
	{
		return this == null && this != null;
	}

	// Token: 0x060019FD RID: 6653 RVA: 0x000FC254 File Offset: 0x000FA454
	public virtual void ValidateSelectionObject()
	{
		if (this.logic != null)
		{
			Logic.Battle battle = this.GetBatlleFromMessageWnd();
			if (battle == null)
			{
				battle = this.GetBattleFromSelection();
			}
			if (battle == this.logic)
			{
				return;
			}
		}
		this.Start();
	}

	// Token: 0x060019FE RID: 6654 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void StoreState()
	{
	}

	// Token: 0x060019FF RID: 6655 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void RestoreState()
	{
	}

	// Token: 0x06001A00 RID: 6656 RVA: 0x000FC28A File Offset: 0x000FA48A
	public void Release()
	{
		this.RemoveListeners();
		this.logic = null;
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x06001A01 RID: 6657 RVA: 0x0002C53B File Offset: 0x0002A73B
	public bool PreserveWindow()
	{
		return true;
	}

	// Token: 0x06001A02 RID: 6658 RVA: 0x000FC2A4 File Offset: 0x000FA4A4
	public GameObject GetPrototype()
	{
		return this.prototype;
	}

	// Token: 0x06001A03 RID: 6659 RVA: 0x000FC2AC File Offset: 0x000FA4AC
	public void SetPrototype(GameObject prorotype)
	{
		this.prototype = prorotype;
	}

	// Token: 0x06001A06 RID: 6662 RVA: 0x000FC361 File Offset: 0x000FA561
	GameObject BaseUI.ISelectionPanel.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06001A07 RID: 6663 RVA: 0x000FC369 File Offset: 0x000FA569
	T BaseUI.ISelectionPanel.GetComponent<T>()
	{
		return base.GetComponent<T>();
	}

	// Token: 0x0400109C RID: 4252
	[NonSerialized]
	public Logic.Battle logic;

	// Token: 0x0400109D RID: 4253
	private global::Battle battle;

	// Token: 0x0400109E RID: 4254
	private UIBattleWindow.Preparation preparation;

	// Token: 0x0400109F RID: 4255
	private UIBattleWindow.Ongoing ongoing;

	// Token: 0x040010A0 RID: 4256
	private UIBattleWindow.Outcome outcome;

	// Token: 0x040010A1 RID: 4257
	private UIBattleWindow.Siege siege;

	// Token: 0x040010A2 RID: 4258
	private UIBattleWindow.BattleStage currentState;

	// Token: 0x040010A3 RID: 4259
	private bool totals_dirty;

	// Token: 0x040010A5 RID: 4261
	private static RenderTexture troops_render_texture;

	// Token: 0x040010A6 RID: 4262
	private static Dictionary<string, UIBattleWindow.TroopModels> troops = new Dictionary<string, UIBattleWindow.TroopModels>();

	// Token: 0x040010A7 RID: 4263
	private static Transform troops_container;

	// Token: 0x040010A8 RID: 4264
	[Header("Rendered troop settings")]
	public GameObject shadow_plane_prefab;

	// Token: 0x040010A9 RID: 4265
	public float dist_between_rows = 1f;

	// Token: 0x040010AA RID: 4266
	public float dist_between_sides = 5f;

	// Token: 0x040010AB RID: 4267
	public int max_units_per_row = 5;

	// Token: 0x040010AC RID: 4268
	public float max_space_for_troops = 17f;

	// Token: 0x040010AD RID: 4269
	public Vector3 cam_position = new Vector3(0f, 20.9f, -86.2f);

	// Token: 0x040010AE RID: 4270
	public Vector3 cam_rotation = new Vector3(12.231f, 0f, 0f);

	// Token: 0x040010AF RID: 4271
	public float cam_fov = 4.559535f;

	// Token: 0x040010B0 RID: 4272
	public int render_tex_width = 1386;

	// Token: 0x040010B1 RID: 4273
	public int render_tex_height = 175;

	// Token: 0x040010B2 RID: 4274
	public int troops_per_unit = 3;

	// Token: 0x040010B3 RID: 4275
	private static bool refresh_troops = false;

	// Token: 0x040010B4 RID: 4276
	private GameObject prototype;

	// Token: 0x0200070F RID: 1807
	public struct TroopModels
	{
		// Token: 0x0400380D RID: 14349
		public Logic.Unit.Def def;

		// Token: 0x0400380E RID: 14350
		public int occupied;

		// Token: 0x0400380F RID: 14351
		public int left;

		// Token: 0x04003810 RID: 14352
		public int right;

		// Token: 0x04003811 RID: 14353
		public List<UnitColliderContainer> models;
	}

	// Token: 0x02000710 RID: 1808
	internal abstract class BattleStage : MonoBehaviour
	{
		// Token: 0x0600495D RID: 18781 RVA: 0x0021B1B8 File Offset: 0x002193B8
		public void UpdateUnitTooltip()
		{
			if (this.m_TroopsView == null)
			{
				return;
			}
			Camera troops_camera = UIBattleWindow.troops_camera;
			Vector3 mousePosition = Input.mousePosition;
			RectTransform component = this.m_TroopsView.GetComponent<RectTransform>();
			Vector2 v;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(component, mousePosition, null, out v);
			v.x += component.rect.width / 2f;
			v.y += component.rect.height / 2f;
			v.x /= component.rect.width;
			v.y /= component.rect.height;
			int num = Physics.RaycastNonAlloc(troops_camera.ViewportPointToRay(v), this.hits);
			global::Unit unit = null;
			float num2 = float.MaxValue;
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.hits[i];
				UnitColliderContainer component2 = this.hits[i].collider.gameObject.GetComponent<UnitColliderContainer>();
				if (component2 != null)
				{
					float num3 = Vector3.Distance(component2.unit.instancer.Position, raycastHit.point);
					if (num3 <= num2)
					{
						unit = component2.unit;
						num2 = num3;
					}
				}
			}
			if (this.m_LastTooltipTargetUnit == unit)
			{
				return;
			}
			if (unit != null)
			{
				Tooltip tooltip = Tooltip.Get(this.m_TroopsView.gameObject, true);
				tooltip.SetDef("UnitTooltip", new Vars(unit.logic));
				BaseUI baseUI = BaseUI.Get();
				if (baseUI != null)
				{
					baseUI.RefreshTooltip(tooltip, true);
				}
			}
			else
			{
				Tooltip tooltip2 = Tooltip.Get(this.m_TroopsView.gameObject, false);
				if (tooltip2 != null)
				{
					tooltip2.SetDef(null, null);
				}
			}
			this.m_LastTooltipTargetUnit = unit;
		}

		// Token: 0x0600495E RID: 18782 RVA: 0x0021B38C File Offset: 0x0021958C
		public void UpdateIllustration()
		{
			if (this.m_ProvinceIllustration != null)
			{
				this.m_ProvinceIllustration.SetObject(this.logic);
				bool flip = (this.logic.type != Logic.Battle.Type.BreakSiege) ? (this.logic_sides[0] == 0) : (this.logic_sides[0] != 0);
				this.m_ProvinceIllustration.Flip(flip);
			}
		}

		// Token: 0x0600495F RID: 18783 RVA: 0x0021B3EC File Offset: 0x002195EC
		protected void StartMusic()
		{
			if (BattleMap.battle != null && BattleMap.battle.IsValid())
			{
				return;
			}
			if (this.logic.stage == Logic.Battle.Stage.Preparing && (global::Battle.PlayerIsAttacker(this.logic, true) || global::Battle.PlayerIsDefender(this.logic, true)))
			{
				BackgroundMusic.OnTrigger("BattlePreparationTrigger", null);
				if (!GameLogic.Get(false).IsPaused())
				{
					StudioEventEmitter eventEmitter = BackgroundMusic.eventEmitter;
					if (eventEmitter == null)
					{
						return;
					}
					eventEmitter.SetParameter("MusicLoop", 1f, false);
				}
			}
		}

		// Token: 0x06004960 RID: 18784 RVA: 0x0021B468 File Offset: 0x00219668
		protected void StopMusic()
		{
			StudioEventEmitter eventEmitter = BackgroundMusic.eventEmitter;
			if (eventEmitter == null)
			{
				return;
			}
			eventEmitter.SetParameter("MusicLoop", 0f, false);
		}

		// Token: 0x06004961 RID: 18785 RVA: 0x0021B484 File Offset: 0x00219684
		public virtual void SetBattle(Logic.Battle b, Vars v)
		{
			this.logic = b;
			this.vars = v;
			UICommon.FindComponents(this, true);
			this.CalcSides();
			this.Init();
			this.AddListeners();
			this.Refresh();
		}

		// Token: 0x06004962 RID: 18786 RVA: 0x0021B4B4 File Offset: 0x002196B4
		protected void UpdateRenderTexture()
		{
			if (this.m_TroopsView != null)
			{
				this.m_TroopsView.gameObject.SetActive(this.logic.type != Logic.Battle.Type.Naval);
				this.m_TroopsView.texture = UIBattleWindow.troops_render_texture;
			}
		}

		// Token: 0x06004963 RID: 18787 RVA: 0x0021B500 File Offset: 0x00219700
		protected virtual void OnDestroy()
		{
			this.RemoveListeners();
			this.StopMusic();
			this.m_LastTooltipTargetUnit = null;
			GameSpeed.OnPaused -= this.OnPaused;
		}

		// Token: 0x06004964 RID: 18788 RVA: 0x0021B526 File Offset: 0x00219726
		private void OnPaused(bool _)
		{
			this.StopMusic();
		}

		// Token: 0x06004965 RID: 18789 RVA: 0x0021B530 File Offset: 0x00219730
		protected virtual void Init()
		{
			this.UpdateRenderTexture();
			this.UpdateIllustration();
			this.InitRetreatButton();
			if (this.m_ValueEstimation != null)
			{
				UIBattleWindow.SetEstimationTooltip(this.m_ValueEstimation.gameObject, this.logic);
			}
			if (this.m_Retreat != null)
			{
				Tooltip.Get(this.m_Retreat.gameObject, true).SetText("Battle.Retreat", null, null);
			}
			if (this.m_StopPlunder != null)
			{
				Tooltip.Get(this.m_StopPlunder.gameObject, true).SetText("Battle.Plunder.Retreat", null, null);
			}
			GameSpeed.OnPaused += this.OnPaused;
		}

		// Token: 0x06004966 RID: 18790 RVA: 0x0021B5DC File Offset: 0x002197DC
		protected virtual void AddListeners()
		{
			if (this.m_Retreat != null)
			{
				this.m_Retreat.onClick = new BSGButton.OnClick(this.Retreat);
			}
			if (this.m_StopPlunder != null)
			{
				this.m_StopPlunder.onClick = new BSGButton.OnClick(this.Retreat);
			}
		}

		// Token: 0x06004967 RID: 18791 RVA: 0x0021B633 File Offset: 0x00219833
		protected virtual void RemoveListeners()
		{
			if (this.m_Retreat != null)
			{
				this.m_Retreat.onClick = null;
			}
			if (this.m_StopPlunder != null)
			{
				this.m_StopPlunder.onClick = null;
			}
		}

		// Token: 0x06004968 RID: 18792 RVA: 0x0021B669 File Offset: 0x00219869
		public void Activate(bool active)
		{
			base.gameObject.SetActive(active);
			this.OnActivate();
		}

		// Token: 0x06004969 RID: 18793 RVA: 0x0021B67D File Offset: 0x0021987D
		public virtual void OnActivate()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				this.StopMusic();
			}
		}

		// Token: 0x0600496A RID: 18794
		public abstract void Refresh();

		// Token: 0x0600496B RID: 18795 RVA: 0x0021B692 File Offset: 0x00219892
		public virtual void UpdateState()
		{
			this.UpdateEstimation();
		}

		// Token: 0x0600496C RID: 18796 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void OnArmiesChanged()
		{
		}

		// Token: 0x0600496D RID: 18797 RVA: 0x0021B69A File Offset: 0x0021989A
		protected void CalcSides()
		{
			global::Battle.CalcSides(this.logic, out this.logic_sides[0], out this.logic_sides[1], false);
		}

		// Token: 0x0600496E RID: 18798 RVA: 0x0021B6C0 File Offset: 0x002198C0
		protected BSGButton CreateButton(GameObject prototype, GameObject parent, string name, string text_key)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject = global::Common.Spawn(prototype, false, false);
			gameObject.name = name;
			gameObject.transform.SetParent(parent.transform, false);
			UIText.SetTextKey(gameObject.GetComponentInChildren<TextMeshProUGUI>(), text_key, null, null);
			return gameObject.GetComponent<BSGButton>();
		}

		// Token: 0x0600496F RID: 18799 RVA: 0x0021B718 File Offset: 0x00219918
		protected UIBattleMap BuildMap(GameObject root)
		{
			UIBattleMap uibattleMap = null;
			GameObject gameObject = global::Common.FindChildByName(root, "MapFrame", true, true);
			if (gameObject != null)
			{
				uibattleMap = gameObject.GetComponent<UIBattleMap>();
			}
			if (uibattleMap != null)
			{
				uibattleMap.BuildMap(this.logic);
			}
			return uibattleMap;
		}

		// Token: 0x06004970 RID: 18800 RVA: 0x0021B758 File Offset: 0x00219958
		protected void UpdateShield()
		{
			for (int i = 0; i <= 1; i++)
			{
				int num = this.logic_sides[i];
				Logic.Object @object = (num == 0) ? this.logic.attacker : this.logic.defender;
				if (@object is Logic.Settlement)
				{
					Logic.Settlement settlement = @object as Logic.Settlement;
					@object = ((settlement != null) ? settlement.GetController() : null);
				}
				GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Kingdom_Side" + i, true, true);
				if (gameObject != null)
				{
					UIKingdomIcon componentInChildren = gameObject.GetComponentInChildren<UIKingdomIcon>();
					if (componentInChildren != null)
					{
						componentInChildren.SetObject(@object, null);
					}
				}
				GameObject gameObject2 = global::Common.FindChildByName(base.gameObject, "id_Kingdom_LoyalTo_Side" + i, true, true);
				if (gameObject2 != null)
				{
					bool? flag;
					if (@object == null)
					{
						flag = null;
					}
					else
					{
						Logic.Kingdom kingdom = @object.GetKingdom();
						flag = ((kingdom != null) ? new bool?(kingdom.IsRebelKingdom()) : null);
					}
					if (flag ?? false)
					{
						this.logic.GetArmy(num);
						if (@object.GetKingdom().type == Logic.Kingdom.Type.LoyalistsFaction)
						{
							Logic.Kingdom kingdom2 = null;
							Logic.Object object2 = @object;
							if (object2 != null)
							{
								Rebellion rebellion;
								if ((rebellion = (object2 as Rebellion)) == null)
								{
									Logic.Army army;
									if ((army = (object2 as Logic.Army)) != null)
									{
										Logic.Army army2 = army;
										kingdom2 = army2.game.GetKingdom(army2.rebel.loyal_to);
									}
								}
								else
								{
									kingdom2 = rebellion.GetLoyalTo();
								}
							}
							if (kingdom2 != null)
							{
								UIKingdomIcon componentInChildren2 = gameObject2.GetComponentInChildren<UIKingdomIcon>();
								if (componentInChildren2 != null)
								{
									componentInChildren2.SetObject(kingdom2, null);
								}
								gameObject2.SetActive(true);
							}
							else
							{
								gameObject2.SetActive(false);
							}
						}
						else
						{
							gameObject2.SetActive(false);
						}
					}
					else
					{
						gameObject2.SetActive(false);
					}
				}
			}
		}

		// Token: 0x06004971 RID: 18801 RVA: 0x0021B910 File Offset: 0x00219B10
		protected void UpdateArmies()
		{
			for (int i = 0; i <= 1; i++)
			{
				int num = this.logic_sides[i];
				string name = "id_LeaderSide" + i;
				GameObject gameObject = global::Common.FindChildByName(base.gameObject, name, true, false);
				if (!(gameObject == null))
				{
					UICharacterIcon leaderIcon = global::Common.FindChildComponent<UICharacterIcon>(gameObject, "id_IconMarshal");
					GameObject rebelIcon = global::Common.FindChildByName(gameObject, "id_LeaderlessRebel", true, true);
					this.SetupSupporter(leaderIcon, rebelIcon, num);
					UICharacterIcon leaderIcon2 = global::Common.FindChildComponent<UICharacterIcon>(gameObject, "id_IconSupporter");
					GameObject rebelIcon2 = global::Common.FindChildByName(gameObject, "id_LeaderlessRebelSupport", true, true);
					this.SetupSupporter(leaderIcon2, rebelIcon2, num + 2);
					UIArmyManpower uiarmyManpower = global::Common.FindChildComponent<UIArmyManpower>(gameObject, "id_Manpower");
					UIArmyMorale uiarmyMorale = global::Common.FindChildComponent<UIArmyMorale>(gameObject, "id_Morale");
					UIArmyFood uiarmyFood = global::Common.FindChildComponent<UIArmyFood>(gameObject, "id_ArmyFood");
					UIArmyFood uiarmyFood2 = global::Common.FindChildComponent<UIArmyFood>(gameObject, "id_ArmyFoodSiege");
					UICastleFood uicastleFood = global::Common.FindChildComponent<UICastleFood>(gameObject, "id_CastleFood");
					GameObject gameObject2 = global::Common.FindChildByName(gameObject, "id_SiegeBars", true, true);
					UIArmyFood uiarmyFood3;
					if (this.logic.is_siege)
					{
						uiarmyFood3 = uiarmyFood2;
						if (uiarmyFood != null)
						{
							GameObject gameObject3 = uiarmyFood.gameObject;
							if (gameObject3 != null)
							{
								gameObject3.SetActive(false);
							}
						}
					}
					else
					{
						uiarmyFood3 = uiarmyFood;
						if (uiarmyFood2 != null)
						{
							GameObject gameObject4 = uiarmyFood2.gameObject;
							if (gameObject4 != null)
							{
								gameObject4.SetActive(false);
							}
						}
					}
					if (uiarmyManpower != null)
					{
						uiarmyManpower.Clear();
						uiarmyManpower.hideIfZero = false;
						if (num == 1)
						{
							uiarmyManpower.SetSettlement(this.logic.settlement);
						}
						else
						{
							uiarmyManpower.SetSettlement(null);
						}
					}
					if (uiarmyMorale != null)
					{
						uiarmyMorale.Clear();
						uiarmyMorale.SetBattle(this.logic);
						if (num == 1)
						{
							uiarmyMorale.SetSettlement(this.logic.settlement);
						}
						else
						{
							uiarmyMorale.SetSettlement(null);
						}
					}
					if (uiarmyFood3 != null)
					{
						uiarmyFood3.Clear();
						uiarmyFood3.gameObject.SetActive(false);
					}
					List<Logic.Army> armies = this.logic.GetArmies(num);
					if (this.logic.is_siege && num == 1)
					{
						if (uiarmyFood3 != null)
						{
							uiarmyFood3.gameObject.SetActive(false);
						}
						if (gameObject2 != null)
						{
							gameObject2.SetActive(true);
							this.vars = this.GetBattleVars(this.logic);
							UIBattleResilience uibattleResilience = global::Common.FindChildComponent<UIBattleResilience>(gameObject2, "id_ResilienceContainer");
							if (uibattleResilience != null)
							{
								uibattleResilience.SetBattle(this.logic, this.vars);
							}
							UIBattleSiegeDefense uibattleSiegeDefense = global::Common.FindChildComponent<UIBattleSiegeDefense>(gameObject2, "id_SiegeDefenseContainer");
							if (uibattleSiegeDefense != null)
							{
								uibattleSiegeDefense.SetBattle(this.logic, this.vars);
							}
						}
					}
					else if (gameObject2 != null)
					{
						gameObject2.SetActive(false);
					}
					for (int j = 0; j < armies.Count; j++)
					{
						global::Army army = armies[j].visuals as global::Army;
						if (!(army == null))
						{
							if (uiarmyManpower != null)
							{
								uiarmyManpower.AddArmy(army);
							}
							if (uiarmyMorale != null)
							{
								uiarmyMorale.AddArmy(army);
							}
							if (uiarmyFood3 != null)
							{
								if (!this.logic.is_siege || num == 0)
								{
									uiarmyFood3.AddArmy(army);
									uiarmyFood3.gameObject.SetActive(true);
								}
								else
								{
									uiarmyFood3.gameObject.SetActive(false);
								}
							}
						}
					}
					if (uicastleFood != null)
					{
						if (this.logic.is_siege && this.logic.settlement != null && num == 1)
						{
							uicastleFood.SetCastle(this.logic.settlement);
							uicastleFood.gameObject.SetActive(true);
						}
						else
						{
							uicastleFood.gameObject.SetActive(false);
							uicastleFood.SetCastle(null);
						}
					}
				}
			}
		}

		// Token: 0x06004972 RID: 18802 RVA: 0x0021BC9C File Offset: 0x00219E9C
		private void SetupSupporter(UICharacterIcon leaderIcon, GameObject rebelIcon, int reinf_id)
		{
			if (leaderIcon != null)
			{
				leaderIcon.OnSelect -= this.OpenAvailableReinforcementsWindow;
				Vars vars = new Vars();
				int num = reinf_id % 2;
				Logic.Army army;
				if (reinf_id < 2)
				{
					army = this.logic.GetArmy(num);
				}
				else
				{
					army = this.logic.GetSupporter(num);
				}
				Logic.Character character = (army != null) ? army.leader : null;
				if (character != null)
				{
					Logic.Army army2 = this.logic.GetArmy(reinf_id);
					bool flag;
					if (army2 == null)
					{
						flag = false;
					}
					else
					{
						Logic.Rebel rebel = army2.rebel;
						bool? flag2 = (rebel != null) ? new bool?(rebel.IsRegular()) : null;
						bool flag3 = true;
						flag = (flag2.GetValueOrDefault() == flag3 & flag2 != null);
					}
					if (flag)
					{
						leaderIcon.gameObject.SetActive(false);
						rebelIcon.SetActive(true);
						return;
					}
					leaderIcon.SetObject(character, null);
					leaderIcon.ShowCrest(true);
					leaderIcon.ShowStatus(false);
					leaderIcon.EnableClassLevel(true);
					rebelIcon.SetActive(false);
					return;
				}
				else
				{
					Logic.Army army3 = this.logic.reinforcements[reinf_id].army;
					if (army3 == null && reinf_id >= 2 && this.logic.GetArmy(reinf_id % 2) != null)
					{
						army3 = this.logic.reinforcements[reinf_id % 2].army;
					}
					else if (reinf_id >= 2 && army3 != null && this.logic.GetArmy(reinf_id % 2) == army3)
					{
						army3 = this.logic.reinforcements[reinf_id % 2].army;
					}
					Logic.Character character2 = (army3 != null) ? army3.leader : null;
					if (!this.logic.game.IsMultiplayer())
					{
						vars.Set<bool>("is_reinforcement", true);
						vars.Set<int>("battle_side", reinf_id);
						vars.Set<Logic.Battle>("battle", this.logic);
						vars.Set<Logic.Kingdom>("side", this.logic.GetSideKingdom(reinf_id));
						vars.Set<Logic.Character>("side_leader", character);
						vars.Set<bool>("side_has_leader", character != null);
						List<Logic.Character> list = this.logic.FindValidReinforcements(num);
						vars.Set<bool>("can_add_reinforcements", this.logic.GetSideKingdom(num).IsOwnStance(BaseUI.LogicKingdom()) && list != null && list.Count > 0);
						if (character2 != null)
						{
							vars.Set<Logic.Character>("obj", character2);
						}
						if (this.logic.GetSideKingdom(num).IsOwnStance(BaseUI.LogicKingdom()))
						{
							vars.Set<bool>("is_player_side", true);
							leaderIcon.OnSelect += this.OpenAvailableReinforcementsWindow;
						}
						else
						{
							vars.Set<bool>("is_player_side", false);
						}
					}
					leaderIcon.SetObject(character2, vars);
				}
			}
		}

		// Token: 0x06004973 RID: 18803 RVA: 0x0021BF20 File Offset: 0x0021A120
		private void OpenAvailableReinforcementsWindow(UICharacterIcon icon)
		{
			int num = icon.vars.Get<int>("battle_side", 0);
			List<Logic.Character> list = this.logic.FindValidReinforcements(num);
			if (list == null || list.Count == 0)
			{
				return;
			}
			List<Value> list2 = new List<Value>();
			List<Vars> list3 = new List<Vars>();
			Vars vars;
			for (int i = 0; i < list.Count; i++)
			{
				Logic.Character character = list[i];
				vars = new Vars();
				Logic.Battle battle = this.logic;
				Logic.Character character2 = character;
				float val = battle.CalcReinforcementTime((character2 != null) ? character2.GetArmy() : null);
				vars.Set<int>("battle_side", num);
				vars.Set<float>("estimation_time", val);
				vars.Set<string>("rightTextKey", "Battle.reinforcement_estimation_text");
				list2.Add(character);
				list3.Add(vars);
			}
			vars = new Vars();
			vars.Set<string>("localization_key", "TargetPicker.none_text");
			List<TargetPickerData> list4 = TargetPickerData.Create(list2, list3, null);
			list4.Insert(0, new TargetPickerData
			{
				Target = "None",
				Vars = vars
			});
			if (num > 1)
			{
				UITargetSelectWindow.ShowDialog(list4, list[0], new Action<Value>(this.OnSelectSecondReinforcementTarget), null, null, null, null, null, "", "");
				return;
			}
			UITargetSelectWindow.ShowDialog(list4, list[0], new Action<Value>(this.OnSelectMainReinforcementTarget), null, null, null, null, null, "", "");
		}

		// Token: 0x06004974 RID: 18804 RVA: 0x0021C088 File Offset: 0x0021A288
		private void OnSelectMainReinforcementTarget(Value value)
		{
			Logic.Character character = value.Get<Logic.Character>();
			if (character == null)
			{
				int reinf_id = -1;
				if (this.logic.GetSideKingdom(0).IsOwnStance(BaseUI.LogicKingdom()))
				{
					reinf_id = 0;
				}
				else if (this.logic.GetSideKingdom(1).IsOwnStance(BaseUI.LogicKingdom()))
				{
					reinf_id = 1;
				}
				this.logic.SetReinforcements(null, reinf_id, -1f, false, true);
			}
			else
			{
				Logic.Army army = character.GetArmy();
				int joinSide = this.logic.GetJoinSide(army, true);
				this.logic.SetReinforcements(army, joinSide, this.logic.CalcReinforcementTime(army), false, true);
				if (army.IsOwnStance(BaseUI.LogicKingdom()))
				{
					army.MoveTo(this.logic, -1f, false);
				}
			}
			this.UpdateArmies();
		}

		// Token: 0x06004975 RID: 18805 RVA: 0x0021C144 File Offset: 0x0021A344
		private void OnSelectSecondReinforcementTarget(Value value)
		{
			Logic.Character character = value.Get<Logic.Character>();
			if (character == null)
			{
				int num = -1;
				if (this.logic.GetSideKingdom(0).IsOwnStance(BaseUI.LogicKingdom()))
				{
					num = 0;
				}
				else if (this.logic.GetSideKingdom(1).IsOwnStance(BaseUI.LogicKingdom()))
				{
					num = 1;
				}
				this.logic.SetReinforcements(null, num + 2, -1f, false, true);
			}
			else
			{
				Logic.Army army = character.GetArmy();
				int joinSide = this.logic.GetJoinSide(army, true);
				this.logic.SetReinforcements(army, joinSide + 2, this.logic.CalcReinforcementTime(army), false, true);
				if (army.IsOwnStance(BaseUI.LogicKingdom()))
				{
					army.MoveTo(this.logic, -1f, false);
				}
			}
			this.UpdateArmies();
		}

		// Token: 0x06004976 RID: 18806 RVA: 0x0021C204 File Offset: 0x0021A404
		protected void UpdateTerrainBonuses()
		{
			if (this.logic == null)
			{
				return;
			}
			if (this.m_BattleBonuses == null)
			{
				return;
			}
			UICommon.DeleteChildren(this.m_BattleBonuses);
			Logic.Battle battle = this.logic;
			bool active = ((battle != null) ? battle.battle_bonuses : null) != null && this.logic.battle_bonuses.Count > 0;
			this.m_BattleBonuses.parent.gameObject.SetActive(active);
			for (int i = 0; i < this.logic.battle_bonuses.Count; i++)
			{
				UIBattleWindow.TerrainBonusSlot terrainBonusSlot = UIBattleWindow.TerrainBonusSlot.Create(this.logic.battle_bonuses[i], UICommon.GetPrefab("TerrainBonusSlot", null), this.m_BattleBonuses);
				if (terrainBonusSlot != null)
				{
					this.terrainSlots.Add(terrainBonusSlot);
				}
			}
		}

		// Token: 0x06004977 RID: 18807 RVA: 0x0021C2D0 File Offset: 0x0021A4D0
		protected Vars GetBattleVars(Logic.Battle b)
		{
			Vars vars = b.Vars();
			vars.Set<Logic.Realm>("realm", GameLogic.Get(true).GetRealm(this.logic.realm_id));
			vars.Set<Logic.Kingdom>("own_kingdom", this.GetKingdom(b, true));
			vars.Set<Logic.Kingdom>("enemy_kingdom", this.GetKingdom(b, false));
			vars.Set<Logic.Army>("reinforcing_army", this.GetReinforcingArmy());
			vars.Set<Logic.Settlement>("settlement", b.settlement);
			return vars;
		}

		// Token: 0x06004978 RID: 18808 RVA: 0x0021C34C File Offset: 0x0021A54C
		private Logic.Army GetReinforcingArmy()
		{
			Logic.Army supporter;
			if (UIBattleWindow.PlayerInvolved(this.logic))
			{
				Logic.Kingdom kingdom = BaseUI.LogicKingdom();
				bool flag = this.logic.attacker_kingdom.id == kingdom.id;
				supporter = this.logic.GetSupporter(flag ? 0 : 1);
			}
			else
			{
				supporter = this.logic.GetSupporter(0);
			}
			return supporter;
		}

		// Token: 0x06004979 RID: 18809 RVA: 0x0021C3AC File Offset: 0x0021A5AC
		private Logic.Kingdom GetKingdom(Logic.Battle b, bool own)
		{
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (GameLogic.Get(true) == null)
			{
				return null;
			}
			if (b.attacker_kingdom.id == kingdom.id)
			{
				if (!own)
				{
					return b.defender_kingdom;
				}
				return b.attacker_kingdom;
			}
			else
			{
				if (b.defender_kingdom.id != kingdom.id)
				{
					return null;
				}
				if (!own)
				{
					return b.attacker_kingdom;
				}
				return b.defender_kingdom;
			}
		}

		// Token: 0x0600497A RID: 18810 RVA: 0x0021C414 File Offset: 0x0021A614
		protected void UpdateEstimation()
		{
			if (this.logic.simulation == null)
			{
				return;
			}
			float num;
			if (this.logic.winner < 0)
			{
				num = this.logic.simulation.GetEstimation();
				if (this.logic_sides[0] != 0)
				{
					num = 1f - num;
				}
			}
			else
			{
				num = ((this.logic_sides[0] == this.logic.winner) ? 1f : 0f);
			}
			if (this.m_ValueEstimation != null)
			{
				int num2 = 0;
				string text;
				if (this.logic.winner >= 0)
				{
					text = ((num2 == this.logic.winner) ? "won" : "lost");
				}
				else
				{
					text = global::Battle.GetEstimationKey((num2 == 0) ? (1f - num) : num);
				}
				this.m_ValueEstimation.color = global::Battle.GetEstimationColor(text);
				if (this.logic.stage == Logic.Battle.Stage.Preparing)
				{
					text = "preparation_" + text;
				}
				else if (UIBattleWindow.PlayerInvolved(this.logic))
				{
					text = "player_" + text;
				}
				UIText.SetText(this.m_ValueEstimation, global::Battle.GetEstimationText(text));
			}
		}

		// Token: 0x0600497B RID: 18811 RVA: 0x0021C530 File Offset: 0x0021A730
		protected void EnterBatle(BSGButton button)
		{
			if (this.logic == null)
			{
				return;
			}
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom == null)
			{
				return;
			}
			AutoSaveManager.Save(AutoSaveManager.Type.Event, null, "battleview");
			int side = this.logic.LeadBattleSide(kingdom);
			BattleViewLoader.InitBattleLoad(this.logic, kingdom.id, side, BattleViewLoader.GetBattleSceneName(this.logic));
			this.StopMusic();
			FMODVoiceProvider.ClearAllInactiveVoices();
		}

		// Token: 0x0600497C RID: 18812 RVA: 0x0021C594 File Offset: 0x0021A794
		protected void Retreat(int side)
		{
			global::Army army = global::Army.Get(this.logic.GetArmy(side));
			this.logic.DoAction("retreat", side, "");
			if (army != null)
			{
				if (this.logic.type == Logic.Battle.Type.Siege)
				{
					if (side == 0)
					{
						if (army.logic.leader != null)
						{
							BaseUI.PlayVoiceEvent(this.logic.def.field.FindChild("LiftSiegeAttacker", null, true, true, true, '.'), army.logic.leader);
						}
					}
					else if (side == 1)
					{
						BaseUI.PlayVoiceEvent(this.logic.def.field.FindChild("LiftSiegeDefender", null, true, true, true, '.'), null);
					}
				}
				WorldUI.Get().SelectObj(army.gameObject, false, true, true, true);
			}
		}

		// Token: 0x0600497D RID: 18813 RVA: 0x0021C663 File Offset: 0x0021A863
		protected virtual string GetRetreatMessageKey()
		{
			if (this.logic.type == Logic.Battle.Type.Plunder)
			{
				return "StopPlunderingMessage";
			}
			return "RetreatMessage";
		}

		// Token: 0x0600497E RID: 18814 RVA: 0x0021C67E File Offset: 0x0021A87E
		protected void Retreat(BSGButton button)
		{
			MessageWnd.Create(global::Defs.GetDefField(this.GetRetreatMessageKey(), null), new Vars(this.logic), null, new MessageWnd.OnButton(this.OnRetreatMessage));
		}

		// Token: 0x0600497F RID: 18815 RVA: 0x0021C6B0 File Offset: 0x0021A8B0
		public virtual bool OnRetreatMessage(MessageWnd wnd, string btn_id)
		{
			if (btn_id == "ok")
			{
				Logic.Kingdom kingdom = BaseUI.LogicKingdom();
				if (kingdom != null && this.logic != null)
				{
					if (kingdom.id == this.logic.attacker_kingdom.id)
					{
						this.Retreat(0);
					}
					else if (kingdom.id == this.logic.defender_kingdom.id)
					{
						this.Retreat(1);
					}
					else if (global::Battle.PlayerIsAttacker(this.logic, true))
					{
						this.RetreatSupporters(0);
					}
					else if (global::Battle.PlayerIsDefender(this.logic, true))
					{
						this.RetreatSupporters(1);
					}
				}
			}
			wnd.CloseAndDismiss(true);
			return true;
		}

		// Token: 0x06004980 RID: 18816 RVA: 0x0021C751 File Offset: 0x0021A951
		protected virtual string GetRefuseSupportMessageKey()
		{
			return "RefuseSupportMessage";
		}

		// Token: 0x06004981 RID: 18817 RVA: 0x0021C758 File Offset: 0x0021A958
		protected void RefuseSupport(BSGButton button)
		{
			MessageWnd.Create(global::Defs.GetDefField(this.GetRefuseSupportMessageKey(), null), new Vars(this.logic), null, new MessageWnd.OnButton(this.OnRefuseSupportMessage));
		}

		// Token: 0x06004982 RID: 18818 RVA: 0x0021C78C File Offset: 0x0021A98C
		public virtual bool OnRefuseSupportMessage(MessageWnd wnd, string btn_id)
		{
			if (btn_id == "ok")
			{
				Logic.Kingdom kingdom = BaseUI.LogicKingdom();
				if (kingdom != null || this.logic != null)
				{
					if (kingdom.id == this.logic.attacker_kingdom.id)
					{
						this.RefuseSupporters(0);
					}
					else if (kingdom.id == this.logic.defender_kingdom.id)
					{
						this.RefuseSupporters(1);
					}
				}
			}
			wnd.CloseAndDismiss(true);
			return true;
		}

		// Token: 0x06004983 RID: 18819 RVA: 0x0021C7FF File Offset: 0x0021A9FF
		protected void RetreatSupporters(int side)
		{
			this.logic.DoAction("retreat_supporters", side, "");
		}

		// Token: 0x06004984 RID: 18820 RVA: 0x0021C817 File Offset: 0x0021AA17
		protected void RefuseSupporters(int side)
		{
			this.logic.DoAction("refuse_supporters", side, "");
		}

		// Token: 0x06004985 RID: 18821 RVA: 0x0021C830 File Offset: 0x0021AA30
		protected void InitRetreatButton()
		{
			bool flag = this.logic.type == Logic.Battle.Type.Plunder;
			bool flag2 = false;
			if (global::Battle.PlayerIsAttacker(this.logic, false))
			{
				flag2 = !this.logic.ArmiesDefeated(0);
			}
			else if (global::Battle.PlayerIsDefender(this.logic, false) && this.logic.type != Logic.Battle.Type.Assault)
			{
				flag2 = !this.logic.ArmiesDefeated(1);
				if (flag2 && this.logic.type == Logic.Battle.Type.BreakSiege && !this.logic.CanRetreat(BaseUI.LogicKingdom()))
				{
					flag2 = false;
				}
			}
			if (this.m_Retreat != null)
			{
				this.m_Retreat.gameObject.SetActive(flag2 && !flag);
			}
			if (this.m_StopPlunder != null)
			{
				this.m_StopPlunder.gameObject.SetActive(flag2 && flag);
			}
		}

		// Token: 0x04003812 RID: 14354
		public Logic.Battle logic;

		// Token: 0x04003813 RID: 14355
		public Vars vars;

		// Token: 0x04003814 RID: 14356
		[UIFieldTarget("id_BattleBonuses")]
		protected RectTransform m_BattleBonuses;

		// Token: 0x04003815 RID: 14357
		[UIFieldTarget("id_TroopsView")]
		private RawImage m_TroopsView;

		// Token: 0x04003816 RID: 14358
		[UIFieldTarget("id_ProvinceIllustration")]
		private UIProvinceIllustration m_ProvinceIllustration;

		// Token: 0x04003817 RID: 14359
		[UIFieldTarget("id_Retreat")]
		protected BSGButton m_Retreat;

		// Token: 0x04003818 RID: 14360
		[UIFieldTarget("id_StopPlunder")]
		protected BSGButton m_StopPlunder;

		// Token: 0x04003819 RID: 14361
		[UIFieldTarget("id_ValueEstimation")]
		private TextMeshProUGUI m_ValueEstimation;

		// Token: 0x0400381A RID: 14362
		protected GameObject UnitFramePrefab;

		// Token: 0x0400381B RID: 14363
		protected int[] logic_sides = new int[2];

		// Token: 0x0400381C RID: 14364
		protected RaycastHit[] hits = new RaycastHit[25];

		// Token: 0x0400381D RID: 14365
		private global::Unit m_LastTooltipTargetUnit;

		// Token: 0x0400381E RID: 14366
		protected List<UIBattleWindow.TerrainBonusSlot> terrainSlots = new List<UIBattleWindow.TerrainBonusSlot>();
	}

	// Token: 0x02000711 RID: 1809
	internal class Preparation : UIBattleWindow.BattleStage
	{
		// Token: 0x06004987 RID: 18823 RVA: 0x0021C934 File Offset: 0x0021AB34
		public override void Refresh()
		{
			if (this.logic == null)
			{
				return;
			}
			base.BuildMap(base.gameObject);
			this.InitProgress();
			base.UpdateArmies();
			base.UpdateShield();
			base.UpdateTerrainBonuses();
			base.UpdateEstimation();
			this.UpdateTexts();
			base.StartMusic();
			this.ShowButtons(true);
			this.UpdateButtons();
		}

		// Token: 0x06004988 RID: 18824 RVA: 0x0021C98E File Offset: 0x0021AB8E
		private void Update()
		{
			this.UpdateProgress();
			this.UpdateButtons();
		}

		// Token: 0x06004989 RID: 18825 RVA: 0x0021C99C File Offset: 0x0021AB9C
		public override void UpdateState()
		{
			base.UpdateState();
			this.UpdateLeadButton();
		}

		// Token: 0x0600498A RID: 18826 RVA: 0x0021C9AA File Offset: 0x0021ABAA
		private void UpdateButtons()
		{
			if (this.logic.player_chosen_tactics)
			{
				this.ShowButtons(false);
			}
		}

		// Token: 0x0600498B RID: 18827 RVA: 0x0021C9C0 File Offset: 0x0021ABC0
		private void UpdateLeadButton()
		{
			if (this.btn_lead == null)
			{
				return;
			}
			if (this.logic.LeadBattleSide(BaseUI.LogicKingdom()) == -1)
			{
				this.btn_lead.Enable(false, false);
				Tooltip.Get(this.btn_lead.gameObject, true).SetDef("EnterBattleButtonNotLeader", this.vars);
			}
			else
			{
				this.btn_lead.Enable(true, false);
				Tooltip.Get(this.btn_lead.gameObject, true).SetDef("EnterBattleButton", this.vars);
			}
			this.btn_lead.enabled = true;
			this.btn_lead.SetSelected(this.logic.stage == Logic.Battle.Stage.EnteringBattle, false);
		}

		// Token: 0x0600498C RID: 18828 RVA: 0x0021CA74 File Offset: 0x0021AC74
		protected override void Init()
		{
			base.Init();
			if (this.m_EstimationBar != null)
			{
				this.m_EstimationBar.SetObject(this.logic);
				UIBattleWindow.SetEstimationTooltip(this.m_EstimationBar.gameObject, this.logic);
			}
			if (this.m_Name != null)
			{
				UIBattleWindow.SetEstimationTooltip(this.m_Name.gameObject, this.logic);
			}
			this.CreateButtons();
			if (this.m_RefuseSupport)
			{
				Tooltip.Get(this.m_RefuseSupport.gameObject, true).SetText("Battle.RefuseSupport", null, base.GetBattleVars(this.logic));
				this.m_RefuseSupport.gameObject.SetActive(false);
			}
			if (this.btn_lead != null)
			{
				this.btn_lead.AllowSelection(true);
			}
			if (this.m_LeadLabel != null)
			{
				UIText.SetTextKey(this.m_LeadLabel, "Message.buttons.lead", null, null);
			}
		}

		// Token: 0x0600498D RID: 18829 RVA: 0x0021CB68 File Offset: 0x0021AD68
		protected override void AddListeners()
		{
			base.AddListeners();
			if (this.btn_lead != null)
			{
				BSGButton bsgbutton = this.btn_lead;
				bsgbutton.onClick = (BSGButton.OnClick)Delegate.Combine(bsgbutton.onClick, new BSGButton.OnClick(this.OnEnterBattleButton));
			}
			for (int i = 0; i < this.btn_tactics.Count; i++)
			{
				BSGButton bsgbutton2 = this.btn_tactics[i];
				bsgbutton2.onClick = (BSGButton.OnClick)Delegate.Combine(bsgbutton2.onClick, new BSGButton.OnClick(this.ChooseTactics));
			}
			if (this.m_RefuseSupport != null)
			{
				this.m_RefuseSupport.onClick = new BSGButton.OnClick(base.RefuseSupport);
			}
		}

		// Token: 0x0600498E RID: 18830 RVA: 0x0021CC18 File Offset: 0x0021AE18
		protected override void RemoveListeners()
		{
			base.RemoveListeners();
			if (this.btn_lead != null)
			{
				BSGButton bsgbutton = this.btn_lead;
				bsgbutton.onClick = (BSGButton.OnClick)Delegate.Remove(bsgbutton.onClick, new BSGButton.OnClick(this.OnEnterBattleButton));
			}
			for (int i = 0; i < this.btn_tactics.Count; i++)
			{
				BSGButton bsgbutton2 = this.btn_tactics[i];
				bsgbutton2.onClick = (BSGButton.OnClick)Delegate.Remove(bsgbutton2.onClick, new BSGButton.OnClick(this.ChooseTactics));
			}
			if (this.m_RefuseSupport != null)
			{
				this.m_RefuseSupport.onClick = null;
			}
		}

		// Token: 0x0600498F RID: 18831 RVA: 0x0021CCBD File Offset: 0x0021AEBD
		private void InitProgress()
		{
			if (this.m_Progress == null)
			{
				return;
			}
			this.m_Progress.fillAmount = 0f;
		}

		// Token: 0x06004990 RID: 18832 RVA: 0x0021CCE0 File Offset: 0x0021AEE0
		private void UpdateProgress()
		{
			if (this.m_Progress == null)
			{
				return;
			}
			float preparation_time_cached = this.logic.preparation_time_cached;
			float num = this.logic.game.time - this.logic.stage_time;
			this.m_Progress.fillAmount = 1f - Mathf.Clamp01(num / preparation_time_cached);
		}

		// Token: 0x06004991 RID: 18833 RVA: 0x0021CD44 File Offset: 0x0021AF44
		private void CreateButtons()
		{
			base.CalcSides();
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Buttons", true, true);
			if (gameObject == null)
			{
				return;
			}
			this.CreateTacticsButtons(gameObject);
			if (this.logic.stage == Logic.Battle.Stage.EnteringBattle)
			{
				this.DisableTacticsButtons(true);
			}
			this.UpdateLeadButton();
		}

		// Token: 0x06004992 RID: 18834 RVA: 0x0021CD98 File Offset: 0x0021AF98
		private void CreateTacticsButtons(GameObject parent)
		{
			UICommon.DeleteChildren(parent.transform);
			this.btn_tactics.Clear();
			if (this.logic.stage != Logic.Battle.Stage.Preparing && this.logic.stage != Logic.Battle.Stage.EnteringBattle)
			{
				return;
			}
			if (this.logic.game.IsMultiplayer())
			{
				return;
			}
			DT.Field defField = global::Defs.GetDefField("BattleWindow", null);
			BattleSimulation simulation = this.logic.simulation;
			List<string> list = (simulation != null) ? simulation.ListTactics(this.logic_sides[0]) : null;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					string text = list[i];
					string text_key = string.Concat(new string[]
					{
						"Battle.tactics.",
						(this.logic_sides[0] == 0) ? "attacker" : "defender",
						".",
						text,
						".name"
					});
					string text_key2 = string.Concat(new string[]
					{
						"Battle.tactics.",
						(this.logic_sides[0] == 0) ? "attacker" : "defender",
						".",
						text,
						".description"
					});
					BSGButton bsgbutton = base.CreateButton(global::Defs.GetObj<GameObject>(defField, "button_tactics", null), parent, text, text_key);
					if (bsgbutton != null)
					{
						this.btn_tactics.Add(bsgbutton);
						Tooltip.Get(bsgbutton.gameObject, true).SetText("#" + global::Defs.Localize(text_key2, null, null, true, true), null, null);
					}
				}
			}
		}

		// Token: 0x06004993 RID: 18835 RVA: 0x0021CF18 File Offset: 0x0021B118
		private void UpdateTexts()
		{
			global::Battle battle = this.logic.visuals as global::Battle;
			Vars vars = (battle != null) ? battle.Vars() : null;
			if (this.m_Name != null && vars != null)
			{
				UIText.SetTextKey(this.m_Name, vars.Get<string>("BATTLE", null), vars, null);
			}
			Vars battleVars = base.GetBattleVars(this.logic);
			if (this.m_Description != null)
			{
				UIText.SetTextKey(this.m_Description, "Battle.Preparation.Description", battleVars, null);
			}
			if (this.m_PrepaingLabel != null)
			{
				UIText.SetTextKey(this.m_PrepaingLabel, "Battle.Preparation.label", battleVars, null);
			}
		}

		// Token: 0x06004994 RID: 18836 RVA: 0x0021CFC0 File Offset: 0x0021B1C0
		private void ChooseTactics(BSGButton button)
		{
			string name = button.name;
			this.logic.DoAction("tactics", this.logic_sides[0], name);
			this.logic.player_chosen_tactics = true;
		}

		// Token: 0x06004995 RID: 18837 RVA: 0x0021CFFC File Offset: 0x0021B1FC
		private void ShowButtons(bool shown)
		{
			bool flag = !this.logic.game.IsMultiplayer() && this.logic.type != Logic.Battle.Type.Naval;
			if (this.btn_lead != null)
			{
				this.btn_lead.gameObject.SetActive(flag && shown);
			}
			if (this.btn_tactics != null && this.btn_tactics.Count > 0)
			{
				for (int i = 0; i < this.btn_tactics.Count; i++)
				{
					this.btn_tactics[i].gameObject.SetActive(shown && flag);
				}
			}
		}

		// Token: 0x06004996 RID: 18838 RVA: 0x0021D098 File Offset: 0x0021B298
		private void DisableTacticsButtons(bool disbale)
		{
			if (this.btn_tactics != null && this.btn_tactics.Count > 0)
			{
				for (int i = 0; i < this.btn_tactics.Count; i++)
				{
					this.btn_tactics[i].Enable(!disbale, false);
				}
			}
		}

		// Token: 0x06004997 RID: 18839 RVA: 0x0021D0E8 File Offset: 0x0021B2E8
		private void OnEnterBattleButton(BSGButton b)
		{
			if (this.logic.LeadBattleSide(BaseUI.LogicKingdom()) == -1)
			{
				return;
			}
			Vars vars = new Vars(this.logic);
			vars.Set<string>("initial_estimation", this.GetInitalEstimationKey());
			vars.Set<Logic.Kingdom>("enemy_kingdom", this.GetEnemyKingdom());
			MessageWnd.Create("ConfirmLeadBattleMessage", vars, null, new MessageWnd.OnButton(this.OnLeadBattleConfirmMessage)).on_update = new MessageWnd.OnUpdate(this.ValidateLeadBattleConfirmation);
			Game game = GameLogic.Get(false);
			if (game == null)
			{
				return;
			}
			game.pause.AddRequest("LeadBattlePause", -2);
		}

		// Token: 0x06004998 RID: 18840 RVA: 0x0021D181 File Offset: 0x0021B381
		private Logic.Kingdom GetEnemyKingdom()
		{
			if (this.logic_sides[0] == 0)
			{
				return this.logic.defender_kingdom;
			}
			return this.logic.attacker_kingdom;
		}

		// Token: 0x06004999 RID: 18841 RVA: 0x0021D1A4 File Offset: 0x0021B3A4
		private string GetInitalEstimationKey()
		{
			float num = this.logic.simulation.GetEstimation();
			if (this.logic_sides[0] == 0)
			{
				num = 1f - num;
			}
			return global::Battle.GetEstimationKey(num);
		}

		// Token: 0x0600499A RID: 18842 RVA: 0x0021D1DA File Offset: 0x0021B3DA
		private bool OnLeadBattleConfirmMessage(MessageWnd wnd, string btn_id)
		{
			if (btn_id == "ok")
			{
				base.EnterBatle(null);
			}
			wnd.Close(false);
			Game game = GameLogic.Get(false);
			if (game != null)
			{
				game.pause.DelRequest("LeadBattlePause", -2);
			}
			return true;
		}

		// Token: 0x0600499B RID: 18843 RVA: 0x0021D218 File Offset: 0x0021B418
		private void ValidateLeadBattleConfirmation(MessageWnd wnd)
		{
			Logic.Battle battle = wnd.vars.obj.Get<Logic.Battle>();
			if (battle != null && battle.stage == Logic.Battle.Stage.Preparing)
			{
				return;
			}
			wnd.Close(false);
			Game game = GameLogic.Get(false);
			if (game == null)
			{
				return;
			}
			game.pause.DelRequest("LeadBattlePause", -2);
		}

		// Token: 0x0600499C RID: 18844 RVA: 0x0021D265 File Offset: 0x0021B465
		public override void OnArmiesChanged()
		{
			base.OnArmiesChanged();
			base.UpdateArmies();
		}

		// Token: 0x0400381F RID: 14367
		[UIFieldTarget("id_Name")]
		private TextMeshProUGUI m_Name;

		// Token: 0x04003820 RID: 14368
		[UIFieldTarget("id_Description")]
		private TextMeshProUGUI m_Description;

		// Token: 0x04003821 RID: 14369
		[UIFieldTarget("id_PrepaingLabel")]
		private TextMeshProUGUI m_PrepaingLabel;

		// Token: 0x04003822 RID: 14370
		[UIFieldTarget("id_LeadButton")]
		private BSGButton btn_lead;

		// Token: 0x04003823 RID: 14371
		[UIFieldTarget("id_LeadLabel")]
		private TextMeshProUGUI m_LeadLabel;

		// Token: 0x04003824 RID: 14372
		[UIFieldTarget("id_ProgressContainer")]
		private GameObject m_ProgressContainer;

		// Token: 0x04003825 RID: 14373
		[UIFieldTarget("id_progress")]
		private Image m_Progress;

		// Token: 0x04003826 RID: 14374
		[UIFieldTarget("id_estimation")]
		private UIBattleEstimationBar m_EstimationBar;

		// Token: 0x04003827 RID: 14375
		[UIFieldTarget("id_RefuseSupport")]
		private BSGButton m_RefuseSupport;

		// Token: 0x04003828 RID: 14376
		private UIBattleMap battleMap;

		// Token: 0x04003829 RID: 14377
		private List<BSGButton> btn_tactics = new List<BSGButton>();
	}

	// Token: 0x02000712 RID: 1810
	internal class Outcome : UIBattleWindow.BattleStage
	{
		// Token: 0x0600499E RID: 18846 RVA: 0x0021D286 File Offset: 0x0021B486
		public override void Refresh()
		{
			if (this.logic == null)
			{
				return;
			}
			base.BuildMap(base.gameObject);
			base.UpdateArmies();
			base.UpdateShield();
			this.UpdateTexts();
		}

		// Token: 0x0600499F RID: 18847 RVA: 0x0021D2B0 File Offset: 0x0021B4B0
		protected override void Init()
		{
			base.Init();
			this.CreateButtons();
		}

		// Token: 0x060049A0 RID: 18848 RVA: 0x0021D2C0 File Offset: 0x0021B4C0
		protected override void AddListeners()
		{
			base.AddListeners();
			if (this.btn_goto != null)
			{
				BSGButton bsgbutton = this.btn_goto;
				bsgbutton.onClick = (BSGButton.OnClick)Delegate.Combine(bsgbutton.onClick, new BSGButton.OnClick(this.GoTo));
			}
			if (this.btn_close != null)
			{
				BSGButton bsgbutton2 = this.btn_close;
				bsgbutton2.onClick = (BSGButton.OnClick)Delegate.Combine(bsgbutton2.onClick, new BSGButton.OnClick(this.Close));
			}
		}

		// Token: 0x060049A1 RID: 18849 RVA: 0x0021D340 File Offset: 0x0021B540
		protected override void RemoveListeners()
		{
			base.RemoveListeners();
			if (this.btn_goto != null)
			{
				BSGButton bsgbutton = this.btn_goto;
				bsgbutton.onClick = (BSGButton.OnClick)Delegate.Remove(bsgbutton.onClick, new BSGButton.OnClick(this.GoTo));
			}
			if (this.btn_close != null)
			{
				BSGButton bsgbutton2 = this.btn_close;
				bsgbutton2.onClick = (BSGButton.OnClick)Delegate.Remove(bsgbutton2.onClick, new BSGButton.OnClick(this.Close));
			}
		}

		// Token: 0x060049A2 RID: 18850 RVA: 0x0021D3BD File Offset: 0x0021B5BD
		private void Close(BSGButton b)
		{
			base.GetComponentInParent<UIBattleWindow>().Close();
		}

		// Token: 0x060049A3 RID: 18851 RVA: 0x0021D3CC File Offset: 0x0021B5CC
		private void CreateButtons()
		{
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Buttons", true, true);
			if (gameObject == null)
			{
				return;
			}
			UICommon.DeleteChildren(gameObject.transform);
			if (this.logic.IsFinishing())
			{
				DT.Field defField = global::Defs.GetDefField("BattleWindow", null);
				this.btn_close = base.CreateButton(global::Defs.GetObj<GameObject>(defField, "button_lead", null), gameObject, "close", "Message.buttons.ok");
				this.btn_goto = base.CreateButton(global::Defs.GetObj<GameObject>(defField, "button_lead", null), gameObject, "goto", "Message.buttons.goto");
			}
		}

		// Token: 0x060049A4 RID: 18852 RVA: 0x0021D460 File Offset: 0x0021B660
		private void UpdateTexts()
		{
			global::Battle battle = this.logic.visuals as global::Battle;
			Vars vars = (battle != null) ? battle.Vars() : null;
			if (this.m_Name != null && vars != null)
			{
				UIText.SetTextKey(this.m_Name, vars.Get<string>("BATTLE", null), vars, null);
			}
			bool flag = this.logic.winner == this.logic_sides[0];
			if (this.m_State != null)
			{
				UIText.SetTextKey(this.m_State, flag ? "Battle.Victory.Caption" : "Battle.Defeat.Caption", null, null);
				this.m_State.color = global::Battle.GetEstimationColor(flag ? "winning_decisively" : "losing_badly");
			}
			if (this.m_Description != null)
			{
				UIText.SetTextKey(this.m_Description, flag ? "Battle.Victory.Description" : "Battle.Defeat.Description", base.GetBattleVars(this.logic), null);
			}
		}

		// Token: 0x060049A5 RID: 18853 RVA: 0x0021D550 File Offset: 0x0021B750
		private void GoTo(BSGButton button)
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null || this.logic == null)
			{
				return;
			}
			worldUI.LookAt(this.logic.position, false);
		}

		// Token: 0x0400382A RID: 14378
		[UIFieldTarget("id_Name")]
		private TextMeshProUGUI m_Name;

		// Token: 0x0400382B RID: 14379
		[UIFieldTarget("id_State")]
		private TextMeshProUGUI m_State;

		// Token: 0x0400382C RID: 14380
		[UIFieldTarget("id_Description")]
		private TextMeshProUGUI m_Description;

		// Token: 0x0400382D RID: 14381
		private BSGButton btn_close;

		// Token: 0x0400382E RID: 14382
		private BSGButton btn_goto;

		// Token: 0x0400382F RID: 14383
		private UIBattleMap battleMap;
	}

	// Token: 0x02000713 RID: 1811
	internal class Ongoing : UIBattleWindow.BattleStage
	{
		// Token: 0x060049A7 RID: 18855 RVA: 0x0021D594 File Offset: 0x0021B794
		public override void Refresh()
		{
			if (this.logic == null)
			{
				return;
			}
			this.InitProgress();
			base.UpdateShield();
			this.UpdateTexts();
			base.UpdateEstimation();
			base.UpdateTerrainBonuses();
			base.UpdateArmies();
		}

		// Token: 0x060049A8 RID: 18856 RVA: 0x0021D5C4 File Offset: 0x0021B7C4
		protected override void Init()
		{
			base.Init();
			if (this.m_Name != null)
			{
				UIBattleWindow.SetEstimationTooltip(this.m_Name.gameObject, this.logic);
			}
			if (this.m_EstimationBar != null)
			{
				this.m_EstimationBar.SetObject(this.logic);
				UIBattleWindow.SetEstimationTooltip(this.m_EstimationBar.gameObject, this.logic);
			}
			if (this.m_RefuseSupport)
			{
				Tooltip.Get(this.m_RefuseSupport.gameObject, true).SetText("Battle.RefuseSupport", null, base.GetBattleVars(this.logic));
				this.m_RefuseSupport.gameObject.SetActive(false);
			}
		}

		// Token: 0x060049A9 RID: 18857 RVA: 0x0021D676 File Offset: 0x0021B876
		private void InitProgress()
		{
			if (this.m_PlunderProgressContianer != null)
			{
				UIBattleWindow.SetEstimationTooltip(this.m_PlunderProgressContianer.gameObject, this.logic);
			}
		}

		// Token: 0x060049AA RID: 18858 RVA: 0x0021D265 File Offset: 0x0021B465
		public override void OnArmiesChanged()
		{
			base.OnArmiesChanged();
			base.UpdateArmies();
		}

		// Token: 0x060049AB RID: 18859 RVA: 0x0021D69C File Offset: 0x0021B89C
		private void UpdateProgress()
		{
			bool flag = !this.HadDefenders();
			bool flag2 = (this.logic.type == Logic.Battle.Type.Plunder || this.logic.type == Logic.Battle.Type.PlunderInterrupt) && flag;
			if (this.m_EstimationBar != null)
			{
				this.m_EstimationBar.gameObject.SetActive(!flag2);
			}
			if (this.m_PlunderProgressContianer != null)
			{
				this.m_PlunderProgressContianer.gameObject.SetActive(flag2);
			}
			if (flag2 && this.m_PlunderProgress != null)
			{
				if (this.logic.def.duration <= 0f)
				{
					this.m_PlunderProgress.gameObject.SetActive(false);
					return;
				}
				this.m_PlunderProgress.fillAmount = this.logic.PlunderProgress();
			}
		}

		// Token: 0x060049AC RID: 18860 RVA: 0x0021D766 File Offset: 0x0021B966
		private bool HadDefenders()
		{
			return this.HasGarrison() || this.HasActiveArmies();
		}

		// Token: 0x060049AD RID: 18861 RVA: 0x0021D778 File Offset: 0x0021B978
		private bool HasActiveArmies()
		{
			if (this.logic.defenders.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < this.logic.defenders.Count; i++)
			{
				if (!this.logic.defenders[i].IsDefeated())
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060049AE RID: 18862 RVA: 0x0021D7D0 File Offset: 0x0021B9D0
		public bool HasGarrison()
		{
			Logic.Battle logic = this.logic;
			Garrison garrison;
			if (logic == null)
			{
				garrison = null;
			}
			else
			{
				Logic.Settlement settlement = logic.settlement;
				garrison = ((settlement != null) ? settlement.garrison : null);
			}
			Garrison garrison2 = garrison;
			if (garrison2 == null)
			{
				return false;
			}
			for (int i = 0; i < garrison2.units.Count; i++)
			{
				if (garrison2.units[i] != null && !garrison2.units[i].IsDefeated())
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060049AF RID: 18863 RVA: 0x0021D83C File Offset: 0x0021BA3C
		private void UpdatePlunderProgress()
		{
			if (this.logic == null)
			{
				if (this.m_PlunderProgress != null)
				{
					this.m_PlunderProgress.gameObject.SetActive(false);
				}
				return;
			}
			Logic.Settlement settlement = this.logic.settlement;
			bool flag = ((settlement != null) ? settlement.garrison : null) != null && this.logic.defenders.Count == 0;
			if ((this.logic.type != Logic.Battle.Type.Plunder && this.logic.type != Logic.Battle.Type.PlunderInterrupt) || !flag)
			{
				if (this.m_PlunderProgress != null)
				{
					this.m_PlunderProgress.gameObject.SetActive(false);
				}
				return;
			}
			if (this.m_PlunderProgress != null)
			{
				this.m_PlunderProgress.gameObject.SetActive(true);
			}
			if (this.m_PlunderProgress == null)
			{
				return;
			}
			if (this.logic.def.duration <= 0f)
			{
				this.m_PlunderProgress.gameObject.SetActive(false);
				return;
			}
			this.m_PlunderProgress.fillAmount = 1f - this.logic.PlunderProgress();
		}

		// Token: 0x060049B0 RID: 18864 RVA: 0x0021D959 File Offset: 0x0021BB59
		private void Update()
		{
			this.UpdateProgress();
		}

		// Token: 0x060049B1 RID: 18865 RVA: 0x0021D961 File Offset: 0x0021BB61
		protected override void AddListeners()
		{
			base.AddListeners();
			if (this.m_RefuseSupport != null)
			{
				this.m_RefuseSupport.onClick = new BSGButton.OnClick(base.RefuseSupport);
			}
		}

		// Token: 0x060049B2 RID: 18866 RVA: 0x0021D98E File Offset: 0x0021BB8E
		protected override void RemoveListeners()
		{
			base.RemoveListeners();
			if (this.m_RefuseSupport != null)
			{
				this.m_RefuseSupport.onClick = null;
			}
		}

		// Token: 0x060049B3 RID: 18867 RVA: 0x0021D9B0 File Offset: 0x0021BBB0
		private void UpdateTexts()
		{
			global::Battle battle = this.logic.visuals as global::Battle;
			Vars vars = (battle != null) ? battle.Vars() : null;
			if (this.m_Name != null && vars != null)
			{
				UIText.SetTextKey(this.m_Name, vars.Get<string>("BATTLE", null), vars, null);
			}
			bool flag = this.logic.winner == this.logic_sides[0];
			if (this.m_State != null)
			{
				UIText.SetTextKey(this.m_State, flag ? "Battle.Victory.Caption" : "Battle.Defeat.Caption", null, null);
				this.m_State.color = global::Battle.GetEstimationColor(flag ? "winning_decisively" : "losing_badly");
			}
			if (this.m_Description != null)
			{
				UIText.SetTextKey(this.m_Description, flag ? "Battle.Victory.Description" : "Battle.Defeat.Description", base.GetBattleVars(this.logic), null);
			}
		}

		// Token: 0x04003830 RID: 14384
		[UIFieldTarget("id_Name")]
		private TextMeshProUGUI m_Name;

		// Token: 0x04003831 RID: 14385
		[UIFieldTarget("id_State")]
		private TextMeshProUGUI m_State;

		// Token: 0x04003832 RID: 14386
		[UIFieldTarget("id_Description")]
		private TextMeshProUGUI m_Description;

		// Token: 0x04003833 RID: 14387
		[UIFieldTarget("id_RefuseSupport")]
		private BSGButton m_RefuseSupport;

		// Token: 0x04003834 RID: 14388
		[UIFieldTarget("id_estimation")]
		private UIBattleEstimationBar m_EstimationBar;

		// Token: 0x04003835 RID: 14389
		[UIFieldTarget("id_PlunderProgressContainer")]
		private GameObject m_PlunderProgressContianer;

		// Token: 0x04003836 RID: 14390
		[UIFieldTarget("id_PlunderProgress")]
		private Image m_PlunderProgress;
	}

	// Token: 0x02000714 RID: 1812
	internal class Siege : UIBattleWindow.BattleStage
	{
		// Token: 0x060049B5 RID: 18869 RVA: 0x0021DA9F File Offset: 0x0021BC9F
		public override void Refresh()
		{
			if (this.logic == null)
			{
				return;
			}
			this.UpdateLiftSiege();
			base.UpdateShield();
			this.UpdateTexts();
			base.UpdateEstimation();
			this.UpdateSiegeBars();
			this.UpdateGarrison();
			this.UpdateAssaultButton();
			base.UpdateArmies();
		}

		// Token: 0x060049B6 RID: 18870 RVA: 0x0021DADC File Offset: 0x0021BCDC
		private void Update()
		{
			this.UpdateSiegeBars();
			if (UIBattleWindow.PlayerHasControl(this.logic))
			{
				bool flag = this.logic.CanAssault();
				if (flag != this.can_assault)
				{
					this.can_assault = flag;
					this.UpdateAssaultButton();
				}
				return;
			}
			BSGButton assault = this.m_Assault;
			if (assault == null)
			{
				return;
			}
			assault.gameObject.SetActive(false);
		}

		// Token: 0x060049B7 RID: 18871 RVA: 0x0021D265 File Offset: 0x0021B465
		public override void OnArmiesChanged()
		{
			base.OnArmiesChanged();
			base.UpdateArmies();
		}

		// Token: 0x060049B8 RID: 18872 RVA: 0x0021DB38 File Offset: 0x0021BD38
		private void UpdateLiftSiege()
		{
			Vars battleVars = base.GetBattleVars(this.logic);
			if (this.logic.type == Logic.Battle.Type.Siege)
			{
				Tooltip.Get(this.m_LiftSiege.gameObject, true).SetText("Battle.Siege.Retreat", null, battleVars);
				BSGButton liftSiege = this.m_LiftSiege;
				if (liftSiege == null)
				{
					return;
				}
				liftSiege.gameObject.SetActive(global::Battle.PlayerIsAttacker(this.logic, false));
			}
		}

		// Token: 0x060049B9 RID: 18873 RVA: 0x0021DBA0 File Offset: 0x0021BDA0
		private void UpdateAssaultButton()
		{
			if (this.m_Assault)
			{
				Tooltip.Get(this.m_Assault.gameObject, true).SetText(this.can_assault ? "Battle.Siege.Assault" : "Battle.Siege.Assault_Disabled", null, this.vars);
				this.m_Assault.Enable(this.can_assault, false);
			}
		}

		// Token: 0x060049BA RID: 18874 RVA: 0x0021DC00 File Offset: 0x0021BE00
		protected override void Init()
		{
			base.Init();
			if (this.m_Name != null)
			{
				UIBattleWindow.SetEstimationTooltip(this.m_Name.gameObject, this.logic);
			}
			if (this.m_SiegeLabel != null)
			{
				UIBattleWindow.SetEstimationTooltip(this.m_SiegeLabel.gameObject, this.logic);
			}
			this.castle = (this.logic.settlement as Castle);
			this.vars = base.GetBattleVars(this.logic);
			if (this.logic.settlement == null || this.logic.settlement.keep_effects == null || this.logic.settlement.keep_effects.CanBeAssaulted())
			{
				if (this.m_BreakSiege)
				{
					Tooltip.Get(this.m_BreakSiege.gameObject, true).SetText("Battle.Siege.BreakSiege", null, this.vars);
				}
				if (this.m_Assault)
				{
					Tooltip.Get(this.m_Assault.gameObject, true).SetText("Battle.Siege.Assault_Disabled", null, this.vars);
				}
			}
			else
			{
				if (this.m_BreakSiege)
				{
					this.m_BreakSiege.gameObject.SetActive(false);
				}
				if (this.m_Assault)
				{
					this.m_Assault.gameObject.SetActive(false);
				}
				this.m_BreakSiege = null;
				this.m_Assault = null;
			}
			if (this.m_RefuseSupport != null)
			{
				Tooltip.Get(this.m_RefuseSupport.gameObject, true).SetText("Battle.Siege.RefuseSupport", null, this.vars);
				this.m_RefuseSupport.gameObject.SetActive(false);
			}
			if (this.m_LiftSiege != null)
			{
				this.m_LiftSiege.onClick = new BSGButton.OnClick(base.Retreat);
			}
			if (this.m_DefenceText != null)
			{
				Tooltip.Get(this.m_DefenceText.transform.parent.gameObject, true).SetText("Battle.Siege.CastleDefenderBonus", null, this.vars);
			}
			if (UIBattleWindow.PlayerHasControl(this.logic))
			{
				Logic.Kingdom kingdom = BaseUI.LogicKingdom();
				bool flag = this.logic.attacker_kingdom.id == kingdom.id;
				Logic.Army attacker_support = this.logic.attacker_support;
				bool flag2 = ((attacker_support != null) ? attacker_support.GetKingdom().id : 0) == kingdom.id;
				BSGButton breakSiege = this.m_BreakSiege;
				if (breakSiege != null)
				{
					breakSiege.gameObject.SetActive(!flag && !flag2 && (this.logic.defenders.Count > 0 || this.logic.settlement.garrison.GetManPower() > 0));
				}
				BSGButton assault = this.m_Assault;
				if (assault != null)
				{
					assault.gameObject.SetActive(flag);
				}
			}
			else
			{
				BSGButton breakSiege2 = this.m_BreakSiege;
				if (breakSiege2 != null)
				{
					breakSiege2.gameObject.SetActive(false);
				}
				BSGButton assault2 = this.m_Assault;
				if (assault2 != null)
				{
					assault2.gameObject.SetActive(false);
				}
			}
			if (this.castle != null)
			{
				if (this.m_CastleFoodText != null)
				{
					this.m_CastleFoodText.text = this.castle.GetFoodStorage().ToString("F0");
				}
				if (this.m_CastleFood != null)
				{
					this.m_CastleFood.SetActive(true);
				}
				if (this.m_FoodBar != null)
				{
					this.m_FoodBar.SetActive(true);
					return;
				}
			}
			else
			{
				if (this.m_CastleFood != null)
				{
					this.m_CastleFood.SetActive(false);
				}
				if (this.m_AttackerArmyFoodText != null)
				{
					this.m_AttackerArmyFoodText.gameObject.SetActive(false);
				}
				if (this.m_FoodBar != null)
				{
					this.m_FoodBar.SetActive(false);
				}
			}
		}

		// Token: 0x060049BB RID: 18875 RVA: 0x0021DFAC File Offset: 0x0021C1AC
		protected override string GetRetreatMessageKey()
		{
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			Logic.Army attacker_support = this.logic.attacker_support;
			if (((attacker_support != null) ? attacker_support.GetKingdom().id : 0) == kingdom.id)
			{
				return "LiftSiegeSupporterMessage";
			}
			return "LiftSiegeMessage";
		}

		// Token: 0x060049BC RID: 18876 RVA: 0x0021DFF0 File Offset: 0x0021C1F0
		protected override void AddListeners()
		{
			base.AddListeners();
			if (this.m_BreakSiege != null)
			{
				this.m_BreakSiege.onClick = new BSGButton.OnClick(this.FightSiege);
			}
			if (this.m_Assault != null)
			{
				this.m_Assault.onClick = new BSGButton.OnClick(this.FightSiege);
			}
			if (this.m_RefuseSupport != null)
			{
				this.m_RefuseSupport.onClick = new BSGButton.OnClick(base.RefuseSupport);
			}
		}

		// Token: 0x060049BD RID: 18877 RVA: 0x0021E074 File Offset: 0x0021C274
		protected override void RemoveListeners()
		{
			base.RemoveListeners();
			if (this.m_BreakSiege != null)
			{
				this.m_BreakSiege.onClick = null;
			}
			if (this.m_Assault != null)
			{
				this.m_Assault.onClick = null;
			}
			if (this.m_RefuseSupport != null)
			{
				this.m_RefuseSupport.onClick = null;
			}
		}

		// Token: 0x060049BE RID: 18878 RVA: 0x0021E0D8 File Offset: 0x0021C2D8
		private void UpdateTexts()
		{
			global::Battle battle = this.logic.visuals as global::Battle;
			Vars vars = (battle != null) ? battle.Vars() : null;
			if (this.m_Name != null && vars != null)
			{
				UIText.SetTextKey(this.m_Name, vars.Get<string>("BATTLE", null), vars, null);
			}
			bool flag = this.logic.winner == this.logic_sides[0];
			if (this.m_State != null)
			{
				UIText.SetTextKey(this.m_State, flag ? "Battle.Victory.Caption" : "Battle.Defeat.Caption", null, null);
				this.m_State.color = global::Battle.GetEstimationColor(flag ? "winning_decisively" : "losing_badly");
			}
			Vars battleVars = base.GetBattleVars(this.logic);
			if (this.m_Description != null)
			{
				UIText.SetTextKey(this.m_Description, flag ? "Battle.Victory.Description" : "Battle.Defeat.Description", battleVars, null);
			}
			if (this.m_SiegeLabel != null)
			{
				UIText.SetTextKey(this.m_SiegeLabel, "Battle.Siege.label", battleVars, null);
			}
		}

		// Token: 0x060049BF RID: 18879 RVA: 0x000023FD File Offset: 0x000005FD
		private void UpdateGarrison()
		{
		}

		// Token: 0x060049C0 RID: 18880 RVA: 0x0021E1EC File Offset: 0x0021C3EC
		private void SelectGarrison()
		{
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_units" + this.logic_sides[1], true, true);
			if (gameObject == null)
			{
				return;
			}
			if (this.castle.garrison == null || this.castle.garrison.units == null)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < Logic.Army.battle_cols; j++)
				{
					UIUnitSlot uiunitSlot = global::Common.FindChildComponent<UIUnitSlot>(gameObject, string.Concat(new object[]
					{
						"unit_",
						i,
						"_",
						j
					}));
					if (!(uiunitSlot == null))
					{
						Logic.Unit unit = (this.castle.garrison.units.Count > num) ? this.castle.garrison.units[num] : null;
						uiunitSlot.SetUnitInstance(unit, -1, null, this.castle);
						num++;
					}
				}
			}
		}

		// Token: 0x060049C1 RID: 18881 RVA: 0x0021E2F9 File Offset: 0x0021C4F9
		private void UpdateSiegeBars()
		{
			Logic.Battle logic = this.logic;
		}

		// Token: 0x060049C2 RID: 18882 RVA: 0x0021E304 File Offset: 0x0021C504
		private void FightSiege(BSGButton button)
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null || this.logic == null)
			{
				return;
			}
			if (worldUI.kingdom == this.logic.attacker_kingdom.id)
			{
				this.logic.DoAction("assault", 0, "");
				return;
			}
			if (worldUI.kingdom == this.logic.defender_kingdom.id)
			{
				this.logic.DoAction("break_siege", 0, "");
			}
		}

		// Token: 0x04003837 RID: 14391
		[UIFieldTarget("id_Name")]
		private TextMeshProUGUI m_Name;

		// Token: 0x04003838 RID: 14392
		[UIFieldTarget("id_State")]
		private TextMeshProUGUI m_State;

		// Token: 0x04003839 RID: 14393
		[UIFieldTarget("id_Description")]
		private TextMeshProUGUI m_Description;

		// Token: 0x0400383A RID: 14394
		[UIFieldTarget("id_SiegeLabel")]
		private TextMeshProUGUI m_SiegeLabel;

		// Token: 0x0400383B RID: 14395
		[UIFieldTarget("id_BreakSiege")]
		private BSGButton m_BreakSiege;

		// Token: 0x0400383C RID: 14396
		[UIFieldTarget("id_Assault")]
		private BSGButton m_Assault;

		// Token: 0x0400383D RID: 14397
		[UIFieldTarget("id_RefuseSupport")]
		private BSGButton m_RefuseSupport;

		// Token: 0x0400383E RID: 14398
		[UIFieldTarget("id_LiftSiege")]
		protected BSGButton m_LiftSiege;

		// Token: 0x0400383F RID: 14399
		[UIFieldTarget("id_FoodBar")]
		private GameObject m_FoodBar;

		// Token: 0x04003840 RID: 14400
		[UIFieldTarget("id_DefenceLabel")]
		private TextMeshProUGUI m_DefenceText;

		// Token: 0x04003841 RID: 14401
		[UIFieldTarget("id_CastleFood")]
		private GameObject m_CastleFood;

		// Token: 0x04003842 RID: 14402
		[UIFieldTarget("id_CastleFoodLabel")]
		private TextMeshProUGUI m_CastleFoodText;

		// Token: 0x04003843 RID: 14403
		[UIFieldTarget("id_AttackerArmyFood")]
		private TextMeshProUGUI m_AttackerArmyFoodText;

		// Token: 0x04003844 RID: 14404
		private Castle castle;

		// Token: 0x04003845 RID: 14405
		private bool can_assault;
	}

	// Token: 0x02000715 RID: 1813
	internal class ArmyLeaderSlot : MonoBehaviour
	{
		// Token: 0x060049C4 RID: 18884 RVA: 0x0021E390 File Offset: 0x0021C590
		public void SetData(object army, Action<UIBattleWindow.ArmyLeaderSlot> action, bool showCrest)
		{
			this.Army = army;
			this.m_Action = action;
			this.m_showCrest = showCrest;
			UICommon.FindComponents(this, false);
			if (army is Logic.Army)
			{
				Logic.Army army2 = army as Logic.Army;
				if (army2.leader != null)
				{
					Logic.Rebel rebel = army2.rebel;
					if (rebel == null || !rebel.IsRegular())
					{
						this.BuildAsArmy(army2.leader);
						goto IL_75;
					}
				}
				this.BuildAsLeaderless(army2);
			}
			else if (army is Garrison)
			{
				this.BuildAsGarrison(army as Garrison);
			}
			IL_75:
			this.UpdateHighlights();
		}

		// Token: 0x060049C5 RID: 18885 RVA: 0x0021E418 File Offset: 0x0021C618
		private void BuildAsArmy(Logic.Character leader)
		{
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Icon", true, true);
			if (gameObject != null)
			{
				Vars vars = new Vars();
				vars.Set<string>("variant", "compact");
				GameObject icon2 = ObjectIcon.GetIcon(leader, vars, gameObject.transform as RectTransform);
				UICharacterIcon icon = icon2.GetComponent<UICharacterIcon>();
				if (icon != null)
				{
					icon.OnSelect += delegate(UICharacterIcon x)
					{
						if (UICommon.GetKey(KeyCode.LeftShift, false) || UICommon.GetKey(KeyCode.RightShift, false))
						{
							icon.ExecuteDefaultSelectAction();
							return;
						}
						this.m_Action(this);
					};
					icon.ShowCrest(this.m_showCrest);
					icon.ShowStatus(false);
				}
			}
		}

		// Token: 0x060049C6 RID: 18886 RVA: 0x0021E4D0 File Offset: 0x0021C6D0
		private void BuildAsLeaderless(Logic.Army army)
		{
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Icon", true, true);
			if (gameObject != null)
			{
				UIKingdomIcon component = ObjectIcon.GetIcon(army.GetKingdom(), null, gameObject.transform as RectTransform).GetComponent<UIKingdomIcon>();
				if (component != null)
				{
					component.GetPrimary().onClick = delegate(PointerEventData e, KingdomShield s)
					{
						this.m_Action(this);
						return true;
					};
				}
			}
		}

		// Token: 0x060049C7 RID: 18887 RVA: 0x0021E53C File Offset: 0x0021C73C
		private void BuildAsGarrison(Garrison garrison)
		{
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Icon", true, true);
			if (gameObject != null)
			{
				GameObject prefab = UICommon.GetPrefab("GarrisonIcon", null);
				if (prefab != null)
				{
					Tooltip.Get(UIGenericActionIcon.Create(delegate
					{
						this.m_Action(this);
					}, prefab, gameObject.transform as RectTransform, null).gameObject, true).SetText("Battle.Siege.Garisson", null, null);
				}
			}
		}

		// Token: 0x060049C8 RID: 18888 RVA: 0x0021E5AF File Offset: 0x0021C7AF
		public void Select(bool selected)
		{
			this.m_Selected = selected;
			this.UpdateHighlights();
		}

		// Token: 0x060049C9 RID: 18889 RVA: 0x0021E5BE File Offset: 0x0021C7BE
		private void UpdateHighlights()
		{
			GameObject selectedBorder = this.m_SelectedBorder;
			if (selectedBorder == null)
			{
				return;
			}
			selectedBorder.gameObject.SetActive(this.m_Selected);
		}

		// Token: 0x060049CA RID: 18890 RVA: 0x0021E5DB File Offset: 0x0021C7DB
		public static UIBattleWindow.ArmyLeaderSlot Create(object army, GameObject prototype, RectTransform parent, Action<UIBattleWindow.ArmyLeaderSlot> action, bool showCrest = false)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			UIBattleWindow.ArmyLeaderSlot armyLeaderSlot = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent).AddComponent<UIBattleWindow.ArmyLeaderSlot>();
			armyLeaderSlot.SetData(army, action, showCrest);
			return armyLeaderSlot;
		}

		// Token: 0x04003846 RID: 14406
		[UIFieldTarget("id_Selected")]
		private GameObject m_SelectedBorder;

		// Token: 0x04003847 RID: 14407
		public object Army;

		// Token: 0x04003848 RID: 14408
		private Action<UIBattleWindow.ArmyLeaderSlot> m_Action;

		// Token: 0x04003849 RID: 14409
		private bool m_Selected;

		// Token: 0x0400384A RID: 14410
		private bool m_showCrest;
	}

	// Token: 0x02000716 RID: 1814
	internal class TerrainBonusSlot : MonoBehaviour
	{
		// Token: 0x060049CE RID: 18894 RVA: 0x0021E630 File Offset: 0x0021C830
		public void SetData(BattleBonus.Def def)
		{
			this.Def = def;
			UICommon.FindComponents(this, false);
			Tooltip.Get(this.m_Icon.gameObject, true).SetDef("TerrainBonusTooltip", new Vars(def));
			this.m_Icon.sprite = def.field.GetValue("icon", null, true, true, true, '.').Get<Sprite>();
		}

		// Token: 0x060049CF RID: 18895 RVA: 0x0021E69A File Offset: 0x0021C89A
		public static UIBattleWindow.TerrainBonusSlot Create(BattleBonus.Def def, GameObject prototype, RectTransform parent)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			UIBattleWindow.TerrainBonusSlot terrainBonusSlot = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent).AddComponent<UIBattleWindow.TerrainBonusSlot>();
			terrainBonusSlot.SetData(def);
			return terrainBonusSlot;
		}

		// Token: 0x0400384B RID: 14411
		[UIFieldTarget("id_Icon")]
		protected Image m_Icon;

		// Token: 0x0400384C RID: 14412
		public BattleBonus.Def Def;
	}
}

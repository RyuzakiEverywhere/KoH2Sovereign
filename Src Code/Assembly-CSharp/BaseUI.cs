using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Logic;
using Logic.ExtensionMethods;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001AE RID: 430
public class BaseUI : MonoBehaviour
{
	// Token: 0x17000142 RID: 322
	// (get) Token: 0x060018C8 RID: 6344 RVA: 0x000F2AB4 File Offset: 0x000F0CB4
	public global::Kingdom.ID kingdom
	{
		get
		{
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom == null)
			{
				return 0;
			}
			return kingdom.id;
		}
	}

	// Token: 0x060018C9 RID: 6345 RVA: 0x000F2ADC File Offset: 0x000F0CDC
	public static bool CanControlAI()
	{
		return BaseUI.control_ai && Game.CheckCheatLevel(Game.CheatLevel.High, "control_ai", false);
	}

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x060018CA RID: 6346 RVA: 0x000F2AF3 File Offset: 0x000F0CF3
	// (set) Token: 0x060018CB RID: 6347 RVA: 0x000F2AFB File Offset: 0x000F0CFB
	public FiltersEventHub FiltersEventHub { get; private set; } = new FiltersEventHub();

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x060018CC RID: 6348 RVA: 0x000F2B04 File Offset: 0x000F0D04
	public Vector3 ptLookAt
	{
		get
		{
			if (!(CameraController.GameCamera != null))
			{
				return new Vector3(0f, 0f, 0f);
			}
			return CameraController.GameCamera.GetLookAtPoint();
		}
	}

	// Token: 0x060018CD RID: 6349 RVA: 0x000F2B34 File Offset: 0x000F0D34
	public static void LoadNeverPlayTogether()
	{
		BaseUI.never_play_together.Clear();
		if (BaseUI.soundsDef == null)
		{
			return;
		}
		DT.Field field = BaseUI.soundsDef.FindChild("never_play_together", null, true, true, true, '.');
		if (((field != null) ? field.children : null) != null && field.children.Count > 0)
		{
			for (int i = 0; i < field.children.Count; i++)
			{
				DT.Field field2 = field.children[i];
				if (!string.IsNullOrEmpty(field2.key))
				{
					BaseUI.NeverPlayTogether neverPlayTogether = new BaseUI.NeverPlayTogether();
					neverPlayTogether.Load(field2);
					BaseUI.never_play_together.Add(neverPlayTogether);
				}
			}
		}
	}

	// Token: 0x1400000F RID: 15
	// (add) Token: 0x060018CE RID: 6350 RVA: 0x000F2BD0 File Offset: 0x000F0DD0
	// (remove) Token: 0x060018CF RID: 6351 RVA: 0x000F2C08 File Offset: 0x000F0E08
	public event Action OnKeyPressed;

	// Token: 0x14000010 RID: 16
	// (add) Token: 0x060018D0 RID: 6352 RVA: 0x000F2C40 File Offset: 0x000F0E40
	// (remove) Token: 0x060018D1 RID: 6353 RVA: 0x000F2C78 File Offset: 0x000F0E78
	public event Action OnKeyReleased;

	// Token: 0x17000145 RID: 325
	// (get) Token: 0x060018D2 RID: 6354 RVA: 0x000F2CAD File Offset: 0x000F0EAD
	public string picked_element_path
	{
		get
		{
			return global::Common.ObjPath(this.picked_UI_element);
		}
	}

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x060018D3 RID: 6355 RVA: 0x000F2CBA File Offset: 0x000F0EBA
	public Tooltip main_tooltip
	{
		get
		{
			Tooltip tooltip = this.tooltip;
			Tooltip result;
			if ((result = ((tooltip != null) ? tooltip.main_tooltip : null)) == null)
			{
				Tooltip tooltip2 = this.pinned_tooltip;
				if (tooltip2 == null)
				{
					return null;
				}
				result = tooltip2.main_tooltip;
			}
			return result;
		}
	}

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x060018D4 RID: 6356 RVA: 0x000F2CE3 File Offset: 0x000F0EE3
	public string tooltip_obj_path
	{
		get
		{
			Tooltip main_tooltip = this.main_tooltip;
			return global::Common.ObjPath((main_tooltip != null) ? main_tooltip.gameObject : null);
		}
	}

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x060018D5 RID: 6357 RVA: 0x000F2CFC File Offset: 0x000F0EFC
	public string tooltip_object_to_string
	{
		get
		{
			return Logic.Object.ToString(BaseUI.TTObj());
		}
	}

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x060018D6 RID: 6358 RVA: 0x000F2D08 File Offset: 0x000F0F08
	public MessageWnd message_wnd
	{
		get
		{
			return this.window_dispatcher.GetWindow<MessageWnd>();
		}
	}

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x060018D7 RID: 6359 RVA: 0x000F2D15 File Offset: 0x000F0F15
	public MessageWnd tutorial_message
	{
		get
		{
			return Tutorial.cur_message_wnd;
		}
	}

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x060018D8 RID: 6360 RVA: 0x000F2D1C File Offset: 0x000F0F1C
	public string drag_source
	{
		get
		{
			Hotspot drag_source = Hotspot.drag_source;
			return global::Common.ObjPath((drag_source != null) ? drag_source.gameObject : null);
		}
	}

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x060018D9 RID: 6361 RVA: 0x000F2D34 File Offset: 0x000F0F34
	public string drop_target
	{
		get
		{
			Hotspot drop_target = Hotspot.drop_target;
			return global::Common.ObjPath((drop_target != null) ? drop_target.gameObject : null);
		}
	}

	// Token: 0x1700014D RID: 333
	// (get) Token: 0x060018DA RID: 6362 RVA: 0x000F2D4C File Offset: 0x000F0F4C
	public string drag_and_drop_operation
	{
		get
		{
			return Hotspot.drag_and_drop_operation;
		}
	}

	// Token: 0x1700014E RID: 334
	// (get) Token: 0x060018DB RID: 6363 RVA: 0x000F2D53 File Offset: 0x000F0F53
	public List<Logic.Unit> selected_units
	{
		get
		{
			return UIUnitSlot.selection;
		}
	}

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x060018DC RID: 6364 RVA: 0x000F2D5A File Offset: 0x000F0F5A
	public int num_selected_units
	{
		get
		{
			return UIUnitSlot.selection.Count;
		}
	}

	// Token: 0x17000150 RID: 336
	// (get) Token: 0x060018DD RID: 6365 RVA: 0x000F2D66 File Offset: 0x000F0F66
	public UIWikiWindow wiki_wnd
	{
		get
		{
			return UIWikiWindow.Get();
		}
	}

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x060018DE RID: 6366 RVA: 0x000F2D6D File Offset: 0x000F0F6D
	public List<UIWikiWindow.HistoryItem> wiki_history
	{
		get
		{
			return UIWikiWindow.history;
		}
	}

	// Token: 0x17000152 RID: 338
	// (get) Token: 0x060018DF RID: 6367 RVA: 0x000F2D74 File Offset: 0x000F0F74
	// (set) Token: 0x060018E0 RID: 6368 RVA: 0x000F2D7C File Offset: 0x000F0F7C
	[HideInInspector]
	public UIInGameChat in_game_chat { get; set; }

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x060018E1 RID: 6369 RVA: 0x000F2D85 File Offset: 0x000F0F85
	// (set) Token: 0x060018E2 RID: 6370 RVA: 0x000F2D8D File Offset: 0x000F0F8D
	public GameObject selection_container { get; protected set; }

	// Token: 0x17000154 RID: 340
	// (get) Token: 0x060018E3 RID: 6371 RVA: 0x000F2D96 File Offset: 0x000F0F96
	// (set) Token: 0x060018E4 RID: 6372 RVA: 0x000F2D9E File Offset: 0x000F0F9E
	public GameObject message_container { get; protected set; }

	// Token: 0x17000155 RID: 341
	// (get) Token: 0x060018E5 RID: 6373 RVA: 0x000F2DA7 File Offset: 0x000F0FA7
	// (set) Token: 0x060018E6 RID: 6374 RVA: 0x000F2DAF File Offset: 0x000F0FAF
	public GameObject system_message_container { get; protected set; }

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x060018E7 RID: 6375 RVA: 0x000F2DB8 File Offset: 0x000F0FB8
	// (set) Token: 0x060018E8 RID: 6376 RVA: 0x000F2DC0 File Offset: 0x000F0FC0
	public GameObject tutorial_message_container { get; protected set; }

	// Token: 0x060018E9 RID: 6377 RVA: 0x000F2DCC File Offset: 0x000F0FCC
	public static BaseUI Get()
	{
		if (BaseUI.current == null)
		{
			BaseUI.FindUnregistered();
		}
		if (BaseUI.sm_Active.Count > 0)
		{
			BaseUI.current = BaseUI.sm_Active[BaseUI.sm_Active.Count - 1];
		}
		return BaseUI.current;
	}

	// Token: 0x060018EA RID: 6378 RVA: 0x000F2E18 File Offset: 0x000F1018
	public void DoubleClickCheck()
	{
		float unscaledTime = UnityEngine.Time.unscaledTime;
		Vector3 mousePosition = Input.mousePosition;
		this.dblclk = (unscaledTime - this.btn_down_time < this.dblclk_delay && Vector2.Distance(mousePosition, this.mouse_click_last_pos) < this.dblclk_max_dist);
		this.btn_down_time = unscaledTime;
		this.mouse_click_last_pos = mousePosition;
	}

	// Token: 0x060018EB RID: 6379 RVA: 0x000F2E78 File Offset: 0x000F1078
	protected static bool Register(BaseUI ui)
	{
		if (ui == null)
		{
			return false;
		}
		for (int i = 0; i < BaseUI.sm_Active.Count; i++)
		{
			if (!(BaseUI.sm_Active[i] == null))
			{
				if (BaseUI.sm_Active[i] == ui)
				{
					return false;
				}
				if (BaseUI.sm_Active[i].GetType() == ui.GetType())
				{
					UnityEngine.Debug.LogWarning("No more than one UI per type can be active. Ignoring...");
					return false;
				}
			}
		}
		BaseUI.sm_Active.Add(ui);
		BaseUI.current = ui;
		return true;
	}

	// Token: 0x060018EC RID: 6380 RVA: 0x000F2F08 File Offset: 0x000F1108
	protected static void UnRegister(BaseUI ui)
	{
		if (BaseUI.sm_Active.Remove(ui) && BaseUI.current == ui)
		{
			BaseUI.current = ((BaseUI.sm_Active.Count > 0) ? BaseUI.sm_Active[BaseUI.sm_Active.Count - 1] : null);
		}
	}

	// Token: 0x060018ED RID: 6381 RVA: 0x000F2F5A File Offset: 0x000F115A
	protected virtual void OnEnable()
	{
		BaseUI.Register(this);
	}

	// Token: 0x060018EE RID: 6382 RVA: 0x000F2F63 File Offset: 0x000F1163
	protected virtual void OnDisable()
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			BaseUI.UnRegister(this);
		}
	}

	// Token: 0x060018EF RID: 6383 RVA: 0x000F2F7C File Offset: 0x000F117C
	private void OnApplicationFocus(bool hasFocus)
	{
		if (!UserSettings.PauseOnOutOfFocus)
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null || !game.IsRunning())
		{
			return;
		}
		if (hasFocus)
		{
			game.pause.DelRequest("AltTabPause", -2);
			return;
		}
		game.pause.AddRequest("AltTabPause", -2);
	}

	// Token: 0x060018F0 RID: 6384 RVA: 0x000F2FCC File Offset: 0x000F11CC
	private void OnApplicationPause(bool pauseStatus)
	{
		this.OnApplicationFocus(!pauseStatus);
	}

	// Token: 0x060018F1 RID: 6385 RVA: 0x000F2FD8 File Offset: 0x000F11D8
	public void ClosePanels()
	{
		if (this.tCanvas != null)
		{
			if (this.message_container != null)
			{
				UICommon.DeleteChildren(this.message_container.transform);
			}
			if (this.tutorial_message_container != null)
			{
				UICommon.DeleteChildren(this.tutorial_message_container.transform);
			}
		}
		this.SelectObj(null, false, true, true, true);
	}

	// Token: 0x060018F2 RID: 6386 RVA: 0x000F303A File Offset: 0x000F123A
	protected virtual void OnDestroy()
	{
		BaseUI.UnRegister(this);
	}

	// Token: 0x060018F3 RID: 6387 RVA: 0x000F3044 File Offset: 0x000F1244
	public static T Get<T>() where T : BaseUI
	{
		for (int i = 0; i < BaseUI.sm_Active.Count; i++)
		{
			if (!(BaseUI.sm_Active[i] == null) && BaseUI.sm_Active[i] is T)
			{
				return BaseUI.sm_Active[i] as T;
			}
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("UIRoot");
		if (array != null && array.Length != 0)
		{
			for (int j = 0; j < array.Length; j++)
			{
				T component = array[j].GetComponent<T>();
				if (BaseUI.Register(component))
				{
					return component;
				}
			}
		}
		return default(T);
	}

	// Token: 0x060018F4 RID: 6388 RVA: 0x000F30E4 File Offset: 0x000F12E4
	private static void FindUnregistered()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("UIRoot");
		if (array != null && array.Length != 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				BaseUI.Register(array[i].GetComponent<BaseUI>());
			}
		}
	}

	// Token: 0x060018F5 RID: 6389 RVA: 0x000F3120 File Offset: 0x000F1320
	public static void LoadDefs()
	{
		for (int i = 0; i < BaseUI.sm_Active.Count; i++)
		{
			if (BaseUI.sm_Active[i] != null)
			{
				BaseUI baseUI = BaseUI.sm_Active[i];
				DT.Field defField = global::Defs.GetDefField("SelectionSettings", null);
				if (baseUI.selectionSettings == null)
				{
					baseUI.selectionSettings = new BaseUI.SelectionSettings();
				}
				baseUI.selectionSettings.Load(defField);
				DT.Field defField2 = global::Defs.GetDefField("TooltipSettings", null);
				if (baseUI.tooltipSettings == null)
				{
					baseUI.tooltipSettings = new BaseUI.TooltipSettings();
				}
				baseUI.tooltipSettings.Load(defField2);
				BaseUI.soundsDef = global::Defs.GetDefField("Sounds", null);
				baseUI.Refresh();
			}
		}
		BaseUI.LoadNeverPlayTogether();
	}

	// Token: 0x060018F6 RID: 6390 RVA: 0x000F31D8 File Offset: 0x000F13D8
	protected virtual void Start()
	{
		BaseUI.LoadDefs();
		Thread currentThread = Thread.CurrentThread;
		if (string.IsNullOrEmpty(currentThread.Name))
		{
			currentThread.Name = "Main thread";
		}
		this.tCanvas = base.transform.Find("Canvas");
		this.canvas = this.tCanvas.GetComponent<Canvas>();
		this.canvasScaler = this.tCanvas.GetComponent<CanvasScaler>();
		if (this.m_statusBar == null)
		{
			Transform transform = this.tCanvas.Find("id_StatusBars");
			this.m_statusBar = ((transform != null) ? transform.GetComponent<RectTransform>() : null);
		}
		this.raycaster = global::Common.GetComponent<GraphicRaycaster>(this.tCanvas, null);
		this.selection_container = global::Common.FindChildByName(base.gameObject, "id_SelectionUI", true, true);
		this.message_container = global::Common.FindChildByName(base.gameObject, "id_MessageContainer", true, true);
		this.system_message_container = global::Common.FindChildByName(base.gameObject, "id_SystemMessageContainer", true, true);
		this.minimap = global::Common.FindChildComponent<MiniMap>(base.gameObject, "id_Minimap");
		MiniMap miniMap = this.minimap;
		this.minimap_army_display = ((miniMap != null) ? miniMap.GetComponent<MinimapArmyDisplay>() : null);
		this.menu = global::Common.FindChildByName(this.tCanvas.gameObject, "id_Menu", false, false);
		this.tutorial_mouse_blocker = global::Common.FindChildByName(base.gameObject, "id_TutorialMouseBlocker", true, true);
		this.tutorial_message_container = global::Common.FindChildByName(base.gameObject, "id_TutorialMessageContainer", true, true);
		this.interaction_blocker = global::Common.FindChildByName(base.gameObject, "id_InteractionBlocker", true, true);
		this.layerMask = (1 << LayerMask.NameToLayer("Armies") | 1 << LayerMask.NameToLayer("Political"));
		DT.Field defField = global::Defs.GetDefField("Cursors", null);
		if (defField != null)
		{
			this.click_reticle_prefab = defField.GetRandomValue("click_reticle_prefab", null, true, true, true, '.').Get<GameObject>();
			this.reticle_duration = defField.GetFloat("reticle_duration", null, this.reticle_duration, true, true, true, '.');
			this.reticle_height_offset = defField.GetFloat("reticle_height_offset", null, this.reticle_height_offset, true, true, true, '.');
		}
		GameObject selection_container = this.selection_container;
		UICommon.SetActiveChildren((selection_container != null) ? selection_container.transform : null, false);
		FMODVoiceProvider.Register();
		this.audio = base.GetComponent<StudioEventEmitter>();
		IMGUIHandler.Get();
		this.unscaledTimeShaderProperyId = Shader.PropertyToID("_UnscaledGameTime");
	}

	// Token: 0x060018F7 RID: 6391 RVA: 0x000F3429 File Offset: 0x000F1629
	public void AddInteractionBlocker(string id)
	{
		this.m_InteractionBlockers.Add(id);
		this.UpdateInteractionBlocker();
	}

	// Token: 0x060018F8 RID: 6392 RVA: 0x000F343E File Offset: 0x000F163E
	public void RemoveInteractionBlocker(string id)
	{
		this.m_InteractionBlockers.Remove(id);
		this.UpdateInteractionBlocker();
	}

	// Token: 0x060018F9 RID: 6393 RVA: 0x000F3453 File Offset: 0x000F1653
	public void ClearInteractionBlockers()
	{
		this.m_InteractionBlockers.Clear();
		this.UpdateInteractionBlocker();
	}

	// Token: 0x060018FA RID: 6394 RVA: 0x000F3468 File Offset: 0x000F1668
	private void UpdateInteractionBlocker()
	{
		bool active = this.m_InteractionBlockers.Count > 0;
		this.interaction_blocker.gameObject.SetActive(active);
	}

	// Token: 0x060018FB RID: 6395 RVA: 0x000F3495 File Offset: 0x000F1695
	protected virtual void Update()
	{
		Shader.SetGlobalFloat(this.unscaledTimeShaderProperyId, UnityEngine.Time.unscaledTime);
		this.UpdateCanvasScaler();
		this.UpdatePicker();
		Tutorial.Update();
		this.UpdateInput();
		this.UpdateSoundQueue();
		FMODVoiceProvider.Update();
		CourtAdvice.Update();
	}

	// Token: 0x060018FC RID: 6396 RVA: 0x000F34CE File Offset: 0x000F16CE
	public void SpawnClickReticle()
	{
		if (this.picked_passable_area == 0)
		{
			this.SpawnClickReticle(this.picked_terrain_point, 0);
			return;
		}
		this.SpawnClickReticle(this.picked_passable_area_pos, this.picked_passable_area);
	}

	// Token: 0x060018FD RID: 6397 RVA: 0x000F34F8 File Offset: 0x000F16F8
	public void SpawnClickReticle(PPos pos)
	{
		if (pos.paID == 0)
		{
			this.SpawnClickReticle(global::Common.SnapToTerrain(pos, 0f, null, -1f, false), 0);
			return;
		}
		this.SpawnClickReticle(pos.Point3D(GameLogic.Get(false), 0f, 0f), pos.paID);
	}

	// Token: 0x060018FE RID: 6398 RVA: 0x000F3554 File Offset: 0x000F1754
	public void SpawnClickReticle(Vector3 pos, int passable_area)
	{
		bool flag = UnityEngine.Time.unscaledTime - this.btn_down_time > 0.25f;
		if (this.click_reticle_prefab != null && !flag)
		{
			GameObject gameObject = global::Common.Spawn(this.click_reticle_prefab, false, false);
			if (passable_area > 0)
			{
				gameObject.transform.position = pos;
				gameObject.transform.rotation = Quaternion.identity;
			}
			else
			{
				Vector3 terrainNormal = global::Common.GetTerrainNormal(pos, null);
				gameObject.transform.position = pos + terrainNormal * this.reticle_height_offset;
				gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, terrainNormal);
			}
			base.StartCoroutine(this.DestroyReticle(gameObject));
		}
	}

	// Token: 0x060018FF RID: 6399 RVA: 0x000F35FF File Offset: 0x000F17FF
	private IEnumerator DestroyReticle(GameObject reticle)
	{
		yield return new WaitForSecondsRealtime(this.reticle_duration);
		global::Common.DestroyObj(reticle);
		yield break;
	}

	// Token: 0x06001900 RID: 6400 RVA: 0x0002C538 File Offset: 0x0002A738
	public virtual bool OverrideEdgeScroll()
	{
		return false;
	}

	// Token: 0x06001901 RID: 6401 RVA: 0x000F3618 File Offset: 0x000F1818
	public int GetCurrentKingdomId()
	{
		Game game = GameLogic.Get(true);
		if (game != null)
		{
			Game game2 = game;
			int? num;
			if (game == null)
			{
				num = null;
			}
			else
			{
				Logic.Multiplayer multiplayer = game.multiplayer;
				if (multiplayer == null)
				{
					num = null;
				}
				else
				{
					Logic.Multiplayer.PlayerData playerData = multiplayer.playerData;
					num = ((playerData != null) ? new int?(playerData.kingdomId) : null);
				}
			}
			Logic.Kingdom kingdom = game2.GetKingdom(num ?? 0);
			if (kingdom != null)
			{
				return kingdom.id;
			}
		}
		return 0;
	}

	// Token: 0x06001902 RID: 6402 RVA: 0x000F3698 File Offset: 0x000F1898
	public static Logic.Kingdom LogicKingdom()
	{
		Game game = GameLogic.Get(false);
		if (((game != null) ? game.multiplayer : null) != null)
		{
			Logic.Kingdom localPlayerKingdom = game.GetLocalPlayerKingdom();
			if (localPlayerKingdom != null)
			{
				return localPlayerKingdom;
			}
		}
		return null;
	}

	// Token: 0x06001903 RID: 6403 RVA: 0x000F36C8 File Offset: 0x000F18C8
	public Vector3 GetTerrainSize()
	{
		if (this.vTerrainSize != Vector3.zero)
		{
			return this.vTerrainSize;
		}
		for (int i = 0; i < Terrain.activeTerrains.Length; i++)
		{
			Terrain terrain = Terrain.activeTerrains[i];
			Vector3 rhs = terrain.GetPosition() + terrain.terrainData.size;
			this.vTerrainSize = Vector3.Max(this.vTerrainSize, rhs);
		}
		return this.vTerrainSize;
	}

	// Token: 0x06001904 RID: 6404 RVA: 0x000F3738 File Offset: 0x000F1938
	public virtual void Refresh()
	{
		ObjectWindow[] componentsInChildren = base.transform.GetComponentsInChildren<ObjectWindow>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Refresh();
		}
	}

	// Token: 0x06001905 RID: 6405 RVA: 0x000F3767 File Offset: 0x000F1967
	public virtual GameObject GetSelectionObj(GameObject obj)
	{
		return obj;
	}

	// Token: 0x06001906 RID: 6406 RVA: 0x000F376A File Offset: 0x000F196A
	public bool IsSelected(GameObject obj)
	{
		return !(obj == null) && !(this.selected_obj == null) && this.GetSelectionObj(obj) == this.GetSelectionObj(this.selected_obj);
	}

	// Token: 0x06001907 RID: 6407 RVA: 0x000F379F File Offset: 0x000F199F
	public virtual void RefreshSelection(GameObject obj)
	{
		if (!this.IsSelected(obj))
		{
			return;
		}
		this.SelectObj(this.selected_obj, true, true, true, true);
	}

	// Token: 0x06001908 RID: 6408 RVA: 0x000F37BB File Offset: 0x000F19BB
	protected void SetSelectTarget(GameObject obj)
	{
		if (obj == this.select_target)
		{
			return;
		}
		this.select_target = obj;
	}

	// Token: 0x06001909 RID: 6409 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void SelectObj(GameObject obj, bool force_refresh = false, bool reload_view = true, bool clicked = true, bool play_sound = true)
	{
	}

	// Token: 0x0600190A RID: 6410 RVA: 0x000F37D4 File Offset: 0x000F19D4
	public virtual void SelectObjFromLogic(Logic.Object obj, bool force_refresh = false, bool reload_view = true)
	{
		GameLogic.Behaviour behaviour = ((obj != null) ? obj.visuals : null) as GameLogic.Behaviour;
		if (behaviour == null)
		{
			if (obj != null && obj.IsValid())
			{
				UnityEngine.Debug.Log("Null visual object for " + Logic.Object.Dump(obj));
			}
			this.SelectObj(null, force_refresh, reload_view, true, true);
			return;
		}
		this.SelectObj(behaviour.gameObject, force_refresh, reload_view, true, true);
	}

	// Token: 0x0600190B RID: 6411 RVA: 0x000F3838 File Offset: 0x000F1A38
	protected void SetSelectionPanel(GameObject prefab)
	{
		this.nextSelectionPanel = prefab;
		this.m_InvalidateSelection = true;
	}

	// Token: 0x0600190C RID: 6412 RVA: 0x000F3848 File Offset: 0x000F1A48
	protected virtual void LateUpdate()
	{
		if (this.m_InvalidateSelection)
		{
			this.UpdateSelectionPanel(this.nextSelectionPanel);
			this.nextSelectionPanel = null;
			this.m_InvalidateSelection = false;
		}
		WorldToScreenObject.UpdateTransforms();
	}

	// Token: 0x0600190D RID: 6413 RVA: 0x000F3874 File Offset: 0x000F1A74
	protected void UpdateSelectionPanel(GameObject prefab)
	{
		UITargetSelectWindow.last_sel = null;
		if (this.SelectionPanel != null && this.SelectionPanel.GetPrototype() == prefab)
		{
			this.SelectionPanel.StoreState();
			this.SelectionPanel.ValidateSelectionObject();
			return;
		}
		if (this.SelectionPanel != null && !this.SelectionPanel.IsDestoryed() && this.SelectionPanel.gameObject != null)
		{
			if (this.SelectionPanel.GetPrototype() == prefab)
			{
				this.SelectionPanel.StoreState();
			}
			string param = global::Common.ObjPath(this.SelectionPanel.gameObject);
			Tutorial.Rule.OnMessage(this.SelectionPanel.gameObject, "ui_window_hidden", param);
			if (this.SelectionPanel.PreserveWindow())
			{
				this.SelectionPanel.Release();
			}
			else
			{
				global::Common.DestroyObj(this.SelectionPanel.gameObject);
			}
		}
		this.SelectionPanel = null;
		if (prefab == null)
		{
			this.OnParentSelect();
			return;
		}
		if (this.selection_container == null)
		{
			this.OnParentSelect();
			return;
		}
		ObjectWindow freeWindow = ObjectWindow.GetFreeWindow(prefab);
		this.SelectionPanel = null;
		if (freeWindow != null)
		{
			this.SelectionPanel = freeWindow.GetComponent<BaseUI.ISelectionPanel>();
		}
		if (this.SelectionPanel == null)
		{
			this.SelectionPanel = global::Common.Spawn(prefab, this.selection_container.transform, false, "").GetComponent<BaseUI.ISelectionPanel>();
			if (this.SelectionPanel != null)
			{
				this.SelectionPanel.SetPrototype(prefab);
			}
		}
		if (this.SelectionPanel == null)
		{
			UnityEngine.Debug.Log("prefab: " + prefab.gameObject.name + " is missing a script with ISelectionPanel implementation ");
			this.OnParentSelect();
			return;
		}
		this.SelectionPanel.gameObject.SetActive(true);
		string param2 = global::Common.ObjPath(this.SelectionPanel.gameObject);
		Tutorial.Rule.OnMessage(this.SelectionPanel.gameObject, "ui_window_shown", param2);
		this.OnParentSelect();
	}

	// Token: 0x0600190E RID: 6414 RVA: 0x000023FD File Offset: 0x000005FD
	protected virtual void OnParentSelect()
	{
	}

	// Token: 0x0600190F RID: 6415 RVA: 0x000F3A50 File Offset: 0x000F1C50
	public virtual Color GetStanceColor(Logic.Object obj, bool primary = true)
	{
		Logic.Kingdom kingdom = GameLogic.Get(true).GetKingdom(this.kingdom);
		if (kingdom == null)
		{
			if (!primary)
			{
				return this.selectionSettings.secondaryNeutralColor;
			}
			return this.selectionSettings.neutralColor;
		}
		else
		{
			RelationUtils.Stance stance = kingdom.GetStance(obj);
			if (stance.IsWar())
			{
				if (!primary)
				{
					return this.selectionSettings.secondaryEnemyColor;
				}
				return this.selectionSettings.enemyColor;
			}
			else if (stance.IsOwn())
			{
				if (!primary)
				{
					return this.selectionSettings.secondaryFriendColor;
				}
				return this.selectionSettings.friendColor;
			}
			else if (stance.IsAnyVassalage())
			{
				if (!primary)
				{
					return this.selectionSettings.secondaryNeutralColor;
				}
				return this.selectionSettings.neutralColor;
			}
			else if (stance.IsAlliance())
			{
				if (!primary)
				{
					return this.selectionSettings.secondaryNeutralColor;
				}
				return this.selectionSettings.neutralColor;
			}
			else if (stance.IsNonAgression())
			{
				if (!primary)
				{
					return this.selectionSettings.secondaryNeutralColor;
				}
				return this.selectionSettings.neutralColor;
			}
			else if (stance.IsPeace())
			{
				if (!primary)
				{
					return this.selectionSettings.secondaryNeutralColor;
				}
				return this.selectionSettings.neutralColor;
			}
			else
			{
				if (!primary)
				{
					return this.selectionSettings.secondaryNeutralColor;
				}
				return this.selectionSettings.neutralColor;
			}
		}
	}

	// Token: 0x06001910 RID: 6416 RVA: 0x000F3B88 File Offset: 0x000F1D88
	public Material GetSelectionMaterial(GameObject obj, Material cur, bool primary = true, bool relation = false)
	{
		if (this.selectionSettings == null)
		{
			return null;
		}
		Material material = relation ? this.selectionSettings.armyRelationsMaterial : this.selectionSettings.armySelectionMaterial;
		global::Battle component = obj.GetComponent<global::Battle>();
		Color value;
		if (component != null)
		{
			value = (primary ? this.selectionSettings.neutralColor : this.selectionSettings.secondaryNeutralColor);
			if (component.logic != null)
			{
				if (component.logic.attacker_kingdom.id == this.kingdom.id)
				{
					value = (primary ? this.selectionSettings.friendColor : this.selectionSettings.secondaryFriendColor);
				}
				else if (component.logic.defender_kingdom.id == this.kingdom.id)
				{
					value = (primary ? this.selectionSettings.enemyColor : this.selectionSettings.secondaryEnemyColor);
				}
			}
		}
		else
		{
			global::Army component2 = obj.GetComponent<global::Army>();
			value = this.GetStanceColor(null, primary);
			if (component2 != null)
			{
				value = this.GetStanceColor(component2.logic, primary);
			}
			global::Squad component3 = obj.GetComponent<global::Squad>();
			if (component3 != null)
			{
				material = this.selectionSettings.squadSelectionMaterial;
				value = this.GetStanceColor(component3.GetLogic().GetKingdom(), primary);
			}
			SelectionCircle component4 = obj.GetComponent<SelectionCircle>();
			if (component4 != null)
			{
				value = this.GetStanceColor(component4.logic.GetKingdom(), primary);
			}
			global::Fortification component5 = obj.GetComponent<global::Fortification>();
			if (component5 != null)
			{
				material = this.selectionSettings.squadSelectionMaterial;
				value = this.GetStanceColor(GameLogic.Get(true).GetKingdom(component5.kingdom), primary);
			}
			global::Settlement component6 = obj.GetComponent<global::Settlement>();
			if (component6 != null)
			{
				material = (component6.IsCastle() ? this.selectionSettings.castleSelectionMaterial : this.selectionSettings.villageSelectionMaterial);
				value = this.GetStanceColor(component6.logic, primary);
			}
		}
		if (cur != null)
		{
			material = cur;
		}
		else if (material != null)
		{
			material = new Material(material);
		}
		if (material != null)
		{
			material.SetColor("_Color", value);
		}
		return material;
	}

	// Token: 0x06001911 RID: 6417 RVA: 0x000F3DA8 File Offset: 0x000F1FA8
	public virtual void OnMenu()
	{
		if (this.menu == null)
		{
			return;
		}
		if (UIFallbackWindow.IsActive())
		{
			return;
		}
		InGameMenu component = this.menu.GetComponent<InGameMenu>();
		if (component != null)
		{
			component.HandleEscapeAction();
			return;
		}
		bool flag = !this.menu.activeSelf;
		this.menu.SetActive(flag);
		if (flag)
		{
			this.menu.transform.SetAsLastSibling();
			this.system_message_container.transform.SetAsLastSibling();
		}
	}

	// Token: 0x06001912 RID: 6418 RVA: 0x000F3E26 File Offset: 0x000F2026
	public virtual bool IsMainMenuOpen()
	{
		return !(this.menu == null) && this.menu.activeSelf;
	}

	// Token: 0x06001913 RID: 6419 RVA: 0x000F3E44 File Offset: 0x000F2044
	private void UpdateCanvasScaler()
	{
		if (this.canvasScaler == null)
		{
			return;
		}
		float num = this.canvasScaler.referenceResolution.x / this.canvasScaler.referenceResolution.y;
		float num2 = (float)Screen.width / (float)Screen.height;
		this.canvasScaler.matchWidthOrHeight = (float)((num2 < num) ? 0 : 1);
	}

	// Token: 0x06001914 RID: 6420 RVA: 0x000F3EA4 File Offset: 0x000F20A4
	private void OnDrawGizmos()
	{
		if (Application.isPlaying && CameraController.MainCamera != null)
		{
			Gizmos.DrawLine(CameraController.MainCamera.transform.position, this.hits[0].point);
		}
	}

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x06001915 RID: 6421 RVA: 0x000F3EDF File Offset: 0x000F20DF
	public TMP_Text PickedTextField
	{
		get
		{
			return BaseUI.picked_text_field;
		}
	}

	// Token: 0x17000158 RID: 344
	// (get) Token: 0x06001916 RID: 6422 RVA: 0x000F3EE6 File Offset: 0x000F20E6
	public UIText PickedUIText
	{
		get
		{
			return BaseUI.picked_ui_text;
		}
	}

	// Token: 0x17000159 RID: 345
	// (get) Token: 0x06001917 RID: 6423 RVA: 0x000F3EED File Offset: 0x000F20ED
	public string PickedText
	{
		get
		{
			TMP_Text tmp_Text = BaseUI.picked_text_field;
			if (tmp_Text == null)
			{
				return null;
			}
			return tmp_Text.text;
		}
	}

	// Token: 0x1700015A RID: 346
	// (get) Token: 0x06001918 RID: 6424 RVA: 0x000F3EFF File Offset: 0x000F20FF
	public int PickedLine
	{
		get
		{
			return BaseUI.picked_line;
		}
	}

	// Token: 0x1700015B RID: 347
	// (get) Token: 0x06001919 RID: 6425 RVA: 0x000F3F06 File Offset: 0x000F2106
	public int PickedCharIdx
	{
		get
		{
			return BaseUI.picked_char_idx;
		}
	}

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x0600191A RID: 6426 RVA: 0x000F3F10 File Offset: 0x000F2110
	public string PickedChar
	{
		get
		{
			if (BaseUI.picked_char_idx >= 0 && this.PickedText != null)
			{
				return this.PickedText[BaseUI.picked_char_idx].ToString();
			}
			return null;
		}
	}

	// Token: 0x1700015D RID: 349
	// (get) Token: 0x0600191B RID: 6427 RVA: 0x000F3F47 File Offset: 0x000F2147
	public int PickedLinkIdx
	{
		get
		{
			return BaseUI.picked_link_idx;
		}
	}

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x0600191C RID: 6428 RVA: 0x000F3F4E File Offset: 0x000F214E
	public UIText.LinkInfo PickedLink
	{
		get
		{
			return BaseUI.picked_link;
		}
	}

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x0600191D RID: 6429 RVA: 0x000F3F55 File Offset: 0x000F2155
	public string PickedLinkType
	{
		get
		{
			return BaseUI.picked_link.type;
		}
	}

	// Token: 0x17000160 RID: 352
	// (get) Token: 0x0600191E RID: 6430 RVA: 0x000F3F61 File Offset: 0x000F2161
	public string PickedLinkValue
	{
		get
		{
			return BaseUI.picked_link.value;
		}
	}

	// Token: 0x17000161 RID: 353
	// (get) Token: 0x0600191F RID: 6431 RVA: 0x000F3F6D File Offset: 0x000F216D
	public Tooltip PickedLinkTooltip
	{
		get
		{
			return BaseUI.picked_link_tooltip;
		}
	}

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06001920 RID: 6432 RVA: 0x000F3F74 File Offset: 0x000F2174
	public Vars PickedLinkVars
	{
		get
		{
			return BaseUI.picked_link_vars;
		}
	}

	// Token: 0x06001921 RID: 6433 RVA: 0x000F3F7C File Offset: 0x000F217C
	private static bool HandleLinkEvent(string message, object param = null)
	{
		BaseUI.link_handled = false;
		for (int i = 0; i < BaseUI.link_handlers.Count; i++)
		{
			BaseUI.link_handlers[i].OnMessage(null, message, param);
			if (BaseUI.link_handled)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001922 RID: 6434 RVA: 0x000F3FC4 File Offset: 0x000F21C4
	public static Camera GetCanvasCamera(TMP_Text component)
	{
		if (component == null)
		{
			return null;
		}
		Canvas componentInParent = component.GetComponentInParent<Canvas>();
		if (componentInParent == null)
		{
			return null;
		}
		if (componentInParent.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			return null;
		}
		return componentInParent.worldCamera;
	}

	// Token: 0x06001923 RID: 6435 RVA: 0x000F4000 File Offset: 0x000F2200
	private static bool IsInsideLine(Vector2 pos, TMP_Text text_field, int line)
	{
		if (line >= 0)
		{
			bool flag;
			if (text_field == null)
			{
				flag = (null != null);
			}
			else
			{
				TMP_TextInfo textInfo = text_field.textInfo;
				flag = (((textInfo != null) ? textInfo.lineInfo : null) != null);
			}
			if (flag && line < text_field.textInfo.lineInfo.Length)
			{
				TMP_LineInfo tmp_LineInfo = text_field.textInfo.lineInfo[line];
				RectTransform rectTransform = text_field.rectTransform;
				TMP_CharacterInfo tmp_CharacterInfo = text_field.textInfo.characterInfo[tmp_LineInfo.firstVisibleCharacterIndex];
				Vector3 vector = rectTransform.TransformPoint(tmp_CharacterInfo.bottomLeft);
				if (pos.x < vector.x)
				{
					return false;
				}
				TMP_CharacterInfo tmp_CharacterInfo2 = text_field.textInfo.characterInfo[tmp_LineInfo.lastVisibleCharacterIndex];
				Vector3 vector2 = rectTransform.TransformPoint(tmp_CharacterInfo2.bottomRight);
				return pos.x <= vector2.x;
			}
		}
		return false;
	}

	// Token: 0x06001924 RID: 6436 RVA: 0x000F40C4 File Offset: 0x000F22C4
	private static void FindPickedLink(GameObject go, out UIText ui_text, out int link_idx, out UIText.LinkInfo link)
	{
		link_idx = -1;
		link = default(UIText.LinkInfo);
		BaseUI.picked_line = -1;
		BaseUI.picked_char_idx = -1;
		ui_text = ((go != null) ? go.GetComponent<UIText>() : null);
		if (BaseUI.pressed_link_idx < 0 && Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
		{
			return;
		}
		UIText uitext = ui_text;
		TMP_Text tmp_Text = (uitext != null) ? uitext.text_field : null;
		if (tmp_Text != null)
		{
			Camera canvasCamera = BaseUI.GetCanvasCamera(tmp_Text);
			Vector3 mousePosition = Input.mousePosition;
			BaseUI.picked_line = TMP_TextUtilities.FindIntersectingLine(tmp_Text, mousePosition, canvasCamera);
			if (BaseUI.IsInsideLine(mousePosition, tmp_Text, BaseUI.picked_line))
			{
				int num = TMP_TextUtilities.FindNearestCharacterOnLine(tmp_Text, mousePosition, BaseUI.picked_line, canvasCamera, false);
				if (num >= 0)
				{
					BaseUI.picked_char_idx = tmp_Text.textInfo.characterInfo[num].index;
					ui_text.FindLink(BaseUI.picked_char_idx, out link_idx, out link);
				}
			}
		}
		if (link_idx < 0)
		{
			ui_text = BaseUI.FindPosessedLinkUIText(go);
			if (ui_text != null && ui_text.GetFirstLink(out link_idx, out link))
			{
				tmp_Text = ui_text.text_field;
				BaseUI.picked_char_idx = link.start_idx;
			}
		}
		if ((tmp_Text != null && BaseUI.pressed_link_text_field != null && tmp_Text != BaseUI.pressed_link_text_field) || (link_idx >= 0 && BaseUI.pressed_link_idx >= 0 && link_idx != BaseUI.pressed_link_idx))
		{
			link_idx = -1;
		}
	}

	// Token: 0x06001925 RID: 6437 RVA: 0x000F4204 File Offset: 0x000F2404
	public static void UpdateTextLink(GameObject go)
	{
		UIText uitext;
		int num;
		UIText.LinkInfo linkInfo;
		BaseUI.FindPickedLink(go, out uitext, out num, out linkInfo);
		TMP_Text x = (uitext != null) ? uitext.text_field : null;
		if (x == BaseUI.picked_text_field && num == BaseUI.picked_link_idx)
		{
			BaseUI.picked_text_field = x;
			BaseUI.picked_ui_text = uitext;
			return;
		}
		if (BaseUI.picked_link_idx >= 0)
		{
			BaseUI.OnLinkLeave();
		}
		BaseUI.picked_text_field = x;
		BaseUI.picked_ui_text = uitext;
		BaseUI.picked_link_idx = num;
		BaseUI.picked_link = linkInfo;
		if (num >= 0)
		{
			BaseUI.OnLinkEnter();
		}
	}

	// Token: 0x06001926 RID: 6438 RVA: 0x000F427C File Offset: 0x000F247C
	private static UIText FindPosessedLinkUIText(GameObject go)
	{
		if (go == null)
		{
			return null;
		}
		UIHyperText.Row row;
		for (;;)
		{
			Transform parent = go.transform.parent;
			UIHyperText uihyperText = (parent != null) ? parent.GetComponentInParent<UIHyperText>() : null;
			if (uihyperText == null)
			{
				break;
			}
			UIHyperText.Element element;
			uihyperText.FindElement(go.transform as RectTransform, out row, out element);
			if (row != null && row.possessed_link_ui_text != null)
			{
				goto Block_5;
			}
			go = uihyperText.gameObject;
		}
		return null;
		Block_5:
		return row.possessed_link_ui_text;
	}

	// Token: 0x06001927 RID: 6439 RVA: 0x000F42F0 File Offset: 0x000F24F0
	public static object PickedLinkObj()
	{
		string type = BaseUI.picked_link.type;
		if (!(type == "def"))
		{
			if (!(type == "wiki"))
			{
				if (!(type == "keyword"))
				{
					if (!(type == "wiki_history"))
					{
						if (!(type == "obj"))
						{
							return null;
						}
						int uid;
						if (!int.TryParse(BaseUI.picked_link.value, out uid))
						{
							return null;
						}
						Logic.Object @object = GameLogic.Get(true).FindObjectByUIDVerySlow(uid);
						if (@object == null)
						{
							return null;
						}
						return @object;
					}
					else
					{
						int uid2;
						if (!int.TryParse(BaseUI.picked_link.value, out uid2))
						{
							return null;
						}
						UIWikiWindow.HistoryItem historyItem = UIWikiWindow.HistoryItem.Get(uid2);
						if (historyItem == null)
						{
							return null;
						}
						return historyItem;
					}
				}
				else
				{
					Wiki wiki = Wiki.Get();
					if (wiki == null)
					{
						return null;
					}
					Wiki.Keyword keyword = wiki.GetKeyword(BaseUI.picked_link.value, false);
					if (keyword == null)
					{
						return null;
					}
					return keyword;
				}
			}
			else
			{
				Wiki wiki2 = Wiki.Get();
				if (wiki2 == null)
				{
					return null;
				}
				Wiki.Article article = wiki2.FindArticle(BaseUI.picked_link.value);
				if (article != null)
				{
					return article;
				}
				DT.Field defField = global::Defs.GetDefField(BaseUI.picked_link.value, null);
				if (defField == null)
				{
					return null;
				}
				return defField;
			}
		}
		else
		{
			global::Defs defs = global::Defs.Get(false);
			DT.Field field;
			if (defs == null)
			{
				field = null;
			}
			else
			{
				DT dt = defs.dt;
				field = ((dt != null) ? dt.Find(BaseUI.picked_link.value, null) : null);
			}
			DT.Field field2 = field;
			if (field2 == null)
			{
				return null;
			}
			Def def = Def.Get(field2);
			if (def != null)
			{
				return def;
			}
			return field2;
		}
	}

	// Token: 0x06001928 RID: 6440 RVA: 0x000F4454 File Offset: 0x000F2654
	private static void CreateLinkVars(object obj, Transform transform)
	{
		BaseUI.picked_link_vars = new Vars(obj);
		BaseUI.picked_link_vars.Set<object>("link_obj", obj);
		IVars componentInParent = transform.GetComponentInParent<IVars>();
		if (componentInParent != null)
		{
			BaseUI.picked_link_vars.Set<IVars>("link_vars", componentInParent);
		}
	}

	// Token: 0x06001929 RID: 6441 RVA: 0x000F4498 File Offset: 0x000F2698
	private static void CreateLinkTooltip(DT.Def ttdef, object obj, TMP_Text component)
	{
		if (ttdef == null)
		{
			return;
		}
		GameObject gameObject = global::Common.FindChildByName(component.gameObject, "LinkTooltip", false, true);
		if (gameObject == null)
		{
			gameObject = new GameObject("LinkTooltip", new Type[]
			{
				typeof(RectTransform)
			});
			RectTransform component2 = gameObject.GetComponent<RectTransform>();
			gameObject.transform.SetParent(component.transform, false);
			UICommon.FillParent(component2);
		}
		BaseUI.picked_link_tooltip = Tooltip.Get(gameObject, true);
		BaseUI.picked_link_tooltip.SetObj(obj, ttdef, BaseUI.picked_link_vars);
	}

	// Token: 0x0600192A RID: 6442 RVA: 0x000F4520 File Offset: 0x000F2720
	private static void OnLinkEnter()
	{
		if (BaseUI.HandleLinkEvent("link_enter", null))
		{
			return;
		}
		object obj = BaseUI.PickedLinkObj();
		if (obj == null)
		{
			return;
		}
		if (BaseUI.picked_ui_text == null || !BaseUI.picked_ui_text.in_wiki)
		{
			BaseUI.CreateLinkVars(obj, BaseUI.picked_text_field.transform);
			BaseUI.CreateLinkTooltip(Tooltip.GetDef(obj, "Link") ?? Tooltip.GetDef(obj, ""), obj, BaseUI.picked_text_field);
		}
		UIText uitext = BaseUI.picked_ui_text;
		if (uitext == null)
		{
			return;
		}
		uitext.AddUnderline(BaseUI.picked_link_idx);
	}

	// Token: 0x0600192B RID: 6443 RVA: 0x000F45A8 File Offset: 0x000F27A8
	private static void OnLinkLeave()
	{
		if (BaseUI.HandleLinkEvent("link_leave", null))
		{
			return;
		}
		if (BaseUI.picked_link_tooltip != null)
		{
			global::Common.DestroyObj(BaseUI.picked_link_tooltip.gameObject);
		}
		BaseUI.picked_link_tooltip = null;
		BaseUI.picked_link_vars = null;
		UIText uitext = BaseUI.picked_ui_text;
		if (uitext == null)
		{
			return;
		}
		uitext.DelUnderline(BaseUI.picked_link_idx);
	}

	// Token: 0x0600192C RID: 6444 RVA: 0x000F45FF File Offset: 0x000F27FF
	private void OnLinkBtnDown()
	{
		BaseUI.pressed_link_text_field = BaseUI.picked_text_field;
		BaseUI.pressed_link_idx = BaseUI.picked_link_idx;
		BaseUI.pressed_link_mouse_pos = Input.mousePosition;
	}

	// Token: 0x0600192D RID: 6445 RVA: 0x000F4624 File Offset: 0x000F2824
	private void OnLinkBtnUp()
	{
		if (BaseUI.picked_link_idx == BaseUI.pressed_link_idx && BaseUI.picked_text_field == BaseUI.pressed_link_text_field)
		{
			this.OnLinkClick();
		}
		BaseUI.pressed_link_text_field = null;
		BaseUI.pressed_link_idx = -1;
	}

	// Token: 0x0600192E RID: 6446 RVA: 0x000F4658 File Offset: 0x000F2858
	private void OnLinkClick()
	{
		this.DoubleClickCheck();
		if (BaseUI.HandleLinkEvent("link_click", this.dblclk))
		{
			return;
		}
		object obj = BaseUI.PickedLinkObj();
		if (obj == null)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"OnLinkClick: ",
				global::Common.ObjPath(BaseUI.picked_text_field.gameObject),
				"(",
				BaseUI.picked_link_idx,
				"): ",
				BaseUI.picked_link.ToString()
			}));
			return;
		}
		Logic.Object obj2;
		if ((obj2 = (obj as Logic.Object)) == null)
		{
			UIWikiWindow.HandleLinkClick(BaseUI.picked_link, obj);
			return;
		}
		if (UICommon.CheckModifierKeys(UICommon.ModifierKey.Shift, UICommon.ModifierKey.None))
		{
			this.HandleChatLink(obj2);
			return;
		}
		Pact pact;
		if ((pact = (obj as Pact)) != null)
		{
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (pact.IsMember(kingdom) | pact.target == kingdom)
			{
				UIWarsOverviewWindow.ToggleOpen(BaseUI.LogicKingdom(), null, pact);
			}
			return;
		}
		War w;
		if ((w = (obj as War)) != null)
		{
			UIWarsOverviewWindow.ToggleOpen(BaseUI.LogicKingdom(), w, null);
			return;
		}
		this.SelectObjFromLogic(obj2, false, true);
		if (this.dblclk)
		{
			this.LookAt(BaseUI.SelLO(), false);
		}
	}

	// Token: 0x0600192F RID: 6447 RVA: 0x000F4778 File Offset: 0x000F2978
	public void HandleChatLink(Logic.Object obj)
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		if (!game.IsMultiplayer())
		{
			return;
		}
		if (this.in_game_chat != null)
		{
			this.in_game_chat.AddInputChatLink(obj);
		}
	}

	// Token: 0x06001930 RID: 6448 RVA: 0x000F47B4 File Offset: 0x000F29B4
	public bool IsPinned(Tooltip tt)
	{
		if (tt == null || tt.instance == null)
		{
			return false;
		}
		if (tt == this.pinned_tooltip)
		{
			return true;
		}
		TooltipInstance tooltipInstance;
		if (tt == null)
		{
			tooltipInstance = null;
		}
		else
		{
			GameObject instance = tt.instance;
			tooltipInstance = ((instance != null) ? instance.GetComponent<TooltipInstance>() : null);
		}
		TooltipInstance tooltipInstance2 = tooltipInstance;
		return tooltipInstance2 != null && tooltipInstance2.pinned;
	}

	// Token: 0x06001931 RID: 6449 RVA: 0x000F4818 File Offset: 0x000F2A18
	public static bool IgnoreRightClick()
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return false;
		}
		if (baseUI.ignore_right_click)
		{
			return true;
		}
		UICommon.ModifierKey modifierKeys = UICommon.GetModifierKeys();
		if (modifierKeys == UICommon.ModifierKey.None || modifierKeys == (UICommon.ModifierKey.Alt | UICommon.ModifierKey.LeftAlt))
		{
			Tooltip tooltip = baseUI.tooltip;
			if (tooltip == null || tooltip.instance == null)
			{
				return false;
			}
			TooltipInstance component = tooltip.instance.GetComponent<TooltipInstance>();
			if (component != null && component.pinable && !component.pinned)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001932 RID: 6450 RVA: 0x000F4896 File Offset: 0x000F2A96
	public static bool IgnoreLeftClick()
	{
		return !(BaseUI.Get() == null) && BaseUI.picked_link_idx >= 0;
	}

	// Token: 0x06001933 RID: 6451 RVA: 0x000F48B4 File Offset: 0x000F2AB4
	public void PinTooltip()
	{
		Tooltip tooltip = this.tooltip;
		if (((tooltip != null) ? tooltip.instance : null) == null)
		{
			return;
		}
		this.pinned_tooltip = this.tooltip;
		this.pinned_tooltip_instance = this.pinned_tooltip.instance;
		this.tooltip = null;
		CanvasGroup component = this.pinned_tooltip.instance.gameObject.GetComponent<CanvasGroup>();
		if (component != null)
		{
			component.blocksRaycasts = true;
			component.interactable = true;
		}
		this.pinned_tooltip_instance.GetOrAddComponent<DragWindow>();
		TooltipInstance orAddComponent = this.pinned_tooltip_instance.GetOrAddComponent<TooltipInstance>();
		if (orAddComponent != null)
		{
			orAddComponent.Pin();
		}
	}

	// Token: 0x06001934 RID: 6452 RVA: 0x000F4954 File Offset: 0x000F2B54
	public void DestroyTooltipInstance(GameObject instance = null)
	{
		if (instance == null)
		{
			instance = this.pinned_tooltip_instance;
			if (instance == null)
			{
				return;
			}
		}
		if (instance == this.pinned_tooltip_instance)
		{
			this.pinned_tooltip_instance = null;
		}
		if (this.pinned_tooltip != null && this.pinned_tooltip.instance == instance)
		{
			this.pinned_tooltip.instance = null;
			this.pinned_tooltip = null;
		}
		TooltipInstance component = instance.GetComponent<TooltipInstance>();
		if (component != null)
		{
			component.Done();
		}
		global::Common.DestroyObj(instance);
	}

	// Token: 0x06001935 RID: 6453 RVA: 0x000F49E4 File Offset: 0x000F2BE4
	protected virtual void UpdatePicker()
	{
		if (Input.mousePosition != this.ptLastMousePos)
		{
			this.tmLastMouseMove = UnityEngine.Time.unscaledTime;
			this.ptLastMousePos = Input.mousePosition;
		}
		if (CameraController.GameCamera.Camera == null)
		{
			return;
		}
		Camera camera = CameraController.GameCamera.Camera;
		Vector3 vector = UICommon.Screen2World(Input.mousePosition.x, Input.mousePosition.y, CameraController.GameCamera.Camera);
		Vector3 direction = Vector3.Normalize(vector - camera.transform.position);
		Ray ray = new Ray(vector, direction);
		int num = Physics.RaycastNonAlloc(ray, this.hits);
		this.picked_map_object = null;
		this.picked_settlement = null;
		this.picked_army = null;
		this.picked_unit = null;
		this.picked_terrain_point = Vector3.zero;
		this.picked_passable_area = 0;
		this.picked_passable_area_pos = Vector3.zero;
		Tooltip tooltip = null;
		for (int i = 0; i < num; i++)
		{
			Transform transform = this.hits[i].transform;
			if (!(transform == null))
			{
				if (tooltip == null)
				{
					tooltip = Tooltip.FindInParents(transform);
				}
				if (!ViewMode.IsPoliticalView())
				{
					if (this.picked_settlement == null)
					{
						global::Settlement componentInParent = transform.GetComponentInParent<global::Settlement>();
						if (componentInParent != null)
						{
							bool? flag;
							if (componentInParent == null)
							{
								flag = null;
							}
							else
							{
								Logic.Settlement logic = componentInParent.logic;
								flag = ((logic != null) ? new bool?(logic.IsActiveSettlement()) : null);
							}
							if (flag ?? true)
							{
								this.picked_settlement = componentInParent;
								this.picked_map_object = this.picked_settlement.logic;
							}
						}
					}
					if (UICommon.GetKey(KeyCode.LeftShift, false))
					{
						this.picked_unit = null;
					}
					if (this.picked_army == null)
					{
						global::Army componentInParent2 = transform.GetComponentInParent<global::Army>();
						if (componentInParent2 != null)
						{
							this.picked_army = componentInParent2;
							this.picked_map_object = this.picked_army.logic;
						}
					}
				}
				if (transform.GetComponentInParent<Terrain>() != null)
				{
					global::Realm realm = global::Realm.At(this.hits[i].point);
					if (realm != null && realm.IsSeaRealm())
					{
						this.picked_terrain_point = this.hits[i].point + ray.direction / ray.direction.y * (MapData.GetWaterLevel() - this.hits[i].point.y);
					}
					else
					{
						this.picked_terrain_point = this.hits[i].point;
					}
				}
			}
		}
		this.SetCursor(this.picked_terrain_point);
		this.UpdateTooltip(tooltip);
	}

	// Token: 0x06001936 RID: 6454 RVA: 0x000F4CA8 File Offset: 0x000F2EA8
	private void UpdateGameSpeedControls()
	{
		if (GameSpeed.SupressSpeedChangesByPlayer)
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (game == null || game.multiplayer == null)
		{
			return;
		}
		if (KeyBindings.GetBindDown("speed_up_game"))
		{
			game.SetSpeed(GameSpeed.GetNextSpeedUp(game), -1);
		}
		if (KeyBindings.GetBindDown("slow_down_game") && game.GetSpeed() >= 0.005f)
		{
			game.SetSpeed(GameSpeed.GetNextSpeedDown(game), -1);
		}
		if (UICommon.GetKeyUp(KeyCode.KeypadMultiply, UICommon.ModifierKey.None, UICommon.ModifierKey.None) || UICommon.GetKeyUp(KeyCode.Alpha0, UICommon.ModifierKey.None, UICommon.ModifierKey.None))
		{
			game.SetSpeed(Mathf.Approximately(game.GetSpeed(), 1f) ? 10f : 1f, -1);
		}
		if (KeyBindings.GetBindDown("pause_game"))
		{
			game.pause.ToggleManualPause(-2);
		}
	}

	// Token: 0x06001937 RID: 6455 RVA: 0x000F4D68 File Offset: 0x000F2F68
	protected virtual void UpdateInput()
	{
		this.ignore_right_click = false;
		if (UICommon.GetKeyUp(KeyCode.Escape, UICommon.ModifierKey.None, UICommon.ModifierKey.None) && !Hotspot.EndDrag(false))
		{
			if (this.IsMainMenuOpen())
			{
				this.OnMenu();
			}
			else if (!this.window_dispatcher.HandleBackInputAction())
			{
				this.OnMenu();
			}
		}
		if (UICommon.GetKeyUp(KeyCode.F10, UICommon.ModifierKey.None, UICommon.ModifierKey.None))
		{
			this.OnMenu();
		}
		if (KeyBindings.GetBindDown("focus_selection"))
		{
			this.LookAt(BaseUI.SelLO(), false);
		}
		if (KeyBindings.GetBindDown("focus_governed_realm"))
		{
			Logic.Character character = BaseUI.SelChar();
			if (character == null)
			{
				return;
			}
			if (character.IsGovernor())
			{
				Logic.Kingdom kingdom = BaseUI.LogicKingdom();
				if (kingdom == null)
				{
					return;
				}
				if (character.IsAllyOrOwn(kingdom))
				{
					this.LookAt(character.governed_castle, true);
					return;
				}
			}
			this.LookAt(BaseUI.SelLO(), false);
		}
		if (KeyBindings.GetBindDown("focus_path_end"))
		{
			Logic.Object @object = BaseUI.SelLO();
			if (@object == null)
			{
				return;
			}
			Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
			if (kingdom2 == null)
			{
				return;
			}
			if (@object.IsAllyOrOwn(kingdom2))
			{
				this.LookAtDest(@object);
			}
		}
		if (UICommon.GetKeyUp(KeyCode.Delete, UICommon.ModifierKey.None, UICommon.ModifierKey.None))
		{
			this.DelSelected();
		}
		if (KeyBindings.GetBindUp("toggle_nameplates"))
		{
			BaseUI.ToggleNameplates();
		}
		if (UICommon.GetKeyUp(KeyCode.Insert, UICommon.ModifierKey.None, UICommon.ModifierKey.All) || UICommon.GetKeyUp(KeyCode.I, UICommon.ModifierKey.None, UICommon.ModifierKey.All))
		{
			if (this is BattleViewUI)
			{
				int num = 0;
				string text = null;
				if (UICommon.GetKey(KeyCode.LeftControl, false))
				{
					GameObject gameObject = this.selected_obj;
					global::Squad squad = (gameObject != null) ? gameObject.GetComponent<global::Squad>() : null;
					if (squad != null)
					{
						text = squad.def.id;
						num = squad.logic.battle_side;
						if (UICommon.GetKey(KeyCode.RightAlt, false))
						{
							num = 1 - num;
						}
					}
				}
				if (text == null)
				{
					num = (UICommon.GetKey(KeyCode.RightAlt, false) ? 1 : 0);
					text = Troops.valid_defs[Random.Range(0, Troops.valid_defs.Count)].field.key;
				}
				global::Squad.Spawn(num, text, global::Common.GetPickedTerrainPoint(), true);
			}
			else
			{
				this.SpawnArmy();
			}
		}
		if (KeyBindings.GetBindUp("toggle_ui"))
		{
			Game game = GameLogic.Get(false);
			if (game != null && game.state != Game.State.Running)
			{
				return;
			}
			Transform transform = base.transform.Find("Canvas");
			if (transform != null)
			{
				transform.gameObject.SetActive(!transform.gameObject.activeSelf);
			}
		}
		if (KeyBindings.GetBindDown("toggle_audio"))
		{
			UserSettings.SettingData setting = UserSettings.GetSetting("volume_master_enabled");
			if (setting != null)
			{
				setting.ApplyValue(!UserSettings.MasterOn);
			}
		}
		if (KeyBindings.GetBind("decrease_master_volume", false))
		{
			UserSettings.SettingData setting2 = UserSettings.GetSetting("volume_master");
			if (setting2 == null)
			{
				return;
			}
			Value min_value = setting2.min_value;
			float num2 = setting2.value.float_val - setting2.step_value.float_val;
			if (num2 < min_value)
			{
				num2 = min_value;
			}
			setting2.ApplyValue(num2);
		}
		if (KeyBindings.GetBind("increase_master_volume", false))
		{
			UserSettings.SettingData setting3 = UserSettings.GetSetting("volume_master");
			if (setting3 == null)
			{
				return;
			}
			Value max_value = setting3.max_value;
			float num3 = setting3.value.float_val + setting3.step_value.float_val;
			if (num3 > max_value)
			{
				num3 = max_value;
			}
			setting3.ApplyValue(num3);
		}
		if (KeyBindings.GetBindDown("toggle_voice"))
		{
			UserSettings.SettingData setting4 = UserSettings.GetSetting("volume_voices_enabled");
			if (setting4 != null)
			{
				setting4.ApplyValue(!UserSettings.VoiceOn);
			}
		}
		if (KeyBindings.GetBindDown("toggle_music"))
		{
			UserSettings.SettingData setting5 = UserSettings.GetSetting("volume_music_enabled");
			if (setting5 != null)
			{
				setting5.ApplyValue(!UserSettings.MusicOn);
			}
		}
		if (KeyBindings.GetBindDown("toggle_ambience"))
		{
			UserSettings.SettingData setting6 = UserSettings.GetSetting("volume_ambient_enabled");
			if (setting6 != null)
			{
				setting6.ApplyValue(!UserSettings.AmbienceOn);
			}
		}
		if (KeyBindings.GetBindDown("toggle_sound_effects"))
		{
			UserSettings.SettingData setting7 = UserSettings.GetSetting("volume_sound_effects_enabled");
			if (setting7 != null)
			{
				setting7.ApplyValue(!UserSettings.SoundEffectsOn);
			}
		}
		if (UICommon.GetKeyUp(KeyCode.F11, UICommon.ModifierKey.None, UICommon.ModifierKey.None))
		{
			FPSCounterIMGUI fpscounterIMGUI = base.GetComponent<FPSCounterIMGUI>();
			if (fpscounterIMGUI == null)
			{
				fpscounterIMGUI = base.gameObject.AddComponent<FPSCounterIMGUI>();
			}
			else
			{
				fpscounterIMGUI.NextMode();
			}
		}
		if (UICommon.GetKeyDown(KeyCode.Backslash, UICommon.ModifierKey.None, UICommon.ModifierKey.None))
		{
			this.PinTooltip();
		}
		if (UICommon.GetKeyUp(KeyCode.F7, UICommon.ModifierKey.None, UICommon.ModifierKey.None) && !UICommon.GetKey(KeyCode.LeftShift, false))
		{
			if (UIBugReportWindow.Create() != null)
			{
				EventSystem eventSystem = EventSystem.current;
				if (((eventSystem != null) ? eventSystem.currentSelectedGameObject : null) != null)
				{
					object[] array = new object[4];
					array[0] = "F7 while having a crurrently selected game event object: ";
					array[1] = EventSystem.current.currentSelectedGameObject;
					array[2] = " \n+ To string: ";
					int num4 = 3;
					EventSystem eventSystem2 = EventSystem.current;
					array[num4] = ((eventSystem2 != null) ? eventSystem2.currentSelectedGameObject.ToString() : null);
					UnityEngine.Debug.LogError(string.Concat(array));
				}
			}
			return;
		}
		if (CameraPaths.instance == null && UICommon.GetKeyDown(KeyCode.M, UICommon.ModifierKey.Ctrl, UICommon.ModifierKey.None) && Game.CheckCheatLevel(Game.CheatLevel.Low, "camera paths", false))
		{
			CameraPaths cameraPaths = CameraPaths.Get(true);
			if (cameraPaths != null)
			{
				cameraPaths.show = true;
			}
		}
		bool flag = this.bAlt;
		this.bAlt = UICommon.GetKey(KeyCode.LeftAlt, false);
		if (!flag && this.bAlt && this.OnKeyPressed != null)
		{
			this.OnKeyPressed();
		}
		else if (flag && !this.bAlt && this.OnKeyReleased != null)
		{
			this.OnKeyReleased();
		}
		if (this.bAlt != flag && this.tooltip != null)
		{
			this.RefreshTooltip(this.tooltip, false);
			TooltipPlacement.dirty = true;
		}
		this.UpdateGameSpeedControls();
		EventSystem eventSystem3 = EventSystem.current;
		bool flag2 = (eventSystem3 != null) ? eventSystem3.IsPointerOverGameObject() : IMGUIHandler.IsPointerOverIMGUI();
		if (flag2 && Input.GetMouseButtonDown(0))
		{
			if (this.picked_UI_element != null)
			{
				string text2 = global::Common.ObjPath(this.picked_UI_element);
				Analytics instance = Analytics.instance;
				if (instance != null)
				{
					instance.OnMessage(this.picked_UI_element, "mouse_click", text2);
				}
				if (Tutorial.cur_message_wnd == null)
				{
					Tutorial.CheckMouseClick(text2);
				}
			}
			if (BaseUI.picked_link_idx >= 0)
			{
				this.OnLinkBtnDown();
			}
		}
		if (BaseUI.pressed_link_idx >= 0)
		{
			if (Vector2.Distance(Input.mousePosition, BaseUI.pressed_link_mouse_pos) >= this.dblclk_max_dist)
			{
				BaseUI.pressed_link_text_field = null;
				BaseUI.pressed_link_idx = -1;
			}
			else if (Input.GetMouseButtonUp(0))
			{
				this.OnLinkBtnUp();
			}
		}
		UICommon.ModifierKey modifierKeys = UICommon.GetModifierKeys();
		if (flag2 && Input.GetMouseButtonDown(1) && this.tooltip != null && this.tooltip.instance != null && (modifierKeys == UICommon.ModifierKey.None || modifierKeys == (UICommon.ModifierKey.Alt | UICommon.ModifierKey.LeftAlt)))
		{
			TooltipInstance component = this.tooltip.instance.GetComponent<TooltipInstance>();
			if (component != null && component.pinable && !component.pinned)
			{
				this.PinTooltip();
				this.ignore_right_click = true;
			}
		}
		if (!flag2)
		{
			int i = 0;
			while (i <= 2)
			{
				if (Input.GetMouseButtonDown(i))
				{
					if (this.mouse_btn_down < 0)
					{
						this.mouse_btn_down = i;
						this.OnMouseDown(Input.mousePosition, i);
						break;
					}
					this.OnMouseDown(Input.mousePosition, i);
					this.OnSecondMouseDown();
					break;
				}
				else
				{
					i++;
				}
			}
		}
		if (this.mouse_btn_down >= 0 && Input.GetMouseButtonUp(this.mouse_btn_down))
		{
			this.OnMouseUp(Input.mousePosition, this.mouse_btn_down);
			this.mouse_btn_down = -1;
		}
		this.OnMouseMove(Input.mousePosition, this.mouse_btn_down);
	}

	// Token: 0x06001938 RID: 6456 RVA: 0x000F54E2 File Offset: 0x000F36E2
	public static void ToggleNameplates()
	{
		if (ViewMode.IsPoliticalView())
		{
			BaseUI.ToggleNameplates(UserSettings.NameplatesEnabledPV ? 0 : 1);
		}
		else
		{
			BaseUI.ToggleNameplates(UserSettings.NameplatesEnabledWV ? 0 : 1);
		}
		UIMinimapOverlay.minimap_filter_settings_changed = true;
	}

	// Token: 0x06001939 RID: 6457 RVA: 0x000F5514 File Offset: 0x000F3714
	public static void ToggleNameplates(int val)
	{
		if (ViewMode.IsPoliticalView())
		{
			UserSettings.SettingData setting = UserSettings.GetSetting("nameplates_pv");
			if (setting != null)
			{
				setting.ApplyValue(val == 1);
			}
		}
		else
		{
			if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "army_nameplates", true))
			{
				return;
			}
			UserSettings.SettingData setting2 = UserSettings.GetSetting("nameplates_wv");
			if (setting2 != null)
			{
				setting2.ApplyValue(val == 1);
			}
		}
		global::Army.RefreshAllArmyVisibility(GameLogic.Get(true).game);
	}

	// Token: 0x0600193A RID: 6458 RVA: 0x000F5588 File Offset: 0x000F3788
	public Vector3 ScreenToGroundPlane(Vector2 pts)
	{
		if (CameraController.MainCamera == null)
		{
			Vector3 terrainSize = this.GetTerrainSize();
			terrainSize.y = 0f;
			return terrainSize;
		}
		Ray ray = CameraController.MainCamera.ScreenPointToRay(pts);
		Plane plane = new Plane(Vector3.up, (CameraController.GameCamera != null) ? (-CameraController.GameCamera.Settings.lookAtHeight) : 0f);
		float distance = 0f;
		if (!plane.Raycast(ray, out distance))
		{
			return Vector3.zero;
		}
		return ray.GetPoint(distance);
	}

	// Token: 0x0600193B RID: 6459 RVA: 0x000F5618 File Offset: 0x000F3818
	public Vector3 ScreenToGroundPlane(float sx, float sy)
	{
		return this.ScreenToGroundPlane(new Vector2(sx, sy));
	}

	// Token: 0x0600193C RID: 6460 RVA: 0x000F5628 File Offset: 0x000F3828
	public Vector3 ScreenToTerrain(Vector2 pts)
	{
		Ray ray = CameraController.MainCamera.ScreenPointToRay(pts);
		int num = 1 << LayerMask.NameToLayer("Terrain");
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, num))
		{
			return raycastHit.point;
		}
		return Vector3.zero;
	}

	// Token: 0x0600193D RID: 6461 RVA: 0x000F5674 File Offset: 0x000F3874
	public void LookAt(Logic.Object obj, bool prefer_governed_castle = false)
	{
		MapObject mapObject = null;
		if (obj != null)
		{
			Logic.Squad squad;
			if ((squad = (obj as Logic.Squad)) == null)
			{
				Logic.Kingdom kingdom;
				if ((kingdom = (obj as Logic.Kingdom)) == null)
				{
					Logic.Realm realm;
					if ((realm = (obj as Logic.Realm)) == null)
					{
						Logic.Battle battle;
						if ((battle = (obj as Logic.Battle)) == null)
						{
							Logic.Army army;
							if ((army = (obj as Logic.Army)) == null)
							{
								Logic.Settlement settlement;
								if ((settlement = (obj as Logic.Settlement)) == null)
								{
									Logic.Character character;
									if ((character = (obj as Logic.Character)) == null)
									{
										Mercenary mercenary;
										if ((mercenary = (obj as Mercenary)) == null)
										{
											Logic.Rebel rebel;
											if ((rebel = (obj as Logic.Rebel)) != null)
											{
												Logic.Rebel rebel2 = rebel;
												mapObject = ((rebel2 != null) ? rebel2.army : null);
											}
										}
										else
										{
											Mercenary mercenary2 = mercenary;
											mapObject = ((mercenary2 != null) ? mercenary2.army : null);
										}
									}
									else
									{
										Logic.Character character2 = character;
										if (prefer_governed_castle)
										{
											mapObject = (character2.GovernTarget() ?? character2.CurLocation());
										}
										else
										{
											mapObject = character2.CurLocation();
										}
									}
								}
								else
								{
									Logic.Settlement settlement2 = settlement;
									if (prefer_governed_castle)
									{
										Logic.Realm realm2 = settlement2.GetRealm();
										Castle castle = (realm2 != null) ? realm2.castle : null;
										if (castle != null)
										{
											mapObject = castle;
											goto IL_152;
										}
									}
									mapObject = settlement2;
								}
							}
							else
							{
								Logic.Army army2 = army;
								if (prefer_governed_castle)
								{
									Logic.Character leader = army2.leader;
									if (((leader != null) ? leader.governed_castle : null) != null)
									{
										mapObject = army2.leader.governed_castle;
										goto IL_152;
									}
								}
								mapObject = army2;
							}
						}
						else
						{
							mapObject = battle;
						}
					}
					else
					{
						mapObject = realm.castle;
					}
				}
				else
				{
					Logic.Realm capital = kingdom.GetCapital();
					mapObject = ((capital != null) ? capital.castle : null);
				}
			}
			else
			{
				mapObject = squad;
			}
		}
		IL_152:
		if (mapObject == null)
		{
			return;
		}
		Vector3 pt = global::Common.SnapToTerrain(mapObject.position, 0f, null, -1f, false);
		this.LookAt(pt, true);
	}

	// Token: 0x0600193E RID: 6462 RVA: 0x000F57FC File Offset: 0x000F39FC
	public void LookAtDest(Logic.Object obj)
	{
		if (obj != null)
		{
			Logic.Army army;
			if ((army = (obj as Logic.Army)) == null)
			{
				Logic.Character character;
				if ((character = (obj as Logic.Character)) != null)
				{
					Logic.Character character2 = character;
					this.LookAt(character2.CurTarget(), false);
					return;
				}
				Castle castle;
				if ((castle = (obj as Castle)) != null)
				{
					Castle castle2 = castle;
					this.LookAt(castle2.governor, false);
					return;
				}
				Logic.Squad squad;
				if ((squad = (obj as Logic.Squad)) != null)
				{
					Logic.Squad squad2 = squad;
					Movement movement = squad2.movement;
					Path path = ((movement != null) ? movement.pf_path : null) ?? squad2.movement.path;
					if (path == null)
					{
						return;
					}
					this.LookAt(path.dst_pt, false);
					return;
				}
			}
			else
			{
				Logic.Army army2 = army;
				Movement movement2 = army2.movement;
				Path path2 = ((movement2 != null) ? movement2.pf_path : null) ?? army2.movement.path;
				if (path2 == null)
				{
					return;
				}
				if (path2.segments != null && path2.segments.Count > 0)
				{
					this.LookAt(path2.segments[path2.segments.Count - 1].pt, false);
					return;
				}
				this.LookAt(path2.dst_pt, false);
				return;
			}
		}
	}

	// Token: 0x0600193F RID: 6463 RVA: 0x000F5934 File Offset: 0x000F3B34
	public void LookAt(Vector3 pt, bool all_cameras = false)
	{
		if (all_cameras)
		{
			CameraController cameraController = CameraController.Get();
			List<GameCamera> list = (cameraController != null) ? cameraController.AllCameras : null;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i].LookAt(pt, true);
				}
				return;
			}
		}
		else
		{
			GameCamera gameCamera = CameraController.GameCamera;
			if (gameCamera == null)
			{
				return;
			}
			gameCamera.LookAt(pt, false);
		}
	}

	// Token: 0x06001940 RID: 6464 RVA: 0x000F598C File Offset: 0x000F3B8C
	public virtual void Select()
	{
		if (this.picked_army != null)
		{
			this.SelectObj(this.picked_army.gameObject, false, true, true, true);
			return;
		}
		if (this.picked_settlement != null)
		{
			this.SelectObj(this.picked_settlement.gameObject, false, true, true, true);
			return;
		}
		global::Realm realm = WorldMap.Get().RealmAt(this.picked_terrain_point.x, this.picked_terrain_point.z);
		if (realm != null && realm.IsSeaRealm())
		{
			DT.Field field = BaseUI.soundsDef;
			BaseUI.PlaySoundEvent((field != null) ? field.GetString("select_terrain_ocean", null, "", true, true, true, '.') : null, null);
		}
		else
		{
			DT.Field field2 = BaseUI.soundsDef;
			BaseUI.PlaySoundEvent((field2 != null) ? field2.GetString("select_terrain_field", null, "", true, true, true, '.') : null, null);
		}
		this.SelectObj(null, false, true, true, true);
	}

	// Token: 0x06001941 RID: 6465 RVA: 0x000F5A68 File Offset: 0x000F3C68
	public virtual void OnMouseDown(Vector2 pts, int btn)
	{
		this.DoubleClickCheck();
	}

	// Token: 0x06001942 RID: 6466 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void OnSecondMouseDown()
	{
	}

	// Token: 0x06001943 RID: 6467 RVA: 0x000F5A70 File Offset: 0x000F3C70
	public virtual void OnMouseUp(Vector2 pts, int btn)
	{
		if (btn == 1)
		{
			this.OnRightClick();
		}
	}

	// Token: 0x06001944 RID: 6468 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void OnMouseMove(Vector2 pts, int btn)
	{
	}

	// Token: 0x06001945 RID: 6469 RVA: 0x000F5A7C File Offset: 0x000F3C7C
	public virtual void OnRightClick()
	{
		global::Army army = null;
		if (this.select_target != null)
		{
			army = this.select_target.GetComponent<global::Army>();
			if (army == null)
			{
				global::Settlement component = this.select_target.GetComponent<global::Settlement>();
				if (component != null)
				{
					army = component.GetArmy();
				}
			}
		}
		if (army == null && this.selected_obj != null)
		{
			army = this.selected_obj.GetComponent<global::Army>();
			if (army == null)
			{
				global::Settlement component2 = this.selected_obj.GetComponent<global::Settlement>();
				if (component2 != null)
				{
					army = component2.GetArmy();
				}
			}
		}
		if (!(army != null))
		{
			return;
		}
		if (this.picked_army != null)
		{
			army.MoveTo(this.picked_army, true, true);
			return;
		}
		if (this.picked_settlement != null)
		{
			army.MoveTo(this.picked_settlement, true, true);
			return;
		}
		if (this.picked_passable_area != 0)
		{
			army.MoveTo(this.picked_passable_area_pos, this.picked_passable_area, true, true);
			return;
		}
		if (this.picked_terrain_point != Vector3.zero)
		{
			army.MoveTo(this.picked_terrain_point, 0, true, true);
		}
	}

	// Token: 0x06001946 RID: 6470 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void ViewModeChanged()
	{
	}

	// Token: 0x06001947 RID: 6471 RVA: 0x000F5B94 File Offset: 0x000F3D94
	[ConsoleMethod("selection", "Show/Hide selection")]
	public void ShowSelection(int bShow)
	{
		this.bSelection = (bShow != 0);
		if (this.selected_obj == null)
		{
			return;
		}
		global::Army component = this.selected_obj.GetComponent<global::Army>();
		if (component != null)
		{
			component.DestroySelection();
			component.CreateSelection();
			return;
		}
	}

	// Token: 0x06001948 RID: 6472 RVA: 0x000F5BDC File Offset: 0x000F3DDC
	public bool PathArrowsShown()
	{
		return this.bPathArrows;
	}

	// Token: 0x06001949 RID: 6473 RVA: 0x000F5BE4 File Offset: 0x000F3DE4
	[ConsoleMethod("pa", "Show/Hide army path arrows")]
	public void ShowPathArrows(int bShow)
	{
		this.bPathArrows = (bShow != 0);
		if (this.selected_obj == null)
		{
			return;
		}
		global::Army component = this.selected_obj.GetComponent<global::Army>();
		if (component != null)
		{
			component.DestroySelection();
			component.CreateSelection();
			return;
		}
	}

	// Token: 0x0600194A RID: 6474 RVA: 0x000F5C2C File Offset: 0x000F3E2C
	public bool SelectionShown()
	{
		return this.bSelection;
	}

	// Token: 0x0600194B RID: 6475 RVA: 0x000F5C34 File Offset: 0x000F3E34
	[ConsoleMethod("del", "Delete selected object")]
	public virtual void DelSelected()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "del", true))
		{
			return;
		}
		GameObject gameObject = this.selected_obj;
		if (gameObject == null)
		{
			return;
		}
		this.SelectObj(null, false, true, true, true);
		gameObject.GetComponent<GameLogic.Behaviour>().DeleteObject();
	}

	// Token: 0x0600194C RID: 6476 RVA: 0x000F5C78 File Offset: 0x000F3E78
	[ConsoleMethod("tr", "Turn the selected character into rebel")]
	public void TurnIntoRebel()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "tr", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			UnityEngine.Debug.Log("No selected character");
			return;
		}
		character.TurnIntoRebel(null, null, null, null);
	}

	// Token: 0x0600194D RID: 6477 RVA: 0x000F5CB4 File Offset: 0x000F3EB4
	[ConsoleMethod("tr", "Turn the selected character into rebel of given type")]
	public void TurnIntoRebel(string rebel_type)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "tr", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			UnityEngine.Debug.Log("No selected character");
			return;
		}
		character.TurnIntoRebel(rebel_type, null, null, null);
	}

	// Token: 0x0600194E RID: 6478 RVA: 0x000F5CF0 File Offset: 0x000F3EF0
	[ConsoleMethod("mrf", "Make rebellion famous")]
	public void MakeRebellionFamous(int val)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "mrf", true))
		{
			return;
		}
		Logic.Character character = BaseUI.SelChar();
		if (character == null)
		{
			UnityEngine.Debug.Log("No selected character");
			return;
		}
		if (character != null)
		{
			Logic.Army army = character.GetArmy();
			if (army == null)
			{
				return;
			}
			Logic.Rebel rebel = army.rebel;
			if (rebel == null)
			{
				return;
			}
			rebel.rebellion.SetFamous(val != 0);
		}
	}

	// Token: 0x0600194F RID: 6479 RVA: 0x000F5D48 File Offset: 0x000F3F48
	[ConsoleMethod("sr", "Spawn rebel at selected realm")]
	public void SpawnRebel(string type, int num)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "sr", true))
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			this.SpawnRebel(type);
		}
	}

	// Token: 0x06001950 RID: 6480 RVA: 0x000F5D78 File Offset: 0x000F3F78
	[ConsoleMethod("sr", "Spawn rebel at selected realm")]
	public Logic.Rebel SpawnRebel(string type)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "sr", true))
		{
			return null;
		}
		global::Realm realm = global::Realm.At(this.picked_terrain_point);
		if (realm == null)
		{
			return null;
		}
		Game game = realm.logic.game;
		if (string.IsNullOrEmpty(type))
		{
			return null;
		}
		Logic.Rebel.Def def = game.defs.Get<Logic.Rebel.Def>(type);
		if (def == null || def.IsBase())
		{
			List<Logic.Rebel.Def> defs = game.defs.GetDefs<Logic.Rebel.Def>();
			def = ((defs != null && defs.Count > 1) ? defs[1] : null);
		}
		if (def == null)
		{
			return null;
		}
		Logic.Kingdom factionKingdom = FactionUtils.GetFactionKingdom(game, def.kingdom_key);
		if (factionKingdom == null)
		{
			return null;
		}
		int num = (def.fraction_type == "LoyalistsFaction") ? realm.logic.pop_majority.kingdom.id : factionKingdom.id;
		if (num == 0)
		{
			if (realm.logic.kingdom_id != realm.logic.init_kingdom_id)
			{
				num = realm.logic.init_kingdom_id;
			}
			else
			{
				UnityEngine.Debug.Log("Realm has no external influence - spawning with player's id");
				num = BaseUI.LogicKingdom().id;
			}
		}
		List<RebelSpawnCondition.Def> defs2 = game.defs.GetDefs<RebelSpawnCondition.Def>();
		List<RebelSpawnCondition.Def> list = new List<RebelSpawnCondition.Def>(defs2.Count);
		for (int i = 0; i < defs2.Count; i++)
		{
			if (defs2[i].periodic && defs2[i].rebel_types.Contains(def))
			{
				list.Add(defs2[i]);
			}
		}
		RebelSpawnCondition.Def cond_def;
		if (list.Count > 0)
		{
			cond_def = list[Random.Range(0, list.Count)];
		}
		else
		{
			cond_def = defs2[0];
		}
		Logic.Rebel rebel = new Logic.Rebel(game, factionKingdom.id, num, realm.logic, cond_def, def, null);
		rebel.SetInitalRisk(realm.logic.GetTotalRebellionRisk());
		return rebel;
	}

	// Token: 0x06001951 RID: 6481 RVA: 0x000F5F44 File Offset: 0x000F4144
	[ConsoleMethod("merc", "enable/disable mercenaries spawn")]
	public void EnableSpawnMercs(int mode)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.Medium, "merc", true))
		{
			return;
		}
		if (mode == 0)
		{
			MercenarySpawner.DisableSpawn();
		}
		if (mode == 1)
		{
			MercenarySpawner.EnableSpawn();
		}
	}

	// Token: 0x06001952 RID: 6482 RVA: 0x000F5F68 File Offset: 0x000F4168
	[ConsoleMethod("sm", "Spawn mercenary at mouse's realm")]
	public void SpawnMercs(int count)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "sm", true))
		{
			return;
		}
		for (int i = 0; i < count; i++)
		{
			this.SpawnMercs("Mercenaries");
		}
	}

	// Token: 0x06001953 RID: 6483 RVA: 0x000F5F9B File Offset: 0x000F419B
	[ConsoleMethod("sm", "Spawn mercenary at mouse's realm")]
	public void SpawnMercs()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "sm", true))
		{
			return;
		}
		this.SpawnMercs("Mercenaries");
	}

	// Token: 0x06001954 RID: 6484 RVA: 0x000F5FB8 File Offset: 0x000F41B8
	public void SpawnMercs(string type)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "sm", true))
		{
			return;
		}
		global::Realm realm = global::Realm.At(this.picked_terrain_point);
		if (realm == null)
		{
			return;
		}
		Game game = realm.logic.game;
		Mercenary.Def def;
		if (!string.IsNullOrEmpty(type))
		{
			def = game.defs.Get<Mercenary.Def>(type);
		}
		def = game.defs.GetBase<Mercenary.Def>();
		if (def == null)
		{
			return;
		}
		int kingdom_id = MercenarySpawner.GetNearbyRealm(realm.logic, def.parent_realm_serch_limit, -1, null).kingdom_id;
		new Mercenary(game, kingdom_id, realm.logic, def);
	}

	// Token: 0x06001955 RID: 6485 RVA: 0x000F6040 File Offset: 0x000F4240
	[ConsoleMethod("sa", "Spawn army at mouse")]
	public void SpawnArmy()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "sa", true))
		{
			return;
		}
		Vector3 vector = this.picked_terrain_point;
		if (vector == Vector3.zero)
		{
			return;
		}
		int id = this.kingdom.id;
		if (UICommon.GetKey(KeyCode.RightAlt, false))
		{
			global::Realm realm = global::Realm.At(vector);
			id = ((realm == null) ? 0 : realm.kingdom.id);
		}
		global::Kingdom kingdom = global::Kingdom.Get(id);
		if (kingdom == null || kingdom.logic == null)
		{
			return;
		}
		if (!kingdom.logic.IsAuthority())
		{
			kingdom.logic.SendEvent(new Logic.Kingdom.SpawnArmyEvent(vector));
			return;
		}
		Logic.Character c = CharacterFactory.CreateCourtCandidate(kingdom.logic.game, kingdom.id, "Marshal");
		kingdom.logic.AddCourtMember(c, -1, false, true, false, true);
		Logic.Army army = new Logic.Army(kingdom.logic.game, vector, kingdom.id);
		army.SetLeader(c, true);
		army.FillWithRandomUnits();
	}

	// Token: 0x06001956 RID: 6486 RVA: 0x000F6134 File Offset: 0x000F4334
	public static Logic.Object SelLO()
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return null;
		}
		if (baseUI.selected_logic_obj != null)
		{
			return baseUI.selected_logic_obj;
		}
		if (baseUI.selected_court_member != null)
		{
			return baseUI.selected_court_member.logic;
		}
		ObjectWindow componentInChildren = baseUI.GetComponentInChildren<ObjectWindow>();
		if (componentInChildren != null)
		{
			return componentInChildren.logicObject;
		}
		return null;
	}

	// Token: 0x06001957 RID: 6487 RVA: 0x000F6190 File Offset: 0x000F4390
	public static object TTObj()
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return null;
		}
		Tooltip tooltip = baseUI.tooltip;
		Tooltip tooltip2;
		if ((tooltip2 = ((tooltip != null) ? tooltip.main_tooltip : null)) == null)
		{
			Tooltip tooltip3 = baseUI.pinned_tooltip;
			tooltip2 = ((tooltip3 != null) ? tooltip3.main_tooltip : null);
		}
		Tooltip tooltip4 = tooltip2;
		if (tooltip4 == null)
		{
			return null;
		}
		if (tooltip4.obj != null)
		{
			return tooltip4.obj;
		}
		if (tooltip4.vars != null && tooltip4.vars.obj.is_object)
		{
			return tooltip4.vars.obj.obj_val;
		}
		return null;
	}

	// Token: 0x06001958 RID: 6488 RVA: 0x000F6220 File Offset: 0x000F4420
	public static Vars TTVars()
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return null;
		}
		Tooltip tooltip = baseUI.tooltip;
		Tooltip tooltip2;
		if ((tooltip2 = ((tooltip != null) ? tooltip.main_tooltip : null)) == null)
		{
			Tooltip tooltip3 = baseUI.pinned_tooltip;
			tooltip2 = ((tooltip3 != null) ? tooltip3.main_tooltip : null);
		}
		Tooltip tooltip4 = tooltip2;
		if (tooltip4 == null)
		{
			return null;
		}
		return tooltip4.vars;
	}

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06001959 RID: 6489 RVA: 0x000F6278 File Offset: 0x000F4478
	public string tooltip_vars_dump
	{
		get
		{
			Vars vars = BaseUI.TTVars();
			if (vars == null)
			{
				return null;
			}
			return vars.DebugText();
		}
	}

	// Token: 0x0600195A RID: 6490 RVA: 0x000F628A File Offset: 0x000F448A
	public static object LinkObj()
	{
		Vars vars = BaseUI.picked_link_vars;
		if (vars == null)
		{
			return null;
		}
		return vars.obj.obj_val;
	}

	// Token: 0x0600195B RID: 6491 RVA: 0x000F62A1 File Offset: 0x000F44A1
	public static IVars LinkVars()
	{
		if (BaseUI.picked_link_vars == null)
		{
			return null;
		}
		return BaseUI.picked_link_vars.Get<IVars>("link_vars", null);
	}

	// Token: 0x0600195C RID: 6492 RVA: 0x000F62BC File Offset: 0x000F44BC
	public static object Alt()
	{
		if (UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.Low, "debug alt key", true))
		{
			return BaseUI.boxed_true;
		}
		return BaseUI.boxed_false;
	}

	// Token: 0x0600195D RID: 6493 RVA: 0x000F62E4 File Offset: 0x000F44E4
	public static object Ctrl()
	{
		if (UICommon.GetKey(KeyCode.LeftControl, false) && Game.CheckCheatLevel(Game.CheatLevel.Low, "debug ctrl key", true))
		{
			return BaseUI.boxed_true;
		}
		return BaseUI.boxed_false;
	}

	// Token: 0x0600195E RID: 6494 RVA: 0x000F630C File Offset: 0x000F450C
	public static Logic.Kingdom SelKingdom()
	{
		Logic.Object @object = BaseUI.SelLO();
		if (@object == null)
		{
			return null;
		}
		return @object.GetKingdom();
	}

	// Token: 0x0600195F RID: 6495 RVA: 0x000F632C File Offset: 0x000F452C
	public static Logic.Character SelChar()
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return null;
		}
		if (baseUI.selected_court_member != null)
		{
			return baseUI.selected_court_member.logic;
		}
		Logic.Object @object = BaseUI.SelLO();
		if (@object == null)
		{
			return null;
		}
		Logic.Character character = @object as Logic.Character;
		if (character != null)
		{
			return character;
		}
		Logic.Army army = @object as Logic.Army;
		if (army != null)
		{
			return army.leader;
		}
		Mercenary mercenary = @object as Mercenary;
		if (mercenary != null)
		{
			return mercenary.army.leader;
		}
		Logic.Rebel rebel = @object as Logic.Rebel;
		if (rebel != null)
		{
			return rebel.character;
		}
		Castle castle = @object as Castle;
		if (castle == null)
		{
			return null;
		}
		if (castle.army != null)
		{
			return castle.army.leader;
		}
		if (castle.governor != null)
		{
			return castle.governor;
		}
		return null;
	}

	// Token: 0x06001960 RID: 6496 RVA: 0x000F63E9 File Offset: 0x000F45E9
	public static string GetPietyIcon()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return null;
		}
		return kingdom.GetPietyIcon(false);
	}

	// Token: 0x06001961 RID: 6497 RVA: 0x000F63FC File Offset: 0x000F45FC
	public bool ValidateSoundQue(float new_priority = 1000f, string path = null)
	{
		if (path != null && this.soundQueue.Count > 0)
		{
			for (int i = 0; i < BaseUI.never_play_together.Count; i++)
			{
				if (!BaseUI.never_play_together[i].Validate(path, this.soundQueue))
				{
					return false;
				}
			}
		}
		for (int j = this.soundQueue.Count - 1; j >= 1; j--)
		{
			float start_time = this.soundQueue[j].Value.start_time;
			float timeout = this.soundQueue[j].Value.timeout;
			if (new_priority == 4f && this.soundQueue[j].Value.priority < 4f)
			{
				return false;
			}
			if (path != null && this.soundQueue[j].Value.path == path)
			{
				return false;
			}
			if ((timeout >= 0f && timeout + start_time < UnityEngine.Time.time) || (this.soundQueue[j].Value.que_clearing && this.soundQueue[j].Value.priority > new_priority))
			{
				if (BaseUI.log_sound_queue)
				{
					UnityEngine.Debug.Log("Removing " + this.soundQueue[j].Value.path + " from queue, either too late or priority is too low");
				}
				this.soundQueue.RemoveAt(j);
				break;
			}
		}
		return true;
	}

	// Token: 0x06001962 RID: 6498 RVA: 0x000F6580 File Offset: 0x000F4780
	public void AddToSoundQue(EventInstance instance, string path, PARAMETER_DESCRIPTION priority, bool que_clearing, float timeout, int index = -1)
	{
		if (!this.ValidateSoundQue(priority.defaultvalue, path))
		{
			return;
		}
		if (index < 0 && this.soundQueue.Count > 0)
		{
			if (priority.defaultvalue >= 3f)
			{
				if (BaseUI.log_sound_queue)
				{
					UnityEngine.Debug.Log("Can't play " + path + ", priority is too low and que isn't empty");
				}
				return;
			}
			if (UserSettings.InterruptMessages && this.soundQueue[0].Value.priority > priority.defaultvalue)
			{
				if (BaseUI.log_sound_queue)
				{
					UnityEngine.Debug.Log("Interrupting " + this.soundQueue[0].Value.path + ", playing " + path);
				}
				this.soundQueue[0].Key.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
				this.AddToSoundQue(instance, path, priority, que_clearing, timeout, 0);
				this.soundQueue[0].Key.start();
				this.soundQueue[0].Key.release();
				return;
			}
		}
		if (index >= 0 && index < this.soundQueue.Count)
		{
			this.soundQueue[index] = new KeyValuePair<EventInstance, BaseUI.MessageAudioSettings>(instance, new BaseUI.MessageAudioSettings
			{
				priority = priority.defaultvalue,
				que_clearing = que_clearing,
				timeout = timeout,
				start_time = UnityEngine.Time.time,
				path = path
			});
			return;
		}
		if (this.soundQueue.Count > UserSettings.MessageQueCap)
		{
			if (BaseUI.log_sound_queue)
			{
				UnityEngine.Debug.Log("Can't play " + path + ", que is full");
			}
			return;
		}
		if (BaseUI.log_sound_queue)
		{
			UnityEngine.Debug.Log("Adding " + path + " to queue");
		}
		this.soundQueue.Add(new KeyValuePair<EventInstance, BaseUI.MessageAudioSettings>(instance, new BaseUI.MessageAudioSettings
		{
			priority = priority.defaultvalue,
			que_clearing = que_clearing,
			timeout = timeout,
			start_time = UnityEngine.Time.time,
			path = path
		}));
		if (this.soundQueue.Count == 1)
		{
			if (BaseUI.log_sound_queue)
			{
				UnityEngine.Debug.Log("Playing " + path);
			}
			this.soundQueue[0].Key.start();
			this.soundQueue[0].Key.release();
		}
	}

	// Token: 0x06001963 RID: 6499 RVA: 0x000F6808 File Offset: 0x000F4A08
	private void UpdateSoundQueue()
	{
		if (this.soundQueue.Count > 0)
		{
			PLAYBACK_STATE playback_STATE;
			this.soundQueue[0].Key.getPlaybackState(out playback_STATE);
			if (playback_STATE == PLAYBACK_STATE.STOPPED)
			{
				this.soundQueue[0].Key.release();
				this.soundQueue.RemoveAt(0);
				this.ValidateSoundQue(1000f, null);
				if (this.soundQueue.Count > 0)
				{
					int num = 0;
					for (int i = 1; i < this.soundQueue.Count; i++)
					{
						float priority = this.soundQueue[i].Value.priority;
						float priority2 = this.soundQueue[num].Value.priority;
						if (priority < priority2 || (priority == priority2 && this.soundQueue[i].Value.start_time < this.soundQueue[num].Value.start_time))
						{
							num = i;
						}
					}
					if (num != 0)
					{
						KeyValuePair<EventInstance, BaseUI.MessageAudioSettings> value = this.soundQueue[num];
						this.soundQueue[num] = this.soundQueue[0];
						this.soundQueue[0] = value;
					}
					if (BaseUI.log_sound_queue)
					{
						UnityEngine.Debug.Log("Playing " + this.soundQueue[0].Value.path);
					}
					this.soundQueue[0].Key.start();
					this.soundQueue[0].Key.release();
				}
			}
		}
	}

	// Token: 0x06001964 RID: 6500 RVA: 0x000F69D0 File Offset: 0x000F4BD0
	public static void PlaySoundEvent(string eventPath, List<KeyValuePair<string, float>> parameters = null)
	{
		BaseUI.PlaySoundEvent(eventPath, Vector3.zero, parameters);
	}

	// Token: 0x06001965 RID: 6501 RVA: 0x000F69E0 File Offset: 0x000F4BE0
	public static void PlaySoundEvent(string eventPath, Vector3 position, List<KeyValuePair<string, float>> parameters = null)
	{
		if (LoadingScreen.IsShown())
		{
			return;
		}
		if (string.IsNullOrEmpty(eventPath))
		{
			return;
		}
		string[] array = new string[]
		{
			"Lowest Priority",
			"Low Priority",
			"Medium Priority",
			"High Priority",
			"Highest Priority"
		};
		EventInstance instance = FMODWrapper.CreateInstance(eventPath, true);
		EventDescription eventDescription = FMODWrapper.GetEventDescription(eventPath);
		USER_PROPERTY user_PROPERTY;
		if (eventDescription.getUserProperty("unique", out user_PROPERTY) == RESULT.OK)
		{
			BaseUI baseUI = BaseUI.Get();
			for (int i = 0; i < baseUI.soundQueue.Count; i++)
			{
				if (baseUI.soundQueue[i].Value.path == eventPath)
				{
					return;
				}
			}
		}
		PARAMETER_DESCRIPTION parameter_DESCRIPTION = default(PARAMETER_DESCRIPTION);
		parameter_DESCRIPTION.defaultvalue = -1f;
		int num = 0;
		while (num < array.Length && eventDescription.getParameterDescriptionByName(array[num], out parameter_DESCRIPTION) != RESULT.OK)
		{
			parameter_DESCRIPTION.defaultvalue = -1f;
			num++;
		}
		bool que_clearing = true;
		float timeout = -1f;
		USER_PROPERTY user_PROPERTY2;
		if (eventDescription.getUserProperty("que_clearing", out user_PROPERTY2) == RESULT.OK)
		{
			que_clearing = (user_PROPERTY2.floatValue() == 1f);
		}
		USER_PROPERTY user_PROPERTY3;
		if (eventDescription.getUserProperty("timeout", out user_PROPERTY3) == RESULT.OK)
		{
			timeout = user_PROPERTY3.floatValue();
		}
		instance.set3DAttributes(new Vector3(position.x, position.y - Mathf.Max(parameter_DESCRIPTION.defaultvalue, 0f) * 100f, position.z).To3DAttributes());
		if (parameters != null)
		{
			foreach (KeyValuePair<string, float> keyValuePair in parameters)
			{
				instance.setParameterByName(keyValuePair.Key, keyValuePair.Value, false);
			}
		}
		if (parameter_DESCRIPTION.defaultvalue >= 0f && parameter_DESCRIPTION.defaultvalue != 4f)
		{
			BaseUI baseUI2 = BaseUI.Get();
			if (BaseUI.log_sound_queue)
			{
				UnityEngine.Debug.Log("Trying to play " + eventPath);
			}
			baseUI2.AddToSoundQue(instance, eventPath, parameter_DESCRIPTION, que_clearing, timeout, -1);
			return;
		}
		instance.start();
		instance.release();
	}

	// Token: 0x06001966 RID: 6502 RVA: 0x000F6C00 File Offset: 0x000F4E00
	public static void PlaySound(DT.Field field)
	{
		if (field == null)
		{
			return;
		}
		List<string> list = new List<string>();
		List<string> list2 = field.Keys(true, true);
		string text;
		for (int i = 0; i < list2.Count; i++)
		{
			text = (field.GetValue(list2[i], null, true, true, true, '.').obj_val as string);
			if (text != null)
			{
				list.Add(text);
			}
		}
		if (list.Count < 1)
		{
			text = (field.Value(null, true, true).obj_val as string);
			if (text != null)
			{
				list.Add(text);
			}
		}
		if (list.Count < 1)
		{
			return;
		}
		text = list[Random.Range(0, list.Count)];
		BaseUI.PlaySoundEvent(text, null);
	}

	// Token: 0x06001967 RID: 6503 RVA: 0x000F6CA4 File Offset: 0x000F4EA4
	public static void PlaySelectionSound(GameObject selectedObject, bool clicked = true)
	{
		if (selectedObject == null)
		{
			return;
		}
		bool flag = ViewMode.IsPoliticalView();
		string text = null;
		string text2 = null;
		Vars vars = new Vars();
		List<KeyValuePair<string, float>> list = new List<KeyValuePair<string, float>>();
		Vector3 position = Vector3.zero;
		if (flag)
		{
			global::Settlement component = selectedObject.GetComponent<global::Settlement>();
			if (component != null && component.setType == "Castle")
			{
				text = (clicked ? "select_town_pv" : "select_town_tab");
			}
		}
		else
		{
			if (selectedObject.GetComponent<global::Battle>() != null)
			{
				text = "select_battle";
			}
			if (text == null)
			{
				global::Settlement component2 = selectedObject.GetComponent<global::Settlement>();
				if (component2 != null)
				{
					if (component2.setType == "Castle")
					{
						Castle castle = component2.logic as Castle;
						if (component2.logic.battle != null)
						{
						}
						if (castle != null && castle.sacked)
						{
							list.Add(new KeyValuePair<string, float>("SettlementState", 7f));
						}
						text = (clicked ? "select_town" : "select_town_tab");
					}
					else if (component2.logic.razed)
					{
						text = "select_settlement_razed";
					}
					else
					{
						text2 = component2.logic.def.field.GetString("selection_sound", component2.logic, "", true, true, true, '.');
					}
					position = component2.transform.position;
				}
			}
			if (text == null && text2 == null)
			{
				global::Army component3 = selectedObject.GetComponent<global::Army>();
				if (component3 != null && component3.logic != null)
				{
					Logic.Character leader = component3.logic.leader;
					string text3 = null;
					if (component3.logic.battle != null)
					{
						text = "select_battle";
						if (leader != null && BaseUI.LogicKingdom().court.Contains(leader))
						{
							if (component3.logic.battle.is_siege)
							{
								if (component3.logic.battle.defenders.Contains(component3.logic))
								{
									text3 = leader.GetVoiceLine("greet_defending_siege_voice_line");
								}
								else
								{
									text3 = leader.GetVoiceLine("greet_besieging_voice_line");
								}
							}
							else
							{
								text3 = leader.GetVoiceLine("greet_in_battle_voice_line");
							}
						}
					}
					else if (component3.logic.rebel != null)
					{
						if (component3.logic.isCamping)
						{
							text = "select_rebel_camp";
						}
						else
						{
							text = "select_rebels";
						}
						if (component3.logic.rebel.loyal_to == BaseUI.LogicKingdom().id)
						{
							text3 = leader.GetVoiceLine("greet_rebel_our_loyalist_voice_line");
						}
						else
						{
							text3 = leader.GetVoiceLine("greet_rebel_voice_line");
						}
					}
					else if (component3.logic.mercenary != null)
					{
						if (component3.logic.isCamping)
						{
							text = "select_mercenary_camp";
						}
						else
						{
							text = "select_mercenary";
						}
						if (component3.GetKingdomID() == BaseUI.LogicKingdom().id)
						{
							text3 = leader.GetVoiceLine("select_character_voice_line");
						}
						else if (!component3.logic.mercenary.IsHired())
						{
							text3 = leader.GetVoiceLine("greet_unhired_mercenary");
						}
					}
					else
					{
						if (component3.logic.isCamping)
						{
							text = "select_army_camp";
						}
						else
						{
							text = "select_army";
						}
						if (leader != null)
						{
							if (leader.IsCrusader() && BaseUI.LogicKingdom().GetKing() != leader)
							{
								text3 = leader.GetVoiceLine("greet_crusader_voice_line");
							}
							else if (BaseUI.LogicKingdom().court.Contains(leader))
							{
								if (component3.logic.leader.IsMarshal() && component3.logic.realm_in.IsSeaRealm())
								{
									text3 = leader.GetVoiceLine("greet_at_sea_voice_line");
								}
								else
								{
									text3 = leader.GetVoiceLine("select_character_voice_line");
								}
							}
						}
					}
					if (!string.IsNullOrEmpty(text3))
					{
						BaseUI.PlayVoiceEvent(text3, leader);
					}
				}
			}
		}
		if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2))
		{
			return;
		}
		if (text2 == null)
		{
			DT.Field field = BaseUI.soundsDef;
			text2 = ((field != null) ? field.GetString(text, vars, "", true, true, true, '.') : null);
		}
		if (!string.IsNullOrEmpty(text2))
		{
			BaseUI.PlaySoundEvent(text2, position, list);
		}
	}

	// Token: 0x06001968 RID: 6504 RVA: 0x000F70B8 File Offset: 0x000F52B8
	public static void PlayCourtMemberVoiceLine(Logic.Character character)
	{
		if (character != null && character.kingdom_id == BaseUI.LogicKingdom().id && !character.IsDead())
		{
			if (character.IsPrisoner())
			{
				BaseUI.PlayVoiceEvent(character.GetVoiceLine("select_own_prisoner_voice_line"), character);
				return;
			}
			BaseUI.PlayVoiceEvent(character.GetVoiceLine("select_character_voice_line"), character);
			string key = "select_character_sound_effect";
			CharacterClass.Def class_def = character.class_def;
			if (character.GetArmy() != null)
			{
				class_def = character.game.defs.Get<CharacterClass.Def>("Marshal");
			}
			BaseUI.PlaySoundEvent(Logic.Character.GetSoundEffect(class_def, key), null);
		}
	}

	// Token: 0x06001969 RID: 6505 RVA: 0x000F7145 File Offset: 0x000F5345
	public static void PlayVoiceEvent(string eventPath, IVars vars, Vector3 position)
	{
		FMODVoiceProvider.PlayVoiceEvent(eventPath, vars, position);
	}

	// Token: 0x0600196A RID: 6506 RVA: 0x000F714F File Offset: 0x000F534F
	public static void PlayVoiceEvent(string eventPath, IVars vars)
	{
		BaseUI.PlayVoiceEvent(eventPath, vars, Vector3.zero);
	}

	// Token: 0x0600196B RID: 6507 RVA: 0x000F715D File Offset: 0x000F535D
	public static void PlayVoiceEvent(DT.Field field, IVars vars)
	{
		BaseUI.PlayVoiceEvent(((field != null) ? field.Value(null, true, true).obj_val : null) as string, vars);
	}

	// Token: 0x0600196C RID: 6508 RVA: 0x000F7180 File Offset: 0x000F5380
	public static void PlayCharacterlessVoiceEvent(Logic.Character character, string key)
	{
		Vars vars = new Vars();
		vars.Set<Logic.Character>("character", character);
		vars.Set<bool>("no_character_allowed", true);
		if (character != null)
		{
			BaseUI.PlayVoiceEvent(character.GetVoiceLine(key), vars);
			return;
		}
		string eventPath;
		GameLogic.Get(true).defs.Get<CharacterClass.Def>("Marshal").voice_lines.TryGetValue(key, out eventPath);
		BaseUI.PlayVoiceEvent(eventPath, vars);
	}

	// Token: 0x0600196D RID: 6509 RVA: 0x000F71E6 File Offset: 0x000F53E6
	public static void PlayVoiceLine(string key, Vector3 position, Voices.VoiceLine voiceLine)
	{
		FMODVoiceProvider.PlayVoiceLine(key, position, voiceLine);
	}

	// Token: 0x0600196E RID: 6510 RVA: 0x000F71F0 File Offset: 0x000F53F0
	private GameObject GetTooltipPrefab()
	{
		GameObject gameObject = this.tooltip.prefab;
		if (gameObject == null && this.tooltip.def != null)
		{
			gameObject = global::Defs.GetObj<GameObject>(this.tooltip.def.field, "prefab", null);
		}
		if (gameObject == null && this.tooltip.GetCaption() != "")
		{
			gameObject = this.tooltipSettings.CaptionPrefab;
		}
		if (gameObject == null && this.tooltip.GetText() != "")
		{
			gameObject = this.tooltipSettings.SimplePrefab;
		}
		return gameObject;
	}

	// Token: 0x0600196F RID: 6511 RVA: 0x000F7294 File Offset: 0x000F5494
	private void CreateTooltip()
	{
		if (this.tooltip == null || this.tooltip.instance != null)
		{
			return;
		}
		DT.Def def = this.tooltip.def;
		DT.Field field;
		if (def == null)
		{
			field = null;
		}
		else
		{
			DT.Field field2 = def.field;
			field = ((field2 != null) ? field2.FindChild("visible_condition", null, true, true, true, '.') : null);
		}
		DT.Field field3 = field;
		if (field3 != null && !field3.Bool(this.tooltip.vars, false))
		{
			return;
		}
		if (this.tooltip.CallHandler(this, Tooltip.Event.Show))
		{
			if (this.tooltip.instance == null)
			{
				this.tooltip = null;
			}
			return;
		}
		if (this.tooltip.instance != null)
		{
			return;
		}
		GameObject tooltipPrefab = this.GetTooltipPrefab();
		if (tooltipPrefab == null)
		{
			return;
		}
		this.tooltip.instance = global::Common.Spawn(tooltipPrefab, this.tCanvas, false, "");
		this.tooltip.instance.transform.localScale = Vector3.one;
		Canvas component = this.tooltip.instance.GetComponent<Canvas>();
		if (component != null)
		{
			component.overrideSorting = true;
			component.sortingOrder = 32767;
		}
		CanvasGroup orAddComponent = this.tooltip.instance.gameObject.GetOrAddComponent<CanvasGroup>();
		orAddComponent.blocksRaycasts = false;
		orAddComponent.interactable = false;
		TooltipInstance orAddComponent2 = this.tooltip.instance.GetOrAddComponent<TooltipInstance>();
		if (orAddComponent2 != null)
		{
			orAddComponent2.Init(this.tooltip, this.tooltip.vars);
		}
	}

	// Token: 0x06001970 RID: 6512 RVA: 0x000F7410 File Offset: 0x000F5610
	public void DestroyTooltip()
	{
		if (this.tooltip == null)
		{
			return;
		}
		if (this.tooltip.instance != null && !this.IsPinned(this.tooltip))
		{
			this.tooltip.CallHandler(this, Tooltip.Event.Hide);
			this.DestroyTooltipInstance(this.tooltip.instance);
			this.tooltip.instance = null;
		}
		this.tooltip = null;
	}

	// Token: 0x06001971 RID: 6513 RVA: 0x000F7480 File Offset: 0x000F5680
	public void FillTooltip(Tooltip tooltip)
	{
		if (tooltip == null || tooltip.instance == null)
		{
			return;
		}
		Tooltip tooltip2 = this.tooltip;
		this.tooltip = tooltip;
		if (tooltip.CallHandler(this, Tooltip.Event.Fill))
		{
			tooltip = tooltip2;
			return;
		}
		UIHyperText componentInChildren = tooltip.instance.GetComponentInChildren<UIHyperText>();
		if (componentInChildren != null)
		{
			DT.Def def = tooltip.def;
			DT.Field field;
			if (def == null)
			{
				field = null;
			}
			else
			{
				DT.Field field2 = def.field;
				field = ((field2 != null) ? field2.GetRef("hypertext", tooltip.vars, true, true, true, '.') : null);
			}
			DT.Field field3 = field;
			if (field3 != null)
			{
				componentInChildren.Load(field3, tooltip.vars);
			}
			tooltip = tooltip2;
			return;
		}
		bool flag = true;
		LayoutGroup component = tooltip.instance.GetComponent<LayoutGroup>();
		TextAnchor childAlignment;
		if (component != null && BaseUI.ParseAlignment(tooltip.alignment, out childAlignment))
		{
			component.childAlignment = childAlignment;
		}
		TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(tooltip.instance, "Text");
		if (textMeshProUGUI != null)
		{
			if (tooltip.text == null)
			{
				tooltip.text = tooltip.GetText();
			}
			UIText.SetText(textMeshProUGUI, tooltip.text);
			if (!string.IsNullOrEmpty(tooltip.text))
			{
				flag = false;
			}
			BaseUI.Fit(textMeshProUGUI, tooltip);
		}
		TextMeshProUGUI textMeshProUGUI2 = global::Common.FindChildComponent<TextMeshProUGUI>(tooltip.instance, "Caption");
		if (textMeshProUGUI2 != null)
		{
			if (tooltip.caption == null)
			{
				tooltip.caption = tooltip.GetCaption();
			}
			UIText.SetText(textMeshProUGUI2, tooltip.caption);
			if (!string.IsNullOrEmpty(tooltip.caption))
			{
				flag = false;
			}
			BaseUI.Fit(textMeshProUGUI2, tooltip);
		}
		Image image = global::Common.FindChildComponent<Image>(tooltip.instance, "id_Icon");
		if (image != null)
		{
			if (tooltip.icon == null)
			{
				tooltip.icon = tooltip.GetIcon();
			}
			image.overrideSprite = tooltip.icon;
			flag = false;
		}
		if (flag)
		{
			this.DestroyTooltip();
		}
		if (tooltip2 != tooltip)
		{
			this.tooltip = tooltip2;
		}
	}

	// Token: 0x06001972 RID: 6514 RVA: 0x000F7650 File Offset: 0x000F5850
	private static bool ParseAlignment(string key, out TextAnchor r)
	{
		if (key == "left")
		{
			r = TextAnchor.MiddleLeft;
			return true;
		}
		if (key == "center")
		{
			r = TextAnchor.MiddleCenter;
			return true;
		}
		if (!(key == "right"))
		{
			r = TextAnchor.MiddleLeft;
			return false;
		}
		r = TextAnchor.MiddleRight;
		return true;
	}

	// Token: 0x06001973 RID: 6515 RVA: 0x000F7690 File Offset: 0x000F5890
	private static void Fit(TextMeshProUGUI text, Tooltip tooltip)
	{
		LayoutElement layoutElement = text.GetComponent<LayoutElement>();
		if (layoutElement == null)
		{
			layoutElement = text.gameObject.AddComponent<LayoutElement>();
		}
		layoutElement.preferredWidth = tooltip.max_width;
		layoutElement.minWidth = tooltip.min_width;
		layoutElement.flexibleWidth = 1f;
		LayoutRebuilder.ForceRebuildLayoutImmediate(tooltip.instance.transform as RectTransform);
		text.ForceMeshUpdate();
		TMP_TextInfo textInfo = text.textInfo;
		Bounds textBounds = text.textBounds;
		if (textInfo.lineCount >= 1)
		{
			if (textBounds.size.x < tooltip.min_width)
			{
				layoutElement.preferredWidth = tooltip.min_width;
				return;
			}
			if (textBounds.size.x > tooltip.max_width)
			{
				layoutElement.preferredWidth = tooltip.max_width;
				return;
			}
			layoutElement.preferredWidth = textBounds.size.x + 2f;
		}
	}

	// Token: 0x06001974 RID: 6516 RVA: 0x000F7768 File Offset: 0x000F5968
	public void PlaceTooltip()
	{
		if (this.tooltip == null || this.tooltip.instance == null)
		{
			return;
		}
		if (this.tooltip.CallHandler(this, Tooltip.Event.Place))
		{
			return;
		}
		RectTransform component = this.tooltip.instance.GetComponent<RectTransform>();
		if (component == null)
		{
			return;
		}
		LayoutRebuilder.MarkLayoutForRebuild(component);
		LayoutRebuilder.ForceRebuildLayoutImmediate(component);
		Canvas.ForceUpdateCanvases();
		if (TooltipPlacement.PlaceTooltip(component, this.tooltip, this.canvas))
		{
			return;
		}
		Transform transform = null;
		Transform transform2 = this.tooltip.transform;
		while (transform2 != null)
		{
			Transform transform3 = transform2.Find("TooltipPos");
			if (transform3 != null)
			{
				transform = transform3;
				break;
			}
			transform2 = transform2.parent;
		}
		TooltipOrigin tooltipOrigin;
		if (TooltipOrigin.TryGetOrigin(out tooltipOrigin))
		{
			transform = tooltipOrigin.transform;
		}
		if (transform != null)
		{
			RectTransform component2 = transform.GetComponent<RectTransform>();
			if (component2 != null)
			{
				component.pivot = component2.pivot;
				component.position = component2.position;
				return;
			}
		}
		if (this.canvas.renderMode == RenderMode.ScreenSpaceCamera)
		{
			this.CalcForScreenSpaceCameraCanvas(component, transform);
			return;
		}
		if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			this.CalcForScreenSpaceOverlayCanvas(component, transform);
			return;
		}
		if (this.canvas.renderMode == RenderMode.WorldSpace)
		{
			this.CalcForWorldSpaceCanvas(component, transform);
			return;
		}
		this.CalcForWorldSpaceCanvas(component, transform);
	}

	// Token: 0x06001975 RID: 6517 RVA: 0x000F78B8 File Offset: 0x000F5AB8
	private void CalcForScreenSpaceOverlayCanvas(RectTransform ttRect, Transform tPos)
	{
		Vector3[] array = new Vector3[4];
		ttRect.GetWorldCorners(array);
		Vector3 vector = (array[2] - array[0]) / 2f;
		Vector3 position = ttRect.position;
		float num = 10f;
		RectTransform component = this.tooltip.GetComponent<RectTransform>();
		Vector3 vector4;
		if (component != null)
		{
			Vector3[] array2 = new Vector3[4];
			component.GetWorldCorners(array2);
			Vector3 vector2 = (array2[0] + array2[2]) / 2f;
			Vector3 vector3 = (array2[2] - array2[0]) / 2f;
			vector4 = vector2;
			num += vector3.y;
		}
		else
		{
			vector4 = CameraController.MainCamera.WorldToScreenPoint((tPos != null) ? tPos.position : this.tooltip.transform.position);
		}
		Vector2 pivot = new Vector2(0.5f, 0f);
		if (vector4.y + vector.y * 2f + num > (float)Screen.height)
		{
			vector4.y -= num;
			pivot.y = 1f;
		}
		else
		{
			vector4.y += num;
		}
		if (vector4.x - vector.x < 0f)
		{
			vector4.x = 0f;
			pivot.x = 0f;
		}
		else if (vector4.x + vector.x >= (float)Screen.width)
		{
			vector4.x = (float)(Screen.width - 1);
			pivot.x = 1f;
		}
		Vector3 position2 = vector4;
		position2.z = position.z;
		ttRect.pivot = pivot;
		ttRect.transform.position = position2;
	}

	// Token: 0x06001976 RID: 6518 RVA: 0x000F7A7C File Offset: 0x000F5C7C
	private void CalcForScreenSpaceCameraCanvas(RectTransform ttRect, Transform tPos)
	{
		Vector3[] array = new Vector3[4];
		ttRect.GetWorldCorners(array);
		Vector3 vector = (array[2] - array[0]) / 2f;
		Vector3 position = ttRect.position;
		float num = 10f * this.tCanvas.lossyScale.x;
		RectTransform component = this.tooltip.GetComponent<RectTransform>();
		Vector3 vector4;
		if (component != null)
		{
			Vector3[] array2 = new Vector3[4];
			component.GetWorldCorners(array2);
			Vector3 vector2 = (array2[0] + array2[2]) / 2f;
			Vector3 vector3 = (array2[2] - array2[0]) / 2f;
			vector4 = vector2;
			num += vector3.y;
		}
		else
		{
			vector4 = CameraController.MainCamera.WorldToScreenPoint((tPos != null) ? tPos.position : this.tooltip.transform.position);
		}
		float num2 = (float)Screen.width * this.tCanvas.lossyScale.x;
		float num3 = (float)Screen.height * this.tCanvas.lossyScale.x;
		Vector2 pivot = new Vector2(0.5f, 0f);
		if (vector4.y > num3 * 0.75f)
		{
			vector4.y -= num;
			pivot.y = 1f;
		}
		else
		{
			vector4.y += num;
		}
		if (vector4.x - vector.x < -num2 / 2f)
		{
			vector4.x = -num2 / 2f;
			pivot.x = 0f;
		}
		else if (vector4.x + vector.x >= num2 / 2f)
		{
			vector4.x = num2 / 2f;
			pivot.x = 1f;
		}
		Vector3 position2 = vector4;
		position2.z = position.z;
		ttRect.pivot = pivot;
		ttRect.transform.position = position2;
	}

	// Token: 0x06001977 RID: 6519 RVA: 0x000023FD File Offset: 0x000005FD
	private void CalcForWorldSpaceCanvas(RectTransform ttRect, Transform tPos)
	{
	}

	// Token: 0x06001978 RID: 6520 RVA: 0x000F7C80 File Offset: 0x000F5E80
	private void SetTooltip(Tooltip tt)
	{
		this.DestroyTooltip();
		if (tt == null || !BaseUI.show_tooltips || this.IsPinned(tt))
		{
			return;
		}
		tt.CallHandler(this, Tooltip.Event.Update);
		this.tooltip = tt;
		this.CreateTooltip();
		this.FillTooltip(this.tooltip);
		this.PlaceTooltip();
	}

	// Token: 0x06001979 RID: 6521 RVA: 0x000F7CD5 File Offset: 0x000F5ED5
	public void RefreshTooltip(Tooltip tt, bool full = false)
	{
		if (tt == null)
		{
			return;
		}
		if (full)
		{
			if (this.tooltip != tt)
			{
				return;
			}
			this.SetTooltip(tt);
			return;
		}
		else
		{
			if (tt.instance == null)
			{
				return;
			}
			this.FillTooltip(tt);
			return;
		}
	}

	// Token: 0x0600197A RID: 6522 RVA: 0x000F7D14 File Offset: 0x000F5F14
	public bool IsOverNameplateOnly()
	{
		if (BaseUI.tmp_reycast_results == null || BaseUI.tmp_reycast_results.Count == 0)
		{
			return false;
		}
		bool flag = false;
		for (int i = 0; i < BaseUI.tmp_reycast_results.Count; i++)
		{
			bool flag2 = false;
			GameObject gameObject = BaseUI.tmp_reycast_results[i].gameObject;
			if (!(gameObject == null))
			{
				Transform transform = gameObject.transform;
				while (transform != null)
				{
					if (transform == this.m_statusBar)
					{
						flag |= true;
						flag2 = true;
						break;
					}
					transform = transform.parent;
				}
				if (!flag2)
				{
					return false;
				}
			}
		}
		return flag;
	}

	// Token: 0x0600197B RID: 6523 RVA: 0x000F7DA8 File Offset: 0x000F5FA8
	public bool IsMousePanEligable()
	{
		if (this.menu != null && this.menu.gameObject.activeSelf)
		{
			return false;
		}
		EventSystem eventSystem = EventSystem.current;
		bool flag = eventSystem != null && eventSystem.IsPointerOverGameObject();
		flag |= IMGUIHandler.IsPointerOverIMGUI();
		if (flag)
		{
			flag = !BaseUI.Get().IsOverNameplateOnly();
		}
		return !flag;
	}

	// Token: 0x0600197C RID: 6524 RVA: 0x000F7E10 File Offset: 0x000F6010
	private void ParceUIReycastHits(List<RaycastResult> hits, Tooltip cur_tt, Tooltip ui_tt, GameObject ui_element)
	{
		ui_tt = null;
		ui_element = null;
		for (int i = 0; i < hits.Count; i++)
		{
			RaycastResult raycastResult = hits[i];
			if (!(raycastResult.gameObject == this.tutorial_mouse_blocker))
			{
				Tooltip tooltip = this.tooltip;
				if (!global::Common.IsParent((tooltip != null) ? tooltip.instance : null, raycastResult.gameObject))
				{
					if (!Tutorial.active)
					{
						ui_element = raycastResult.gameObject;
						cur_tt = Tooltip.FindInParents(raycastResult.gameObject);
						return;
					}
					if (ui_element == null)
					{
						ui_element = raycastResult.gameObject;
					}
					if (ui_tt == null)
					{
						ui_tt = Tooltip.FindInParents(raycastResult.gameObject);
						if (ui_tt == null)
						{
							Tutorial.HotspotDef hotspotDef = Tutorial.ResolveHotspotDef(global::Common.ObjPath(ui_element), null, null);
							if (hotspotDef == null || !hotspotDef.mouse_transparent)
							{
								break;
							}
						}
						else
						{
							Tooltip main_tooltip = ui_tt.main_tooltip;
							if (!(main_tooltip.tutorial_highlight_obj == null) && main_tooltip.tutorial_highlight_obj.activeInHierarchy)
							{
								break;
							}
							ui_tt = null;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600197D RID: 6525 RVA: 0x000F7F10 File Offset: 0x000F6110
	protected void UpdateTooltip(Tooltip tt)
	{
		GameObject gameObject = null;
		if (this.raycaster != null)
		{
			BaseUI.tmp_reycast_results.Clear();
			BaseUI.pointerData.position = Input.mousePosition;
			this.raycaster.Raycast(BaseUI.pointerData, BaseUI.tmp_reycast_results);
			Tooltip tooltip = null;
			for (int i = 0; i < BaseUI.tmp_reycast_results.Count; i++)
			{
				RaycastResult raycastResult = BaseUI.tmp_reycast_results[i];
				if (!(raycastResult.gameObject == this.tutorial_mouse_blocker))
				{
					Tooltip tooltip2 = this.tooltip;
					if (!global::Common.IsParent((tooltip2 != null) ? tooltip2.instance : null, raycastResult.gameObject))
					{
						if (!Tutorial.active)
						{
							gameObject = raycastResult.gameObject;
							tt = Tooltip.FindInParents(raycastResult.gameObject);
							break;
						}
						if (gameObject == null)
						{
							gameObject = raycastResult.gameObject;
						}
						if (tooltip == null)
						{
							tooltip = Tooltip.FindInParents(raycastResult.gameObject);
							if (tooltip == null)
							{
								Tutorial.HotspotDef hotspotDef = Tutorial.ResolveHotspotDef(global::Common.ObjPath(gameObject), null, null);
								if (hotspotDef == null || !hotspotDef.mouse_transparent)
								{
									break;
								}
							}
							else
							{
								Tooltip main_tooltip = tooltip.main_tooltip;
								if (!(main_tooltip.tutorial_highlight_obj == null) && main_tooltip.tutorial_highlight_obj.activeInHierarchy)
								{
									break;
								}
								tooltip = null;
							}
						}
					}
				}
			}
			if (tooltip != null)
			{
				tt = tooltip;
			}
			if (gameObject != null)
			{
				BaseUI.UpdateTextLink(gameObject);
				tt = (BaseUI.picked_link_tooltip ?? tt);
			}
			else
			{
				BaseUI.UpdateTextLink(null);
			}
		}
		if (this.raycasters != null && this.raycasters.Count > 0)
		{
			bool flag = true;
			gameObject = null;
			for (int j = this.raycasters.Count - 1; j >= 0; j--)
			{
				BaseUI.tmp_reycast_results.Clear();
				BaseUI.pointerData.position = Input.mousePosition;
				if (!(this.raycasters[j] == null))
				{
					this.raycasters[j].Raycast(BaseUI.pointerData, BaseUI.tmp_reycast_results);
					for (int k = 0; k < BaseUI.tmp_reycast_results.Count; k++)
					{
						RaycastResult raycastResult2 = BaseUI.tmp_reycast_results[k];
						Tooltip tooltip3 = this.tooltip;
						if (!global::Common.IsParent((tooltip3 != null) ? tooltip3.instance : null, raycastResult2.gameObject))
						{
							gameObject = raycastResult2.gameObject;
							break;
						}
					}
					if (gameObject != null)
					{
						BaseUI.UpdateTextLink(gameObject);
						flag = false;
						tt = Tooltip.FindInParents(gameObject);
						if (tt != null)
						{
							break;
						}
					}
				}
			}
			if (flag)
			{
				BaseUI.UpdateTextLink(null);
			}
		}
		if (this.picked_UI_element != gameObject)
		{
			this.picked_UI_element = gameObject;
			Hotspot y = (gameObject == null) ? null : gameObject.GetComponentInParent<Hotspot>();
			BSGButton y2 = (gameObject == null) ? null : gameObject.GetComponentInParent<BSGButton>();
			if (this.picked_hotspot != y || this.picked_button != y2)
			{
				this.picked_hotspot = y;
				this.picked_button = y2;
				if (BaseUI.on_picked_element_changed != null)
				{
					BaseUI.on_picked_element_changed(this);
				}
			}
		}
		if (tt == null && CameraController.MainCamera != null)
		{
			int num = Physics.RaycastNonAlloc(CameraController.MainCamera.ScreenPointToRay(Input.mousePosition), this.hits, 10000f, this.layerMask, QueryTriggerInteraction.Ignore);
			int num2 = 0;
			if (num2 < num)
			{
				Tooltip tooltip4 = Tooltip.FindInParents(this.hits[num2].collider.gameObject);
				if (tooltip4 != null)
				{
					tt = tooltip4;
				}
			}
		}
		if (Tutorial.active && tt != null && !tt.is_tutorial_tooltip && !tt.IsWikiLinkTooltip())
		{
			tt = tt.tutorial_tooltip;
		}
		if (tt == this.pinned_tooltip)
		{
			tt = null;
		}
		if (tt != null && this.pinned_tooltip != null && this.pinned_tooltip.obj != null && tt.obj == this.pinned_tooltip.obj && this.pinned_tooltip == tt)
		{
			tt = null;
		}
		if (this.tooltip == tt)
		{
			if (tt != null)
			{
				TooltipPlacement.Update();
			}
			return;
		}
		if (tt != null && UnityEngine.Time.unscaledTime - this.tmLastMouseMove < tt.delay)
		{
			tt = null;
		}
		this.SetTooltip(tt);
	}

	// Token: 0x0600197E RID: 6526 RVA: 0x000F8358 File Offset: 0x000F6558
	protected void SetCursor(DT.Field field)
	{
		if (this.active_cursor != field)
		{
			if (field != null)
			{
				Cursor.SetCursor(field.GetValue("tex", null, true, true, true, '.').Get<Texture2D>(), field.GetPoint("hotspot", null, true, true, true, '.'), CursorMode.Auto);
			}
			this.active_cursor = field;
		}
	}

	// Token: 0x0600197F RID: 6527 RVA: 0x000F83B0 File Offset: 0x000F65B0
	public virtual string DecideCursorType(Logic.Army army)
	{
		Logic.Army army2 = this.picked_map_object as Logic.Army;
		Logic.Settlement settlement = this.picked_map_object as Logic.Settlement;
		Logic.Battle battle;
		if ((battle = (this.picked_map_object as Logic.Battle)) == null)
		{
			battle = (((army2 != null) ? army2.battle : null) ?? ((settlement != null) ? settlement.battle : null));
		}
		Logic.Battle battle2 = battle;
		if (battle2 != null)
		{
			if (battle2.CanBreakSiege(army))
			{
				return "BreakSiege_Cursor";
			}
			if (!battle2.CanJoin(army))
			{
				return "CantAttack_Cursor";
			}
			if (battle2.type == Logic.Battle.Type.Siege && battle2.GetJoinSide(army, true) != 2)
			{
				return "Siege_Cursor";
			}
			return "Attack_Cursor";
		}
		else if (settlement != null)
		{
			if (settlement.keep_effects == null || settlement.keep_effects.GetController() != army.GetKingdom())
			{
				if (!settlement.razed && settlement.IsActiveSettlement() && army.IsEnemy(settlement))
				{
					if (Logic.Battle.CanPillage(army, settlement))
					{
						return "Pillage_Cursor";
					}
					if (settlement.keep_effects != null)
					{
						if (Logic.Battle.CanSiege(army))
						{
							return "Siege_Cursor";
						}
						return "CantSiege_Cursor";
					}
				}
				return "Interactable_Cursor";
			}
			if (settlement.type == "Castle" && army.castle != settlement)
			{
				return "Enter_Cursor";
			}
			return "Interactable_Cursor";
		}
		else
		{
			if (army2 == null)
			{
				return "Interactable_Cursor";
			}
			if (army2.kingdom_id == army.kingdom_id)
			{
				if (army.IsMercenary())
				{
					if (!army.IsHiredMercenary())
					{
						return "Trade_Cursor";
					}
				}
				else if (army2.IsMercenary())
				{
					if (!army2.IsHiredMercenary())
					{
						return "Trade_Cursor";
					}
				}
				else if (army != army2)
				{
					return "UnitTransfer_Cursor";
				}
				return "Interactable_Cursor";
			}
			if (army.IsEnemy(army2))
			{
				return "Attack_Cursor";
			}
			if (army2.IsMercenary() && !army2.IsHiredMercenary())
			{
				return "Trade_Cursor";
			}
			return "Interactable_Cursor";
		}
	}

	// Token: 0x06001980 RID: 6528 RVA: 0x000F8554 File Offset: 0x000F6754
	public virtual void SetCursor(Vector3 pos)
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		Logic.Realm realm = game.GetRealm(pos);
		if (realm == null)
		{
			return;
		}
		DT.Field defField = global::Defs.GetDefField("Cursors", null);
		DT.Field cursor = defField.FindChild("Normal_Cursor", null, true, true, true, '.');
		if (!Application.isFocused)
		{
			this.SetCursor(cursor);
			return;
		}
		if (Tutorial.active)
		{
			cursor = defField.FindChild("Tutorial_Cursor", null, true, true, true, '.');
			this.SetCursor(cursor);
			return;
		}
		EventSystem eventSystem = EventSystem.current;
		if ((eventSystem != null) ? eventSystem.IsPointerOverGameObject() : IMGUIHandler.IsPointerOverIMGUI())
		{
			if (Hotspot.picked != null || BSGButton.picked != null)
			{
				cursor = defField.FindChild("Normal_Cursor_Highlight", null, true, true, true, '.');
			}
			if (Tutorial.cur_message_wnd != null)
			{
				cursor = defField.FindChild("Tutorial_Cursor", null, true, true, true, '.');
			}
			this.SetCursor(cursor);
			return;
		}
		Logic.Army army = this.selected_logic_obj as Logic.Army;
		if (army == null)
		{
			Castle castle = this.selected_logic_obj as Castle;
			if (castle != null)
			{
				army = castle.army;
			}
		}
		if (ViewMode.IsPoliticalView() || army == null || army.GetKingdom() != BaseUI.LogicKingdom() || (army.IsMercenary() && !BaseUI.CanControlAI()))
		{
			if (this.picked_map_object != null)
			{
				cursor = defField.FindChild("Interactable_Cursor", null, true, true, true, '.');
			}
			this.SetCursor(cursor);
			return;
		}
		if (this.picked_map_object != null)
		{
			string path = this.DecideCursorType(army);
			cursor = defField.FindChild(path, null, true, true, true, '.');
			this.SetCursor(cursor);
			return;
		}
		bool flag = false;
		PPos ppos = pos;
		if (game == null || game.path_finding == null || game.path_finding.data == null || !game.path_finding.data.initted)
		{
			cursor = defField.FindChild("Normal_Cursor", null, true, true, true, '.');
			this.SetCursor(cursor);
			return;
		}
		PathData.Node node = game.path_finding.data.GetNode(ppos);
		for (int i = -3; i <= 3; i++)
		{
			for (int j = -3; j <= 3; j++)
			{
				PPos ppos2 = ppos + new Point((float)j, (float)i) * 2f;
				if (!game.path_finding.data.GetNode(ppos2).water)
				{
					Logic.Realm realm2 = game.GetRealm(ppos2);
					if (realm2 != null && realm2.kingdom_id == army.kingdom_id)
					{
						flag = true;
					}
				}
			}
		}
		if (army.is_in_water && realm.id >= 0)
		{
			if (flag)
			{
				cursor = defField.FindChild("Disembark_Fast_Cursor", null, true, true, true, '.');
			}
			else
			{
				cursor = defField.FindChild("Disembark_Cursor", null, true, true, true, '.');
			}
		}
		else if (realm.IsSeaRealm())
		{
			if (flag)
			{
				cursor = defField.FindChild("Embark_Fast_Cursor", null, true, true, true, '.');
			}
			else
			{
				cursor = defField.FindChild("Embark_Cursor", null, true, true, true, '.');
			}
		}
		else
		{
			cursor = defField.FindChild("Move_Cursor", null, true, true, true, '.');
		}
		this.SetCursor(cursor);
	}

	// Token: 0x06001981 RID: 6529 RVA: 0x000F8848 File Offset: 0x000F6A48
	public static bool DeepOcean(PPos pos)
	{
		Game game = GameLogic.Get(true);
		if (!game.path_finding.data.GetNode(pos).ocean)
		{
			return false;
		}
		for (int i = -3; i <= 3; i++)
		{
			for (int j = -3; j <= 3; j++)
			{
				PPos pos2 = pos + new Point((float)j, (float)i) * 2f;
				if (!game.path_finding.data.GetNode(pos2).water)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06001982 RID: 6530 RVA: 0x000F88D0 File Offset: 0x000F6AD0
	public void ClearFromControlGroup(Logic.Character m_obj)
	{
		if (m_obj == null)
		{
			return;
		}
		foreach (KeyValuePair<int, BaseUI.ControlGroup> keyValuePair in this.groups)
		{
			if (m_obj == keyValuePair.Value.character)
			{
				keyValuePair.Value.character = null;
			}
		}
	}

	// Token: 0x06001983 RID: 6531 RVA: 0x000F893C File Offset: 0x000F6B3C
	public void ClearFromControlGroup(Logic.Settlement m_obj)
	{
		if (m_obj == null)
		{
			return;
		}
		foreach (KeyValuePair<int, BaseUI.ControlGroup> keyValuePair in this.groups)
		{
			if (m_obj == keyValuePair.Value.settlement)
			{
				keyValuePair.Value.settlement = null;
			}
		}
	}

	// Token: 0x06001984 RID: 6532 RVA: 0x000F89A8 File Offset: 0x000F6BA8
	public void AddToControlGroup(Logic.Settlement m_obj, int group_index, BaseUI.ControlGroup group = null)
	{
		if (m_obj == null)
		{
			return;
		}
		if (group == null && !this.groups.TryGetValue(group_index, out group))
		{
			return;
		}
		group.settlement = m_obj;
	}

	// Token: 0x06001985 RID: 6533 RVA: 0x000F89C9 File Offset: 0x000F6BC9
	public void AddToControlGroup(Logic.Character m_obj, int group_index, BaseUI.ControlGroup group = null)
	{
		if (m_obj == null)
		{
			return;
		}
		if (group == null && !this.groups.TryGetValue(group_index, out group))
		{
			return;
		}
		group.character = m_obj;
	}

	// Token: 0x06001986 RID: 6534 RVA: 0x000F89EC File Offset: 0x000F6BEC
	protected virtual void UpdateControlGroupInput()
	{
		KeyCode keyCode = KeyCode.Alpha0;
		while (keyCode < KeyCode.Alpha9)
		{
			if (UICommon.GetKeyUp(keyCode, UICommon.ModifierKey.None, UICommon.ModifierKey.All))
			{
				int num = keyCode - KeyCode.Alpha0;
				BaseUI.ControlGroup controlGroup;
				if (!this.groups.TryGetValue(num, out controlGroup))
				{
					controlGroup = new BaseUI.ControlGroup();
					this.groups[num] = controlGroup;
				}
				if (UICommon.GetKey(KeyCode.LeftControl, false) || UICommon.GetKey(KeyCode.RightControl, false))
				{
					Logic.Character character = null;
					Logic.Settlement settlement = null;
					Logic.Kingdom kingdom = BaseUI.LogicKingdom();
					if (this.selected_logic_obj != null)
					{
						settlement = (this.selected_logic_obj as Logic.Settlement);
						if (settlement == null)
						{
							Logic.Army army = this.selected_logic_obj as Logic.Army;
							if (army != null)
							{
								settlement = army.castle;
								if (settlement == null)
								{
									character = army.leader;
								}
							}
						}
						if (settlement != null && settlement.kingdom_id == kingdom.id)
						{
							this.AddToControlGroup(settlement, num, controlGroup);
						}
					}
					if (settlement == null && character == null && this.selected_court_member != null)
					{
						character = this.selected_court_member.logic;
					}
					if (character != null && character.kingdom_id == kingdom.id)
					{
						this.AddToControlGroup(character, num, controlGroup);
					}
					return;
				}
				if (UICommon.GetKey(KeyCode.LeftAlt, false) || UICommon.GetKey(KeyCode.RightAlt, false))
				{
					if (controlGroup.settlement != null)
					{
						this.SelectObjFromLogic(controlGroup.settlement, false, true);
						this.DoubleTapControlGroup(controlGroup.settlement);
					}
					return;
				}
				if (controlGroup.character != null)
				{
					this.SelectObjFromLogic(controlGroup.character, false, true);
					this.DoubleTapControlGroup(controlGroup.character.GetArmy());
				}
				return;
			}
			else
			{
				keyCode++;
			}
		}
	}

	// Token: 0x06001987 RID: 6535 RVA: 0x000F8B68 File Offset: 0x000F6D68
	private void DoubleTapControlGroup(MapObject obj)
	{
		if (obj == null)
		{
			return;
		}
		this.DoubleClickCheck();
		if (this.dblclk)
		{
			this.LookAt(obj.position, false);
		}
	}

	// Token: 0x0400100F RID: 4111
	public static bool control_ai = true;

	// Token: 0x04001010 RID: 4112
	public BaseUI.TooltipSettings tooltipSettings;

	// Token: 0x04001012 RID: 4114
	public BaseUI.SelectionSettings selectionSettings;

	// Token: 0x04001013 RID: 4115
	protected StudioEventEmitter audio;

	// Token: 0x04001014 RID: 4116
	private CameraController cameraController;

	// Token: 0x04001015 RID: 4117
	protected Vector3 vTerrainSize = Vector3.zero;

	// Token: 0x04001016 RID: 4118
	private DT.Field active_cursor;

	// Token: 0x04001017 RID: 4119
	public int mouse_btn_down = -1;

	// Token: 0x04001018 RID: 4120
	public float btn_down_time = -1f;

	// Token: 0x04001019 RID: 4121
	public bool dblclk;

	// Token: 0x0400101A RID: 4122
	public float dblclk_delay = 0.3f;

	// Token: 0x0400101B RID: 4123
	public float dbtap_delay = 0.26f;

	// Token: 0x0400101C RID: 4124
	public float dblclk_max_dist = 20f;

	// Token: 0x0400101D RID: 4125
	public Vector2 mouse_click_last_pos = Vector2.zero;

	// Token: 0x0400101E RID: 4126
	protected Vector3 ptLastMousePos = Vector3.zero;

	// Token: 0x0400101F RID: 4127
	protected float tmLastMouseMove;

	// Token: 0x04001020 RID: 4128
	protected bool bAlt;

	// Token: 0x04001023 RID: 4131
	public MapObject picked_map_object;

	// Token: 0x04001024 RID: 4132
	public global::Settlement picked_settlement;

	// Token: 0x04001025 RID: 4133
	public global::Army picked_army;

	// Token: 0x04001026 RID: 4134
	public global::Unit picked_unit;

	// Token: 0x04001027 RID: 4135
	public Vector3 picked_terrain_point = Vector3.zero;

	// Token: 0x04001028 RID: 4136
	public Vector3 picked_passable_area_pos = Vector3.zero;

	// Token: 0x04001029 RID: 4137
	public int picked_passable_area;

	// Token: 0x0400102A RID: 4138
	public GameObject picked_UI_element;

	// Token: 0x0400102B RID: 4139
	public Hotspot picked_hotspot;

	// Token: 0x0400102C RID: 4140
	public BSGButton picked_button;

	// Token: 0x0400102D RID: 4141
	private float reticle_duration = 1f;

	// Token: 0x0400102E RID: 4142
	private float reticle_height_offset = 1f;

	// Token: 0x0400102F RID: 4143
	private GameObject click_reticle_prefab;

	// Token: 0x04001030 RID: 4144
	private int unscaledTimeShaderProperyId;

	// Token: 0x04001031 RID: 4145
	public static bool log_sound_queue = false;

	// Token: 0x04001032 RID: 4146
	public RectTransform m_statusBar;

	// Token: 0x04001033 RID: 4147
	[HideInInspector]
	public Transform tCanvas;

	// Token: 0x04001034 RID: 4148
	[NonSerialized]
	public Canvas canvas;

	// Token: 0x04001035 RID: 4149
	private CanvasScaler canvasScaler;

	// Token: 0x04001036 RID: 4150
	protected GraphicRaycaster raycaster;

	// Token: 0x04001037 RID: 4151
	[NonSerialized]
	public List<GraphicRaycaster> raycasters = new List<GraphicRaycaster>();

	// Token: 0x04001038 RID: 4152
	public MiniMap minimap;

	// Token: 0x04001039 RID: 4153
	public MinimapArmyDisplay minimap_army_display;

	// Token: 0x0400103A RID: 4154
	[HideInInspector]
	public GameObject menu;

	// Token: 0x0400103B RID: 4155
	[NonSerialized]
	public GameObject tutorial_mouse_blocker;

	// Token: 0x0400103C RID: 4156
	public Tooltip tooltip;

	// Token: 0x0400103D RID: 4157
	public Tooltip pinned_tooltip;

	// Token: 0x0400103E RID: 4158
	public GameObject pinned_tooltip_instance;

	// Token: 0x04001040 RID: 4160
	public bool ignore_right_click;

	// Token: 0x04001041 RID: 4161
	public GameObject select_target;

	// Token: 0x04001042 RID: 4162
	public GameObject selected_orig;

	// Token: 0x04001043 RID: 4163
	public GameObject selected_obj;

	// Token: 0x04001044 RID: 4164
	public List<GameObject> selected_objects = new List<GameObject>();

	// Token: 0x04001045 RID: 4165
	public Logic.Object selected_logic_obj;

	// Token: 0x04001046 RID: 4166
	public List<Logic.Object> selected_logic_objects = new List<Logic.Object>();

	// Token: 0x04001047 RID: 4167
	[HideInInspector]
	public global::Kingdom selected_kingdom;

	// Token: 0x04001048 RID: 4168
	[HideInInspector]
	public global::Character selected_court_member;

	// Token: 0x04001049 RID: 4169
	protected int layerMask;

	// Token: 0x0400104A RID: 4170
	protected bool bSelection = true;

	// Token: 0x0400104B RID: 4171
	protected bool bPathArrows = true;

	// Token: 0x04001050 RID: 4176
	public GameObject interaction_blocker;

	// Token: 0x04001051 RID: 4177
	protected List<KeyValuePair<EventInstance, BaseUI.MessageAudioSettings>> soundQueue = new List<KeyValuePair<EventInstance, BaseUI.MessageAudioSettings>>();

	// Token: 0x04001052 RID: 4178
	public static DT.Field soundsDef;

	// Token: 0x04001053 RID: 4179
	public static List<BaseUI.NeverPlayTogether> never_play_together = new List<BaseUI.NeverPlayTogether>();

	// Token: 0x04001054 RID: 4180
	public BaseUI.ISelectionPanel SelectionPanel;

	// Token: 0x04001055 RID: 4181
	public UIWindowDispatcher window_dispatcher = new UIWindowDispatcher();

	// Token: 0x04001056 RID: 4182
	private static List<BaseUI> sm_Active = new List<BaseUI>();

	// Token: 0x04001057 RID: 4183
	private static BaseUI current;

	// Token: 0x04001058 RID: 4184
	private HashSet<string> m_InteractionBlockers = new HashSet<string>();

	// Token: 0x04001059 RID: 4185
	public bool m_InvalidateSelection;

	// Token: 0x0400105A RID: 4186
	private GameObject nextSelectionPanel;

	// Token: 0x0400105B RID: 4187
	public static TMP_Text picked_text_field = null;

	// Token: 0x0400105C RID: 4188
	public static UIText picked_ui_text = null;

	// Token: 0x0400105D RID: 4189
	public static int picked_line = -1;

	// Token: 0x0400105E RID: 4190
	public static int picked_char_idx = -1;

	// Token: 0x0400105F RID: 4191
	public static int picked_link_idx = -1;

	// Token: 0x04001060 RID: 4192
	public static UIText.LinkInfo picked_link;

	// Token: 0x04001061 RID: 4193
	public static Tooltip picked_link_tooltip = null;

	// Token: 0x04001062 RID: 4194
	public static Vars picked_link_vars = new Vars();

	// Token: 0x04001063 RID: 4195
	public static TMP_Text pressed_link_text_field = null;

	// Token: 0x04001064 RID: 4196
	public static int pressed_link_idx = -1;

	// Token: 0x04001065 RID: 4197
	public static Vector2 pressed_link_mouse_pos;

	// Token: 0x04001066 RID: 4198
	public static bool link_handled = false;

	// Token: 0x04001067 RID: 4199
	public static List<IListener> link_handlers = new List<IListener>();

	// Token: 0x04001068 RID: 4200
	protected RaycastHit[] hits = new RaycastHit[1024];

	// Token: 0x04001069 RID: 4201
	private static object boxed_false = new Value(false).Object(true);

	// Token: 0x0400106A RID: 4202
	private static object boxed_true = new Value(true).Object(true);

	// Token: 0x0400106B RID: 4203
	public static bool show_tooltips = true;

	// Token: 0x0400106C RID: 4204
	public static BaseUI.OnPickedElementChanged on_picked_element_changed = null;

	// Token: 0x0400106D RID: 4205
	public static List<RaycastResult> tmp_reycast_results = new List<RaycastResult>();

	// Token: 0x0400106E RID: 4206
	private static PointerEventData pointerData = new PointerEventData(EventSystem.current);

	// Token: 0x0400106F RID: 4207
	public Dictionary<int, BaseUI.ControlGroup> groups = new Dictionary<int, BaseUI.ControlGroup>();

	// Token: 0x02000704 RID: 1796
	public class TooltipSettings
	{
		// Token: 0x06004939 RID: 18745 RVA: 0x0021A9B2 File Offset: 0x00218BB2
		public void Save(DT.Field field)
		{
			if (field == null)
			{
				return;
			}
			field.SetValue("simple_prefab", null, this.SimplePrefab).type = "prefab";
			field.SetValue("caption_prefab", null, this.CaptionPrefab).type = "prefab";
		}

		// Token: 0x0600493A RID: 18746 RVA: 0x0021A9F0 File Offset: 0x00218BF0
		public void Load(DT.Field field)
		{
			if (field == null)
			{
				return;
			}
			this.SimplePrefab = global::Defs.GetObj<GameObject>(field, "simple_prefab", null);
			this.CaptionPrefab = global::Defs.GetObj<GameObject>(field, "caption_prefab", null);
		}

		// Token: 0x040037DD RID: 14301
		public GameObject SimplePrefab;

		// Token: 0x040037DE RID: 14302
		public GameObject CaptionPrefab;
	}

	// Token: 0x02000705 RID: 1797
	public class SelectionSettings
	{
		// Token: 0x0600493C RID: 18748 RVA: 0x0021AA1C File Offset: 0x00218C1C
		public void Load(DT.Field field)
		{
			if (field == null)
			{
				return;
			}
			this.friendColor = global::Defs.GetColor(field, "primary_friend_color", null).linear;
			this.enemyColor = global::Defs.GetColor(field, "primary_enemy_color", null).linear;
			this.neutralColor = global::Defs.GetColor(field, "primary_neutral_color", null).linear;
			this.secondaryFriendColor = global::Defs.GetColor(field, "secondary_friend_color", null).linear;
			this.secondaryEnemyColor = global::Defs.GetColor(field, "secondary_enemy_color", null).linear;
			this.secondaryNeutralColor = global::Defs.GetColor(field, "secondary_neutral_color", null).linear;
			this.friendColorMinimap = global::Defs.GetColor(field, "primary_friend_color_minimap", null).linear;
			this.enemyColorMinimap = global::Defs.GetColor(field, "primary_enemy_color_minimap", null).linear;
			this.neutralColorMinimap = global::Defs.GetColor(field, "primary_neutral_color_minimap", null).linear;
			this.secondaryFriendColorMinimap = global::Defs.GetColor(field, "secondary_friend_color_minimap", null).linear;
			this.secondaryEnemyColorMinimap = global::Defs.GetColor(field, "secondary_enemy_color_minimap", null).linear;
			this.secondaryNeutralColorMinimap = global::Defs.GetColor(field, "secondary_neutral_color_minimap", null).linear;
			this.armySelectionMaterial = global::Defs.GetObj<Material>(field, "army_selection_material", null);
			this.armyRelationsMaterial = global::Defs.GetObj<Material>(field, "army_relations_material", null);
			this.castleSelectionMaterial = global::Defs.GetObj<Material>(field, "castle_selection_material", null);
			this.villageSelectionMaterial = global::Defs.GetObj<Material>(field, "village_selection_material", null);
			this.squadSelectionMaterial = global::Defs.GetObj<Material>(field, "squad_selection_material", null);
			this.attritionRangeMaterial = global::Defs.GetObj<Material>(field, "attrition_range_material", null);
			this.retreatingColor = global::Defs.GetColor(field, "retreating_color", null).linear;
		}

		// Token: 0x040037DF RID: 14303
		public Color friendColor = new Color32(69, 188, 0, byte.MaxValue);

		// Token: 0x040037E0 RID: 14304
		public Color secondaryFriendColor = new Color32(69, 188, 0, byte.MaxValue);

		// Token: 0x040037E1 RID: 14305
		public Color enemyColor = new Color32(225, 61, 40, byte.MaxValue);

		// Token: 0x040037E2 RID: 14306
		public Color secondaryEnemyColor = new Color32(225, 61, 40, byte.MaxValue);

		// Token: 0x040037E3 RID: 14307
		public Color neutralColor = new Color32(192, 192, 192, byte.MaxValue);

		// Token: 0x040037E4 RID: 14308
		public Color secondaryNeutralColor = new Color32(192, 192, 192, byte.MaxValue);

		// Token: 0x040037E5 RID: 14309
		public Color friendColorMinimap = new Color32(69, 188, 0, byte.MaxValue);

		// Token: 0x040037E6 RID: 14310
		public Color secondaryFriendColorMinimap = new Color32(69, 188, 0, byte.MaxValue);

		// Token: 0x040037E7 RID: 14311
		public Color enemyColorMinimap = new Color32(225, 61, 40, byte.MaxValue);

		// Token: 0x040037E8 RID: 14312
		public Color secondaryEnemyColorMinimap = new Color32(225, 61, 40, byte.MaxValue);

		// Token: 0x040037E9 RID: 14313
		public Color neutralColorMinimap = new Color32(192, 192, 192, byte.MaxValue);

		// Token: 0x040037EA RID: 14314
		public Color secondaryNeutralColorMinimap = new Color32(192, 192, 192, byte.MaxValue);

		// Token: 0x040037EB RID: 14315
		public Color retreatingColor = new Color(150f, 150f, 150f, 255f);

		// Token: 0x040037EC RID: 14316
		public Material armySelectionMaterial;

		// Token: 0x040037ED RID: 14317
		public Material armyRelationsMaterial;

		// Token: 0x040037EE RID: 14318
		public Material castleSelectionMaterial;

		// Token: 0x040037EF RID: 14319
		public Material villageSelectionMaterial;

		// Token: 0x040037F0 RID: 14320
		public Material squadSelectionMaterial;

		// Token: 0x040037F1 RID: 14321
		public Material attritionRangeMaterial;
	}

	// Token: 0x02000706 RID: 1798
	public class NeverPlayTogether
	{
		// Token: 0x0600493E RID: 18750 RVA: 0x0021AD9C File Offset: 0x00218F9C
		public void Load(DT.Field child)
		{
			this.keys.Clear();
			this.targets.Clear();
			DT.Field field = child.FindChild("path", null, true, true, true, '.');
			if (field == null)
			{
				return;
			}
			int num = field.NumValues();
			for (int i = 0; i < num; i++)
			{
				string item = field.Value(i, null, true, true).String(null);
				this.keys.Add(item);
			}
			DT.Field field2 = child.FindChild("targets", null, true, true, true, '.');
			if (field2 == null)
			{
				return;
			}
			num = field2.NumValues();
			for (int j = 0; j < num; j++)
			{
				string item2 = field2.Value(j, null, true, true).String(null);
				this.targets.Add(item2);
			}
		}

		// Token: 0x0600493F RID: 18751 RVA: 0x0021AE5C File Offset: 0x0021905C
		public bool Validate(string path, List<KeyValuePair<EventInstance, BaseUI.MessageAudioSettings>> soundQueue)
		{
			if (path != null && soundQueue.Count > 0)
			{
				for (int i = 0; i < this.keys.Count; i++)
				{
					string text = this.keys[i];
					if (!string.IsNullOrEmpty(text))
					{
						if (text == path)
						{
							for (int j = 0; j < this.targets.Count; j++)
							{
								string text2 = this.targets[j];
								for (int k = soundQueue.Count - 1; k >= 0; k--)
								{
									if (soundQueue[k].Value.path == text2)
									{
										if (BaseUI.log_sound_queue)
										{
											UnityEngine.Debug.Log("Removing " + text2 + " from queue, can't play together with " + path);
										}
										if (k == 0)
										{
											soundQueue[k].Key.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
										}
										soundQueue.RemoveAt(k);
									}
								}
							}
						}
						else
						{
							for (int l = 0; l < this.targets.Count; l++)
							{
								if (!(this.targets[l] != path))
								{
									for (int m = soundQueue.Count - 1; m >= 0; m--)
									{
										if (soundQueue[m].Value.path == text)
										{
											if (BaseUI.log_sound_queue)
											{
												UnityEngine.Debug.Log(text + " is in queue, can't play " + path + " with it");
											}
											return false;
										}
									}
								}
							}
						}
					}
				}
			}
			return true;
		}

		// Token: 0x040037F2 RID: 14322
		public List<string> keys = new List<string>();

		// Token: 0x040037F3 RID: 14323
		public List<string> targets = new List<string>();
	}

	// Token: 0x02000707 RID: 1799
	public struct MessageAudioSettings
	{
		// Token: 0x040037F4 RID: 14324
		public float priority;

		// Token: 0x040037F5 RID: 14325
		public bool que_clearing;

		// Token: 0x040037F6 RID: 14326
		public float timeout;

		// Token: 0x040037F7 RID: 14327
		public float start_time;

		// Token: 0x040037F8 RID: 14328
		public string path;
	}

	// Token: 0x02000708 RID: 1800
	// (Invoke) Token: 0x06004942 RID: 18754
	public delegate void OnPickedElementChanged(BaseUI ui);

	// Token: 0x02000709 RID: 1801
	public interface ISelectionPanel
	{
		// Token: 0x06004945 RID: 18757
		void Refresh();

		// Token: 0x06004946 RID: 18758
		void ValidateSelectionObject();

		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x06004947 RID: 18759
		GameObject gameObject { get; }

		// Token: 0x06004948 RID: 18760
		bool IsDestoryed();

		// Token: 0x06004949 RID: 18761
		T GetComponent<T>();

		// Token: 0x0600494A RID: 18762
		void StoreState();

		// Token: 0x0600494B RID: 18763
		void RestoreState();

		// Token: 0x0600494C RID: 18764
		void Release();

		// Token: 0x0600494D RID: 18765
		bool PreserveWindow();

		// Token: 0x0600494E RID: 18766
		GameObject GetPrototype();

		// Token: 0x0600494F RID: 18767
		void SetPrototype(GameObject prorotype);
	}

	// Token: 0x0200070A RID: 1802
	public class ControlGroup
	{
		// Token: 0x040037F9 RID: 14329
		public Logic.Character character;

		// Token: 0x040037FA RID: 14330
		public Logic.Settlement settlement;

		// Token: 0x040037FB RID: 14331
		public List<Logic.Squad> squads = new List<Logic.Squad>();
	}
}

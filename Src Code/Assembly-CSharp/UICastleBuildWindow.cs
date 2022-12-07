using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002A7 RID: 679
public class UICastleBuildWindow : UIWindow, IListener
{
	// Token: 0x17000210 RID: 528
	// (get) Token: 0x06002A75 RID: 10869 RVA: 0x00161BCA File Offset: 0x0015FDCA
	public Game game
	{
		get
		{
			return GameLogic.Get(false);
		}
	}

	// Token: 0x06002A76 RID: 10870 RVA: 0x00168A8C File Offset: 0x00166C8C
	public override string GetDefId()
	{
		return UICastleBuildWindow.def_id;
	}

	// Token: 0x06002A77 RID: 10871 RVA: 0x00168A94 File Offset: 0x00166C94
	public static UICastleBuildWindow Create(Logic.Kingdom kingdom, Castle castle, int selected_slot_idx = -1, Building.Def selected_def = null)
	{
		if (UICastleBuildWindow.instance != null && UICastleBuildWindow.instance.castle == castle && UICastleBuildWindow.instance.kingdom == kingdom && UICastleBuildWindow.targetSlotIdx == selected_slot_idx)
		{
			UICastleBuildWindow.instance.Hide(false);
			return null;
		}
		if (UICastleBuildWindow.instance == null)
		{
			BaseUI baseUI = BaseUI.Get();
			Transform transform;
			if (baseUI == null)
			{
				transform = null;
			}
			else
			{
				GameObject message_container = baseUI.message_container;
				transform = ((message_container != null) ? message_container.transform : null);
			}
			Transform transform2 = transform;
			if (transform2 == null)
			{
				return null;
			}
			GameObject obj = global::Defs.GetObj<GameObject>(UICastleBuildWindow.def_id, "prefab", null);
			if (obj == null)
			{
				return null;
			}
			GameObject gameObject = global::Common.Spawn(obj, transform2, false, "");
			UICastleBuildWindow.instance = ((gameObject != null) ? gameObject.GetComponent<UICastleBuildWindow>() : null);
			if (UICastleBuildWindow.instance == null)
			{
				return null;
			}
		}
		UICastleBuildWindow.selectedSlotIdx = selected_slot_idx;
		UICastleBuildWindow.targetSlotIdx = selected_slot_idx;
		UICastleBuildWindow.instance.SetObject(kingdom, castle, selected_def);
		UICastleBuildWindow.instance.gameObject.SetActive(true);
		UICastleBuildWindow.instance.Open();
		UICastleBuildWindow.instance.play_voice_time = UnityEngine.Time.unscaledTime + BaseUI.soundsDef.GetFloat("narrator_building_delay", null, 20f, true, true, true, '.');
		return UICastleBuildWindow.instance;
	}

	// Token: 0x06002A78 RID: 10872 RVA: 0x00168BBE File Offset: 0x00166DBE
	public static void CloseInstance()
	{
		if (UICastleBuildWindow.instance != null)
		{
			UICastleBuildWindow.instance.Hide(false);
		}
	}

	// Token: 0x06002A79 RID: 10873 RVA: 0x00168BD8 File Offset: 0x00166DD8
	public void SetObject(Logic.Kingdom kingdom, Castle castle, Building.Def selected_def = null)
	{
		this.Init();
		if (this.kingdom != null)
		{
			this.kingdom.DelListener(this);
		}
		if (this.castle != null)
		{
			this.castle.DelListener(this);
		}
		this.kingdom = (kingdom ?? ((castle != null) ? castle.GetKingdom() : null));
		this.castle = castle;
		if (this.kingdom != null)
		{
			this.kingdom.AddListener(this);
		}
		if (this.castle != null)
		{
			this.castle.AddListener(this);
		}
		UICastleBuildWindow.selectedDef = selected_def;
		UICastleBuildWindow.selectedSlot = null;
		this.Refresh();
	}

	// Token: 0x06002A7A RID: 10874 RVA: 0x00168C6B File Offset: 0x00166E6B
	public static int TargetSlotIndex()
	{
		if (UICastleBuildWindow.instance == null)
		{
			return -1;
		}
		return UICastleBuildWindow.targetSlotIdx;
	}

	// Token: 0x06002A7B RID: 10875 RVA: 0x00168C84 File Offset: 0x00166E84
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		this.m_Initialized = true;
		UICommon.FindComponents(this, false);
		global::Common.FindChildrenWithComponent<UIDistrictPanel>(this.m_Top, this.district_panels);
		if (this.m_Close != null)
		{
			this.m_Close.onClick = new BSGButton.OnClick(this.HandleOnClose);
		}
	}

	// Token: 0x06002A7C RID: 10876 RVA: 0x00168CE0 File Offset: 0x00166EE0
	protected override void Update()
	{
		base.Update();
		if (global::Defs.Version == UICastleBuildWindow.defs_version)
		{
			Castle castle = this.castle;
			Religion religion;
			if (castle == null)
			{
				religion = null;
			}
			else
			{
				Logic.Realm realm = castle.GetRealm();
				religion = ((realm != null) ? realm.religion : null);
			}
			if (religion == this.realm_religion)
			{
				Castle castle2 = this.castle;
				Religion religion2;
				if (castle2 == null)
				{
					religion2 = null;
				}
				else
				{
					Logic.Kingdom kingdom = castle2.GetKingdom();
					religion2 = ((kingdom != null) ? kingdom.religion : null);
				}
				if (religion2 == this.kingdom_religion)
				{
					goto IL_64;
				}
			}
		}
		this.Refresh();
		IL_64:
		Castle castle3 = BaseUI.SelLO() as Castle;
		if (castle3 == this.castle || this.castle == null)
		{
			return;
		}
		if (castle3 == null)
		{
			this.Hide(false);
			return;
		}
		Logic.Realm realm2 = castle3.GetRealm();
		if (realm2.controller.GetKingdom() != realm2.GetKingdom())
		{
			this.Hide(false);
			return;
		}
		if (realm2.controller.GetKingdom() != BaseUI.LogicKingdom())
		{
			this.Hide(false);
			return;
		}
		UICastleBuildWindow.Create(null, castle3, -1, null);
	}

	// Token: 0x06002A7D RID: 10877 RVA: 0x00168DC0 File Offset: 0x00166FC0
	private void LateUpdate()
	{
		if (UnityEngine.Time.unscaledTime > this.play_voice_time && this.play_voice_time > 0f)
		{
			DT.Field soundsDef = BaseUI.soundsDef;
			BaseUI.PlayVoiceEvent((soundsDef != null) ? soundsDef.GetString("narrator_building_window", null, "", true, true, true, '.') : null, null);
			this.play_voice_time = -1f;
		}
	}

	// Token: 0x06002A7E RID: 10878 RVA: 0x00168E1C File Offset: 0x0016701C
	private void Refresh()
	{
		this.Init();
		UICastleBuildWindow.defs_version = global::Defs.Version;
		Castle castle = this.castle;
		Religion religion;
		if (castle == null)
		{
			religion = null;
		}
		else
		{
			Logic.Realm realm = castle.GetRealm();
			religion = ((realm != null) ? realm.religion : null);
		}
		this.realm_religion = religion;
		Castle castle2 = this.castle;
		Religion religion2;
		if (castle2 == null)
		{
			religion2 = null;
		}
		else
		{
			Logic.Kingdom kingdom = castle2.GetKingdom();
			religion2 = ((kingdom != null) ? kingdom.religion : null);
		}
		this.kingdom_religion = religion2;
		this.wnd_def = global::Defs.GetDefField(UICastleBuildWindow.def_id, null);
		this.RefreshCaption();
		this.RefreshCommonBuildings();
		this.RefreshDistricts();
	}

	// Token: 0x06002A7F RID: 10879 RVA: 0x00168EA4 File Offset: 0x001670A4
	private static bool ShouldBeSelected(UIBuildingSlot slot)
	{
		return (UICastleBuildWindow.selectedDef != null && UICastleBuildWindow.selectedDef == slot.Def) || (UICastleBuildWindow.selectedSlotIdx >= 0 && slot.SlotIndex == UICastleBuildWindow.selectedSlotIdx) || slot == UICastleBuildWindow.selectedSlot;
	}

	// Token: 0x06002A80 RID: 10880 RVA: 0x00168EE3 File Offset: 0x001670E3
	public static void UpdateSelected(UIBuildingSlot slot)
	{
		if (UICastleBuildWindow.ShouldBeSelected(slot))
		{
			slot.Select(true);
			if (UICastleBuildWindow.selectedSlot == null)
			{
				UICastleBuildWindow.selectedSlot = slot;
				return;
			}
		}
		else
		{
			slot.Select(false);
		}
	}

	// Token: 0x06002A81 RID: 10881 RVA: 0x00168F0F File Offset: 0x0016710F
	public static void OnSlotClick(UIBuildingSlot slot, PointerEventData e)
	{
		UICastleBuildWindow uicastleBuildWindow = UICastleBuildWindow.instance;
		if (uicastleBuildWindow == null)
		{
			return;
		}
		uicastleBuildWindow.Select(slot);
	}

	// Token: 0x06002A82 RID: 10882 RVA: 0x00168F21 File Offset: 0x00167121
	private void RefreshCaption()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		UIText.SetText(this.m_CaptionText, this.wnd_def, "caption", this.castle, null);
	}

	// Token: 0x06002A83 RID: 10883 RVA: 0x00168F48 File Offset: 0x00167148
	private void RefreshCommonBuildings()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		UIBuildingsPanel common = this.m_Common;
		if (common != null)
		{
			common.Init(District.Def.GetCommon(this.game), this.kingdom, this.castle);
		}
		if (this.m_Common != null && !this.m_Common.def.IsUpgrades())
		{
			this.m_Common.OnSelected = new Action<UIBuildingSlot, PointerEventData>(this.HandelOnSlotSelected);
		}
		UIBuildingsPanel pf = this.m_PF;
		if (pf != null)
		{
			pf.Init(District.Def.GetPF(this.game), this.kingdom, this.castle);
		}
		if (this.m_PF != null && !this.m_PF.def.IsUpgrades())
		{
			this.m_PF.OnSelected = new Action<UIBuildingSlot, PointerEventData>(this.HandelOnSlotSelected);
		}
	}

	// Token: 0x06002A84 RID: 10884 RVA: 0x0016901C File Offset: 0x0016721C
	private void HandelOnSlotSelected(UIBuildingSlot slot, PointerEventData e)
	{
		float num = 20f;
		Castle castle = ((slot != null) ? slot.Castle : null) ?? this.castle;
		Logic.Kingdom kingdom = this.kingdom ?? ((castle != null) ? castle.GetKingdom() : null);
		bool flag;
		UIBuildingWindow uibuildingWindow = UIBuildingWindow.Create(castle, kingdom, slot.Def, this.castle, out flag);
		if (!flag)
		{
			return;
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(uibuildingWindow.transform as RectTransform);
		Vector3[] array = new Vector3[4];
		(slot.transform as RectTransform).GetWorldCorners(array);
		Rect rect = (uibuildingWindow.transform as RectTransform).rect;
		if (array[1].y + rect.height < (float)Screen.height)
		{
			uibuildingWindow.transform.position = array[1] + new Vector3(0f, num, 0f);
		}
		else
		{
			uibuildingWindow.transform.position = array[1] - new Vector3(0f, num + rect.height, 0f);
		}
		UICommon.EnsureInScreen(uibuildingWindow.gameObject, 0.85f);
	}

	// Token: 0x06002A85 RID: 10885 RVA: 0x0016913C File Offset: 0x0016733C
	private void RefreshDistricts()
	{
		if (this.m_Top != null)
		{
			HorizontalLayoutGroup component = this.m_Top.GetComponent<HorizontalLayoutGroup>();
			if (component != null)
			{
				float @float = global::Defs.GetFloat("District", "panels_spacing", null, -1f);
				if (@float >= 0f)
				{
					component.spacing = @float;
				}
			}
		}
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.district_panels.Count == 0)
		{
			return;
		}
		Castle castle = this.castle;
		List<District.Def> list;
		if ((list = ((castle != null) ? castle.GetBuildableDistricts() : null)) == null)
		{
			Game game = this.game;
			list = ((game != null) ? game.defs.GetDefs<District.Def>() : null);
		}
		List<District.Def> list2 = list;
		int num = (list2 == null) ? 0 : list2.Count;
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			District.Def def = list2[i];
			if (def.buildable && !def.IsCommon() && !def.IsPF())
			{
				UIDistrictPanel uidistrictPanel;
				if (num2 == this.district_panels.Count)
				{
					GameObject gameObject = global::Common.Spawn(this.district_panels[0].gameObject, this.district_panels[0].transform.parent, false, "");
					uidistrictPanel = gameObject.GetOrAddComponent<UIDistrictPanel>();
					this.district_panels.Add(uidistrictPanel);
					GameObject gameObject2 = new GameObject(string.Format("Space{0}", num2 + 1), new Type[]
					{
						typeof(RectTransform),
						typeof(LayoutElement)
					});
					gameObject2.transform.SetParent(gameObject.transform.parent);
					gameObject2.GetOrAddComponent<LayoutElement>().flexibleWidth = 1f;
				}
				else
				{
					uidistrictPanel = this.district_panels[num2];
					Transform child = uidistrictPanel.transform.parent.GetChild(uidistrictPanel.transform.GetSiblingIndex() + 1);
					if (child == null)
					{
						goto IL_21B;
					}
					LayoutElement component2 = child.GetComponent<LayoutElement>();
					if (component2 != null && component2.flexibleWidth > 0f)
					{
						child.gameObject.SetActive(true);
					}
				}
				uidistrictPanel.Init(def, this.kingdom, this.castle);
				uidistrictPanel.gameObject.SetActive(true);
				num2++;
			}
			IL_21B:;
		}
		for (int j = num2; j < this.district_panels.Count; j++)
		{
			UIDistrictPanel uidistrictPanel2 = this.district_panels[j];
			uidistrictPanel2.gameObject.SetActive(false);
			Transform child2 = uidistrictPanel2.transform.parent.GetChild(uidistrictPanel2.transform.GetSiblingIndex() + 1);
			if (!(child2 == null))
			{
				LayoutElement component3 = child2.GetComponent<LayoutElement>();
				if (component3 != null && component3.flexibleWidth > 0f)
				{
					child2.gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06002A86 RID: 10886 RVA: 0x000023FD File Offset: 0x000005FD
	private void RefreshSelectionPanel()
	{
	}

	// Token: 0x06002A87 RID: 10887 RVA: 0x00169404 File Offset: 0x00167604
	public void Select(UIBuildingSlot slot)
	{
		if (slot != null && slot == UICastleBuildWindow.selectedSlot)
		{
			slot = null;
		}
		UICastleBuildWindow.selectedSlotIdx = ((slot == null) ? -1 : slot.SlotIndex);
		UICastleBuildWindow.selectedDef = ((slot != null) ? slot.Def : null);
		UIBuildingSlot uibuildingSlot = UICastleBuildWindow.selectedSlot;
		if (uibuildingSlot != null)
		{
			uibuildingSlot.Select(false);
		}
		UICastleBuildWindow.selectedSlot = slot;
		UIBuildingSlot uibuildingSlot2 = UICastleBuildWindow.selectedSlot;
		if (uibuildingSlot2 != null)
		{
			uibuildingSlot2.Select(true);
		}
		this.Refresh();
		Castle castle = this.castle;
		if (castle == null)
		{
			return;
		}
		castle.NotifyListeners("build_window_select", slot);
	}

	// Token: 0x06002A88 RID: 10888 RVA: 0x00169496 File Offset: 0x00167696
	private void OnCancelBuild(BSGButton btn)
	{
		UIBuildingSlot.HandleCancelBuild(this.castle, null, null, btn.gameObject);
	}

	// Token: 0x06002A89 RID: 10889 RVA: 0x001694AC File Offset: 0x001676AC
	private void OnBuild(BSGButton btn)
	{
		if (this.castle == null)
		{
			return;
		}
		Building.Def def = UICastleBuildWindow.selectedDef;
	}

	// Token: 0x06002A8A RID: 10890 RVA: 0x000F1E2B File Offset: 0x000F002B
	private void HandleOnClose(object p)
	{
		this.Hide(false);
	}

	// Token: 0x06002A8B RID: 10891 RVA: 0x001694C0 File Offset: 0x001676C0
	public override void Hide(bool silent = false)
	{
		base.Hide(silent);
		if (this.kingdom != null)
		{
			this.kingdom.DelListener(this);
			this.kingdom = null;
		}
		if (this.castle != null)
		{
			this.castle.DelListener(this);
			this.castle = null;
		}
		UICastleBuildWindow.targetSlotIdx = -1;
		UICastleBuildWindow.selectedSlotIdx = -1;
		UICastleBuildWindow.selectedDef = null;
		UICastleBuildWindow.selectedSlot = null;
		UICastleBuildings uicastleBuildings = UICastleBuildings.Get();
		if (uicastleBuildings != null)
		{
			uicastleBuildings.DeselectAll();
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002A8C RID: 10892 RVA: 0x0016953E File Offset: 0x0016773E
	protected override void OnDestroy()
	{
		base.OnDestroy();
		UICastleBuildWindow.instance = null;
		UICastleBuildWindow.targetSlotIdx = -1;
		UICastleBuildWindow.selectedSlotIdx = -1;
		UICastleBuildWindow.selectedDef = null;
		UICastleBuildWindow.selectedSlot = null;
	}

	// Token: 0x06002A8D RID: 10893 RVA: 0x000EDE65 File Offset: 0x000EC065
	public override bool OnBackInputAction()
	{
		this.Hide(false);
		return true;
	}

	// Token: 0x06002A8E RID: 10894 RVA: 0x00169564 File Offset: 0x00167764
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "build_started")
		{
			this.play_voice_time = -1f;
			this.Refresh();
			return;
		}
		if (message == "structures_changed")
		{
			this.Refresh();
			return;
		}
		if (!(message == "build_canceled") && !(message == "build_finished"))
		{
			return;
		}
		this.play_voice_time = UnityEngine.Time.unscaledTime + BaseUI.soundsDef.GetFloat("narrator_building_delay", null, 20f, true, true, true, '.');
		this.Refresh();
	}

	// Token: 0x06002A8F RID: 10895 RVA: 0x001695EB File Offset: 0x001677EB
	public static bool IsVisible()
	{
		return UICastleBuildWindow.instance != null && UICastleBuildWindow.instance.isActiveAndEnabled;
	}

	// Token: 0x04001CC7 RID: 7367
	[UIFieldTarget("id_CaptionText")]
	private TextMeshProUGUI m_CaptionText;

	// Token: 0x04001CC8 RID: 7368
	[UIFieldTarget("id_Close")]
	private BSGButton m_Close;

	// Token: 0x04001CC9 RID: 7369
	[UIFieldTarget("id_DistrictCaptions")]
	private GameObject m_DistrictCaptions;

	// Token: 0x04001CCA RID: 7370
	[UIFieldTarget("id_Top")]
	private GameObject m_Top;

	// Token: 0x04001CCB RID: 7371
	[UIFieldTarget("id_Common")]
	private UIBuildingsPanel m_Common;

	// Token: 0x04001CCC RID: 7372
	[UIFieldTarget("id_PF")]
	private UIBuildingsPanel m_PF;

	// Token: 0x04001CCD RID: 7373
	private List<UIDistrictPanel> district_panels = new List<UIDistrictPanel>();

	// Token: 0x04001CCE RID: 7374
	public DT.Field wnd_def;

	// Token: 0x04001CCF RID: 7375
	public Logic.Kingdom kingdom;

	// Token: 0x04001CD0 RID: 7376
	public Castle castle;

	// Token: 0x04001CD1 RID: 7377
	private static int targetSlotIdx = -1;

	// Token: 0x04001CD2 RID: 7378
	private static int selectedSlotIdx = -1;

	// Token: 0x04001CD3 RID: 7379
	private static Building.Def selectedDef = null;

	// Token: 0x04001CD4 RID: 7380
	private static UIBuildingSlot selectedSlot = null;

	// Token: 0x04001CD5 RID: 7381
	private static UICastleBuildWindow instance = null;

	// Token: 0x04001CD6 RID: 7382
	private static int defs_version = 0;

	// Token: 0x04001CD7 RID: 7383
	public Religion realm_religion;

	// Token: 0x04001CD8 RID: 7384
	public Religion kingdom_religion;

	// Token: 0x04001CD9 RID: 7385
	private float play_voice_time;

	// Token: 0x04001CDA RID: 7386
	private static string def_id = "BuildWindowSettings";
}

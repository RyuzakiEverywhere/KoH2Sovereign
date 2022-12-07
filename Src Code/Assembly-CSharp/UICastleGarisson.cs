using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020002AB RID: 683
public class UICastleGarisson : Hotspot, IListener
{
	// Token: 0x17000216 RID: 534
	// (get) Token: 0x06002AC6 RID: 10950 RVA: 0x0016A989 File Offset: 0x00168B89
	// (set) Token: 0x06002AC7 RID: 10951 RVA: 0x0016A991 File Offset: 0x00168B91
	public Castle Castle { get; private set; }

	// Token: 0x06002AC8 RID: 10952 RVA: 0x0016A99A File Offset: 0x00168B9A
	public override void Awake()
	{
		this.m_AwakePassed = true;
		if (this.m_AddListeners && this.Castle != null)
		{
			this.Castle.AddListener(this);
		}
	}

	// Token: 0x06002AC9 RID: 10953 RVA: 0x0016A9C0 File Offset: 0x00168BC0
	private void Init()
	{
		if (this.m_Initiazlied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_UnitFramePrefab = UICommon.GetPrefab("GarrisonSlot", null);
		if (this.m_TownGuards != null)
		{
			this.m_TownGuards.ShowSign(false);
		}
		this.m_Initiazlied = true;
	}

	// Token: 0x06002ACA RID: 10954 RVA: 0x0016AA10 File Offset: 0x00168C10
	public void SetCastle(Castle castle)
	{
		this.Init();
		this.RemoveListeners();
		this.Castle = castle;
		this.realm = castle.GetRealm();
		this.kingdom = castle.GetKingdom();
		if (this.m_AwakePassed)
		{
			this.AddListeners();
		}
		else
		{
			this.m_AddListeners = true;
		}
		if (this.m_TownGuards != null)
		{
			this.m_TownGuards.SetObject(ResourceType.TownGuards, this.Castle, null);
		}
		if (this.m_GarrisonSummary != null)
		{
			Tooltip.Get(this.m_GarrisonSummary.gameObject, true).SetDef("ManpowerGarrisonTooltip", new Vars(castle));
		}
		if (this.m_CurrentHireWindow != null)
		{
			this.m_CurrentHireWindow.SetCastle(castle);
		}
		this.Refresh();
	}

	// Token: 0x06002ACB RID: 10955 RVA: 0x0016AAD4 File Offset: 0x00168CD4
	private void AddListeners()
	{
		this.Castle.AddListener(this);
		Logic.Realm realm = this.realm;
		if (realm != null)
		{
			realm.AddListener(this);
		}
		Logic.Kingdom kingdom = this.kingdom;
		if (kingdom == null)
		{
			return;
		}
		kingdom.AddListener(this);
	}

	// Token: 0x06002ACC RID: 10956 RVA: 0x0016AB05 File Offset: 0x00168D05
	private void RemoveListeners()
	{
		Castle castle = this.Castle;
		if (castle != null)
		{
			castle.DelListener(this);
		}
		Logic.Realm realm = this.realm;
		if (realm != null)
		{
			realm.DelListener(this);
		}
		Logic.Kingdom kingdom = this.kingdom;
		if (kingdom == null)
		{
			return;
		}
		kingdom.DelListener(this);
	}

	// Token: 0x06002ACD RID: 10957 RVA: 0x0016AB3C File Offset: 0x00168D3C
	private void Update()
	{
		if (this.m_InvalidateUnits)
		{
			UIUnitSlot.lock_selection = true;
			this.Refresh();
			UIUnitSlot.lock_selection = false;
			this.m_InvalidateUnits = false;
			return;
		}
		this.UpdateManpower();
		this.UpdateHealIcon();
	}

	// Token: 0x06002ACE RID: 10958 RVA: 0x0016AB6C File Offset: 0x00168D6C
	private void Refresh()
	{
		if (this == null)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (this.m_UnitSlotsContainer == null)
		{
			return;
		}
		UICommon.DeleteChildren(this.m_UnitSlotsContainer);
		if (this.Castle == null)
		{
			return;
		}
		this.unitSlots.Clear();
		int garrisnoSlotsMaxCount = this.Castle.GetGarrisnoSlotsMaxCount();
		int garrisonSlotCount = this.Castle.GetGarrisonSlotCount();
		if (this.Castle.garrison != null)
		{
			List<Logic.Unit> units = this.Castle.garrison.units;
			for (int i = 0; i < garrisnoSlotsMaxCount; i++)
			{
				if (units != null && units.Count > i)
				{
					this.AddSlot(units[i], i, i >= garrisonSlotCount);
				}
				else
				{
					this.AddSlot(null, i, i >= garrisonSlotCount);
				}
			}
		}
		this.UpdateManpower();
		this.UpdateFocusedUnit();
		this.RefreshFirstFreeTutorialHighlightSlot();
	}

	// Token: 0x06002ACF RID: 10959 RVA: 0x0016AC40 File Offset: 0x00168E40
	private void RefreshFirstFreeTutorialHighlightSlot()
	{
		bool flag = false;
		for (int i = 0; i < this.unitSlots.Count; i++)
		{
			UIUnitSlot uiunitSlot = this.unitSlots[i];
			bool flag2 = !flag && uiunitSlot.IsEmpty();
			uiunitSlot.EnableTutorialHighlight(flag2);
			if (flag2)
			{
				flag = true;
			}
		}
	}

	// Token: 0x06002AD0 RID: 10960 RVA: 0x0016AC8C File Offset: 0x00168E8C
	private void UpdateFocusedUnit()
	{
		bool flag = this.m_CurrentHireWindow != null;
		int i = 0;
		int count = this.unitSlots.Count;
		while (i < count)
		{
			UIUnitSlot uiunitSlot = this.unitSlots[i];
			if (flag && uiunitSlot.IsEmpty())
			{
				uiunitSlot.SetFocused(true);
				flag = false;
			}
			else
			{
				uiunitSlot.SetFocused(false);
			}
			i++;
		}
		if (this.m_CurrentHireWindow != null && !this.HasFreeSlots())
		{
			UICastleWindow componentInParent = base.GetComponentInParent<UICastleWindow>();
			UIArmyWindow uiarmyWindow = (componentInParent != null) ? componentInParent.ArmyWindow : null;
			if (uiarmyWindow != null && uiarmyWindow.gameObject != null && uiarmyWindow.HasFreeSlots())
			{
				componentInParent.ArmyWindow.OpenRecruitWindow(-1, false);
			}
		}
	}

	// Token: 0x06002AD1 RID: 10961 RVA: 0x0016AD46 File Offset: 0x00168F46
	public bool HasFreeSlots()
	{
		return this.Castle != null && this.Castle.garrison != null && this.Castle.GetGarrisonSlotCount() > this.Castle.garrison.units.Count;
	}

	// Token: 0x06002AD2 RID: 10962 RVA: 0x0016AD84 File Offset: 0x00168F84
	private void UpdateManpower()
	{
		if (this.m_GarrisonTotalManpower != null && this.Castle.garrison != null)
		{
			int num;
			int num2;
			this.Castle.garrison.GetManpower(out num, out num2);
			if (num != this.m_LastManpower || num2 != this.m_LastMaxManpower)
			{
				this.m_LastManpower = num;
				this.m_LastMaxManpower = num2;
				this.m_GarrisonTotalManpower.text = num.ToString();
				DT.Field defField = global::Defs.GetDefField("ArmyStatusBar", "Manpower");
				DT.Field thresholdField = this.GetThresholdField(defField, (float)this.m_LastManpower);
				if (thresholdField == null)
				{
					return;
				}
				Color color = global::Defs.GetColor(thresholdField, "col", null);
				this.m_GarrisonTotalManpower.color = color;
			}
		}
	}

	// Token: 0x06002AD3 RID: 10963 RVA: 0x0016AE38 File Offset: 0x00169038
	private DT.Field GetThresholdField(DT.Field levels_field, float val)
	{
		if (((levels_field != null) ? levels_field.children : null) == null || levels_field.children.Count == 0)
		{
			return null;
		}
		DT.Field result = null;
		int num = int.MinValue;
		for (int i = 0; i < levels_field.children.Count; i++)
		{
			DT.Field field = levels_field.children[i];
			if (field.children != null && field.children.Count != 0)
			{
				int @int = field.GetInt("threshold", null, 0, true, true, true, '.');
				if ((float)@int <= val && @int > num)
				{
					num = @int;
					result = field;
				}
			}
		}
		return result;
	}

	// Token: 0x06002AD4 RID: 10964 RVA: 0x0016AEC8 File Offset: 0x001690C8
	private void AddSlot(Logic.Unit unit, int slotIndex, bool locked)
	{
		if (this.m_UnitFramePrefab == null)
		{
			return;
		}
		if (this.m_UnitSlotsContainer == null)
		{
			return;
		}
		GameObject gameObject = global::Common.Spawn(this.m_UnitFramePrefab, false, false);
		gameObject.transform.SetParent(this.m_UnitSlotsContainer, false);
		UIUnitSlot component = gameObject.GetComponent<UIUnitSlot>();
		if (component == null)
		{
			UnityEngine.Object.Destroy(gameObject);
			return;
		}
		if (unit == null)
		{
			component.SetUnitInstance(null, slotIndex, null, this.Castle);
		}
		else
		{
			component.SetUnitInstance(unit, slotIndex, null, this.Castle);
		}
		this.unitSlots.Add(component);
	}

	// Token: 0x06002AD5 RID: 10965 RVA: 0x0016AF5C File Offset: 0x0016915C
	private void UpdateHealIcon()
	{
		Castle castle = this.Castle;
		if (((castle != null) ? castle.garrison : null) == null)
		{
			return;
		}
		if (this.m_HealGarrisonIcon != null)
		{
			this.m_HealGarrisonIcon.gameObject.SetActive(this.Castle.garrison.CanReplenish());
		}
	}

	// Token: 0x06002AD6 RID: 10966 RVA: 0x0016AFAC File Offset: 0x001691AC
	public override void OnClick(PointerEventData e)
	{
		Castle castle = this.Castle;
		if (castle == null)
		{
			return;
		}
		castle.HealGarrisonUnits();
	}

	// Token: 0x06002AD7 RID: 10967 RVA: 0x0016AFC0 File Offset: 0x001691C0
	public void OpenRecruitWindow(int slotIndex, bool debug = false)
	{
		Castle castle = this.Castle;
		if (castle == null && !debug)
		{
			return;
		}
		if (this.m_CurrentHireWindow != null)
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		GameObject gameObject = global::Common.FindChildByName((baseUI != null) ? baseUI.gameObject : null, "id_MessageContainer", true, true);
		GameObject prefab = UIUnitRecruitmentWindow.GetPrefab(null);
		if (prefab != null && gameObject != null)
		{
			GameObject gameObject2 = global::Common.Spawn(prefab, gameObject.transform, false, "");
			this.m_CurrentHireWindow = gameObject2.GetComponent<UIUnitRecruitmentWindow>();
			if (this.m_CurrentHireWindow != null)
			{
				this.m_CurrentHireWindow.SetData(castle, null, slotIndex, new Action<Logic.Unit.Def, int>(this.HandleOnHire), debug);
				UIUnitRecruitmentWindow currentHireWindow = this.m_CurrentHireWindow;
				currentHireWindow.on_close = (UIWindow.OnClose)Delegate.Combine(currentHireWindow.on_close, new UIWindow.OnClose(this.OnHireWindowClose));
			}
			else
			{
				global::Common.DestroyObj(gameObject2);
			}
		}
		this.UpdateFocusedUnit();
	}

	// Token: 0x06002AD8 RID: 10968 RVA: 0x0016B0A4 File Offset: 0x001692A4
	private void HandleOnHire(Logic.Unit.Def unitDef, int slotIndex)
	{
		if (this.Castle == null)
		{
			return;
		}
		if (this.Castle.HireGarrisonUnit(unitDef))
		{
			BaseUI.PlaySoundEvent(unitDef.field.GetString("hire_sound", null, "", true, true, true, '.'), null);
			return;
		}
		BaseUI.PlaySoundEvent(unitDef.field.GetString("hire_failed_sound", null, "", true, true, true, '.'), null);
	}

	// Token: 0x06002AD9 RID: 10969 RVA: 0x0016B10B File Offset: 0x0016930B
	private void OnHireWindowClose(UIWindow window)
	{
		this.m_CurrentHireWindow = null;
		window.on_close = (UIWindow.OnClose)Delegate.Remove(window.on_close, new UIWindow.OnClose(this.OnHireWindowClose));
		this.UpdateFocusedUnit();
	}

	// Token: 0x06002ADA RID: 10970 RVA: 0x0016B13C File Offset: 0x0016933C
	public bool IsRecruitmentWindowActive()
	{
		return this.m_CurrentHireWindow != null && this.m_CurrentHireWindow.isActiveAndEnabled;
	}

	// Token: 0x06002ADB RID: 10971 RVA: 0x0016B15C File Offset: 0x0016935C
	public void OnMessage(object obj, string message, object param)
	{
		if (!(message == "structures_changed"))
		{
			if (message == "garisson_changed" || message == "units_changed")
			{
				this.m_InvalidateUnits = true;
				return;
			}
			if (!(message == "religion_changed"))
			{
				return;
			}
			if (this.m_CurrentHireWindow != null)
			{
				this.m_CurrentHireWindow.Refresh();
			}
		}
		else if (this.m_CurrentHireWindow != null)
		{
			this.m_CurrentHireWindow.Refresh();
			return;
		}
	}

	// Token: 0x06002ADC RID: 10972 RVA: 0x0016B1DC File Offset: 0x001693DC
	protected override void OnDisable()
	{
		if (this.m_CurrentHireWindow != null)
		{
			UIUnitRecruitmentWindow currentHireWindow = this.m_CurrentHireWindow;
			currentHireWindow.on_close = (UIWindow.OnClose)Delegate.Remove(currentHireWindow.on_close, new UIWindow.OnClose(this.OnHireWindowClose));
			this.m_CurrentHireWindow.Close(false);
			this.m_CurrentHireWindow = null;
		}
		base.OnDisable();
	}

	// Token: 0x06002ADD RID: 10973 RVA: 0x0016B237 File Offset: 0x00169437
	private void OnDestroy()
	{
		if (this.Castle != null)
		{
			this.Castle.DelListener(this);
		}
	}

	// Token: 0x06002ADE RID: 10974 RVA: 0x0016B24D File Offset: 0x0016944D
	public static bool CheckIfUnlocked(Castle castle)
	{
		return castle != null && castle.GetGarrisonSlotCount() > 0;
	}

	// Token: 0x04001D01 RID: 7425
	private Logic.Realm realm;

	// Token: 0x04001D02 RID: 7426
	private Logic.Kingdom kingdom;

	// Token: 0x04001D03 RID: 7427
	[UIFieldTarget("id_UnitSlots")]
	private RectTransform m_UnitSlotsContainer;

	// Token: 0x04001D04 RID: 7428
	[UIFieldTarget("id_TownGuards")]
	private UIResourceIncome m_TownGuards;

	// Token: 0x04001D05 RID: 7429
	[UIFieldTarget("id_GarrisonSummary")]
	private GameObject m_GarrisonSummary;

	// Token: 0x04001D06 RID: 7430
	[UIFieldTarget("id_GarrisonTotalManpower")]
	private TextMeshProUGUI m_GarrisonTotalManpower;

	// Token: 0x04001D07 RID: 7431
	[UIFieldTarget("id_HealGarrisonIcon")]
	private GameObject m_HealGarrisonIcon;

	// Token: 0x04001D08 RID: 7432
	private List<UIUnitSlot> unitSlots = new List<UIUnitSlot>();

	// Token: 0x04001D09 RID: 7433
	private GameObject m_UnitFramePrefab;

	// Token: 0x04001D0A RID: 7434
	private UIUnitRecruitmentWindow m_CurrentHireWindow;

	// Token: 0x04001D0B RID: 7435
	private int m_LastManpower = -1;

	// Token: 0x04001D0C RID: 7436
	private int m_LastMaxManpower = -1;

	// Token: 0x04001D0D RID: 7437
	private bool m_InvalidateUnits;

	// Token: 0x04001D0E RID: 7438
	private bool m_Initiazlied;

	// Token: 0x04001D0F RID: 7439
	private bool m_AwakePassed;

	// Token: 0x04001D10 RID: 7440
	private bool m_AddListeners;
}

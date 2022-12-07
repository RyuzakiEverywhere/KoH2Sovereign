using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002A4 RID: 676
public class UIBuildingWindow : UIWindow, IListener
{
	// Token: 0x06002A45 RID: 10821 RVA: 0x001672E3 File Offset: 0x001654E3
	public override string GetDefId()
	{
		return UIBuildingWindow.def_id;
	}

	// Token: 0x06002A46 RID: 10822 RVA: 0x001672EA File Offset: 0x001654EA
	public static bool CorrectInstanceExists(Castle castle, Building.Def selectedDef, Castle selectedCastle)
	{
		return UIBuildingWindow.instance != null && UIBuildingWindow.instance.castle == castle && UIBuildingWindow.instance.selectedDef == selectedDef && UIBuildingWindow.instance.sel_castle == selectedCastle;
	}

	// Token: 0x06002A47 RID: 10823 RVA: 0x00167324 File Offset: 0x00165524
	public static UIBuildingWindow Create(Castle castle, Logic.Kingdom kingdom, Building.Def selected_def, Castle sel_castle, out bool resetPosition)
	{
		if (kingdom == null)
		{
			kingdom = ((castle != null) ? castle.GetKingdom() : null);
		}
		if (!UIBuildingWindow.CorrectInstanceExists(castle, selected_def, sel_castle))
		{
			resetPosition = true;
			if (UIBuildingWindow.instance == null)
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
				GameObject obj = global::Defs.GetObj<GameObject>(UIBuildingWindow.def_id, "prefab", null);
				if (obj == null)
				{
					return null;
				}
				GameObject gameObject = global::Common.Spawn(obj, transform2, false, "");
				UIBuildingWindow.instance = ((gameObject != null) ? gameObject.GetComponent<UIBuildingWindow>() : null);
				if (UIBuildingWindow.instance == null)
				{
					return null;
				}
			}
			UIBuildingWindow.instance.Open();
			UIBuildingWindow.instance.SetObject(castle, kingdom, selected_def, sel_castle);
			UIBuildingWindow.instance.gameObject.SetActive(true);
			UIBuildingWindow.instance.transform.SetAsLastSibling();
			return UIBuildingWindow.instance;
		}
		resetPosition = false;
		if (UIBuildingWindow.instance.IsOnFocus())
		{
			UIBuildingWindow.instance.Hide(false);
			return null;
		}
		UIBuildingWindow.instance.SetOnFocus();
		UIBuildingWindow.instance.transform.SetAsLastSibling();
		return UIBuildingWindow.instance;
	}

	// Token: 0x06002A48 RID: 10824 RVA: 0x00167448 File Offset: 0x00165648
	public void SetObject(Castle castle, Logic.Kingdom kingdom, Building.Def selected_def, Castle sel_castle)
	{
		this.Init();
		this.sel_castle = sel_castle;
		if (this.castle != null)
		{
			this.castle.DelListener(this);
		}
		this.castle = castle;
		this.kingdom = (kingdom ?? ((castle != null) ? castle.GetKingdom() : null));
		if (this.castle != null)
		{
			this.castle.AddListener(this);
		}
		this.selectedDef = selected_def;
		this.Refresh();
	}

	// Token: 0x06002A49 RID: 10825 RVA: 0x001674B8 File Offset: 0x001656B8
	private void RebuildTooltips()
	{
		Vars vars = this.BuildToltipVars();
		Tooltip tooltip = Tooltip.Get(this.m_BuildingsCount, true);
		if (tooltip != null)
		{
			tooltip.SetDef("BuildingsCountTooltip", vars);
		}
		BSGButton buildButton = this.m_BuildButton;
		Tooltip tooltip2 = Tooltip.Get((buildButton != null) ? buildButton.gameObject : null, true);
		if (tooltip2 != null)
		{
			tooltip2.SetDef("BeginBuildTooltip", vars);
		}
		BSGButton cancelBuildButton = this.m_CancelBuildButton;
		Tooltip tooltip3 = Tooltip.Get((cancelBuildButton != null) ? cancelBuildButton.gameObject : null, true);
		if (tooltip3 != null)
		{
			tooltip3.SetDef("CancelBuildTooltip", vars);
		}
		BSGButton removeButton = this.m_RemoveButton;
		Tooltip tooltip4 = Tooltip.Get((removeButton != null) ? removeButton.gameObject : null, true);
		if (tooltip4 == null)
		{
			return;
		}
		tooltip4.SetDef("RemoveBuildTooltip", vars);
	}

	// Token: 0x06002A4A RID: 10826 RVA: 0x00167564 File Offset: 0x00165764
	private Vars BuildToltipVars()
	{
		if (this.vars == null)
		{
			this.vars = new Vars(this.selectedDef);
		}
		else
		{
			this.vars.Clear();
		}
		this.vars.Set<Building.Def>("building", this.selectedDef);
		Vars vars = this.vars;
		string key = "curent_build";
		Castle castle = this.castle;
		Building.Def val;
		if (castle == null)
		{
			val = null;
		}
		else
		{
			Castle.Build structure_build = castle.structure_build;
			val = ((structure_build != null) ? structure_build.current_building_def : null);
		}
		vars.Set<Building.Def>(key, val);
		this.vars.Set<Castle>("castle", this.castle);
		this.vars.Set<Logic.Kingdom>("kingdom", this.kingdom);
		Vars vars2 = this.vars;
		string key2 = "original_cost";
		Building.Def def = this.selectedDef;
		vars2.Set<Resource>(key2, (def != null) ? def.cost : null);
		Vars vars3 = this.vars;
		string key3 = "cost";
		Building.Def def2 = this.selectedDef;
		Resource val2;
		if (def2 == null)
		{
			val2 = null;
		}
		else
		{
			Castle castle2 = this.castle;
			val2 = def2.GetCost((castle2 != null) ? castle2.GetRealm() : null, null);
		}
		vars3.Set<Resource>(key3, val2);
		Vars vars4 = this.vars;
		string key4 = "upkeep";
		Building.Def def3 = this.selectedDef;
		vars4.Set<Resource>(key4, (def3 != null) ? def3.CalcUpkeep(this.kingdom) : null);
		ResourceInfo resourceInfo = (this.castle != null && this.selectedDef != null) ? this.castle.GetResourceInfo(this.selectedDef.id, true, true) : null;
		if (resourceInfo != null)
		{
			resourceInfo.SetColorVars(this.vars);
		}
		else
		{
			this.vars.Set<string>("availability_color", "");
			this.vars.Set<string>("/availability_color", "");
		}
		if (this.castle != null && this.selectedDef != null)
		{
			string str = UIBuildingSlot.BuildPromptKey(this.castle, this.selectedDef);
			DT.Field val3 = this.selectedDef.field.FindChild("texts.build_prompts." + str, null, true, true, true, '.');
			this.vars.Set<DT.Field>("build_prompt", val3);
		}
		UIBuildingSlot.FillBuiltLocationsText(this.vars, this.selectedDef, this.kingdom);
		return this.vars;
	}

	// Token: 0x06002A4B RID: 10827 RVA: 0x00167764 File Offset: 0x00165964
	private void RefreshVars()
	{
		if (this.vars == null)
		{
			return;
		}
		Vars vars = this.vars;
		string key = "curent_build";
		Castle castle = this.castle;
		Building.Def val;
		if (castle == null)
		{
			val = null;
		}
		else
		{
			Castle.Build structure_build = castle.structure_build;
			val = ((structure_build != null) ? structure_build.current_building_def : null);
		}
		vars.Set<Building.Def>(key, val);
		Vars vars2 = this.vars;
		string key2 = "cost";
		Building.Def def = this.selectedDef;
		Resource val2;
		if (def == null)
		{
			val2 = null;
		}
		else
		{
			Castle castle2 = this.castle;
			val2 = def.GetCost((castle2 != null) ? castle2.GetRealm() : null, null);
		}
		vars2.Set<Resource>(key2, val2);
		Vars vars3 = this.vars;
		string key3 = "upkeep";
		Building.Def def2 = this.selectedDef;
		vars3.Set<Resource>(key3, (def2 != null) ? def2.CalcUpkeep(this.kingdom) : null);
	}

	// Token: 0x06002A4C RID: 10828 RVA: 0x00167808 File Offset: 0x00165A08
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		this.m_Initialized = true;
		UICommon.FindComponents(this, false);
		if (this.m_Close != null)
		{
			this.m_Close.onClick = new BSGButton.OnClick(this.HandleOnClose);
		}
		if (this.m_BuildButton != null)
		{
			this.m_BuildButton.onClick = new BSGButton.OnClick(this.OnBuild);
		}
		if (this.m_CancelBuildButton != null)
		{
			this.m_CancelBuildButton.onClick = new BSGButton.OnClick(this.OnCancelBuild);
		}
		if (this.m_RemoveButton != null)
		{
			this.m_RemoveButton.onClick = new BSGButton.OnClick(this.OnRemoveBuilding);
		}
	}

	// Token: 0x06002A4D RID: 10829 RVA: 0x001678C0 File Offset: 0x00165AC0
	private void OnDisable()
	{
		if (this.m_RemoveBuiildingConfirmation != null)
		{
			this.m_RemoveBuiildingConfirmation.Close(false);
		}
		this.m_RemoveBuiildingConfirmation = null;
	}

	// Token: 0x06002A4E RID: 10830 RVA: 0x001678E4 File Offset: 0x00165AE4
	protected override void Update()
	{
		base.Update();
		if (global::Defs.Version == UIBuildingWindow.defs_version)
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
		this.RefreshVars();
		Castle castle3 = BaseUI.SelLO() as Castle;
		if (castle3 == this.sel_castle)
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
		bool flag;
		UIBuildingWindow.Create(castle3, (castle3 != null) ? castle3.GetKingdom() : null, this.selectedDef, castle3, out flag);
	}

	// Token: 0x06002A4F RID: 10831 RVA: 0x001679D4 File Offset: 0x00165BD4
	private void Refresh()
	{
		this.Init();
		UIBuildingWindow.defs_version = global::Defs.Version;
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
		Logic.Kingdom kingdom = this.kingdom;
		this.kingdom_religion = ((kingdom != null) ? kingdom.religion : null);
		this.wnd_def = global::Defs.GetDefField(UIBuildingWindow.def_id, null);
		this.RefreshLabels();
		this.RefreshBuilding();
		this.RefreshUpgrades();
		this.UpdateIntreactioButton();
		this.UpdateBuildingProgress();
		this.RebuildTooltips();
	}

	// Token: 0x06002A50 RID: 10832 RVA: 0x00167A62 File Offset: 0x00165C62
	private void LateUpdate()
	{
		this.UpdateIntreactioButton();
		this.UpdateBuildingProgress();
		this.UpdateBuildingCount();
		this.UpdatStatus();
	}

	// Token: 0x06002A51 RID: 10833 RVA: 0x00167A7C File Offset: 0x00165C7C
	private void UpdateBuildingCount()
	{
		int buildingsCountValue = this.GetBuildingsCountValue();
		GameObject buildingsCount = this.m_BuildingsCount;
		if (buildingsCount != null)
		{
			buildingsCount.gameObject.SetActive(buildingsCountValue > 0);
		}
		if (this.m_BuildingsCountValue != null)
		{
			this.m_BuildingsCountValue.text = buildingsCountValue.ToString();
		}
	}

	// Token: 0x06002A52 RID: 10834 RVA: 0x00167ACC File Offset: 0x00165CCC
	private void UpdatStatus()
	{
		if (this.m_Status == null)
		{
			return;
		}
		UIBuildingSlot icon = this.m_Icon;
		DT.Field field = (icon != null) ? icon.state_def : null;
		if (field == null)
		{
			this.m_Status.gameObject.SetActive(false);
			return;
		}
		bool @bool = field.GetBool("show_status_icon", null, false, true, true, true, '.');
		this.m_Status.gameObject.SetActive(@bool);
		if (this.m_StatusIcon != null && @bool)
		{
			this.m_StatusIcon.sprite = global::Defs.GetObj<Sprite>(field, "status_icon", null);
		}
	}

	// Token: 0x06002A53 RID: 10835 RVA: 0x00167B5C File Offset: 0x00165D5C
	private int GetBuildingsCountValue()
	{
		if (this.selectedDef == null || this.selectedDef.IsUpgrade())
		{
			return -1;
		}
		Logic.Kingdom kingdom = this.castle.GetKingdom();
		if (kingdom == null)
		{
			return -1;
		}
		int num = 0;
		for (int i = 0; i < kingdom.realms.Count; i++)
		{
			Logic.Realm realm = kingdom.realms[i];
			if (((realm != null) ? realm.castle : null) != null && realm.castle.HasBuilding(this.selectedDef))
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06002A54 RID: 10836 RVA: 0x00167BDC File Offset: 0x00165DDC
	private void UpdateIntreactioButton()
	{
		if (this.castle == null || this.castle.GetRealm().IsDisorder())
		{
			this.m_BuildButton.gameObject.SetActive(false);
			this.m_RemoveButton.gameObject.SetActive(false);
			this.m_CancelBuildButton.gameObject.SetActive(false);
			return;
		}
		bool flag = this.castle.IsBuilding();
		Castle.StructureBuildAvailability structureBuildAvailability = this.castle.CanBuildBuilding(this.selectedDef, false, false, true);
		bool flag2 = structureBuildAvailability == Castle.StructureBuildAvailability.AlreadyBuilt;
		bool flag3 = flag && this.castle.GetCurrentBuildingBuild() == this.selectedDef;
		bool flag4 = this.castle.MayBuildBuilding(this.selectedDef, true);
		bool active = !flag2 && !flag3;
		if (this.m_BuildButton != null)
		{
			bool flag5 = flag4 && !flag;
			flag5 &= (structureBuildAvailability != Castle.StructureBuildAvailability.MaxCountReached);
			this.m_BuildButton.gameObject.SetActive(active);
			this.m_BuildButton.Enable(flag5, false);
		}
		if (this.m_RemoveButton)
		{
			this.m_RemoveButton.gameObject.SetActive(flag2);
		}
		if (this.m_CancelBuildButton)
		{
			this.m_CancelBuildButton.gameObject.SetActive(flag3);
		}
	}

	// Token: 0x06002A55 RID: 10837 RVA: 0x00167D1C File Offset: 0x00165F1C
	private void UpdateBuildingProgress()
	{
		if (this.castle == null)
		{
			return;
		}
		bool flag = this.castle.GetCurrentBuildingBuild() == this.selectedDef;
		GameObject buildProgressContainer = this.m_BuildProgressContainer;
		if (buildProgressContainer != null)
		{
			buildProgressContainer.SetActive(flag);
		}
		if (flag && this.m_BuildProgress != null)
		{
			this.m_BuildProgress.fillAmount = this.castle.GetBuildPorgress();
		}
	}

	// Token: 0x06002A56 RID: 10838 RVA: 0x00167D80 File Offset: 0x00165F80
	private void RefreshLabels()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		UIText.SetText(this.m_CaptionText, this.wnd_def, "caption", this.castle, null);
		if (this.m_LabelUpgrades != null)
		{
			UIText.SetText(this.m_LabelUpgrades, this.wnd_def, "label_upgrades", null, null);
		}
		if (this.m_BuildLabel != null)
		{
			UIText.SetText(this.m_BuildLabel, this.window_def, "label_build", null, null);
		}
		if (this.m_CancelBuildLabel != null)
		{
			UIText.SetText(this.m_CancelBuildLabel, this.window_def, "label_cancel", null, null);
		}
		if (this.m_RemoveLabel != null)
		{
			UIText.SetText(this.m_RemoveLabel, this.window_def, "label_remove", null, null);
		}
	}

	// Token: 0x06002A57 RID: 10839 RVA: 0x00167E4C File Offset: 0x0016604C
	private void RefreshBuilding()
	{
		if (this.selectedDef == null)
		{
			return;
		}
		if (this.m_Icon != null)
		{
			this.m_Icon.SlotIndex = -1;
			this.m_Icon.AllowUpgardeIcons = false;
			this.m_Icon.SetDef(this.kingdom, this.castle, this.selectedDef, null);
		}
		if (this.m_Illustration != null)
		{
			bool flag = this.castle.MayBuildBuilding(this.selectedDef, true);
			this.m_Illustration.overrideSprite = global::Defs.GetObj<Sprite>(this.selectedDef.field, "illustration", this.kingdom);
			this.m_Illustration.color = global::Defs.GetColor(this.wnd_def, flag ? "illustration.normal" : "illustration.invalid", null);
		}
		if (this.m_Name != null)
		{
			UIText.SetText(this.m_Name, this.selectedDef.field, "name", this.castle, null);
		}
		if (this.m_Descritpion != null)
		{
			UIText.SetText(this.m_Descritpion, this.selectedDef.field, "flavor", this.castle, null);
		}
	}

	// Token: 0x06002A58 RID: 10840 RVA: 0x00167F74 File Offset: 0x00166174
	private void RefreshUpgrades()
	{
		if (this.m_Upgrades == null)
		{
			return;
		}
		if (this.selectedDef == null)
		{
			return;
		}
		this.m_Upgrades.Init(this.selectedDef.upgrades, this.kingdom, this.castle);
		GameObject upgradesContainer = this.m_UpgradesContainer;
		if (upgradesContainer == null)
		{
			return;
		}
		upgradesContainer.SetActive(!this.m_Upgrades.IsEmpty());
	}

	// Token: 0x06002A59 RID: 10841 RVA: 0x00167FD9 File Offset: 0x001661D9
	private void OnCancelBuild(BSGButton btn)
	{
		if (this.castle != null && this.castle.IsBuilding())
		{
			UIBuildingSlot.HandleCancelBuild(this.castle, null, null, btn.gameObject);
		}
	}

	// Token: 0x06002A5A RID: 10842 RVA: 0x00168004 File Offset: 0x00166204
	public static void CloseInstance()
	{
		if (UIBuildingWindow.instance != null)
		{
			UIBuildingWindow.instance.Hide(false);
		}
	}

	// Token: 0x06002A5B RID: 10843 RVA: 0x00168020 File Offset: 0x00166220
	private void OnRemoveBuilding(BSGButton btn)
	{
		if (this.castle == null)
		{
			return;
		}
		Building inst = this.castle.FindBuilding(this.selectedDef);
		this.m_RemoveBuiildingConfirmation = UIBuildingSlot.HandleRemoveBuilding(this.selectedDef, inst, this.castle);
	}

	// Token: 0x06002A5C RID: 10844 RVA: 0x00168060 File Offset: 0x00166260
	private void OnBuild(BSGButton btn)
	{
		if (this.castle == null)
		{
			return;
		}
		if (this.selectedDef == null)
		{
			return;
		}
		int selectedSlotIndex = UICastleBuildings.GetSelectedSlotIndex();
		this.castle.BuildBuilding(this.selectedDef, selectedSlotIndex, UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "build instant", true));
	}

	// Token: 0x06002A5D RID: 10845 RVA: 0x000F1E2B File Offset: 0x000F002B
	private void HandleOnClose(object p)
	{
		this.Hide(false);
	}

	// Token: 0x06002A5E RID: 10846 RVA: 0x001680B4 File Offset: 0x001662B4
	public override void Hide(bool silent = false)
	{
		base.Hide(silent);
		Castle castle = this.castle;
		if (castle != null)
		{
			castle.DelListener(this);
		}
		this.castle = null;
		this.selectedDef = null;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002A5F RID: 10847 RVA: 0x001680E9 File Offset: 0x001662E9
	protected override void OnDestroy()
	{
		base.OnDestroy();
		UIBuildingWindow.instance = null;
		this.realm_religion = null;
		this.kingdom_religion = null;
		this.selectedDef = null;
	}

	// Token: 0x06002A60 RID: 10848 RVA: 0x000EDE65 File Offset: 0x000EC065
	public override bool OnBackInputAction()
	{
		this.Hide(false);
		return true;
	}

	// Token: 0x06002A61 RID: 10849 RVA: 0x0016810C File Offset: 0x0016630C
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "build_started")
		{
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
		this.Refresh();
	}

	// Token: 0x06002A62 RID: 10850 RVA: 0x00168162 File Offset: 0x00166362
	public static bool IsVisible()
	{
		return UIBuildingWindow.instance != null && UIBuildingWindow.instance.isActiveAndEnabled;
	}

	// Token: 0x04001C99 RID: 7321
	[UIFieldTarget("id_CaptionText")]
	private TextMeshProUGUI m_CaptionText;

	// Token: 0x04001C9A RID: 7322
	[UIFieldTarget("id_Close")]
	private BSGButton m_Close;

	// Token: 0x04001C9B RID: 7323
	[UIFieldTarget("id_BuidlingIcon")]
	private UIBuildingSlot m_Icon;

	// Token: 0x04001C9C RID: 7324
	[UIFieldTarget("id_Illustration")]
	private Image m_Illustration;

	// Token: 0x04001C9D RID: 7325
	[UIFieldTarget("id_Name")]
	private TextMeshProUGUI m_Name;

	// Token: 0x04001C9E RID: 7326
	[UIFieldTarget("id_Descritpion")]
	private TextMeshProUGUI m_Descritpion;

	// Token: 0x04001C9F RID: 7327
	[UIFieldTarget("id_Background")]
	private Image m_Background;

	// Token: 0x04001CA0 RID: 7328
	[UIFieldTarget("id_BuildButton")]
	private BSGButton m_BuildButton;

	// Token: 0x04001CA1 RID: 7329
	[UIFieldTarget("id_BuildLabel")]
	private TextMeshProUGUI m_BuildLabel;

	// Token: 0x04001CA2 RID: 7330
	[UIFieldTarget("id_CancelBuildButton")]
	private BSGButton m_CancelBuildButton;

	// Token: 0x04001CA3 RID: 7331
	[UIFieldTarget("id_CancelBuildLabel")]
	private TextMeshProUGUI m_CancelBuildLabel;

	// Token: 0x04001CA4 RID: 7332
	[UIFieldTarget("id_BuildProgressContainer")]
	private GameObject m_BuildProgressContainer;

	// Token: 0x04001CA5 RID: 7333
	[UIFieldTarget("id_BuildProgress")]
	private Image m_BuildProgress;

	// Token: 0x04001CA6 RID: 7334
	[UIFieldTarget("id_RemoveButton")]
	private BSGButton m_RemoveButton;

	// Token: 0x04001CA7 RID: 7335
	[UIFieldTarget("id_RemoveLabel")]
	private TextMeshProUGUI m_RemoveLabel;

	// Token: 0x04001CA8 RID: 7336
	[UIFieldTarget("id_Upgrades")]
	private UIBuildingsPanel m_Upgrades;

	// Token: 0x04001CA9 RID: 7337
	[UIFieldTarget("id_Status")]
	private GameObject m_Status;

	// Token: 0x04001CAA RID: 7338
	[UIFieldTarget("id_StatusIcon")]
	private Image m_StatusIcon;

	// Token: 0x04001CAB RID: 7339
	[UIFieldTarget("id_BuildingsCount")]
	private GameObject m_BuildingsCount;

	// Token: 0x04001CAC RID: 7340
	[UIFieldTarget("id_BuildingsCountValue")]
	private TextMeshProUGUI m_BuildingsCountValue;

	// Token: 0x04001CAD RID: 7341
	[UIFieldTarget("id_UpgradesContainer")]
	private GameObject m_UpgradesContainer;

	// Token: 0x04001CAE RID: 7342
	[UIFieldTarget("id_LabelUpgrades")]
	private TextMeshProUGUI m_LabelUpgrades;

	// Token: 0x04001CAF RID: 7343
	private MessageWnd m_RemoveBuiildingConfirmation;

	// Token: 0x04001CB0 RID: 7344
	public DT.Field wnd_def;

	// Token: 0x04001CB1 RID: 7345
	public Castle sel_castle;

	// Token: 0x04001CB2 RID: 7346
	public Castle castle;

	// Token: 0x04001CB3 RID: 7347
	public Logic.Kingdom kingdom;

	// Token: 0x04001CB4 RID: 7348
	private static UIBuildingWindow instance = null;

	// Token: 0x04001CB5 RID: 7349
	private static int defs_version = 0;

	// Token: 0x04001CB6 RID: 7350
	private Religion realm_religion;

	// Token: 0x04001CB7 RID: 7351
	private Religion kingdom_religion;

	// Token: 0x04001CB8 RID: 7352
	private Building.Def selectedDef;

	// Token: 0x04001CB9 RID: 7353
	private Vars vars;

	// Token: 0x04001CBA RID: 7354
	private static string def_id = "BuildingWindowSettings";
}

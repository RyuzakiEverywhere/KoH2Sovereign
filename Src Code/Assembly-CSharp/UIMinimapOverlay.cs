using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200023B RID: 571
public class UIMinimapOverlay : MonoBehaviour
{
	// Token: 0x06002305 RID: 8965 RVA: 0x000DF539 File Offset: 0x000DD739
	private void OnEnable()
	{
		TooltipPlacement.AddBlocker(base.gameObject, null);
	}

	// Token: 0x06002306 RID: 8966 RVA: 0x000DF547 File Offset: 0x000DD747
	private void OnDisable()
	{
		TooltipPlacement.DelBlocker(base.gameObject);
	}

	// Token: 0x06002307 RID: 8967 RVA: 0x0013D993 File Offset: 0x0013BB93
	private IEnumerator Start()
	{
		bool flag = true;
		while (flag)
		{
			WorldUI ui = WorldUI.Get();
			if (ui == null)
			{
				yield return null;
			}
			if (ui.kingdom == 0)
			{
				yield return null;
			}
			flag = false;
			ui = null;
		}
		this.Populate();
		yield break;
	}

	// Token: 0x06002308 RID: 8968 RVA: 0x0013D9A4 File Offset: 0x0013BBA4
	private void Update()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (this.Kingdom != kingdom && kingdom != null && kingdom.IsValid())
		{
			this.SetKingdom(kingdom);
			return;
		}
		if (this.minimapButtonsPalette != null)
		{
			this.minimapButtonsPalette.gameObject.SetActive(UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.Low, "Debug view modes", false));
		}
		this.UpdateHotkeyInput();
		if (UIMinimapOverlay.minimap_filter_settings_changed)
		{
			UIMinimapOverlay.minimap_filter_settings_changed = false;
			this.UpdateToggleButtons();
		}
	}

	// Token: 0x06002309 RID: 8969 RVA: 0x0013DA26 File Offset: 0x0013BC26
	public void SetKingdom(Logic.Kingdom k)
	{
		this.Kingdom = k;
		this.SetupTooltips();
		this.UpdateLegend();
	}

	// Token: 0x0600230A RID: 8970 RVA: 0x0013DA3B File Offset: 0x0013BC3B
	private void Populate()
	{
		this.Init();
		this.InitViewModeButtons();
		this.UpdateToggleButtons();
	}

	// Token: 0x0600230B RID: 8971 RVA: 0x0013DA50 File Offset: 0x0013BC50
	private void Init()
	{
		if (this.m_Initalized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.backText != null)
		{
			UIText.SetTextKey(this.backText, "MinimapButtons.Back.label", null, null);
		}
		if (this.homeText != null)
		{
			UIText.SetTextKey(this.homeText, "MinimapButtons.Home.label", null, null);
		}
		this.m_Initalized = true;
	}

	// Token: 0x0600230C RID: 8972 RVA: 0x0013DAB4 File Offset: 0x0013BCB4
	private void InitViewModeButtons()
	{
		if (this.minimapBackButton != null)
		{
			this.minimapBackButton.onClick = new BSGButton.OnClick(this.OnBackButtonClick);
		}
		if (this.m_ToggleClickableNameplates != null)
		{
			this.m_ToggleClickableNameplates.onClick = new BSGButton.OnClick(this.OnToggleClickableNameplates);
			this.m_ToggleClickableNameplates.AllowSelection(true);
		}
		if (this.m_ToggleClickableArmies != null)
		{
			this.m_ToggleClickableArmies.onClick = new BSGButton.OnClick(this.OnToggleClickableArmies);
			this.m_ToggleClickableArmies.AllowSelection(true);
		}
		if (this.m_ToggleClickableSettlements != null)
		{
			this.m_ToggleClickableSettlements.onClick = new BSGButton.OnClick(this.OnToggleClickableSettlements);
			this.m_ToggleClickableSettlements.AllowSelection(true);
		}
		if (this.m_ToggleClickableBattles != null)
		{
			this.m_ToggleClickableBattles.onClick = new BSGButton.OnClick(this.OnToggleClickableBattles);
			this.m_ToggleClickableBattles.AllowSelection(true);
		}
		if (this.toggleNameplates != null)
		{
			this.toggleNameplates.onClick = new BSGButton.OnClick(this.OnToggleNameplates);
			this.toggleNameplates.AllowSelection(true);
		}
		if (this.toggleArmies != null)
		{
			this.toggleArmies.onClick = new BSGButton.OnClick(this.OnToogleArmies);
			this.toggleArmies.AllowSelection(true);
		}
		if (this.toggleBattles != null)
		{
			this.toggleBattles.onClick = new BSGButton.OnClick(this.OnToggleBattles);
			this.toggleBattles.AllowSelection(true);
		}
		if (this.toggleTowns != null)
		{
			this.toggleTowns.onClick = new BSGButton.OnClick(this.OnToggleTowns);
			this.toggleTowns.AllowSelection(true);
		}
		if (this.toggleRealms != null)
		{
			this.toggleRealms.onClick = new BSGButton.OnClick(this.OnToggleRealms);
			this.toggleRealms.AllowSelection(true);
		}
		if (this.toggleKingdoms != null)
		{
			this.toggleKingdoms.onClick = new BSGButton.OnClick(this.OnToggleKingdoms);
			this.toggleKingdoms.AllowSelection(true);
		}
		if (this.homeButton != null)
		{
			this.homeButton.onClick = new BSGButton.OnClick(this.OnHomeClick);
		}
		this.AllocateVWButtons();
		this.GenerateMinimapButtonsPalette();
		this.SetMinimapOrButtonsPaletteActive();
	}

	// Token: 0x0600230D RID: 8973 RVA: 0x0013DD08 File Offset: 0x0013BF08
	private void InitMinimapPaletteSelection()
	{
		if (this.minimapButtonsPalette == null)
		{
			return;
		}
		MinimapPalette component = global::Common.GetComponent<MinimapPalette>(this.minimapButtonsPalette, null);
		if (component == null)
		{
			return;
		}
		if (component.selectedButtons == null)
		{
			return;
		}
		int i = 0;
		int childCount = this.minimapButtonsPalette.transform.childCount;
		while (i < childCount)
		{
			Transform child = this.minimapButtonsPalette.transform.GetChild(i);
			if (!(child == null))
			{
				MinimapPaletteButton component2 = child.GetComponent<MinimapPaletteButton>();
				if (!(component2 == null))
				{
					component.selectedButtons.Add(component2.view_mode);
					if (component.selectedButtons.Count >= 6)
					{
						break;
					}
				}
			}
			i++;
		}
	}

	// Token: 0x0600230E RID: 8974 RVA: 0x0013DDB0 File Offset: 0x0013BFB0
	private void GenerateMinimapButtonsPalette()
	{
		List<DT.Field> viewModeButtonDefs = this.GetViewModeButtonDefs();
		for (int i = 0; i < viewModeButtonDefs.Count; i++)
		{
			DT.Field f = viewModeButtonDefs[i];
			if (!(this.viewbuttons.Find((MinimapViewModeButton x) => x.Data == f) != null))
			{
				this.CreateMinimapPaletteButton(viewModeButtonDefs[i]);
			}
		}
	}

	// Token: 0x0600230F RID: 8975 RVA: 0x0013DE14 File Offset: 0x0013C014
	private List<DT.Field> GetViewModeButtonDefs()
	{
		List<DT.Field> list = new List<DT.Field>();
		if (this.buttonsDef == null)
		{
			this.buttonsDef = global::Defs.GetDefField("MinimapButtons", null);
		}
		List<DT.Field> list2 = this.buttonsDef.Children();
		for (int i = 0; i < list2.Count; i++)
		{
			DT.Field field = list2[i];
			if (!string.IsNullOrEmpty(field.key) && ViewMode.Get(field.key) != null)
			{
				list.Add(list2[i]);
			}
		}
		return list;
	}

	// Token: 0x06002310 RID: 8976 RVA: 0x0013DE90 File Offset: 0x0013C090
	private void AllocateVWButtons()
	{
		if (this.minimap_buttons == null || this.minimapButtonsPalette == null)
		{
			return;
		}
		List<DT.Field> viewModeButtonDefs = this.GetViewModeButtonDefs();
		MinimapButtons component = this.minimap_buttons.GetComponent<MinimapButtons>();
		if (component != null)
		{
			this.ButtonsScript = component;
			this.Defs = this.buttonsDef;
		}
		int i = 0;
		int childCount = this.minimap_buttons.transform.childCount;
		while (i < childCount)
		{
			MinimapViewModeButton orAddComponent = this.minimap_buttons.transform.GetChild(i).GetOrAddComponent<MinimapViewModeButton>();
			DT.Field field = (viewModeButtonDefs.Count > i) ? viewModeButtonDefs[i] : null;
			if (field != null)
			{
				orAddComponent.SetData(field);
				orAddComponent.OnSelect = new Action<MinimapViewModeButton>(this.OnViewModeSelect);
				orAddComponent.OnLongPress = new Action<MinimapViewModeButton>(this.OnViewModeButtonLongPress);
				orAddComponent.minimapOverlay = this;
				this.viewbuttons.Add(orAddComponent);
				if (!(orAddComponent.listMenuPalette == null))
				{
					this.GenerateViewModeSelectionPopup(orAddComponent.listMenuPalette);
					orAddComponent.listMenuPalette.SetActive(false);
				}
			}
			i++;
		}
	}

	// Token: 0x06002311 RID: 8977 RVA: 0x0013DFAC File Offset: 0x0013C1AC
	private void CreateMinimapPaletteButton(DT.Field buttonDef)
	{
		if (buttonDef == null || this.minimapButtonsPalette == null)
		{
			return;
		}
		GameObject obj = global::Defs.GetObj<GameObject>(buttonDef, "image_button", null);
		if (obj == null)
		{
			return;
		}
		GameObject gameObject = global::Common.Spawn(obj, false, false);
		MinimapViewModeButton orAddComponent = gameObject.GetOrAddComponent<MinimapViewModeButton>();
		orAddComponent.SetData(buttonDef);
		gameObject.transform.SetParent(this.minimapButtonsPalette.transform, true);
		this.palettebuttons.Add(orAddComponent);
		if (orAddComponent.view_mode == null)
		{
			return;
		}
		orAddComponent.OnSelect = new Action<MinimapViewModeButton>(this.OnMinimapPaletteButtonClick);
	}

	// Token: 0x06002312 RID: 8978 RVA: 0x0013E038 File Offset: 0x0013C238
	private void GenerateViewModeSelectionPopup(GameObject listMenuPalette)
	{
		List<DT.Field> viewModeButtonDefs = this.GetViewModeButtonDefs();
		for (int i = 0; i < viewModeButtonDefs.Count; i++)
		{
			this.CreateListMenuPaletteButton(viewModeButtonDefs[i], listMenuPalette);
		}
	}

	// Token: 0x06002313 RID: 8979 RVA: 0x0013E06C File Offset: 0x0013C26C
	private void CreateListMenuPaletteButton(DT.Field buttonDef, GameObject listMenuPalette)
	{
		if (buttonDef == null || listMenuPalette == null)
		{
			return;
		}
		GameObject obj = global::Defs.GetObj<GameObject>(buttonDef, "image_button", null);
		if (this.textPaletteButtonPrefab == null || obj == null)
		{
			return;
		}
		Sprite obj2 = global::Defs.GetObj<Sprite>(buttonDef, "icon", null);
		GameObject gameObject = global::Common.Spawn(obj, false, false);
		gameObject.transform.SetParent(listMenuPalette.transform, true);
		MinimapPaletteButton component = gameObject.GetComponent<MinimapPaletteButton>();
		if (component == null)
		{
			return;
		}
		component.view_mode = ViewMode.Get(buttonDef.key);
		Button btn = gameObject.GetComponent<Button>();
		if (btn == null)
		{
			return;
		}
		btn.onClick.AddListener(delegate()
		{
			this.OnListMenuPaletteButtonClick(btn);
		});
		RectTransform component2 = gameObject.GetComponent<RectTransform>();
		if (component2 != null)
		{
			component2.localScale = new Vector3(1f, 1f, 1f);
			GameObject gameObject2 = global::Common.FindChildByName(gameObject, "id_Button", true, true);
			if (gameObject2 == null)
			{
				return;
			}
			if (obj2 != null)
			{
				gameObject2.GetComponent<Image>().sprite = obj2;
			}
		}
	}

	// Token: 0x06002314 RID: 8980 RVA: 0x0013E19C File Offset: 0x0013C39C
	private void SetupTooltips()
	{
		Vars vars = new Vars(this.Kingdom);
		if (this.m_ToggleClickableNameplates != null)
		{
			Vars vars2 = new Vars();
			Tooltip.Get(this.m_ToggleClickableNameplates.gameObject, true).SetDef("MimimapFilterClickableNameplatesTooltip", vars2);
		}
		if (this.m_ToggleClickableArmies != null)
		{
			Vars vars3 = new Vars();
			Tooltip.Get(this.m_ToggleClickableArmies.gameObject, true).SetDef("MimimapFilterClickableArmiesTooltip", vars3);
		}
		if (this.m_ToggleClickableBattles != null)
		{
			Vars vars4 = new Vars();
			Tooltip.Get(this.m_ToggleClickableBattles.gameObject, true).SetDef("MimimapFilterClickableBattlesTooltip", vars4);
		}
		if (this.m_ToggleClickableSettlements != null)
		{
			Vars vars5 = new Vars();
			Tooltip.Get(this.m_ToggleClickableSettlements.gameObject, true).SetDef("MimimapFilterClickableTownTooltip", vars5);
		}
		if (this.toggleNameplates != null)
		{
			Tooltip.Get(this.toggleNameplates.gameObject, true).SetDef("MimimapFilterNameplatesTooltip", vars);
		}
		if (this.toggleArmies != null)
		{
			Tooltip.Get(this.toggleArmies.gameObject, true).SetDef("MimimapFilterArmiesTooltip", vars);
		}
		if (this.toggleBattles != null)
		{
			Tooltip.Get(this.toggleBattles.gameObject, true).SetDef("MimimapFilterBattlesTooltip", vars);
		}
		if (this.toggleTowns != null)
		{
			Tooltip.Get(this.toggleTowns.gameObject, true).SetDef("MimimapFilterTownsTooltip", vars);
		}
		if (this.toggleRealms != null)
		{
			Tooltip.Get(this.toggleRealms.gameObject, true).SetDef("MimimapToggleRealmsTooltip", vars);
		}
		if (this.toggleKingdoms != null)
		{
			Tooltip.Get(this.toggleKingdoms.gameObject, true).SetDef("MimimapToggleKingdomsTooltip", vars);
		}
		if (this.homeButton != null)
		{
			Tooltip.Get(this.homeButton.gameObject, true).SetDef("MinimapHomeTooltip", vars);
		}
	}

	// Token: 0x06002315 RID: 8981 RVA: 0x0013E3A2 File Offset: 0x0013C5A2
	public void UpdateViewMode()
	{
		if (this.buttonsPalette != null)
		{
			this.buttonsPalette.SetActive(ViewMode.IsPoliticalView());
		}
		this.SetMinimapOrButtonsPaletteActive();
		this.UpdateViewModeButtons();
		this.UpdateToggleButtons();
		this.UpdateLegend();
	}

	// Token: 0x06002316 RID: 8982 RVA: 0x0013E3DC File Offset: 0x0013C5DC
	public void UpdateLegend()
	{
		if (this.legend_button == null)
		{
			return;
		}
		Tooltip tooltip = Tooltip.Get(this.legend_button.gameObject, true);
		if (tooltip == null || ViewMode.current == null)
		{
			return;
		}
		tooltip.SetDef("ViewModeLegendTooltip", new Vars(ViewMode.current.def));
		if (tooltip.instance != null)
		{
			tooltip.instance.SetActive(ViewMode.IsPoliticalView());
		}
	}

	// Token: 0x06002317 RID: 8983 RVA: 0x0013E454 File Offset: 0x0013C654
	public void DeselectPaletteSelection()
	{
		if (this.minimapButtonsPalette == null)
		{
			return;
		}
		MinimapPalette component = global::Common.GetComponent<MinimapPalette>(this.minimapButtonsPalette, null);
		if (component == null || component.selection == null)
		{
			return;
		}
		Image component2 = global::Common.GetComponent<Image>(component.selection.transform.parent, null);
		if (component2 != null)
		{
			component2.color = this.paletteButtonColor;
		}
		component.selection = null;
	}

	// Token: 0x06002318 RID: 8984 RVA: 0x0013E4C8 File Offset: 0x0013C6C8
	public void RefreshPaletteButtons()
	{
		if (this.minimapButtonsPalette == null)
		{
			return;
		}
		MinimapPalette component = global::Common.GetComponent<MinimapPalette>(this.minimapButtonsPalette, null);
		if (component == null)
		{
			return;
		}
		int i = 0;
		int childCount = this.minimapButtonsPalette.transform.childCount;
		while (i < childCount)
		{
			Transform child = this.minimapButtonsPalette.transform.GetChild(i);
			Button component2 = child.GetComponent<Button>();
			Image component3 = child.GetComponent<Image>();
			if (!(component2 == null) && !(component3 == null) && !(child.GetComponent<MinimapPaletteButton>() == null))
			{
				component2.interactable = true;
				component3.color = this.paletteButtonColor;
			}
			i++;
		}
		component.selection = null;
		if (this.minimap_buttons == null)
		{
			return;
		}
		int j = 0;
		int childCount2 = this.minimap_buttons.transform.childCount;
		while (j < childCount2)
		{
			GameObject gameObject = global::Common.FindChildByName(this.minimap_buttons.transform.GetChild(j).gameObject, "id_ListMenuButtonsPalette", true, true);
			if (!(gameObject == null))
			{
				int k = 0;
				int childCount3 = gameObject.transform.childCount;
				while (k < childCount3)
				{
					Transform child2 = gameObject.transform.GetChild(k);
					Button component2 = child2.GetComponent<Button>();
					Image component3 = child2.GetComponent<Image>();
					if (!(component2 == null) && !(component3 == null))
					{
						if (this.IsPaletteButtonApplied(child2))
						{
							component2.interactable = false;
						}
						else
						{
							component2.interactable = true;
							component3.color = this.paletteButtonColor;
						}
					}
					k++;
				}
			}
			j++;
		}
	}

	// Token: 0x06002319 RID: 8985 RVA: 0x0013E658 File Offset: 0x0013C858
	public void UpdateViewModeButtons()
	{
		if (this.minimap_buttons == null)
		{
			return;
		}
		new BaseEventData(EventSystem.current);
		for (int i = 0; i < this.palettebuttons.Count; i++)
		{
			MinimapViewModeButton minimapViewModeButton = this.palettebuttons[i];
			minimapViewModeButton.Select(minimapViewModeButton.view_mode == ViewMode.current);
		}
		for (int j = 0; j < this.viewbuttons.Count; j++)
		{
			MinimapViewModeButton minimapViewModeButton2 = this.viewbuttons[j];
			minimapViewModeButton2.Select(minimapViewModeButton2.view_mode == ViewMode.current);
		}
		if (this.selectedView != null)
		{
			UIText.SetTextKey(this.selectedView, ViewMode.current.def_id + ".name", null, null);
		}
	}

	// Token: 0x0600231A RID: 8986 RVA: 0x0013E718 File Offset: 0x0013C918
	public void UpdateMinimapPVInputButtons()
	{
		int i = 0;
		while (i < this.viewbuttons.Count)
		{
			MinimapViewModeButton minimapViewModeButton = this.viewbuttons[i];
			if (Game.CheckCheatLevel((Game.CheatLevel)minimapViewModeButton.Data.GetInt("cheat_level", null, 0, true, true, true, '.'), "cheat political view keybind", false) && KeyBindings.GetBindDown(minimapViewModeButton.keybind))
			{
				Action<MinimapViewModeButton> onSelect = minimapViewModeButton.OnSelect;
				if (onSelect == null)
				{
					return;
				}
				onSelect(minimapViewModeButton);
				return;
			}
			else
			{
				i++;
			}
		}
		int j = 0;
		while (j < this.palettebuttons.Count)
		{
			MinimapViewModeButton minimapViewModeButton2 = this.palettebuttons[j];
			if (Game.CheckCheatLevel((Game.CheatLevel)minimapViewModeButton2.Data.GetInt("cheat_level", null, 0, true, true, true, '.'), "cheat political view keybind", false) && KeyBindings.GetBindDown(minimapViewModeButton2.keybind))
			{
				Action<MinimapViewModeButton> onSelect2 = minimapViewModeButton2.OnSelect;
				if (onSelect2 == null)
				{
					return;
				}
				onSelect2(minimapViewModeButton2);
				return;
			}
			else
			{
				j++;
			}
		}
	}

	// Token: 0x0600231B RID: 8987 RVA: 0x0013E7F4 File Offset: 0x0013C9F4
	public void RefreshMinimapPVButtons()
	{
		for (int i = 0; i < this.palettebuttons.Count; i++)
		{
			this.palettebuttons[i].Refresh();
		}
		for (int j = 0; j < this.viewbuttons.Count; j++)
		{
			this.viewbuttons[j].Refresh();
		}
	}

	// Token: 0x0600231C RID: 8988 RVA: 0x0013E850 File Offset: 0x0013CA50
	private void SetMinimapOrButtonsPaletteActive()
	{
		if (this.minimapObject != null)
		{
			this.minimapObject.SetActive(!ViewMode.IsPoliticalView());
		}
		if (this.buttonsPalette != null)
		{
			this.buttonsPalette.SetActive(ViewMode.IsPoliticalView());
		}
		if (this.groupPoliticalView != null)
		{
			this.groupPoliticalView.SetActive(ViewMode.IsPoliticalView());
		}
	}

	// Token: 0x0600231D RID: 8989 RVA: 0x0013E8BA File Offset: 0x0013CABA
	public void Deselect()
	{
		if (this.listMenuShown != null)
		{
			this.listMenuShown.SetActive(false);
		}
	}

	// Token: 0x0600231E RID: 8990 RVA: 0x0013E8D8 File Offset: 0x0013CAD8
	private void UpdateHotkeyInput()
	{
		if (KeyBindings.GetBindDown("toggle_selection_mode"))
		{
			if (!ViewMode.IsPoliticalView())
			{
				return;
			}
			ViewMode.SubMode subMode = ViewMode.current.GetSubMode();
			if (subMode == ViewMode.SubMode.Kingdom)
			{
				this.ToggleRealms();
				return;
			}
			if (subMode == ViewMode.SubMode.Realm)
			{
				this.ToggleKingdoms();
			}
		}
	}

	// Token: 0x0600231F RID: 8991 RVA: 0x0013E918 File Offset: 0x0013CB18
	private void ToggleRealms()
	{
		ViewMode.current.SetSubMode(ViewMode.SubMode.Realm);
		UIMinimapOverlay.minimap_filter_settings_changed = true;
	}

	// Token: 0x06002320 RID: 8992 RVA: 0x0013E92C File Offset: 0x0013CB2C
	private void ToggleKingdoms()
	{
		ViewMode.current.SetSubMode(ViewMode.SubMode.Kingdom);
		UIMinimapOverlay.minimap_filter_settings_changed = true;
		WorldUI worldUI = WorldUI.Get();
		global::Settlement settlement = (worldUI != null && worldUI.selected_obj != null) ? worldUI.selected_obj.GetComponent<global::Settlement>() : null;
		if (settlement != null)
		{
			worldUI.SelectKingdom(settlement.GetKingdomID(), true);
		}
	}

	// Token: 0x06002321 RID: 8993 RVA: 0x000023FD File Offset: 0x000005FD
	private void OnViewModeButtonLongPress(MinimapViewModeButton VMButton)
	{
	}

	// Token: 0x06002322 RID: 8994 RVA: 0x0013E98C File Offset: 0x0013CB8C
	public void OnViewModeSelect(MinimapViewModeButton VMButton)
	{
		if (VMButton == null)
		{
			return;
		}
		this.ButtonsScript.selected = VMButton;
		MinimapButtons buttonsScript = this.ButtonsScript;
		ViewMode viewMode;
		if (buttonsScript == null)
		{
			viewMode = null;
		}
		else
		{
			MinimapViewModeButton selected = buttonsScript.selected;
			viewMode = ((selected != null) ? selected.view_mode : null);
		}
		ViewMode vm = viewMode;
		if (!VMButton.listMenuPalette.activeSelf)
		{
			this.ChangeViewMode(vm);
		}
		this.UpdateViewModeButtons();
	}

	// Token: 0x06002323 RID: 8995 RVA: 0x0013E9E8 File Offset: 0x0013CBE8
	private void OnListMenuPaletteButtonClick(Button button)
	{
		EventSystem current = EventSystem.current;
		if (current != null)
		{
			current.SetSelectedGameObject(null);
		}
		MinimapPaletteButton component = button.GetComponent<MinimapPaletteButton>();
		if (component == null)
		{
			return;
		}
		MinimapViewModeButton component2 = button.transform.parent.parent.GetComponent<MinimapViewModeButton>();
		if (component2 == null)
		{
			return;
		}
		if (this.minimapButtonsPalette == null)
		{
			return;
		}
		MinimapPalette component3 = global::Common.GetComponent<MinimapPalette>(this.minimapButtonsPalette, null);
		if (component3 == null)
		{
			return;
		}
		component3.selectedButtons.Remove(component2.view_mode);
		component3.selectedButtons.Add(component.view_mode);
		GameObject gameObject = global::Common.FindChildByName(button.transform.parent.parent.gameObject, "id_VMButton", true, true);
		if (gameObject != null)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		GameObject paletteButton = global::Common.FindChildByName(button.gameObject, "id_Button", true, true);
		gameObject = component2.SpawnNewChildButton(paletteButton);
		component2.view_mode = component.view_mode;
		this.RefreshPaletteButtons();
		component2.listMenuPalette.SetActive(false);
	}

	// Token: 0x06002324 RID: 8996 RVA: 0x0013EAEC File Offset: 0x0013CCEC
	private void OnMinimapPaletteButtonClick(MinimapViewModeButton button)
	{
		if (button == null)
		{
			return;
		}
		EventSystem current = EventSystem.current;
		if (current != null)
		{
			current.SetSelectedGameObject(null);
		}
		if (this.ButtonsScript.selected != null)
		{
			this.ButtonsScript.selected.Select(false);
		}
		ViewMode view_mode = button.view_mode;
		button.Select(!view_mode.IsActive());
		this.ChangeViewMode(view_mode);
	}

	// Token: 0x06002325 RID: 8997 RVA: 0x0013EB58 File Offset: 0x0013CD58
	private void ChangeViewMode(ViewMode vm)
	{
		if (vm == null)
		{
			return;
		}
		if (!UICommon.GetModifierKey(UICommon.ModifierKey.Ctrl) || ViewMode.IsPoliticalView())
		{
			if (vm.IsActive())
			{
				ViewMode.WorldView.Apply();
			}
			else
			{
				vm.Apply();
			}
			if (ViewMode.IsPoliticalView())
			{
				BaseUI baseUI = BaseUI.Get();
				Logic.Kingdom kingdom;
				if (baseUI == null)
				{
					kingdom = null;
				}
				else
				{
					Logic.Object selected_logic_obj = baseUI.selected_logic_obj;
					kingdom = ((selected_logic_obj != null) ? selected_logic_obj.GetKingdom() : null);
				}
				Logic.Kingdom kingdom2 = kingdom;
				if (kingdom2 != null && kingdom2.type == Logic.Kingdom.Type.Regular)
				{
					baseUI.SelectObjFromLogic(kingdom2, false, true);
				}
				else if (baseUI != null)
				{
					baseUI.SelectObjFromLogic(BaseUI.LogicKingdom(), false, true);
				}
				this.RefreshPaletteButtons();
			}
			return;
		}
		if (ViewMode.secondary == vm)
		{
			ViewMode.Kingdoms.ApplySecondary();
			return;
		}
		vm.ApplySecondary();
	}

	// Token: 0x06002326 RID: 8998 RVA: 0x0013EC00 File Offset: 0x0013CE00
	private void UpdateToggleButtons()
	{
		if (this.toggleNameplates != null)
		{
			this.toggleNameplates.SetSelected(UserSettings.NameplatesEnabledPV, false);
			if (this.m_ToggleClickableNameplates != null)
			{
				this.m_ToggleClickableNameplatesBackground.gameObject.SetActive(this.toggleNameplates.IsSelected());
				this.m_ToggleClickableNameplates.SetSelected(UserSettings.ClickableNameplates, false);
				Tooltip tooltip = Tooltip.Get(this.m_ToggleClickableNameplates.gameObject, false);
				if (tooltip != null)
				{
					Vars vars = tooltip.vars;
					if (vars != null)
					{
						vars.Set<bool>("is_disabled", !this.m_ToggleClickableNameplates.IsSelected());
					}
				}
			}
		}
		if (this.toggleArmies != null)
		{
			this.toggleArmies.SetSelected((ViewMode.figuresFilter & ViewMode.AllowedFigures.Army) > ViewMode.AllowedFigures.None, false);
			if (this.m_ToggleClickableArmies != null)
			{
				this.m_ToggleClickableArmiesBackground.gameObject.SetActive(this.toggleArmies.IsSelected());
				this.m_ToggleClickableArmies.SetSelected(UserSettings.ClickableArmyPVFigures, false);
				Tooltip tooltip2 = Tooltip.Get(this.m_ToggleClickableArmies.gameObject, false);
				if (tooltip2 != null)
				{
					Vars vars2 = tooltip2.vars;
					if (vars2 != null)
					{
						vars2.Set<bool>("is_disabled", !this.m_ToggleClickableArmies.IsSelected());
					}
				}
			}
		}
		if (this.toggleBattles != null)
		{
			this.toggleBattles.SetSelected((ViewMode.figuresFilter & ViewMode.AllowedFigures.Battle) > ViewMode.AllowedFigures.None, false);
			if (this.m_ToggleClickableBattles != null)
			{
				this.m_ToggleClickableBattlesBackground.gameObject.SetActive(this.toggleBattles.IsSelected());
				this.m_ToggleClickableBattles.SetSelected(UserSettings.ClickableBattlePVFigures, false);
				Tooltip tooltip3 = Tooltip.Get(this.m_ToggleClickableBattles.gameObject, false);
				if (tooltip3 != null)
				{
					Vars vars3 = tooltip3.vars;
					if (vars3 != null)
					{
						vars3.Set<bool>("is_disabled", !this.m_ToggleClickableBattles.IsSelected());
					}
				}
			}
		}
		if (this.toggleTowns != null)
		{
			this.toggleTowns.SetSelected((ViewMode.figuresFilter & ViewMode.AllowedFigures.Castle) > ViewMode.AllowedFigures.None, false);
			if (this.m_ToggleClickableSettlements != null)
			{
				this.m_ToggleClickableSettlementsBackground.gameObject.SetActive(this.toggleTowns.IsSelected());
				this.m_ToggleClickableSettlements.SetSelected(UserSettings.ClickableSettlementPVFigures, false);
				Tooltip tooltip4 = Tooltip.Get(this.m_ToggleClickableSettlements.gameObject, false);
				if (tooltip4 != null)
				{
					Vars vars4 = tooltip4.vars;
					if (vars4 != null)
					{
						vars4.Set<bool>("is_disabled", !this.m_ToggleClickableSettlements.IsSelected());
					}
				}
			}
		}
		if (this.toggleRealms != null)
		{
			this.toggleRealms.SetSelected(ViewMode.current.GetSubMode() == ViewMode.SubMode.Realm, false);
		}
		if (this.toggleKingdoms != null)
		{
			this.toggleKingdoms.SetSelected(ViewMode.current.GetSubMode() == ViewMode.SubMode.Kingdom, false);
		}
	}

	// Token: 0x06002327 RID: 8999 RVA: 0x0013EEC2 File Offset: 0x0013D0C2
	private void OnBackButtonClick(BSGButton b)
	{
		ViewMode.WorldView.Apply();
		this.UpdateViewModeButtons();
	}

	// Token: 0x06002328 RID: 9000 RVA: 0x0013EED4 File Offset: 0x0013D0D4
	private void OnToggleClickableNameplates(BSGButton b)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting("clickable_nameplates");
		setting.ApplyValue(!setting.value);
		UIMinimapOverlay.minimap_filter_settings_changed = true;
	}

	// Token: 0x06002329 RID: 9001 RVA: 0x0013EEFE File Offset: 0x0013D0FE
	private void OnToggleClickableSettlements(BSGButton b)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting("clickable_settlement_pv_figures");
		setting.ApplyValue(!setting.value);
		UIMinimapOverlay.minimap_filter_settings_changed = true;
	}

	// Token: 0x0600232A RID: 9002 RVA: 0x0013EF28 File Offset: 0x0013D128
	private void OnToggleClickableArmies(BSGButton b)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting("clickable_army_pv_figures");
		setting.ApplyValue(!setting.value);
		UIMinimapOverlay.minimap_filter_settings_changed = true;
	}

	// Token: 0x0600232B RID: 9003 RVA: 0x0013EF52 File Offset: 0x0013D152
	private void OnToggleClickableBattles(BSGButton b)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting("clickable_battle_pv_figures");
		setting.ApplyValue(!setting.value);
		UIMinimapOverlay.minimap_filter_settings_changed = true;
	}

	// Token: 0x0600232C RID: 9004 RVA: 0x0013EF7C File Offset: 0x0013D17C
	private void OnToggleNameplates(BSGButton b)
	{
		BaseUI.ToggleNameplates();
		UIMinimapOverlay.minimap_filter_settings_changed = true;
	}

	// Token: 0x0600232D RID: 9005 RVA: 0x0013EF89 File Offset: 0x0013D189
	private void OnToogleArmies(BSGButton b)
	{
		if ((ViewMode.figuresFilter & ViewMode.AllowedFigures.Army) > ViewMode.AllowedFigures.None)
		{
			ViewMode.SetFilter(ViewMode.figuresFilter &= ~(ViewMode.AllowedFigures.Army | ViewMode.AllowedFigures.Mercenary));
		}
		else
		{
			ViewMode.SetFilter(ViewMode.figuresFilter |= (ViewMode.AllowedFigures.Army | ViewMode.AllowedFigures.Mercenary));
		}
		UIMinimapOverlay.minimap_filter_settings_changed = true;
	}

	// Token: 0x0600232E RID: 9006 RVA: 0x0013EFC4 File Offset: 0x0013D1C4
	private void OnToggleBattles(BSGButton b)
	{
		if ((ViewMode.figuresFilter & ViewMode.AllowedFigures.Battle) > ViewMode.AllowedFigures.None)
		{
			ViewMode.SetFilter(ViewMode.figuresFilter &= ~ViewMode.AllowedFigures.Battle);
		}
		else
		{
			ViewMode.SetFilter(ViewMode.figuresFilter |= ViewMode.AllowedFigures.Battle);
		}
		UIMinimapOverlay.minimap_filter_settings_changed = true;
	}

	// Token: 0x0600232F RID: 9007 RVA: 0x0013EFFF File Offset: 0x0013D1FF
	private void OnToggleTowns(BSGButton b)
	{
		if ((ViewMode.figuresFilter & ViewMode.AllowedFigures.Castle) > ViewMode.AllowedFigures.None)
		{
			ViewMode.SetFilter(ViewMode.figuresFilter &= ~ViewMode.AllowedFigures.Castle);
		}
		else
		{
			ViewMode.SetFilter(ViewMode.figuresFilter |= ViewMode.AllowedFigures.Castle);
		}
		UIMinimapOverlay.minimap_filter_settings_changed = true;
	}

	// Token: 0x06002330 RID: 9008 RVA: 0x0013F03A File Offset: 0x0013D23A
	private void OnToggleRealms(BSGButton b)
	{
		this.ToggleRealms();
	}

	// Token: 0x06002331 RID: 9009 RVA: 0x0013F042 File Offset: 0x0013D242
	private void OnToggleKingdoms(BSGButton b)
	{
		this.ToggleKingdoms();
	}

	// Token: 0x06002332 RID: 9010 RVA: 0x0013F04A File Offset: 0x0013D24A
	private void OnHomeClick(BSGButton b)
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		worldUI.LookAt((kingdom != null) ? kingdom.GetCapital() : null, false);
	}

	// Token: 0x06002333 RID: 9011 RVA: 0x0013F070 File Offset: 0x0013D270
	private bool IsPaletteButtonApplied(Transform transform)
	{
		MinimapPaletteButton component = global::Common.GetComponent<MinimapPaletteButton>(transform, null);
		if (component == null || this.minimapButtonsPalette == null)
		{
			return false;
		}
		MinimapPalette component2 = global::Common.GetComponent<MinimapPalette>(this.minimapButtonsPalette, null);
		if (component2 == null || component2.selectedButtons == null)
		{
			return false;
		}
		for (int i = 0; i < component2.selectedButtons.Count; i++)
		{
			if (component2.selectedButtons[i] == component.view_mode)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04001777 RID: 6007
	[UIFieldTarget("id_Minimap")]
	private GameObject minimapObject;

	// Token: 0x04001778 RID: 6008
	[UIFieldTarget("id_ViewModeButtons")]
	private GameObject minimap_buttons;

	// Token: 0x04001779 RID: 6009
	[UIFieldTarget("id_ButtonsPalette")]
	private GameObject buttonsPalette;

	// Token: 0x0400177A RID: 6010
	[UIFieldTarget("id_MinimapButtonsPalette")]
	private GameObject minimapButtonsPalette;

	// Token: 0x0400177B RID: 6011
	[UIFieldTarget("id_BackButton")]
	private BSGButton minimapBackButton;

	// Token: 0x0400177C RID: 6012
	[UIFieldTarget("id_ViewModeButtons")]
	public MinimapButtons ButtonsScript;

	// Token: 0x0400177D RID: 6013
	[UIFieldTarget("id_ToggleNameplates")]
	private BSGButton toggleNameplates;

	// Token: 0x0400177E RID: 6014
	[UIFieldTarget("id_ToggleTowns")]
	private BSGButton toggleTowns;

	// Token: 0x0400177F RID: 6015
	[UIFieldTarget("id_ToggleArmies")]
	private BSGButton toggleArmies;

	// Token: 0x04001780 RID: 6016
	[UIFieldTarget("id_ToggleBattles")]
	private BSGButton toggleBattles;

	// Token: 0x04001781 RID: 6017
	[UIFieldTarget("id_ToggleClickableNameplates")]
	private BSGButton m_ToggleClickableNameplates;

	// Token: 0x04001782 RID: 6018
	[UIFieldTarget("id_ToggleClickableNameplatesBackground")]
	private GameObject m_ToggleClickableNameplatesBackground;

	// Token: 0x04001783 RID: 6019
	[UIFieldTarget("id_ToggleClickableSettlements")]
	private BSGButton m_ToggleClickableSettlements;

	// Token: 0x04001784 RID: 6020
	[UIFieldTarget("id_ToggleClickableSettlementsBackground")]
	private GameObject m_ToggleClickableSettlementsBackground;

	// Token: 0x04001785 RID: 6021
	[UIFieldTarget("id_ToggleClickableArmies")]
	private BSGButton m_ToggleClickableArmies;

	// Token: 0x04001786 RID: 6022
	[UIFieldTarget("id_ToggleClickableArmiesBackground")]
	private GameObject m_ToggleClickableArmiesBackground;

	// Token: 0x04001787 RID: 6023
	[UIFieldTarget("id_ToggleClickableBattles")]
	private BSGButton m_ToggleClickableBattles;

	// Token: 0x04001788 RID: 6024
	[UIFieldTarget("id_ToggleClickableBattlesBackground")]
	private GameObject m_ToggleClickableBattlesBackground;

	// Token: 0x04001789 RID: 6025
	[UIFieldTarget("id_Group_PoliticalView")]
	private GameObject groupPoliticalView;

	// Token: 0x0400178A RID: 6026
	[UIFieldTarget("id_ToggleRealm")]
	private BSGButton toggleRealms;

	// Token: 0x0400178B RID: 6027
	[UIFieldTarget("id_ToggleKingdom")]
	private BSGButton toggleKingdoms;

	// Token: 0x0400178C RID: 6028
	[UIFieldTarget("id_HomeButton")]
	private BSGButton homeButton;

	// Token: 0x0400178D RID: 6029
	[UIFieldTarget("id_Legend")]
	private BSGButton legend_button;

	// Token: 0x0400178E RID: 6030
	[UIFieldTarget("id_SelectedView")]
	private TextMeshProUGUI selectedView;

	// Token: 0x0400178F RID: 6031
	[UIFieldTarget("id_BackText")]
	private TextMeshProUGUI backText;

	// Token: 0x04001790 RID: 6032
	[UIFieldTarget("id_HomeText")]
	private TextMeshProUGUI homeText;

	// Token: 0x04001791 RID: 6033
	public static bool minimap_filter_settings_changed;

	// Token: 0x04001792 RID: 6034
	public GameObject textPaletteButtonPrefab;

	// Token: 0x04001793 RID: 6035
	public GameObject imagePaletteButtonPrefab;

	// Token: 0x04001794 RID: 6036
	private List<MinimapViewModeButton> palettebuttons = new List<MinimapViewModeButton>();

	// Token: 0x04001795 RID: 6037
	private List<MinimapViewModeButton> viewbuttons = new List<MinimapViewModeButton>();

	// Token: 0x04001796 RID: 6038
	private Color paletteButtonColor = new Color(0.913f, 0.533f, 0.133f);

	// Token: 0x04001797 RID: 6039
	private DT.Field buttonsDef;

	// Token: 0x04001798 RID: 6040
	private bool m_Initalized;

	// Token: 0x04001799 RID: 6041
	public DT.Field Defs;

	// Token: 0x0400179A RID: 6042
	public GameObject listMenuShown;

	// Token: 0x0400179B RID: 6043
	public Logic.Kingdom Kingdom;
}

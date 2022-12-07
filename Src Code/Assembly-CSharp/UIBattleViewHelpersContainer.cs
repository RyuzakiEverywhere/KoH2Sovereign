using System;
using Logic;
using UnityEngine;

// Token: 0x020001CD RID: 461
public class UIBattleViewHelpersContainer : MonoBehaviour
{
	// Token: 0x06001B17 RID: 6935 RVA: 0x0010426A File Offset: 0x0010246A
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06001B18 RID: 6936 RVA: 0x00104272 File Offset: 0x00102472
	private void OnDestroy()
	{
		if (this.m_buttonsGroup != null)
		{
			ButtonsGroup buttonsGroup = this.m_buttonsGroup;
			buttonsGroup.onButtonSelected = (ButtonsGroup.OnSelected)Delegate.Remove(buttonsGroup.onButtonSelected, new ButtonsGroup.OnSelected(this.ButtonsGroup_OnButtonSelected));
		}
	}

	// Token: 0x06001B19 RID: 6937 RVA: 0x001042A4 File Offset: 0x001024A4
	private void Initialize()
	{
		UICommon.FindComponents(this, false);
		if (this.m_definition == null)
		{
			this.m_definition = global::Defs.GetDefField("UIBattleViewHelpersContainer", null);
		}
		this.m_ArmyCommandsButton.AllowSelection(true);
		this.m_FiltersButton.AllowSelection(true);
		this.m_buttonsGroup = new ButtonsGroup();
		this.m_buttonsGroup.AddButton(this.m_ArmyCommandsButton, true);
		this.m_buttonsGroup.AddButton(this.m_FiltersButton, true);
		this.m_buttonsGroup.AllowSwitchOff(true);
		ButtonsGroup buttonsGroup = this.m_buttonsGroup;
		buttonsGroup.onButtonSelected = (ButtonsGroup.OnSelected)Delegate.Combine(buttonsGroup.onButtonSelected, new ButtonsGroup.OnSelected(this.ButtonsGroup_OnButtonSelected));
		this.SetupButtons();
	}

	// Token: 0x06001B1A RID: 6938 RVA: 0x00104354 File Offset: 0x00102554
	private void SetupButtons()
	{
		this.SetupButton(this.m_ArmyCommandsButton, "ArmyCommandsButton", "ArmyCommandsButton", new BSGButton.OnClick(this.ArmyCommandsButton_OnClicked));
		this.SetupButton(this.m_FiltersButton, "FiltersButton", "FiltersWindowButton", new BSGButton.OnClick(this.FiltersButton_OnClicked));
	}

	// Token: 0x06001B1B RID: 6939 RVA: 0x001043A8 File Offset: 0x001025A8
	private void SetupButton(BSGButton button, string definitionName, string tooltipDefinitionName, BSGButton.OnClick callback)
	{
		if (button != null)
		{
			button.onClick = (BSGButton.OnClick)Delegate.Combine(button.onClick, callback);
		}
		BSGButtonImage component = button.GetComponent<BSGButtonImage>();
		DT.Field field = this.m_definition.FindChild(definitionName, null, true, true, true, '.');
		component.normalImage = (component.disabledImage = global::Defs.GetObj<Sprite>(field, "normal", null));
		component.rolloverImage = global::Defs.GetObj<Sprite>(field, "hover", null);
		component.pressedImage = (component.selectedImage = global::Defs.GetObj<Sprite>(field, "selected", null));
		button.UpdateState(false);
		if (button != null)
		{
			Tooltip.Get(button.gameObject, true).SetDef(tooltipDefinitionName, null);
		}
	}

	// Token: 0x06001B1C RID: 6940 RVA: 0x0010445C File Offset: 0x0010265C
	private void ButtonsGroup_OnButtonSelected(BSGButton button)
	{
		if (!button.IsSelected())
		{
			if (button == this.m_ArmyCommandsButton && this.m_armyCommandsWindow != null)
			{
				this.CloseArmyCommandsPanel();
				return;
			}
			if (button == this.m_FiltersButton && this.m_filtersWindow != null)
			{
				this.CloseFiltersPanel();
			}
		}
	}

	// Token: 0x06001B1D RID: 6941 RVA: 0x001044B6 File Offset: 0x001026B6
	private void FiltersButton_OnClicked(BSGButton button)
	{
		if (button.IsSelected() && this.m_filtersWindow == null)
		{
			this.CreateFiltersPanel();
			return;
		}
		this.CloseFiltersPanel();
	}

	// Token: 0x06001B1E RID: 6942 RVA: 0x001044DC File Offset: 0x001026DC
	private void CreateFiltersPanel()
	{
		if (this.m_filtersWindow != null)
		{
			UICommon.SetAligment(this.m_filtersWindow.transform as RectTransform, TextAnchor.LowerRight);
			UICommon.DockTo(this.m_filtersWindow.transform as RectTransform, this.m_SpawnPoint, TextAnchor.UpperRight, Vector2.zero);
			return;
		}
		BattleViewUI battleViewUI = BattleViewUI.Get();
		GameObject gameObject = global::Common.FindChildByName((battleViewUI != null) ? battleViewUI.gameObject : null, "id_MessageContainer", true, true);
		GameObject prefab = UIBattleViewFiltersWindow.GetPrefab();
		if (prefab != null && gameObject != null)
		{
			GameObject gameObject2 = global::Common.Spawn(prefab, gameObject.transform, true, "");
			this.m_filtersWindow = gameObject2.GetComponent<UIBattleViewFiltersWindow>();
			if (this.m_filtersWindow != null)
			{
				UICommon.SetAligment(this.m_filtersWindow.transform as RectTransform, TextAnchor.LowerRight);
				UICommon.DockTo(this.m_filtersWindow.transform as RectTransform, this.m_SpawnPoint, TextAnchor.UpperRight, Vector2.zero);
				this.m_filtersWindow.Show();
				UIBattleViewFiltersWindow filtersWindow = this.m_filtersWindow;
				filtersWindow.on_close = (UIWindow.OnClose)Delegate.Combine(filtersWindow.on_close, new UIWindow.OnClose(this.FiltersWindow_OnClose));
				return;
			}
			global::Common.DestroyObj(gameObject2);
		}
	}

	// Token: 0x06001B1F RID: 6943 RVA: 0x00104608 File Offset: 0x00102808
	private void CloseFiltersPanel()
	{
		if (this.m_filtersWindow != null)
		{
			UIBattleViewFiltersWindow filtersWindow = this.m_filtersWindow;
			filtersWindow.on_close = (UIWindow.OnClose)Delegate.Remove(filtersWindow.on_close, new UIWindow.OnClose(this.FiltersWindow_OnClose));
			this.m_filtersWindow.Close(false);
			this.m_filtersWindow = null;
		}
	}

	// Token: 0x06001B20 RID: 6944 RVA: 0x00104660 File Offset: 0x00102860
	private void FiltersWindow_OnClose(UIWindow wnd)
	{
		this.m_filtersWindow = null;
		wnd.on_close = (UIWindow.OnClose)Delegate.Remove(wnd.on_close, new UIWindow.OnClose(this.FiltersWindow_OnClose));
		this.m_FiltersButton.SetSelected(false, false);
		this.m_buttonsGroup.Refresh();
	}

	// Token: 0x06001B21 RID: 6945 RVA: 0x001046AE File Offset: 0x001028AE
	private void ArmyCommandsButton_OnClicked(BSGButton button)
	{
		if (button.IsSelected() && this.m_armyCommandsWindow == null)
		{
			this.CreateArmyCommandsPanel();
			return;
		}
		this.CloseArmyCommandsPanel();
	}

	// Token: 0x06001B22 RID: 6946 RVA: 0x001046D4 File Offset: 0x001028D4
	private void CreateArmyCommandsPanel()
	{
		if (this.m_armyCommandsWindow != null)
		{
			UICommon.SetAligment(this.m_armyCommandsWindow.transform as RectTransform, TextAnchor.LowerRight);
			UICommon.DockTo(this.m_armyCommandsWindow.transform as RectTransform, this.m_SpawnPoint, TextAnchor.UpperRight, Vector2.zero);
			return;
		}
		BattleViewUI battleViewUI = BattleViewUI.Get();
		GameObject gameObject = global::Common.FindChildByName((battleViewUI != null) ? battleViewUI.gameObject : null, "id_MessageContainer", true, true);
		GameObject prefab = UIBattleViewArmyCommandsWindow.GetPrefab();
		if (prefab != null && gameObject != null)
		{
			GameObject gameObject2 = global::Common.Spawn(prefab, gameObject.transform, true, "");
			this.m_armyCommandsWindow = gameObject2.GetComponent<UIBattleViewArmyCommandsWindow>();
			if (this.m_armyCommandsWindow != null)
			{
				UICommon.SetAligment(this.m_armyCommandsWindow.transform as RectTransform, TextAnchor.LowerRight);
				UICommon.DockTo(this.m_armyCommandsWindow.transform as RectTransform, this.m_SpawnPoint, TextAnchor.UpperRight, Vector2.zero);
				this.m_armyCommandsWindow.Show();
				UIBattleViewArmyCommandsWindow armyCommandsWindow = this.m_armyCommandsWindow;
				armyCommandsWindow.on_close = (UIWindow.OnClose)Delegate.Combine(armyCommandsWindow.on_close, new UIWindow.OnClose(this.ArmyCommandsWindow_OnClose));
				return;
			}
			global::Common.DestroyObj(gameObject2);
		}
	}

	// Token: 0x06001B23 RID: 6947 RVA: 0x00104800 File Offset: 0x00102A00
	private void CloseArmyCommandsPanel()
	{
		if (this.m_armyCommandsWindow != null)
		{
			UIBattleViewArmyCommandsWindow armyCommandsWindow = this.m_armyCommandsWindow;
			armyCommandsWindow.on_close = (UIWindow.OnClose)Delegate.Remove(armyCommandsWindow.on_close, new UIWindow.OnClose(this.ArmyCommandsWindow_OnClose));
			this.m_armyCommandsWindow.Close(false);
			this.m_armyCommandsWindow = null;
		}
	}

	// Token: 0x06001B24 RID: 6948 RVA: 0x00104858 File Offset: 0x00102A58
	private void ArmyCommandsWindow_OnClose(UIWindow wnd)
	{
		UIBattleViewArmyCommandsWindow armyCommandsWindow = this.m_armyCommandsWindow;
		armyCommandsWindow.on_close = (UIWindow.OnClose)Delegate.Remove(armyCommandsWindow.on_close, new UIWindow.OnClose(this.ArmyCommandsWindow_OnClose));
		this.m_armyCommandsWindow = null;
		wnd.on_close = (UIWindow.OnClose)Delegate.Remove(wnd.on_close, new UIWindow.OnClose(this.ArmyCommandsWindow_OnClose));
		this.m_ArmyCommandsButton.SetSelected(false, false);
		this.m_buttonsGroup.Refresh();
	}

	// Token: 0x040011A3 RID: 4515
	[UIFieldTarget("id_ArmyCommandsButton")]
	private BSGButton m_ArmyCommandsButton;

	// Token: 0x040011A4 RID: 4516
	[UIFieldTarget("id_FiltersButton")]
	private BSGButton m_FiltersButton;

	// Token: 0x040011A5 RID: 4517
	[UIFieldTarget("id_SpawnPoint")]
	protected RectTransform m_SpawnPoint;

	// Token: 0x040011A6 RID: 4518
	private DT.Field m_definition;

	// Token: 0x040011A7 RID: 4519
	private UIBattleViewArmyCommandsWindow m_armyCommandsWindow;

	// Token: 0x040011A8 RID: 4520
	private UIBattleViewFiltersWindow m_filtersWindow;

	// Token: 0x040011A9 RID: 4521
	private ButtonsGroup m_buttonsGroup;
}

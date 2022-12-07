using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001CF RID: 463
public class UIBattleViewMarkTargetPanel : MonoBehaviour
{
	// Token: 0x06001B3E RID: 6974 RVA: 0x0010547A File Offset: 0x0010367A
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06001B3F RID: 6975 RVA: 0x00105482 File Offset: 0x00103682
	private void OnDestroy()
	{
		this.RemoveButtonListener();
		if (this.m_BattleViewSquadActions != null)
		{
			this.m_BattleViewSquadActions.SquadActionPerformed -= this.BattleViewSquadActions_SquadActionPerformed;
		}
	}

	// Token: 0x06001B40 RID: 6976 RVA: 0x001054A9 File Offset: 0x001036A9
	public void Setup(Vars vars = null)
	{
		this.m_vars = vars;
		this.Initialize();
		this.Refresh();
	}

	// Token: 0x06001B41 RID: 6977 RVA: 0x001054C0 File Offset: 0x001036C0
	private void Initialize()
	{
		if (this.m_isInitialized)
		{
			return;
		}
		this.m_BattleViewSquadActions = BattleViewUI.Get().SquadActions;
		this.m_BattleViewSquadActions.SquadActionPerformed += this.BattleViewSquadActions_SquadActionPerformed;
		this.InitializeDefinition();
		this.SetupBorder();
		this.SetupButton();
		this.Refresh();
		this.m_isInitialized = true;
	}

	// Token: 0x06001B42 RID: 6978 RVA: 0x0010551C File Offset: 0x0010371C
	private void BattleViewSquadActions_SquadActionPerformed(string action)
	{
		if (!(action == "retreating") && !(action == "defeated") && action != this.m_actionKey)
		{
			return;
		}
		this.UpdateBorderState();
		this.UpdateButtonState();
	}

	// Token: 0x06001B43 RID: 6979 RVA: 0x00105553 File Offset: 0x00103753
	private void InitializeDefinition()
	{
		if (this.m_definition == null)
		{
			this.m_definition = global::Defs.GetDefField("UISquadMarkTargetPanel", null);
		}
	}

	// Token: 0x06001B44 RID: 6980 RVA: 0x00105570 File Offset: 0x00103770
	private void SetupBorder()
	{
		this.m_MarkTargetBorderImage = global::Common.GetComponent<Image>(base.transform, null);
		this.m_borderNormalImage = global::Defs.GetObj<Sprite>(this.m_definition, "border_normal", null);
		this.m_borderSelectedImage = global::Defs.GetObj<Sprite>(this.m_definition, "border_selected", null);
	}

	// Token: 0x06001B45 RID: 6981 RVA: 0x001055C0 File Offset: 0x001037C0
	private void SetupButton()
	{
		this.m_MarkTargetButton = global::Common.FindChildComponent<BSGButton>(base.gameObject, "id_MarkTargetButton");
		if (this.m_MarkTargetButton != null)
		{
			this.m_image = global::Common.GetComponent<BSGButtonImage>(this.m_MarkTargetButton.transform, null);
			if (this.m_definition != null)
			{
				this.m_actionKey = this.m_definition.GetString("action_key", null, "", true, true, true, '.');
				this.m_tooltipKey = this.m_definition.GetString("tooltip_key", null, "", true, true, true, '.');
				this.m_tooltipKeybind = this.m_definition.GetString("tooltip_keybind", null, "", true, true, true, '.');
				this.m_MarkTargetButton.AllowSelection(true);
				this.RemoveButtonListener();
				this.AddButtonListener();
				this.SetupSprites();
			}
		}
	}

	// Token: 0x06001B46 RID: 6982 RVA: 0x00105697 File Offset: 0x00103897
	private void AddButtonListener()
	{
		if (this.m_MarkTargetButton != null)
		{
			BSGButton markTargetButton = this.m_MarkTargetButton;
			markTargetButton.onClick = (BSGButton.OnClick)Delegate.Combine(markTargetButton.onClick, new BSGButton.OnClick(this.Button_OnClick));
		}
	}

	// Token: 0x06001B47 RID: 6983 RVA: 0x001056CE File Offset: 0x001038CE
	private void RemoveButtonListener()
	{
		if (this.m_MarkTargetButton != null)
		{
			BSGButton markTargetButton = this.m_MarkTargetButton;
			markTargetButton.onClick = (BSGButton.OnClick)Delegate.Remove(markTargetButton.onClick, new BSGButton.OnClick(this.Button_OnClick));
		}
	}

	// Token: 0x06001B48 RID: 6984 RVA: 0x00105708 File Offset: 0x00103908
	private void SetupSprites()
	{
		if (this.m_image == null)
		{
			return;
		}
		DT.Field definition = this.m_definition;
		this.m_image.normalImage = (this.m_image.disabledImage = global::Defs.GetObj<Sprite>(definition, "button_normal", null));
		this.m_image.rolloverImage = global::Defs.GetObj<Sprite>(definition, "button_hover", null);
		this.m_image.pressedImage = global::Defs.GetObj<Sprite>(definition, "button_pressed", null);
		this.m_image.selectedImage = global::Defs.GetObj<Sprite>(definition, "button_selected", null);
		this.m_image.SetState(BSGButton.State.Normal);
	}

	// Token: 0x06001B49 RID: 6985 RVA: 0x001057A4 File Offset: 0x001039A4
	private void UpdateTooltip()
	{
		Vars vars = new Vars();
		vars.Set<bool>("is_on", this.m_MarkTargetButton.IsSelected());
		vars.Set<bool>("squad_is_ours", this.m_BattleViewSquadActions.AreOurUnitsSelected);
		vars.Set<string>("keybind", KeyBindings.LocalizeKeybind(this.m_tooltipKeybind, 0, true));
		Tooltip.Get(this.m_MarkTargetButton.gameObject, true).SetDef(this.m_tooltipKey, vars);
	}

	// Token: 0x06001B4A RID: 6986 RVA: 0x00105818 File Offset: 0x00103A18
	private void Button_OnClick(BSGButton button)
	{
		if (!this.m_BattleViewSquadActions.AreEnemyUnitsSelected || this.m_BattleViewSquadActions.IsSquadRetreating)
		{
			return;
		}
		if (this.m_BattleViewSquadActions != null)
		{
			this.m_BattleViewSquadActions.CallSquadActions(this.m_actionKey);
		}
		this.Refresh();
	}

	// Token: 0x06001B4B RID: 6987 RVA: 0x00105854 File Offset: 0x00103A54
	public void Refresh()
	{
		this.UpdateButtonState();
		this.UpdateBorderState();
		this.UpdateTooltip();
	}

	// Token: 0x06001B4C RID: 6988 RVA: 0x00105868 File Offset: 0x00103A68
	private void UpdateButtonState()
	{
		this.m_MarkTargetButton.SetSelected(this.m_BattleViewSquadActions.GetActionState(this.m_actionKey), true);
		if (!this.m_BattleViewSquadActions.AreEnemyUnitsSelected || this.m_BattleViewSquadActions.IsSquadRetreating)
		{
			this.m_MarkTargetButton.gameObject.SetActive(false);
			this.m_MarkTargetButton.Enable(false, false);
			return;
		}
		this.m_MarkTargetButton.gameObject.SetActive(true);
		this.m_MarkTargetButton.Enable(this.m_BattleViewSquadActions.AreEnemyUnitsSelected && this.m_BattleViewSquadActions.IsAnySquadAlive && !this.m_BattleViewSquadActions.IsSquadRetreating, false);
	}

	// Token: 0x06001B4D RID: 6989 RVA: 0x00105914 File Offset: 0x00103B14
	private void UpdateBorderState()
	{
		if (!this.m_BattleViewSquadActions.AreEnemyUnitsSelected || this.m_BattleViewSquadActions.IsSquadRetreating)
		{
			this.m_MarkTargetBorderImage.enabled = false;
			return;
		}
		this.m_MarkTargetBorderImage.enabled = true;
		if (this.m_BattleViewSquadActions.GetActionState(this.m_actionKey) && this.m_BattleViewSquadActions.AreEnemyUnitsSelected)
		{
			this.m_MarkTargetBorderImage.sprite = this.m_borderSelectedImage;
			return;
		}
		this.m_MarkTargetBorderImage.sprite = this.m_borderNormalImage;
	}

	// Token: 0x040011B6 RID: 4534
	[UIFieldTarget("MarkTargetBorder")]
	private Image m_MarkTargetBorderImage;

	// Token: 0x040011B7 RID: 4535
	[UIFieldTarget("MarkTargetButton")]
	private BSGButton m_MarkTargetButton;

	// Token: 0x040011B8 RID: 4536
	private BSGButtonImage m_image;

	// Token: 0x040011B9 RID: 4537
	private BattleViewSquadActions m_BattleViewSquadActions;

	// Token: 0x040011BA RID: 4538
	private DT.Field m_definition;

	// Token: 0x040011BB RID: 4539
	private Vars m_vars;

	// Token: 0x040011BC RID: 4540
	private string m_actionKey;

	// Token: 0x040011BD RID: 4541
	private string m_tooltipKey;

	// Token: 0x040011BE RID: 4542
	private string m_tooltipKeybind;

	// Token: 0x040011BF RID: 4543
	private Sprite m_borderNormalImage;

	// Token: 0x040011C0 RID: 4544
	private Sprite m_borderSelectedImage;

	// Token: 0x040011C1 RID: 4545
	private bool m_isInitialized;
}

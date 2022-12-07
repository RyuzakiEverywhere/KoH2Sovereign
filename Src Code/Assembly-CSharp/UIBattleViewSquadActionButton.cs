using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001D6 RID: 470
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(BSGButton))]
[RequireComponent(typeof(BSGButtonImage))]
public class UIBattleViewSquadActionButton : MonoBehaviour
{
	// Token: 0x17000176 RID: 374
	// (get) Token: 0x06001BF5 RID: 7157 RVA: 0x001098C2 File Offset: 0x00107AC2
	// (set) Token: 0x06001BF6 RID: 7158 RVA: 0x001098CA File Offset: 0x00107ACA
	public BSGButton Button
	{
		get
		{
			return this.m_button;
		}
		private set
		{
			this.m_button = value;
		}
	}

	// Token: 0x06001BF7 RID: 7159 RVA: 0x001098D3 File Offset: 0x00107AD3
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06001BF8 RID: 7160 RVA: 0x001098DB File Offset: 0x00107ADB
	private void OnDestroy()
	{
		this.RemoveButtonListener();
		if (this.m_BattleViewSquadActions != null)
		{
			this.m_BattleViewSquadActions.SquadActionPerformed -= this.BattleViewSquadActions_SquadActionPerformed;
			this.m_BattleViewSquadActions.SquadStatesChanged -= this.BattleViewSquadActions_SquadStatesChanged;
		}
	}

	// Token: 0x06001BF9 RID: 7161 RVA: 0x0010991C File Offset: 0x00107B1C
	public void Setup(DT.Field squad)
	{
		this.m_definition = squad;
		if (this.m_definition == null)
		{
			return;
		}
		this.m_actionKey = this.m_definition.GetString("action_key", null, "", true, true, true, '.');
		this.m_tooltipKey = this.m_definition.GetString("tooltip_key", null, "", true, true, true, '.');
		this.m_tooltipKeybind = this.m_definition.GetString("tooltip_keybind", null, "", true, true, true, '.');
		if (this.m_button != null)
		{
			this.m_button.AllowSelection(this.m_definition.GetBool("is_toggle", null, false, true, true, true, '.'));
		}
		this.RemoveButtonListener();
		this.AddButtonListener();
		this.SetupSprites();
		this.Refresh();
	}

	// Token: 0x06001BFA RID: 7162 RVA: 0x001099E4 File Offset: 0x00107BE4
	public void SetGroup(ButtonsGroup group)
	{
		this.m_group = group;
		this.m_group.AddButton(this.m_button, true);
	}

	// Token: 0x06001BFB RID: 7163 RVA: 0x001099FF File Offset: 0x00107BFF
	public void Refresh()
	{
		if (!this.IsActive())
		{
			return;
		}
		this.UpdateButtonState();
		this.UpdateTooltip();
	}

	// Token: 0x06001BFC RID: 7164 RVA: 0x00101716 File Offset: 0x000FF916
	public void SetActive(bool active)
	{
		base.gameObject.SetActive(active);
	}

	// Token: 0x06001BFD RID: 7165 RVA: 0x00109A16 File Offset: 0x00107C16
	public DT.Field GetDefinition()
	{
		return this.m_definition;
	}

	// Token: 0x06001BFE RID: 7166 RVA: 0x00109A1E File Offset: 0x00107C1E
	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	// Token: 0x06001BFF RID: 7167 RVA: 0x00109A2C File Offset: 0x00107C2C
	private void Initialize()
	{
		if (this.m_BattleViewSquadActions == null)
		{
			this.m_BattleViewSquadActions = BattleViewUI.Get().SquadActions;
		}
		this.m_button = global::Common.GetComponent<BSGButton>(base.transform, null);
		this.m_image = global::Common.GetComponent<BSGButtonImage>(base.transform, null);
		this.m_BattleViewSquadActions.SquadActionPerformed += this.BattleViewSquadActions_SquadActionPerformed;
		this.m_BattleViewSquadActions.SquadStatesChanged += this.BattleViewSquadActions_SquadStatesChanged;
	}

	// Token: 0x06001C00 RID: 7168 RVA: 0x00109AA3 File Offset: 0x00107CA3
	private void AddButtonListener()
	{
		if (this.m_button != null)
		{
			BSGButton button = this.m_button;
			button.onClick = (BSGButton.OnClick)Delegate.Combine(button.onClick, new BSGButton.OnClick(this.Button_OnClick));
		}
	}

	// Token: 0x06001C01 RID: 7169 RVA: 0x00109ADA File Offset: 0x00107CDA
	private void RemoveButtonListener()
	{
		if (this.m_button != null)
		{
			BSGButton button = this.m_button;
			button.onClick = (BSGButton.OnClick)Delegate.Remove(button.onClick, new BSGButton.OnClick(this.Button_OnClick));
		}
	}

	// Token: 0x06001C02 RID: 7170 RVA: 0x00109B14 File Offset: 0x00107D14
	private void BattleViewSquadActions_SquadActionPerformed(string action)
	{
		if (!(action == "retreating") && !(action == "defeated"))
		{
			if (action != this.m_actionKey)
			{
				return;
			}
			if (this.m_group != null)
			{
				this.m_group.ButtonSelected(this.m_button);
			}
		}
		this.UpdateButtonState();
		this.UpdateTooltip();
	}

	// Token: 0x06001C03 RID: 7171 RVA: 0x00109B6F File Offset: 0x00107D6F
	private void BattleViewSquadActions_SquadStatesChanged()
	{
		this.UpdateButtonState();
		this.UpdateTooltip();
	}

	// Token: 0x06001C04 RID: 7172 RVA: 0x00109B80 File Offset: 0x00107D80
	private void SetupSprites()
	{
		if (this.m_image == null)
		{
			return;
		}
		DT.Field definition = this.m_definition;
		this.m_image.normalImage = (this.m_image.disabledImage = global::Defs.GetObj<Sprite>(definition, "normal", null));
		this.m_image.rolloverImage = global::Defs.GetObj<Sprite>(definition, "hover", null);
		this.m_image.pressedImage = (this.m_image.selectedImage = global::Defs.GetObj<Sprite>(definition, "selected", null));
		this.m_image.SetState(BSGButton.State.Normal);
	}

	// Token: 0x06001C05 RID: 7173 RVA: 0x00109C10 File Offset: 0x00107E10
	private void UpdateTooltip()
	{
		Vars vars = new Vars();
		vars.Set<bool>("is_on", this.m_button.IsSelected());
		vars.Set<bool>("squad_is_ours", this.m_BattleViewSquadActions.AreOurUnitsSelected);
		vars.Set<bool>("can_deploy", this.m_BattleViewSquadActions.CanSquadsDeploy);
		vars.Set<bool>("is_walking", this.m_BattleViewSquadActions.GetActionState("charge"));
		vars.Set<bool>("is_deploy_blocked", this.m_BattleViewSquadActions.IsDeployBlocked || this.m_BattleViewSquadActions.IsDeployingInProgress || this.m_BattleViewSquadActions.IsPackingInProgress);
		vars.Set<bool>("is_deploying", this.m_BattleViewSquadActions.IsDeployingInProgress);
		vars.Set<bool>("is_packing", this.m_BattleViewSquadActions.IsPackingInProgress);
		vars.Set<string>("keybind", KeyBindings.LocalizeKeybind(this.m_tooltipKeybind, 0, true));
		Tooltip.Get(this.m_button.gameObject, true).SetDef(this.m_tooltipKey, vars);
	}

	// Token: 0x06001C06 RID: 7174 RVA: 0x00109D14 File Offset: 0x00107F14
	private void UpdateButtonState()
	{
		this.m_button.SetSelected(this.m_BattleViewSquadActions.GetActionState(this.m_actionKey), true);
		if ((!this.m_BattleViewSquadActions.AreOurUnitsSelected || this.m_BattleViewSquadActions.IsSquadRetreating || (this.m_BattleViewSquadActions.IsDeployBlocked && this.m_actionKey == "deploy")) && this.m_button.IsSelected())
		{
			this.m_button.Enable(false, false);
			this.m_image.SetState(BSGButton.State.Selected);
		}
		else
		{
			this.m_button.Enable(this.m_BattleViewSquadActions.AreOurUnitsSelected && this.m_BattleViewSquadActions.IsAnySquadAlive && !this.m_BattleViewSquadActions.IsSquadRetreating && (!this.m_BattleViewSquadActions.IsDeployBlocked || !(this.m_actionKey == "deploy")), false);
		}
		if (this.m_group != null)
		{
			this.m_group.Refresh();
		}
	}

	// Token: 0x06001C07 RID: 7175 RVA: 0x00109E0B File Offset: 0x0010800B
	private void Button_OnClick(BSGButton button)
	{
		if (!this.m_BattleViewSquadActions.AreOurUnitsSelected || this.m_BattleViewSquadActions.IsSquadRetreating)
		{
			return;
		}
		if (this.m_BattleViewSquadActions != null)
		{
			this.m_BattleViewSquadActions.CallSquadActions(this.m_actionKey);
		}
		this.Refresh();
	}

	// Token: 0x04001231 RID: 4657
	private BSGButton m_button;

	// Token: 0x04001232 RID: 4658
	private BSGButtonImage m_image;

	// Token: 0x04001233 RID: 4659
	private ButtonsGroup m_group;

	// Token: 0x04001234 RID: 4660
	private BattleViewSquadActions m_BattleViewSquadActions;

	// Token: 0x04001235 RID: 4661
	private DT.Field m_definition;

	// Token: 0x04001236 RID: 4662
	private string m_actionKey;

	// Token: 0x04001237 RID: 4663
	private string m_tooltipKey;

	// Token: 0x04001238 RID: 4664
	private string m_tooltipKeybind;
}

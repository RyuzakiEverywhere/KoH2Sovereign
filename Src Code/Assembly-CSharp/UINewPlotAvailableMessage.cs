using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020001F1 RID: 497
public class UINewPlotAvailableMessage : MonoBehaviour
{
	// Token: 0x1700018C RID: 396
	// (get) Token: 0x06001DF8 RID: 7672 RVA: 0x001186BA File Offset: 0x001168BA
	// (set) Token: 0x06001DF9 RID: 7673 RVA: 0x001186C2 File Offset: 0x001168C2
	public Opportunity Data { get; private set; }

	// Token: 0x06001DFA RID: 7674 RVA: 0x001186CB File Offset: 0x001168CB
	private void Start()
	{
		this.ExtractData();
		this.m_Started = true;
	}

	// Token: 0x06001DFB RID: 7675 RVA: 0x001186DA File Offset: 0x001168DA
	private void OnEnable()
	{
		if (this.m_Started)
		{
			this.m_DataExtracted = false;
		}
	}

	// Token: 0x06001DFC RID: 7676 RVA: 0x001186EB File Offset: 0x001168EB
	private void LateUpdate()
	{
		if (!this.m_DataExtracted)
		{
			this.ExtractData();
		}
	}

	// Token: 0x06001DFD RID: 7677 RVA: 0x001186FC File Offset: 0x001168FC
	private void ExtractData()
	{
		this.m_DataExtracted = true;
		MessageWnd component = base.GetComponent<MessageWnd>();
		this.field = component.def_field;
		if (component)
		{
			Opportunity opportunity = component.vars.obj.obj_val as Opportunity;
			if (opportunity != null)
			{
				this.SetData(opportunity, component.vars);
			}
		}
	}

	// Token: 0x06001DFE RID: 7678 RVA: 0x00118751 File Offset: 0x00116951
	private void Update()
	{
		this.UpdateActivateButtonState();
	}

	// Token: 0x06001DFF RID: 7679 RVA: 0x0011875C File Offset: 0x0011695C
	public void SetData(Opportunity o, Vars vars)
	{
		UICommon.FindComponents(this, false);
		this.Data = o;
		this.Vars = vars;
		if (this.Data == null || this.Vars == null)
		{
			this.Close();
			return;
		}
		this.activateButton = this.FindActivateButton();
		if (this.activateButton != null)
		{
			Tooltip tooltip = Tooltip.Get(this.activateButton.gameObject, true);
			if (tooltip != null)
			{
				Tooltip tooltip2 = tooltip;
				Opportunity data = this.Data;
				tooltip2.SetObj((data != null) ? data.action : null, null, null);
				tooltip.vars.Set<Logic.Object>("target", this.Data.target);
				tooltip.vars.Set<List<Value>>("args", this.Data.args);
			}
			this.activateButton.onClick = new BSGButton.OnClick(this.ExecutePlot);
		}
	}

	// Token: 0x06001E00 RID: 7680 RVA: 0x00118834 File Offset: 0x00116A34
	private void UpdateActivateButtonState()
	{
		if (this.activateButton == null)
		{
			return;
		}
		Opportunity data = this.Data;
		if (((data != null) ? data.action : null) == null)
		{
			this.activateButton.Enable(false, false);
			return;
		}
		bool flag = false;
		Opportunity data2 = this.Data;
		Action a = (data2 != null) ? data2.action : null;
		Opportunity data3 = this.Data;
		Logic.Object temp_target;
		if (data3 == null)
		{
			temp_target = null;
		}
		else
		{
			Action action = data3.action;
			temp_target = ((action != null) ? action.target : null);
		}
		Opportunity data4 = this.Data;
		using (new Opportunity.TempActionArgs(a, temp_target, (data4 != null) ? data4.args : null))
		{
			flag = this.Data.action.CheckCost(this.Data.target);
			bool flag2 = flag;
			Opportunity data5 = this.Data;
			flag = (flag2 & ((data5 != null) ? data5.action.Validate(false) : null) == "ok");
		}
		this.activateButton.Enable(flag, false);
	}

	// Token: 0x06001E01 RID: 7681 RVA: 0x00118928 File Offset: 0x00116B28
	private void ExecutePlot(BSGButton b)
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.Data.action == null)
		{
			return;
		}
		this.Data.action.args = this.Data.args;
		if (!this.Data.action.CheckCost(this.Data.target))
		{
			return;
		}
		this.Data.action.target = this.Data.target;
		ActionVisuals.ExecuteAction(this.Data.action);
		this.Close();
	}

	// Token: 0x06001E02 RID: 7682 RVA: 0x001189B6 File Offset: 0x00116BB6
	private BSGButton FindActivateButton()
	{
		global::Common.FindChildByName(base.gameObject, "id_Buttons", true, true);
		return global::Common.FindChildComponent<BSGButton>(base.gameObject, "activate");
	}

	// Token: 0x06001E03 RID: 7683 RVA: 0x001189DC File Offset: 0x00116BDC
	private void Close()
	{
		MessageWnd component = base.GetComponent<MessageWnd>();
		if (component != null)
		{
			component.CloseAndDismiss(true);
		}
	}

	// Token: 0x06001E04 RID: 7684 RVA: 0x00118A00 File Offset: 0x00116C00
	private void OnDestroy()
	{
		if (this.activateButton != null)
		{
			this.activateButton.onClick = null;
			this.activateButton = null;
		}
	}

	// Token: 0x040013A4 RID: 5028
	public Vars Vars;

	// Token: 0x040013A5 RID: 5029
	public DT.Field field;

	// Token: 0x040013A6 RID: 5030
	private BSGButton activateButton;

	// Token: 0x040013A7 RID: 5031
	private bool m_Started;

	// Token: 0x040013A8 RID: 5032
	private bool m_DataExtracted;
}

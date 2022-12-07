using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200026D RID: 621
public abstract class UIPoliticalViewFilter : MonoBehaviour
{
	// Token: 0x06002624 RID: 9764
	protected abstract void SpawnFilterButtons();

	// Token: 0x06002625 RID: 9765
	public abstract void SelectFilter(PoliticalViewFilterIcon icon, PointerEventData e);

	// Token: 0x06002626 RID: 9766 RVA: 0x000DF539 File Offset: 0x000DD739
	private void OnEnable()
	{
		TooltipPlacement.AddBlocker(base.gameObject, null);
	}

	// Token: 0x06002627 RID: 9767 RVA: 0x000DF547 File Offset: 0x000DD747
	private void OnDisable()
	{
		TooltipPlacement.DelBlocker(base.gameObject);
	}

	// Token: 0x06002628 RID: 9768 RVA: 0x0014FC48 File Offset: 0x0014DE48
	public void Init(string politicalView)
	{
		this.politicalView = politicalView;
		this.SetDef();
		UICommon.FindComponents(this, false);
		this.SpawnFilterButtons();
		this.SetupFilterButtonsCallback();
		if (this.buttonShrink != null)
		{
			this.buttonShrink.onClick = new BSGButton.OnClick(this.HandleOnShrink);
		}
		if (this.buttonsExpand != null)
		{
			this.buttonsExpand.onClick = new BSGButton.OnClick(this.HandleOnExpand);
		}
		this.UpdateExpand();
	}

	// Token: 0x06002629 RID: 9769 RVA: 0x0014FCC8 File Offset: 0x0014DEC8
	protected virtual PoliticalViewFilterIcon[] SetupFilterButtonsCallback()
	{
		if (this.buttons == null)
		{
			return null;
		}
		PoliticalViewFilterIcon[] componentsInChildren = this.buttons.GetComponentsInChildren<PoliticalViewFilterIcon>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].OnSelected += this.SelectFilter;
		}
		return componentsInChildren;
	}

	// Token: 0x0600262A RID: 9770 RVA: 0x0014FD15 File Offset: 0x0014DF15
	protected void SetDef()
	{
		this.def = global::Defs.GetDefField(this.politicalView + "Screen", null);
	}

	// Token: 0x0600262B RID: 9771 RVA: 0x0014FD33 File Offset: 0x0014DF33
	private void HandleOnShrink(BSGButton b)
	{
		if (this.expanded)
		{
			this.expanded = false;
			this.UpdateExpand();
		}
	}

	// Token: 0x0600262C RID: 9772 RVA: 0x0014FD4A File Offset: 0x0014DF4A
	private void HandleOnExpand(BSGButton b)
	{
		if (!this.expanded)
		{
			this.expanded = true;
			this.UpdateExpand();
		}
	}

	// Token: 0x0600262D RID: 9773 RVA: 0x0014FD64 File Offset: 0x0014DF64
	private void UpdateExpand()
	{
		if (this.buttonShrink != null)
		{
			this.buttonShrink.gameObject.SetActive(this.expanded);
		}
		if (this.buttonsExpand != null)
		{
			this.buttonsExpand.gameObject.SetActive(!this.expanded);
		}
		if (this.shrinkGradient != null)
		{
			this.shrinkGradient.gameObject.SetActive(!this.expanded);
		}
		if (this.buttons != null)
		{
			LayoutElement component = this.buttons.GetComponent<LayoutElement>();
			if (component != null)
			{
				component.ignoreLayout = !this.expanded;
			}
		}
	}

	// Token: 0x040019CC RID: 6604
	public string politicalView;

	// Token: 0x040019CD RID: 6605
	protected DT.Field def;

	// Token: 0x040019CE RID: 6606
	protected List<PoliticalViewFilterIcon> selected = new List<PoliticalViewFilterIcon>();

	// Token: 0x040019CF RID: 6607
	protected bool expanded = true;

	// Token: 0x040019D0 RID: 6608
	[UIFieldTarget("id_Buttons")]
	protected GameObject buttons;

	// Token: 0x040019D1 RID: 6609
	[UIFieldTarget("id_ButtonShrink")]
	protected BSGButton buttonShrink;

	// Token: 0x040019D2 RID: 6610
	[UIFieldTarget("id_ButtonExpand")]
	protected BSGButton buttonsExpand;

	// Token: 0x040019D3 RID: 6611
	[UIFieldTarget("id_ShrinkGradient")]
	protected GameObject shrinkGradient;
}

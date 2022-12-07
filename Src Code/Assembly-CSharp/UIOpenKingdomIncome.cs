using System;
using System.Collections;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200022A RID: 554
public class UIOpenKingdomIncome : Hotspot, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x060021A4 RID: 8612 RVA: 0x001312F8 File Offset: 0x0012F4F8
	// (set) Token: 0x060021A5 RID: 8613 RVA: 0x00131300 File Offset: 0x0012F500
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x060021A6 RID: 8614 RVA: 0x00131309 File Offset: 0x0012F509
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
		yield break;
	}

	// Token: 0x060021A7 RID: 8615 RVA: 0x00131311 File Offset: 0x0012F511
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x060021A8 RID: 8616 RVA: 0x0013132C File Offset: 0x0012F52C
	public void SetKingdom(Logic.Kingdom k)
	{
		this.Init();
		this.Kingdom = k;
		if (this.vars == null)
		{
			this.vars = new Vars(this.Kingdom);
		}
		Tooltip.Get(base.gameObject, true).SetDef("KingdomGoldTooltip", this.vars);
	}

	// Token: 0x060021A9 RID: 8617 RVA: 0x00131380 File Offset: 0x0012F580
	private void UpdateText()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		if (this.vars != null && this.Kingdom != this.vars.obj)
		{
			this.vars.obj = this.Kingdom;
		}
		float num = this.Kingdom.resources[ResourceType.Gold];
		float num2 = this.Kingdom.income[ResourceType.Gold] - this.Kingdom.expenses[ResourceType.Gold];
		if (num == this.last_amount && num2 == this.last_income)
		{
			return;
		}
		this.last_amount = num;
		this.last_income = num2;
		this.vars.Set<float>("income", Mathf.Round(num2));
		this.vars.Set<float>("total", Mathf.Round(num));
		if (this.m_Income != null)
		{
			UIText.SetTextKey(this.m_Income, "KingdomResources.gold_income", this.vars, null);
		}
		if (this.m_Total != null)
		{
			UIText.SetTextKey(this.m_Total, "KingdomResources.gold_total", this.vars, null);
		}
		if (this.m_Full != null)
		{
			UIText.SetTextKey(this.m_Full, "KingdomResources.gold_full", this.vars, null);
		}
	}

	// Token: 0x060021AA RID: 8618 RVA: 0x001314C4 File Offset: 0x0012F6C4
	private void Update()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (this.Kingdom != kingdom)
		{
			if (kingdom != null && kingdom.IsValid() && kingdom.visuals != null)
			{
				this.SetKingdom(kingdom);
			}
			return;
		}
		if (this.m_Selected != null)
		{
			this.m_Selected.SetActive(this.currentWindow != null);
		}
		this.UpdateText();
	}

	// Token: 0x060021AB RID: 8619 RVA: 0x00131528 File Offset: 0x0012F728
	public void OnPointerClick(PointerEventData pointerEventData)
	{
		if (UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "add/remove resources", true))
		{
			if (pointerEventData.button == PointerEventData.InputButton.Left)
			{
				this.Kingdom.AddResources(KingdomAI.Expense.Category.Economy, ResourceType.Gold, 1000f, true);
			}
			if (pointerEventData.button == PointerEventData.InputButton.Right)
			{
				this.Kingdom.SubResources(KingdomAI.Expense.Category.Economy, ResourceType.Gold, 1000f, true);
				return;
			}
		}
		else if (pointerEventData.button == PointerEventData.InputButton.Left)
		{
			this.ToggleWindow();
		}
	}

	// Token: 0x060021AC RID: 8620 RVA: 0x00131596 File Offset: 0x0012F796
	public void OnCloseCallback(UIWindow _)
	{
		this.currentWindow = null;
	}

	// Token: 0x060021AD RID: 8621 RVA: 0x001315A0 File Offset: 0x0012F7A0
	public void ToggleWindow()
	{
		if (this.currentWindow != null)
		{
			this.currentWindow.Close(false);
			return;
		}
		GameObject gameObject = global::Common.FindChildByName(base.gameObject.transform.root.gameObject, this.HostRectName, true, true);
		if (gameObject == null)
		{
			return;
		}
		UICommon.DeleteChildren(gameObject.transform, typeof(UIRoyalFamily));
		this.currentWindow = UIKingdomIncomeWindow.Create(this.Kingdom, gameObject.transform as RectTransform, new UIWindow.OnClose(this.OnCloseCallback));
	}

	// Token: 0x04001696 RID: 5782
	public string HostRectName = "id_MessageContainer";

	// Token: 0x04001697 RID: 5783
	private UIKingdomIncomeWindow currentWindow;

	// Token: 0x04001698 RID: 5784
	[UIFieldTarget("id_Income")]
	private TextMeshProUGUI m_Income;

	// Token: 0x04001699 RID: 5785
	[UIFieldTarget("id_Total")]
	private TextMeshProUGUI m_Total;

	// Token: 0x0400169A RID: 5786
	[UIFieldTarget("id_Full")]
	private TextMeshProUGUI m_Full;

	// Token: 0x0400169B RID: 5787
	[UIFieldTarget("id_Selected")]
	private GameObject m_Selected;

	// Token: 0x0400169D RID: 5789
	private Vars vars;

	// Token: 0x0400169E RID: 5790
	private bool m_Initialzied;

	// Token: 0x0400169F RID: 5791
	private float last_amount = -1f;

	// Token: 0x040016A0 RID: 5792
	private float last_income = -1f;
}

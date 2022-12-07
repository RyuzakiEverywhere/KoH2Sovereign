using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000197 RID: 407
public class UIArmyIcon : ObjectIcon, IListener
{
	// Token: 0x1700012C RID: 300
	// (get) Token: 0x060016A6 RID: 5798 RVA: 0x000E26E9 File Offset: 0x000E08E9
	// (set) Token: 0x060016A7 RID: 5799 RVA: 0x000E26F1 File Offset: 0x000E08F1
	public Logic.Army Data { get; private set; }

	// Token: 0x1400000B RID: 11
	// (add) Token: 0x060016A8 RID: 5800 RVA: 0x000E26FC File Offset: 0x000E08FC
	// (remove) Token: 0x060016A9 RID: 5801 RVA: 0x000E2734 File Offset: 0x000E0934
	public event Action<UIArmyIcon> OnSelect;

	// Token: 0x060016AA RID: 5802 RVA: 0x000E2769 File Offset: 0x000E0969
	private void OnDestroy()
	{
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		this.OnSelect = null;
	}

	// Token: 0x060016AB RID: 5803 RVA: 0x000E2788 File Offset: 0x000E0988
	public override void SetObject(object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		if (this.logicObject != null)
		{
			Tooltip.Get(base.gameObject, true).SetObj(obj, null, null);
			if (obj is Logic.Army)
			{
				this.Data = (obj as Logic.Army);
				this.Data.AddListener(this);
			}
		}
		else
		{
			this.Data = null;
		}
		UICommon.FindComponents(this, false);
		this.Refresh();
	}

	// Token: 0x060016AC RID: 5804 RVA: 0x000E27F0 File Offset: 0x000E09F0
	private void Refresh()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_Crest != null)
		{
			Logic.Kingdom kingdom = this.Data.GetKingdom();
			Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
			bool flag = kingdom != kingdom2;
			if (flag)
			{
				this.m_Crest.SetObject(this.Data, null);
			}
			this.m_Crest.gameObject.SetActive(flag);
		}
		this.UpdateHighlight();
	}

	// Token: 0x060016AD RID: 5805 RVA: 0x000E2858 File Offset: 0x000E0A58
	public void Select(bool selected)
	{
		if (this.m_Selected == selected)
		{
			return;
		}
		this.m_Selected = selected;
		this.UpdateHighlight();
	}

	// Token: 0x060016AE RID: 5806 RVA: 0x000E2871 File Offset: 0x000E0A71
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (this.OnSelect != null)
		{
			this.OnSelect(this);
			return;
		}
		if (this.Data == null)
		{
			return;
		}
		if (e.button == PointerEventData.InputButton.Left)
		{
			this.SelectArmy(e.clickCount > 1);
		}
	}

	// Token: 0x060016AF RID: 5807 RVA: 0x000E28B0 File Offset: 0x000E0AB0
	private void SelectArmy(bool focus = false)
	{
		if (this.Data == null)
		{
			return;
		}
		global::Army army = this.Data.visuals as global::Army;
		if (army == null)
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		worldUI.SelectObj(army.gameObject, false, true, true, true);
		if (focus)
		{
			worldUI.LookAt(army.gameObject.transform.position, false);
		}
	}

	// Token: 0x060016B0 RID: 5808 RVA: 0x000E291B File Offset: 0x000E0B1B
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x060016B1 RID: 5809 RVA: 0x000E292A File Offset: 0x000E0B2A
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x060016B2 RID: 5810 RVA: 0x000E293C File Offset: 0x000E0B3C
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.m_Background != null)
		{
			Color color;
			if (this.m_Selected)
			{
				if (this.mouse_in)
				{
					color = this.selected;
				}
				else
				{
					color = this.selected;
				}
			}
			else if (this.mouse_in)
			{
				color = this.over;
			}
			else
			{
				color = this.normal;
			}
			this.m_Background.color = color;
		}
	}

	// Token: 0x060016B3 RID: 5811 RVA: 0x000E29A5 File Offset: 0x000E0BA5
	void IListener.OnMessage(object obj, string message, object param)
	{
		this.Refresh();
	}

	// Token: 0x04000EA8 RID: 3752
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x04000EA9 RID: 3753
	[UIFieldTarget("id_Border")]
	private Image m_Border;

	// Token: 0x04000EAA RID: 3754
	[UIFieldTarget("id_Background")]
	private Image m_Background;

	// Token: 0x04000EAB RID: 3755
	[UIFieldTarget("id_Crest")]
	private UIKingdomIcon m_Crest;

	// Token: 0x04000EAC RID: 3756
	[SerializeField]
	private Color normal = new Color32(92, 92, 92, byte.MaxValue);

	// Token: 0x04000EAD RID: 3757
	[SerializeField]
	private Color over = new Color32(92, 92, 92, byte.MaxValue);

	// Token: 0x04000EAE RID: 3758
	[SerializeField]
	private Color selected = new Color32(92, 92, 92, byte.MaxValue);

	// Token: 0x04000EB1 RID: 3761
	private bool m_Selected;
}

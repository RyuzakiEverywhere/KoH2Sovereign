using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000269 RID: 617
public class PoliticalViewFilterIcon : Hotspot
{
	// Token: 0x170001C5 RID: 453
	// (get) Token: 0x06002605 RID: 9733 RVA: 0x0014F3F0 File Offset: 0x0014D5F0
	// (set) Token: 0x06002606 RID: 9734 RVA: 0x0014F3F8 File Offset: 0x0014D5F8
	public DT.Field ButtonDef { get; set; }

	// Token: 0x170001C6 RID: 454
	// (get) Token: 0x06002607 RID: 9735 RVA: 0x0014F401 File Offset: 0x0014D601
	// (set) Token: 0x06002608 RID: 9736 RVA: 0x0014F409 File Offset: 0x0014D609
	public Def DataDef { get; set; }

	// Token: 0x14000031 RID: 49
	// (add) Token: 0x06002609 RID: 9737 RVA: 0x0014F414 File Offset: 0x0014D614
	// (remove) Token: 0x0600260A RID: 9738 RVA: 0x0014F44C File Offset: 0x0014D64C
	public event Action<PoliticalViewFilterIcon, PointerEventData> OnSelected;

	// Token: 0x0600260B RID: 9739 RVA: 0x0014F481 File Offset: 0x0014D681
	public override void Awake()
	{
		base.Awake();
		UICommon.FindComponents(this, false);
	}

	// Token: 0x0600260C RID: 9740 RVA: 0x0014F490 File Offset: 0x0014D690
	private void Start()
	{
		if (this.m_Selected != null)
		{
			this.m_Selected.gameObject.SetActive(this.active);
		}
		this.SetIcon();
	}

	// Token: 0x0600260D RID: 9741 RVA: 0x0014F4BC File Offset: 0x0014D6BC
	public override void OnClick(PointerEventData e)
	{
		if (this.OnSelected != null)
		{
			this.OnSelected(this, e);
		}
	}

	// Token: 0x0600260E RID: 9742 RVA: 0x0014F4D3 File Offset: 0x0014D6D3
	public void SetActive(bool a)
	{
		this.active = a;
		this.ChangeState(a);
	}

	// Token: 0x0600260F RID: 9743 RVA: 0x0014F4E3 File Offset: 0x0014D6E3
	public void ChangeState(bool active)
	{
		if (this.m_Selected != null)
		{
			this.m_Selected.gameObject.SetActive(active);
		}
	}

	// Token: 0x06002610 RID: 9744 RVA: 0x0014F504 File Offset: 0x0014D704
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		if (this.ButtonDef != null && this.DataDef != null)
		{
			Vars vars = new Vars(this.DataDef);
			string txt = "";
			string title = "";
			DT.Field field = this.DataDef.field.FindChild(this.ButtonDef.GetString("buttons.tooltip.text", null, "", true, true, true, '.'), null, true, true, true, '.');
			if (field != null)
			{
				txt = "#" + global::Defs.Localize(field, vars, null, true, true);
			}
			DT.Field field2 = this.DataDef.field.FindChild(this.ButtonDef.GetString("buttons.tooltip.caption", null, "", true, true, true, '.'), null, true, true, true, '.');
			if (field2 != null)
			{
				title = "#" + global::Defs.Localize(field2, vars, null, true, true);
			}
			Tooltip.Get(base.gameObject, true).SetText(txt, title, vars);
		}
	}

	// Token: 0x06002611 RID: 9745 RVA: 0x0014F5F8 File Offset: 0x0014D7F8
	public void SetIcon()
	{
		if (this.m_Icon == null)
		{
			return;
		}
		if (this.image != null)
		{
			this.m_Icon.sprite = this.image;
			return;
		}
		if (this.DataDef != null && this.ButtonDef != null)
		{
			this.m_Icon.sprite = global::Defs.GetObj<Sprite>(this.DataDef.field, this.ButtonDef.GetString("buttons.button_icon", null, "", true, true, true, '.'), null);
		}
	}

	// Token: 0x06002612 RID: 9746 RVA: 0x0014F67B File Offset: 0x0014D87B
	public void SetIcon(Sprite sprite)
	{
		this.image = sprite;
	}

	// Token: 0x040019C0 RID: 6592
	[UIFieldTarget("id_Icon")]
	protected Image m_Icon;

	// Token: 0x040019C1 RID: 6593
	[UIFieldTarget("id_Selected")]
	protected Image m_Selected;

	// Token: 0x040019C2 RID: 6594
	[UIFieldTarget("id_Available")]
	protected Image m_Available;

	// Token: 0x040019C3 RID: 6595
	[UIFieldTarget("id_Background")]
	protected Image m_Background;

	// Token: 0x040019C4 RID: 6596
	public bool active;

	// Token: 0x040019C5 RID: 6597
	[HideInInspector]
	public Sprite image;
}

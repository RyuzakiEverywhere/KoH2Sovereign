using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200021B RID: 539
public class UIReligionIcon : ObjectIcon
{
	// Token: 0x060020A2 RID: 8354 RVA: 0x00129F9C File Offset: 0x0012819C
	private void Init()
	{
		if (this.m_Initiazled)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		BSGButton component = base.GetComponent<BSGButton>();
		if (component != null)
		{
			component.onClick = new BSGButton.OnClick(this.HandleOnClick);
		}
		this.m_Vars = new Vars();
		this.m_Initiazled = true;
	}

	// Token: 0x060020A3 RID: 8355 RVA: 0x00129FF0 File Offset: 0x001281F0
	public override void SetObject(object obj, Vars vars)
	{
		this.Init();
		base.SetObject(obj, vars);
		this.Religion = (obj as Religion);
		this.m_Vars.obj = this.Religion;
		Tooltip.Get(base.gameObject, true).SetDef("ReligionTooltip", this.m_Vars);
	}

	// Token: 0x060020A4 RID: 8356 RVA: 0x000023FD File Offset: 0x000005FD
	private void HandleOnClick(BSGButton b)
	{
	}

	// Token: 0x060020A5 RID: 8357 RVA: 0x0012A04C File Offset: 0x0012824C
	private void Refresh()
	{
		if (this.m_Icon == null)
		{
			return;
		}
		Image icon = this.m_Icon;
		Religion religion = this.Religion;
		DT.Field field;
		if (religion == null)
		{
			field = null;
		}
		else
		{
			Religion.Def def = religion.def;
			field = ((def != null) ? def.field : null);
		}
		icon.overrideSprite = global::Defs.GetObj<Sprite>(field, "icon", null);
	}

	// Token: 0x060020A6 RID: 8358 RVA: 0x0012A09C File Offset: 0x0012829C
	public static UIReligionIcon Create(Logic.Object obj, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (obj == null)
		{
			return null;
		}
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UIReligionIcon orAddComponent = global::Common.Spawn(prototype, parent, false, "").GetOrAddComponent<UIReligionIcon>();
		orAddComponent.SetObject(obj, vars);
		return orAddComponent;
	}

	// Token: 0x040015BD RID: 5565
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x040015BE RID: 5566
	private Religion Religion;

	// Token: 0x040015BF RID: 5567
	private Vars m_Vars;

	// Token: 0x040015C0 RID: 5568
	private bool m_Initiazled;
}

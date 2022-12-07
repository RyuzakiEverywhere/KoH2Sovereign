using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002FD RID: 765
public class FieldIcon : Hotspot
{
	// Token: 0x06003005 RID: 12293 RVA: 0x0018786D File Offset: 0x00185A6D
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x06003006 RID: 12294 RVA: 0x00187886 File Offset: 0x00185A86
	public virtual void SetObject(DT.Field f, Vars vars = null)
	{
		this.Init();
		this.field = f;
		if (vars == null)
		{
			vars = new Vars(f);
		}
		this.vars = vars;
		this.Populate();
	}

	// Token: 0x06003007 RID: 12295 RVA: 0x001878AD File Offset: 0x00185AAD
	private void Populate()
	{
		if (this.m_Icon != null)
		{
			this.m_Icon.overrideSprite = global::Defs.GetObj<Sprite>(this.field, "icon", null);
		}
	}

	// Token: 0x06003008 RID: 12296 RVA: 0x0016EC61 File Offset: 0x0016CE61
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
	}

	// Token: 0x06003009 RID: 12297 RVA: 0x001878D9 File Offset: 0x00185AD9
	public override Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		if (key == "obj")
		{
			return new Value(this.field);
		}
		if (!(key == "vars"))
		{
			return base.GetVar(key, vars, as_value);
		}
		return new Value(vars);
	}

	// Token: 0x0400205C RID: 8284
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x0400205D RID: 8285
	public DT.Field field;

	// Token: 0x0400205E RID: 8286
	public Vars vars;

	// Token: 0x0400205F RID: 8287
	private bool m_Initialzied;
}

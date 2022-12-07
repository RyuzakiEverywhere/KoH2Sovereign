using System;
using Logic;
using UnityEngine;

// Token: 0x020000E4 RID: 228
public class TitleView : ViewMode
{
	// Token: 0x06000B5E RID: 2910 RVA: 0x00080D16 File Offset: 0x0007EF16
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.color_none = global::Defs.GetColor(this.def_id, "colors.None", Color.clear);
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x00080D3C File Offset: 0x0007EF3C
	private void Init()
	{
		if (this.politicalView == null)
		{
			TitleMap titleMap = TitleMap.Get();
			if (titleMap != null)
			{
				this.politicalView = titleMap.PoliticalMapObject;
				if (this.politicalView)
				{
					this.tt = Tooltip.Get(this.politicalView, true);
				}
			}
		}
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x00080D91 File Offset: 0x0007EF91
	protected override void OnActivate()
	{
		base.OnActivate();
		this.Init();
		if (this.politicalView != null)
		{
			this.politicalView.SetActive(true);
			this.tt.handler = new Tooltip.Handler(this.HandleTooltip);
		}
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x00080DD4 File Offset: 0x0007EFD4
	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		if (this.politicalView != null)
		{
			this.politicalView.SetActive(false);
			if (this.tt != null)
			{
				this.tt.Clear(false);
				this.tt.handler = null;
			}
		}
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x00080E27 File Offset: 0x0007F027
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		this.Init();
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x00080E38 File Offset: 0x0007F038
	public override void SetShaderGlobals(bool secondary)
	{
		base.SetShaderGlobals(secondary);
		TitleMap titleMap = TitleMap.Get();
		if (titleMap != null)
		{
			Shader.SetGlobalTexture("_RealmsData", titleMap.RealmsDataTexture);
		}
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x00080D0C File Offset: 0x0007EF0C
	public TitleView() : base(null, null)
	{
	}

	// Token: 0x040008C4 RID: 2244
	protected GameObject politicalView;

	// Token: 0x040008C5 RID: 2245
	protected Color color_none;
}

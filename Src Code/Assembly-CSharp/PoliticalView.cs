using System;
using Logic;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public class PoliticalView : ViewMode
{
	// Token: 0x06000B56 RID: 2902 RVA: 0x00080A1C File Offset: 0x0007EC1C
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		if (field != null)
		{
			this.allowedFigures = ViewMode.AllowedFigures.None;
			if (field.GetBool("allowed_figures_army", null, false, true, true, true, '.'))
			{
				this.allowedFigures |= ViewMode.AllowedFigures.Army;
			}
			if (field.GetBool("allowed_figures_mercenary", null, false, true, true, true, '.'))
			{
				this.allowedFigures |= ViewMode.AllowedFigures.Mercenary;
			}
			if (field.GetBool("allowed_figures_town", null, false, true, true, true, '.'))
			{
				this.allowedFigures |= ViewMode.AllowedFigures.Castle;
			}
			if (field.GetBool("allowed_figures_battle", null, false, true, true, true, '.'))
			{
				this.allowedFigures |= ViewMode.AllowedFigures.Battle;
			}
		}
		this.color_none = global::Defs.GetColor(this.def_id, "colors.None", Color.clear);
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x00080AE0 File Offset: 0x0007ECE0
	private void Init()
	{
		if (this.politicalView == null)
		{
			WorldMap worldMap = WorldMap.Get();
			if (worldMap != null)
			{
				this.politicalView = worldMap.PoliticalMapObject;
				this.tt = Tooltip.Get(this.politicalView, true);
				this.snapshot = new FMODWrapper.Snapshot("political_view_snapshot");
			}
		}
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x00080B38 File Offset: 0x0007ED38
	protected override void OnActivate()
	{
		base.OnActivate();
		this.Init();
		Vars vars = new Vars();
		vars.SetVar("view", "political_view");
		DT.Field soundsDef = BaseUI.soundsDef;
		BaseUI.PlayVoiceEvent((soundsDef != null) ? soundsDef.GetString("narrator_political_view", null, "", true, true, true, '.') : null, vars);
		FMODWrapper.Snapshot snapshot = this.snapshot;
		if (snapshot != null)
		{
			snapshot.StartSnapshot();
		}
		if (this.politicalView != null)
		{
			this.politicalView.SetActive(true);
			this.tt.handler = new Tooltip.Handler(this.HandleTooltip);
		}
		while (LabelUpdater.IsProcessing())
		{
			LabelUpdater.Get(true).Update();
		}
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x00080BEC File Offset: 0x0007EDEC
	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		FMODWrapper.Snapshot snapshot = this.snapshot;
		if (snapshot != null)
		{
			snapshot.EndSnapshot();
		}
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

	// Token: 0x06000B5A RID: 2906 RVA: 0x00080C50 File Offset: 0x0007EE50
	public void ToggleSnapshot(bool bv_active)
	{
		if (bv_active)
		{
			FMODWrapper.Snapshot snapshot = this.snapshot;
			if (snapshot == null)
			{
				return;
			}
			snapshot.EndSnapshot();
			return;
		}
		else
		{
			FMODWrapper.Snapshot snapshot2 = this.snapshot;
			if (snapshot2 == null)
			{
				return;
			}
			snapshot2.StartSnapshot();
			return;
		}
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x00080C76 File Offset: 0x0007EE76
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		this.Init();
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x00080C88 File Offset: 0x0007EE88
	protected Logic.Realm GetSelecteRelam()
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return null;
		}
		if (worldUI.selected_logic_obj is Castle)
		{
			return (worldUI.selected_logic_obj as Castle).GetRealm();
		}
		if (worldUI.selected_logic_obj is Logic.Realm)
		{
			return worldUI.selected_logic_obj as Logic.Realm;
		}
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null && worldMap.selected_realm != 0)
		{
			Logic.Realm realm = GameLogic.Get(true).GetRealm(worldMap.selected_realm);
			if (realm != null)
			{
				return realm;
			}
		}
		return null;
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x00080D0C File Offset: 0x0007EF0C
	public PoliticalView() : base(null, null)
	{
	}

	// Token: 0x040008C1 RID: 2241
	protected GameObject politicalView;

	// Token: 0x040008C2 RID: 2242
	protected Color color_none;

	// Token: 0x040008C3 RID: 2243
	private FMODWrapper.Snapshot snapshot;
}

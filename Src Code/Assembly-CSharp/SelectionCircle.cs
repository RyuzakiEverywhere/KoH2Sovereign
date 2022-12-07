using System;
using System.Runtime.CompilerServices;
using Logic;
using UnityEngine;

// Token: 0x020000CA RID: 202
public class SelectionCircle : MonoBehaviour
{
	// Token: 0x060008FE RID: 2302 RVA: 0x00061E5B File Offset: 0x0006005B
	public void SetObject(MapObject obj)
	{
		this.logic = obj;
		if (this.logic != null)
		{
			this.movement = this.logic.GetComponent<Movement>();
			return;
		}
		this.movement = null;
	}

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x060008FF RID: 2303 RVA: 0x00061E85 File Offset: 0x00060085
	private BattleViewUI ui
	{
		get
		{
			if (this._ui == null)
			{
				this._ui = BattleViewUI.Get();
			}
			return this._ui;
		}
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x00061EA6 File Offset: 0x000600A6
	public void SetSelected(bool bSelected, bool bPrimaryselection = true)
	{
		this.selected = bSelected;
		this.primarySelection = bPrimaryselection;
		if (this.selected)
		{
			this.CreateSelection();
			return;
		}
		this.DestroySelection();
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x00061ECB File Offset: 0x000600CB
	public void RecreateSelectionUI()
	{
		if (this.ui != null)
		{
			this.ui.RefreshSelection(base.gameObject);
		}
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x00061EEC File Offset: 0x000600EC
	private void CreateRelationMarker()
	{
		if (this.relation != null)
		{
			return;
		}
		if (this.logic == null || !this.logic.started)
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null || !baseUI.SelectionShown())
		{
			return;
		}
		this.relation = MeshUtils.CreateRelectionDisc(base.gameObject, 4.2f, baseUI.GetSelectionMaterial(base.gameObject, null, this.primarySelection, true));
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00061F60 File Offset: 0x00060160
	private void DestroyRelationMarker()
	{
		if (this.relation != null)
		{
			UnityEngine.Object.Destroy(this.relation.sharedMaterial);
			MeshFilter component = this.relation.GetComponent<MeshFilter>();
			UnityEngine.Object.Destroy((component != null) ? component.sharedMesh : null);
			UnityEngine.Object.Destroy(this.relation.gameObject);
			this.relation = null;
		}
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00061FC0 File Offset: 0x000601C0
	public void CreateSelection()
	{
		if (this.selection != null)
		{
			return;
		}
		if (this.logic == null || !this.logic.started)
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null || !baseUI.SelectionShown())
		{
			return;
		}
		this.selection = MeshUtils.CreateSelectionCircle(base.gameObject, 4f, baseUI.GetSelectionMaterial(base.gameObject, null, this.primarySelection, false), 0.25f);
		MeshUtils.SnapSelectionToTerrain(this.selection, WorldMap.GetTerrain());
		this.ShowSelectionGlow(true);
		this.CreateRelationMarker();
		this.CreatePathArrows();
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x0006205C File Offset: 0x0006025C
	public void DestroySelection()
	{
		this.DestroyPathArrows();
		this.DestroyRelationMarker();
		if (this.selection == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(this.selection.sharedMaterial);
		MeshFilter component = this.selection.GetComponent<MeshFilter>();
		UnityEngine.Object.Destroy((component != null) ? component.sharedMesh : null);
		UnityEngine.Object.Destroy(this.selection.gameObject);
		this.selection = null;
		this.ShowSelectionGlow(false);
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x000620D0 File Offset: 0x000602D0
	public void UpdateSelectionColors()
	{
		if (this.selection != null)
		{
			this.selection.material = this.ui.GetSelectionMaterial(base.gameObject, this.selection.material, this.primarySelection, false);
		}
		if (this.relation != null)
		{
			this.relation.material = this.ui.GetSelectionMaterial(base.gameObject, this.relation.material, this.primarySelection, false);
		}
		if (this.banner != null)
		{
			BaseUI baseUI = BaseUI.Get();
			if (baseUI == null || !baseUI.SelectionShown())
			{
				return;
			}
			if (global::Common.FindChildByName(this.banner.gameObject, "id_BannerSelection", true, true) == null)
			{
				return;
			}
			Color stanceColor = baseUI.GetStanceColor(this.logic, true);
			SelectionCircle.<UpdateSelectionColors>g__UpdateParticleColor|19_0(this.banner.gameObject, "id_EdgeGlows", stanceColor);
			SelectionCircle.<UpdateSelectionColors>g__UpdateParticleColor|19_0(this.banner.gameObject, "id_SmallDust", stanceColor);
			SelectionCircle.<UpdateSelectionColors>g__UpdateParticleColor|19_0(this.banner.gameObject, "id_GodRays", stanceColor);
			SelectionCircle.<UpdateSelectionColors>g__UpdateParticleColor|19_0(this.banner.gameObject, "id_FlagGlowBig", stanceColor);
			SelectionCircle.<UpdateSelectionColors>g__UpdateParticleColor|19_0(this.banner.gameObject, "id_GodRaysCenter", stanceColor);
		}
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x0006221C File Offset: 0x0006041C
	private void ShowSelectionGlow(bool shown)
	{
		if (this.banner == null)
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null || !baseUI.SelectionShown())
		{
			return;
		}
		GameObject gameObject = global::Common.FindChildByName(this.banner.gameObject, "id_BannerSelection", true, true);
		if (gameObject == null)
		{
			return;
		}
		if (shown)
		{
			this.UpdateSelectionColors();
		}
		gameObject.gameObject.SetActive(shown);
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x00062288 File Offset: 0x00060488
	public void UpdateSelection()
	{
		if (this.selection != null)
		{
			this.selection.material = this.ui.GetSelectionMaterial(base.gameObject, this.selection.material, this.primarySelection, false);
			MeshUtils.SnapSelectionToTerrain(this.selection, WorldMap.GetTerrain());
		}
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x000622E1 File Offset: 0x000604E1
	private void DestroyPathArrows()
	{
		if (this.path_arrows == null)
		{
			return;
		}
		global::Common.DestroyObj(this.path_arrows.gameObject);
		this.path_arrows = null;
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x0006230C File Offset: 0x0006050C
	public void CreatePathArrows()
	{
		if (this.movement == null)
		{
			return;
		}
		this.DestroyPathArrows();
		if (!this.ui.PathArrowsShown())
		{
			return;
		}
		Logic.Kingdom kingdom = this.logic.GetKingdom();
		if (kingdom != null && kingdom.is_player && kingdom != BaseUI.LogicKingdom() && !BaseUI.CanControlAI())
		{
			return;
		}
		Path path = this.movement.path;
		if (path == null || path.IsDone())
		{
			return;
		}
		this.path_arrows = PathArrows.Create(this.movement, 3, false);
		this.path_arrows.gameObject.layer = base.gameObject.layer;
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x000623A3 File Offset: 0x000605A3
	private void OnDestroy()
	{
		this.DestroyPathArrows();
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x000623AC File Offset: 0x000605AC
	[CompilerGenerated]
	internal static void <UpdateSelectionColors>g__UpdateParticleColor|19_0(GameObject host, string childId, Color tintColor)
	{
		ParticleSystem particleSystem = global::Common.FindChildComponent<ParticleSystem>(host, childId);
		ParticleSystem.MainModule main = particleSystem.main;
		Color color = tintColor;
		color.a = main.startColor.colorMax.a;
		main.startColor = color;
		particleSystem.Stop();
		particleSystem.Clear();
		main.prewarm = true;
		particleSystem.Play();
	}

	// Token: 0x0400071D RID: 1821
	public MapObject logic;

	// Token: 0x0400071E RID: 1822
	private Movement movement;

	// Token: 0x0400071F RID: 1823
	public Billboard banner;

	// Token: 0x04000720 RID: 1824
	private PathArrows path_arrows;

	// Token: 0x04000721 RID: 1825
	private bool selected;

	// Token: 0x04000722 RID: 1826
	private bool primarySelection;

	// Token: 0x04000723 RID: 1827
	private MeshRenderer selection;

	// Token: 0x04000724 RID: 1828
	private MeshRenderer relation;

	// Token: 0x04000725 RID: 1829
	private BattleViewUI _ui;

	// Token: 0x04000726 RID: 1830
	public const float Radius = 4f;
}

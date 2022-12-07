using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x0200016A RID: 362
[ExecuteInEditMode]
public class HexGrid : MonoBehaviour
{
	// Token: 0x0600126B RID: 4715 RVA: 0x000C0F77 File Offset: 0x000BF177
	public static global::HexGrid Get()
	{
		return global::HexGrid.instance;
	}

	// Token: 0x0600126C RID: 4716 RVA: 0x000C0F7E File Offset: 0x000BF17E
	public static Logic.HexGrid Grid()
	{
		if (global::HexGrid.instance == null)
		{
			return null;
		}
		return global::HexGrid.instance.grid;
	}

	// Token: 0x0600126D RID: 4717 RVA: 0x000C0F9C File Offset: 0x000BF19C
	public static Vector3 Center(Logic.HexGrid.Coord tile, float y = 0f)
	{
		if (global::HexGrid.instance == null)
		{
			return Vector3.zero;
		}
		Vector3 result = global::HexGrid.instance.grid.Center(tile);
		result.y = y;
		return result;
	}

	// Token: 0x0600126E RID: 4718 RVA: 0x000C0FDB File Offset: 0x000BF1DB
	public static Vector3 Snap(Vector3 pt)
	{
		if (global::HexGrid.instance == null)
		{
			return pt;
		}
		return global::HexGrid.Center(global::HexGrid.instance.grid.WorldToGrid(pt), pt.y);
	}

	// Token: 0x0600126F RID: 4719 RVA: 0x000C100C File Offset: 0x000BF20C
	public static float Radius()
	{
		if (global::HexGrid.instance == null)
		{
			return 0f;
		}
		return global::HexGrid.instance.grid.tile_radius;
	}

	// Token: 0x06001270 RID: 4720 RVA: 0x000C1030 File Offset: 0x000BF230
	private void OnEnable()
	{
		global::HexGrid.instance = this;
		this.SetProperties();
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x000C103E File Offset: 0x000BF23E
	private void OnDisable()
	{
		if (global::HexGrid.instance == this)
		{
			global::HexGrid.instance = null;
		}
	}

	// Token: 0x06001272 RID: 4722 RVA: 0x000C1054 File Offset: 0x000BF254
	public void SetProperties()
	{
		this.terrain = Terrain.activeTerrain;
		if (this.terrain != null)
		{
			Vector3 size = this.terrain.terrainData.size;
			this.rows = size.z / (this.grid.tile_radius * 0.8660254f * 2f) * 2f;
			this.cols = size.x / this.grid.tile_radius * 2f;
			Shader.SetGlobalFloat("_Thickness", this.edge_width);
			Shader.SetGlobalFloat("_NumHexX", this.cols);
			Shader.SetGlobalFloat("_NumHexY", this.rows);
			Shader.SetGlobalTexture("_EdgeTexture", this.tex);
		}
	}

	// Token: 0x06001273 RID: 4723 RVA: 0x000C1118 File Offset: 0x000BF318
	public void GenerateTexture(bool createNewTexture = false)
	{
		int num = Mathf.CeilToInt(this.cols);
		int num2 = Mathf.CeilToInt(this.rows);
		if (createNewTexture || this.tex == null)
		{
			this.tex = new Texture2D(num, num2, TextureFormat.ARGB32, false);
		}
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				this.tex.SetPixel(i, j, Color.white);
			}
		}
		global::HexGroups hexGroups = global::HexGroups.Get();
		if (hexGroups != null)
		{
			Logic.HexGroups groups = hexGroups.groups;
			if (groups != null)
			{
				for (int k = 0; k < groups.Count; k++)
				{
					List<Point> edges = groups[k].GetEdges();
					for (int l = 0; l < edges.Count; l++)
					{
						this.tex.SetPixel(Mathf.CeilToInt(edges[l].x), Mathf.CeilToInt(edges[l].y), Color.clear);
					}
				}
			}
		}
		this.tex.filterMode = FilterMode.Point;
		this.tex.wrapMode = TextureWrapMode.Clamp;
		this.tex.Apply();
		Shader.SetGlobalTexture("_EdgeTexture", this.tex);
	}

	// Token: 0x04000C61 RID: 3169
	public Logic.HexGrid grid = new Logic.HexGrid(50f);

	// Token: 0x04000C62 RID: 3170
	public float edge_width = 2f;

	// Token: 0x04000C63 RID: 3171
	public float edge_fade = 8f;

	// Token: 0x04000C64 RID: 3172
	public Texture2D tex;

	// Token: 0x04000C65 RID: 3173
	public Terrain terrain;

	// Token: 0x04000C66 RID: 3174
	public float rows;

	// Token: 0x04000C67 RID: 3175
	public float cols;

	// Token: 0x04000C68 RID: 3176
	private static global::HexGrid instance;
}

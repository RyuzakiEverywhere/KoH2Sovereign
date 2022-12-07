using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x0200016C RID: 364
[ExecuteInEditMode]
[RequireComponent(typeof(global::HexGroups))]
public class HexSelection : MonoBehaviour
{
	// Token: 0x06001285 RID: 4741 RVA: 0x000C1847 File Offset: 0x000BFA47
	private void OnEnable()
	{
		this.LoadDefs();
		this.pos = base.transform.position;
		this.hasRotation = (UnityEngine.Object.FindObjectOfType<BattleTemplate>() != null);
		this.canPlace = this.CanPlace();
	}

	// Token: 0x06001286 RID: 4742 RVA: 0x000C1880 File Offset: 0x000BFA80
	public void Copy()
	{
		TerrainData terrainData = this.GetTerrainData();
		if (terrainData == null)
		{
			return;
		}
		this.heights = this.CopyHeights(terrainData);
		this.splats = this.CopySplats(terrainData);
	}

	// Token: 0x06001287 RID: 4743 RVA: 0x000C18B8 File Offset: 0x000BFAB8
	public void Remember()
	{
		TerrainData terrainData = this.GetTerrainData();
		if (terrainData == null)
		{
			return;
		}
		this.bg_heights = this.CopyHeights(terrainData);
		this.bg_splats = this.CopySplats(terrainData);
	}

	// Token: 0x06001288 RID: 4744 RVA: 0x000C18F0 File Offset: 0x000BFAF0
	private void Paste(BSGTerrainTools.Float2D heights, BSGTerrainTools.Float3D splats)
	{
		if (heights == null && splats == null)
		{
			return;
		}
		TerrainData terrainData = this.GetTerrainData();
		if (terrainData == null)
		{
			return;
		}
		Bounds bounds = this.CalcBounds();
		if (heights != null)
		{
			Bounds bounds2 = BSGTerrainTools.SnapHeightsBounds(bounds, terrainData);
			heights.SetWorldRect(bounds2.min.x, bounds2.min.z, bounds2.size.x, bounds2.size.z);
		}
		if (splats != null)
		{
			Bounds bounds3 = BSGTerrainTools.SnapSplatsBounds(bounds, terrainData);
			splats.SetWorldRect(bounds3.min.x, bounds3.min.z, bounds3.size.x, bounds3.size.z);
		}
		BSGTerrainTools.TerrainBlock terrainBlock = new BSGTerrainTools.TerrainBlock(terrainData, bounds);
		terrainBlock.Import(heights != null, splats != null, false);
		BSGTerrainTools.Gen2D gen2D = new BSGTerrainTools.Gen2D(1f);
		gen2D.func = new BSGTerrainTools.Gen2D.Func(this.CalcAlpha);
		if (heights != null)
		{
			BSGTerrainTools.ModifyHeights(terrainBlock, bounds, new BSGTerrainTools.Gen2D(heights), gen2D);
		}
		if (splats != null)
		{
			BSGTerrainTools.ModifySplats(terrainBlock, bounds, new BSGTerrainTools.Gen3D(splats), gen2D);
		}
		terrainBlock.Apply(heights != null, splats != null, true);
	}

	// Token: 0x06001289 RID: 4745 RVA: 0x000C1A08 File Offset: 0x000BFC08
	public void Paste()
	{
		global::HexGrid hexGrid = global::HexGrid.Get();
		if (hexGrid == null)
		{
			return;
		}
		this.edge_width = hexGrid.edge_width;
		this.edge_fade = hexGrid.edge_fade;
		this.Paste(this.heights, this.splats);
	}

	// Token: 0x0600128A RID: 4746 RVA: 0x000023FD File Offset: 0x000005FD
	public void Apply()
	{
	}

	// Token: 0x0600128B RID: 4747 RVA: 0x000C1A50 File Offset: 0x000BFC50
	public void Erase()
	{
		global::HexGrid hexGrid = global::HexGrid.Get();
		if (hexGrid == null)
		{
			return;
		}
		this.edge_width = hexGrid.edge_width;
		this.edge_fade = 0f;
		this.Paste(this.bg_heights, this.bg_splats);
	}

	// Token: 0x0600128C RID: 4748 RVA: 0x000C1A98 File Offset: 0x000BFC98
	private TerrainData GetTerrainData()
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null)
		{
			return null;
		}
		TerrainData terrainData = activeTerrain.terrainData;
		if (terrainData == null)
		{
			return null;
		}
		return terrainData;
	}

	// Token: 0x0600128D RID: 4749 RVA: 0x000C1ACC File Offset: 0x000BFCCC
	public Logic.HexGrid.Coord Tile()
	{
		Logic.HexGrid hexGrid = global::HexGrid.Grid();
		if (hexGrid == null)
		{
			return Logic.HexGrid.Coord.Invalid;
		}
		return hexGrid.WorldToGrid(base.transform.position);
	}

	// Token: 0x0600128E RID: 4750 RVA: 0x000C1B00 File Offset: 0x000BFD00
	public Bounds CalcBounds()
	{
		Logic.HexGrid hexGrid = global::HexGrid.Grid();
		if (hexGrid == null)
		{
			return default(Bounds);
		}
		Logic.HexGrid.Coord c = hexGrid.WorldToGrid(base.transform.position);
		if (!c.valid)
		{
			return default(Bounds);
		}
		Point pt = hexGrid.Center(c);
		Vector3 size = new Vector3(2f * hexGrid.tile_radius, 0f, 1.732f * hexGrid.tile_radius);
		return new Bounds(pt, size);
	}

	// Token: 0x0600128F RID: 4751 RVA: 0x000C1B80 File Offset: 0x000BFD80
	public float CalcAlpha(BSGTerrainTools.Gen2D gen, float wx, float wy, float src_val, float tgt_val, ref float alpha)
	{
		Logic.HexGrid.Coord pt = this.Tile();
		if (!pt.valid)
		{
			return 0f;
		}
		Logic.HexGrid.Coord pt2;
		int num;
		float num2;
		global::HexGrid.Grid().FindNearestEdge(new Point(wx, wy), out pt2, out num, out num2, byte.MaxValue);
		if (pt2 != pt)
		{
			return 0f;
		}
		float num3 = this.edge_width / 2f;
		if (num2 < num3)
		{
			return 0f;
		}
		num2 -= num3;
		if (num2 >= this.edge_fade)
		{
			return 1f;
		}
		return num2 / this.edge_fade;
	}

	// Token: 0x06001290 RID: 4752 RVA: 0x000C1C04 File Offset: 0x000BFE04
	private void CalcHeightmapBounds(TerrainData td, out Vector2Int tile, out Vector2Int size)
	{
		Bounds bounds = this.CalcBounds();
		Vector2Int resolution = BSGTerrainTools.HeightsResolution(td);
		BSGTerrainTools.CalcGridBounds(bounds, td.bounds, resolution, out tile, out size);
	}

	// Token: 0x06001291 RID: 4753 RVA: 0x000C1C2C File Offset: 0x000BFE2C
	private void CalcSplatmapBounds(TerrainData td, out Vector2Int tile, out Vector2Int size)
	{
		Bounds bounds = this.CalcBounds();
		Vector2Int resolution = BSGTerrainTools.SplatsResolution(td);
		BSGTerrainTools.CalcGridBounds(bounds, td.bounds, resolution, out tile, out size);
	}

	// Token: 0x06001292 RID: 4754 RVA: 0x000C1C54 File Offset: 0x000BFE54
	private BSGTerrainTools.Float2D CopyHeights(TerrainData td)
	{
		Vector2Int vector2Int;
		Vector2Int vector2Int2;
		this.CalcHeightmapBounds(td, out vector2Int, out vector2Int2);
		return BSGTerrainTools.Float2D.ImportHeights(td, vector2Int.x, vector2Int.y, vector2Int2.x, vector2Int2.y);
	}

	// Token: 0x06001293 RID: 4755 RVA: 0x000C1C90 File Offset: 0x000BFE90
	private BSGTerrainTools.Float3D CopySplats(TerrainData td)
	{
		Vector2Int vector2Int;
		Vector2Int vector2Int2;
		this.CalcSplatmapBounds(td, out vector2Int, out vector2Int2);
		return BSGTerrainTools.Float3D.ImportSplats(td, vector2Int.x, vector2Int.y, vector2Int2.x, vector2Int2.y);
	}

	// Token: 0x06001294 RID: 4756 RVA: 0x000C1CCC File Offset: 0x000BFECC
	public void DrawHex(Logic.HexGrid grid, Logic.HexGrid.Coord c)
	{
		if (grid == null)
		{
			return;
		}
		Vector3 vector = grid.Center(c);
		Vector3 vector2 = grid.Vertex(c, 0);
		vector2 = vector + (vector2 - vector) * 1f;
		vector2 = global::Common.SnapToTerrain(vector2, 0.1f, null, -1f, false);
		Vector3 from = vector2;
		for (int i = 1; i < 6; i++)
		{
			Vector3 vector3 = grid.Vertex(c, i);
			vector3 = vector + (vector3 - vector) * 1f;
			vector3 = global::Common.SnapToTerrain(vector3, 0.1f, null, -1f, false);
			Gizmos.DrawLine(from, vector3);
			from = vector3;
		}
		Gizmos.DrawLine(from, vector2);
	}

	// Token: 0x06001295 RID: 4757 RVA: 0x000C1D84 File Offset: 0x000BFF84
	public List<Logic.HexGrid.Coord> GetSelected()
	{
		List<Logic.HexGrid.Coord> list = new List<Logic.HexGrid.Coord>();
		Logic.HexGrid.Coord coord = this.Tile();
		if (coord == Logic.HexGrid.Coord.Invalid)
		{
			return list;
		}
		if (this.shapes.Count == 0)
		{
			list.Add(coord);
			return list;
		}
		for (int i = 0; i < this.shapes[this.shape].Count; i++)
		{
			Logic.HexGrid.Coord pt = new Logic.HexGrid.Coord(this.shapes[this.shape][i].x, this.shapes[this.shape][i].y).Rotate(this.rotation);
			Logic.HexGrid.Coord item = coord + pt;
			list.Add(item);
		}
		return list;
	}

	// Token: 0x06001296 RID: 4758 RVA: 0x000C1E44 File Offset: 0x000C0044
	public void LoadDefs()
	{
		DT.Field defField = global::Defs.GetDefField("Hex", null);
		if (defField == null)
		{
			return;
		}
		DT.Field field = defField.FindChild("hex_shapes", null, true, true, true, '.');
		if (field == null)
		{
			return;
		}
		this.def = field;
		List<DT.Field> list = field.Children();
		if (list.Count == 0)
		{
			Debug.LogError("Invalid def");
			return;
		}
		this.options = field.Keys(true, true).ToArray();
		this.shape = this.options[0];
		for (int i = 0; i < list.Count; i++)
		{
			string[] array = list[i].value_str.Replace('"', ' ').Trim().Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			this.shapes[this.options[i]] = new List<Logic.HexGrid.Coord>();
			for (int j = 0; j < array.Length; j++)
			{
				string[] array2 = array[j].Split(new char[]
				{
					','
				});
				this.shapes[this.options[i]].Add(new Logic.HexGrid.Coord(int.Parse(array2[0]), int.Parse(array2[1])));
			}
		}
	}

	// Token: 0x06001297 RID: 4759 RVA: 0x000C1F70 File Offset: 0x000C0170
	public void DrawGizmo(Logic.HexGrid.Coord c, Color color)
	{
		Gizmos.color = color;
		Logic.HexGrid grid = global::HexGrid.Grid();
		if (this.shapes.Count == 0)
		{
			this.DrawHex(grid, c);
			return;
		}
		List<Logic.HexGrid.Coord> selected = this.GetSelected();
		for (int i = 0; i < selected.Count; i++)
		{
			this.DrawHex(grid, selected[i]);
		}
	}

	// Token: 0x06001298 RID: 4760 RVA: 0x000C1FC8 File Offset: 0x000C01C8
	private void Update()
	{
		if (base.transform.hasChanged && this.pos != base.transform.position)
		{
			this.pos = base.transform.position;
			this.canPlace = this.CanPlace();
		}
	}

	// Token: 0x06001299 RID: 4761 RVA: 0x000C2018 File Offset: 0x000C0218
	public bool CanPlace()
	{
		List<Logic.HexGrid.Coord> selected = this.GetSelected();
		if (global::HexGroups.Get() == null)
		{
			return false;
		}
		for (int i = 0; i < selected.Count; i++)
		{
			if (global::HexGroups.Get().groups.Contains(selected[i]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600129A RID: 4762 RVA: 0x000C2068 File Offset: 0x000C0268
	public void BreakGroup()
	{
		List<Logic.HexGrid.Coord> selected = this.GetSelected();
		if (global::HexGroups.Get() == null)
		{
			return;
		}
		Logic.HexGroups groups = global::HexGroups.Get().groups;
		for (int i = 0; i < selected.Count; i++)
		{
			HexGroup group = groups.GetGroup(selected[i]);
			if (group != null)
			{
				groups.Remove(group);
			}
		}
		global::HexGrid.Get().GenerateTexture(false);
		this.canPlace = true;
	}

	// Token: 0x0600129B RID: 4763 RVA: 0x000C20D0 File Offset: 0x000C02D0
	private void OnDrawGizmosSelected()
	{
		this.DrawGizmo(this.Tile(), this.canPlace ? Color.cyan : Color.red);
	}

	// Token: 0x04000C6D RID: 3181
	public string node_type = "";

	// Token: 0x04000C6E RID: 3182
	public bool locked;

	// Token: 0x04000C6F RID: 3183
	public string shape = "0";

	// Token: 0x04000C70 RID: 3184
	public int rotation;

	// Token: 0x04000C71 RID: 3185
	public string[] options;

	// Token: 0x04000C72 RID: 3186
	public string[] rotations = new string[]
	{
		"0",
		"1",
		"2",
		"3",
		"4",
		"5",
		"-1",
		"-2",
		"-3",
		"-4",
		"-5"
	};

	// Token: 0x04000C73 RID: 3187
	public Dictionary<string, List<Logic.HexGrid.Coord>> shapes = new Dictionary<string, List<Logic.HexGrid.Coord>>();

	// Token: 0x04000C74 RID: 3188
	public DT.Field def;

	// Token: 0x04000C75 RID: 3189
	public BSGTerrainTools.Float2D heights;

	// Token: 0x04000C76 RID: 3190
	public BSGTerrainTools.Float3D splats;

	// Token: 0x04000C77 RID: 3191
	public BSGTerrainTools.Float2D bg_heights;

	// Token: 0x04000C78 RID: 3192
	public BSGTerrainTools.Float3D bg_splats;

	// Token: 0x04000C79 RID: 3193
	public Vector3 pos;

	// Token: 0x04000C7A RID: 3194
	public bool canPlace = true;

	// Token: 0x04000C7B RID: 3195
	public bool hasRotation;

	// Token: 0x04000C7C RID: 3196
	private float edge_width;

	// Token: 0x04000C7D RID: 3197
	private float edge_fade;
}

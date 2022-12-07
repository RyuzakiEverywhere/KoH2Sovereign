using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A3 RID: 163
[ExecuteInEditMode]
public class TerrainObject : MonoBehaviour
{
	// Token: 0x1700004A RID: 74
	// (get) Token: 0x060005BB RID: 1467 RVA: 0x0003EA9C File Offset: 0x0003CC9C
	public List<BSGTerrainTools.PerTerrainInfo> terrain_infos
	{
		get
		{
			if (this._terrain_infos != null && this.has_terrains_gameobject)
			{
				return this._terrain_infos;
			}
			GameObject gameObject = GameObject.Find("Terrains");
			this.has_terrains_gameobject = (gameObject != null);
			if (!(gameObject == null))
			{
				this._terrain_infos = new List<BSGTerrainTools.PerTerrainInfo>();
				for (int i = 0; i < gameObject.transform.childCount; i++)
				{
					Terrain component = gameObject.transform.GetChild(i).GetComponent<Terrain>();
					if (!(component == null))
					{
						BSGTerrainTools.PerTerrainInfo perTerrainInfo = new BSGTerrainTools.PerTerrainInfo();
						perTerrainInfo.td = component.terrainData;
						this._terrain_infos.Add(perTerrainInfo);
					}
				}
				return this._terrain_infos;
			}
			Vector3 vector;
			Terrain terrainAt = Common.GetTerrainAt(base.transform.position, out vector, null);
			if (terrainAt == null)
			{
				return this._terrain_infos;
			}
			if (this._terrain_infos == null)
			{
				this._terrain_infos = new List<BSGTerrainTools.PerTerrainInfo>();
			}
			if (this._terrain_infos.Count == 0)
			{
				BSGTerrainTools.PerTerrainInfo perTerrainInfo2 = new BSGTerrainTools.PerTerrainInfo();
				perTerrainInfo2.td = terrainAt.terrainData;
				perTerrainInfo2.t = terrainAt;
				this._terrain_infos.Add(perTerrainInfo2);
			}
			else
			{
				this._terrain_infos[0].td = terrainAt.terrainData;
				this._terrain_infos[0].t = terrainAt;
			}
			return this._terrain_infos;
		}
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x0003EBEA File Offset: 0x0003CDEA
	private void Update()
	{
		if (base.transform.position != this.last_pos)
		{
			if (this.auto_move)
			{
				this.Move();
			}
			this.last_pos = base.transform.position;
		}
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x0003EC24 File Offset: 0x0003CE24
	public void Clear()
	{
		if (this.terrain_infos == null)
		{
			return;
		}
		for (int i = 0; i < this.terrain_infos.Count; i++)
		{
			this.terrain_infos[i].cells = null;
		}
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x0003EC64 File Offset: 0x0003CE64
	private void Move()
	{
		Vector3 position = base.transform.position;
		base.transform.position = this.last_pos;
		this.Erase();
		base.transform.position = position;
		this.Remember();
		this.Paste();
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x0003ECAC File Offset: 0x0003CEAC
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

	// Token: 0x060005C0 RID: 1472 RVA: 0x0003ECE0 File Offset: 0x0003CEE0
	private void CalcHeightmapBounds(TerrainData td, Vector3 t_position, out Vector2Int tile, out Vector2Int size)
	{
		Bounds bounds = new Bounds(base.transform.position - t_position, new Vector3(this.width, 0f, this.height));
		Vector2Int resolution = BSGTerrainTools.HeightsResolution(td);
		BSGTerrainTools.CalcGridBounds(bounds, td.bounds, resolution, out tile, out size);
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x0003ED30 File Offset: 0x0003CF30
	private void CalcSplatmapBounds(TerrainData td, Vector3 t_position, out Vector2Int tile, out Vector2Int size)
	{
		Bounds bounds = new Bounds(base.transform.position - t_position, new Vector3(this.width, 0f, this.height));
		Vector2Int resolution = BSGTerrainTools.SplatsResolution(td);
		BSGTerrainTools.CalcGridBounds(bounds, td.bounds, resolution, out tile, out size);
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x0003ED80 File Offset: 0x0003CF80
	private BSGTerrainTools.Float2D CopyHeights(TerrainData td, Vector3 t_position)
	{
		Vector2Int vector2Int;
		Vector2Int vector2Int2;
		this.CalcHeightmapBounds(td, t_position, out vector2Int, out vector2Int2);
		return BSGTerrainTools.Float2D.ImportHeights(td, vector2Int.x, vector2Int.y, vector2Int2.x, vector2Int2.y);
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x0003EDBC File Offset: 0x0003CFBC
	private BSGTerrainTools.Float3D CopySplats(TerrainData td, Vector3 t_position)
	{
		Vector2Int vector2Int;
		Vector2Int vector2Int2;
		this.CalcSplatmapBounds(td, t_position, out vector2Int, out vector2Int2);
		return BSGTerrainTools.Float3D.ImportSplats(td, vector2Int.x, vector2Int.y, vector2Int2.x, vector2Int2.y);
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x0003EDF8 File Offset: 0x0003CFF8
	private List<TreeInstance> CopyTrees(TerrainData td, Vector3 t_position)
	{
		List<TreeInstance> list = new List<TreeInstance>();
		Bounds bounds = new Bounds(base.transform.position - t_position, new Vector3(this.width, td.size.y, this.height));
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		for (int i = 0; i < td.treeInstanceCount; i++)
		{
			TreeInstance treeInstance = td.GetTreeInstance(i);
			Vector3 vector = Vector3.Scale(treeInstance.position, td.size);
			if (vector.x > min.x && vector.x < max.x && vector.z > min.z && vector.z < max.z)
			{
				vector = new Vector3(vector.x - bounds.center.x, vector.y, vector.z - bounds.center.z);
				vector = new Vector3(vector.x / td.size.x, vector.y / td.size.y, vector.z / td.size.z);
				treeInstance.position = vector;
				list.Add(treeInstance);
			}
		}
		return list;
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x0003EF58 File Offset: 0x0003D158
	public void Copy()
	{
		if (this.terrain_infos == null)
		{
			return;
		}
		for (int i = 0; i < this.terrain_infos.Count; i++)
		{
			BSGTerrainTools.PerTerrainInfo perTerrainInfo = this.terrain_infos[i];
			BSGTerrainTools.PerTerrainInfo.PerCellInfo perCellInfo = new BSGTerrainTools.PerTerrainInfo.PerCellInfo();
			Vector3? vector;
			if (perTerrainInfo == null)
			{
				vector = null;
			}
			else
			{
				Terrain t = perTerrainInfo.t;
				if (t == null)
				{
					vector = null;
				}
				else
				{
					Transform transform = t.transform;
					vector = ((transform != null) ? new Vector3?(transform.position) : null);
				}
			}
			Vector3 t_position = vector ?? Vector3.zero;
			perCellInfo.heights = this.CopyHeights(perTerrainInfo.td, t_position);
			perCellInfo.splats = this.CopySplats(perTerrainInfo.td, t_position);
			perCellInfo.trees = this.CopyTrees(perTerrainInfo.td, t_position);
			perTerrainInfo.cells = new BSGTerrainTools.PerTerrainInfo.PerCellInfo[1, 1];
			perTerrainInfo.cells[0, 0] = perCellInfo;
		}
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x0003F050 File Offset: 0x0003D250
	public void Remember()
	{
		if (this.terrain_infos == null)
		{
			return;
		}
		for (int i = 0; i < this.terrain_infos.Count; i++)
		{
			BSGTerrainTools.PerTerrainInfo perTerrainInfo = this.terrain_infos[i];
			if (perTerrainInfo.cells != null)
			{
				Vector3? vector;
				if (perTerrainInfo == null)
				{
					vector = null;
				}
				else
				{
					Terrain t = perTerrainInfo.t;
					if (t == null)
					{
						vector = null;
					}
					else
					{
						Transform transform = t.transform;
						vector = ((transform != null) ? new Vector3?(transform.position) : null);
					}
				}
				Vector3 t_position = vector ?? Vector3.zero;
				perTerrainInfo.cells[0, 0].bg_heights = this.CopyHeights(perTerrainInfo.td, t_position);
				perTerrainInfo.cells[0, 0].bg_splats = this.CopySplats(perTerrainInfo.td, t_position);
				perTerrainInfo.cells[0, 0].bg_trees = this.CopyTrees(perTerrainInfo.td, t_position);
			}
		}
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x0003F154 File Offset: 0x0003D354
	private void PasteHeights(TerrainData td, Vector3 t_position, BSGTerrainTools.Float2D heights)
	{
		if (heights == null)
		{
			return;
		}
		Vector2Int vector2Int;
		Vector2Int vector2Int2;
		this.CalcHeightmapBounds(td, t_position, out vector2Int, out vector2Int2);
		heights.ExportHeights(td, vector2Int.x, vector2Int.y);
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x0003F188 File Offset: 0x0003D388
	private void PasteSplats(TerrainData td, Vector3 t_position, BSGTerrainTools.Float3D splats)
	{
		if (splats == null)
		{
			return;
		}
		Vector2Int vector2Int;
		Vector2Int vector2Int2;
		this.CalcSplatmapBounds(td, t_position, out vector2Int, out vector2Int2);
		splats.ExportSplats(td, vector2Int.x, vector2Int.y, false, 1f, 1f, 0f, null);
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x0003F1CC File Offset: 0x0003D3CC
	public void Paste()
	{
		if (this.terrain_infos == null)
		{
			return;
		}
		this.EraseTrees(false);
		for (int i = 0; i < this.terrain_infos.Count; i++)
		{
			BSGTerrainTools.PerTerrainInfo perTerrainInfo = this.terrain_infos[i];
			if (perTerrainInfo.cells != null)
			{
				BSGTerrainTools.PerTerrainInfo.PerCellInfo perCellInfo = perTerrainInfo.cells[0, 0];
				Vector3? vector;
				if (perTerrainInfo == null)
				{
					vector = null;
				}
				else
				{
					Terrain t = perTerrainInfo.t;
					if (t == null)
					{
						vector = null;
					}
					else
					{
						Transform transform = t.transform;
						vector = ((transform != null) ? new Vector3?(transform.position) : null);
					}
				}
				Vector3 b = vector ?? Vector3.zero;
				Bounds bounds = new Bounds(base.transform.position - b, new Vector3(this.width, 0f, this.height));
				if (perCellInfo.heights != null)
				{
					Bounds bounds3 = BSGTerrainTools.SnapHeightsBounds(bounds, perTerrainInfo.td);
					perCellInfo.heights.SetWorldRect(bounds3.min.x, bounds3.min.z, bounds3.size.x, bounds3.size.z);
				}
				if (perCellInfo.splats != null)
				{
					Bounds bounds2 = BSGTerrainTools.SnapSplatsBounds(bounds, perTerrainInfo.td);
					perCellInfo.splats.SetWorldRect(bounds2.min.x, bounds2.min.z, bounds2.size.x, bounds2.size.z);
				}
				BSGTerrainTools.TerrainBlock terrainBlock = new BSGTerrainTools.TerrainBlock(perTerrainInfo.td, bounds);
				terrainBlock.Import(true, true, false);
				BSGTerrainTools.Gen2D gen2D = new BSGTerrainTools.Gen2D(1f);
				gen2D.func = delegate(BSGTerrainTools.Gen2D gen, float wx, float wy, float src_val, float tgt_val, ref float alpha)
				{
					float num = wx - bounds.min.x;
					float num2 = bounds.max.x - wx;
					float num3 = wy - bounds.min.z;
					float num4 = bounds.max.z - wy;
					float num5 = Mathf.Min(new float[]
					{
						num,
						num2,
						num3,
						num4
					});
					if (num5 <= 0f)
					{
						return 0f;
					}
					if (num5 >= this.fade)
					{
						return 1f;
					}
					return num5 / this.fade;
				};
				if (perCellInfo.heights != null && this.copy_heights)
				{
					BSGTerrainTools.ModifyHeights(terrainBlock, bounds, new BSGTerrainTools.Gen2D(perCellInfo.heights), gen2D);
				}
				if (perCellInfo.splats != null && this.copy_splats)
				{
					BSGTerrainTools.ModifySplats(terrainBlock, bounds, new BSGTerrainTools.Gen3D(perCellInfo.splats), gen2D);
				}
				if (perCellInfo.trees != null && this.copy_trees)
				{
					BSGTerrainTools.PasteTrees(perTerrainInfo.td, bounds.center, perTerrainInfo.cells[0, 0].trees, null, Vector3.one, 0f);
				}
				terrainBlock.Apply(true, true, true);
			}
		}
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x0003F458 File Offset: 0x0003D658
	public void Erase()
	{
		if (this.terrain_infos == null)
		{
			return;
		}
		for (int i = 0; i < this.terrain_infos.Count; i++)
		{
			BSGTerrainTools.PerTerrainInfo perTerrainInfo = this.terrain_infos[i];
			if (perTerrainInfo.cells != null)
			{
				Vector3? vector;
				if (perTerrainInfo == null)
				{
					vector = null;
				}
				else
				{
					Terrain t = perTerrainInfo.t;
					if (t == null)
					{
						vector = null;
					}
					else
					{
						Transform transform = t.transform;
						vector = ((transform != null) ? new Vector3?(transform.position) : null);
					}
				}
				Vector3 t_position = vector ?? Vector3.zero;
				this.PasteHeights(perTerrainInfo.td, t_position, perTerrainInfo.cells[0, 0].bg_heights);
				this.PasteSplats(perTerrainInfo.td, t_position, perTerrainInfo.cells[0, 0].bg_splats);
			}
		}
		this.EraseTrees(true);
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x0003F544 File Offset: 0x0003D744
	public void EraseTrees(bool paste = true)
	{
		if (this.terrain_infos == null)
		{
			return;
		}
		for (int i = 0; i < this.terrain_infos.Count; i++)
		{
			BSGTerrainTools.PerTerrainInfo perTerrainInfo = this.terrain_infos[i];
			if (perTerrainInfo.cells != null)
			{
				Vector3? vector;
				if (perTerrainInfo == null)
				{
					vector = null;
				}
				else
				{
					Terrain t = perTerrainInfo.t;
					if (t == null)
					{
						vector = null;
					}
					else
					{
						Transform transform = t.transform;
						vector = ((transform != null) ? new Vector3?(transform.position) : null);
					}
				}
				Vector3 b = vector ?? Vector3.zero;
				BSGTerrainTools.EraseTrees(perTerrainInfo.td, new Bounds(base.transform.position - b, new Vector3(this.width, perTerrainInfo.td.size.y, this.height)));
				if (paste)
				{
					BSGTerrainTools.PasteTrees(perTerrainInfo.td, base.transform.position - b, perTerrainInfo.cells[0, 0].bg_trees, null, Vector3.one, 0f);
				}
			}
		}
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x0003F670 File Offset: 0x0003D870
	private void OnDrawGizmos()
	{
		TerrainData terrainData = this.GetTerrainData();
		if (terrainData == null)
		{
			return;
		}
		base.transform.position.y = terrainData.size.y / 2f;
		Vector3 size = new Vector3(this.width, terrainData.size.y, this.height);
		Gizmos.color = ((this.terrain_infos == null || this.terrain_infos[0].cells == null) ? Color.red : Color.green);
		Gizmos.DrawWireCube(base.transform.position, size);
	}

	// Token: 0x04000541 RID: 1345
	public float width = 100f;

	// Token: 0x04000542 RID: 1346
	public float height = 50f;

	// Token: 0x04000543 RID: 1347
	public float fade = 8f;

	// Token: 0x04000544 RID: 1348
	public bool auto_move = true;

	// Token: 0x04000545 RID: 1349
	private bool has_terrains_gameobject;

	// Token: 0x04000546 RID: 1350
	public bool copy_heights = true;

	// Token: 0x04000547 RID: 1351
	public bool copy_splats = true;

	// Token: 0x04000548 RID: 1352
	public bool copy_trees = true;

	// Token: 0x04000549 RID: 1353
	public List<BSGTerrainTools.PerTerrainInfo> _terrain_infos;

	// Token: 0x0400054A RID: 1354
	private Vector3 last_pos = Vector3.zero;
}

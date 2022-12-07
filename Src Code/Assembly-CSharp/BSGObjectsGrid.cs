using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200009A RID: 154
public class BSGObjectsGrid
{
	// Token: 0x17000048 RID: 72
	// (get) Token: 0x06000582 RID: 1410 RVA: 0x0003D2AF File Offset: 0x0003B4AF
	public Vector2Int Grid_Size
	{
		get
		{
			return this.grid_size;
		}
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x0003D2B7 File Offset: 0x0003B4B7
	public Vector2Int WorldToGrid(Vector3 ptw)
	{
		ptw /= this.tile_size;
		return new Vector2Int((int)ptw.x, (int)ptw.z);
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x0003D2DA File Offset: 0x0003B4DA
	public Vector2 GridToWorld(Vector2Int ptg)
	{
		return new Vector3((float)ptg.x + 0.5f, 0f, (float)ptg.y + 0.5f) * this.tile_size;
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x0003D314 File Offset: 0x0003B514
	public void ImportObjects(Terrain terrain, float tile_size = 16f)
	{
		TerrainData terrainData = terrain.terrainData;
		Stopwatch stopwatch = Stopwatch.StartNew();
		List<GameObject> game_objects = new List<GameObject>(SceneManager.GetActiveScene().GetRootGameObjects());
		GameObject[] array = this.GetObjectsToClone(game_objects).ToArray();
		if (this.grid != null && terrain == this.selected_terrain && this.total_objects_count == array.Length)
		{
			return;
		}
		this.selected_terrain = terrain;
		this.total_objects_count = array.Length;
		this.tile_size = tile_size;
		this.grid_size = new Vector2Int((int)(terrainData.size.x / tile_size), (int)(terrainData.size.z / tile_size));
		this.grid = new List<GameObject>[this.grid_size.y, this.grid_size.x];
		this.objects_count = 0;
		for (int i = 0; i < array.Length; i++)
		{
			this.AddObject(array[i]);
		}
		this.total_objects_count = array.Length;
		long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		Debug.Log(string.Format("Imported {0} objects for {1}ms", this.objects_count, elapsedMilliseconds));
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x0003D428 File Offset: 0x0003B628
	public List<GameObject> GetObjectsToClone(List<GameObject> game_objects)
	{
		List<GameObject> list = new List<GameObject>();
		List<GameObject> collection = new List<GameObject>();
		List<GameObject> game_objects2 = new List<GameObject>();
		for (int i = 0; i < game_objects.Count; i++)
		{
			BSGCloneHelper.ObjectCloneAction objectCloneAction = BSGCloneHelper.GetObjectCloneAction(game_objects[i]);
			if (objectCloneAction == BSGCloneHelper.ObjectCloneAction.Clone)
			{
				list.Add(game_objects[i]);
			}
			else if (objectCloneAction == BSGCloneHelper.ObjectCloneAction.GoDeeper)
			{
				game_objects2 = BSGObjectsGrid.GetAllChilds(game_objects[i].transform);
				collection = this.GetObjectsToClone(game_objects2);
				list.AddRange(collection);
			}
		}
		return list;
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x0003D4A0 File Offset: 0x0003B6A0
	private static List<GameObject> GetAllChilds(Transform object_transform)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < object_transform.childCount; i++)
		{
			list.Add(object_transform.GetChild(i).gameObject);
		}
		return list;
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x0003D4D8 File Offset: 0x0003B6D8
	public bool AddObject(GameObject go)
	{
		Vector3 vector = this.selected_terrain.transform.InverseTransformPoint(go.transform.position);
		Vector2Int vector2Int = this.WorldToGrid(vector);
		if (vector.x <= 0f || vector.z <= 0f || vector2Int.x < 0 || vector2Int.y < 0 || vector2Int.x >= this.grid_size.x || vector2Int.y >= this.grid_size.y)
		{
			return false;
		}
		List<GameObject> list = this.grid[vector2Int.y, vector2Int.x];
		if (list == null)
		{
			list = new List<GameObject>();
			this.grid[vector2Int.y, vector2Int.x] = list;
		}
		list.Add(go);
		Transform[] componentsInChildren = go.GetComponentsInChildren<Transform>();
		this.objects_count += componentsInChildren.Length;
		this.total_objects_count += componentsInChildren.Length;
		return true;
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x0003D5D0 File Offset: 0x0003B7D0
	public void DelObject(GameObject go)
	{
		Vector3 vector = this.selected_terrain.transform.InverseTransformPoint(go.transform.position);
		Vector2Int vector2Int = this.WorldToGrid(vector);
		if (vector.x <= 0f || vector.z <= 0f || vector2Int.x < 0 || vector2Int.y < 0 || vector2Int.x >= this.grid_size.x || vector2Int.y >= this.grid_size.y)
		{
			return;
		}
		List<GameObject> list = this.grid[vector2Int.y, vector2Int.x];
		list.Remove(go);
		int num = go.GetComponentsInChildren<Transform>().Length;
		Object.DestroyImmediate(go);
		int count = list.Count;
		this.objects_count -= num;
		this.total_objects_count -= num;
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x0003D6AA File Offset: 0x0003B8AA
	public List<GameObject> GetGrid(int y, int x)
	{
		return this.grid[y, x];
	}

	// Token: 0x0400051E RID: 1310
	private Vector2Int grid_size;

	// Token: 0x0400051F RID: 1311
	private float tile_size;

	// Token: 0x04000520 RID: 1312
	private int objects_count;

	// Token: 0x04000521 RID: 1313
	private int total_objects_count;

	// Token: 0x04000522 RID: 1314
	private Terrain selected_terrain;

	// Token: 0x04000523 RID: 1315
	private List<GameObject>[,] grid;
}

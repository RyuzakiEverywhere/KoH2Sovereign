using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000077 RID: 119
public static class MeshUtils
{
	// Token: 0x0600047B RID: 1147 RVA: 0x00034F58 File Offset: 0x00033158
	public static Mesh CreateGridMesh(Vector3 pos, int width, int height, Vector3 vx, Vector3 vy, bool tile_x = false, bool tile_y = false)
	{
		if (width < 1 || height < 1)
		{
			return null;
		}
		int num = (width + 1) * (height + 1);
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		int[] array3 = new int[width * height * 6];
		int num2 = 0;
		int num3 = 1;
		int num4 = width + 1;
		float num5 = tile_x ? vx.magnitude : (1f / (float)width);
		float num6 = tile_y ? vy.magnitude : (1f / (float)height);
		for (int i = 0; i <= height; i++)
		{
			float y = (float)i * num6;
			for (int j = 0; j <= width; j++)
			{
				array[num2] = pos + (float)j * vx + (float)i * vy;
				float x = (float)j * num5;
				array2[num2] = new Vector2(x, y);
				if (i < height && j < width)
				{
					int num7 = (width * i + j) * 6;
					array3[num7] = num2;
					array3[num7 + 1] = num2 + num4;
					array3[num7 + 2] = num2 + num3 + num4;
					array3[num7 + 3] = num2;
					array3[num7 + 4] = num2 + num3 + num4;
					array3[num7 + 5] = num2 + num3;
				}
				num2++;
			}
		}
		Mesh mesh = new Mesh();
		mesh.MarkDynamic();
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		return mesh;
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x000350AC File Offset: 0x000332AC
	public static Vector3 RotateVector2D(Vector3 v, float angle)
	{
		float f = -angle * 0.017453292f;
		float num = Mathf.Sin(f);
		float num2 = Mathf.Cos(f);
		return new Vector3(v.x * num2 - v.z * num, v.y, v.x * num + v.z * num2);
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x000350FC File Offset: 0x000332FC
	public static float DistPtLine2D(Vector3 P, Vector3 P1, Vector3 P2)
	{
		float num = P2.x - P1.x;
		float num2 = P2.z - P1.z;
		float num3 = num * num + num2 * num2;
		if (num3 < 0.0001f)
		{
			return new Vector2(P.x - P1.x, P.z - P1.z).magnitude;
		}
		float num4 = P2.x * P1.z - P2.z * P1.x;
		float num5 = Mathf.Sqrt(num3);
		return (num2 * P.x - num * P.z + num4) / num5;
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x00035198 File Offset: 0x00033398
	public static Vector3 Intersect2DLines(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4)
	{
		if ((P3 - P1).magnitude < 0.1f)
		{
			return P1;
		}
		float num = P1.x - P2.x;
		float num2 = P1.z - P2.z;
		float num3 = P3.x - P4.x;
		float num4 = P3.z - P4.z;
		float num5 = num * num4 - num2 * num3;
		if (Mathf.Abs(num5) < 0.01f)
		{
			return P1;
		}
		float num6 = P1.x * P2.z - P1.z * P2.x;
		float num7 = P3.x * P4.z - P3.z * P4.x;
		float x = (num6 * num3 - num7 * num) / num5;
		float z = (num6 * num4 - num7 * num2) / num5;
		return new Vector3(x, P1.y, z);
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x0003526F File Offset: 0x0003346F
	public static float Perp2D(Vector3 v1, Vector3 v2)
	{
		return v1.x * v2.z - v1.z * v2.x;
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x0003528C File Offset: 0x0003348C
	public static bool Intersect2DSegments(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4)
	{
		Vector3 v = P2 - P1;
		Vector3 vector = P4 - P3;
		Vector3 v2 = P1 - P3;
		float num = MeshUtils.Perp2D(v, vector);
		if (Mathf.Abs(num) < 0.0001f)
		{
			return false;
		}
		float num2 = MeshUtils.Perp2D(vector, v2) / num;
		if (num2 < -0.2f || num2 > 1.2f)
		{
			return false;
		}
		float num3 = MeshUtils.Perp2D(v, v2) / num;
		return num3 >= -0.2f && num3 <= 1.2f;
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x00035308 File Offset: 0x00033508
	private static float CalcLineVertices(Vector3 pt, Vector3 v, float thickness, out Vector3 ptBot, out Vector3 ptTop)
	{
		Vector3 vector = new Vector3(v.z, 0f, -v.x);
		float magnitude = v.magnitude;
		if (magnitude > 0f)
		{
			vector *= thickness / (2f * magnitude);
		}
		ptBot = pt + vector;
		ptTop = pt - vector;
		return magnitude;
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x0003536C File Offset: 0x0003356C
	private static float CalcCornerVertices(List<Vector3> points, float thickness, int idx, out Vector3 ptMid, out Vector3 ptBot, out Vector3 ptTop)
	{
		ptMid = points[idx];
		if (idx == 0)
		{
			return MeshUtils.CalcLineVertices(ptMid, points[1] - ptMid, thickness, out ptBot, out ptTop);
		}
		if (idx + 1 == points.Count)
		{
			MeshUtils.CalcLineVertices(ptMid, ptMid - points[idx - 1], thickness, out ptBot, out ptTop);
			return 0f;
		}
		Vector3 vector = points[idx + 1] - ptMid;
		float magnitude = vector.magnitude;
		Vector3 vector2 = new Vector3(vector.z, 0f, -vector.x);
		if (magnitude > 0f)
		{
			vector2 /= magnitude;
		}
		Vector3 vector3 = ptMid - points[idx - 1];
		float magnitude2 = vector3.magnitude;
		Vector3 vector4 = new Vector3(vector3.z, 0f, -vector3.x);
		if (magnitude2 > 0f)
		{
			vector4 /= magnitude2;
		}
		vector2 = (vector2 + vector4).normalized * (thickness / 2f);
		ptBot = ptMid + vector2;
		ptTop = ptMid - vector2;
		return magnitude;
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x000354B8 File Offset: 0x000336B8
	public static Mesh CreateLinesMesh(List<Vector3> points, float thickness, int y_vertices = 2, bool tile_x = true, bool tile_y = false, List<float> thicknesses = null, List<Color> colors = null)
	{
		int count = points.Count;
		if (count < 2)
		{
			return null;
		}
		int num = count * y_vertices;
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		Vector4[] array3 = new Vector4[num];
		int[] array4 = new int[(count - 1) * (y_vertices - 1) * 6];
		Color[] array5 = null;
		bool flag = colors != null;
		if (flag)
		{
			array5 = new Color[num];
		}
		if (thicknesses != null && thicknesses.Count != count)
		{
			thicknesses = null;
		}
		int num2 = 0;
		int num3 = 1;
		float num4 = 0f;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		Vector3 zero3 = Vector3.zero;
		for (int i = 0; i < count; i++)
		{
			float num5 = thickness;
			if (thicknesses != null)
			{
				num5 *= thicknesses[i];
			}
			float num6 = tile_y ? (num5 / (float)(y_vertices - 1)) : (1f / (float)(y_vertices - 1));
			Vector3 vector;
			Vector3 vector2;
			Vector3 a;
			float num7 = MeshUtils.CalcCornerVertices(points, num5, i, out vector, out vector2, out a);
			Color color = Color.white;
			if (colors != null)
			{
				color = colors[i];
			}
			Vector3 vector3 = a - vector2;
			vector3 /= (float)(y_vertices - 1);
			Vector4 vector4 = Common.GetRightVector(vector3, 1f);
			vector4.w = -1f;
			Vector3 vector5 = vector2;
			for (int j = 0; j < y_vertices; j++)
			{
				array[num2] = vector5;
				if (flag)
				{
					array5[num2] = color;
				}
				float y = (float)j * num6;
				array2[num2] = new Vector2(num4, y);
				array3[num2] = vector4;
				if (j < y_vertices - 1 && i < count - 1)
				{
					int num8 = ((y_vertices - 1) * i + j) * 6;
					array4[num8] = num2;
					array4[num8 + 1] = num2 + num3;
					array4[num8 + 2] = num2 + y_vertices;
					array4[num8 + 3] = num2 + num3;
					array4[num8 + 4] = num2 + y_vertices + num3;
					array4[num8 + 5] = num2 + y_vertices;
				}
				num2++;
				vector5 += vector3;
			}
			num4 += num7;
		}
		Mesh mesh = new Mesh();
		mesh.MarkDynamic();
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array4;
		mesh.tangents = array3;
		if (flag)
		{
			mesh.colors = array5;
		}
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		return mesh;
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x00035708 File Offset: 0x00033908
	public static Mesh CreateDiscMesh(float radius, int sectors, bool looksUp)
	{
		if (sectors < 4)
		{
			sectors = 4;
		}
		int[] array = new int[sectors * 3];
		Vector3[] array2 = new Vector3[sectors + 2];
		Vector3[] array3 = new Vector3[sectors + 2];
		Vector2[] array4 = new Vector2[sectors + 2];
		array2[0] = Vector3.zero;
		array3[0] = (looksUp ? Vector3.up : Vector3.down);
		array4[0] = new Vector2(0.5f, 0.5f);
		for (int i = 0; i <= sectors; i++)
		{
			float f = 6.2831855f / (float)sectors * (float)i;
			float num = Mathf.Cos(f);
			float num2 = Mathf.Sin(f);
			Vector3 a = new Vector3(num, 0f, num2);
			array2[i + 1] = radius * a;
			array3[i + 1] = (looksUp ? Vector3.up : Vector3.down);
			array4[i + 1] = new Vector2(Common.map(num, -1f, 1f, 0f, 1f, false), Common.map(num2, -1f, 1f, 0f, 1f, false));
		}
		for (int j = 0; j < sectors; j++)
		{
			if (looksUp)
			{
				array[j * 3] = 0;
				array[j * 3 + 1] = j + 2;
				array[j * 3 + 2] = j + 1;
			}
			else
			{
				array[j * 3] = 0;
				array[j * 3 + 1] = j + 1;
				array[j * 3 + 2] = j + 2;
			}
		}
		Mesh mesh = new Mesh();
		mesh.MarkDynamic();
		mesh.vertices = array2;
		mesh.uv = array4;
		mesh.SetIndices(array, MeshTopology.Triangles, 0);
		mesh.RecalculateNormals();
		return mesh;
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x000358A8 File Offset: 0x00033AA8
	public static Mesh CreateTriangleFanMesh(List<Vector3> pts)
	{
		if (pts == null)
		{
			return null;
		}
		int count = pts.Count;
		if (count < 3)
		{
			return null;
		}
		int[] array = new int[count * 3];
		Vector3[] array2 = new Vector3[count + 1];
		array2[0] = Vector3.zero;
		for (int i = 0; i < count; i++)
		{
			array2[i + 1] = pts[i];
		}
		for (int j = 0; j < count; j++)
		{
			array[j * 3] = 0;
			array[j * 3 + 1] = 1 + (j + 1) % count;
			array[j * 3 + 2] = 1 + j;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = array2;
		mesh.SetIndices(array, MeshTopology.Triangles, 0);
		return mesh;
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x0003594C File Offset: 0x00033B4C
	public static GameObject CreateLinesObject(Material material, List<Vector3> points, float thickness, float terrain_snap_ofs = -1f, bool ajdust_normals_to_terrain = false, int y_vertices = 2, bool tile_x = true, bool tile_y = false, List<float> thicknesses = null)
	{
		Mesh mesh = MeshUtils.CreateLinesMesh(points, thickness, y_vertices, tile_x, tile_y, thicknesses, null);
		if (mesh == null)
		{
			return null;
		}
		GameObject gameObject = new GameObject();
		if (terrain_snap_ofs >= 0f)
		{
			mesh = MeshUtils.SnapMeshToTerrain(mesh, gameObject.transform, terrain_snap_ofs, ajdust_normals_to_terrain, null);
		}
		if (!ajdust_normals_to_terrain)
		{
			mesh.RecalculateNormals();
		}
		mesh.RecalculateBounds();
		gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = material;
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		return gameObject;
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x000359C4 File Offset: 0x00033BC4
	public static GameObject CreateLineRendererObject(Material material, List<Vector3> points, float thickness, float terrain_snap_ofs = -1f)
	{
		GameObject gameObject = new GameObject();
		LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.numCapVertices = 2;
		lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
		lineRenderer.textureMode = LineTextureMode.Tile;
		lineRenderer.sortingLayerName = "OnTop";
		lineRenderer.sortingOrder = 5;
		lineRenderer.enabled = true;
		lineRenderer.useWorldSpace = false;
		lineRenderer.material = material;
		LineRenderer lineRenderer2 = lineRenderer;
		lineRenderer.endWidth = thickness;
		lineRenderer2.startWidth = thickness;
		lineRenderer.positionCount = points.Count;
		Vector3[] array;
		if (terrain_snap_ofs < 0f)
		{
			array = points.ToArray();
		}
		else
		{
			array = new Vector3[points.Count];
			for (int i = 0; i < points.Count; i++)
			{
				Vector3 vector = Common.SnapToTerrain(points[i], terrain_snap_ofs, null, -1f, false);
				array[i] = vector;
			}
		}
		lineRenderer.SetPositions(array);
		return gameObject;
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x00035A94 File Offset: 0x00033C94
	public static GameObject CreateDiscObject(Material material, float radius, int radiusTiles, float terrain_snap_ofs = -1f, int y_vertices = 2)
	{
		Mesh mesh = MeshUtils.CreateDiscMesh(radius, radiusTiles, true);
		GameObject gameObject = new GameObject();
		if (terrain_snap_ofs >= 0f)
		{
			mesh = MeshUtils.SnapMeshToTerrain(mesh, gameObject.transform, terrain_snap_ofs, false, null);
		}
		gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = material;
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		return gameObject;
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x00035AE8 File Offset: 0x00033CE8
	public static Mesh SnapMeshToTerrain(Mesh mesh, Transform t, float ofs, bool adjust_normals = false, Terrain terrain = null)
	{
		if (mesh == null)
		{
			return null;
		}
		Vector3[] vertices = mesh.vertices;
		Vector3[] array = adjust_normals ? new Vector3[vertices.Length] : null;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector = t.TransformPoint(vertices[i]);
			vector = Common.SnapToTerrain(vector, ofs, terrain, -1f, false);
			vertices[i] = t.InverseTransformPoint(vector);
			if (adjust_normals)
			{
				Vector3 terrainNormal = Common.GetTerrainNormal(vector, terrain);
				array[i] = t.InverseTransformDirection(terrainNormal);
			}
		}
		mesh.vertices = vertices;
		if (adjust_normals)
		{
			mesh.normals = array;
		}
		return mesh;
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x00035B80 File Offset: 0x00033D80
	public static Mesh AdjustMeshToTerrain(Mesh mesh, Transform t, float ofs, Terrain terrain = null, bool water = false)
	{
		Vector3[] vertices = mesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector = t.TransformPoint(vertices[i]);
			float num = Common.GetTerrainHeight(vector, terrain, false);
			if (water)
			{
				num = Mathf.Max(MapData.GetWaterLevel(), num);
			}
			vector.y += num - t.position.y + ofs;
			vertices[i] = t.InverseTransformPoint(vector);
		}
		mesh.vertices = vertices;
		mesh.RecalculateBounds();
		return mesh;
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x00035C00 File Offset: 0x00033E00
	public static void AdjustObjToTerrain(GameObject obj, float ofs, Terrain terrain = null, bool water = false)
	{
		foreach (MeshFilter meshFilter in obj.GetComponentsInChildren<MeshFilter>())
		{
			Mesh mesh = Object.Instantiate<Mesh>(meshFilter.sharedMesh);
			meshFilter.mesh = MeshUtils.AdjustMeshToTerrain(mesh, obj.transform, ofs, terrain, water);
		}
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x00035C48 File Offset: 0x00033E48
	public static Mesh CreateCircle(float radius, float thickness, int vertices)
	{
		List<Vector3> list = new List<Vector3>();
		float num = 0f;
		float num2 = 6.2831855f / (float)vertices;
		for (int i = 0; i <= vertices; i++)
		{
			float num3 = Mathf.Sin(num);
			float num4 = Mathf.Cos(num);
			Vector3 item = new Vector3(num4 * radius, 0f, num3 * radius);
			list.Add(item);
			num += num2;
		}
		return MeshUtils.CreateLinesMesh(list, thickness, 2, true, false, null, null);
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x00035CB4 File Offset: 0x00033EB4
	public static MeshRenderer CreateSelectionCircle(GameObject obj, float radius, Material mat, float selection_thickness = 0.25f)
	{
		List<Vector3> list = new List<Vector3>();
		radius += selection_thickness;
		float num = 0f;
		float num2 = 0.19634955f;
		for (int i = 0; i <= 32; i++)
		{
			float num3 = Mathf.Sin(num);
			float num4 = Mathf.Cos(num);
			Vector3 item = new Vector3(num4 * radius, 0f, num3 * radius);
			list.Add(item);
			num += num2;
		}
		GameObject gameObject = MeshUtils.CreateLinesObject(mat, list, selection_thickness, -1f, false, 2, true, false, null);
		gameObject.name = "Selection";
		gameObject.layer = obj.layer;
		gameObject.transform.SetParent(obj.transform, false);
		return gameObject.GetComponent<MeshRenderer>();
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x00035D55 File Offset: 0x00033F55
	public static MeshRenderer CreateRelectionDisc(GameObject obj, float radius, Material mat)
	{
		GameObject gameObject = MeshUtils.CreateDiscObject(mat, radius, 24, -0.12f, 2);
		gameObject.name = "Relation";
		gameObject.layer = obj.layer;
		gameObject.transform.SetParent(obj.transform, false);
		return gameObject.GetComponent<MeshRenderer>();
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x00035D94 File Offset: 0x00033F94
	public static void SnapSelectionToTerrain(MeshRenderer selection, Terrain terrain = null)
	{
		if (selection == null)
		{
			return;
		}
		MeshFilter component = selection.GetComponent<MeshFilter>();
		if (component == null)
		{
			return;
		}
		MeshUtils.SnapMeshToTerrain(component.sharedMesh, selection.transform, 0.1f, false, terrain);
	}
}

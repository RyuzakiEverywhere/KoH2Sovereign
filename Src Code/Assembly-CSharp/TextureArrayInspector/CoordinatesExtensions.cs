using System;
using System.Collections.Generic;
using UnityEngine;

namespace TextureArrayInspector
{
	// Token: 0x0200047E RID: 1150
	public static class CoordinatesExtensions
	{
		// Token: 0x06003BF8 RID: 15352 RVA: 0x001C95F4 File Offset: 0x001C77F4
		public static bool InRange(this Rect rect, Vector2 pos)
		{
			return (rect.center - pos).sqrMagnitude < rect.width / 2f * (rect.width / 2f);
		}

		// Token: 0x06003BF9 RID: 15353 RVA: 0x001C9633 File Offset: 0x001C7833
		public static Vector3 ToDir(this float angle)
		{
			return new Vector3(Mathf.Sin(angle * 0.017453292f), 0f, Mathf.Cos(angle * 0.017453292f));
		}

		// Token: 0x06003BFA RID: 15354 RVA: 0x001C9657 File Offset: 0x001C7857
		public static float ToAngle(this Vector3 dir)
		{
			return Mathf.Atan2(dir.x, dir.z) * 57.29578f;
		}

		// Token: 0x06003BFB RID: 15355 RVA: 0x001C9670 File Offset: 0x001C7870
		public static Vector3 V3(this Vector2 v2)
		{
			return new Vector3(v2.x, 0f, v2.y);
		}

		// Token: 0x06003BFC RID: 15356 RVA: 0x00026090 File Offset: 0x00024290
		public static Vector2 V2(this Vector3 v3)
		{
			return new Vector2(v3.x, v3.z);
		}

		// Token: 0x06003BFD RID: 15357 RVA: 0x001C9688 File Offset: 0x001C7888
		public static Vector3 ToV3(this float f)
		{
			return new Vector3(f, f, f);
		}

		// Token: 0x06003BFE RID: 15358 RVA: 0x001C9694 File Offset: 0x001C7894
		public static Quaternion EulerToQuat(this Vector3 v)
		{
			Quaternion identity = Quaternion.identity;
			identity.eulerAngles = v;
			return identity;
		}

		// Token: 0x06003BFF RID: 15359 RVA: 0x001C96B0 File Offset: 0x001C78B0
		public static Quaternion EulerToQuat(this float f)
		{
			Quaternion identity = Quaternion.identity;
			identity.eulerAngles = new Vector3(0f, f, 0f);
			return identity;
		}

		// Token: 0x06003C00 RID: 15360 RVA: 0x001C9633 File Offset: 0x001C7833
		public static Vector3 Direction(this float angle)
		{
			return new Vector3(Mathf.Sin(angle * 0.017453292f), 0f, Mathf.Cos(angle * 0.017453292f));
		}

		// Token: 0x06003C01 RID: 15361 RVA: 0x001C9657 File Offset: 0x001C7857
		public static float Angle(this Vector3 dir)
		{
			return Mathf.Atan2(dir.x, dir.z) * 57.29578f;
		}

		// Token: 0x06003C02 RID: 15362 RVA: 0x001C96DB File Offset: 0x001C78DB
		public static Rect Clamp(this Rect r, float p)
		{
			return new Rect(r.x, r.y, r.width * p, r.height);
		}

		// Token: 0x06003C03 RID: 15363 RVA: 0x001C9700 File Offset: 0x001C7900
		public static Rect ClampFromLeft(this Rect r, float p)
		{
			return new Rect(r.x + r.width * (1f - p), r.y, r.width * p, r.height);
		}

		// Token: 0x06003C04 RID: 15364 RVA: 0x001C9735 File Offset: 0x001C7935
		public static Rect Clamp(this Rect r, int p)
		{
			return new Rect(r.x, r.y, (float)p, r.height);
		}

		// Token: 0x06003C05 RID: 15365 RVA: 0x001C9753 File Offset: 0x001C7953
		public static Rect ClampFromLeft(this Rect r, int p)
		{
			return new Rect(r.x + (r.width - (float)p), r.y, (float)p, r.height);
		}

		// Token: 0x06003C06 RID: 15366 RVA: 0x001C977C File Offset: 0x001C797C
		public static Rect Intersect(Rect r1, Rect r2)
		{
			Rect result = new Rect(0f, 0f, 0f, 0f);
			result.x = Mathf.Max(r1.x, r2.x);
			result.y = Mathf.Max(r1.y, r2.y);
			result.max = new Vector2(Mathf.Min(r1.max.x, r2.max.x), Mathf.Min(r1.max.y, r2.max.y));
			if (result.size.x < 0f)
			{
				result.size = new Vector2(0f, result.size.y);
			}
			if (result.size.y < 0f)
			{
				result.size = new Vector2(result.size.y, 0f);
			}
			return result;
		}

		// Token: 0x06003C07 RID: 15367 RVA: 0x001C9880 File Offset: 0x001C7A80
		public static Rect Intersect(Rect r1, CoordRect r2)
		{
			Rect result = new Rect(0f, 0f, 0f, 0f);
			result.x = Mathf.Max(r1.x, (float)r2.offset.x);
			result.y = Mathf.Max(r1.y, (float)r2.offset.z);
			result.max = new Vector2(Mathf.Min(r1.max.x, (float)(r2.offset.x + r2.size.x)), Mathf.Min(r1.max.y, (float)(r2.offset.z + r2.size.z)));
			if (result.size.x < 0f)
			{
				result.size = new Vector2(0f, result.size.y);
			}
			if (result.size.y < 0f)
			{
				result.size = new Vector2(result.size.y, 0f);
			}
			return result;
		}

		// Token: 0x06003C08 RID: 15368 RVA: 0x001C99A5 File Offset: 0x001C7BA5
		public static Rect ToRect(this Vector3 center, float range)
		{
			return new Rect(center.x - range, center.z - range, range * 2f, range * 2f);
		}

		// Token: 0x06003C09 RID: 15369 RVA: 0x001C99CC File Offset: 0x001C7BCC
		public static Vector3 Average(this Vector3[] vecs)
		{
			Vector3 a = Vector3.zero;
			for (int i = 0; i < vecs.Length; i++)
			{
				a += vecs[i];
			}
			return a / (float)vecs.Length;
		}

		// Token: 0x06003C0A RID: 15370 RVA: 0x001C9A08 File Offset: 0x001C7C08
		public static bool Intersects(this Rect r1, Rect r2)
		{
			Vector2 min = r1.min;
			Vector2 max = r1.max;
			Vector2 min2 = r2.min;
			Vector2 max2 = r2.max;
			return max2.x >= min.x && min2.x <= max.x && max2.y >= min.y && min2.y <= max.y;
		}

		// Token: 0x06003C0B RID: 15371 RVA: 0x001C9A70 File Offset: 0x001C7C70
		public static bool Intersects(this Rect r1, Rect[] rects)
		{
			for (int i = 0; i < rects.Length; i++)
			{
				if (r1.Intersects(rects[i]))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003C0C RID: 15372 RVA: 0x001C9AA0 File Offset: 0x001C7CA0
		public static bool Contains(this Rect r1, Rect r2)
		{
			Vector2 min = r1.min;
			Vector2 max = r1.max;
			Vector2 min2 = r2.min;
			Vector2 max2 = r2.max;
			return min2.x > min.x && max2.x < max.x && min2.y > min.y && max2.y < max.y;
		}

		// Token: 0x06003C0D RID: 15373 RVA: 0x001C9B08 File Offset: 0x001C7D08
		public static Rect Extend(this Rect r, float f)
		{
			return new Rect(r.x - f, r.y - f, r.width + f * 2f, r.height + f * 2f);
		}

		// Token: 0x06003C0E RID: 15374 RVA: 0x001C9B40 File Offset: 0x001C7D40
		public static float DistToRectCenter(this Vector3 pos, float offsetX, float offsetZ, float size)
		{
			float num = pos.x - (offsetX + size / 2f);
			float num2 = pos.z - (offsetZ + size / 2f);
			return Mathf.Sqrt(num * num + num2 * num2);
		}

		// Token: 0x06003C0F RID: 15375 RVA: 0x001C9B7C File Offset: 0x001C7D7C
		public static float DistToRectAxisAligned(this Vector3 pos, float offsetX, float offsetZ, float size)
		{
			float num = offsetX - pos.x;
			float num2 = pos.x - offsetX - size;
			float num3;
			if (num >= 0f)
			{
				num3 = num;
			}
			else if (num2 >= 0f)
			{
				num3 = num2;
			}
			else
			{
				num3 = 0f;
			}
			float num4 = offsetZ - pos.z;
			float num5 = pos.z - offsetZ - size;
			float num6;
			if (num4 >= 0f)
			{
				num6 = num4;
			}
			else if (num5 >= 0f)
			{
				num6 = num5;
			}
			else
			{
				num6 = 0f;
			}
			if (num3 > num6)
			{
				return num3;
			}
			return num6;
		}

		// Token: 0x06003C10 RID: 15376 RVA: 0x001C9BFC File Offset: 0x001C7DFC
		public static float DistToRectCenter(this Vector3[] poses, float offsetX, float offsetZ, float size)
		{
			float num = 200000000f;
			for (int i = 0; i < poses.Length; i++)
			{
				float num2 = poses[i].DistToRectCenter(offsetX, offsetZ, size);
				if (num2 < num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x06003C11 RID: 15377 RVA: 0x001C9C34 File Offset: 0x001C7E34
		public static float DistToRectAxisAligned(this Vector3[] poses, float offsetX, float offsetZ, float size)
		{
			float num = 200000000f;
			for (int i = 0; i < poses.Length; i++)
			{
				float num2 = poses[i].DistToRectAxisAligned(offsetX, offsetZ, size);
				if (num2 < num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x06003C12 RID: 15378 RVA: 0x001C9C6C File Offset: 0x001C7E6C
		public static float DistAxisAligned(this Vector3 center, Vector3 pos)
		{
			float num = center.x - pos.x;
			if (num < 0f)
			{
				num = -num;
			}
			float num2 = center.z - pos.z;
			if (num2 < 0f)
			{
				num2 = -num2;
			}
			if (num > num2)
			{
				return num;
			}
			return num2;
		}

		// Token: 0x06003C13 RID: 15379 RVA: 0x001C9CB4 File Offset: 0x001C7EB4
		public static Coord RoundToCoord(this Vector2 pos)
		{
			int num = (int)(pos.x + 0.5f);
			if (pos.x < 0f)
			{
				num--;
			}
			int num2 = (int)(pos.y + 0.5f);
			if (pos.y < 0f)
			{
				num2--;
			}
			return new Coord(num, num2);
		}

		// Token: 0x06003C14 RID: 15380 RVA: 0x001C9D06 File Offset: 0x001C7F06
		public static Coord FloorToCoord(this Vector3 pos, float cellSize)
		{
			return new Coord(Mathf.FloorToInt(pos.x / cellSize), Mathf.FloorToInt(pos.z / cellSize));
		}

		// Token: 0x06003C15 RID: 15381 RVA: 0x001C9D27 File Offset: 0x001C7F27
		public static Coord CeilToCoord(this Vector3 pos, float cellSize)
		{
			return new Coord(Mathf.CeilToInt(pos.x / cellSize), Mathf.CeilToInt(pos.z / cellSize));
		}

		// Token: 0x06003C16 RID: 15382 RVA: 0x001C9D48 File Offset: 0x001C7F48
		public static Coord RoundToCoord(this Vector3 pos, float cellSize)
		{
			return new Coord(Mathf.RoundToInt(pos.x / cellSize), Mathf.RoundToInt(pos.z / cellSize));
		}

		// Token: 0x06003C17 RID: 15383 RVA: 0x001C9D6C File Offset: 0x001C7F6C
		public static CoordRect ToCoordRect(this Vector3 pos, float range, float cellSize)
		{
			Coord coord = new Coord(Mathf.FloorToInt((pos.x - range) / cellSize), Mathf.FloorToInt((pos.z - range) / cellSize));
			Coord c = new Coord(Mathf.FloorToInt((pos.x + range) / cellSize), Mathf.FloorToInt((pos.z + range) / cellSize)) + 1;
			return new CoordRect(coord, c - coord);
		}

		// Token: 0x06003C18 RID: 15384 RVA: 0x001C9DD8 File Offset: 0x001C7FD8
		public static CoordRect ToFixedSizeCoordRect(this Vector3 pos, float range, float cellSize)
		{
			Coord coord = (Vector3.one * range * 2f).CeilToCoord(cellSize) + 1;
			return new CoordRect(pos.RoundToCoord(cellSize) - coord / 2, coord);
		}

		// Token: 0x06003C19 RID: 15385 RVA: 0x001C9E20 File Offset: 0x001C8020
		public static CoordRect GetHeightRect(this Terrain terrain)
		{
			float num = terrain.terrainData.size.x / (float)terrain.terrainData.heightmapResolution;
			int num2 = (int)(terrain.transform.localPosition.x / num + 0.5f);
			if (terrain.transform.localPosition.x < 0f)
			{
				num2--;
			}
			int num3 = (int)(terrain.transform.localPosition.z / num + 0.5f);
			if (terrain.transform.localPosition.z < 0f)
			{
				num3--;
			}
			return new CoordRect(num2, num3, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
		}

		// Token: 0x06003C1A RID: 15386 RVA: 0x001C9ED4 File Offset: 0x001C80D4
		public static CoordRect GetSplatRect(this Terrain terrain)
		{
			float num = terrain.terrainData.size.x / (float)terrain.terrainData.alphamapResolution;
			int num2 = (int)(terrain.transform.localPosition.x / num + 0.5f);
			if (terrain.transform.localPosition.x < 0f)
			{
				num2--;
			}
			int num3 = (int)(terrain.transform.localPosition.z / num + 0.5f);
			if (terrain.transform.localPosition.z < 0f)
			{
				num3--;
			}
			return new CoordRect(num2, num3, terrain.terrainData.alphamapResolution, terrain.terrainData.alphamapResolution);
		}

		// Token: 0x06003C1B RID: 15387 RVA: 0x001C9F88 File Offset: 0x001C8188
		public static float[,] SafeGetHeights(this TerrainData data, int offsetX, int offsetZ, int sizeX, int sizeZ)
		{
			if (offsetX < 0)
			{
				sizeX += offsetX;
				offsetX = 0;
			}
			if (offsetZ < 0)
			{
				sizeZ += offsetZ;
				offsetZ = 0;
			}
			int heightmapResolution = data.heightmapResolution;
			if (sizeX + offsetX > heightmapResolution)
			{
				sizeX = heightmapResolution - offsetX;
			}
			if (sizeZ + offsetZ > heightmapResolution)
			{
				sizeZ = heightmapResolution - offsetZ;
			}
			return data.GetHeights(offsetX, offsetZ, sizeX, sizeZ);
		}

		// Token: 0x06003C1C RID: 15388 RVA: 0x001C9FD8 File Offset: 0x001C81D8
		public static float[,,] SafeGetAlphamaps(this TerrainData data, int offsetX, int offsetZ, int sizeX, int sizeZ)
		{
			if (offsetX < 0)
			{
				sizeX += offsetX;
				offsetX = 0;
			}
			if (offsetZ < 0)
			{
				sizeZ += offsetZ;
				offsetZ = 0;
			}
			int alphamapResolution = data.alphamapResolution;
			if (sizeX + offsetX > alphamapResolution)
			{
				sizeX = alphamapResolution - offsetX;
			}
			if (sizeZ + offsetZ > alphamapResolution)
			{
				sizeZ = alphamapResolution - offsetZ;
			}
			return data.GetAlphamaps(offsetX, offsetZ, sizeX, sizeZ);
		}

		// Token: 0x06003C1D RID: 15389 RVA: 0x001CA028 File Offset: 0x001C8228
		public static float GetInterpolated(this float[,] array, float x, float z)
		{
			int num = (int)x;
			if (x < 0f)
			{
				num--;
			}
			int num2 = num + 1;
			int num3 = (int)z;
			if (z < 0f)
			{
				num3--;
			}
			int num4 = num3 + 1;
			float num5 = array[num, num3];
			float num6 = array[num, num4];
			float num7 = array[num2, num3];
			float num8 = array[num2, num4];
			float num9 = x - (float)num;
			float num10 = z - (float)num3;
			float num11 = num5 * (1f - num9) + num6 * num9;
			float num12 = num7 * (1f - num9) + num8 * num9;
			return num11 * (1f - num10) + num12 * num10;
		}

		// Token: 0x06003C1E RID: 15390 RVA: 0x001CA0BE File Offset: 0x001C82BE
		public static bool Equal(Vector3 v1, Vector3 v2)
		{
			return Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.y, v2.y) && Mathf.Approximately(v1.z, v2.z);
		}

		// Token: 0x06003C1F RID: 15391 RVA: 0x001CA0F9 File Offset: 0x001C82F9
		public static bool Equal(Ray r1, Ray r2)
		{
			return CoordinatesExtensions.Equal(r1.origin, r2.origin) && CoordinatesExtensions.Equal(r1.direction, r2.direction);
		}

		// Token: 0x06003C20 RID: 15392 RVA: 0x001CA128 File Offset: 0x001C8328
		public static Vector3[] InverseTransformPoint(this Transform tfm, Vector3[] points)
		{
			for (int i = 0; i < points.Length; i++)
			{
				points[i] = tfm.InverseTransformPoint(points[i]);
			}
			return points;
		}

		// Token: 0x06003C21 RID: 15393 RVA: 0x001CA158 File Offset: 0x001C8358
		public static Vector3 GetCenter(this Vector3[] poses)
		{
			if (poses.Length == 0)
			{
				return default(Vector3);
			}
			if (poses.Length == 1)
			{
				return poses[0];
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < poses.Length; i++)
			{
				num += poses[i].x;
				num2 += poses[i].y;
				num3 += poses[i].z;
			}
			return new Vector3(num / (float)poses.Length, num2 / (float)poses.Length, num3 / (float)poses.Length);
		}

		// Token: 0x06003C22 RID: 15394 RVA: 0x001CA1EC File Offset: 0x001C83EC
		public static bool Approximately(Rect r1, Rect r2)
		{
			return Mathf.Approximately(r1.x, r2.x) && Mathf.Approximately(r1.y, r2.y) && Mathf.Approximately(r1.width, r2.width) && Mathf.Approximately(r1.height, r2.height);
		}

		// Token: 0x06003C23 RID: 15395 RVA: 0x001CA24D File Offset: 0x001C844D
		public static IEnumerable<Vector3> CircleAround(this Vector3 center, float radius, int numPoints, bool endWhereStart = false)
		{
			float radianStep = 6.2831855f / (float)numPoints;
			int num;
			if (endWhereStart)
			{
				num = numPoints;
				numPoints = num + 1;
			}
			for (int i = 0; i < numPoints; i = num + 1)
			{
				float f = (float)i * radianStep;
				Vector3 a = new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f));
				yield return center + a * radius;
				num = i;
			}
			yield break;
		}
	}
}

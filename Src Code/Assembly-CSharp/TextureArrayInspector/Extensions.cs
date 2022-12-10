using System;
using System.Collections.Generic;
using UnityEngine;

namespace TextureArrayInspector
{
	// Token: 0x0200047F RID: 1151
	public static class Extensions
	{
		// Token: 0x06003C24 RID: 15396 RVA: 0x001CA274 File Offset: 0x001C8474
		public static void RemoveChildren(this Transform tfm)
		{
			for (int i = tfm.childCount - 1; i >= 0; i--)
			{
				Object.DestroyImmediate(tfm.GetChild(i).gameObject);
			}
		}

		// Token: 0x06003C25 RID: 15397 RVA: 0x001CA2A8 File Offset: 0x001C84A8
		public static Transform FindChildRecursive(this Transform tfm, string name)
		{
			int childCount = tfm.childCount;
			for (int i = 0; i < childCount; i++)
			{
				if (tfm.GetChild(i).name == name)
				{
					return tfm.GetChild(i);
				}
			}
			for (int j = 0; j < childCount; j++)
			{
				Transform transform = tfm.GetChild(j).FindChildRecursive(name);
				if (transform != null)
				{
					return transform;
				}
			}
			return null;
		}

		// Token: 0x06003C26 RID: 15398 RVA: 0x000023FD File Offset: 0x000005FD
		public static void ToggleDisplayWireframe(this Transform tfm, bool show)
		{
		}

		// Token: 0x06003C27 RID: 15399 RVA: 0x001CA30C File Offset: 0x001C850C
		public static int ToInt(this Coord coord)
		{
			int num = (coord.x < 0) ? (-coord.x) : coord.x;
			int num2 = (coord.z < 0) ? (-coord.z) : coord.z;
			return (((coord.z < 0) ? 1000000000 : 0) + num * 30000 + num2) * ((coord.x < 0) ? -1 : 1);
		}

		// Token: 0x06003C28 RID: 15400 RVA: 0x001CA374 File Offset: 0x001C8574
		public static Coord ToCoord(this int hash)
		{
			int num = (hash < 0) ? (-hash) : hash;
			int num2 = num / 1000000000 * 1000000000;
			int num3 = (num - num2) / 30000;
			int num4 = num - num2 - num3 * 30000;
			return new Coord((hash < 0) ? (-num3) : num3, (num2 == 0) ? num4 : (-num4));
		}

		// Token: 0x06003C29 RID: 15401 RVA: 0x001CA3C3 File Offset: 0x001C85C3
		public static void CheckAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value, bool replace = true)
		{
			if (dict.ContainsKey(key))
			{
				if (replace)
				{
					dict[key] = value;
					return;
				}
			}
			else
			{
				dict.Add(key, value);
			}
		}

		// Token: 0x06003C2A RID: 15402 RVA: 0x001CA3E2 File Offset: 0x001C85E2
		public static void CheckRemove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
		{
			if (dict.ContainsKey(key))
			{
				dict.Remove(key);
			}
		}

		// Token: 0x06003C2B RID: 15403 RVA: 0x001CA3F8 File Offset: 0x001C85F8
		public static TValue CheckGet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
		{
			if (dict.ContainsKey(key))
			{
				return dict[key];
			}
			return default(TValue);
		}

		// Token: 0x06003C2C RID: 15404 RVA: 0x001CA420 File Offset: 0x001C8620
		public static TKey AnyKey<TKey, TValue>(this Dictionary<TKey, TValue> dict)
		{
			using (Dictionary<TKey, TValue>.Enumerator enumerator = dict.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					KeyValuePair<TKey, TValue> keyValuePair = enumerator.Current;
					return keyValuePair.Key;
				}
			}
			return default(TKey);
		}

		// Token: 0x06003C2D RID: 15405 RVA: 0x001CA47C File Offset: 0x001C867C
		public static TValue AnyValue<TKey, TValue>(this Dictionary<TKey, TValue> dict)
		{
			using (Dictionary<TKey, TValue>.Enumerator enumerator = dict.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					KeyValuePair<TKey, TValue> keyValuePair = enumerator.Current;
					return keyValuePair.Value;
				}
			}
			return default(TValue);
		}

		// Token: 0x06003C2E RID: 15406 RVA: 0x001CA4D8 File Offset: 0x001C86D8
		public static T Any<T>(this HashSet<T> hashSet)
		{
			using (HashSet<T>.Enumerator enumerator = hashSet.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			return default(T);
		}

		// Token: 0x06003C2F RID: 15407 RVA: 0x001CA52C File Offset: 0x001C872C
		public static void CheckAdd<T>(this HashSet<T> set, T obj)
		{
			if (!set.Contains(obj))
			{
				set.Add(obj);
			}
		}

		// Token: 0x06003C30 RID: 15408 RVA: 0x001CA53F File Offset: 0x001C873F
		public static void CheckRemove<T>(this HashSet<T> set, T obj)
		{
			if (set.Contains(obj))
			{
				set.Remove(obj);
			}
		}

		// Token: 0x06003C31 RID: 15409 RVA: 0x001CA552 File Offset: 0x001C8752
		public static void SetState<T>(this HashSet<T> set, T obj, bool state)
		{
			if (state && !set.Contains(obj))
			{
				set.Add(obj);
			}
			if (!state && set.Contains(obj))
			{
				set.Remove(obj);
			}
		}

		// Token: 0x06003C32 RID: 15410 RVA: 0x001CA57C File Offset: 0x001C877C
		public static void Normalize(this float[,,] array, int pinnedLayer)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			int length3 = array.GetLength(2);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					float num = 0f;
					for (int k = 0; k < length3; k++)
					{
						if (k != pinnedLayer)
						{
							num += array[i, j, k];
						}
					}
					float num2 = array[i, j, pinnedLayer];
					if (num2 > 1f)
					{
						num2 = 1f;
						array[i, j, pinnedLayer] = 1f;
					}
					if (num2 < 0f)
					{
						num2 = 0f;
						array[i, j, pinnedLayer] = 0f;
					}
					float num3 = 1f - num2;
					float num4 = (num > 0f) ? (num3 / num) : 0f;
					for (int l = 0; l < length3; l++)
					{
						if (l != pinnedLayer)
						{
							array[i, j, l] *= num4;
						}
					}
				}
			}
		}

		// Token: 0x06003C33 RID: 15411 RVA: 0x001CA688 File Offset: 0x001C8888
		public static void DrawDebug(this Vector3 pos, float range = 1f, Color color = default(Color))
		{
			if (color.a < 0.001f)
			{
				color = Color.white;
			}
			Debug.DrawLine(pos + new Vector3(-1f, 0f, 1f) * range, pos + new Vector3(1f, 0f, 1f) * range, color);
			Debug.DrawLine(pos + new Vector3(1f, 0f, 1f) * range, pos + new Vector3(1f, 0f, -1f) * range, color);
			Debug.DrawLine(pos + new Vector3(1f, 0f, -1f) * range, pos + new Vector3(-1f, 0f, -1f) * range, color);
			Debug.DrawLine(pos + new Vector3(-1f, 0f, -1f) * range, pos + new Vector3(-1f, 0f, 1f) * range, color);
		}

		// Token: 0x06003C34 RID: 15412 RVA: 0x001CA7C4 File Offset: 0x001C89C4
		public static void DrawDebug(this Rect rect, Color color = default(Color))
		{
			if (color.a < 0.001f)
			{
				color = Color.white;
			}
			Debug.DrawLine(new Vector3(rect.x, 0f, rect.y), new Vector3(rect.x + rect.width, 0f, rect.y), color);
			Debug.DrawLine(new Vector3(rect.x + rect.width, 0f, rect.y), new Vector3(rect.x + rect.width, 0f, rect.y + rect.height), color);
			Debug.DrawLine(new Vector3(rect.x + rect.width, 0f, rect.y + rect.height), new Vector3(rect.x, 0f, rect.y + rect.height), color);
			Debug.DrawLine(new Vector3(rect.x, 0f, rect.y + rect.height), new Vector3(rect.x, 0f, rect.y), color);
		}

		// Token: 0x06003C35 RID: 15413 RVA: 0x001CA900 File Offset: 0x001C8B00
		public static void Resize(this Terrain terrain, int resolution, Vector3 size)
		{
			if ((terrain.terrainData.size - size).sqrMagnitude > 0.01f || terrain.terrainData.heightmapResolution != resolution)
			{
				if (resolution <= 64)
				{
					terrain.terrainData.heightmapResolution = resolution;
					terrain.terrainData.size = new Vector3(size.x, size.y, size.z);
					return;
				}
				terrain.terrainData.heightmapResolution = 65;
				terrain.Flush();
				int num = (resolution - 1) / 64;
				terrain.terrainData.size = new Vector3(size.x / (float)num, size.y, size.z / (float)num);
				terrain.terrainData.heightmapResolution = resolution;
			}
		}

		// Token: 0x06003C36 RID: 15414 RVA: 0x001CA9C0 File Offset: 0x001C8BC0
		public static Transform AddChild(this Transform tfm, string name = "", Vector3 offset = default(Vector3))
		{
			return new GameObject
			{
				name = name,
				transform = 
				{
					parent = tfm,
					localPosition = offset
				}
			}.transform;
		}

		// Token: 0x06003C37 RID: 15415 RVA: 0x001CA9EC File Offset: 0x001C8BEC
		public static T CreateObjectWithComponent<T>(string name = "", Transform parent = null, Vector3 offset = default(Vector3)) where T : MonoBehaviour
		{
			GameObject gameObject = new GameObject();
			if (name != null && parent != null)
			{
				gameObject.transform.parent = parent.transform;
			}
			gameObject.transform.localPosition = offset;
			return gameObject.AddComponent<T>();
		}

		// Token: 0x06003C38 RID: 15416 RVA: 0x001CAA30 File Offset: 0x001C8C30
		public static float EvaluateMultithreaded(this AnimationCurve curve, float time)
		{
			int num = curve.keys.Length;
			if (time <= curve.keys[0].time)
			{
				return curve.keys[0].value;
			}
			if (time >= curve.keys[num - 1].time)
			{
				return curve.keys[num - 1].value;
			}
			int num2 = 0;
			int num3 = 0;
			while (num3 < num - 1 && curve.keys[num2 + 1].time <= time)
			{
				num2++;
				num3++;
			}
			float num4 = curve.keys[num2 + 1].time - curve.keys[num2].time;
			float num5 = (time - curve.keys[num2].time) / num4;
			float num6 = num5 * num5;
			float num7 = num6 * num5;
			float num8 = 2f * num7 - 3f * num6 + 1f;
			float num9 = num7 - 2f * num6 + num5;
			float num10 = num7 - num6;
			float num11 = -2f * num7 + 3f * num6;
			return num8 * curve.keys[num2].value + num9 * curve.keys[num2].outTangent * num4 + num10 * curve.keys[num2 + 1].inTangent * num4 + num11 * curve.keys[num2 + 1].value;
		}

		// Token: 0x06003C39 RID: 15417 RVA: 0x001CABA4 File Offset: 0x001C8DA4
		public static bool IdenticalTo(this AnimationCurve c1, AnimationCurve c2)
		{
			if (c1 == null || c2 == null)
			{
				return false;
			}
			if (c1.keys.Length != c2.keys.Length)
			{
				return false;
			}
			int num = c1.keys.Length;
			for (int i = 0; i < num; i++)
			{
				if (c1.keys[i].time != c2.keys[i].time || c1.keys[i].value != c2.keys[i].value || c1.keys[i].inTangent != c2.keys[i].inTangent || c1.keys[i].outTangent != c2.keys[i].outTangent)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06003C3A RID: 15418 RVA: 0x001CAC7C File Offset: 0x001C8E7C
		public static Keyframe[] Copy(this Keyframe[] src)
		{
			Keyframe[] array = new Keyframe[src.Length];
			for (int i = 0; i < src.Length; i++)
			{
				array[i].value = src[i].value;
				array[i].time = src[i].time;
				array[i].inTangent = src[i].inTangent;
				array[i].outTangent = src[i].outTangent;
			}
			return array;
		}

		// Token: 0x06003C3B RID: 15419 RVA: 0x001CAD01 File Offset: 0x001C8F01
		public static AnimationCurve Copy(this AnimationCurve src)
		{
			return new AnimationCurve
			{
				keys = src.keys.Copy()
			};
		}

		// Token: 0x06003C3C RID: 15420 RVA: 0x001CAD1C File Offset: 0x001C8F1C
		public static object Parse(this string s, Type t)
		{
			if (s.Contains("="))
			{
				s = s.Remove(0, s.IndexOf('=') + 1);
			}
			object result;
			if (t == typeof(float))
			{
				result = float.Parse(s);
			}
			else if (t == typeof(int))
			{
				result = int.Parse(s);
			}
			else if (t == typeof(bool))
			{
				result = bool.Parse(s);
			}
			else if (t == typeof(string))
			{
				result = s;
			}
			else if (t == typeof(byte))
			{
				result = byte.Parse(s);
			}
			else if (t == typeof(short))
			{
				result = short.Parse(s);
			}
			else if (t == typeof(long))
			{
				result = long.Parse(s);
			}
			else if (t == typeof(double))
			{
				result = double.Parse(s);
			}
			else if (t == typeof(char))
			{
				result = char.Parse(s);
			}
			else if (t == typeof(decimal))
			{
				result = decimal.Parse(s);
			}
			else if (t == typeof(sbyte))
			{
				result = sbyte.Parse(s);
			}
			else if (t == typeof(uint))
			{
				result = uint.Parse(s);
			}
			else
			{
				if (!(t == typeof(ulong)))
				{
					return null;
				}
				result = ulong.Parse(s);
			}
			return result;
		}

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x06003C3D RID: 15421 RVA: 0x0002C53B File Offset: 0x0002A73B
		public static bool isPlaying
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003C3E RID: 15422 RVA: 0x0002C538 File Offset: 0x0002A738
		public static bool IsEditor()
		{
			return false;
		}

		// Token: 0x06003C3F RID: 15423 RVA: 0x0002C538 File Offset: 0x0002A738
		public static bool IsSelected(Transform transform)
		{
			return false;
		}

		// Token: 0x06003C40 RID: 15424 RVA: 0x001CAF00 File Offset: 0x001C9100
		public static Camera GetMainCamera()
		{
			if (Extensions.IsEditor())
			{
				return null;
			}
			Camera camera = Camera.main;
			if (camera == null)
			{
				camera = Object.FindObjectOfType<Camera>();
			}
			return camera;
		}

		// Token: 0x06003C41 RID: 15425 RVA: 0x001CAF2C File Offset: 0x001C912C
		public static Vector3[] GetCamPoses(bool genAroundMainCam = true, string genAroundTag = null, Vector3[] camPoses = null)
		{
			if (Extensions.IsEditor())
			{
				camPoses = new Vector3[1];
			}
			else
			{
				GameObject[] array = null;
				if (genAroundTag != null && genAroundTag.Length != 0)
				{
					array = GameObject.FindGameObjectsWithTag(genAroundTag);
				}
				int num = 0;
				if (genAroundMainCam)
				{
					num++;
				}
				if (array != null)
				{
					num += array.Length;
				}
				if (num == 0)
				{
					Debug.LogError("No Main Camera to deploy");
					return new Vector3[0];
				}
				if (camPoses == null || num != camPoses.Length)
				{
					camPoses = new Vector3[num];
				}
				int num2 = 0;
				if (genAroundMainCam)
				{
					Camera camera = Camera.main;
					if (camera == null)
					{
						camera = Object.FindObjectOfType<Camera>();
					}
					camPoses[0] = camera.transform.position;
					num2++;
				}
				if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						camPoses[i + num2] = array[i].transform.position;
					}
				}
			}
			return camPoses;
		}

		// Token: 0x06003C42 RID: 15426 RVA: 0x001CAFF6 File Offset: 0x001C91F6
		public static Vector2 GetMousePosition()
		{
			Extensions.IsEditor();
			return Input.mousePosition;
		}

		// Token: 0x06003C43 RID: 15427 RVA: 0x001CB008 File Offset: 0x001C9208
		public static void GizmosDrawFrame(Vector3 center, Vector3 size, int resolution, float level = 30f)
		{
			Vector3 vector = center - size / 2f;
			Vector3 from = Vector3.zero;
			Vector3 from2 = Vector3.zero;
			for (float num = 0f; num < size.x + 0.0001f; num += 1f * size.x / (float)resolution)
			{
				RaycastHit raycastHit = default(RaycastHit);
				Vector3 vector2 = new Vector3(vector.x + num, 10000f, vector.z);
				if (Physics.Raycast(new Ray(vector2, Vector3.down * 20000f), out raycastHit, 20000f))
				{
					vector2.y = raycastHit.point.y;
				}
				else if (Physics.Raycast(new Ray(vector2 + new Vector3(1f, 0f, 0f), Vector3.down * 20000f), out raycastHit, 20000f))
				{
					vector2.y = raycastHit.point.y;
				}
				else if (Physics.Raycast(new Ray(vector2 + new Vector3(-1f, 0f, 0f), Vector3.down * 20000f), out raycastHit, 20000f))
				{
					vector2.y = raycastHit.point.y;
				}
				else if (Physics.Raycast(new Ray(vector2 + new Vector3(0f, 0f, 1f), Vector3.down * 20000f), out raycastHit, 20000f))
				{
					vector2.y = raycastHit.point.y;
				}
				else if (Physics.Raycast(new Ray(vector2 + new Vector3(0f, 0f, -1f), Vector3.down * 20000f), out raycastHit, 20000f))
				{
					vector2.y = raycastHit.point.y;
				}
				else
				{
					vector2.y = level;
				}
				if (num > 0.0001f)
				{
					Gizmos.DrawLine(from, vector2);
				}
				from = vector2;
				Vector3 vector3 = new Vector3(vector.x + num, 10000f, vector.z + size.z);
				if (Physics.Raycast(new Ray(vector3, Vector3.down * 20000f), out raycastHit, 20000f))
				{
					vector3.y = raycastHit.point.y;
				}
				else if (Physics.Raycast(new Ray(vector3 + new Vector3(1f, 0f, 0f), Vector3.down * 20000f), out raycastHit, 20000f))
				{
					vector3.y = raycastHit.point.y;
				}
				else if (Physics.Raycast(new Ray(vector3 + new Vector3(-1f, 0f, 0f), Vector3.down * 20000f), out raycastHit, 20000f))
				{
					vector3.y = raycastHit.point.y;
				}
				else if (Physics.Raycast(new Ray(vector3 + new Vector3(0f, 0f, 1f), Vector3.down * 20000f), out raycastHit, 20000f))
				{
					vector3.y = raycastHit.point.y;
				}
				else if (Physics.Raycast(new Ray(vector3 + new Vector3(0f, 0f, -1f), Vector3.down * 20000f), out raycastHit, 20000f))
				{
					vector3.y = raycastHit.point.y;
				}
				else
				{
					vector3.y = level;
				}
				if (num > 0.0001f)
				{
					Gizmos.DrawLine(from2, vector3);
				}
				from2 = vector3;
			}
			for (float num2 = 0f; num2 < size.z + 0.0001f; num2 += 1f * size.z / (float)resolution)
			{
				RaycastHit raycastHit2 = default(RaycastHit);
				Vector3 vector4 = new Vector3(vector.x, 10000f, vector.z + num2);
				if (Physics.Raycast(new Ray(vector4, Vector3.down * 20000f), out raycastHit2, 20000f))
				{
					vector4.y = raycastHit2.point.y;
				}
				else if (Physics.Raycast(new Ray(vector4 + new Vector3(1f, 0f, 0f), Vector3.down * 20000f), out raycastHit2, 20000f))
				{
					vector4.y = raycastHit2.point.y;
				}
				else if (Physics.Raycast(new Ray(vector4 + new Vector3(-1f, 0f, 0f), Vector3.down * 20000f), out raycastHit2, 20000f))
				{
					vector4.y = raycastHit2.point.y;
				}
				else if (Physics.Raycast(new Ray(vector4 + new Vector3(0f, 0f, 1f), Vector3.down * 20000f), out raycastHit2, 20000f))
				{
					vector4.y = raycastHit2.point.y;
				}
				else if (Physics.Raycast(new Ray(vector4 + new Vector3(0f, 0f, -1f), Vector3.down * 20000f), out raycastHit2, 20000f))
				{
					vector4.y = raycastHit2.point.y;
				}
				else
				{
					vector4.y = level;
				}
				if (num2 > 0.0001f)
				{
					Gizmos.DrawLine(from, vector4);
				}
				from = vector4;
				Vector3 vector5 = new Vector3(vector.x + size.x, 10000f, vector.z + num2);
				if (Physics.Raycast(new Ray(vector5, Vector3.down * 20000f), out raycastHit2, 20000f))
				{
					vector5.y = raycastHit2.point.y;
				}
				else if (Physics.Raycast(new Ray(vector5 + new Vector3(1f, 0f, 0f), Vector3.down * 20000f), out raycastHit2, 20000f))
				{
					vector5.y = raycastHit2.point.y;
				}
				else if (Physics.Raycast(new Ray(vector5 + new Vector3(-1f, 0f, 0f), Vector3.down * 20000f), out raycastHit2, 20000f))
				{
					vector5.y = raycastHit2.point.y;
				}
				else if (Physics.Raycast(new Ray(vector5 + new Vector3(0f, 0f, 1f), Vector3.down * 20000f), out raycastHit2, 20000f))
				{
					vector5.y = raycastHit2.point.y;
				}
				else if (Physics.Raycast(new Ray(vector5 + new Vector3(0f, 0f, -1f), Vector3.down * 20000f), out raycastHit2, 20000f))
				{
					vector5.y = raycastHit2.point.y;
				}
				else
				{
					vector5.y = level;
				}
				if (num2 > 0.0001f)
				{
					Gizmos.DrawLine(from2, vector5);
				}
				from2 = vector5;
			}
		}

		// Token: 0x06003C44 RID: 15428 RVA: 0x001CB7B4 File Offset: 0x001C99B4
		public static void Planar(this Mesh mesh, float size, int resolution)
		{
			float num = size / (float)resolution;
			Vector3[] array = new Vector3[(resolution + 1) * (resolution + 1)];
			Vector2[] array2 = new Vector2[array.Length];
			int[] array3 = new int[resolution * resolution * 2 * 3];
			int num2 = 0;
			int num3 = 0;
			for (float num4 = 0f; num4 < size + 0.001f; num4 += num)
			{
				for (float num5 = 0f; num5 < size + 0.001f; num5 += num)
				{
					array[num2] = new Vector3(num4, 0f, num5);
					array2[num2] = new Vector2(num4 / size, num5 / size);
					if (num4 > 0.001f && num5 > 0.001f)
					{
						array3[num3] = num2 - (resolution + 1);
						array3[num3 + 1] = num2 - 1;
						array3[num3 + 2] = num2 - resolution - 2;
						array3[num3 + 3] = num2 - 1;
						array3[num3 + 4] = num2 - (resolution + 1);
						array3[num3 + 5] = num2;
						num3 += 6;
					}
					num2++;
				}
			}
			mesh.Clear();
			mesh.vertices = array;
			mesh.uv = array2;
			mesh.triangles = array3;
		}

		// Token: 0x06003C45 RID: 15429 RVA: 0x001CB8D4 File Offset: 0x001C9AD4
		public static string LogBinary(this int src)
		{
			string text = "";
			for (int i = 0; i < 32; i++)
			{
				if (i % 4 == 0)
				{
					text = " " + text;
				}
				text = (src & 1) + text;
				src >>= 1;
			}
			return text;
		}

		// Token: 0x06003C46 RID: 15430 RVA: 0x001CB91C File Offset: 0x001C9B1C
		public static string ToStringArray<T>(this T[] array)
		{
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text += array[i].ToString();
				if (i != array.Length - 1)
				{
					text += ",";
				}
			}
			return text;
		}

		// Token: 0x06003C47 RID: 15431 RVA: 0x001CB970 File Offset: 0x001C9B70
		public static Color[] ToColors(this Vector4[] src)
		{
			Color[] array = new Color[src.Length];
			for (int i = 0; i < src.Length; i++)
			{
				array[i] = src[i];
			}
			return array;
		}

		// Token: 0x06003C48 RID: 15432 RVA: 0x001CB9A8 File Offset: 0x001C9BA8
		public static Texture2D GetBiggestTexture(this Texture2D[] textures)
		{
			int num = 0;
			int num2 = -1;
			for (int i = 0; i < textures.Length; i++)
			{
				if (!(textures[i] == null))
				{
					if (textures[i].width > num)
					{
						num = textures[i].width;
						num2 = i;
					}
					if (textures[i].height > num)
					{
						num = textures[i].height;
						num2 = i;
					}
				}
			}
			if (num2 >= 0)
			{
				return textures[num2];
			}
			return null;
		}

		// Token: 0x06003C49 RID: 15433 RVA: 0x001CBA07 File Offset: 0x001C9C07
		public static void CheckSetInt(this Material mat, string name, int val)
		{
			if (mat.HasProperty(name))
			{
				mat.SetInt(name, val);
			}
		}

		// Token: 0x06003C4A RID: 15434 RVA: 0x001CBA1A File Offset: 0x001C9C1A
		public static void CheckSetFloat(this Material mat, string name, float val)
		{
			if (mat.HasProperty(name))
			{
				mat.SetFloat(name, val);
			}
		}

		// Token: 0x06003C4B RID: 15435 RVA: 0x001CBA2D File Offset: 0x001C9C2D
		public static void CheckSetTexture(this Material mat, string name, Texture tex)
		{
			if (mat.HasProperty(name))
			{
				mat.SetTexture(name, tex);
			}
		}

		// Token: 0x06003C4C RID: 15436 RVA: 0x001CBA40 File Offset: 0x001C9C40
		public static void CheckSetVector(this Material mat, string name, Vector4 val)
		{
			if (mat.HasProperty(name))
			{
				mat.SetVector(name, val);
			}
		}

		// Token: 0x06003C4D RID: 15437 RVA: 0x001CBA53 File Offset: 0x001C9C53
		public static void CheckSetColor(this Material mat, string name, Color val)
		{
			if (mat.HasProperty(name))
			{
				mat.SetColor(name, val);
			}
		}
	}
}

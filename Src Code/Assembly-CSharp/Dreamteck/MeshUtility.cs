using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Dreamteck
{
	// Token: 0x020004A6 RID: 1190
	public class MeshUtility
	{
		// Token: 0x06003E64 RID: 15972 RVA: 0x001DCB80 File Offset: 0x001DAD80
		public static int[] GeneratePlaneTriangles(int x, int z, bool flip, int startTriangleIndex = 0, int startVertex = 0)
		{
			int[] result = new int[x * (z - 1) * 6];
			MeshUtility.GeneratePlaneTriangles(ref result, x, z, flip, 0, 0, false);
			return result;
		}

		// Token: 0x06003E65 RID: 15973 RVA: 0x001DCBAC File Offset: 0x001DADAC
		public static int[] GeneratePlaneTriangles(ref int[] triangles, int x, int z, bool flip, int startTriangleIndex = 0, int startVertex = 0, bool reallocateArray = false)
		{
			int num = x * (z - 1);
			if (reallocateArray && triangles.Length != num * 6)
			{
				if (startTriangleIndex > 0)
				{
					int[] array = new int[startTriangleIndex + num * 6];
					for (int i = 0; i < startTriangleIndex; i++)
					{
						array[i] = triangles[i];
					}
					triangles = array;
				}
				else
				{
					triangles = new int[num * 6];
				}
			}
			int num2 = x + 1;
			int num3 = startTriangleIndex;
			for (int j = 0; j < num + z - 2; j++)
			{
				if ((float)(j + 1) % (float)num2 == 0f && j != 0)
				{
					j++;
				}
				if (flip)
				{
					triangles[num3++] = j + x + 1 + startVertex;
					triangles[num3++] = j + 1 + startVertex;
					triangles[num3++] = j + startVertex;
					triangles[num3++] = j + x + 1 + startVertex;
					triangles[num3++] = j + x + 2 + startVertex;
					triangles[num3++] = j + 1 + startVertex;
				}
				else
				{
					triangles[num3++] = j + startVertex;
					triangles[num3++] = j + 1 + startVertex;
					triangles[num3++] = j + x + 1 + startVertex;
					triangles[num3++] = j + 1 + startVertex;
					triangles[num3++] = j + x + 2 + startVertex;
					triangles[num3++] = j + x + 1 + startVertex;
				}
			}
			return triangles;
		}

		// Token: 0x06003E66 RID: 15974 RVA: 0x001DCD04 File Offset: 0x001DAF04
		public static void CalculateTangents(TS_Mesh mesh)
		{
			int num = mesh.triangles.Length / 3;
			if (mesh.tangents.Length != mesh.vertexCount)
			{
				mesh.tangents = new Vector4[mesh.vertexCount];
			}
			if (MeshUtility.tan1.Length != mesh.vertexCount)
			{
				MeshUtility.tan1 = new Vector3[mesh.vertexCount];
				MeshUtility.tan2 = new Vector3[mesh.vertexCount];
			}
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				int num3 = mesh.triangles[num2];
				int num4 = mesh.triangles[num2 + 1];
				int num5 = mesh.triangles[num2 + 2];
				float num6 = mesh.vertices[num4].x - mesh.vertices[num3].x;
				float num7 = mesh.vertices[num5].x - mesh.vertices[num3].x;
				float num8 = mesh.vertices[num4].y - mesh.vertices[num3].y;
				float num9 = mesh.vertices[num5].y - mesh.vertices[num3].y;
				float num10 = mesh.vertices[num4].z - mesh.vertices[num3].z;
				float num11 = mesh.vertices[num5].z - mesh.vertices[num3].z;
				float num12 = mesh.uv[num4].x - mesh.uv[num3].x;
				float num13 = mesh.uv[num5].x - mesh.uv[num3].x;
				float num14 = mesh.uv[num4].y - mesh.uv[num3].y;
				float num15 = mesh.uv[num5].y - mesh.uv[num3].y;
				float num16 = num12 * num15 - num13 * num14;
				float num17 = (num16 == 0f) ? 0f : (1f / num16);
				Vector3 b = new Vector3((num15 * num6 - num14 * num7) * num17, (num15 * num8 - num14 * num9) * num17, (num15 * num10 - num14 * num11) * num17);
				Vector3 b2 = new Vector3((num12 * num7 - num13 * num6) * num17, (num12 * num9 - num13 * num8) * num17, (num12 * num11 - num13 * num10) * num17);
				MeshUtility.tan1[num3] += b;
				MeshUtility.tan1[num4] += b;
				MeshUtility.tan1[num5] += b;
				MeshUtility.tan2[num3] += b2;
				MeshUtility.tan2[num4] += b2;
				MeshUtility.tan2[num5] += b2;
				num2 += 3;
			}
			for (int j = 0; j < mesh.vertexCount; j++)
			{
				Vector3 lhs = mesh.normals[j];
				Vector3 vector = MeshUtility.tan1[j];
				Vector3.OrthoNormalize(ref lhs, ref vector);
				mesh.tangents[j].x = vector.x;
				mesh.tangents[j].y = vector.y;
				mesh.tangents[j].z = vector.z;
				mesh.tangents[j].w = ((Vector3.Dot(Vector3.Cross(lhs, vector), MeshUtility.tan2[j]) < 0f) ? -1f : 1f);
			}
		}

		// Token: 0x06003E67 RID: 15975 RVA: 0x001DD128 File Offset: 0x001DB328
		public static void MakeDoublesided(Mesh input)
		{
			Vector3[] vertices = input.vertices;
			Vector3[] normals = input.normals;
			Vector2[] uv = input.uv;
			Color[] colors = input.colors;
			int[] triangles = input.triangles;
			List<int[]> list = new List<int[]>();
			for (int i = 0; i < input.subMeshCount; i++)
			{
				list.Add(input.GetTriangles(i));
			}
			Vector3[] array = new Vector3[vertices.Length * 2];
			Vector3[] array2 = new Vector3[normals.Length * 2];
			Vector2[] array3 = new Vector2[uv.Length * 2];
			Color[] array4 = new Color[colors.Length * 2];
			int[] array5 = new int[triangles.Length * 2];
			List<int[]> list2 = new List<int[]>();
			for (int j = 0; j < list.Count; j++)
			{
				list2.Add(new int[list[j].Length * 2]);
				list[j].CopyTo(list2[j], 0);
			}
			for (int k = 0; k < vertices.Length; k++)
			{
				array[k] = vertices[k];
				array2[k] = normals[k];
				array3[k] = uv[k];
				if (colors.Length > k)
				{
					array4[k] = colors[k];
				}
				array[k + vertices.Length] = vertices[k];
				array2[k + vertices.Length] = -normals[k];
				array3[k + vertices.Length] = uv[k];
				if (colors.Length > k)
				{
					array4[k + vertices.Length] = colors[k];
				}
			}
			for (int l = 0; l < triangles.Length; l += 3)
			{
				int num = triangles[l];
				int num2 = triangles[l + 1];
				int num3 = triangles[l + 2];
				array5[l] = num;
				array5[l + 1] = num2;
				array5[l + 2] = num3;
				array5[l + triangles.Length] = num3 + vertices.Length;
				array5[l + triangles.Length + 1] = num2 + vertices.Length;
				array5[l + triangles.Length + 2] = num + vertices.Length;
			}
			for (int m = 0; m < list.Count; m++)
			{
				for (int n = 0; n < list[m].Length; n += 3)
				{
					int num4 = list[m][n];
					int num5 = list[m][n + 1];
					int num6 = list[m][n + 2];
					list2[m][n] = num4;
					list2[m][n + 1] = num5;
					list2[m][n + 2] = num6;
					list2[m][n + list[m].Length] = num6 + vertices.Length;
					list2[m][n + list[m].Length + 1] = num5 + vertices.Length;
					list2[m][n + list[m].Length + 2] = num4 + vertices.Length;
				}
			}
			input.vertices = array;
			input.normals = array2;
			input.uv = array3;
			input.colors = array4;
			input.triangles = array5;
			for (int num7 = 0; num7 < list2.Count; num7++)
			{
				input.SetTriangles(list2[num7], num7);
			}
		}

		// Token: 0x06003E68 RID: 15976 RVA: 0x001DD490 File Offset: 0x001DB690
		public static void MakeDoublesided(TS_Mesh input)
		{
			Vector3[] vertices = input.vertices;
			Vector3[] normals = input.normals;
			Vector2[] uv = input.uv;
			Color[] colors = input.colors;
			int[] triangles = input.triangles;
			List<int[]> subMeshes = input.subMeshes;
			Vector3[] array = new Vector3[vertices.Length * 2];
			Vector3[] array2 = new Vector3[normals.Length * 2];
			Vector2[] array3 = new Vector2[uv.Length * 2];
			Color[] array4 = new Color[colors.Length * 2];
			int[] array5 = new int[triangles.Length * 2];
			List<int[]> list = new List<int[]>();
			for (int i = 0; i < subMeshes.Count; i++)
			{
				list.Add(new int[subMeshes[i].Length * 2]);
				subMeshes[i].CopyTo(list[i], 0);
			}
			for (int j = 0; j < vertices.Length; j++)
			{
				array[j] = vertices[j];
				array2[j] = normals[j];
				array3[j] = uv[j];
				if (colors.Length > j)
				{
					array4[j] = colors[j];
				}
				array[j + vertices.Length] = vertices[j];
				array2[j + vertices.Length] = -normals[j];
				array3[j + vertices.Length] = uv[j];
				if (colors.Length > j)
				{
					array4[j + vertices.Length] = colors[j];
				}
			}
			for (int k = 0; k < triangles.Length; k += 3)
			{
				int num = triangles[k];
				int num2 = triangles[k + 1];
				int num3 = triangles[k + 2];
				array5[k] = num;
				array5[k + 1] = num2;
				array5[k + 2] = num3;
				array5[k + triangles.Length] = num3 + vertices.Length;
				array5[k + triangles.Length + 1] = num2 + vertices.Length;
				array5[k + triangles.Length + 2] = num + vertices.Length;
			}
			for (int l = 0; l < subMeshes.Count; l++)
			{
				for (int m = 0; m < subMeshes[l].Length; m += 3)
				{
					int num4 = subMeshes[l][m];
					int num5 = subMeshes[l][m + 1];
					int num6 = subMeshes[l][m + 2];
					list[l][m] = num4;
					list[l][m + 1] = num5;
					list[l][m + 2] = num6;
					list[l][m + subMeshes[l].Length] = num6 + vertices.Length;
					list[l][m + subMeshes[l].Length + 1] = num5 + vertices.Length;
					list[l][m + subMeshes[l].Length + 2] = num4 + vertices.Length;
				}
			}
			input.vertices = array;
			input.normals = array2;
			input.uv = array3;
			input.colors = array4;
			input.triangles = array5;
			input.subMeshes = list;
		}

		// Token: 0x06003E69 RID: 15977 RVA: 0x001DD7B8 File Offset: 0x001DB9B8
		public static void MakeDoublesidedHalf(TS_Mesh input)
		{
			int num = input.vertices.Length / 2;
			int num2 = input.triangles.Length / 2;
			for (int i = 0; i < num; i++)
			{
				input.vertices[i + num] = input.vertices[i];
				if (input.normals.Length > i)
				{
					input.normals[i + num] = -input.normals[i];
				}
				if (input.tangents.Length > i)
				{
					input.tangents[i + num] = input.tangents[i];
				}
				if (input.uv.Length > i)
				{
					input.uv[i + num] = input.uv[i];
				}
				if (input.uv2.Length > i)
				{
					input.uv2[i + num] = input.uv2[i];
				}
				if (input.uv3.Length > i)
				{
					input.uv3[i + num] = input.uv3[i];
				}
				if (input.uv4.Length > i)
				{
					input.uv4[i + num] = input.uv4[i];
				}
				if (input.colors.Length > i)
				{
					input.colors[i + num] = input.colors[i];
				}
			}
			for (int j = 0; j < num2; j += 3)
			{
				input.triangles[j + num2 + 2] = input.triangles[j] + num;
				input.triangles[j + num2 + 1] = input.triangles[j + 1] + num;
				input.triangles[j + num2] = input.triangles[j + 2] + num;
			}
			for (int k = 0; k < input.subMeshes.Count; k++)
			{
				num2 = input.subMeshes[k].Length / 2;
				for (int l = 0; l < num2; l += 3)
				{
					input.subMeshes[k][l + num2 + 2] = input.subMeshes[k][l] + num;
					input.subMeshes[k][l + num2 + 1] = input.subMeshes[k][l + 1] + num;
					input.subMeshes[k][l + num2] = input.subMeshes[k][l + 2] + num;
				}
			}
		}

		// Token: 0x06003E6A RID: 15978 RVA: 0x001DDA18 File Offset: 0x001DBC18
		public static void InverseTransformMesh(TS_Mesh input, TS_Transform transform)
		{
			if (input.vertices == null || input.normals == null)
			{
				return;
			}
			for (int i = 0; i < input.vertices.Length; i++)
			{
				input.vertices[i] = transform.InverseTransformPoint(input.vertices[i]);
				input.normals[i] = transform.InverseTransformDirection(input.normals[i]);
			}
		}

		// Token: 0x06003E6B RID: 15979 RVA: 0x001DDA88 File Offset: 0x001DBC88
		public static void TransformMesh(TS_Mesh input, TS_Transform transform)
		{
			if (input.vertices == null || input.normals == null)
			{
				return;
			}
			for (int i = 0; i < input.vertices.Length; i++)
			{
				input.vertices[i] = transform.TransformPoint(input.vertices[i]);
				input.normals[i] = transform.TransformDirection(input.normals[i]);
			}
		}

		// Token: 0x06003E6C RID: 15980 RVA: 0x001DDAF8 File Offset: 0x001DBCF8
		public static void InverseTransformMesh(TS_Mesh input, Transform transform)
		{
			if (input.vertices == null || input.normals == null)
			{
				return;
			}
			for (int i = 0; i < input.vertices.Length; i++)
			{
				input.vertices[i] = transform.InverseTransformPoint(input.vertices[i]);
				input.normals[i] = transform.InverseTransformDirection(input.normals[i]);
			}
		}

		// Token: 0x06003E6D RID: 15981 RVA: 0x001DDB68 File Offset: 0x001DBD68
		public static void TransformMesh(TS_Mesh input, Transform transform)
		{
			if (input.vertices == null || input.normals == null)
			{
				return;
			}
			for (int i = 0; i < input.vertices.Length; i++)
			{
				input.vertices[i] = transform.TransformPoint(input.vertices[i]);
				input.normals[i] = transform.TransformDirection(input.normals[i]);
			}
		}

		// Token: 0x06003E6E RID: 15982 RVA: 0x001DDBD8 File Offset: 0x001DBDD8
		public static void InverseTransformMesh(Mesh input, Transform transform)
		{
			Vector3[] vertices = input.vertices;
			Vector3[] vertices2 = input.vertices;
			Matrix4x4 worldToLocalMatrix = transform.worldToLocalMatrix;
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = worldToLocalMatrix.MultiplyPoint3x4(vertices[i]);
				vertices2[i] = worldToLocalMatrix.MultiplyVector(vertices2[i]);
			}
			input.vertices = vertices;
			input.normals = vertices2;
		}

		// Token: 0x06003E6F RID: 15983 RVA: 0x001DDC40 File Offset: 0x001DBE40
		public static void TransformMesh(Mesh input, Transform transform)
		{
			Vector3[] vertices = input.vertices;
			Vector3[] vertices2 = input.vertices;
			Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
			if (input.vertices == null || input.normals == null)
			{
				return;
			}
			for (int i = 0; i < input.vertices.Length; i++)
			{
				vertices[i] = localToWorldMatrix.MultiplyPoint3x4(vertices[i]);
				vertices2[i] = localToWorldMatrix.MultiplyVector(vertices2[i]);
			}
			input.vertices = vertices;
			input.normals = vertices2;
		}

		// Token: 0x06003E70 RID: 15984 RVA: 0x001DDCC0 File Offset: 0x001DBEC0
		public static void TransformVertices(Vector3[] vertices, Transform transform)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = transform.TransformPoint(vertices[i]);
			}
		}

		// Token: 0x06003E71 RID: 15985 RVA: 0x001DDCF0 File Offset: 0x001DBEF0
		public static void InverseTransformVertices(Vector3[] vertices, Transform transform)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = transform.InverseTransformPoint(vertices[i]);
			}
		}

		// Token: 0x06003E72 RID: 15986 RVA: 0x001DDD20 File Offset: 0x001DBF20
		public static void TransformNormals(Vector3[] normals, Transform transform)
		{
			for (int i = 0; i < normals.Length; i++)
			{
				normals[i] = transform.TransformDirection(normals[i]);
			}
		}

		// Token: 0x06003E73 RID: 15987 RVA: 0x001DDD50 File Offset: 0x001DBF50
		public static void InverseTransformNormals(Vector3[] normals, Transform transform)
		{
			for (int i = 0; i < normals.Length; i++)
			{
				normals[i] = transform.InverseTransformDirection(normals[i]);
			}
		}

		// Token: 0x06003E74 RID: 15988 RVA: 0x001DDD80 File Offset: 0x001DBF80
		public static string ToOBJString(Mesh mesh, Material[] materials)
		{
			int num = 0;
			if (mesh == null)
			{
				return "####Error####";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("g " + mesh.name + "\n");
			foreach (Vector3 vector in mesh.vertices)
			{
				num++;
				stringBuilder.Append(string.Format("v {0} {1} {2}\n", -vector.x, vector.y, vector.z));
			}
			stringBuilder.Append("\n");
			foreach (Vector3 vector2 in mesh.normals)
			{
				stringBuilder.Append(string.Format("vn {0} {1} {2}\n", -vector2.x, vector2.y, vector2.z));
			}
			stringBuilder.Append("\n");
			Vector2[] array2 = mesh.uv;
			for (int i = 0; i < array2.Length; i++)
			{
				Vector3 vector3 = array2[i];
				stringBuilder.Append(string.Format("vt {0} {1}\n", vector3.x, vector3.y));
			}
			stringBuilder.Append("\n");
			foreach (Vector2 vector4 in mesh.uv2)
			{
				stringBuilder.Append(string.Format("vt2 {0} {1}\n", vector4.x, vector4.y));
			}
			stringBuilder.Append("\n");
			foreach (Vector2 vector5 in mesh.uv3)
			{
				stringBuilder.Append(string.Format("vt2 {0} {1}\n", vector5.x, vector5.y));
			}
			stringBuilder.Append("\n");
			foreach (Color color in mesh.colors)
			{
				stringBuilder.Append(string.Format("vc {0} {1} {2} {3}\n", new object[]
				{
					color.r,
					color.g,
					color.b,
					color.a
				}));
			}
			for (int j = 0; j < mesh.subMeshCount; j++)
			{
				stringBuilder.Append("\n");
				stringBuilder.Append("usemtl ").Append(materials[j].name).Append("\n");
				stringBuilder.Append("usemap ").Append(materials[j].name).Append("\n");
				int[] triangles = mesh.GetTriangles(j);
				for (int k = 0; k < triangles.Length; k += 3)
				{
					stringBuilder.Append(string.Format("f {2}/{2}/{2} {1}/{1}/{1} {0}/{0}/{0}\n", triangles[k] + 1, triangles[k + 1] + 1, triangles[k + 2] + 1));
				}
			}
			return stringBuilder.ToString().Replace(',', '.');
		}

		// Token: 0x06003E75 RID: 15989 RVA: 0x001DE0D4 File Offset: 0x001DC2D4
		public static Mesh Copy(Mesh input)
		{
			Mesh mesh = new Mesh();
			mesh.name = input.name;
			mesh.vertices = input.vertices;
			mesh.normals = input.normals;
			mesh.colors = input.colors;
			mesh.uv = input.uv;
			mesh.uv2 = input.uv2;
			mesh.uv3 = input.uv3;
			mesh.uv4 = input.uv4;
			mesh.tangents = input.tangents;
			mesh.boneWeights = input.boneWeights;
			mesh.bindposes = input.bindposes;
			mesh.triangles = input.triangles;
			mesh.subMeshCount = input.subMeshCount;
			for (int i = 0; i < input.subMeshCount; i++)
			{
				mesh.SetTriangles(input.GetTriangles(i), i);
			}
			return mesh;
		}

		// Token: 0x06003E76 RID: 15990 RVA: 0x001DE1A4 File Offset: 0x001DC3A4
		public static void Triangulate(Vector2[] points, ref int[] output)
		{
			List<int> list = new List<int>();
			int num = points.Length;
			if (num < 3)
			{
				output = new int[0];
				return;
			}
			int[] array = new int[num];
			if (MeshUtility.Area(points, num) > 0f)
			{
				for (int i = 0; i < num; i++)
				{
					array[i] = i;
				}
			}
			else
			{
				for (int j = 0; j < num; j++)
				{
					array[j] = num - 1 - j;
				}
			}
			int k = num;
			int num2 = 2 * k;
			int num3 = 0;
			int num4 = k - 1;
			while (k > 2)
			{
				if (num2-- <= 0)
				{
					if (output.Length != list.Count)
					{
						output = new int[list.Count];
					}
					list.CopyTo(output, 0);
					return;
				}
				int num5 = num4;
				if (k <= num5)
				{
					num5 = 0;
				}
				num4 = num5 + 1;
				if (k <= num4)
				{
					num4 = 0;
				}
				int num6 = num4 + 1;
				if (k <= num6)
				{
					num6 = 0;
				}
				if (MeshUtility.Snip(points, num5, num4, num6, k, array))
				{
					int item = array[num5];
					int item2 = array[num4];
					int item3 = array[num6];
					list.Add(item3);
					list.Add(item2);
					list.Add(item);
					num3++;
					int num7 = num4;
					for (int l = num4 + 1; l < k; l++)
					{
						array[num7] = array[l];
						num7++;
					}
					k--;
					num2 = 2 * k;
				}
			}
			list.Reverse();
			if (output.Length != list.Count)
			{
				output = new int[list.Count];
			}
			list.CopyTo(output, 0);
		}

		// Token: 0x06003E77 RID: 15991 RVA: 0x001DE318 File Offset: 0x001DC518
		public static void FlipTriangles(ref int[] triangles)
		{
			for (int i = 0; i < triangles.Length; i += 3)
			{
				int num = triangles[i];
				triangles[i] = triangles[i + 2];
				triangles[i + 2] = num;
			}
		}

		// Token: 0x06003E78 RID: 15992 RVA: 0x001DE34C File Offset: 0x001DC54C
		public static void FlipFaces(TS_Mesh input)
		{
			for (int i = 0; i < input.subMeshes.Count; i++)
			{
				int[] array = input.subMeshes[i];
				MeshUtility.FlipTriangles(ref array);
			}
			MeshUtility.FlipTriangles(ref input.triangles);
			for (int j = 0; j < input.normals.Length; j++)
			{
				input.normals[j] *= -1f;
			}
		}

		// Token: 0x06003E79 RID: 15993 RVA: 0x001DE3C4 File Offset: 0x001DC5C4
		public static void BreakMesh(Mesh input, bool keepNormals = true)
		{
			Vector3[] array = new Vector3[input.triangles.Length];
			Vector3[] array2 = new Vector3[array.Length];
			Vector2[] array3 = new Vector2[array.Length];
			Vector4[] array4 = new Vector4[array.Length];
			Color[] array5 = new Color[array.Length];
			BoneWeight[] array6 = new BoneWeight[array.Length];
			Vector3[] vertices = input.vertices;
			Vector2[] uv = input.uv;
			Vector3[] normals = input.normals;
			Vector4[] tangents = input.tangents;
			Color[] array7 = input.colors;
			BoneWeight[] boneWeights = input.boneWeights;
			if (array7.Length != vertices.Length)
			{
				array7 = new Color[vertices.Length];
				for (int i = 0; i < array7.Length; i++)
				{
					array7[i] = Color.white;
				}
			}
			List<int[]> list = new List<int[]>();
			int subMeshCount = input.subMeshCount;
			int num = 0;
			for (int j = 0; j < subMeshCount; j++)
			{
				int[] triangles = input.GetTriangles(j);
				for (int k = 0; k < triangles.Length; k += 3)
				{
					array[num] = vertices[triangles[k]];
					array[num + 1] = vertices[triangles[k + 1]];
					array[num + 2] = vertices[triangles[k + 2]];
					if (normals.Length > triangles[k + 2])
					{
						if (!keepNormals)
						{
							array2[num] = (array2[num + 1] = (array2[num + 2] = (normals[triangles[k]] + normals[triangles[k + 1]] + normals[triangles[k + 2]]).normalized));
						}
						else
						{
							array2[num] = normals[triangles[k]];
							array2[num + 1] = normals[triangles[k + 1]];
							array2[num + 2] = normals[triangles[k + 2]];
						}
					}
					if (array7.Length > triangles[k + 2])
					{
						array5[num] = (array5[num + 1] = (array5[num + 2] = (array7[triangles[k]] + array7[triangles[k + 1]] + array7[triangles[k + 2]]) / 3f));
					}
					if (uv.Length > triangles[k + 2])
					{
						array3[num] = uv[triangles[k]];
						array3[num + 1] = uv[triangles[k + 1]];
						array3[num + 2] = uv[triangles[k + 2]];
					}
					if (tangents.Length > triangles[k + 2])
					{
						array4[num] = tangents[triangles[k]];
						array4[num + 1] = tangents[triangles[k + 1]];
						array4[num + 2] = tangents[triangles[k + 2]];
					}
					if (boneWeights.Length > triangles[k + 2])
					{
						array6[num] = boneWeights[triangles[k]];
						array6[num + 1] = boneWeights[triangles[k + 1]];
						array6[num + 2] = boneWeights[triangles[k + 2]];
					}
					triangles[k] = num;
					triangles[k + 1] = num + 1;
					triangles[k + 2] = num + 2;
					num += 3;
				}
				list.Add(triangles);
			}
			input.vertices = array;
			input.normals = array2;
			input.colors = array5;
			input.uv = array3;
			input.tangents = array4;
			input.subMeshCount = list.Count;
			input.boneWeights = array6;
			for (int l = 0; l < list.Count; l++)
			{
				input.SetTriangles(list[l], l);
			}
		}

		// Token: 0x06003E7A RID: 15994 RVA: 0x001DE7B4 File Offset: 0x001DC9B4
		private static float Area(Vector2[] points, int maxCount)
		{
			float num = 0f;
			int num2 = maxCount - 1;
			int i = 0;
			while (i < maxCount)
			{
				Vector2 vector = points[num2];
				Vector2 vector2 = points[i];
				num += vector.x * vector2.y - vector2.x * vector.y;
				num2 = i++;
			}
			return num * 0.5f;
		}

		// Token: 0x06003E7B RID: 15995 RVA: 0x001DE814 File Offset: 0x001DCA14
		private static bool Snip(Vector2[] points, int u, int v, int w, int n, int[] V)
		{
			Vector2 vector = points[V[u]];
			Vector2 vector2 = points[V[v]];
			Vector2 vector3 = points[V[w]];
			if (Mathf.Epsilon > (vector2.x - vector.x) * (vector3.y - vector.y) - (vector2.y - vector.y) * (vector3.x - vector.x))
			{
				return false;
			}
			for (int i = 0; i < n; i++)
			{
				if (i != u && i != v && i != w)
				{
					Vector2 p = points[V[i]];
					if (MeshUtility.InsideTriangle(vector, vector2, vector3, p))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06003E7C RID: 15996 RVA: 0x001DE8B8 File Offset: 0x001DCAB8
		private static bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
		{
			float num = C.x - B.x;
			float num2 = C.y - B.y;
			float num3 = A.x - C.x;
			float num4 = A.y - C.y;
			float num5 = B.x - A.x;
			float num6 = B.y - A.y;
			float num7 = P.x - A.x;
			float num8 = P.y - A.y;
			float num9 = P.x - B.x;
			float num10 = P.y - B.y;
			float num11 = P.x - C.x;
			float num12 = P.y - C.y;
			float num13 = num * num10 - num2 * num9;
			float num14 = num5 * num8 - num6 * num7;
			float num15 = num3 * num12 - num4 * num11;
			return num13 >= 0f && num15 >= 0f && num14 >= 0f;
		}

		// Token: 0x04002C4D RID: 11341
		private static Vector3[] tan1 = new Vector3[0];

		// Token: 0x04002C4E RID: 11342
		private static Vector3[] tan2 = new Vector3[0];
	}
}

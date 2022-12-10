using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck
{
	// Token: 0x020004AA RID: 1194
	public class TS_Mesh
	{
		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06003E89 RID: 16009 RVA: 0x001DEF46 File Offset: 0x001DD146
		// (set) Token: 0x06003E8A RID: 16010 RVA: 0x000023FD File Offset: 0x000005FD
		public int vertexCount
		{
			get
			{
				return this.vertices.Length;
			}
			set
			{
			}
		}

		// Token: 0x06003E8B RID: 16011 RVA: 0x001DEF50 File Offset: 0x001DD150
		public TS_Mesh()
		{
		}

		// Token: 0x06003E8C RID: 16012 RVA: 0x001DEFF0 File Offset: 0x001DD1F0
		public TS_Mesh(Mesh mesh)
		{
			this.CreateFromMesh(mesh);
		}

		// Token: 0x06003E8D RID: 16013 RVA: 0x001DF098 File Offset: 0x001DD298
		public void Clear()
		{
			this.vertices = new Vector3[0];
			this.normals = new Vector3[0];
			this.tangents = new Vector4[0];
			this.colors = new Color[0];
			this.uv = new Vector2[0];
			this.uv2 = new Vector2[0];
			this.uv3 = new Vector2[0];
			this.uv4 = new Vector2[0];
			this.triangles = new int[0];
			this.subMeshes = new List<int[]>();
			this.bounds = new TS_Bounds(Vector3.zero, Vector3.zero);
		}

		// Token: 0x06003E8E RID: 16014 RVA: 0x001DF134 File Offset: 0x001DD334
		public void CreateFromMesh(Mesh mesh)
		{
			this.vertices = mesh.vertices;
			this.normals = mesh.normals;
			this.tangents = mesh.tangents;
			this.colors = mesh.colors;
			this.uv = mesh.uv;
			this.uv2 = mesh.uv2;
			this.uv3 = mesh.uv3;
			this.uv4 = mesh.uv4;
			this.triangles = mesh.triangles;
			this.bounds = new TS_Bounds(mesh.bounds);
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				this.subMeshes.Add(mesh.GetTriangles(i));
			}
		}

		// Token: 0x06003E8F RID: 16015 RVA: 0x001DF1E4 File Offset: 0x001DD3E4
		public void Combine(List<TS_Mesh> newMeshes, bool overwrite = false)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < newMeshes.Count; i++)
			{
				num += newMeshes[i].vertexCount;
				num2 += newMeshes[i].triangles.Length;
				if (newMeshes[i].subMeshes.Count > num3)
				{
					num3 = newMeshes[i].subMeshes.Count;
				}
			}
			int[] array = new int[num3];
			int[] array2 = new int[num3];
			for (int j = 0; j < newMeshes.Count; j++)
			{
				for (int k = 0; k < newMeshes[j].subMeshes.Count; k++)
				{
					array[k] += newMeshes[j].subMeshes[k].Length;
				}
			}
			if (overwrite)
			{
				int num4 = 0;
				int num5 = 0;
				if (this.vertices.Length != num)
				{
					this.vertices = new Vector3[num];
				}
				if (this.normals.Length != num)
				{
					this.normals = new Vector3[num];
				}
				if (this.uv.Length != num)
				{
					this.uv = new Vector2[num];
				}
				if (this.uv2.Length != num)
				{
					this.uv2 = new Vector2[num];
				}
				if (this.uv3.Length != num)
				{
					this.uv3 = new Vector2[num];
				}
				if (this.uv4.Length != num)
				{
					this.uv4 = new Vector2[num];
				}
				if (this.colors.Length != num)
				{
					this.colors = new Color[num];
				}
				if (this.tangents.Length != num)
				{
					this.tangents = new Vector4[num];
				}
				if (this.triangles.Length != num2)
				{
					this.triangles = new int[num2];
				}
				if (this.subMeshes.Count != num3)
				{
					this.subMeshes.Clear();
				}
				for (int l = 0; l < newMeshes.Count; l++)
				{
					newMeshes[l].vertices.CopyTo(this.vertices, num4);
					newMeshes[l].normals.CopyTo(this.normals, num4);
					newMeshes[l].uv.CopyTo(this.uv, num4);
					newMeshes[l].uv2.CopyTo(this.uv2, num4);
					newMeshes[l].uv3.CopyTo(this.uv3, num4);
					newMeshes[l].uv4.CopyTo(this.uv4, num4);
					newMeshes[l].colors.CopyTo(this.colors, num4);
					newMeshes[l].tangents.CopyTo(this.tangents, num4);
					for (int m = num5; m < num5 + newMeshes[l].triangles.Length; m++)
					{
						this.triangles[m] = newMeshes[l].triangles[m - num2] + num4;
					}
					num5 += newMeshes[l].triangles.Length;
					for (int n = 0; n < newMeshes[l].subMeshes.Count; n++)
					{
						if (n >= this.subMeshes.Count)
						{
							this.subMeshes.Add(new int[array[n]]);
						}
						else if (this.subMeshes[n].Length != array[n])
						{
							this.subMeshes[n] = new int[array[n]];
						}
						for (int num6 = array2[n]; num6 < array2[n] + newMeshes[l].subMeshes[n].Length; num6++)
						{
							this.subMeshes[n][num6] = newMeshes[l].subMeshes[n][num6 - array2[n]] + num4;
						}
						array2[n] += newMeshes[l].subMeshes[n].Length;
					}
					num4 += newMeshes[l].vertexCount;
				}
				return;
			}
			Vector3[] array3 = new Vector3[this.vertices.Length + num];
			Vector3[] array4 = new Vector3[this.vertices.Length + num];
			Vector2[] array5 = new Vector2[this.vertices.Length + num];
			Vector2[] array6 = new Vector2[this.vertices.Length + num];
			Vector2[] array7 = new Vector2[this.vertices.Length + num];
			Vector2[] array8 = new Vector2[this.vertices.Length + num];
			Color[] array9 = new Color[this.vertices.Length + num];
			Vector4[] array10 = new Vector4[this.tangents.Length + num];
			int[] array11 = new int[this.triangles.Length + num2];
			List<int[]> list = new List<int[]>();
			for (int num7 = 0; num7 < array.Length; num7++)
			{
				list.Add(new int[array[num7]]);
				if (num7 < this.subMeshes.Count)
				{
					array[num7] = this.subMeshes[num7].Length;
				}
				else
				{
					array[num7] = 0;
				}
			}
			num = this.vertexCount;
			num2 = this.triangles.Length;
			this.vertices.CopyTo(array3, 0);
			this.normals.CopyTo(array4, 0);
			this.uv.CopyTo(array5, 0);
			this.uv2.CopyTo(array6, 0);
			this.uv3.CopyTo(array7, 0);
			this.uv4.CopyTo(array8, 0);
			this.colors.CopyTo(array9, 0);
			this.tangents.CopyTo(array10, 0);
			this.triangles.CopyTo(array11, 0);
			for (int num8 = 0; num8 < newMeshes.Count; num8++)
			{
				newMeshes[num8].vertices.CopyTo(array3, num);
				newMeshes[num8].normals.CopyTo(array4, num);
				newMeshes[num8].uv.CopyTo(array5, num);
				newMeshes[num8].uv2.CopyTo(array6, num);
				newMeshes[num8].uv3.CopyTo(array7, num);
				newMeshes[num8].uv4.CopyTo(array8, num);
				newMeshes[num8].colors.CopyTo(array9, num);
				newMeshes[num8].tangents.CopyTo(array10, num);
				for (int num9 = num2; num9 < num2 + newMeshes[num8].triangles.Length; num9++)
				{
					array11[num9] = newMeshes[num8].triangles[num9 - num2] + num;
				}
				for (int num10 = 0; num10 < newMeshes[num8].subMeshes.Count; num10++)
				{
					for (int num11 = array[num10]; num11 < array[num10] + newMeshes[num8].subMeshes[num10].Length; num11++)
					{
						list[num10][num11] = newMeshes[num8].subMeshes[num10][num11 - array[num10]] + num;
					}
					array[num10] += newMeshes[num8].subMeshes[num10].Length;
				}
				num2 += newMeshes[num8].triangles.Length;
				num += newMeshes[num8].vertexCount;
			}
			this.vertices = array3;
			this.normals = array4;
			this.uv = array5;
			this.uv2 = array6;
			this.uv3 = array7;
			this.uv4 = array8;
			this.colors = array9;
			this.tangents = array10;
			this.triangles = array11;
			this.subMeshes = list;
		}

		// Token: 0x06003E90 RID: 16016 RVA: 0x001DF980 File Offset: 0x001DDB80
		public void Combine(TS_Mesh newMesh)
		{
			Vector3[] array = new Vector3[this.vertices.Length + newMesh.vertices.Length];
			Vector3[] array2 = new Vector3[this.normals.Length + newMesh.normals.Length];
			Vector2[] array3 = new Vector2[this.uv.Length + newMesh.uv.Length];
			Vector2[] array4 = new Vector2[this.uv.Length + newMesh.uv2.Length];
			Vector2[] array5 = new Vector2[this.uv.Length + newMesh.uv3.Length];
			Vector2[] array6 = new Vector2[this.uv.Length + newMesh.uv4.Length];
			Color[] array7 = new Color[this.colors.Length + newMesh.colors.Length];
			Vector4[] array8 = new Vector4[this.tangents.Length + newMesh.tangents.Length];
			int[] array9 = new int[this.triangles.Length + newMesh.triangles.Length];
			this.vertices.CopyTo(array, 0);
			newMesh.vertices.CopyTo(array, this.vertices.Length);
			this.normals.CopyTo(array2, 0);
			newMesh.normals.CopyTo(array2, this.normals.Length);
			this.uv.CopyTo(array3, 0);
			newMesh.uv.CopyTo(array3, this.uv.Length);
			this.uv2.CopyTo(array4, 0);
			newMesh.uv2.CopyTo(array4, this.uv2.Length);
			this.uv3.CopyTo(array5, 0);
			newMesh.uv3.CopyTo(array5, this.uv3.Length);
			this.uv4.CopyTo(array6, 0);
			newMesh.uv4.CopyTo(array6, this.uv4.Length);
			this.colors.CopyTo(array7, 0);
			newMesh.colors.CopyTo(array7, this.colors.Length);
			this.tangents.CopyTo(array8, 0);
			newMesh.tangents.CopyTo(array8, this.tangents.Length);
			for (int i = 0; i < array9.Length; i++)
			{
				if (i < this.triangles.Length)
				{
					array9[i] = this.triangles[i];
				}
				else
				{
					array9[i] = newMesh.triangles[i - this.triangles.Length] + this.vertices.Length;
				}
			}
			for (int j = 0; j < newMesh.subMeshes.Count; j++)
			{
				if (j >= this.subMeshes.Count)
				{
					this.subMeshes.Add(newMesh.subMeshes[j]);
				}
				else
				{
					int[] array10 = new int[this.subMeshes[j].Length + newMesh.subMeshes[j].Length];
					this.subMeshes[j].CopyTo(array10, 0);
					for (int k = 0; k < newMesh.subMeshes[j].Length; k++)
					{
						array10[this.subMeshes[j].Length + k] = newMesh.subMeshes[j][k] + this.vertices.Length;
					}
					this.subMeshes[j] = array10;
				}
			}
			this.vertices = array;
			this.normals = array2;
			this.uv = array3;
			this.uv2 = array4;
			this.uv3 = array5;
			this.uv4 = array6;
			this.colors = array7;
			this.tangents = array8;
			this.triangles = array9;
		}

		// Token: 0x06003E91 RID: 16017 RVA: 0x001DFCE8 File Offset: 0x001DDEE8
		public static TS_Mesh Copy(TS_Mesh input)
		{
			TS_Mesh ts_Mesh = new TS_Mesh();
			ts_Mesh.vertices = new Vector3[input.vertices.Length];
			input.vertices.CopyTo(ts_Mesh.vertices, 0);
			ts_Mesh.normals = new Vector3[input.normals.Length];
			input.normals.CopyTo(ts_Mesh.normals, 0);
			ts_Mesh.uv = new Vector2[input.uv.Length];
			input.uv.CopyTo(ts_Mesh.uv, 0);
			ts_Mesh.uv2 = new Vector2[input.uv2.Length];
			input.uv2.CopyTo(ts_Mesh.uv2, 0);
			ts_Mesh.uv3 = new Vector2[input.uv3.Length];
			input.uv3.CopyTo(ts_Mesh.uv3, 0);
			ts_Mesh.uv4 = new Vector2[input.uv4.Length];
			input.uv4.CopyTo(ts_Mesh.uv4, 0);
			ts_Mesh.colors = new Color[input.colors.Length];
			input.colors.CopyTo(ts_Mesh.colors, 0);
			ts_Mesh.tangents = new Vector4[input.tangents.Length];
			input.tangents.CopyTo(ts_Mesh.tangents, 0);
			ts_Mesh.triangles = new int[input.triangles.Length];
			input.triangles.CopyTo(ts_Mesh.triangles, 0);
			ts_Mesh.subMeshes = new List<int[]>();
			for (int i = 0; i < input.subMeshes.Count; i++)
			{
				ts_Mesh.subMeshes.Add(new int[input.subMeshes[i].Length]);
				input.subMeshes[i].CopyTo(ts_Mesh.subMeshes[i], 0);
			}
			ts_Mesh.bounds = new TS_Bounds(input.bounds.center, input.bounds.size);
			return ts_Mesh;
		}

		// Token: 0x06003E92 RID: 16018 RVA: 0x001DFEC8 File Offset: 0x001DE0C8
		public void Absorb(TS_Mesh input)
		{
			if (this.vertices.Length != input.vertexCount)
			{
				this.vertices = new Vector3[input.vertexCount];
			}
			if (this.normals.Length != input.normals.Length)
			{
				this.normals = new Vector3[input.normals.Length];
			}
			if (this.colors.Length != input.colors.Length)
			{
				this.colors = new Color[input.colors.Length];
			}
			if (this.uv.Length != input.uv.Length)
			{
				this.uv = new Vector2[input.uv.Length];
			}
			if (this.uv2.Length != input.uv2.Length)
			{
				this.uv2 = new Vector2[input.uv2.Length];
			}
			if (this.uv3.Length != input.uv3.Length)
			{
				this.uv3 = new Vector2[input.uv3.Length];
			}
			if (this.uv4.Length != input.uv4.Length)
			{
				this.uv4 = new Vector2[input.uv4.Length];
			}
			if (this.tangents.Length != input.tangents.Length)
			{
				this.tangents = new Vector4[input.tangents.Length];
			}
			if (this.triangles.Length != input.triangles.Length)
			{
				this.triangles = new int[input.triangles.Length];
			}
			input.vertices.CopyTo(this.vertices, 0);
			input.normals.CopyTo(this.normals, 0);
			input.colors.CopyTo(this.colors, 0);
			input.uv.CopyTo(this.uv, 0);
			input.uv2.CopyTo(this.uv2, 0);
			input.uv3.CopyTo(this.uv3, 0);
			input.uv4.CopyTo(this.uv4, 0);
			input.tangents.CopyTo(this.tangents, 0);
			input.triangles.CopyTo(this.triangles, 0);
			if (this.subMeshes.Count == input.subMeshes.Count)
			{
				for (int i = 0; i < this.subMeshes.Count; i++)
				{
					if (input.subMeshes[i].Length != this.subMeshes[i].Length)
					{
						this.subMeshes[i] = new int[input.subMeshes[i].Length];
					}
					input.subMeshes[i].CopyTo(this.subMeshes[i], 0);
				}
			}
			else
			{
				this.subMeshes = new List<int[]>();
				for (int j = 0; j < input.subMeshes.Count; j++)
				{
					this.subMeshes.Add(new int[input.subMeshes[j].Length]);
					input.subMeshes[j].CopyTo(this.subMeshes[j], 0);
				}
			}
			this.bounds = new TS_Bounds(input.bounds.center, input.bounds.size);
		}

		// Token: 0x06003E93 RID: 16019 RVA: 0x001E01CC File Offset: 0x001DE3CC
		public void WriteMesh(ref Mesh input)
		{
			if (input == null)
			{
				input = new Mesh();
			}
			if (this.vertices == null || this.vertices.Length <= 65000)
			{
				input.Clear();
				input.vertices = this.vertices;
				input.normals = this.normals;
				if (this.tangents.Length == this.vertices.Length)
				{
					input.tangents = this.tangents;
				}
				if (this.colors.Length == this.vertices.Length)
				{
					input.colors = this.colors;
				}
				if (this.uv.Length == this.vertices.Length)
				{
					input.uv = this.uv;
				}
				if (this.uv2.Length == this.vertices.Length)
				{
					input.uv2 = this.uv2;
				}
				if (this.uv3.Length == this.vertices.Length)
				{
					input.uv3 = this.uv3;
				}
				if (this.uv4.Length == this.vertices.Length)
				{
					input.uv4 = this.uv4;
				}
				input.triangles = this.triangles;
				if (this.subMeshes.Count > 0)
				{
					input.subMeshCount = this.subMeshes.Count;
					for (int i = 0; i < this.subMeshes.Count; i++)
					{
						input.SetTriangles(this.subMeshes[i], i);
					}
				}
				input.RecalculateBounds();
				this.hasUpdate = false;
			}
		}

		// Token: 0x04002C54 RID: 11348
		public Vector3[] vertices = new Vector3[0];

		// Token: 0x04002C55 RID: 11349
		public Vector3[] normals = new Vector3[0];

		// Token: 0x04002C56 RID: 11350
		public Vector4[] tangents = new Vector4[0];

		// Token: 0x04002C57 RID: 11351
		public Color[] colors = new Color[0];

		// Token: 0x04002C58 RID: 11352
		public Vector2[] uv = new Vector2[0];

		// Token: 0x04002C59 RID: 11353
		public Vector2[] uv2 = new Vector2[0];

		// Token: 0x04002C5A RID: 11354
		public Vector2[] uv3 = new Vector2[0];

		// Token: 0x04002C5B RID: 11355
		public Vector2[] uv4 = new Vector2[0];

		// Token: 0x04002C5C RID: 11356
		public int[] triangles = new int[0];

		// Token: 0x04002C5D RID: 11357
		public List<int[]> subMeshes = new List<int[]>();

		// Token: 0x04002C5E RID: 11358
		public TS_Bounds bounds = new TS_Bounds(Vector3.zero, Vector3.zero);

		// Token: 0x04002C5F RID: 11359
		public volatile bool hasUpdate;
	}
}

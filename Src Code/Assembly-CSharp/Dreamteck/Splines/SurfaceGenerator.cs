using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004C8 RID: 1224
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[AddComponentMenu("Dreamteck/Splines/Users/Surface Generator")]
	public class SurfaceGenerator : MeshGenerator
	{
		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x060040F0 RID: 16624 RVA: 0x001ECC41 File Offset: 0x001EAE41
		// (set) Token: 0x060040F1 RID: 16625 RVA: 0x001ECC49 File Offset: 0x001EAE49
		public float expand
		{
			get
			{
				return this._expand;
			}
			set
			{
				if (value != this._expand)
				{
					this._expand = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x060040F2 RID: 16626 RVA: 0x001ECC61 File Offset: 0x001EAE61
		// (set) Token: 0x060040F3 RID: 16627 RVA: 0x001ECC69 File Offset: 0x001EAE69
		public float extrude
		{
			get
			{
				return this._extrude;
			}
			set
			{
				if (value != this._extrude)
				{
					this._extrude = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x060040F4 RID: 16628 RVA: 0x001ECC81 File Offset: 0x001EAE81
		// (set) Token: 0x060040F5 RID: 16629 RVA: 0x001ECC89 File Offset: 0x001EAE89
		public double extrudeClipFrom
		{
			get
			{
				return this._extrudeFrom;
			}
			set
			{
				if (value != this._extrudeFrom)
				{
					this._extrudeFrom = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x060040F6 RID: 16630 RVA: 0x001ECCA1 File Offset: 0x001EAEA1
		// (set) Token: 0x060040F7 RID: 16631 RVA: 0x001ECCA9 File Offset: 0x001EAEA9
		public double extrudeClipTo
		{
			get
			{
				return this._extrudeTo;
			}
			set
			{
				if (value != this._extrudeTo)
				{
					this._extrudeTo = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x060040F8 RID: 16632 RVA: 0x001ECCC1 File Offset: 0x001EAEC1
		// (set) Token: 0x060040F9 RID: 16633 RVA: 0x001ECCC9 File Offset: 0x001EAEC9
		public Vector2 sideUvScale
		{
			get
			{
				return this._sideUvScale;
			}
			set
			{
				if (value != this._sideUvScale)
				{
					this._sideUvScale = value;
					this.Rebuild();
					return;
				}
				this._sideUvScale = value;
			}
		}

		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x060040FA RID: 16634 RVA: 0x001ECCEE File Offset: 0x001EAEEE
		// (set) Token: 0x060040FB RID: 16635 RVA: 0x001ECCF6 File Offset: 0x001EAEF6
		public Vector2 sideUvOffset
		{
			get
			{
				return this._sideUvOffset;
			}
			set
			{
				if (value != this._sideUvOffset)
				{
					this._sideUvOffset = value;
					this.Rebuild();
					return;
				}
				this._sideUvOffset = value;
			}
		}

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x060040FC RID: 16636 RVA: 0x001ECD1B File Offset: 0x001EAF1B
		// (set) Token: 0x060040FD RID: 16637 RVA: 0x001ECD24 File Offset: 0x001EAF24
		public SplineComputer extrudeSpline
		{
			get
			{
				return this._extrudeSpline;
			}
			set
			{
				if (value != this._extrudeSpline)
				{
					if (this._extrudeSpline != null)
					{
						this._extrudeSpline.Unsubscribe(this);
					}
					this._extrudeSpline = value;
					if (value != null)
					{
						this._extrudeSpline.Subscribe(this);
					}
					this.Rebuild();
				}
			}
		}

		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x060040FE RID: 16638 RVA: 0x001ECD7B File Offset: 0x001EAF7B
		// (set) Token: 0x060040FF RID: 16639 RVA: 0x001ECD83 File Offset: 0x001EAF83
		public bool uniformUvs
		{
			get
			{
				return this._uniformUvs;
			}
			set
			{
				if (value != this._uniformUvs)
				{
					this._uniformUvs = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x06004100 RID: 16640 RVA: 0x001ECD9B File Offset: 0x001EAF9B
		protected override void Awake()
		{
			base.Awake();
			this.mesh.name = "surface";
		}

		// Token: 0x06004101 RID: 16641 RVA: 0x001ECDB3 File Offset: 0x001EAFB3
		protected override void BuildMesh()
		{
			if (base.spline.pointCount == 0)
			{
				return;
			}
			base.BuildMesh();
			this.Generate();
		}

		// Token: 0x06004102 RID: 16642 RVA: 0x001ECDD0 File Offset: 0x001EAFD0
		public void Generate()
		{
			int num = base.sampleCount;
			if (base.spline.isClosed)
			{
				num--;
			}
			int num2 = num;
			if (this._extrudeSpline)
			{
				this._extrudeSpline.Evaluate(ref this.extrudeResults, this._extrudeFrom, this._extrudeTo);
			}
			bool flag = this._extrudeSpline && this.extrudeResults.Length != 0;
			bool flag2 = !flag && this._extrude != 0f;
			if (flag)
			{
				num2 *= 2;
				num2 += base.sampleCount * this.extrudeResults.Length;
			}
			else if (flag2)
			{
				num2 *= 4;
				num2 += 2;
			}
			Vector3 center;
			Vector3 vector;
			this.GetProjectedVertices(num, out center, out vector);
			bool flag3 = this.IsClockwise(this.projectedVerts);
			bool flag4 = false;
			bool flag5 = false;
			if (!flag3)
			{
				flag5 = !flag5;
			}
			if (flag2 && this._extrude < 0f)
			{
				flag4 = !flag4;
				flag5 = !flag5;
			}
			this.GenerateSurfaceTris(flag4);
			int num3 = this.surfaceTris.Length;
			if (flag2)
			{
				num3 *= 2;
				num3 += 2 * base.sampleCount * 2 * 3;
			}
			else
			{
				num3 *= 2;
				num3 += this.extrudeResults.Length * base.sampleCount * 2 * 3;
			}
			this.AllocateMesh(num2, num3);
			Vector3 b = this.trs.right * base.offset.x + this.trs.up * base.offset.y + this.trs.forward * base.offset.z;
			for (int i = 0; i < num; i++)
			{
				base.GetSample(i, this.evalResult);
				this.tsMesh.vertices[i] = this.evalResult.position + b;
				this.tsMesh.normals[i] = this.evalResult.up;
				this.tsMesh.colors[i] = this.evalResult.color * base.color;
			}
			Vector2 vector2 = this.projectedVerts[0];
			Vector2 vector3 = this.projectedVerts[0];
			for (int j = 1; j < this.projectedVerts.Length; j++)
			{
				if (vector2.x < this.projectedVerts[j].x)
				{
					vector2.x = this.projectedVerts[j].x;
				}
				if (vector2.y < this.projectedVerts[j].y)
				{
					vector2.y = this.projectedVerts[j].y;
				}
				if (vector3.x > this.projectedVerts[j].x)
				{
					vector3.x = this.projectedVerts[j].x;
				}
				if (vector3.y > this.projectedVerts[j].y)
				{
					vector3.y = this.projectedVerts[j].y;
				}
			}
			for (int k = 0; k < this.projectedVerts.Length; k++)
			{
				this.tsMesh.uv[k].x = Mathf.InverseLerp(vector3.x, vector2.x, this.projectedVerts[k].x) * base.uvScale.x - base.uvScale.x * 0.5f + base.uvOffset.x + 0.5f;
				this.tsMesh.uv[k].y = Mathf.InverseLerp(vector2.y, vector3.y, this.projectedVerts[k].y) * base.uvScale.y - base.uvScale.y * 0.5f + base.uvOffset.y + 0.5f;
			}
			if (flag4)
			{
				for (int l = 0; l < num; l++)
				{
					this.tsMesh.normals[l] *= -1f;
				}
			}
			if (this._expand != 0f)
			{
				for (int m = 0; m < num; m++)
				{
					base.GetSample(m, this.evalResult);
					this.tsMesh.vertices[m] += (flag3 ? (-this.evalResult.right) : this.evalResult.right) * this._expand;
				}
			}
			if (flag)
			{
				this.GetIdentityVerts(center, vector, flag3);
				for (int n = 0; n < num; n++)
				{
					this.tsMesh.vertices[n + num] = this.extrudeResults[0].position + this.extrudeResults[0].rotation * this.identityVertices[n] + b;
					this.tsMesh.normals[n + num] = -this.extrudeResults[0].forward;
					this.tsMesh.colors[n + num] = this.tsMesh.colors[n] * this.extrudeResults[0].color;
					this.tsMesh.uv[n + num] = new Vector2(1f - this.tsMesh.uv[n].x, this.tsMesh.uv[n].y);
					this.tsMesh.vertices[n] = this.extrudeResults[this.extrudeResults.Length - 1].position + this.extrudeResults[this.extrudeResults.Length - 1].rotation * this.identityVertices[n] + b;
					this.tsMesh.normals[n] = this.extrudeResults[this.extrudeResults.Length - 1].forward;
					this.tsMesh.colors[n] *= this.extrudeResults[this.extrudeResults.Length - 1].color;
				}
				float num4 = 0f;
				for (int num5 = 0; num5 < this.extrudeResults.Length; num5++)
				{
					if (this._uniformUvs && num5 > 0)
					{
						num4 += Vector3.Distance(this.extrudeResults[num5].position, this.extrudeResults[num5 - 1].position);
					}
					int num6 = num * 2 + num5 * base.sampleCount;
					for (int num7 = 0; num7 < this.identityVertices.Length; num7++)
					{
						this.tsMesh.vertices[num6 + num7] = this.extrudeResults[num5].position + this.extrudeResults[num5].rotation * this.identityVertices[num7] + b;
						this.tsMesh.normals[num6 + num7] = this.extrudeResults[num5].rotation * this.identityNormals[num7];
						if (this._uniformUvs)
						{
							this.tsMesh.uv[num6 + num7] = new Vector2((float)num7 / (float)(this.identityVertices.Length - 1) * this._sideUvScale.x + this._sideUvOffset.x, num4 * this._sideUvScale.y + this._sideUvOffset.y);
						}
						else
						{
							this.tsMesh.uv[num6 + num7] = new Vector2((float)num7 / (float)(this.identityVertices.Length - 1) * this._sideUvScale.x + this._sideUvOffset.x, (float)num5 / (float)(this.extrudeResults.Length - 1) * this._sideUvScale.y + this._sideUvOffset.y);
						}
						if (flag3)
						{
							this.tsMesh.uv[num6 + num7].x = 1f - this.tsMesh.uv[num6 + num7].x;
						}
					}
				}
				int trisOffset = this.WriteTris(ref this.surfaceTris, ref this.tsMesh.triangles, 0, 0, false);
				trisOffset = this.WriteTris(ref this.surfaceTris, ref this.tsMesh.triangles, num, trisOffset, true);
				MeshUtility.GeneratePlaneTriangles(ref this.wallTris, base.sampleCount - 1, this.extrudeResults.Length, flag5, 0, 0, true);
				this.WriteTris(ref this.wallTris, ref this.tsMesh.triangles, num * 2, trisOffset, false);
				return;
			}
			if (flag2)
			{
				for (int num8 = 0; num8 < num; num8++)
				{
					this.tsMesh.vertices[num8 + num] = this.tsMesh.vertices[num8];
					this.tsMesh.normals[num8 + num] = -this.tsMesh.normals[num8];
					this.tsMesh.colors[num8 + num] = this.tsMesh.colors[num8];
					this.tsMesh.uv[num8 + num] = new Vector2(1f - this.tsMesh.uv[num8].x, this.tsMesh.uv[num8].y);
					this.tsMesh.vertices[num8] += vector * this._extrude;
				}
				for (int num9 = 0; num9 < num + 1; num9++)
				{
					int num10 = num9;
					if (num9 >= num)
					{
						num10 = num9 - num;
					}
					base.GetSample(num10, this.evalResult);
					this.tsMesh.vertices[num9 + num * 2] = this.tsMesh.vertices[num10] - vector * this._extrude;
					this.tsMesh.normals[num9 + num * 2] = (flag3 ? (-this.evalResult.right) : this.evalResult.right);
					this.tsMesh.colors[num9 + num * 2] = this.tsMesh.colors[num10];
					this.tsMesh.uv[num9 + num * 2] = new Vector2((float)num9 / (float)(num - 1) * this._sideUvScale.x + this._sideUvOffset.x, 0f + this._sideUvOffset.y);
					if (flag3)
					{
						this.tsMesh.uv[num9 + num * 2].x = 1f - this.tsMesh.uv[num9 + num * 2].x;
					}
					int num11 = num9 + num * 3 + 1;
					this.tsMesh.vertices[num11] = this.tsMesh.vertices[num10];
					this.tsMesh.normals[num11] = this.tsMesh.normals[num9 + num * 2];
					this.tsMesh.colors[num11] = this.tsMesh.colors[num10];
					if (this._uniformUvs)
					{
						this.tsMesh.uv[num11] = new Vector2((float)num9 / (float)num * this._sideUvScale.x + this._sideUvOffset.x, this._extrude * this._sideUvScale.y + this._sideUvOffset.y);
					}
					else
					{
						this.tsMesh.uv[num11] = new Vector2((float)num9 / (float)num * this._sideUvScale.x + this._sideUvOffset.x, 1f * this._sideUvScale.y + this._sideUvOffset.y);
					}
					if (flag3)
					{
						this.tsMesh.uv[num11].x = 1f - this.tsMesh.uv[num11].x;
					}
				}
				int trisOffset2 = this.WriteTris(ref this.surfaceTris, ref this.tsMesh.triangles, 0, 0, false);
				trisOffset2 = this.WriteTris(ref this.surfaceTris, ref this.tsMesh.triangles, num, trisOffset2, true);
				MeshUtility.GeneratePlaneTriangles(ref this.wallTris, base.sampleCount - 1, 2, flag5, 0, 0, true);
				this.WriteTris(ref this.wallTris, ref this.tsMesh.triangles, num * 2, trisOffset2, false);
				return;
			}
			this.WriteTris(ref this.surfaceTris, ref this.tsMesh.triangles, 0, 0, false);
		}

		// Token: 0x06004103 RID: 16643 RVA: 0x001EDB6E File Offset: 0x001EBD6E
		private void GenerateSurfaceTris(bool flip)
		{
			MeshUtility.Triangulate(this.projectedVerts, ref this.surfaceTris);
			if (flip)
			{
				MeshUtility.FlipTriangles(ref this.surfaceTris);
			}
		}

		// Token: 0x06004104 RID: 16644 RVA: 0x001EDB90 File Offset: 0x001EBD90
		private int WriteTris(ref int[] tris, ref int[] target, int vertexOffset, int trisOffset, bool flip)
		{
			for (int i = trisOffset; i < trisOffset + tris.Length; i += 3)
			{
				if (flip)
				{
					target[i] = tris[i + 2 - trisOffset] + vertexOffset;
					target[i + 1] = tris[i + 1 - trisOffset] + vertexOffset;
					target[i + 2] = tris[i - trisOffset] + vertexOffset;
				}
				else
				{
					target[i] = tris[i - trisOffset] + vertexOffset;
					target[i + 1] = tris[i + 1 - trisOffset] + vertexOffset;
					target[i + 2] = tris[i + 2 - trisOffset] + vertexOffset;
				}
			}
			return trisOffset + tris.Length;
		}

		// Token: 0x06004105 RID: 16645 RVA: 0x001EDC1C File Offset: 0x001EBE1C
		private bool IsClockwise(Vector2[] points2D)
		{
			float num = 0f;
			for (int i = 1; i < points2D.Length; i++)
			{
				Vector2 vector = points2D[i];
				Vector2 vector2 = points2D[(i + 1) % points2D.Length];
				num += (vector2.x - vector.x) * (vector2.y + vector.y);
			}
			num += (points2D[0].x - points2D[points2D.Length - 1].x) * (points2D[0].y + points2D[points2D.Length - 1].y);
			return num <= 0f;
		}

		// Token: 0x06004106 RID: 16646 RVA: 0x001EDCBC File Offset: 0x001EBEBC
		private void GetIdentityVerts(Vector3 center, Vector3 normal, bool clockwise)
		{
			Quaternion rotation = Quaternion.Inverse(Quaternion.LookRotation(normal));
			if (this.identityVertices.Length != base.sampleCount)
			{
				this.identityVertices = new Vector3[base.sampleCount];
				this.identityNormals = new Vector3[base.sampleCount];
			}
			for (int i = 0; i < base.sampleCount; i++)
			{
				this.identityVertices[i] = rotation * (base.GetSampleRaw(i).position - center + (clockwise ? (-base.GetSampleRaw(i).right) : base.GetSampleRaw(i).right) * this._expand);
				this.identityNormals[i] = rotation * (clockwise ? (-base.GetSampleRaw(i).right) : base.GetSampleRaw(i).right);
			}
		}

		// Token: 0x06004107 RID: 16647 RVA: 0x001EDDA8 File Offset: 0x001EBFA8
		private void GetProjectedVertices(int count, out Vector3 center, out Vector3 normal)
		{
			center = Vector3.zero;
			normal = Vector3.zero;
			Vector3 b = this.trs.right * base.offset.x + this.trs.up * base.offset.y + this.trs.forward * base.offset.z;
			for (int i = 0; i < count; i++)
			{
				center += base.GetSampleRaw(i).position + b;
				normal += base.GetSampleRaw(i).up;
			}
			normal.Normalize();
			center /= (float)count;
			Quaternion rotation = Quaternion.LookRotation(normal, Vector3.up);
			Vector3 vector = rotation * Vector3.up;
			Vector3 vector2 = rotation * Vector3.right;
			if (this.projectedVerts.Length != count)
			{
				this.projectedVerts = new Vector2[count];
			}
			for (int j = 0; j < count; j++)
			{
				Vector3 vector3 = base.GetSampleRaw(j).position + b - center;
				float num = Vector3.Project(vector3, vector2).magnitude;
				if (Vector3.Dot(vector3, vector2) < 0f)
				{
					num *= -1f;
				}
				float num2 = Vector3.Project(vector3, vector).magnitude;
				if (Vector3.Dot(vector3, vector) < 0f)
				{
					num2 *= -1f;
				}
				this.projectedVerts[j].x = num;
				this.projectedVerts[j].y = num2;
			}
		}

		// Token: 0x04002D65 RID: 11621
		[SerializeField]
		[HideInInspector]
		private float _expand;

		// Token: 0x04002D66 RID: 11622
		[SerializeField]
		[HideInInspector]
		private float _extrude;

		// Token: 0x04002D67 RID: 11623
		[SerializeField]
		[HideInInspector]
		private Vector2 _sideUvScale = Vector2.one;

		// Token: 0x04002D68 RID: 11624
		[SerializeField]
		[HideInInspector]
		private Vector2 _sideUvOffset = Vector2.zero;

		// Token: 0x04002D69 RID: 11625
		[SerializeField]
		[HideInInspector]
		private SplineComputer _extrudeSpline;

		// Token: 0x04002D6A RID: 11626
		[SerializeField]
		[HideInInspector]
		private SplineSample[] extrudeResults = new SplineSample[0];

		// Token: 0x04002D6B RID: 11627
		[SerializeField]
		[HideInInspector]
		private Vector3[] identityVertices = new Vector3[0];

		// Token: 0x04002D6C RID: 11628
		[SerializeField]
		[HideInInspector]
		private Vector3[] identityNormals = new Vector3[0];

		// Token: 0x04002D6D RID: 11629
		[SerializeField]
		[HideInInspector]
		private Vector2[] projectedVerts = new Vector2[0];

		// Token: 0x04002D6E RID: 11630
		[SerializeField]
		[HideInInspector]
		private int[] surfaceTris = new int[0];

		// Token: 0x04002D6F RID: 11631
		[SerializeField]
		[HideInInspector]
		private int[] wallTris = new int[0];

		// Token: 0x04002D70 RID: 11632
		[SerializeField]
		[HideInInspector]
		private double _extrudeFrom;

		// Token: 0x04002D71 RID: 11633
		[SerializeField]
		[HideInInspector]
		private double _extrudeTo = 1.0;

		// Token: 0x04002D72 RID: 11634
		[SerializeField]
		[HideInInspector]
		private bool _uniformUvs;
	}
}

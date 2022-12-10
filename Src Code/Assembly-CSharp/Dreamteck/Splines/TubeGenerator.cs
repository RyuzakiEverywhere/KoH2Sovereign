using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004C9 RID: 1225
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[AddComponentMenu("Dreamteck/Splines/Users/Tube Generator")]
	public class TubeGenerator : MeshGenerator
	{
		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x06004109 RID: 16649 RVA: 0x001EDFFC File Offset: 0x001EC1FC
		// (set) Token: 0x0600410A RID: 16650 RVA: 0x001EE004 File Offset: 0x001EC204
		public int sides
		{
			get
			{
				return this._sides;
			}
			set
			{
				if (value != this._sides)
				{
					if (value < 3)
					{
						value = 3;
					}
					this._sides = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x0600410B RID: 16651 RVA: 0x001EE023 File Offset: 0x001EC223
		// (set) Token: 0x0600410C RID: 16652 RVA: 0x001EE02B File Offset: 0x001EC22B
		public TubeGenerator.CapMethod capMode
		{
			get
			{
				return this._capMode;
			}
			set
			{
				if (value != this._capMode)
				{
					this._capMode = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x0600410D RID: 16653 RVA: 0x001EE043 File Offset: 0x001EC243
		// (set) Token: 0x0600410E RID: 16654 RVA: 0x001EE04B File Offset: 0x001EC24B
		public int roundCapLatitude
		{
			get
			{
				return this._roundCapLatitude;
			}
			set
			{
				if (value < 1)
				{
					value = 1;
				}
				if (value != this._roundCapLatitude)
				{
					this._roundCapLatitude = value;
					if (this._capMode == TubeGenerator.CapMethod.Round)
					{
						this.Rebuild();
					}
				}
			}
		}

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x0600410F RID: 16655 RVA: 0x001EE073 File Offset: 0x001EC273
		// (set) Token: 0x06004110 RID: 16656 RVA: 0x001EE07B File Offset: 0x001EC27B
		public float revolve
		{
			get
			{
				return this._revolve;
			}
			set
			{
				if (value != this._revolve)
				{
					this._revolve = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x06004111 RID: 16657 RVA: 0x001EE093 File Offset: 0x001EC293
		// (set) Token: 0x06004112 RID: 16658 RVA: 0x001EE09B File Offset: 0x001EC29B
		public float capUVScale
		{
			get
			{
				return this._capUVScale;
			}
			set
			{
				if (value != this._capUVScale)
				{
					this._capUVScale = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x06004113 RID: 16659 RVA: 0x001EE0B4 File Offset: 0x001EC2B4
		private bool useCap
		{
			get
			{
				bool flag = this._capMode > TubeGenerator.CapMethod.None;
				if (base.spline != null)
				{
					return flag && (!base.spline.isClosed || base.span < 1.0);
				}
				return flag;
			}
		}

		// Token: 0x06004114 RID: 16660 RVA: 0x001E5251 File Offset: 0x001E3451
		protected override void Reset()
		{
			base.Reset();
		}

		// Token: 0x06004115 RID: 16661 RVA: 0x001EE100 File Offset: 0x001EC300
		protected override void Awake()
		{
			base.Awake();
			this.mesh.name = "tube";
		}

		// Token: 0x06004116 RID: 16662 RVA: 0x001EE118 File Offset: 0x001EC318
		protected override void BuildMesh()
		{
			if (this._sides <= 2)
			{
				return;
			}
			base.BuildMesh();
			this.bodyVertexCount = (this._sides + 1) * base.sampleCount;
			TubeGenerator.CapMethod capMethod = this._capMode;
			if (!this.useCap)
			{
				capMethod = TubeGenerator.CapMethod.None;
			}
			if (capMethod != TubeGenerator.CapMethod.Flat)
			{
				if (capMethod != TubeGenerator.CapMethod.Round)
				{
					this.capVertexCount = 0;
				}
				else
				{
					this.capVertexCount = this._roundCapLatitude * (this.sides + 1);
				}
			}
			else
			{
				this.capVertexCount = this._sides + 1;
			}
			int vertexCount = this.bodyVertexCount + this.capVertexCount * 2;
			this.bodyTrisCount = this._sides * (base.sampleCount - 1) * 2 * 3;
			if (capMethod != TubeGenerator.CapMethod.Flat)
			{
				if (capMethod != TubeGenerator.CapMethod.Round)
				{
					this.capTrisCount = 0;
				}
				else
				{
					this.capTrisCount = this._sides * this._roundCapLatitude * 6;
				}
			}
			else
			{
				this.capTrisCount = (this._sides - 1) * 3 * 2;
			}
			this.AllocateMesh(vertexCount, this.bodyTrisCount + this.capTrisCount * 2);
			this.Generate();
			if (capMethod == TubeGenerator.CapMethod.Flat)
			{
				this.GenerateFlatCaps();
				return;
			}
			if (capMethod != TubeGenerator.CapMethod.Round)
			{
				return;
			}
			this.GenerateRoundCaps();
		}

		// Token: 0x06004117 RID: 16663 RVA: 0x001EE22C File Offset: 0x001EC42C
		private void Generate()
		{
			int num = 0;
			base.ResetUVDistance();
			bool flag = base.offset != Vector3.zero;
			for (int i = 0; i < base.sampleCount; i++)
			{
				base.GetSample(i, this.evalResult);
				Vector3 vector = this.evalResult.position;
				Vector3 right = this.evalResult.right;
				if (flag)
				{
					vector += base.offset.x * right + base.offset.y * this.evalResult.up + base.offset.z * this.evalResult.forward;
				}
				if (base.uvMode == MeshGenerator.UVMode.UniformClamp || base.uvMode == MeshGenerator.UVMode.UniformClip)
				{
					base.AddUVDistance(i);
				}
				Color color = this.evalResult.color * base.color;
				for (int j = 0; j < this._sides + 1; j++)
				{
					float num2 = (float)j / (float)this._sides;
					Quaternion rotation = Quaternion.AngleAxis(this._revolve * num2 + base.rotation + 180f, this.evalResult.forward);
					this.tsMesh.vertices[num] = vector + rotation * right * base.size * this.evalResult.size * 0.5f;
					base.CalculateUVs(this.evalResult.percent, num2);
					this.tsMesh.uv[num] = Vector2.one * 0.5f + Quaternion.AngleAxis(base.uvRotation, Vector3.forward) * (Vector2.one * 0.5f - MeshGenerator.uvs);
					this.tsMesh.normals[num] = Vector3.Normalize(this.tsMesh.vertices[num] - vector);
					this.tsMesh.colors[num] = color;
					num++;
				}
			}
			MeshUtility.GeneratePlaneTriangles(ref this.tsMesh.triangles, this._sides, base.sampleCount, false, 0, 0, false);
		}

		// Token: 0x06004118 RID: 16664 RVA: 0x001EE488 File Offset: 0x001EC688
		private void GenerateFlatCaps()
		{
			base.GetSample(0, this.evalResult);
			for (int i = 0; i < this._sides + 1; i++)
			{
				int num = this.bodyVertexCount + i;
				this.tsMesh.vertices[num] = this.tsMesh.vertices[i];
				this.tsMesh.normals[num] = -this.evalResult.forward;
				this.tsMesh.colors[num] = this.tsMesh.colors[i];
				this.tsMesh.uv[num] = Quaternion.AngleAxis(this._revolve * ((float)i / (float)(this._sides - 1)), Vector3.forward) * Vector2.right * 0.5f * this.capUVScale + Vector3.right * 0.5f + Vector3.up * 0.5f;
			}
			base.GetSample(base.sampleCount - 1, this.evalResult);
			for (int j = 0; j < this._sides + 1; j++)
			{
				int num2 = this.bodyVertexCount + (this._sides + 1) + j;
				int num3 = this.bodyVertexCount - (this._sides + 1) + j;
				this.tsMesh.vertices[num2] = this.tsMesh.vertices[num3];
				this.tsMesh.normals[num2] = base.GetSampleRaw(base.sampleCount - 1).forward;
				this.tsMesh.colors[num2] = this.tsMesh.colors[num3];
				this.tsMesh.uv[num2] = Quaternion.AngleAxis(this._revolve * ((float)num3 / (float)(this._sides - 1)), Vector3.forward) * Vector2.right * 0.5f * this.capUVScale + Vector3.right * 0.5f + Vector3.up * 0.5f;
			}
			int num4 = this.bodyTrisCount;
			int num5 = (this._revolve == 360f) ? (this._sides - 1) : this._sides;
			for (int k = 0; k < num5 - 1; k++)
			{
				this.tsMesh.triangles[num4++] = k + this.bodyVertexCount + 2;
				this.tsMesh.triangles[num4++] = k + this.bodyVertexCount + 1;
				this.tsMesh.triangles[num4++] = this.bodyVertexCount;
			}
			for (int l = 0; l < num5 - 1; l++)
			{
				this.tsMesh.triangles[num4++] = this.bodyVertexCount + (this._sides + 1);
				this.tsMesh.triangles[num4++] = l + 1 + this.bodyVertexCount + (this._sides + 1);
				this.tsMesh.triangles[num4++] = l + 2 + this.bodyVertexCount + (this._sides + 1);
			}
		}

		// Token: 0x06004119 RID: 16665 RVA: 0x001EE7F4 File Offset: 0x001EC9F4
		private void GenerateRoundCaps()
		{
			base.GetSample(0, this.evalResult);
			Vector3 vector = this.evalResult.position;
			bool flag = base.offset != Vector3.zero;
			if (flag)
			{
				vector += base.offset.x * this.evalResult.right + base.offset.y * this.evalResult.up + base.offset.z * this.evalResult.forward;
			}
			Quaternion lhs = Quaternion.LookRotation(-this.evalResult.forward, this.evalResult.up);
			float num = 0f;
			float num2 = 0f;
			switch (base.uvMode)
			{
			case MeshGenerator.UVMode.Clip:
				num = (float)this.evalResult.percent;
				num2 = base.size * 0.5f / base.spline.CalculateLength(0.0, 1.0);
				break;
			case MeshGenerator.UVMode.UniformClip:
				num = base.spline.CalculateLength(0.0, this.evalResult.percent);
				num2 = base.size * 0.5f;
				break;
			case MeshGenerator.UVMode.Clamp:
				num2 = base.size * 0.5f / base.spline.CalculateLength(base.clipFrom, base.clipTo);
				break;
			case MeshGenerator.UVMode.UniformClamp:
				num = 0f;
				num2 = base.size * 0.5f / (float)base.span;
				break;
			}
			Color color = this.evalResult.color * base.color;
			for (int i = 1; i < this._roundCapLatitude + 1; i++)
			{
				float num3 = (float)i / (float)this._roundCapLatitude;
				float angle = 90f * num3;
				for (int j = 0; j <= this.sides; j++)
				{
					float num4 = (float)j / (float)this.sides;
					int num5 = this.bodyVertexCount + j + (i - 1) * (this.sides + 1);
					Quaternion rhs = Quaternion.AngleAxis(this._revolve * num4 + base.rotation + 180f, -Vector3.forward) * Quaternion.AngleAxis(angle, Vector3.up);
					this.tsMesh.vertices[num5] = vector + lhs * rhs * -Vector3.right * base.size * 0.5f * this.evalResult.size;
					this.tsMesh.colors[num5] = color;
					this.tsMesh.normals[num5] = (this.tsMesh.vertices[num5] - vector).normalized;
					this.tsMesh.uv[num5] = new Vector2(num4 * base.uvScale.x, (num - num2 * num3) * base.uvScale.y) - base.uvOffset;
				}
			}
			int num6 = this.bodyTrisCount;
			for (int k = -1; k < this._roundCapLatitude - 1; k++)
			{
				for (int l = 0; l < this.sides; l++)
				{
					int num7 = this.bodyVertexCount + l + k * (this.sides + 1);
					int num8 = num7 + (this.sides + 1);
					if (k == -1)
					{
						num7 = l;
						num8 = this.bodyVertexCount + l;
					}
					this.tsMesh.triangles[num6++] = num8 + 1;
					this.tsMesh.triangles[num6++] = num7 + 1;
					this.tsMesh.triangles[num6++] = num7;
					this.tsMesh.triangles[num6++] = num8;
					this.tsMesh.triangles[num6++] = num8 + 1;
					this.tsMesh.triangles[num6++] = num7;
				}
			}
			base.GetSample(base.sampleCount - 1, this.evalResult);
			vector = this.evalResult.position;
			if (flag)
			{
				vector += base.offset.x * this.evalResult.right + base.offset.y * this.evalResult.up + base.offset.z * this.evalResult.forward;
			}
			lhs = Quaternion.LookRotation(this.evalResult.forward, this.evalResult.up);
			switch (base.uvMode)
			{
			case MeshGenerator.UVMode.Clip:
				num = (float)this.evalResult.percent;
				break;
			case MeshGenerator.UVMode.UniformClip:
				num = base.spline.CalculateLength(0.0, this.evalResult.percent);
				break;
			case MeshGenerator.UVMode.Clamp:
				num = 1f;
				break;
			case MeshGenerator.UVMode.UniformClamp:
				num = base.spline.CalculateLength(0.0, 1.0);
				break;
			}
			color = this.evalResult.color * base.color;
			for (int m = 1; m < this._roundCapLatitude + 1; m++)
			{
				float num9 = (float)m / (float)this._roundCapLatitude;
				float angle2 = 90f * num9;
				for (int n = 0; n <= this.sides; n++)
				{
					float num10 = (float)n / (float)this.sides;
					int num11 = this.bodyVertexCount + this.capVertexCount + n + (m - 1) * (this.sides + 1);
					Quaternion rhs2 = Quaternion.AngleAxis(this._revolve * num10 + base.rotation + 180f, Vector3.forward) * Quaternion.AngleAxis(angle2, -Vector3.up);
					this.tsMesh.vertices[num11] = vector + lhs * rhs2 * Vector3.right * base.size * 0.5f * this.evalResult.size;
					this.tsMesh.normals[num11] = (this.tsMesh.vertices[num11] - vector).normalized;
					this.tsMesh.colors[num11] = color;
					this.tsMesh.uv[num11] = new Vector2(num10 * base.uvScale.x, (num + num2 * num9) * base.uvScale.y) - base.uvOffset;
				}
			}
			for (int num12 = -1; num12 < this._roundCapLatitude - 1; num12++)
			{
				for (int num13 = 0; num13 < this.sides; num13++)
				{
					int num14 = this.bodyVertexCount + this.capVertexCount + num13 + num12 * (this.sides + 1);
					int num15 = num14 + (this.sides + 1);
					if (num12 == -1)
					{
						num14 = this.bodyVertexCount - (this._sides + 1) + num13;
						num15 = this.bodyVertexCount + this.capVertexCount + num13;
					}
					this.tsMesh.triangles[num6++] = num14 + 1;
					this.tsMesh.triangles[num6++] = num15 + 1;
					this.tsMesh.triangles[num6++] = num15;
					this.tsMesh.triangles[num6++] = num15;
					this.tsMesh.triangles[num6++] = num14;
					this.tsMesh.triangles[num6++] = num14 + 1;
				}
			}
		}

		// Token: 0x04002D73 RID: 11635
		[SerializeField]
		[HideInInspector]
		private int _sides = 12;

		// Token: 0x04002D74 RID: 11636
		[SerializeField]
		[HideInInspector]
		private int _roundCapLatitude = 6;

		// Token: 0x04002D75 RID: 11637
		[SerializeField]
		[HideInInspector]
		private TubeGenerator.CapMethod _capMode;

		// Token: 0x04002D76 RID: 11638
		[SerializeField]
		[HideInInspector]
		[Range(0f, 360f)]
		private float _revolve = 360f;

		// Token: 0x04002D77 RID: 11639
		[SerializeField]
		[HideInInspector]
		private float _capUVScale = 1f;

		// Token: 0x04002D78 RID: 11640
		private int bodyVertexCount;

		// Token: 0x04002D79 RID: 11641
		private int bodyTrisCount;

		// Token: 0x04002D7A RID: 11642
		private int capVertexCount;

		// Token: 0x04002D7B RID: 11643
		private int capTrisCount;

		// Token: 0x020009AA RID: 2474
		public enum CapMethod
		{
			// Token: 0x04004508 RID: 17672
			None,
			// Token: 0x04004509 RID: 17673
			Flat,
			// Token: 0x0400450A RID: 17674
			Round
		}
	}
}
